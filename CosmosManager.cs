using System.Runtime.CompilerServices;
using Microsoft.Azure.Cosmos;

namespace AzureCosmosDb;

public class CosmosManager
{
    private CosmosClient _client;
    private Container _container;

    public CosmosManager(string cosmosUri, string cosmosKey, string dbName, string containerName)
    {
        _client = new CosmosClient(cosmosUri, cosmosKey);
        _container = _client.GetContainer(dbName, containerName);
    }

    public async Task InsertItems(List<StoreOrder> items)
    {
        foreach(var item in items)
        {
            var res = await _container.CreateItemAsync<StoreOrder>(item, new PartitionKey(item.GetPartitionKey()));

            Console.WriteLine($"Item with id:{item.ID} inserted for RU:{res.RequestCharge}");
        }
    }

    public async Task UpdateItems(List<StoreOrder> items)
    {
        foreach(var item in items)
        {
            // Check if item is in db
            var target = (await ReadItems(@$"SELECT * FROM StoreOrder o WHERE o.OrderID = ""{item.OrderID}""")).FirstOrDefault();

            // Update if item is found in db
            if (target != null)
            {
                target.Quantity = item.Quantity;
            }

            var res = await _container.ReplaceItemAsync<StoreOrder>(target, target.ID, new PartitionKey(item.GetPartitionKey()));

            Console.WriteLine($"Item with id:{item.ID} updated for RU:{res.RequestCharge}");
        }
    }

    public async Task<List<StoreOrder>> ReadItems(string sql)
    {
        var query = new QueryDefinition(sql);

        var resultSetIterator = _container.GetItemQueryIterator<StoreOrder>(query);

        var result = new List<StoreOrder>();

        while (resultSetIterator.HasMoreResults)
        {
            var response = await resultSetIterator.ReadNextAsync();
            result.AddRange(response);
        }

        foreach(var item in result)
        {
            Console.WriteLine(item);
        }

        return result;
    }
}

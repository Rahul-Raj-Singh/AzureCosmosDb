using Microsoft.Extensions.Configuration;

namespace AzureCosmosDb;

public class Program
{
    public async static Task Main(string[] args)
    {
        var config = GetConfig();

        var cosmosUri = config["CosmosDBUri"];
        var cosmosKey = config["CosmosDBKey"];

        var cosmosManager = new CosmosManager(cosmosUri, cosmosKey, dbName: "DoodleCosmos", containerName: "StoreOrder");

        // INSERT
        await cosmosManager.InsertItems(new List<StoreOrder> 
        {
            new StoreOrder {OrderID = "O1", Category = "Laptop", Quantity = 100},
            new StoreOrder {OrderID = "O2", Category = "Mobile", Quantity = 150},
            new StoreOrder {OrderID = "O3", Category = "Desktop", Quantity = 75}
        });

        // READ
        await cosmosManager.ReadItems("SELECT * FROM StoreOrder o WHERE o.Category = \"Laptop\"");

        // UPDATE
        await cosmosManager.UpdateItems(new List<StoreOrder> 
        {
            new StoreOrder {OrderID = "O2", Category = "Mobile", Quantity = 200},
        });

        Console.ReadKey();


    }

    public static IConfiguration GetConfig()
    {
        var config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

        return config;
    }
}
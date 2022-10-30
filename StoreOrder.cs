using Newtonsoft.Json;

namespace AzureCosmosDb;

public class StoreOrder
{
    [JsonProperty("id")]
    public string ID { get; set; } = Guid.NewGuid().ToString(); // This is required prop for Cosmos Db.
    public string OrderID { get; set; }
    public string Category { get; set; }
    public int Quantity { get; set; }
    public string GetPartitionKey() => OrderID;

    public override string ToString() => JsonConvert.SerializeObject(this);
}

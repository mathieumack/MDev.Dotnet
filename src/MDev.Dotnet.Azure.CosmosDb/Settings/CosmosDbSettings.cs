namespace MDev.Dotnet.Azure.CosmosDb.Settings;

public class CosmosDbSettings
{
    public const string SectionName = "CosmosDb";

    public string Endpoint { get; set; }

    public string DatabaseName { get; set; }
}

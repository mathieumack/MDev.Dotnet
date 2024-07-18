namespace MDev.Dotnet.Azure.StorageAccount.Settings;

public class StorageAccountSettings
{
    public const string SectionName = "StorageAccount";

    public string BlobsEndpoint { get; set; }

    public string QueuesEndpoint { get; set; }

    public List<StorageAccountQueuesSettings> Queues { get; set; }
}

public class StorageAccountQueuesSettings
{
    public string Id { get; set; }

    public List<string> Queues { get; set; }
}
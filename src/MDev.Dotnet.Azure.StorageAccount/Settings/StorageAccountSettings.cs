namespace MDev.Dotnet.Azure.StorageAccount.Settings;

public class StorageAccountSettings
{
    public const string SectionName = "StorageAccount";

    public string BlobsEndpoint { get; set; }

    public string QueuesEndpoint { get; set; }

    public bool QueueMessagesEncodeBase64 { get; set; } = false;

    public List<StorageAccountQueuesSettings> Queues { get; set; }
}
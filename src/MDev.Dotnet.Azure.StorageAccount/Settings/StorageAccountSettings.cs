namespace MDev.Dotnet.Azure.StorageAccount.Settings;

public class StorageAccountSettings
{
    public const string SectionName = "StorageAccount";

    public string BlobsEndpoint { get; set; }

    public string QueuesEndpoint { get; set; }
}

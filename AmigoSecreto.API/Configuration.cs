namespace AmigoSecreto.API
{
    public class Configuration
    {
        public static string GetAmigoFilePath()
            => "wwwroot\\amigo.csv";
        public static string GetAmigoSecretoFilePath()
            => "wwwroot\\amigo-secreto.csv";
        public static string GetFlag()
            => "3608d62b-35b3-483a-ab14-3c3242b8bbe6";
        public static string GetBlobContainerName()
            => "armazenamento-geral";
        public static string GetAmigoFileNameFromAzureBlob()
            => "amigo.csv";
        public static string GetAmigoSecretoFileNameFromBlob()
            => "amigo-secreto.csv";
        public static string GetBlobConnectionString()
            => Environment.GetEnvironmentVariable("BLOB_ACCOUNT_KEY") ?? "";
    }
}
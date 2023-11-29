using System.Text;

using Azure.Storage.Blobs;

using AmigoSecreto.API.Models;
using AmigoSecreto.API.Data.Interfaces;
using System.Globalization;

namespace AmigoSecreto.API.Data;

public class AmigoDAO : IAmigoDAO
{
    #region [ Constructors ]
    private readonly string _blobContainerName = Configuration.GetBlobContainerName();
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _blobContainerClient;
    private const string fileName = "amigos.csv";

    public AmigoDAO()
    {
        _blobServiceClient = new BlobServiceClient(Configuration.GetBlobConnectionString());
        _blobContainerClient = _blobServiceClient.GetBlobContainerClient(Configuration.GetBlobContainerName());
    }

    #endregion [ Constructors ]

    #region [ Call archive To Local ]
    public IEnumerable<Amigo> GetAll()
    {
        var amigos = new List<Amigo>();

        try
        {
            StreamReader sr = new StreamReader(Configuration.GetAmigoFilePath(), Encoding.UTF8);
            string[] linhaSplit;
            string linha;
            do
            {
                linha = sr.ReadLine();
                linhaSplit = linha.Split(";");
                amigos.Add(
                    new Amigo(
                        Guid.Parse(linhaSplit[0]),
                        linhaSplit[1],
                        linhaSplit[2],
                        DateTime.Parse(linhaSplit[3])
                        )
                    );
            } while (!sr.EndOfStream);
            sr.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nErro ao ler aquivo.\n{ex.Message}");
        }
        return amigos;
    }
    public bool Save(Amigo amigo)
    {
        try
        {
            var sw = new StreamWriter(Configuration.GetAmigoFilePath(), append: true);
            sw.WriteLine(amigo.ToCsv());
            sw.Close();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ADSP2 - Erro ao salvar registro.");
            return false;
        }
    }
    private void SaveMany(List<Amigo> amigos)
    {
        try
        {
            var sw = new StreamWriter(Configuration.GetAmigoFilePath(), append: false, Encoding.UTF8);
            foreach (var amigo in amigos)
                sw.WriteLine(amigo.ToCsv());

            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message); ;
        }
    }

    #endregion [ Call archive To Local ]

    #region [ Dependents Methods ]
    public bool Delete(Guid id)
    {
        var amigos = GetAllFromAzureBlobAsync().ToList();

        var amigoSelecionado = amigos.FirstOrDefault(item => item.Id == id);
        if (amigoSelecionado is null)
            return false;

        amigos.Remove(amigoSelecionado);
        SaveManyInAzureBlobAsync(amigos);
        return true;
    }
    public void Update(Amigo amigoIn)
    {
        var amigos = GetAllFromAzureBlobAsync().ToList();

        var amigoSelecionado = amigos.FirstOrDefault(item => item.Id == amigoIn.Id);

        if (amigoSelecionado is null)
            return;

        amigoSelecionado.Update(amigoIn);

        SaveManyInAzureBlobAsync(amigos);
    }
    public Amigo? GetById(string id)
    => GetAllFromAzureBlobAsync()
        .Where(amg => amg.Id?
            .ToString()
            .ToUpper() == id
            .ToUpper())
        .FirstOrDefault();

    #endregion [ Dependents Methods ]

    #region [ Call to Azure ]
    public IEnumerable<Amigo> GetAllFromAzureBlobAsync()
    {
        var amigos = new List<Amigo>();

        try
        {
            var blobClient = _blobContainerClient
                .GetBlobClient(Configuration
                    .GetAmigoFileNameFromAzureBlob());

            using (var memoryStream = new MemoryStream())
            {
                blobClient.DownloadTo(memoryStream);
                memoryStream.Position = 0;

                var response = blobClient.OpenRead();

                using (var sr = new StreamReader(response, Encoding.UTF8))
                {
                    string[] linhaSplit;
                    string linha;

                    do
                    {
                        linha = sr.ReadLine();
                        if (!string.IsNullOrEmpty(linha))
                        {
                            linhaSplit = linha.Split(";");
                            amigos.Add(new Amigo(
                                Guid.Parse(linhaSplit[0]),
                                linhaSplit[1],
                                linhaSplit[2],
                                DateTime.ParseExact(linhaSplit[3], "dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture))
                            );
                        }
                    } while (!sr.EndOfStream);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nErro ao ler arquivo no Azure Blob Storage.\n{ex.Message}");
        }

        return amigos;
    }

    public bool SaveInAzureBlob(Amigo amigo)
    {
        try
        {
            var blobClient = _blobContainerClient
                .GetBlobClient(Configuration.GetAmigoFileNameFromAzureBlob());

            string currentContent = string.Empty;

            using (var memoryStream = new MemoryStream())
            {
                blobClient.DownloadTo(memoryStream);
                memoryStream.Position = 0;

                using (var streamReader = new StreamReader(memoryStream, Encoding.UTF8))
                {
                    currentContent = streamReader.ReadToEnd();
                }
            }

            string newContent = currentContent + Environment.NewLine + amigo.ToCsv();

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(newContent)))
            {
                blobClient.Upload(memoryStream, true);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("ADSP2 - Erro ao salvar registro: " + ex.Message);
            return false;
        }
    }

    public async void SaveManyInAzureBlobAsync(List<Amigo> amigos)
    {
        try
        {
            var blobClient = _blobContainerClient
                .GetBlobClient(Configuration.GetAmigoFileNameFromAzureBlob());

            string newContent = string.Empty;

            foreach (var amigo in amigos)
                newContent = newContent + Environment.NewLine + amigo.ToCsv();

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(newContent)))
            {
                blobClient.Upload(memoryStream, true);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("ADSP2 - Erro ao salvar registro: " + ex.Message);
        }
    }
    #endregion [ Call to Azure ]
}
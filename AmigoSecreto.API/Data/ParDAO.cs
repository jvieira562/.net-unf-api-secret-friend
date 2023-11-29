using System.Text;

using Azure.Storage.Blobs;

using AmigoSecreto.API.Models;
using AmigoSecreto.API.Data.Interfaces;

using System.Globalization;

namespace AmigoSecreto.API.Data;

public class ParDAO : IParDAO
{
    #region [ Constructors ]
    private readonly IAmigoDAO _amigoDao;
    private readonly string _blobContainerName = Configuration.GetBlobContainerName();
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _blobContainerClient;

    public ParDAO(IAmigoDAO amigoDAO)
    {
        _amigoDao = amigoDAO;
        _blobServiceClient = new BlobServiceClient(Configuration.GetBlobConnectionString());
        _blobContainerClient = _blobServiceClient.GetBlobContainerClient(Configuration.GetBlobContainerName());
    }

    #endregion [ Constructors ]

    #region [Call to local]
    public void GerarPares()
    {
        var pares = new List<Par>();
        var amigos = _amigoDao
        .GetAll()
        .OrderBy(a => new Random()
            .Next())
        .ToList();

        for (int i = 0; i < amigos.Count(); i += 2)
        {
            var amigo1 = amigos[i];
            var amigo2 = amigos[i + 1];
            var par = new Par(Guid.NewGuid(), amigo1, amigo2);
            pares.Add(par);
        }
        try
        {
            File.WriteAllText(Configuration.GetAmigoSecretoFilePath(), string.Empty);
            var sw = new StreamWriter(Configuration.GetAmigoSecretoFilePath(), append: false, Encoding.UTF8);
            foreach (var par in pares)
                sw.WriteLine(par.ToCsv());

            sw.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message); ;
        }
    }

    public IEnumerable<Par> GetAll()
    {
        var duplas = new List<Par>();

        try
        {
            var sr = new StreamReader(Configuration.GetAmigoSecretoFilePath(), Encoding.UTF8);

            string[] linhaSplit;
            string linha;
            do
            {
                linha = sr.ReadLine();
                linhaSplit = linha.Split(",");

                var duplaId = Guid.Parse(linhaSplit[0]);

                # region [ Construção Amigo 1]
                var amigo1Split = linhaSplit[1].Split(";");

                var amigo1 = new Amigo(
                    Guid.Parse(amigo1Split[0]),
                     amigo1Split[1],
                      amigo1Split[2],
                       DateTime.Parse(amigo1Split[2]));
                #endregion [Call to local]
                #region [ Construção Amigo 2]
                var amigo2Split = linhaSplit[1].Split(";");
                var amigo2 = new Amigo(
                    Guid.Parse(amigo2Split[0]),
                     amigo2Split[1],
                      amigo2Split[2],
                       DateTime.Parse(amigo2Split[2]));
                #endregion [ Construção Amigo 2]

                duplas.Add(new Par(duplaId, amigo1, amigo2));
            } while (!sr.EndOfStream);
            sr.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nErro ao ler aquivo.\n{ex.Message}");
        }
        return duplas;
    }

    public Par GetById(Guid id)
        => GetAll()
        .FirstOrDefault(par => par.Id == id);
    #endregion

    #region [Call to Azure Blob]

    public bool GerarParesFromAzureBlob()
    {
        bool status = false;
        var pares = new List<Par>();

        var blobClient = _blobContainerClient
            .GetBlobClient(Configuration
            .GetAmigoSecretoFileNameFromBlob());

        var amigos = _amigoDao
            .GetAllFromAzureBlobAsync()
            .OrderBy(a => new Random()
                .Next())
            .ToList();

        if (amigos.Count <= 0)
            return status;

        if ((amigos.Count % 2) != 0)
            return status;

        for (int i = 0; i < amigos.Count(); i += 2)
        {
            var amigo1 = amigos[i];
            var amigo2 = amigos[i + 1];
            var par = new Par(Guid.NewGuid(), amigo1, amigo2);
            pares.Add(par);
        }
        try
        {
            var newContent = string.Empty;

            foreach (var par in pares)
                newContent = newContent + Environment.NewLine + par.ToCsv();

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(newContent)))
                blobClient.Upload(memoryStream, true);

            status = true;
            return status;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return status;
        }
    }

    public IEnumerable<Par> GetAllFromAzureBlob()
    {
        var duplas = new List<Par>();
        try
        {
            var blobClient = _blobContainerClient
                .GetBlobClient(Configuration
                    .GetAmigoSecretoFileNameFromBlob());

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
                        if (string.IsNullOrEmpty(linha))
                            continue;

                        linhaSplit = linha.Split(",");

                        var duplaId = Guid.Parse(linhaSplit[0]);

                        #region [ Construção Amigo 1]
                        var amigo1Split = linhaSplit[1].Split(";");

                        var amigo1 = new Amigo(
                            Guid.Parse(amigo1Split[0]),
                            amigo1Split[1],
                            amigo1Split[2],
                            DateTime.ParseExact(amigo1Split[3], "dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture)
                        );
                        #endregion

                        #region [ Construção Amigo 2]
                        var amigo2Split = linhaSplit[2].Split(";");
                        DateTime data;
                        var amigo2 = new Amigo(
                            Guid.Parse(amigo2Split[0]),
                            amigo2Split[1],
                            amigo2Split[2],
                            DateTime.ParseExact(amigo2Split[3], "dd/MM/yyyy HH:mm:ss", CultureInfo.CurrentCulture)
                        );
                        #endregion

                        duplas.Add(new Par(duplaId, amigo1, amigo2));
                    } while (!sr.EndOfStream);
                    sr.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nErro ao ler arquivo no Azure Blob Storage.\n{ex.Message}");
        }

        return duplas;
    }

    public Par GetByIdFromAzureBlob(Guid id)
        => GetAllFromAzureBlob()
        .FirstOrDefault(par => par.Id == id);

    #endregion
}
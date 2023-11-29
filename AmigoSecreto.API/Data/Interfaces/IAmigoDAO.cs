using AmigoSecreto.API.Models;

namespace AmigoSecreto.API.Data.Interfaces;

public interface IAmigoDAO
{
    public IEnumerable<Amigo> GetAll();
    public IEnumerable<Amigo> GetAllFromAzureBlobAsync();
    public Amigo GetById(string id);
    public bool Save(Amigo amigo);
    public bool SaveInAzureBlob(Amigo amigo);
    public bool Delete(Guid id);
    public void Update(Amigo amigoIn);
}
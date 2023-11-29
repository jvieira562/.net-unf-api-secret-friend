using AmigoSecreto.API.Data.Interfaces;
using AmigoSecreto.API.Models;
using AmigoSecreto.API.Services.Interfaces;

namespace AmigoSecreto.API.Services;

public class AmigoService : IAmigoService
{
    private readonly IAmigoDAO _dao;
    public AmigoService(IAmigoDAO dao)
        => _dao = dao;

    public bool Delete(Guid id)
        => _dao.Delete(id);

    public IEnumerable<Amigo> GetAll()
        => _dao.GetAllFromAzureBlobAsync();

    public Amigo GetById(string id)
        => _dao.GetById(id);

    public bool Save(Amigo amigo)
        => _dao.SaveInAzureBlob(amigo);

    public bool Update(Amigo amigo)
    {
        if (!amigo.IsValid())
            return false;

        _dao.Update(amigo);
        return true;
    }
}
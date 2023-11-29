using AmigoSecreto.API.Models;
using AmigoSecreto.API.Data.Interfaces;
using AmigoSecreto.API.Services.Interfaces;

namespace AmigoSecreto.API.Services;

public class ParService : IParService
{
    #region [ Constructors ]
    private readonly IParDAO _dao;
    private readonly IAmigoService _amigoService;

    public ParService(IParDAO dao, IAmigoService amigoService)
    {
        _dao = dao;
        _amigoService = amigoService;
    }
    #endregion [ Constructors ]

    public bool GerarPares()
    {
        var paresCount = _amigoService.GetAll().Count();

        if ((paresCount % 2) != 0)
            return false;

        if (paresCount < 4)
            return false;

        return _dao.GerarParesFromAzureBlob();
    }

    public IEnumerable<Par> GetAll()
        => _dao.GetAllFromAzureBlob();

    public Par GetById(Guid id)
        => _dao.GetByIdFromAzureBlob(id);
}
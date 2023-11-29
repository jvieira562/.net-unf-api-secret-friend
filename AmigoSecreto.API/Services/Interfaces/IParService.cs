using AmigoSecreto.API.Models;

namespace AmigoSecreto.API.Services.Interfaces;

public interface IParService
{
    public IEnumerable<Par> GetAll();
    public bool GerarPares();
    public Par GetById(Guid id);
}
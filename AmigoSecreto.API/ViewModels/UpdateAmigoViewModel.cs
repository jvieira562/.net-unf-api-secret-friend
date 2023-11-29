using AmigoSecreto.API.Models;

namespace AmigoSecreto.API.ViewModels;

public class UpdateAmigoViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime RegistradoEm { get; set; }

    public UpdateAmigoViewModel(Guid id, string name, string email, DateTime registradoEm)
    {
        Id = id;
        Name = name;
        Email = email;
        RegistradoEm = registradoEm;
    }
    public Amigo ToEntity()
        => new Amigo(Id, Name, Email, RegistradoEm);
}
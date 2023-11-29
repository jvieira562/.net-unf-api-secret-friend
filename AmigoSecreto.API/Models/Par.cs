using AmigoSecreto.API.ViewModels;

namespace AmigoSecreto.API.Models;

public class Par
{
    public Guid Id { get; set; }
    public Amigo[] Dupla { get; set; } = new Amigo[2];

    public Par(Amigo[] array)
    {
        Id = Guid.NewGuid();
        Dupla = array;
    }
    public Par(Guid id, Amigo amg1, Amigo amg2)
    {
        Id = id;
        Dupla[0] = amg1;
        Dupla[1] = amg2;
    }
    public string ToCsv()
        => $"{Id},{Dupla[0].ToCsv()},{Dupla[1].ToCsv()},";
}
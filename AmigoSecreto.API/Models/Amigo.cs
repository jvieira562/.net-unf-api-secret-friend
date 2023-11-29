namespace AmigoSecreto.API.Models
{
    public class Amigo
    {
        public Guid? Id { get; private set; }
        public string? Name { get; private set; }
        public string? Email { get; private set; }
        public DateTime? RegistradoEm { get; private set; }

        public Amigo(string name, string email)
        {
            Id = Guid.NewGuid();
            RegistradoEm = DateTime.UtcNow;
            Name = name.Replace(";", "");
            Email = email.Replace(";", "");
        }

        public Amigo(Guid id, string name, string email, DateTime registradoEm)
        {
            Id = id;
            Name = name.Replace(";", "");
            Email = email.Replace(";", "");
            RegistradoEm = registradoEm;
        }

        public void Update(Amigo amigo)
        {
            Id = amigo.Id;
            Name = amigo.Name?.Replace(";", "");
            Email = amigo.Email?.Replace(";", "");
            RegistradoEm = amigo.RegistradoEm;
        }

        public string ToCsv()
            => $"{Id};{Name};{Email};{RegistradoEm};";

        public bool IsValid()
            => (Name is not null) && (Email is not null);

        public string FormatarRegistradoEm()
            => RegistradoEm?.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
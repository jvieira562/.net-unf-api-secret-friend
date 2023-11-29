namespace AmigoSecreto.Desktop.Models
{
    public class Amigo
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime? RegistradoEm { get; set; }

        public string ToCsv()
            => $"{Id};{Name};{Email};{RegistradoEm};";

        public string ToString()
            => $"{Id}\t{Name}   {Email}\t{RegistradoEm}";
    }
}
namespace AmigoSecreto.Web.Models
{
    public class Amigo
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public DateTime? RegistradoEm { get; set; }
    }
}
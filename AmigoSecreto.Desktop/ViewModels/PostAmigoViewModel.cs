namespace AmigoSecreto.Desktop.ViewModels
{
    public record PostAmigoViewModel
    {
        public string Name { get; private set; }
        public string Email { get; private set; }

        public PostAmigoViewModel(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }
}
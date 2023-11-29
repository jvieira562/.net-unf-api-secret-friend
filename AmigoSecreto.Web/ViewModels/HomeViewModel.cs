using AmigoSecreto.Web.Models;

namespace AmigoSecreto.Web.ViewModels;

public class HomeViewModel
{
    public List<Amigo>? Amigos { get; set; } = new();
    public FeedbackModel? Feedback { get; set; } = new();
}
public class FeedbackModel
{
    private Tipo _tipo;
    public Tipo Tipo
    {
        get => _tipo;
        set
        {
            if (!Enum.IsDefined(typeof(Tipo), value))
                throw new ArgumentOutOfRangeException(nameof(value), "Tipo inválido.");

            _tipo = value;
        }
    }
    public string Message { get; set; } = string.Empty;

    public static FeedbackModel Create(Tipo tipo, string message)
        => new FeedbackModel
        {
            Tipo = tipo,
            Message = message
        };

    public bool IsValid()
        => string.IsNullOrEmpty(Message) ? false : true;
}
public enum Tipo
{
    Error,
    Success,
    Info
}
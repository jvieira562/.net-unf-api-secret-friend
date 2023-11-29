using AmigoSecreto.Desktop.Models;
using AmigoSecreto.Desktop.ViewModels;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;

namespace AmigoSecreto.Desktop
{
    public partial class AmigoSecretoHome : Form
    {
        private List<Amigo> amigos = new List<Amigo>();
        private readonly ApiHelper _apiHelper;

        public AmigoSecretoHome()
        {
            InitializeComponent();
            listaAmigosView.DataSource = amigos;
            _apiHelper = new ApiHelper();

            AtualizarInformacoes();
        }
        private async void AtualizarInformacoes()
        {
            try
            {
                var response = await _apiHelper.GetAsync("/v1/buscar-todos");
                amigos = JsonConvert.DeserializeObject<List<Amigo>>(response);

                listaAmigosView.DataSource = null;
                listaAmigosView.DataSource = amigos;
                lblQuantidadeDeCadastrados.Text = amigos.Count().ToString();
            }
            catch
            {
                MessageBox.Show("A API está fora do ar. Contate um administrador", "Sistema fora do ar");
            }
        }

        private void AmigoSecretoHome_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AtualizarInformacoes();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            var result = await _apiHelper.PostAync("/v1/gerar-pares", new { flag = Configuration.GetApiFlag() });

            if (result > 0)
            {
                AtualizarInformacoes();
                inNome = null;
                inEmail = null;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
        }

        private async void btnCadastrar_Click(object sender, EventArgs e)
        {
            var nome = inNome
                .Text
                .Replace(";", "")
                .Replace("-", "")
                .Replace("@", "")
                .Trim();

            var email = inEmail
                .Text
                .Replace(";", "")
                .Trim();

            var model = new PostAmigoViewModel(nome, email);

            var result = await _apiHelper.PostAync("/v1/registrar", model);
            if (result > 0)
            {
                AtualizarInformacoes();
                inNome = null;
                inEmail = null;
            }
            Console.WriteLine(result);
        }

        private void listaAmigosView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void btnBuscarAmigo_Click(object sender, EventArgs e)
        {
            var param = inParam.Text;
            IEnumerable<Amigo> result;

            if (param.Contains("@"))
            {
                param = param.Replace(" ", "");

                result = amigos
                    .Where(amg => amg.Email.ToUpper().Contains(param.ToUpper())).ToList();

                if (result is null)
                    MessageBox
                        .Show($"O amigo com e-mail {param} não foi encontrado.", "Não encontrado!");

                AtualizarTabela(result);
                return;
            }

            if (param.Contains("-"))
            {
                param = param.Replace(" ", "");
                result = amigos
                    .Where(amg => amg.Id == Guid.Parse(param));

                if (result is null)
                {
                    MessageBox.Show(@$"Amigo com id {param} não foi encontrado.", "Não Encontrado");
                    return;
                }

                AtualizarTabela(result);
                return;
            }
            else
                result = amigos
                    .Where(amg => amg.Name
                        .ToUpper()
                        .Contains(param.ToUpper()))
                    .ToList();

            if (result is null)
            {
                MessageBox
                    .Show($"Não foi encontrado nenhum amigo com o parâmetro fornecido!\n {param}", "Não encontrado!");
                return;
            }
            AtualizarTabela(result);
            return;
        }
        private void AtualizarTabela(IEnumerable<Amigo> amigos)
        {
            listaAmigosView.DataSource = null;
            listaAmigosView.DataSource = amigos;
        }
    }
}
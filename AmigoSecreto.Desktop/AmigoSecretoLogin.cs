using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AmigoSecreto.Desktop
{
    public partial class AmigoSecretoLogin : Form
    {
        public Dictionary<string, string> usuariosPermitidos;

        public AmigoSecretoLogin()
        {
            InitializeComponent();
            usuariosPermitidos = new Dictionary<string, string>();
            usuariosPermitidos.Add("jose.victor@amigosecreto.com.br", "1q2w!Q@W");
            usuariosPermitidos.Add("cayan.mello@amigosecreto.com.br", "!Q@W1q2w");
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void AmigoSecretoLogin_Load(object sender, EventArgs e)
        {
            lblErrorMessage.Hide();
        }

        private void btnEntrar_Click(object sender, EventArgs e)
        {
            if (!(usuariosPermitidos.ContainsKey(inEmail.Text)
                && usuariosPermitidos[inEmail.Text] == inSenha.Text))
            {
                lblErrorMessage.Show();
                return;
            }

            var telaHome = new AmigoSecretoHome();
            telaHome.Show();
            Hide();
        }

        private void inEmail_TextChanged(object sender, EventArgs e)
        {
        }
    }
}
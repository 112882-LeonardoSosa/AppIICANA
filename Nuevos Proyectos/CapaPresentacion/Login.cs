using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaNegocio;
using CapaEntidad;

namespace CapaPresentacion
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            List<Usuario> Test = new CN_Usuario().Listar();

            Usuario usuario = new CN_Usuario().Listar().Where(u => u.Documento == txtNroDocumento.Text &&
            u.Clave == txtContrasenia.Text).FirstOrDefault();

            if (usuario != null)
            {

                Inicio form = new Inicio(usuario);
                form.Show();
                this.Hide();

                form.FormClosing += frm_closing;
            }
            else 
            {
                MessageBox.Show("Usuario No Registrado!","Mensaje",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
        }

        private void frm_closing(object sender, FormClosingEventArgs e)
        {
            txtContrasenia.Text = String.Empty;
            txtNroDocumento.Text = String.Empty;    

            this.Show();
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        private void txtContrasenia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                List<Usuario> Test = new CN_Usuario().Listar();

                Usuario usuario = new CN_Usuario().Listar().Where(u => u.Documento == txtNroDocumento.Text &&
                u.Clave == txtContrasenia.Text).FirstOrDefault();

                if (usuario != null)
                {

                    Inicio form = new Inicio(usuario);
                    form.Show();
                    this.Hide();

                    form.FormClosing += frm_closing;
                }
                else
                {
                    MessageBox.Show("Usuario No Registrado!", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void txtContrasenia_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmNegocio : Form
    {
        public frmNegocio()
        {
            InitializeComponent();
        }

        public Image ByteToImage(byte[] imageBytes)
        {
            MemoryStream Ms = new MemoryStream();
            Ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = new Bitmap(Ms);

            return image;

        }
        private void frmNegocio_Load(object sender, EventArgs e)
        {
            bool Obtenido = true;
            byte[] ByteImage = new CN_Negocio().ObtenerLogo(out Obtenido);

            if (Obtenido)
            {
                picLogo.Image = ByteToImage(ByteImage);
            }

            Negocio Datos = new CN_Negocio().ObtenerDatos();

            txtNombreNegocio.Text = Datos.NombreNegocio;
            txtCuil.Text = Datos.CUIL;
            txtDireccion.Text = Datos.Direccion;
        }

        private void btnSubir_Click(object sender, EventArgs e)
        {
            string Mensaje = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Files|*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                byte[] byteimage = File.ReadAllBytes(openFileDialog.FileName);
                bool Respuesta = new CN_Negocio().ActualizarLogo(byteimage,out Mensaje);

                if (Respuesta)
                    picLogo.Image = ByteToImage(byteimage);
                else
                    MessageBox.Show(Mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string Mensaje = string.Empty;

            Negocio Obj = new Negocio()
            {
                NombreNegocio = txtNombreNegocio.Text,
                CUIL = txtCuil.Text,
                Direccion = txtDireccion.Text
            };
            
            bool Respuesta = new CN_Negocio().GuardarDatos(Obj,out Mensaje);

            if (Respuesta)
            {
                MessageBox.Show("Los cambios fueron guardados!", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                MessageBox.Show("No se pudieron efectuar los cambios...", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }
    }
}

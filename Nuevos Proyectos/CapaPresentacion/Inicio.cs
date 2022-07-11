using CapaEntidad;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FontAwesome.Sharp;
using CapaNegocio;
using CapaPresentacion.Modales;

namespace CapaPresentacion
{
    public partial class Inicio : Form
    {
        private static Usuario UsuarioActual;
        private static IconMenuItem MenuActivo = null;
        private static Form FormularioActivo = null;
        public Inicio(Usuario objUsuario = null)
        {
            if (objUsuario == null)
            {
                UsuarioActual = new Usuario() { NombreCompleto = "Admin Predefinido", IdUsuario = 1};
            }

            else
            UsuarioActual = objUsuario;


            InitializeComponent();
        }

        private void Inicio_Load(object sender, EventArgs e)
        {
            List<Permiso> ListaPermisos = new CN_Permiso().Listar(UsuarioActual.IdUsuario);

            foreach (IconMenuItem iconmenu in Menu.Items) 
            {
                bool Encontrado = ListaPermisos.Any(m => m.NombreMenu == iconmenu.Name);
                if (Encontrado == false) 
                {
                    iconmenu.Visible = false;
                }
            }

            lblUsuario.Text = UsuarioActual.NombreCompleto.ToString();
        }

        private void AbrirFormulario(IconMenuItem Menu, Form Formulario) 
        {
            if (MenuActivo != null)
            {
                MenuActivo.BackColor = Color.White;
            }

            Menu.BackColor = Color.Silver;
            MenuActivo = Menu;

            if (FormularioActivo !=null)
            {
                FormularioActivo.Close();
            }
            FormularioActivo = Formulario;
            Formulario.TopLevel = false;
            Formulario.FormBorderStyle = FormBorderStyle.None;
            Formulario.Dock = DockStyle.Fill;//QUE EL CONTENIDO RELLENE TODO EL FORM
            //Formulario.BackColor = Color.;
            Contenedor.Controls.Add(Formulario);
            Formulario.Show();

        }
        private void menuUsuarios_Click(object sender, EventArgs e)
        {
            AbrirFormulario((IconMenuItem)sender, new frmUsuarios());
            
        }

        private void SubMenuCategoria_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuMantenimiento, new frmCategoria());
        }

        private void SubMenuProducto_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuMantenimiento, new frmProducto());
        }

        private void SubMenuRegistrarVenta_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuVentas, new frmVentas(UsuarioActual));
        }

        private void SubMenuVerDetalleVenta_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuVentas, new frmDetalleVentas());
        }

        private void SubMenuRegistrarCompra_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuCompras, new frmCompras(UsuarioActual));
        }

        private void SubMenuVerDetalleCompra_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuCompras, new frmDetalleCompras());
        }

        private void menuClientes_Click(object sender, EventArgs e)
        {
            AbrirFormulario((IconMenuItem)sender, new frmClientes());
        }

        private void menuProveedores_Click(object sender, EventArgs e)
        {
            AbrirFormulario((IconMenuItem)sender, new frmProveedores());
        }

        private void Contenedor_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void subMenuNegocio_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuMantenimiento, new frmNegocio());
        }

        private void subMenuReporteCompras_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuReportes, new frmReporteCompras());
        }

        private void subMenuReporteVentas_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuReportes, new frmReporteVentas());
        }

        private void menuAcercaDe_Click(object sender, EventArgs e)
        {
            mdAcercaDe md = new mdAcercaDe();
            md.ShowDialog();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Desea Salir?","Mensaje",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void menuTitulo_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Menu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}

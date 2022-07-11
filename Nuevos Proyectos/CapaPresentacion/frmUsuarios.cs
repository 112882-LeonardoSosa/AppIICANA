using CapaPresentacion.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaEntidad;
using CapaNegocio;

namespace CapaPresentacion
{
    public partial class frmUsuarios : Form
    {
        public frmUsuarios()
        {
            InitializeComponent();
        }


        private void frmUsuarios_Load(object sender, EventArgs e)
        {
            
            cboEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cboEstado.DisplayMember = "Texto";
            cboEstado.ValueMember = "Valor";
            cboEstado.SelectedIndex = 0;

            List<Rol> listaRol = new CN_Rol().Listar();
            foreach (Rol item in listaRol) 
            {

                cboRol.Items.Add(new OpcionCombo() { Valor = item.IdRol, Texto = item.Descripcion });
            }
            cboRol.DisplayMember = "Texto";
            cboRol.ValueMember = "Valor";
            cboRol.SelectedIndex = 0;

            foreach (DataGridViewColumn columna in dgvUsuarios.Columns) 
            {
                if (columna.Visible == true && columna.Name != "btnSeleccionar")
                {
                    cboBuscar.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }
            
            }
            cboBuscar.DisplayMember = "Texto";
            cboBuscar.ValueMember = "Valor";
            cboBuscar.SelectedIndex = 0;

            //MOSTRAR TODOS LOS USUARIOS
            List<Usuario> listaUsuario = new CN_Usuario().Listar();
            foreach (Usuario item in listaUsuario)
            {

            dgvUsuarios.Rows.Add(new object[] {"",item.IdUsuario,item.Documento,item.NombreCompleto,item.Correo,item.Clave,
                item.oRol.IdRol,
                item.oRol.Descripcion,
                item.Estado == true ? 1 : 0,
                item.Estado == true ? "Activo" : "No Activo"

            });

            }
           

        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string Mensaje = string.Empty;


            Usuario objUsuario = new Usuario()
            {
                IdUsuario = Convert.ToInt32(txtId.Text),
                Documento = txtDocumento.Text,
                NombreCompleto = txtNombreCom.Text,
                Correo = txtCorreo.Text,
                Clave = txtContrasenia.Text,
                oRol = new Rol() { IdRol = Convert.ToInt32(((OpcionCombo)cboRol.SelectedItem).Valor) },
                Estado = Convert.ToInt32(((OpcionCombo)cboEstado.SelectedItem).Valor) == 1 ? true : false

            };

            

            if (objUsuario.IdUsuario == 0)
            {
                int idUsuarioGenerado = new CN_Usuario().Registrar(objUsuario, out Mensaje);

                if (idUsuarioGenerado != 0)
                {
                    dgvUsuarios.Rows.Add(new object[] {"",idUsuarioGenerado,txtDocumento.Text,txtNombreCom.Text,txtCorreo.Text,txtContrasenia.Text,
                    ((OpcionCombo)cboRol.SelectedItem).Valor.ToString(),
                    ((OpcionCombo)cboRol.SelectedItem).Texto.ToString(),
                    ((OpcionCombo)cboEstado.SelectedItem).Valor.ToString(),
                    ((OpcionCombo)cboEstado.SelectedItem).Texto.ToString()
            ,});

                    Limpiar();
                }
                else
                {
                    MessageBox.Show(Mensaje);
                }
            }
            else
            {
                bool resultado = new CN_Usuario().Editar(objUsuario, out Mensaje);

                if (resultado)
                {
                    DataGridViewRow Row = dgvUsuarios.Rows[Convert.ToInt32(txtIndice.Text)];
                    Row.Cells["IdUsuario"].Value = txtId.Text;
                    Row.Cells["Documento"].Value = txtDocumento.Text;
                    Row.Cells["NombreCompleto"].Value = txtNombreCom.Text;
                    Row.Cells["Correo"].Value = txtCorreo.Text;
                    Row.Cells["Clave"].Value = txtContrasenia.Text;
                    Row.Cells["IdRol"].Value = ((OpcionCombo)cboRol.SelectedItem).Valor.ToString();
                    Row.Cells["Rol"].Value = ((OpcionCombo)cboRol.SelectedItem).Texto.ToString();
                    Row.Cells["EstadoValor"].Value = ((OpcionCombo)cboEstado.SelectedItem).Valor.ToString();
                    Row.Cells["Estado"].Value = ((OpcionCombo)cboEstado.SelectedItem).Texto.ToString();

                    Limpiar();

                }
                else
                {
                    MessageBox.Show(Mensaje);
                }
            }

            
        }

        private void Limpiar()
        {
            txtIndice.Text = "-1";
            txtId.Text = "0";
            txtDocumento.Text = String.Empty;
            txtNombreCom.Text = String.Empty;
            txtCorreo.Text = String.Empty;
            txtContrasenia.Text = String.Empty;
            txtConfirmarCont.Text = String.Empty;
            cboEstado.SelectedIndex = 0;
            cboRol.SelectedIndex=0;
            
            txtDocumento.Focus();
        
        }

        private void dgvUsuarios_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.comprobado.Width;
                var h = Properties.Resources.comprobado.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.comprobado, new Rectangle(x, y, w, h));
                e.Handled = true;

            }
        }

        private void dgvUsuarios_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvUsuarios.Columns[e.ColumnIndex].Name == "btnSeleccionar")
            {
                int indice = e.RowIndex;

                if (indice >= 0)
                {
                    txtIndice.Text = indice.ToString();
                    txtId.Text = dgvUsuarios.Rows[indice].Cells["IdUsuario"].Value.ToString();
                    txtDocumento.Text = dgvUsuarios.Rows[indice].Cells["Documento"].Value.ToString();
                    txtNombreCom.Text = dgvUsuarios.Rows[indice].Cells["NombreCompleto"].Value.ToString();
                    txtCorreo.Text = dgvUsuarios.Rows[indice].Cells["Correo"].Value.ToString();
                    txtContrasenia.Text = dgvUsuarios.Rows[indice].Cells["Clave"].Value.ToString();
                    txtConfirmarCont.Text = dgvUsuarios.Rows[indice].Cells["Clave"].Value.ToString();

                    foreach (OpcionCombo Op in cboRol.Items)
                    {
                        if (Convert.ToInt32(Op.Valor) == Convert.ToInt32(dgvUsuarios.Rows[indice].Cells["IdRol"].Value))
                        {
                            int indiceCombo = cboRol.Items.IndexOf(Op);
                            cboRol.SelectedIndex = indiceCombo;
                            break;
                        }
                    }

                    foreach (OpcionCombo Op in cboEstado.Items)
                    {
                        if (Convert.ToInt32(Op.Valor) == Convert.ToInt32(dgvUsuarios.Rows[indice].Cells["EstadoValor"].Value))
                        {
                            int indiceCombo = cboEstado.Items.IndexOf(Op);
                            cboEstado.SelectedIndex = indiceCombo;
                            break;
                        }
                    }
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtId.Text) != 0)
            {      
                if (MessageBox.Show("Desea Eliminar el Usuario?","Mensaje",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string Mensaje = string.Empty;
                    Usuario objUsuario = new Usuario()
                    { IdUsuario = Convert.ToInt32(txtId.Text) };

                    bool Respuesta = new CN_Usuario().Eliminar(objUsuario, out Mensaje);

                    if (Respuesta)
                    {
                        dgvUsuarios.Rows.RemoveAt(Convert.ToInt32(txtIndice.Text));
                    }
                    else
                    {
                        MessageBox.Show(Mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string ColumnaFiltro = ((OpcionCombo)cboBuscar.SelectedItem).Valor.ToString();

            if (dgvUsuarios.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvUsuarios.Rows)
                {
                    if (row.Cells[ColumnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtBuscar.Text.Trim().ToUpper()))
                    {
                        row.Visible = true;
                    }
                    else
                    {
                        row.Visible=false;
                    }
                }
            }

        }

        private void btnLimpiarBuscador_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = String.Empty;
            foreach (DataGridViewRow row in dgvUsuarios.Rows)
            {
                row.Visible = true;
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }
    }
}

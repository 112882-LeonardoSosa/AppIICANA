using CapaEntidad;
using CapaNegocio;
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

namespace CapaPresentacion
{
    public partial class frmProveedores : Form
    {
        public frmProveedores()
        {
            InitializeComponent();
        }

        private void frmProveedores_Load(object sender, EventArgs e)
        {
            cboEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cboEstado.DisplayMember = "Texto";
            cboEstado.ValueMember = "Valor";
            cboEstado.SelectedIndex = 0;


            foreach (DataGridViewColumn columna in dgvProveedores.Columns)
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
            List<Proveedor> listaProveedor = new CN_Proveedor().Listar();
            
            foreach (Proveedor item in listaProveedor)
            {

                dgvProveedores.Rows.Add(new object[] {"",item.IdProveedor,item.Documento,item.RazonSocial,item.Correo,item.Telefono,
                item.Estado == true ? 1 : 0,
                item.Estado == true ? "Activo" : "No Activo"

            });

            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string Mensaje = string.Empty;


            Proveedor objProveedor = new Proveedor()
            {
                //IdProveedor = Convert.ToInt32(txtId.Text),
                Documento = txtDocumento.Text,
                RazonSocial = txtRazonSocial.Text,
                Correo = txtCorreo.Text,
                Telefono = txtTelefono.Text,
                Estado = Convert.ToInt32(((OpcionCombo)cboEstado.SelectedItem).Valor) == 1 ? true : false

            };

            if (txtIndice.Text == "-1")
            {

                int idGenerado = new CN_Proveedor().Registrar(objProveedor, out Mensaje);

                if (idGenerado != 0)
                {
                    dgvProveedores.Rows.Add(new object[] {"",idGenerado,txtDocumento.Text,txtRazonSocial.Text,txtCorreo.Text,txtTelefono.Text,
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
                bool resultado = new CN_Proveedor().Editar(objProveedor, out Mensaje);

                if (resultado)
                {
                    DataGridViewRow Row = dgvProveedores.Rows[Convert.ToInt32(txtIndice.Text)];
                    Row.Cells["IdProveedor"].Value = txtId.Text;
                    Row.Cells["Documento"].Value = txtDocumento.Text;
                    Row.Cells["RazonSocial"].Value = txtRazonSocial.Text;
                    Row.Cells["Correo"].Value = txtCorreo.Text;
                    Row.Cells["Telefono"].Value = txtTelefono.Text;
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
            txtRazonSocial.Text = String.Empty;
            txtCorreo.Text = String.Empty;
            txtTelefono.Text = String.Empty;
            cboEstado.SelectedIndex = 0;


            txtDocumento.Focus();

        }

        private void dgvProveedores_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
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

        private void dgvProveedores_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvProveedores.Columns[e.ColumnIndex].Name == "btnSeleccionar")
            {
                int indice = e.RowIndex;

                if (indice >= 0)
                {
                    txtIndice.Text = indice.ToString();
                    txtId.Text = dgvProveedores.Rows[indice].Cells["IdProveedor"].Value.ToString();
                    txtDocumento.Text = dgvProveedores.Rows[indice].Cells["Documento"].Value.ToString();
                    txtRazonSocial.Text = dgvProveedores.Rows[indice].Cells["RazonSocial"].Value.ToString();
                    txtCorreo.Text = dgvProveedores.Rows[indice].Cells["Correo"].Value.ToString();
                    txtTelefono.Text = dgvProveedores.Rows[indice].Cells["Telefono"].Value.ToString();


                    foreach (OpcionCombo Op in cboEstado.Items)
                    {
                        if (Convert.ToInt32(Op.Valor) == Convert.ToInt32(dgvProveedores.Rows[indice].Cells["EstadoValor"].Value))
                        {
                            int indiceCombo = cboEstado.Items.IndexOf(Op);
                            cboEstado.SelectedIndex = indiceCombo;
                            break;
                        }
                    }
                }
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtId.Text) != 0)
            {
                if (MessageBox.Show("Desea Eliminar el Proveedor?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string Mensaje = string.Empty;
                    Proveedor objProveedor = new Proveedor()
                    { IdProveedor = Convert.ToInt32(txtId.Text) };

                    bool Respuesta = new CN_Proveedor().Eliminar(objProveedor, out Mensaje);

                    if (Respuesta)
                    {
                        dgvProveedores.Rows.RemoveAt(Convert.ToInt32(txtIndice.Text));
                    }
                    else
                    {
                        MessageBox.Show(Mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string ColumnaFiltro = ((OpcionCombo)cboBuscar.SelectedItem).Valor.ToString();

            if (dgvProveedores.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvProveedores.Rows)
                {
                    if (row.Cells[ColumnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtBuscar.Text.Trim().ToUpper()))
                    {
                        row.Visible = true;
                    }
                    else
                    {
                        row.Visible = false;
                    }
                }
            }
        }

        private void btnLimpiarBuscador_Click(object sender, EventArgs e)
        {
            txtBuscar.Text = String.Empty;
            foreach (DataGridViewRow row in dgvProveedores.Rows)
            {
                row.Visible = true;
            }
        }

        private void cboEstado_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboBuscar_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }
    }
}

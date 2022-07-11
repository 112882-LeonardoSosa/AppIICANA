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

namespace CapaPresentacion.Modales
{
    public partial class mdProducto : Form
    {
        public Producto oProducto { get; set; }
        public mdProducto()
        {
            InitializeComponent();
        }

        private void mdProducto_Load(object sender, EventArgs e)
        {

            foreach (DataGridViewColumn columna in dgvProductos.Columns)
            {
                if (columna.Visible == true)
                {
                    cboBuscar.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }

            }
            cboBuscar.DisplayMember = "Texto";
            cboBuscar.ValueMember = "Valor";
            cboBuscar.SelectedIndex = 1;

            //MOSTRAR TODOS LOS USUARIOS
            List<Producto> listaPro = new CN_Producto().Listar();
            foreach (Producto item in listaPro)
            {

                dgvProductos.Rows.Add(new object[] {item.IdProducto,item.Codigo,item.Nombre,
                item.Descripcion,
                item.PrecioVenta            
            });

            }
        }

        private void dgvProductos_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int iRow = e.RowIndex;
            int iColum = e.ColumnIndex;

            if (iRow >= 0 && iColum > 0)
            {
                oProducto = new Producto()
                {
                    IdProducto = Convert.ToInt32(dgvProductos.Rows[iRow].Cells["IdProducto"].Value.ToString()),
                    Codigo = dgvProductos.Rows[iRow].Cells["Codigo"].Value.ToString(),
                    Nombre = dgvProductos.Rows[iRow].Cells["Nombre"].Value.ToString(),
                    Descripcion = dgvProductos.Rows[iRow].Cells["Descripcion"].Value.ToString(),                   
                    PrecioVenta = Convert.ToDecimal(dgvProductos.Rows[iRow].Cells["PrecioVenta"].Value.ToString())

                };

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string ColumnaFiltro = ((OpcionCombo)cboBuscar.SelectedItem).Valor.ToString();

            if (dgvProductos.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvProductos.Rows)
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
            foreach (DataGridViewRow row in dgvProductos.Rows)
            {
                row.Visible = true;
            }
        }

        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter)) 
            {
                string ColumnaFiltro = ((OpcionCombo)cboBuscar.SelectedItem).Valor.ToString();

                if (dgvProductos.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvProductos.Rows)
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
        }

        private void dgvProductos_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvProductos_CellContentDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            int iRow = e.RowIndex;
            int iColum = e.ColumnIndex;

            if (iRow >= 0 && iColum > 0)
            {
                oProducto = new Producto()
                {
                    IdProducto = Convert.ToInt32(dgvProductos.Rows[iRow].Cells["IdProducto"].Value.ToString()),
                    Codigo = dgvProductos.Rows[iRow].Cells["Codigo"].Value.ToString(),
                    Nombre = dgvProductos.Rows[iRow].Cells["Nombre"].Value.ToString(),
                    Descripcion = dgvProductos.Rows[iRow].Cells["Descripcion"].Value.ToString(),
                    PrecioVenta = Convert.ToDecimal(dgvProductos.Rows[iRow].Cells["PrecioVenta"].Value.ToString())

                };

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}

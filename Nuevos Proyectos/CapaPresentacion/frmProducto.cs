using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
using ClosedXML.Excel;
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
    public partial class frmProducto : Form
    {
        public frmProducto()
        {
            InitializeComponent();
        }

        private void frmProducto_Load(object sender, EventArgs e)
        {
            cboEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cboEstado.DisplayMember = "Texto";
            cboEstado.ValueMember = "Valor";
            cboEstado.SelectedIndex = 0;

            List<Categoria> lista = new CN_Categoria().Listar();
            foreach (Categoria item in lista)
            {

                cboCategoria.Items.Add(new OpcionCombo() { Valor = item.IdCategoria, Texto = item.Descripcion });
            }
            cboCategoria.DisplayMember = "Texto";
            cboCategoria.ValueMember = "Valor";
            cboCategoria.SelectedIndex = 0;

            foreach (DataGridViewColumn columna in dgvProductos.Columns)
            {
                if (columna.Visible == true && columna.Name != "btnSeleccionar")
                {
                    cboBuscar.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }

            }
            cboBuscar.DisplayMember = "Texto";
            cboBuscar.ValueMember = "Valor";
            cboBuscar.SelectedIndex = 0;

            //MOSTRAR TODOS LOS PRODUCTOS
            List<Producto> listaPro = new CN_Producto().Listar();
            foreach (Producto item in listaPro)
            {

                dgvProductos.Rows.Add(new object[] {"",item.IdProducto,item.Codigo,item.Nombre,item.Descripcion,
                item.oCategoria.IdCategoria,item.oCategoria.Descripcion,
                item.PrecioVenta,
                item.Estado == true ? 1 : 0,
                item.Estado == true ? "Activo" : "No Activo"

            });

            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string Mensaje = string.Empty;


            Producto objProducto = new Producto()
            {
                IdProducto = Convert.ToInt32(txtId.Text),
                Codigo = txtCodigo.Text,
                Nombre = txtNombre.Text,
                Descripcion = txtDescripcion.Text,
                oCategoria = new Categoria() { IdCategoria = Convert.ToInt32(((OpcionCombo)cboCategoria.SelectedItem).Valor) },
                PrecioVenta = Convert.ToDecimal(txtPrecio.Text),
                Estado = Convert.ToInt32(((OpcionCombo)cboEstado.SelectedItem).Valor) == 1 ? true : false

            };

            

            if (objProducto.IdProducto == 0)
            {
                int idProductoGenerado = new CN_Producto().Registrar(objProducto, out Mensaje);

                if (idProductoGenerado != 0)
                {
                    dgvProductos.Rows.Add(new object[] {"",idProductoGenerado,txtCodigo.Text,txtNombre.Text,txtDescripcion.Text,
                    ((OpcionCombo)cboCategoria.SelectedItem).Valor.ToString(),
                    ((OpcionCombo)cboCategoria.SelectedItem).Texto.ToString(),
                    txtPrecio.Text,
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
                bool resultado = new CN_Producto().Editar(objProducto, out Mensaje);

                if (resultado == true)
                {
                    DataGridViewRow Row = dgvProductos.Rows[Convert.ToInt32(txtIndice.Text)];
                    Row.Cells["IdProducto"].Value = txtId.Text;
                    Row.Cells["Codigo"].Value = txtCodigo.Text;
                    Row.Cells["Nombre"].Value = txtNombre.Text;
                    Row.Cells["Descripcion"].Value = txtDescripcion.Text;
                    Row.Cells["IdCategoria"].Value = ((OpcionCombo)cboCategoria.SelectedItem).Valor.ToString();
                    Row.Cells["Categoria"].Value = ((OpcionCombo)cboCategoria.SelectedItem).Texto.ToString();
                    Row.Cells["PrecioVenta"].Value = txtPrecio.Text;
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
            txtCodigo.Text = String.Empty;
            txtNombre.Text = String.Empty;
            txtDescripcion.Text = String.Empty;
            txtPrecio.Text = String.Empty;
            cboEstado.SelectedIndex = 0;
            cboCategoria.SelectedIndex = 0;

            txtCodigo.Focus();

        }

        private void dgvProductos_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
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

        private void dgvProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvProductos.Columns[e.ColumnIndex].Name == "btnSeleccionar")
            {
                int indice = e.RowIndex;

                if (indice >= 0)
                {
                    txtIndice.Text = indice.ToString();
                    txtId.Text = dgvProductos.Rows[indice].Cells["IdProducto"].Value.ToString();
                    txtCodigo.Text = dgvProductos.Rows[indice].Cells["Codigo"].Value.ToString();
                    txtNombre.Text = dgvProductos.Rows[indice].Cells["Nombre"].Value.ToString();
                    txtDescripcion.Text = dgvProductos.Rows[indice].Cells["Descripcion"].Value.ToString();
                    txtPrecio.Text = dgvProductos.Rows[indice].Cells["PrecioVenta"].Value.ToString();


                    foreach (OpcionCombo Op in cboCategoria.Items)
                    {
                        if (Convert.ToInt32(Op.Valor) == Convert.ToInt32(dgvProductos.Rows[indice].Cells["IdCategoria"].Value))
                        {
                            int indiceCombo = cboCategoria.Items.IndexOf(Op);
                            cboCategoria.SelectedIndex = indiceCombo;
                            break;
                        }
                    }

                    foreach (OpcionCombo Op in cboEstado.Items)
                    {
                        if (Convert.ToInt32(Op.Valor) == Convert.ToInt32(dgvProductos.Rows[indice].Cells["EstadoValor"].Value))
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
                if (MessageBox.Show("Desea Eliminar el Producto?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string Mensaje = string.Empty;
                    Producto objProducto = new Producto()
                    { IdProducto = Convert.ToInt32(txtId.Text) };

                    bool Respuesta = new CN_Producto().Eliminar(objProducto, out Mensaje);

                    if (Respuesta)
                    {
                        dgvProductos.Rows.RemoveAt(Convert.ToInt32(txtIndice.Text));
                        Limpiar();
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

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnDescargar_Click(object sender, EventArgs e)
        {
            if (dgvProductos.Rows.Count < 1)
            {
                MessageBox.Show("No hay datos para exportar...", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DataTable dt = new DataTable();
                foreach (DataGridViewColumn Columna in dgvProductos.Columns)
                {
                    if (Columna.HeaderText != String.Empty && Columna.Visible)
                    {
                        dt.Columns.Add(Columna.HeaderText,typeof(string));
                    }
                }
                foreach (DataGridViewRow Row in dgvProductos.Rows)
                {
                    if (Row.Visible)
                    {
                        dt.Rows.Add(new object[] {
                            Row.Cells[2].Value.ToString(),
                            Row.Cells[3].Value.ToString(),                           
                            Row.Cells[4].Value.ToString(),
                            Row.Cells[6].Value.ToString(),
                            Row.Cells[7].Value.ToString(),                           
                            Row.Cells[9].Value.ToString()
                            
                        });
                    }
                }

                SaveFileDialog SaveFile = new SaveFileDialog();
                SaveFile.FileName = string.Format("ReporteProducto_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
                SaveFile.Filter = "Excel Files | *.xlsx";

                if (SaveFile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        XLWorkbook WB = new XLWorkbook();
                        var Hoja = WB.Worksheets.Add(dt, "Informe");
                        Hoja.ColumnsUsed().AdjustToContents();
                        WB.SaveAs(SaveFile.FileName);
                        MessageBox.Show("Reporte Generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }
                    catch
                    {
                        MessageBox.Show("Error al generar Informe...", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw;
                    }
                }
            }
        }


    }
}

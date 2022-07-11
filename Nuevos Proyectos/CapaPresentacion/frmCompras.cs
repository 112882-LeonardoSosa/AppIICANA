using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
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
    public partial class frmCompras : Form
    {
        private Usuario _Usuario;
        public frmCompras(Usuario oUsuario = null)
        {
            _Usuario = oUsuario;
            InitializeComponent();
        }

        private void frmCompras_Load(object sender, EventArgs e)
        {
            cboTipoDoc.Items.Add(new OpcionCombo() { Valor = "Recibo", Texto = "Recibo" });
            cboTipoDoc.Items.Add(new OpcionCombo() { Valor = "Factura", Texto = "Factura" });
            cboTipoDoc.DisplayMember = "Texto";
            cboTipoDoc.ValueMember = "Valor";
            cboTipoDoc.SelectedIndex = 0;

            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtIdProveedor.Text = "0";
            txtIdProd.Text = "0";
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            using (var modal = new mdProveedor())
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK)
                {
                    txtIdProveedor.Text = modal.oProveedor.IdProveedor.ToString();
                    txtDocProv.Text = modal.oProveedor.Documento;
                    txtNomProve.Text = modal.oProveedor.RazonSocial;
                }
                else
                {
                    txtDocProv.Select();
                }

            }
        }

        private void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            using (var modal = new mdProducto())
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK)
                {
                    txtIdProd.Text = modal.oProducto.IdProducto.ToString();
                    txtCodProd.Text = modal.oProducto.Codigo;
                    txtProd.Text = modal.oProducto.Nombre;
                    txtPrecioCom.Select();
                }
                else
                {
                    txtCodProd.Select();
                }

            }
        }

        private void txtCodProd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                Producto oProducto = new CN_Producto().Listar().Where(p => p.Codigo == txtCodProd.Text && p.Estado == true).FirstOrDefault();
                if (oProducto != null)
                {
                    txtCodProd.BackColor = Color.LightGreen;
                    txtIdProd.Text = oProducto.IdProducto.ToString();
                    txtProd.Text = oProducto.Nombre;
                    txtPrecioCom.Select();
                }
                else
                {
                    txtCodProd.BackColor = Color.OrangeRed;
                    txtIdProd.Text = "0";
                    txtProd.Text = String.Empty;
                }
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            decimal precioCompra = 0;
            decimal precioVenta = 0;
            bool productoExiste = false;

            if (int.Parse(txtIdProd.Text) == 0)
            {
                MessageBox.Show("Debe Seleccionar un Producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!decimal.TryParse(txtPrecioCom.Text, out precioCompra))
            {
                MessageBox.Show("Precio Compra - Formato Moneda Incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecioCom.Focus();
                return;
            }

            if (!decimal.TryParse(txtPrecioVen.Text, out precioVenta))
            {
                MessageBox.Show("Precio Venta - Formato Moneda Incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecioVen.Focus();
                return;
            }

            foreach (DataGridViewRow fila in dgvCompras.Rows)
            {
                if (Convert.ToString(fila.Cells["IdProducto"].Value) == txtIdProd.Text)
                {
                    productoExiste = true;
                    break;
                }
            }

            if (!productoExiste) //si es falso
            {
                dgvCompras.Rows.Add(new object[]{
                    txtIdProd.Text,
                    txtProd.Text,
                    precioCompra.ToString("0.00"),
                    precioVenta.ToString("0.00"),
                    nudCantidad.Value.ToString(),
                    (nudCantidad.Value * precioCompra).ToString("0.00")

                });
                calcularTotal();
                limpiarProducto();
                txtCodProd.Focus();
            }
        }

        private void limpiarProducto() 
        {
            txtIdProd.Text = "0";
            txtCodProd.Text = "";
            txtCodProd.BackColor = System.Drawing.Color.White;
            txtProd.Text = "";
            txtPrecioCom.Text = "";
            txtPrecioVen.Text = "";
            nudCantidad.Value = 1;
        }

        private void calcularTotal() 
        {
            decimal total = 0;
            if (dgvCompras.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvCompras.Rows)
                {
                    total += Convert.ToDecimal(row.Cells["ColSubTotal"].Value);
                }

                txtTotal.Text = total.ToString("0.00");
            }
        }

        private void dgvCompras_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex == 6)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);

                var w = Properties.Resources.cancelar.Width;
                var h = Properties.Resources.cancelar.Height;
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2;
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2;

                e.Graphics.DrawImage(Properties.Resources.cancelar, new Rectangle(x, y, w, h));
                e.Handled = true;

            }
        }

        private void dgvCompras_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvCompras.Columns[e.ColumnIndex].Name == "ColAccion")
            {
                int indice = e.RowIndex;

                if (indice >= 0)
                {
                    dgvCompras.Rows.RemoveAt(indice);
                    calcularTotal();

                }
            }
        }

        private void txtPrecioCom_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtPrecioCom.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    if (Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void txtPrecioVen_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtPrecioVen.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
                {
                    e.Handled = true;
                }
                else
                {
                    if (Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtIdProveedor.Text) == 0)
            {
                MessageBox.Show("Debe Seleccionar un Proveedor", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (dgvCompras.Rows.Count < 1)
            {
                MessageBox.Show("Debe Ingresar Productos en la Compra...", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DataTable Detalle_Compra = new DataTable();
            Detalle_Compra.Columns.Add("IdProducto", typeof(int));
            Detalle_Compra.Columns.Add("PrecioCompra", typeof(decimal));
            Detalle_Compra.Columns.Add("PrecioVenta", typeof(decimal));
            Detalle_Compra.Columns.Add("Cantidad", typeof(int));
            Detalle_Compra.Columns.Add("MontoTotal", typeof(decimal));

            foreach (DataGridViewRow row in dgvCompras.Rows)
            {
                Detalle_Compra.Rows.Add
                    (new object[]
                    {
                        Convert.ToInt32(row.Cells["IdProducto"].Value.ToString()),
                        row.Cells["ColPrecioCompra"].Value.ToString(),
                        row.Cells["ColPrecioVenta"].Value.ToString(),
                        row.Cells["ColCantidad"].Value.ToString(),
                        row.Cells["ColSubTotal"].Value.ToString()
                    }
                    );
                int idCorrelativo = new CN_Compra().ObtenerCorrelativo();
                string NumeroDocumento = string.Format("{0:00000}", idCorrelativo);

                Compra oCompra = new Compra()
                {
                    oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario },
                    oProveedor = new Proveedor() { IdProveedor = Convert.ToInt32(txtIdProveedor.Text) },
                    TipoDocumento = ((OpcionCombo)cboTipoDoc.SelectedItem).Texto,
                    NroDocumento = NumeroDocumento,
                    MontoTotal = Convert.ToDecimal(txtTotal.Text)
                };

                string Mensaje = string.Empty;
                bool Respuesta = new CN_Compra().Registrar(oCompra,Detalle_Compra,out Mensaje);

                if (Respuesta)
                {
                    var Result = MessageBox.Show("Numero de Compra:\n"+NumeroDocumento+ "\n\n¿Desea copiar al PortaPapeles","Mensaje",MessageBoxButtons.YesNo,MessageBoxIcon.Information);
                    if (Result == DialogResult.Yes)
                    {
                        Clipboard.SetText(NumeroDocumento);
                    }
                    txtIdProd.Text = "0";
                    txtDocProv.Text = "";
                    txtNomProve.Text = "";
                    dgvCompras.Rows.Clear();
                    calcularTotal();
                }
                else
                {
                    MessageBox.Show(Mensaje,"Mensaje",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                }
            }
        }
    }
}

using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Modales;
using CapaPresentacion.Utilidades;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text;
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
    public partial class frmVentas : Form
    {
        private Usuario _Usuario;
        private Venta nuevo;

        public frmVentas(Usuario oUsuario = null)
        {

            _Usuario = oUsuario;
            InitializeComponent();
        }

        private void frmVentas_Load(object sender, EventArgs e)
        {
            cboTipoDoc.Items.Add(new OpcionCombo() { Valor = "Recibo", Texto = "Recibo" });
            cboTipoDoc.Items.Add(new OpcionCombo() { Valor = "Factura", Texto = "Factura" });
            cboTipoDoc.DisplayMember = "Texto";
            cboTipoDoc.ValueMember = "Valor";
            cboTipoDoc.SelectedIndex = 0;

            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");          
            txtIdProducto.Text = "0";

            txtPagaCon.Text = "";
            txtVuelto.Text = "";
            txtTotal.Text = "0";
        }

        private void btnBuscarCl_Click(object sender, EventArgs e)
        {
            using (var modal = new mdCliente())
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK)
                {
                    
                    txtDocCliente.Text = modal._Cliente.Documento;
                    txtNombreCliente.Text = modal._Cliente.NombreCompleto;
                    txtCodProd.Focus();
                    
                }
                else
                {
                    txtDocCliente.Select();
                }

            }
        }

        private void btnBuscarProd_Click(object sender, EventArgs e)
        {
            using (var modal = new mdProducto())
            {
                var result = modal.ShowDialog();

                if (result == DialogResult.OK)
                {
                    txtIdProducto.Text = modal.oProducto.IdProducto.ToString();
                    txtCodProducto.Text = modal.oProducto.Codigo;
                    txtProducto.Text = modal.oProducto.Nombre;
                    txtDescripcion.Text = modal.oProducto.Descripcion.ToString();
                    txtPrecio.Text = modal.oProducto.PrecioVenta.ToString("0.00");
                    txtCantidad.Select();
                }
                else
                {
                    txtCodProd.Select();
                }

            }
        }

        private void txtCodProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)//EVENTO ENTER
            {
                Producto oProducto = new CN_Producto().Listar().Where(p => p.Codigo == txtCodProducto.Text && p.Estado == true).FirstOrDefault();
                if (oProducto != null)
                {
                    txtCodProducto.BackColor = Color.LightGreen;
                    txtIdProducto.Text = oProducto.IdProducto.ToString();
                    txtProducto.Text = oProducto.Nombre;
                    txtDescripcion.Text = oProducto.Descripcion;
                    txtPrecio.Text = oProducto.PrecioVenta.ToString("0.00");                   
                    txtCantidad.Select();
                }
                else
                {
                    txtCodProducto.BackColor = Color.OrangeRed;
                    txtIdProducto.Text = "0";
                    txtProducto.Text = String.Empty;
                    txtDescripcion.Text = String.Empty;
                    txtObservaciones.Text = String.Empty;
                    cboConcepto.SelectedIndex = 0;
                    txtPrecio.Text = String.Empty;
                    txtCantidad.Value = 1;
                }
            }
        }

        private void calcularTotal()
        {
            decimal total = 0;
            if (dgvVentas.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvVentas.Rows)
                {
                    total += Convert.ToDecimal(row.Cells["SubTotal"].Value);
                }

                txtTotal.Text = total.ToString("0.00");
            }
        }
        private void limpiarProducto()
        {
            txtIdProd.Text = "0";
            txtCodProducto.Text = "";
            txtCodProducto.BackColor = System.Drawing.Color.White;
            txtProducto.Text = "";
            txtDescripcion.Text = "";
            txtPrecio.Text = "";
            txtCantidad.Text = "1";
            nudCantidad.Value = 1;          
            txtCodProducto.Focus();
        }


        private void dgvVentas_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)//PARA AGREGAR EL BOTON QUITAR
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

                //e.Graphics.DrawImage(Properties.Resources.cancelar, new Rectangle());
                e.Handled = true;

            }
        }

        private void dgvVentas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvVentas.Columns[e.ColumnIndex].Name == "ColAccion")
            {
                int indice = e.RowIndex;

                if (indice >= 0)
                {                 
                    dgvVentas.Rows.RemoveAt(indice);
                    calcularTotal();
                }
                else
                {
                    MessageBox.Show("Debe Seleccionar una Fila para Eliminar","Mensaje",MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void txtPrecio_KeyPress(object sender, KeyPressEventArgs e)
        {//METODO PARA NO PERMITIR INGRESAR LETRAS EN UNA CAJA DE TEXTO
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                if (txtPrecio.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")
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
        private void CalcularCambio() 
        {
            decimal PagaCon;
            decimal Total = Convert.ToDecimal(txtTotal.Text);

            if (txtPagaCon.Text.Trim() == "")
            {
                txtPagaCon.Text = "0";
            }

            if (decimal.TryParse(txtPagaCon.Text.Trim(), out PagaCon))
            {
                if (PagaCon < Total)
                {
                    txtVuelto.Text = "0.00";
                }
                else
                {
                    decimal Vuelto = PagaCon - Total;
                    txtVuelto.Text = Vuelto.ToString("0.00");
                }
            }
        }

        private void txtPagaCon_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                CalcularCambio();
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)//BOTON REGISTRAR VENTA
        {
            if (txtDocCliente.Text == "")
            {
                MessageBox.Show("Debe Ingresar Documento del Cliente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtNombreCliente.Text == "")
            {
                MessageBox.Show("Debe Ingresar Nombre del Cliente", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (dgvVentas.Rows.Count < 1)
            {
                MessageBox.Show("Debe Ingresar Productos a la Venta", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            
            if (cboFormaPago.SelectedIndex == -1)
            {
                MessageBox.Show("Debe Seleccionar una Forma de Pago...", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                cboFormaPago.Focus();
                return;
            }

            if (cboFormaPago.SelectedIndex == 0)
            {
                txtTotal.Text = (Convert.ToDouble(txtTotal.Text) * 0.80).ToString();
            }

            if (cboFormaPago.SelectedIndex == 1)
            {
                txtTotal.Text = (Convert.ToDouble(txtTotal.Text) * 0.90).ToString();
            }

            DataTable Detalle_Venta = new DataTable();
            Detalle_Venta.Columns.Add("IdProducto",typeof(int));
            Detalle_Venta.Columns.Add("PrecioVenta", typeof(decimal));
            Detalle_Venta.Columns.Add("Cantidad", typeof(int));
            Detalle_Venta.Columns.Add("SubTotal", typeof(decimal));

            foreach (DataGridViewRow row in dgvVentas.Rows)
            {
                Detalle_Venta.Rows.Add(new object[]
                {
                    row.Cells["IdProducto"].Value.ToString(),
                    row.Cells["Precio"].Value.ToString(),
                    row.Cells["Cantidad"].Value.ToString(),
                    row.Cells["SubTotal"].Value.ToString()

                });
            }

            int idCorrelativo = new CN_Venta().ObtenerCorrelativo();
            string numeroDocumento = string.Format("{0:0000000}", idCorrelativo);
            CalcularCambio();

            Venta oVenta = new Venta()
            {
                oUsuario = new Usuario() { IdUsuario = _Usuario.IdUsuario, NombreCompleto = _Usuario.NombreCompleto },
                TipoDocumento = ((OpcionCombo)cboTipoDoc.SelectedItem).Texto,
                NroDocumento = numeroDocumento,
                DocumentoCliente = txtDocCliente.Text,
                NombreCliente = txtNombreCliente.Text,
                Alumnos = txtAlumnos.Text,
                Concepto = cboConcepto.Text,
                Observaciones = txtObservaciones.Text,
                MontoPago = Convert.ToDecimal(txtPagaCon.Text),
                MontoCambio = Convert.ToDecimal(txtVuelto.Text),
                MontoTotal = Convert.ToDecimal(txtTotal.Text)

            };

            string Mensaje = string.Empty;

            bool respuesta = new CN_Venta().Registrar(oVenta, Detalle_Venta, out Mensaje);

            if (respuesta)
            {
                var Result = MessageBox.Show("Numero de Venta:\n" + numeroDocumento,"Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (MessageBox.Show("Desea Descargar el Documento?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string Texto_Html = Properties.Resources.PlantillaVenta.ToString();
                    Negocio oDatos = new CN_Negocio().ObtenerDatos();

                    Texto_Html = Texto_Html.Replace("@nombrenegocio", oDatos.NombreNegocio.ToUpper());
                    Texto_Html = Texto_Html.Replace("@docnegocio", oDatos.CUIL);
                    Texto_Html = Texto_Html.Replace("@direcnegocio", oDatos.Direccion);

                    Texto_Html = Texto_Html.Replace("@tipodocumento", oVenta.TipoDocumento.ToUpper());
                    Texto_Html = Texto_Html.Replace("@numerodocumento", oVenta.NroDocumento);

                    Texto_Html = Texto_Html.Replace("@doccliente", txtDocCliente.Text);
                    Texto_Html = Texto_Html.Replace("@nombrecliente", txtNombreCliente.Text);
                    Texto_Html = Texto_Html.Replace("@nombrealumnos", txtAlumnos.Text);
                    Texto_Html = Texto_Html.Replace("@concepto", cboConcepto.Text);
                    Texto_Html = Texto_Html.Replace("@formapago", cboFormaPago.Text);
                    if (txtObservaciones.Text == String.Empty)
                    {
                        Texto_Html = Texto_Html.Replace("@observaciones", "");
                    }
                    else
                    {
                        Texto_Html = Texto_Html.Replace("@observaciones", txtObservaciones.Text);
                    }
                    Texto_Html = Texto_Html.Replace("@fecharegistro", txtFecha.Text);
                    Texto_Html = Texto_Html.Replace("@usuarioregistro", oVenta.oUsuario.NombreCompleto);

                    string filas = string.Empty;
                    foreach (DataGridViewRow row in dgvVentas.Rows)
                    {
                        filas += "<tr>";
                        filas += "<td>" + row.Cells["Producto"].Value.ToString() + "</td>";
                        filas += "<td>" + row.Cells["Descripcion"].Value.ToString() + "</td>";
                        filas += "<td>" + row.Cells["Precio"].Value.ToString() + "</td>";
                        filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "</td>";
                        filas += "<td>" + row.Cells["SubTotal"].Value.ToString() + "</td>";
                        filas += "</tr>";
                    }
                    Texto_Html = Texto_Html.Replace("@filas", filas);
                    Texto_Html = Texto_Html.Replace("@montototal", "$"+txtTotal.Text+".00");
                    Texto_Html = Texto_Html.Replace("@pagocon", "$"+ oVenta.MontoPago.ToString("0.00"));
                    Texto_Html = Texto_Html.Replace("@cambio", "$"+ oVenta.MontoCambio.ToString("0.00"));

                    string nombreRecibo;
                    if (txtAlumnos.Text == string.Empty)
                    {
                        nombreRecibo = txtNombreCliente.Text;
                    }
                    else
                    {
                        nombreRecibo= txtAlumnos.Text;
                    }

                    SaveFileDialog SaveFile = new SaveFileDialog();
                    SaveFile.FileName = string.Format(nombreRecibo+cboConcepto.Text+"Recibo_{0}.pdf", oVenta.NroDocumento);
                    SaveFile.Filter = "Pdf Files|*.pdf";

                    if (SaveFile.ShowDialog() == DialogResult.OK)
                    {
                        using (FileStream stream = new FileStream(SaveFile.FileName, FileMode.Create))
                        {
                            Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);

                            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                            pdfDoc.Open();

                            bool Obtenido = true;
                            byte[] byteImage = new CN_Negocio().ObtenerLogo(out Obtenido);

                            if (Obtenido)
                            {
                                iTextSharp.text.Image Img = iTextSharp.text.Image.GetInstance(byteImage);
                                Img.ScaleToFit(150, 150);
                                Img.Alignment = iTextSharp.text.Image.UNDERLYING;
                                Img.SetAbsolutePosition(pdfDoc.Left, pdfDoc.GetTop(51));
                                pdfDoc.Add(Img);
                            }

                            using (StringReader Sr = new StringReader(Texto_Html))
                            {
                                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, Sr);
                            }

                            pdfDoc.Close();
                            stream.Close();
                            MessageBox.Show("Documento Generado", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            txtDocCliente.Text = "";
                            txtNombreCliente.Text = "";
                            txtAlumnos.Text = "";
                            txtObservaciones.Text = "";
                            txtDescripcion.Text = "";
                            cboConcepto.SelectedIndex = -1;
                            dgvVentas.Rows.Clear();
                            calcularTotal();
                            txtPagaCon.Text = "";
                            txtVuelto.Text = "";
                            txtTotal.Text = "0.00";
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show(Mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void dgvVentas_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvVentas.CurrentCell.ColumnIndex == 6)//PARA REMOVER UN ITEM DE LA DGV
            {             
                dgvVentas.Rows.Remove(dgvVentas.CurrentRow);
                calcularTotal();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            decimal Precio = 0;
            bool productoExiste = false;

            //if (int.Parse(txtIdProducto.Text) == 0)
            //{
            //    MessageBox.Show("Debe Seleccionar un Producto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}

            if (!decimal.TryParse(txtPrecio.Text, out Precio))
            {
                MessageBox.Show("Precio Compra - Formato Moneda Incorrecto", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecioCom.Focus();
                return;
            }

            //if (Convert.ToInt32(txtStock.Text) < Convert.ToInt32(txtCantidad.Value.ToString()))
            //{
            //    MessageBox.Show("La Cantidad no puede ser mayor al stock...", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    txtCantidad.Focus();
            //    return;
            //}

            foreach (DataGridViewRow fila in dgvVentas.Rows)
            {
                if (Convert.ToString(fila.Cells["IdProducto"].Value) == txtIdProducto.Text)
                {
                    productoExiste = true;
                    break;
                }
            }

            if (!productoExiste) //si es falso
            {

                dgvVentas.Rows.Add(new object[]{
                    txtIdProducto.Text,
                    txtProducto.Text,
                    txtDescripcion.Text,
                    Precio.ToString("0.00"),
                    txtCantidad.Value.ToString(),
                    (txtCantidad.Value * Precio).ToString("0.00")

                });
                calcularTotal();
                limpiarProducto();
                txtCodProd.Focus();

            }
        }
    }
}

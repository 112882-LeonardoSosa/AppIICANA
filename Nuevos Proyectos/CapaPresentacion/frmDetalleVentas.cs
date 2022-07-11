using CapaEntidad;
using CapaNegocio;
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
    public partial class frmDetalleVentas : Form
    {
        public frmDetalleVentas()
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Venta oVenta = new CN_Venta().ObtenerVenta(txtBuscar.Text);

            if (oVenta.IdVenta != 0)
            {
                txtnroDoc.Text = oVenta.NroDocumento;

                txtTipoDoc.Text = oVenta.TipoDocumento;
                txtFecha.Text = oVenta.FechaRegistro;
                txtUsuario.Text = oVenta.oUsuario.NombreCompleto;

                txtDocCliente.Text = oVenta.DocumentoCliente;
                txtNombreCliente.Text = oVenta.NombreCliente;
                txtAlumnos.Text = oVenta.Alumnos;
                txtConcepto.Text = oVenta.Concepto;
                txtObservaciones.Text = oVenta.Observaciones;

                dgvVentas.Rows.Clear();
                foreach (Detalle_Venta dv in oVenta.DetallesVentas)
                {
                    dgvVentas.Rows.Add(new object[] {dv.oProducto.Nombre,dv.Descripcion, dv.PrecioVenta, dv.Cantidad, dv.SubTotal });
                }

                txtTotal.Text = oVenta.MontoTotal.ToString("0.00");
                txtPago.Text = oVenta.MontoPago.ToString("0.00");
                txtCambio.Text = oVenta.MontoCambio.ToString("0.00");
            }
        }
        private void Limpiar() 
        {
            txtnroDoc.Text = "";

            txtTipoDoc.Text = "";
            txtFecha.Text = "";
            txtUsuario.Text = "";
            txtAlumnos.Text = "";
            txtObservaciones.Text = "";
            txtConcepto.Text = "";
            txtDocCliente.Text = "";
            txtNombreCliente.Text = "";

            txtTotal.Text = "0.00";
            txtPago.Text = "0.00";
            txtCambio.Text = "0.00";

            txtBuscar.Text = "";
            dgvVentas.Rows.Clear();
        }
        private void btnLimpiarBuscador_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void frmDetalleVentas_Load(object sender, EventArgs e)
        {
            txtBuscar.Focus();
        }

        private void btnDescargar_Click(object sender, EventArgs e)
        {
            string Texto_Html = Properties.Resources.PlantillaVenta.ToString();
            Negocio oDatos = new CN_Negocio().ObtenerDatos();

            Texto_Html = Texto_Html.Replace("@nombrenegocio", oDatos.NombreNegocio.ToUpper());
            Texto_Html = Texto_Html.Replace("@docnegocio", oDatos.CUIL);
            Texto_Html = Texto_Html.Replace("@direcnegocio", oDatos.Direccion);

            Texto_Html = Texto_Html.Replace("@tipodocumento", txtTipoDoc.Text.ToUpper());
            Texto_Html = Texto_Html.Replace("@numerodocumento", txtnroDoc.Text);

            Texto_Html = Texto_Html.Replace("@doccliente", txtDocCliente.Text);
            Texto_Html = Texto_Html.Replace("@nombrecliente", txtNombreCliente.Text);
            Texto_Html = Texto_Html.Replace("@nombrealumnos", txtAlumnos.Text);
            Texto_Html = Texto_Html.Replace("@concepto", txtConcepto.Text);
            Texto_Html = Texto_Html.Replace("@formapago", txtFormaPago.Text);
            if (txtObservaciones.Text == String.Empty)
            {
                Texto_Html = Texto_Html.Replace("@observaciones", "");
            }
            else
            {
                Texto_Html = Texto_Html.Replace("@observaciones", txtObservaciones.Text);
            }
            Texto_Html = Texto_Html.Replace("@fecharegistro", txtFecha.Text);
            Texto_Html = Texto_Html.Replace("@usuarioregistro", txtUsuario.Text);

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
            Texto_Html = Texto_Html.Replace("@montototal", txtTotal.Text);
            Texto_Html = Texto_Html.Replace("@pagocon", txtPago.Text);
            Texto_Html = Texto_Html.Replace("@cambio", txtCambio.Text);

            string nombreRecibo;
            if (txtAlumnos.Text == string.Empty)
            {
                nombreRecibo = txtNombreCliente.Text;
            }
            else
            {
                nombreRecibo = txtAlumnos.Text;
            }

            SaveFileDialog SaveFile = new SaveFileDialog();
            SaveFile.FileName = string.Format(nombreRecibo + txtConcepto.Text+"Recibo_{0}.pdf", txtnroDoc.Text);
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
                    Limpiar();
                }
            }
        }
    }
}

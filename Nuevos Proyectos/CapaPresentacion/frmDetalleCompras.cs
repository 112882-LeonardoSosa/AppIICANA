using CapaEntidad;
using CapaNegocio;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
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
    public partial class frmDetalleCompras : Form
    {
        public frmDetalleCompras()
        {
            InitializeComponent();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Compra oCompra = new CN_Compra().ObtenerCompra(txtBuscar.Text);

            if (oCompra.IdCompra != 0)
            {
                txtNroDoc.Text = oCompra.NroDocumento;
                txtFecha.Text = oCompra.FechaRegistro;
                txtTipoDoc.Text = oCompra.TipoDocumento;
                txtUsuario.Text = oCompra.oUsuario.NombreCompleto;
                txtDocProv.Text = oCompra.oProveedor.Documento;
                txtNomProve.Text = oCompra.oProveedor.RazonSocial;

                dgvData.Rows.Clear();
                foreach (Detalle_Compra dc in oCompra.ListaCompra)
                {
                    dgvData.Rows.Add(new object[] { dc.oProducto.Nombre, dc.PrecioCompra, dc.Cantidad, dc.MontoTotal });
                }
                txtTotal.Text = oCompra.MontoTotal.ToString("0.00");

            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string Texto_Html = Properties.Resources.PlantillaCompra.ToString();
            Negocio oDatos = new CN_Negocio().ObtenerDatos();

            Texto_Html = Texto_Html.Replace("@nombrenegocio", oDatos.NombreNegocio.ToUpper());
            Texto_Html = Texto_Html.Replace("@docnegocio", oDatos.CUIL);
            Texto_Html = Texto_Html.Replace("@direcnegocio", oDatos.Direccion);

            Texto_Html = Texto_Html.Replace("@tipodocumento", txtTipoDoc.Text.ToUpper());
            Texto_Html = Texto_Html.Replace("@numerodocumento", txtNroDoc.Text);

            Texto_Html = Texto_Html.Replace("@docproveedor", txtDocProv.Text);
            Texto_Html = Texto_Html.Replace("@nombreproveedor", txtNomProve.Text);
            Texto_Html = Texto_Html.Replace("@fecharegistro", txtFecha.Text);
            Texto_Html = Texto_Html.Replace("@usuarioregistro", txtUsuario.Text);

            string filas = string.Empty;
            foreach (DataGridViewRow row in dgvData.Rows)
            {
                filas += "<tr>";
                filas += "<td>" + row.Cells["Producto"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["PrecioCompra"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "</td>";
                filas += "<td>" + row.Cells["SubTotal"].Value.ToString() + "</td>";
                filas += "</tr>";
            }
            Texto_Html = Texto_Html.Replace("@filas", filas);
            Texto_Html = Texto_Html.Replace("@montototal", txtTotal.Text);

            SaveFileDialog SaveFile = new SaveFileDialog();
            SaveFile.FileName = string.Format("Compra_{0}.pdf",txtNroDoc.Text);
            SaveFile.Filter = "Pdf Files|*.pdf";

            if (SaveFile.ShowDialog() == DialogResult.OK )
            {
                using (FileStream stream = new FileStream(SaveFile.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4,25,25,25,25);

                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc,stream);
                    pdfDoc.Open();

                    bool Obtenido = true;
                    byte[] byteImage = new CN_Negocio().ObtenerLogo(out Obtenido);

                    if (Obtenido)
                    {
                        iTextSharp.text.Image Img = iTextSharp.text.Image.GetInstance(byteImage);
                        Img.ScaleToFit(60,60);
                        Img.Alignment = iTextSharp.text.Image.UNDERLYING;
                        Img.SetAbsolutePosition(pdfDoc.Left,pdfDoc.GetTop(51));
                        pdfDoc.Add(Img);
                    }

                    using (StringReader Sr = new StringReader(Texto_Html))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, Sr);
                    }

                    pdfDoc.Close();
                    stream.Close();
                    MessageBox.Show("Documento Genetado","Mensaje",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
            }
        }

        private void frmDetalleCompras_Load(object sender, EventArgs e)
        {

        }
    }
}

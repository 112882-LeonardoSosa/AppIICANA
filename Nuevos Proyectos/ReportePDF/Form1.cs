using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.IO;

namespace ReportePDF
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnagregarproducto_Click(object sender, EventArgs e)
        {
            int indiceFila = dgvData.Rows.Add();

            DataGridViewRow Fila = dgvData.Rows[indiceFila];
            Fila.Cells["Cantidad"].Value = txtcantidad.Text;
            Fila.Cells["Descripcion"].Value = txtNomProducto.Text;
            Fila.Cells["PrecioUnitario"].Value = txtPrecioVenta.Text;
            Fila.Cells["Importe"].Value = decimal.Parse(txtcantidad.Text) * decimal.Parse(txtPrecioVenta.Text);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            dgvData.Columns.Add("Cantidad", "Cantidad");
            dgvData.Columns.Add("Descripcion", "Descripcion");
            dgvData.Columns.Add("PrecioUnitario", "Precio Unitario");
            dgvData.Columns.Add("Importe", "Importe");
            
        }

        private void btnterminarventa_Click(object sender, EventArgs e)
        {
            //string pdf = ".pdf";
            SaveFileDialog Guardar = new SaveFileDialog();
            Guardar.FileName = DateTime.Now.ToString("ddMMyyyyHHmmss")+".pdf";
            Guardar.ShowDialog();
            string PaginaHtml_Texto = Properties.Resources.plantilla.ToString();
            PaginaHtml_Texto = PaginaHtml_Texto.Replace("@CLIENTE", txtnombrecliente.Text);
            PaginaHtml_Texto = PaginaHtml_Texto.Replace("@DOCUMENTO", txtdocumentocliente.Text);
            PaginaHtml_Texto = PaginaHtml_Texto.Replace("@FECHA", DateTime.Now.ToString("dd/MM/yyyy"));

            string Filas = string.Empty;
            decimal Total = 0;
            foreach (DataGridViewRow row in dgvData.Rows)
            {
                Filas += "<tr>";
                Filas += "<td>" + row.Cells["Cantidad"].Value.ToString() + "</td>";
                Filas += "<td>" + row.Cells["Descripcion"].Value.ToString() + "</td>";
                Filas += "<td>" + row.Cells["PrecioUnitario"].Value.ToString() + "</td>";
                Filas += "<td>" + row.Cells["Importe"].Value.ToString() + "</td>";
                Filas += "</tr>";
                Total += decimal.Parse(row.Cells["Importe"].Value.ToString());
            }
            PaginaHtml_Texto = PaginaHtml_Texto.Replace("@FILAS", Filas);
            PaginaHtml_Texto = PaginaHtml_Texto.Replace("@TOTAL", Total.ToString());


            if (Guardar.ShowDialog() == DialogResult.OK)
            {
                using (FileStream Stream = new FileStream(Guardar.FileName,FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 25);

                    PdfWriter Writer = PdfWriter.GetInstance(pdfDoc,Stream);

                    pdfDoc.Open();

                    pdfDoc.Add(new Phrase(""));

                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Properties.Resources.logoiicana, System.Drawing.Imaging.ImageFormat.Png);
                    img.ScaleToFit(80, 60);
                    img.Alignment = iTextSharp.text.Image.UNDERLYING;
                    img.SetAbsolutePosition(pdfDoc.LeftMargin,pdfDoc.Top -60);
                    pdfDoc.Add(img);

                    using (StringReader Sr = new StringReader(PaginaHtml_Texto)) 
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(Writer,pdfDoc,Sr);
                    }

                        pdfDoc.Close();

                    Stream.Close();
                }

                    
                
            }
            

        }
    }
}

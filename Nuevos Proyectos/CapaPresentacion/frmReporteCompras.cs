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
    public partial class frmReporteCompras : Form
    {
        public frmReporteCompras()
        {
            InitializeComponent();
        }

        private void frmReporteCompras_Load(object sender, EventArgs e)
        {
            List<Proveedor> lista = new CN_Proveedor().Listar();

            cboProveedor.Items.Add(new OpcionCombo() { Valor = 0, Texto = "TODOS" });

            foreach (Proveedor item in lista)
            {
                cboProveedor.Items.Add(new OpcionCombo() { Valor = item.IdProveedor, Texto = item.RazonSocial });
            }

            cboProveedor.DisplayMember = "Texto";
            cboProveedor.ValueMember = "Valor";
            cboProveedor.SelectedIndex = 0;

            foreach (DataGridViewColumn columna in dgvReporteCompras.Columns)
            {
                               
              cboBuscar.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                
            }
            cboBuscar.DisplayMember = "Texto";
            cboBuscar.ValueMember = "Valor";
            cboBuscar.SelectedIndex = 0;

        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            int idProveedor = Convert.ToInt32(((OpcionCombo)cboProveedor.SelectedItem).Valor.ToString());

            List<ReporteCompra> lista = new List<ReporteCompra>();

            lista = new CN_Reporte().Compra(
                dtpInicio.Value.ToString(),
                dtpFin.Value.ToString(),
                idProveedor
                );

            dgvReporteCompras.Rows.Clear();

            foreach (ReporteCompra rc in lista)
            {
                dgvReporteCompras.Rows.Add(new object[] {
                    rc.FechaRegistro,
                    rc.TipoDocumento,
                    rc.NumeroDocumento,
                    rc.MontoTotal,
                    rc.UsuarioRegistro,
                    rc.DocumentoProveedor,
                    rc.RazonSocial,
                    rc.CodigoProducto,
                    rc.NombreProducto,
                    rc.Categoria,
                    rc.PrecioCompra,
                    rc.PrecioVenta,
                    rc.Cantidad,
                    rc.SubTotal


                });
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (dgvReporteCompras.Rows.Count < 1)
            {
                MessageBox.Show("No hay registros para exportar...", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DataTable dt = new DataTable();
                foreach (DataGridViewColumn Columna in dgvReporteCompras.Columns)
                {
                    if (Columna.HeaderText != String.Empty && Columna.Visible)
                    {
                        dt.Columns.Add(Columna.HeaderText, typeof(string));
                    }
                }
                foreach (DataGridViewRow Row in dgvReporteCompras.Rows)
                {
                    if (Row.Visible)
                    {
                        dt.Rows.Add(new object[] {
                            Row.Cells[0].Value.ToString(),
                            Row.Cells[1].Value.ToString(),
                            Row.Cells[2].Value.ToString(),
                            Row.Cells[3].Value.ToString(),
                            Row.Cells[4].Value.ToString(),
                            Row.Cells[5].Value.ToString(),
                            Row.Cells[6].Value.ToString(),
                            Row.Cells[7].Value.ToString(),
                            Row.Cells[8].Value.ToString(),
                            Row.Cells[9].Value.ToString(),
                            Row.Cells[10].Value.ToString(),
                            Row.Cells[11].Value.ToString(),
                            Row.Cells[12].Value.ToString(),
                            Row.Cells[13].Value.ToString()
                            
                        });
                    }
                }

                SaveFileDialog SaveFile = new SaveFileDialog();
                SaveFile.FileName = string.Format("ReporteCompras_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
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

        private void iconButton1_Click(object sender, EventArgs e)
        {
            string ColumnaFiltro = ((OpcionCombo)cboBuscar.SelectedItem).Valor.ToString();

            if (dgvReporteCompras.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvReporteCompras.Rows)
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
            foreach (DataGridViewRow row in dgvReporteCompras.Rows)
            {
                row.Visible = true;
            }
        }
    }
}

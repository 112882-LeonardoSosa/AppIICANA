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
    public partial class frmReporteVentas : Form
    {
        public frmReporteVentas()
        {
            InitializeComponent();
        }

        private void frmReporteVentas_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn columna in dgvReporteVentas.Columns)
            {

                cboBuscar.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });

            }
            cboBuscar.DisplayMember = "Texto";
            cboBuscar.ValueMember = "Valor";
            cboBuscar.SelectedIndex = 0;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            List<ReporteVenta> lista = new List<ReporteVenta>();

            lista = new CN_Reporte().Venta(dtpInicio.Value.ToString(), dtpFin.Value.ToString());

            dgvReporteVentas.Rows.Clear();

            foreach (ReporteVenta row in lista)
            {
                dgvReporteVentas.Rows.Add(new object[]
                {
                    row.FechaRegistro,
                    row.TipoDocumento,
                    row.NumeroDocumento,
                    row.MontoTotal,
                    row.UsuarioRegistro,
                    row.DocumentoCliente,
                    row.NombreCliente,
                    row.Alumnos,
                    row.CodigoProducto,
                    row.NombreProducto,
                    row.Descripcion,
                    row.Concepto,
                    row.PrecioVenta,
                    row.Cantidad,
                    row.SubTotal
                });
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            string ColumnaFiltro = ((OpcionCombo)cboBuscar.SelectedItem).Valor.ToString();

            if (dgvReporteVentas.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvReporteVentas.Rows)
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
            foreach (DataGridViewRow row in dgvReporteVentas.Rows)
            {
                row.Visible = true;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (dgvReporteVentas.Rows.Count < 1)
            {
                MessageBox.Show("No hay registros para exportar...", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DataTable dt = new DataTable();
                foreach (DataGridViewColumn Columna in dgvReporteVentas.Columns)
                {
                    if (Columna.HeaderText != String.Empty && Columna.Visible)
                    {
                        dt.Columns.Add(Columna.HeaderText, typeof(string));
                    }
                }
                foreach (DataGridViewRow Row in dgvReporteVentas.Rows)
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
                            Row.Cells[13].Value.ToString(),
                            Row.Cells[14].Value.ToString()

                        });
                    }
                }
                
                SaveFileDialog SaveFile = new SaveFileDialog();
                SaveFile.FileName = string.Format("ReporteVentas_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
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

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                string ColumnaFiltro = ((OpcionCombo)cboBuscar.SelectedItem).Valor.ToString();

                if (dgvReporteVentas.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvReporteVentas.Rows)
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
    }   
}

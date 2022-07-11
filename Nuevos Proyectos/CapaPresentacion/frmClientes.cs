using CapaDatos;
using CapaEntidad;
using CapaNegocio;
using CapaPresentacion.Utilidades;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class frmClientes : Form
    {
        public frmClientes()
        {
            InitializeComponent();
        }

        private void frmClientes_Load(object sender, EventArgs e)
        {
            cboEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" });
            cboEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });
            cboEstado.DisplayMember = "Texto";
            cboEstado.ValueMember = "Valor";
            cboEstado.SelectedIndex = 0;


            foreach (DataGridViewColumn columna in dgvClientes.Columns)
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
            List<Cliente> listaCliente = new CN_Cliente().Listar();
            foreach (Cliente item in listaCliente)
            {

                dgvClientes.Rows.Add(new object[] {"",item.IdCliente,item.Documento,item.NombreCompleto,item.FechaNacimiento,
                    item.Domicilio,item.Correo,item.Curso,item.Telefono,item.Sede,
                item.Estado == true ? 1 : 0,
                item.Estado == true ? "Activo" : "No Activo"

            });

            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string Mensaje = string.Empty;
            

            Cliente objCliente = new Cliente()
            {

                IdCliente = Convert.ToInt32(txtId.Text),
                Documento = txtDocumento.Text,
                NombreCompleto = txtNombreCom.Text,
                FechaNacimiento = txtFechaNac.Text,
                Domicilio = txtDomicilio.Text,
                Correo = txtCorreo.Text,
                Curso = cboCurso.Text,
                Telefono = txtDomicilio.Text,
                Sede = cboSede.Text,
                Estado = Convert.ToInt32(((OpcionCombo)cboEstado.SelectedItem).Valor) == 1 ? true : false

            };

            

            if (objCliente.IdCliente == 0)
            {
                int idClienteGenerado = new CN_Cliente().Registrar(objCliente, out Mensaje);

                if (idClienteGenerado != 0)
                {
                    dgvClientes.Rows.Add(new object[] {"",idClienteGenerado,txtDocumento.Text,txtNombreCom.Text,txtFechaNac.Text,txtDomicilio.Text,
                        txtCorreo.Text,cboCurso.Text,txtTelefono.Text,cboSede.Text,
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
                bool resultado = new CN_Cliente().Editar(objCliente, out Mensaje);

                if (resultado)
                {
                    DataGridViewRow Row = dgvClientes.Rows[Convert.ToInt32(txtIndice.Text)];
                    Row.Cells["IdCliente"].Value = txtId.Text;
                    Row.Cells["Documento"].Value = txtDocumento.Text;
                    Row.Cells["NombreCompleto"].Value = txtNombreCom.Text;
                    Row.Cells["FechaNacimiento"].Value = txtFechaNac.Text;
                    Row.Cells["Domicilio"].Value = txtDomicilio.Text;
                    Row.Cells["Correo"].Value = txtCorreo.Text;
                    Row.Cells["Curso"].Value = cboCurso.Text;
                    Row.Cells["Telefono"].Value = txtTelefono.Text;
                    Row.Cells["Sede"].Value = cboSede.Text;
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
            txtNombreCom.Text = String.Empty;
            txtFechaNac.Text = String.Empty;
            txtDomicilio.Text = String.Empty;
            txtCorreo.Text = String.Empty;
            cboCurso.SelectedIndex = -1;
            txtTelefono.Text = String.Empty;
            cboSede.SelectedIndex = -1;
            cboEstado.SelectedIndex = 0;


            txtDocumento.Focus();

        }

        private void dgvClientes_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
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

        private void dgvClientes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvClientes.Columns[e.ColumnIndex].Name == "btnSeleccionar")
            {
                int indice = e.RowIndex;

                if (indice >= 0)
                {
                    txtIndice.Text = indice.ToString();
                    txtId.Text = dgvClientes.Rows[indice].Cells["IdCliente"].Value.ToString();
                    txtDocumento.Text = dgvClientes.Rows[indice].Cells["Documento"].Value.ToString();
                    txtNombreCom.Text = dgvClientes.Rows[indice].Cells["NombreCompleto"].Value.ToString();
                    txtFechaNac.Text = dgvClientes.Rows[indice].Cells["FechaNacimiento"].Value.ToString();
                    txtDomicilio.Text = dgvClientes.Rows[indice].Cells["Domicilio"].Value.ToString();
                    txtCorreo.Text = dgvClientes.Rows[indice].Cells["Correo"].Value.ToString();
                    cboCurso.Text = dgvClientes.Rows[indice].Cells["Curso"].Value.ToString();
                    txtTelefono.Text = dgvClientes.Rows[indice].Cells["Telefono"].Value.ToString();
                    cboSede.Text = dgvClientes.Rows[indice].Cells["Sede"].Value.ToString();


                    foreach (OpcionCombo Op in cboEstado.Items)
                    {
                        if (Convert.ToInt32(Op.Valor) == Convert.ToInt32(dgvClientes.Rows[indice].Cells["EstadoValor"].Value))
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
                if (MessageBox.Show("Desea Eliminar el Alumno?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string Mensaje = string.Empty;
                    Cliente objCliente = new Cliente()
                    { IdCliente = Convert.ToInt32(txtId.Text) };

                    bool Respuesta = new CN_Cliente().Eliminar(objCliente, out Mensaje);

                    if (Respuesta)
                    {
                        dgvClientes.Rows.RemoveAt(Convert.ToInt32(txtIndice.Text));
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

            if (dgvClientes.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgvClientes.Rows)
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
            foreach (DataGridViewRow row in dgvClientes.Rows)
            {
                row.Visible = true;
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnDeudores_Click(object sender, EventArgs e)
        {
            //dgvClientes.Rows.Clear();
            foreach (DataGridViewColumn columna in dgvClientes.Columns)
            {
                if (columna.Visible == true && columna.Name != "btnSeleccionar")
                {
                    cboBuscar.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText });
                }

            }

            List<Cliente> listaCliente = new CN_Cliente().ListarDeudores();
            foreach (Cliente item in listaCliente)
            {

                dgvClientes.Rows.Add(new object[] {"",item.IdCliente,item.Documento,item.NombreCompleto,item.FechaNacimiento,
                    item.Domicilio,item.Correo,item.Curso,item.Telefono,item.Sede,
                item.Estado == true ? 1 : 0,
                item.Estado == true ? "Activo" : "No Activo"

                });

            }
        }

        private void txtBuscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                string ColumnaFiltro = ((OpcionCombo)cboBuscar.SelectedItem).Valor.ToString();

                if (dgvClientes.Rows.Count > 0)
                {
                    foreach (DataGridViewRow row in dgvClientes.Rows)
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

        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (dgvClientes.Rows.Count < 1)
            {
                MessageBox.Show("No hay registros para exportar...", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DataTable dt = new DataTable();
                foreach (DataGridViewColumn Columna in dgvClientes.Columns)
                {
                    if (Columna.HeaderText != String.Empty && Columna.Visible)
                    {
                        dt.Columns.Add(Columna.HeaderText, typeof(string));
                    }
                }
                foreach (DataGridViewRow Row in dgvClientes.Rows)
                {
                    if (Row.Visible)
                    {
                        dt.Rows.Add(new object[] {      
                            
                            Row.Cells[2].Value.ToString(),
                            Row.Cells[3].Value.ToString(),
                            Row.Cells[4].Value.ToString(),
                            Row.Cells[5].Value.ToString(),
                            Row.Cells[6].Value.ToString(),
                            Row.Cells[7].Value.ToString(),
                            Row.Cells[8].Value.ToString(),
                            Row.Cells[9].Value.ToString(),
                            Row.Cells[11].Value.ToString()

                        });
                    }
                }

                SaveFileDialog SaveFile = new SaveFileDialog();
                SaveFile.FileName = string.Format("ReporteAlumnos_{0}.xlsx", DateTime.Now.ToString("ddMMyyyyHHmmss"));
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

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

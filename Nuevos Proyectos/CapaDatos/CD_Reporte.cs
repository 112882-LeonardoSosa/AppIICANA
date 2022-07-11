using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Reporte
    {
        public List<ReporteCompra> Compra(string fechaInicio, string fechaFin, int idProveedor)
        {
            List<ReporteCompra> lista = new List<ReporteCompra>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {

                    StringBuilder query = new StringBuilder();
                    SqlCommand cmd = new SqlCommand("SP_ReporteCompras", conexion);
                    cmd.Parameters.AddWithValue("@fechainicio",fechaInicio);
                    cmd.Parameters.AddWithValue("@fechafin",fechaFin);
                    cmd.Parameters.AddWithValue("@idproveedor",idProveedor);

                    cmd.CommandType = CommandType.StoredProcedure;

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new ReporteCompra()
                            {
                                FechaRegistro = reader["FechaRegistro"].ToString(),
                                TipoDocumento = reader["TipoDocumento"].ToString(),
                                NumeroDocumento = reader["NumeroDocumento"].ToString(),
                                MontoTotal = reader["MontoTotal"].ToString(),
                                UsuarioRegistro = reader["UsuarioRegistro"].ToString(),
                                DocumentoProveedor = reader["DocumentoProveedor"].ToString(),
                                RazonSocial = reader["RazonSocial"].ToString(),
                                CodigoProducto = reader["CodigoProducto"].ToString(),
                                NombreProducto = reader["NombreProducto"].ToString(),
                                Categoria = reader["Categoria"].ToString(),
                                PrecioCompra = reader["PrecioCompra"].ToString(),
                                PrecioVenta = reader["PrecioVenta"].ToString(),
                                Cantidad = reader["Cantidad"].ToString(),
                                SubTotal = reader["SubTotal"].ToString()


                            });
                        }
                    }
                }
                catch (Exception)
                {

                    lista = new List<ReporteCompra>();
                }
            }
            return lista;
        }

        public List<ReporteVenta> Venta(string fechaInicio, string fechaFin)
        {
            List<ReporteVenta> lista = new List<ReporteVenta>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {

                    StringBuilder query = new StringBuilder();
                    SqlCommand cmd = new SqlCommand("sp_ReporteVentas", conexion);
                    cmd.Parameters.AddWithValue("@fechainicio", fechaInicio);
                    cmd.Parameters.AddWithValue("@fechafin", fechaFin);
                    

                    cmd.CommandType = CommandType.StoredProcedure;

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new ReporteVenta()
                            {
                                FechaRegistro = reader["FechaRegistro"].ToString(),
                                TipoDocumento = reader["TipoDocumento"].ToString(),
                                NumeroDocumento = reader["NumeroDocumento"].ToString(),
                                MontoTotal = reader["MontoTotal"].ToString(),
                                UsuarioRegistro = reader["UsuarioRegistro"].ToString(),
                                DocumentoCliente = reader["DocumentoCliente"].ToString(),
                                NombreCliente = reader["NombreCliente"].ToString(),
                                Alumnos = reader["Alumnos"].ToString(),
                                CodigoProducto = reader["CodigoProducto"].ToString(),
                                NombreProducto = reader["NombreProducto"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                Concepto = reader["Concepto"].ToString(),
                                PrecioVenta = reader["PrecioVenta"].ToString(),
                                Cantidad = reader["Cantidad"].ToString(),
                                SubTotal = reader["SubTotal"].ToString()


                            });
                        }
                    }
                }
                catch (Exception)
                {

                    lista = new List<ReporteVenta>();
                }
            }
            return lista;
        }

    }
}

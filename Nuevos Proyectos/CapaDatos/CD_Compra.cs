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
    public class CD_Compra
    {
        public int ObtenerCorrelativo()
        {
            int idCorrelativo = 0;
            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT COUNT(*) + 1 FROM COMPRA");


                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.CommandType = CommandType.Text;

                    conexion.Open();
                    idCorrelativo = Convert.ToInt32(cmd.ExecuteScalar());

                }
                catch (Exception ex)
                {

                    idCorrelativo = 0;
                }

                return idCorrelativo;
            }
        }

        public bool Registrar(Compra obj, DataTable DetalleCompra, out string Mensaje)
        {
            bool Respuesta = false;
            Mensaje = string.Empty;

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SP_RegistrarCompra", conexion);
                    cmd.Parameters.AddWithValue("IdUsuario", obj.oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("IdProveedor", obj.oProveedor.IdProveedor);
                    cmd.Parameters.AddWithValue("TipoDocumento", obj.TipoDocumento);
                    cmd.Parameters.AddWithValue("NumeroDocumento", obj.NroDocumento);
                    cmd.Parameters.AddWithValue("MontoTotal", obj.MontoTotal);
                    cmd.Parameters.AddWithValue("DetalleCompra", DetalleCompra);
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.ExecuteNonQuery();


                    Respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                }
                catch (Exception ex)
                {
                    Respuesta = false;
                    Mensaje = ex.Message;

                }

                return Respuesta;
            }
        }
        public Compra ObtenerCompra(string Numero)
        {
            Compra obj = new Compra();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select c.IdCompra, u.NombreCompleto, pr.Documento,pr.RazonSocial,c.TipoDocumento,c.NumeroDocumento,");
                    query.AppendLine("c.MontoTotal,convert(char(10),c.FechaRegistro,103)[FechaRegistro]");
                    query.AppendLine("from COMPRA c");
                    query.AppendLine("join USUARIO u on u.IdUsuario = c.IdUsuario");
                    query.AppendLine("join PROVEEDOR pr on pr.IdProveedor = c.IdProveedor");
                    query.AppendLine("join PROVEEDOR pr on pr.IdProveedor = c.IdProveedor");
                    query.AppendLine("where c.NumeroDocumento = @numero");




                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@numero", Numero);
                    cmd.CommandType = CommandType.Text;

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obj = new Compra()
                            {
                                IdCompra = Convert.ToInt32(reader["IdCompra"]),
                                oUsuario = new Usuario() { NombreCompleto = reader["NombreCompleto"].ToString() },
                                oProveedor = new Proveedor() { Documento = reader["Documento"].ToString(), RazonSocial = reader["RazonSocial"].ToString() },
                                TipoDocumento = reader["TipoDocumento"].ToString(),
                                NroDocumento = reader["NumeroDocumento"].ToString(),
                                MontoTotal = Convert.ToDecimal(reader["MontoTotal"].ToString()),
                                FechaRegistro = reader["FechaRegistro"].ToString()

                            };

                        }
                    }
                }
                catch (Exception ex)
                {

                    obj = new Compra();
                }
            }

            return obj;
        }

        public List<Detalle_Compra> ObtenerDetalleCompra(int idCompra)
        {
            List<Detalle_Compra> Lista = new List<Detalle_Compra>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {

                    conexion.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select p.Nombre,dc.PrecioCompra,dc.Cantidad, dc.Total");
                    query.AppendLine("from DETALLE_COMPRA dc");
                    query.AppendLine("join PRODUCTO p on p.IdProducto = dc.IdProducto");
                    query.AppendLine("where dc.IdCompra = @idCompra");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@idCompra", idCompra);
                    cmd.CommandType = CommandType.Text;



                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Lista.Add(new Detalle_Compra()
                            {
                                oProducto = new Producto() { Nombre = reader["Nombre"].ToString() },
                                PrecioCompra = Convert.ToDecimal(reader["PrecioCompra"].ToString()),
                                Cantidad = Convert.ToInt32(reader["Cantidad"].ToString()),
                                MontoTotal = Convert.ToDecimal(reader["Total"].ToString())

                            });

                        }
                    }
                }
            }
            catch (Exception)
            {

                Lista = new List<Detalle_Compra>();
            }
            return Lista;
        }
    }
}
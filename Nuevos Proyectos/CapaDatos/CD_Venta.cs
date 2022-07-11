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
    public class CD_Venta
    {
        public int ObtenerCorrelativo()
        {
            int idCorrelativo = 0;
            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("SELECT COUNT(*) + 1 FROM VENTA");


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

        public bool RestarStock(int idproducto, int cantidad)
        {
            bool respuesta = true;

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("Update Producto set stock = stock - @cantidad where idproducto = @idproducto");


                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@idproducto", idproducto);
                    cmd.CommandType = CommandType.Text;

                    conexion.Open();

                    respuesta = cmd.ExecuteNonQuery() > 0 ? true : false;

                }
                catch (Exception ex)
                {

                    respuesta = false;
                }

                return respuesta;
            }
        }

        public bool SumarStock(int idproducto, int cantidad)
        {
            bool respuesta = true;

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("Update Producto set stock = stock + @cantidad where idproducto = @idproducto");


                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@idproducto", idproducto);
                    cmd.CommandType = CommandType.Text;

                    conexion.Open();

                    respuesta = cmd.ExecuteNonQuery() > 0 ? true : false;

                }
                catch (Exception ex)
                {

                    respuesta = false;
                }

                return respuesta;
            }
        }

        public bool Registrar(Venta obj, DataTable DetalleVenta, out string Mensaje)
        {
            bool Respuesta = false;
            Mensaje = string.Empty;

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarVenta", conexion);
                    cmd.Parameters.AddWithValue("IdUsuario", obj.oUsuario.IdUsuario);
                    cmd.Parameters.AddWithValue("TipoDocumento", obj.TipoDocumento);
                    cmd.Parameters.AddWithValue("NumeroDocumento", obj.NroDocumento);
                    cmd.Parameters.AddWithValue("DocumentoCliente", obj.DocumentoCliente);
                    cmd.Parameters.AddWithValue("NombreCliente", obj.NombreCliente);
                    cmd.Parameters.AddWithValue("Alumnos", obj.Alumnos);
                    cmd.Parameters.AddWithValue("Concepto", obj.Concepto);
                    cmd.Parameters.AddWithValue("Observaciones", obj.Observaciones);
                    cmd.Parameters.AddWithValue("FormaPago",obj.FormaPago);
                    cmd.Parameters.AddWithValue("MontoPago", obj.MontoPago);
                    cmd.Parameters.AddWithValue("MontoCambio", obj.MontoCambio);
                    cmd.Parameters.AddWithValue("MontoTotal", obj.MontoTotal);
                    cmd.Parameters.AddWithValue("DetalleVenta", DetalleVenta);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
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

        public Venta ObtenerVenta(string Numero)
        {
            Venta obj = new Venta();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select v.IdVenta,u.NombreCompleto,v.DocumentoCliente,v.NombreCliente,v.Alumnos,v.Concepto,v.Observaciones,v.FormaPago");
                    query.AppendLine("v.TipoDocumento,v.NumeroDocumento,v.MontoPago,v.MontoCambio,v.MontoTotal,");
                    query.AppendLine("convert(char(10),v.FechaRegistro,103)[FechaRegistro]");
                    query.AppendLine("from VENTA v");
                    query.AppendLine("join USUARIO u on u.IdUsuario = v.IdUsuario");
                    query.AppendLine("where v.NumeroDocumento = @numero");


                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@numero", Numero);
                    cmd.CommandType = CommandType.Text;

                    

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            obj = new Venta()
                            {
                                IdVenta = Convert.ToInt32(reader["IdVenta"]),
                                oUsuario = new Usuario() { NombreCompleto = reader["NombreCompleto"].ToString() },
                                DocumentoCliente = reader["DocumentoCliente"].ToString(),
                                NombreCliente = reader["NombreCliente"].ToString(),
                                Alumnos = reader["Alumnos"].ToString(),
                                Concepto = reader["Concepto"].ToString(),
                                Observaciones = reader["Observaciones"].ToString(),
                                FormaPago = reader["FormaPago"].ToString(),
                                TipoDocumento = reader["TipoDocumento"].ToString(),
                                NroDocumento = reader["NumeroDocumento"].ToString(),
                                MontoPago = Convert.ToDecimal(reader["MontoPago"].ToString()),
                                MontoCambio = Convert.ToDecimal(reader["MontoCambio"].ToString()),
                                MontoTotal = Convert.ToDecimal(reader["MontoTotal"].ToString()),
                                FechaRegistro = reader["FechaRegistro"].ToString()

                            };

                        }
                    }
                }
                catch (Exception ex)
                {

                    obj = new Venta();
                }

                return obj;
            }
        }

        public List<Detalle_Venta> ObtenerDetalleVenta(int idVenta) 
        {
            List<Detalle_Venta> oLista = new List<Detalle_Venta>();
            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    conexion.Open();
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select p.Nombre,p.Descripcion,dv.PrecioVenta,dv.Cantidad,dv.SubTotal");
                    query.AppendLine("from DETALLE_VENTA dv");
                    query.AppendLine("join PRODUCTO p on p.IdProducto = dv.IdProducto");
                    query.AppendLine("where dv.IdVenta = @idventa");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@idventa", idVenta);
                    cmd.CommandType = CommandType.Text;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            oLista.Add(new Detalle_Venta()
                            {
                                oProducto = new Producto() { Nombre = reader["Nombre"].ToString()},
                                Descripcion = reader["Descripcion"].ToString(),
                                PrecioVenta = Convert.ToDecimal(reader["PrecioVenta"].ToString()),
                                Cantidad = Convert.ToInt32(reader["Cantidad"].ToString()),
                                SubTotal = Convert.ToDecimal(reader["SubTotal"].ToString()),
                                
                            });

                        }
                    }

                }
                catch (Exception)
                {

                    throw;
                }
            }

            return oLista;
        }
    }
}


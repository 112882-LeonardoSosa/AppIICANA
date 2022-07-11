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
    public class CD_Producto
    {
        public List<Producto> Listar()
        {
            List<Producto> lista = new List<Producto>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {

                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select IdProducto, Codigo, Nombre, p.Descripcion,c.IdCategoria,c.Descripcion[DescripcionCategoria],p.Precio,p.Estado");
                    query.AppendLine("from PRODUCTO p join CATEGORIA c on p.IdCategoria = c.IdCategoria");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.CommandType = CommandType.Text;

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Producto()
                            {
                                IdProducto = Convert.ToInt32(reader["IdProducto"]),
                                Codigo = reader["Codigo"].ToString(),
                                Nombre = reader["Nombre"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                oCategoria = new Categoria() { IdCategoria = Convert.ToInt32(reader["IdCategoria"]), Descripcion = 
                                reader["DescripcionCategoria"].ToString()},
                                PrecioVenta = Convert.ToDecimal(reader["Precio"]),
                                Estado = Convert.ToBoolean(reader["Estado"]),                             

                            });
                        }
                    }
                }
                catch (Exception)
                {

                    lista = new List<Producto>();
                }
            }
            return lista;
        }

        public int Registrar(Producto ob, out string Mensaje)
        {
            int idProductoGenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarProducto", conexion);
                    //PARAMETROS DE ENTRADA
                    cmd.Parameters.AddWithValue("Codigo", ob.Codigo);
                    cmd.Parameters.AddWithValue("Nombre", ob.Nombre);
                    cmd.Parameters.AddWithValue("Descripcion", ob.Descripcion);
                    cmd.Parameters.AddWithValue("IdCategoria", ob.oCategoria.IdCategoria);
                    cmd.Parameters.AddWithValue("Precio",ob.PrecioVenta);
                    cmd.Parameters.AddWithValue("Estado", ob.Estado);
                    //PARAMETROS DE SALIDA
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.ExecuteNonQuery();

                    idProductoGenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {

                idProductoGenerado = 0;
                Mensaje = ex.Message;
            }

            return idProductoGenerado;
        }

        public bool Editar(Producto ob, out string Mensaje)
        {
            bool Respuesta = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_ModificarProducto", conexion);
                    //PARAMETROS DE ENTRADA
                    cmd.Parameters.AddWithValue("IdProducto", ob.IdProducto);
                    cmd.Parameters.AddWithValue("Codigo", ob.Codigo);
                    cmd.Parameters.AddWithValue("Nombre", ob.Nombre);
                    cmd.Parameters.AddWithValue("Descripcion", ob.Descripcion);
                    cmd.Parameters.AddWithValue("IdCategoria", ob.oCategoria.IdCategoria);
                    cmd.Parameters.AddWithValue("Precio", ob.PrecioVenta);
                    cmd.Parameters.AddWithValue("Estado", ob.Estado);
                    //PARAMETROS DE SALIDA
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.ExecuteNonQuery();
                    
                    Respuesta = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = Convert.ToString(cmd.Parameters["Mensaje"].Value);
                }
            }
            catch (Exception ex)
            {

                Respuesta = false;
                Mensaje = ex.Message;
            }

            return Respuesta;
        }

        public bool Eliminar(Producto ob, out string Mensaje)
        {
            bool Respuesta = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_EliminarProducto", conexion);
                    //PARAMETROS DE ENTRADA
                    cmd.Parameters.AddWithValue("IdProducto", ob.IdProducto);
                    //PARAMETROS DE SALIDA
                    cmd.Parameters.Add("Respuesta", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.ExecuteNonQuery();

                    Respuesta = Convert.ToBoolean(cmd.Parameters["Respuesta"].Value);
                    Mensaje = Convert.ToString(cmd.Parameters["Mensaje"].Value);
                }
            }
            catch (Exception ex)
            {

                Respuesta = false;
                Mensaje = ex.Message;
            }

            return Respuesta;
        }
    }
}

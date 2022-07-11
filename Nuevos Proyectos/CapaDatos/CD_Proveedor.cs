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
    public class CD_Proveedor
    {
        public List<Proveedor> Listar()
        {
            List<Proveedor> lista = new List<Proveedor>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select IdProveedor, Documento, RazonSocial, Correo, Telefono, Estado");
                    query.AppendLine("from PROVEEDOR");

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.CommandType = CommandType.Text;

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Proveedor()
                            {
                                IdProveedor = Convert.ToInt32(reader["IdProveedor"]),
                                Documento = reader["Documento"].ToString(),
                                RazonSocial = reader["RazonSocial"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                Estado = Convert.ToBoolean(reader["Estado"]),

                            });
                        }
                    }
                }
                catch (Exception)
                {

                    lista = new List<Proveedor>();
                }
            }
            return lista;
        }

        public int Registrar(Proveedor ob, out string Mensaje)
        {
            int idProveedorGenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_REGISTRAR_PROVEEDOR", conexion);
                    //PARAMETROS DE ENTRADA
                    cmd.Parameters.AddWithValue("Documento", ob.Documento);
                    cmd.Parameters.AddWithValue("RazonSocial", ob.RazonSocial);
                    cmd.Parameters.AddWithValue("Correo", ob.Correo);
                    cmd.Parameters.AddWithValue("Telefono", ob.Telefono);
                    cmd.Parameters.AddWithValue("Estado", ob.Estado);
                    //PARAMETROS DE SALIDA
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.ExecuteNonQuery();

                    idProveedorGenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {

                idProveedorGenerado = 0;
                Mensaje = ex.Message;
            }

            return idProveedorGenerado;
        }

        public bool Editar(Proveedor ob, out string Mensaje)
        {
            bool Respuesta = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_MODIFICAR_PROVEEDOR", conexion);
                    //PARAMETROS DE ENTRADA
                    cmd.Parameters.AddWithValue("IdProveedor", ob.IdProveedor);
                    cmd.Parameters.AddWithValue("Documento", ob.Documento);
                    cmd.Parameters.AddWithValue("RazonSocial", ob.RazonSocial);
                    cmd.Parameters.AddWithValue("Correo", ob.Correo);
                    cmd.Parameters.AddWithValue("Telefono", ob.Telefono);                 
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

        public bool Eliminar(Proveedor ob, out string Mensaje)
        {
            bool Respuesta = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_ELIMINAR_PROVEEDOR", conexion);
                    //PARAMETROS DE ENTRADA
                    cmd.Parameters.AddWithValue("IdProveedor", ob.IdProveedor);
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

    }
}


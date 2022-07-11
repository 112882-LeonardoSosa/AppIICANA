using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using CapaEntidad;

namespace CapaDatos
{
    public class CD_Usuario
    {
        public List<Usuario> Listar() 
        {
            List<Usuario> lista = new List<Usuario>();

            using(SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select u.IdUsuario, u.Documento,u.NombreCompleto, u.Correo, u.Clave, u.Estado, r.IdRol,r.Descripcion from USUARIO u");
                    query.AppendLine("inner join ROL r on u.IdRol = r.IdRol");
                    
                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.CommandType = CommandType.Text;

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader()) {
                        while (reader.Read())
                        {
                            lista.Add(new Usuario()
                            {
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                Documento = reader["Documento"].ToString(),
                                NombreCompleto = reader["NombreCompleto"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                Clave = reader["Clave"].ToString(),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                oRol = new Rol() { IdRol= Convert.ToInt32(reader["IdRol"]), Descripcion= reader["Descripcion"].ToString() }

                            });
                        }
                    }
                }
                catch (Exception)
                {

                    lista = new List<Usuario>();
                }
            }
            return lista;
        }

        public int Registrar(Usuario ob, out string Mensaje) 
        {
            int idUsuarioGenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_REGISTRARUSUARIO", conexion);
                    //PARAMETROS DE ENTRADA
                    cmd.Parameters.AddWithValue("Documento", ob.Documento);
                    cmd.Parameters.AddWithValue("NombreCompleto", ob.NombreCompleto);
                    cmd.Parameters.AddWithValue("Correo", ob.Correo);
                    cmd.Parameters.AddWithValue("Clave", ob.Clave);
                    cmd.Parameters.AddWithValue("IdRol", ob.oRol.IdRol);
                    cmd.Parameters.AddWithValue("Estado", ob.Estado);
                    //PARAMETROS DE SALIDA
                    cmd.Parameters.Add("IdUsuarioResultado",SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar,500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.ExecuteNonQuery();

                    idUsuarioGenerado = Convert.ToInt32(cmd.Parameters["IdUsuarioResultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {

                idUsuarioGenerado = 0;
                Mensaje = ex.Message;
            }

            return idUsuarioGenerado;
        }

        public bool Editar(Usuario ob, out string Mensaje)
        {
            bool Respuesta = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_EDITARUSUARIO", conexion);
                    //PARAMETROS DE ENTRADA
                    cmd.Parameters.AddWithValue("IdUsuario", ob.IdUsuario);
                    cmd.Parameters.AddWithValue("Documento", ob.Documento);
                    cmd.Parameters.AddWithValue("NombreCompleto", ob.NombreCompleto);
                    cmd.Parameters.AddWithValue("Correo", ob.Correo);
                    cmd.Parameters.AddWithValue("Clave", ob.Clave);
                    cmd.Parameters.AddWithValue("IdRol", ob.oRol.IdRol);
                    cmd.Parameters.AddWithValue("Estado", ob.Estado);
                    //PARAMETROS DE SALIDA
                    cmd.Parameters.Add("Respuesta", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar,500).Direction = ParameterDirection.Output;

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

        public bool Eliminar(Usuario ob, out string Mensaje)
        {
            bool Respuesta = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("SP_ELIMINARUSUARIO", conexion);
                    //PARAMETROS DE ENTRADA
                    cmd.Parameters.AddWithValue("IdUsuario", ob.IdUsuario);           
                    //PARAMETROS DE SALIDA
                    cmd.Parameters.Add("Respuesta", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar,500).Direction = ParameterDirection.Output;

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

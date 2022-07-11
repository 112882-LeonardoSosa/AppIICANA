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
    public class CD_Cliente
    {
        public List<Cliente> Listar()
        {
            List<Cliente> lista = new List<Cliente>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select IdCliente,Documento,NombreCompleto,FechaNacimiento,Domicilio,Correo,Curso,Telefono,Sede,Estado from Cliente");
                    

                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.CommandType = CommandType.Text;

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Cliente()
                            {
                                IdCliente = Convert.ToInt32(reader["IdCliente"]),
                                Documento = reader["Documento"].ToString(),
                                NombreCompleto = reader["NombreCompleto"].ToString(),
                                FechaNacimiento = reader["FechaNacimiento"].ToString(),
                                Domicilio = reader["Domicilio"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                Curso = reader["Curso"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                Sede = reader["Sede"].ToString(),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                
                            });
                        }
                    }
                }
                catch (Exception)
                {

                    lista = new List<Cliente>();
                }
            }
            return lista;
        }
        public List<Cliente> ListarDeudores()
        {
            List<Cliente> lista = new List<Cliente>();

            using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select distinct(IdCliente),c.Documento,c.NombreCompleto,c.FechaNacimiento,c.Domicilio,c.Correo,");
                    query.AppendLine("c.Curso,c.Telefono,c.Sede,c.Estado");
                    query.AppendLine("from VENTA v");
                    query.AppendLine("join CLIENTE c on v.DocumentoCliente = c.Documento");
                    query.AppendLine("where month(v.FechaRegistro) < month(getdate())");


                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.CommandType = CommandType.Text;

                    conexion.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Cliente()
                            {
                                IdCliente = Convert.ToInt32(reader["IdCliente"]),
                                Documento = reader["Documento"].ToString(),
                                NombreCompleto = reader["NombreCompleto"].ToString(),
                                FechaNacimiento = reader["FechaNacimiento"].ToString(),
                                Domicilio = reader["Domicilio"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                Curso = reader["Curso"].ToString(),
                                Telefono = reader["Telefono"].ToString(),
                                Sede = reader["Sede"].ToString(),
                                Estado = Convert.ToBoolean(reader["Estado"]),

                            });
                        }
                    }
                }
                catch (Exception)
                {

                    lista = new List<Cliente>();
                }
            }
            return lista;
        }

        public int Registrar(Cliente ob, out string Mensaje)
        {
            int idClienteGenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_RegistrarCliente", conexion);
                    //PARAMETROS DE ENTRADA
                    cmd.Parameters.AddWithValue("Documento", ob.Documento);
                    cmd.Parameters.AddWithValue("NombreCompleto", ob.NombreCompleto);
                    cmd.Parameters.AddWithValue("FechaNacimiento", ob.FechaNacimiento);
                    cmd.Parameters.AddWithValue("Domicilio", ob.Domicilio);
                    cmd.Parameters.AddWithValue("Correo", ob.Correo);
                    cmd.Parameters.AddWithValue("Curso", ob.Curso);
                    cmd.Parameters.AddWithValue("Telefono", ob.Telefono);
                    cmd.Parameters.AddWithValue("Sede", ob.Sede);
                    cmd.Parameters.AddWithValue("Estado", ob.Estado);
                    //PARAMETROS DE SALIDA
                    cmd.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    cmd.CommandType = CommandType.StoredProcedure;
                    conexion.Open();
                    cmd.ExecuteNonQuery();

                    idClienteGenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {

                idClienteGenerado = 0;
                Mensaje = ex.Message;
            }

            return idClienteGenerado;
        }

        public bool Editar(Cliente ob, out string Mensaje)
        {
            bool Respuesta = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("sp_ModificarCliente", conexion);
                    //PARAMETROS DE ENTRADA
                    cmd.Parameters.AddWithValue("IdCliente", ob.IdCliente);
                    cmd.Parameters.AddWithValue("Documento", ob.Documento);
                    cmd.Parameters.AddWithValue("NombreCompleto", ob.NombreCompleto);
                    cmd.Parameters.AddWithValue("FechaNacimiento", ob.FechaNacimiento);
                    cmd.Parameters.AddWithValue("Domicilio", ob.Domicilio);
                    cmd.Parameters.AddWithValue("Correo", ob.Correo);
                    cmd.Parameters.AddWithValue("Curso", ob.Curso);
                    cmd.Parameters.AddWithValue("Telefono", ob.Telefono);
                    cmd.Parameters.AddWithValue("Sede", ob.Sede);
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

        public bool Eliminar(Cliente ob, out string Mensaje)
        {
            bool Respuesta = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    SqlCommand cmd = new SqlCommand("Delete from Cliente where IdCliente = @id", conexion);
                    //PARAMETROS DE ENTRADA
                    cmd.Parameters.AddWithValue("@id", ob.IdCliente);

                    conexion.Open();
                    Respuesta = cmd.ExecuteNonQuery() > 0 ? true : false;

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

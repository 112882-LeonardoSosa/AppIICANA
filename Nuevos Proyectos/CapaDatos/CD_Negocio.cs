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
    public class CD_Negocio
    {
        public Negocio ObtenerDatos()
        {
            Negocio objNegocio = new Negocio();

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();

                    string query = "Select IdNegocio, Nombre, CUIL, Direccion from NEGOCIO where IdNegocio = 1";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            objNegocio = new Negocio()
                            {
                                IdNegocio = Convert.ToInt32(dr["IdNegocio"].ToString()),
                                NombreNegocio = dr["Nombre"].ToString(),
                                CUIL = dr["CUIL"].ToString(),
                                Direccion = dr["Direccion"].ToString()
                            };
                        }
                    }
                }
            }
            catch
            {

                objNegocio = new Negocio();
            }


            return objNegocio;
        }

        public bool GuardarDatos(Negocio objNegocio, out string Mensaje)
        {
            Mensaje = string.Empty;
            bool respuesta = true;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();

                    StringBuilder query = new StringBuilder();
                    query.AppendLine("Update NEGOCIO set Nombre = @Nombre,");
                    query.AppendLine("CUIL = @CUIL,");
                    query.AppendLine("Direccion = @Direccion");
                    query.AppendLine("Where IdNegocio = 1");
                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@nombre", objNegocio.NombreNegocio);
                    cmd.Parameters.AddWithValue("@CUIL", objNegocio.CUIL);
                    cmd.Parameters.AddWithValue("@Direccion", objNegocio.Direccion);
                    cmd.CommandType = CommandType.Text;

                    if (cmd.ExecuteNonQuery() < 1)
                    {
                        Mensaje = "No se pudo Guardar los Datos Correctamente...";
                        respuesta = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                respuesta = false;

            }
            return respuesta;
        }
        public byte[] ObtenerLogo(out bool Obtenido) //METODO PARA CARGAR LOGO
        {
            Obtenido = true;
            byte[] LogoBytes = new byte[0];

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();

                    string query = "Select Logo from NEGOCIO where IdNegocio = 1";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            LogoBytes = (byte[])dr["Logo"];
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                Obtenido=false;
                LogoBytes = new byte[0];
            }

            return LogoBytes;
        }

        public bool ActualizarLogo(byte[] Image,out string Mensaje) 
        {
            Mensaje = string.Empty;
            bool respuesta = true;

            try
            {
                using (SqlConnection conexion = new SqlConnection(Conexion.cadena))
                {
                    conexion.Open();

                    StringBuilder query = new StringBuilder();
                    query.AppendLine("Update NEGOCIO set Logo = @Imagen");
                    query.AppendLine("Where IdNegocio = 1");
                    SqlCommand cmd = new SqlCommand(query.ToString(), conexion);
                    cmd.Parameters.AddWithValue("@Imagen", Image);
                    cmd.CommandType = CommandType.Text;

                    if (cmd.ExecuteNonQuery() < 1)
                    {
                        Mensaje = "No se pudo Actualizar el Logo...";
                        respuesta = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;
                respuesta = false;

            }

            return respuesta;
        }
    }
}

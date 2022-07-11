using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Usuario
    {
        private CD_Usuario objCD_Usuario = new CD_Usuario();

        public List<Usuario> Listar() 
        {
            return objCD_Usuario.Listar();
        }

        public int Registrar(Usuario obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.Documento == "")
            {
                Mensaje += "Debe Ingresar el DNI del Usuario\n";
            }
            if (obj.NombreCompleto == "")
            {
                Mensaje += "Debe Ingresar el nombre del Usuario\n";
            }
            if (obj.Clave == "")
            {
                Mensaje += "Debe Ingresar la Clave del Usuario\n";
            }
            if (Mensaje != string.Empty)
            {
                return 0;
            }
            else
            {
                return objCD_Usuario.Registrar(obj, out Mensaje);
            }
        }

        public bool Editar(Usuario obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.NombreCompleto == "")
            {
                Mensaje += "Debe Ingresar el nombre del Usuario\n";
            }
            if (obj.Documento == "")
            {
                Mensaje += "Debe Ingresar el DNI del Usuario\n";
            }
            if (obj.Correo == "")
            {
                Mensaje += "Debe Ingresar el Email del Usuario\n";
            }
            if (obj.Clave == "")
            {
                Mensaje += "Debe Ingresar la Clave del Usuario\n";
            }
            if (Mensaje != string.Empty)
            {
                return false;
            }
            else
            {         
                return objCD_Usuario.Editar(obj, out Mensaje);
            }
        }

        public bool Eliminar(Usuario obj, out string Mensaje)
        {
            return objCD_Usuario.Eliminar(obj, out Mensaje);
        }
    }
}

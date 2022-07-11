using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Cliente
    {
        private CD_Cliente objCD_Cliente = new CD_Cliente();

        public List<Cliente> Listar()
        {
            return objCD_Cliente.Listar();
        }
        public List<Cliente> ListarDeudores()
        {
            return objCD_Cliente.ListarDeudores();
        }

        public int Registrar(Cliente obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.Documento == "")
            {
                Mensaje += "Debe Ingresar el DNI del Alumno\n";
            }
            if (obj.NombreCompleto == "")
            {
                Mensaje += "Debe Ingresar el Nombre del Alumno\n";
            }
            if (obj.Telefono == "")
            {
                Mensaje += "Debe Ingresar el Telefono del Alumno\n";
            }
            if (obj.Domicilio == "")
            {
                Mensaje += "Debe Ingresar el Domicilio del Alumno\n";
            }
            if (obj.FechaNacimiento == "")
            {
                Mensaje += "Debe Ingresar la fecha de Nacimiento del Alumno\n";
            }
            if (obj.Curso == "")
            {
                Mensaje += "Debe Ingresar el Curso del Alumno\n";
            }
            if (obj.Sede == "")
            {
                Mensaje += "Debe Ingresar la Sede del Alumno\n";
            }
            if (Mensaje != string.Empty)
            {
                return 0;
            }
            else
            {
                return objCD_Cliente.Registrar(obj, out Mensaje);
            }
        }

        public bool Editar(Cliente obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.NombreCompleto == "")
            {
                Mensaje += "Debe Ingresar el Nombre del Alumno\n";
            }
            if (obj.Telefono == "")
            {
                Mensaje += "Debe Ingresar el Telefono del Alumno\n";
            }
            if (obj.Domicilio == "")
            {
                Mensaje += "Debe Ingresar el Domicilio del Alumno\n";
            }
            if (obj.FechaNacimiento == "")
            {
                Mensaje += "Debe Ingresar la fecha de Nacimiento del Alumno\n";
            }
            if (obj.Curso == "")
            {
                Mensaje += "Debe Ingresar el Curso del Alumno\n";
            }
            if (obj.Sede == "")
            {
                Mensaje += "Debe Ingresar la Sede del Alumno\n";
            }
            if (Mensaje != string.Empty)
            {
                return false;
            }
            else
            {
                return objCD_Cliente.Editar(obj, out Mensaje);
            }
        }

        public bool Eliminar(Cliente obj, out string Mensaje)
        {
            return objCD_Cliente.Eliminar(obj, out Mensaje);
        }
    }
}

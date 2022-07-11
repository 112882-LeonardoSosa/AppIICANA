using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Negocio
    {
        private CD_Negocio objCD_Negocio = new CD_Negocio();

        public Negocio ObtenerDatos()
        {
            return objCD_Negocio.ObtenerDatos();
        }

        public bool GuardarDatos(Negocio obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.NombreNegocio == "")
            {
                Mensaje += "Debe Ingresar el Nombre del Negocio\n";
            }
            if (obj.CUIL == "")
            {
                Mensaje += "Debe Ingresar el CUIL del Negocio\n";
            }
            if (obj.Direccion == "")
            {
                Mensaje += "Debe Ingresar la Direccion del Negocio\n";
            }
            if (Mensaje != string.Empty)
            {
                return false;
            }
            else
            {
                return objCD_Negocio.GuardarDatos(obj, out Mensaje);
            }
        }

        public byte[] ObtenerLogo(out bool obtenido) 
        {
            return objCD_Negocio.ObtenerLogo(out obtenido);
        }

        public bool ActualizarLogo(byte[] imagen,out string Mensaje)
        {
            return objCD_Negocio.ActualizarLogo(imagen, out Mensaje);
        }
    }
}

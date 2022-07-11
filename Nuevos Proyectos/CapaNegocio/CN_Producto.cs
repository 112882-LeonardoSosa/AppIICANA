using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Producto
    {
        private CD_Producto objCD_Producto = new CD_Producto();

        public List<Producto> Listar()
        {
            return objCD_Producto.Listar();
        }

        public int Registrar(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.Codigo == "")
            {
                Mensaje += "Debe Ingresar el Codigo del Producto\n";
            }
            if (obj.Nombre == "")
            {
                Mensaje += "Debe Ingresar el Nombre del Producto\n";
            }
            if (obj.Descripcion == "")
            {
                Mensaje += "Debe Ingresar la Descripcion del Producto\n";
            }
            if (Mensaje != string.Empty)
            {
                return 0;
            }
            else
            {
                return objCD_Producto.Registrar(obj, out Mensaje);
            }
        }

        public bool Editar(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            if (obj.Codigo == "")
            {
                Mensaje += "Debe Ingresar el Codigo del Producto\n";
            }
            if (obj.Nombre == "")
            {
                Mensaje += "Debe Ingresar el Nombre del Producto\n";
            }
            if (obj.Descripcion == "")
            {
                Mensaje += "Debe Ingresar la Descripcion del Producto\n";
            }
            if (Mensaje != string.Empty)
            {
                return false;
            }
            else
            {
                return objCD_Producto.Editar(obj, out Mensaje);
            }
        }

        public bool Eliminar(Producto obj, out string Mensaje)
        {
            return objCD_Producto.Eliminar(obj, out Mensaje);
        }
    }
}

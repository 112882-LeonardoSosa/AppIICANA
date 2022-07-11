using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Rol
    {
        public CD_Rol objCN_Rol = new CD_Rol();

        public List<Rol> Listar()
        {
            return objCN_Rol.Listar();
        }
    }
}

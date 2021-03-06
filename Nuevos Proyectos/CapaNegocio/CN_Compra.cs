using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Compra
    {
        private CD_Compra objCD_Compra = new CD_Compra();

        public int ObtenerCorrelativo()
        {
            return objCD_Compra.ObtenerCorrelativo();
        }

        public bool Registrar(Compra obj,DataTable DetalleCompra, out string Mensaje)
        {     
            return objCD_Compra.Registrar(obj, DetalleCompra,out Mensaje);
        }

        public Compra ObtenerCompra(string Numero) 
        {
            Compra oCompra = objCD_Compra.ObtenerCompra(Numero);
            if (oCompra.IdCompra != 0) 
            {
                List<Detalle_Compra> oDetalleCompra = objCD_Compra.ObtenerDetalleCompra(oCompra.IdCompra);

                oCompra.ListaCompra = oDetalleCompra;

            }
            return oCompra;
        }
    }
}

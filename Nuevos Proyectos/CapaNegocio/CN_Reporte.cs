using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Reporte
    {
        private CD_Reporte objCD_Reporte = new CD_Reporte();

        public List<ReporteCompra> Compra(string fechaInicio, string fechaFin, int idProveedor) 
        {
            return objCD_Reporte.Compra(fechaInicio,fechaFin,idProveedor);
        }

        public List<ReporteVenta> Venta(string fechaInicio, string fechaFin)
        {
            return objCD_Reporte.Venta(fechaInicio, fechaFin);
        }
    }
}

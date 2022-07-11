using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public Usuario oUsuario { get; set; }
        public string TipoDocumento { get; set; }
        public string NroDocumento { get; set; }
        public string DocumentoCliente { get; set; }
        public string NombreCliente { get; set; }
        public string Alumnos { get; set; }
        public string Concepto { get; set; }
        public string Observaciones { get; set; }
        public string FormaPago { get; set; }
        public decimal MontoPago { get; set; }
        public decimal MontoCambio { get; set; }
        public decimal MontoTotal { get; set; }
        public List<Detalle_Venta> DetallesVentas { get; set; }
        public string FechaRegistro { get; set; }

        public void QuitarDetalle(int indice)
        {
            DetallesVentas.RemoveAt(indice);//QUITAR EL DETALLE AL QUE LE PASAMOS EL INDICE NRO
        }

    }
}

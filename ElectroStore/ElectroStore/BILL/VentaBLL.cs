using System;
using System.Data;
using ElectroStore.DAL;

namespace ElectroStore.BLL
{
    public class VentaBLL
    {
        private readonly VentaDAL ventaDal = new VentaDAL();

        public DataTable ObtenerModelosCatalogo()
        {
            return ventaDal.ObtenerModelosCatalogo();
        }

        public DataTable ObtenerEmpleadosActivos()
        {
            return ventaDal.ObtenerEmpleadosActivos();
        }

        public DataTable ObtenerPromocionesActivas()
        {
            return ventaDal.ObtenerPromocionesActivas();
        }

        public DataRow BuscarClientePorIdentidad(string numIdentidad)
        {
            return ventaDal.BuscarClientePorIdentidad(numIdentidad);
        }

        public int RegistrarCliente(string identidad, string nombre, string apellido, string telefono)
        {
            if (string.IsNullOrWhiteSpace(identidad))
            {
                throw new ArgumentException("La cédula o RUC es obligatoria.");
            }
            return ventaDal.RegistrarCliente(identidad, nombre, apellido, telefono);
        }

        public void RegistrarVenta(int idCliente, int idEmpleado, int idMetodoPago, string numComprobante, decimal subtotal, decimal descuentoAplicado, decimal iva, decimal total, int idModelo, int stockActual)
        {
            if (stockActual <= 0)
            {
                throw new InvalidOperationException("No hay stock disponible para este producto.");
            }

            ventaDal.RegistrarVenta(idCliente, idEmpleado, idMetodoPago, numComprobante, subtotal, descuentoAplicado, iva, total, idModelo);
        }
    }
}
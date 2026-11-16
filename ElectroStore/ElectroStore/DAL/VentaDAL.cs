using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace ElectroStore.DAL
{
    public class VentaDAL
    {
        private readonly string cadenaConexion = ConfigurationManager.ConnectionStrings["CadenaSQL"].ConnectionString;

        public DataTable ObtenerModelosCatalogo()
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string query = @"SELECT m.id_modelo, 
                                        c.nombre_categoria + ': ' + ma.nombre + ' ' + m.nombre_modelo + 
                                        CASE WHEN m.almacenamiento_gb IS NOT NULL THEN ' (' + CAST(m.almacenamiento_gb AS VARCHAR) + 'GB)' ELSE '' END + 
                                        ' - ' + m.color AS Descripcion, 
                                        m.precio_venta_sugerido,
                                        m.stock_actual
                                 FROM modelo m
                                 INNER JOIN marca ma ON m.id_marca = ma.id_marca
                                 INNER JOIN categoria c ON m.id_categoria = c.id_categoria";

                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                    {
                        conexion.Open();
                        adaptador.Fill(tabla);
                    }
                }
            }
            return tabla;
        }

        public DataTable ObtenerEmpleadosActivos()
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string query = "SELECT id_empleado, primer_nombre + ' ' + primer_apellido AS NombreEmpleado FROM empleado WHERE activo = 1";
                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                    {
                        conexion.Open();
                        adaptador.Fill(tabla);
                    }
                }
            }
            return tabla;
        }

        public DataTable ObtenerPromocionesActivas()
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string query = "SELECT id_promocion, nombre_promo + ' (' + CAST(CAST(porcentaje_descuento AS INT) AS VARCHAR) + '% OFF)' AS DescripcionPromo, porcentaje_descuento FROM promocion WHERE activa = 1";
                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                    {
                        conexion.Open();
                        adaptador.Fill(tabla);
                    }
                }
            }
            return tabla;
        }

        public DataRow BuscarClientePorIdentidad(string numIdentidad)
        {
            DataTable tabla = new DataTable();
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                string query = "SELECT id_cliente, primer_nombre, primer_apellido, telefono FROM cliente WHERE num_identidad = @identidad";
                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@identidad", numIdentidad);
                    using (SqlDataAdapter adaptador = new SqlDataAdapter(comando))
                    {
                        conexion.Open();
                        adaptador.Fill(tabla);
                    }
                }
            }
            if (tabla.Rows.Count > 0) return tabla.Rows[0];
            return null;
        }

        public int RegistrarCliente(string identidad, string nombre, string apellido, string telefono)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                string query = @"INSERT INTO cliente (num_identidad, primer_nombre, primer_apellido, telefono, fecha_registro) 
                                 VALUES (@identidad, @nombre, @apellido, @telefono, GETDATE());
                                 SELECT SCOPE_IDENTITY();";
                using (SqlCommand comando = new SqlCommand(query, conexion))
                {
                    comando.Parameters.AddWithValue("@identidad", identidad);
                    comando.Parameters.AddWithValue("@nombre", nombre);
                    comando.Parameters.AddWithValue("@apellido", apellido);
                    comando.Parameters.AddWithValue("@telefono", telefono);
                    return Convert.ToInt32(comando.ExecuteScalar());
                }
            }
        }

        public void RegistrarVenta(int idCliente, int idEmpleado, int idMetodoPago, string numComprobante, decimal subtotal, decimal descuentoAplicado, decimal iva, decimal total, int idModelo)
        {
            using (SqlConnection conexion = new SqlConnection(cadenaConexion))
            {
                conexion.Open();
                SqlTransaction transaccion = conexion.BeginTransaction();

                try
                {
                    string queryVenta = @"INSERT INTO venta (id_cliente, id_empleado, id_metodo_pago, numero_comprobante, fecha_venta, subtotal, descuento_aplicado, impuesto_iva, total) 
                                          VALUES (@idCliente, @idEmpleado, @idMetodoPago, @numComprobante, GETDATE(), @subtotal, @descuento, @iva, @total);
                                          SELECT SCOPE_IDENTITY();";

                    SqlCommand cmdVenta = new SqlCommand(queryVenta, conexion, transaccion);
                    cmdVenta.Parameters.AddWithValue("@idCliente", idCliente);
                    cmdVenta.Parameters.AddWithValue("@idEmpleado", idEmpleado);
                    cmdVenta.Parameters.AddWithValue("@idMetodoPago", idMetodoPago);
                    cmdVenta.Parameters.AddWithValue("@numComprobante", numComprobante);
                    cmdVenta.Parameters.AddWithValue("@subtotal", subtotal);
                    cmdVenta.Parameters.AddWithValue("@descuento", descuentoAplicado);
                    cmdVenta.Parameters.AddWithValue("@iva", iva);
                    cmdVenta.Parameters.AddWithValue("@total", total);

                    int idVentaGenerada = Convert.ToInt32(cmdVenta.ExecuteScalar());

                    string queryDetalle = @"INSERT INTO detalle_venta (id_venta, id_modelo, cantidad, precio_unitario, descuento_unitario) 
                                            VALUES (@idVenta, @idModelo, 1, @precio, @descUnitario)";

                    SqlCommand cmdDetalle = new SqlCommand(queryDetalle, conexion, transaccion);
                    cmdDetalle.Parameters.AddWithValue("@idVenta", idVentaGenerada);
                    cmdDetalle.Parameters.AddWithValue("@idModelo", idModelo);
                    cmdDetalle.Parameters.AddWithValue("@precio", subtotal);
                    cmdDetalle.Parameters.AddWithValue("@descUnitario", descuentoAplicado);
                    cmdDetalle.ExecuteNonQuery();

                    string queryStock = "UPDATE modelo SET stock_actual = stock_actual - 1 WHERE id_modelo = @idModelo";
                    SqlCommand cmdStock = new SqlCommand(queryStock, conexion, transaccion);
                    cmdStock.Parameters.AddWithValue("@idModelo", idModelo);
                    cmdStock.ExecuteNonQuery();

                    transaccion.Commit();
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    throw new Exception("Error al procesar la venta: " + ex.Message);
                }
            }
        }
    }
}
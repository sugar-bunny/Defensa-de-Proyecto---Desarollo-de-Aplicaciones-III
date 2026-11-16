using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ElectroStore.BLL;

namespace ElectroStore
{
    public partial class Form1 : Form
    {
        private VentaBLL ventaBll = new VentaBLL();

        private Label lblTitulo, lblModelo, lblPrecio, lblPromo, lblDescuentoMonto, lblPrecioIva, lblStock;
        private ComboBox cmbModelo, cmbPromocion, cmbEmpleado, cmbMetodoPago;
        private TextBox txtPrecio, txtDescuentoMonto, txtPrecioIva, txtStock;
        private Button btnComprar, btnSalir;

        private Panel pnlConfirmacion;
        private Label lblResumenTitulo, lblClienteId, lblNombre, lblApellido, lblTelefono;
        private TextBox txtClienteId, txtClienteNombre, txtClienteApellido, txtClienteTelefono;
        private RichTextBox txtTicketResumen;
        private Button btnConfirmarCompra, btnVolver;

        public Form1()
        {
            InitializeComponent();
            ConstruirInterfazPrincipal();
            ConstruirPanelConfirmacion();
            CargarCatalogo();
            CargarPromociones();
            CargarEmpleados();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void ConstruirInterfazPrincipal()
        {
            this.Text = "ElectroStore Mobile - Punto de Venta (USD)";
            this.Size = new Size(540, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            lblTitulo = new Label { Text = "ElectroStore Mobile POS", Font = new Font("Segoe UI", 16, FontStyle.Bold), AutoSize = true, Location = new Point(110, 15) };

            lblModelo = new Label { Text = "Producto:", Location = new Point(30, 70), AutoSize = true };
            cmbModelo = new ComboBox { Location = new Point(160, 67), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbModelo.SelectedIndexChanged += RecalcularValoresProducto;

            lblPrecio = new Label { Text = "Precio Base:", Location = new Point(30, 120), AutoSize = true };
            txtPrecio = new TextBox { Location = new Point(160, 117), Width = 320, ReadOnly = true };

            lblPromo = new Label { Text = "Promoción:", Location = new Point(30, 170), AutoSize = true };
            cmbPromocion = new ComboBox { Location = new Point(160, 167), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbPromocion.SelectedIndexChanged += RecalcularValoresProducto;

            lblDescuentoMonto = new Label { Text = "Descuento:", Location = new Point(30, 220), AutoSize = true };
            txtDescuentoMonto = new TextBox { Location = new Point(160, 217), Width = 320, ReadOnly = true };

            lblPrecioIva = new Label { Text = "Total con IVA (15%):", Location = new Point(30, 270), AutoSize = true };
            txtPrecioIva = new TextBox { Location = new Point(160, 267), Width = 320, ReadOnly = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };

            lblStock = new Label { Text = "Stock Disponible:", Location = new Point(30, 320), AutoSize = true };
            txtStock = new TextBox { Location = new Point(160, 317), Width = 320, ReadOnly = true };

            btnComprar = new Button { Text = "Proceder a Venta", Location = new Point(110, 390), Size = new Size(150, 45), BackColor = Color.ForestGreen, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnComprar.Click += BtnComprar_Click;

            btnSalir = new Button { Text = "Salir", Location = new Point(280, 390), Size = new Size(150, 45), BackColor = Color.Firebrick, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnSalir.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitulo, lblModelo, cmbModelo, lblPrecio, txtPrecio, lblPromo, cmbPromocion, lblDescuentoMonto, txtDescuentoMonto, lblPrecioIva, txtPrecioIva, lblStock, txtStock, btnComprar, btnSalir });
        }

        private void ConstruirPanelConfirmacion()
        {
            pnlConfirmacion = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Visible = false };

            lblResumenTitulo = new Label { Text = "Finalizar Transacción de Venta", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };

            Label lblEmp = new Label { Text = "Atendido por:", Location = new Point(20, 65), AutoSize = true };
            cmbEmpleado = new ComboBox { Location = new Point(130, 62), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

            lblClienteId = new Label { Text = "Cédula Cliente:", Location = new Point(20, 110), AutoSize = true };
            txtClienteId = new TextBox { Location = new Point(130, 107), Width = 200 };
            txtClienteId.Leave += TxtClienteId_Leave;

            lblNombre = new Label { Text = "Nombre:", Location = new Point(20, 155), AutoSize = true };
            txtClienteNombre = new TextBox { Location = new Point(130, 152), Width = 200 };

            lblApellido = new Label { Text = "Apellido:", Location = new Point(20, 200), AutoSize = true };
            txtClienteApellido = new TextBox { Location = new Point(130, 197), Width = 200 };

            lblTelefono = new Label { Text = "Teléfono:", Location = new Point(20, 245), AutoSize = true };
            txtClienteTelefono = new TextBox { Location = new Point(130, 242), Width = 200 };

            Label lblMetodo = new Label { Text = "Método Pago:", Location = new Point(20, 290), AutoSize = true };
            cmbMetodoPago = new ComboBox { Location = new Point(130, 287), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMetodoPago.Items.AddRange(new object[] { "Efectivo", "Tarjeta de Crédito/Débito", "Transferencia Bancaria" });
            cmbMetodoPago.SelectedIndex = 0;

            Label lblPreview = new Label { Text = "Resumen del Ticket (USD):", Location = new Point(350, 65), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            txtTicketResumen = new RichTextBox { Location = new Point(350, 90), Size = new Size(160, 240), ReadOnly = true, BackColor = Color.WhiteSmoke };

            btnConfirmarCompra = new Button { Text = "Confirmar y Cobrar", Location = new Point(110, 410), Size = new Size(180, 45), BackColor = Color.DodgerBlue, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnConfirmarCompra.Click += BtnConfirmarCompra_Click;

            btnVolver = new Button { Text = "Regresar", Location = new Point(310, 410), Size = new Size(150, 45), BackColor = Color.Gray, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnVolver.Click += (s, e) => pnlConfirmacion.Visible = false;

            pnlConfirmacion.Controls.AddRange(new Control[] { lblResumenTitulo, lblEmp, cmbEmpleado, lblClienteId, txtClienteId, lblNombre, txtClienteNombre, lblApellido, txtClienteApellido, lblTelefono, txtClienteTelefono, lblMetodo, cmbMetodoPago, lblPreview, txtTicketResumen, btnConfirmarCompra, btnVolver });

            this.Controls.Add(pnlConfirmacion);
            pnlConfirmacion.BringToFront();
            pnlConfirmacion.Visible = false;
        }

        private void CargarCatalogo()
        {
            try
            {
                DataTable dt = ventaBll.ObtenerModelosCatalogo();
                cmbModelo.DataSource = dt;
                cmbModelo.DisplayMember = "Descripcion";
                cmbModelo.ValueMember = "id_modelo";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el catálogo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarPromociones()
        {
            try
            {
                DataTable dt = ventaBll.ObtenerPromocionesActivas();
                cmbPromocion.DataSource = dt;
                cmbPromocion.DisplayMember = "DescripcionPromo";
                cmbPromocion.ValueMember = "porcentaje_descuento";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar promociones: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarEmpleados()
        {
            try
            {
                DataTable dt = ventaBll.ObtenerEmpleadosActivos();
                cmbEmpleado.DataSource = dt;
                cmbEmpleado.DisplayMember = "NombreEmpleado";
                cmbEmpleado.ValueMember = "id_empleado";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar empleados: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RecalcularValoresProducto(object sender, EventArgs e)
        {
            if (cmbModelo.SelectedItem is DataRowView filaModelo && cmbPromocion.SelectedItem is DataRowView filaPromo)
            {
                decimal precioBase = Convert.ToDecimal(filaModelo["precio_venta_sugerido"]);
                decimal porcentajeDesc = Convert.ToDecimal(filaPromo["porcentaje_descuento"]);
                int stock = Convert.ToInt32(filaModelo["stock_actual"]);

                decimal montoDescuento = precioBase * (porcentajeDesc / 100m);
                decimal precioConDescuento = precioBase - montoDescuento;
                decimal iva = precioConDescuento * 0.15m;
                decimal totalFinal = precioConDescuento + iva;

                txtPrecio.Text = "$ " + precioBase.ToString("N2");
                txtDescuentoMonto.Text = "$ " + montoDescuento.ToString("N2") + " (" + porcentajeDesc.ToString("0") + "%)";
                txtPrecioIva.Text = "$ " + totalFinal.ToString("N2") + " (Sub: $" + precioConDescuento.ToString("N2") + " + IVA: $" + iva.ToString("N2") + ")";
                txtStock.Text = stock.ToString();

                if (stock <= 0)
                {
                    txtStock.Text = "0 (AGOTADO - SIN STOCK)";
                    txtStock.ForeColor = Color.Red;
                }
                else
                {
                    txtStock.ForeColor = Color.Black;
                }
            }
        }

        private void TxtClienteId_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtClienteId.Text))
            {
                DataRow cliente = ventaBll.BuscarClientePorIdentidad(txtClienteId.Text.Trim());
                if (cliente != null)
                {
                    txtClienteNombre.Text = cliente["primer_nombre"].ToString();
                    txtClienteApellido.Text = cliente["primer_apellido"].ToString();
                    txtClienteTelefono.Text = cliente["telefono"].ToString();
                    txtClienteNombre.Tag = cliente["id_cliente"];
                }
                else
                {
                    txtClienteNombre.Tag = null;
                    txtClienteNombre.Clear();
                    txtClienteApellido.Clear();
                    txtClienteTelefono.Clear();
                    MessageBox.Show("Cliente no registrado en el sistema. Ingrese sus datos para agregarlo automáticamente.", "Nuevo Cliente", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnComprar_Click(object sender, EventArgs e)
        {
            if (cmbModelo.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un producto válido.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbModelo.SelectedItem is DataRowView filaModelo)
            {
                int stock = Convert.ToInt32(filaModelo["stock_actual"]);
                if (stock <= 0)
                {
                    MessageBox.Show("Este producto se encuentra AGOTADO (Stock 0). No es posible procesar la venta.", "Stock Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                decimal precioBase = Convert.ToDecimal(filaModelo["precio_venta_sugerido"]);
                DataRowView filaPromo = (DataRowView)cmbPromocion.SelectedItem;
                decimal porcentajeDesc = Convert.ToDecimal(filaPromo["porcentaje_descuento"]);

                decimal montoDesc = precioBase * (porcentajeDesc / 100m);
                decimal subtotalNeto = precioBase - montoDesc;
                decimal iva = subtotalNeto * 0.15m;
                decimal total = subtotalNeto + iva;

                txtTicketResumen.Text = "ELECTROSTORE TICKET\n" +
                                        "--------------------\n" +
                                        "Prod: " + filaModelo["Descripcion"].ToString() + "\n" +
                                        "Precio: $ " + precioBase.ToString("N2") + "\n" +
                                        "Desc: -$" + montoDesc.ToString("N2") + "\n" +
                                        "IVA 15%: $" + iva.ToString("N2") + "\n" +
                                        "--------------------\n" +
                                        "TOTAL: $ " + total.ToString("N2");
            }

            pnlConfirmacion.Visible = true;
            pnlConfirmacion.BringToFront();
        }

        private void BtnConfirmarCompra_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtClienteId.Text) || string.IsNullOrWhiteSpace(txtClienteNombre.Text))
                {
                    MessageBox.Show("Complete la cédula y nombre del cliente.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idCliente;
                if (txtClienteNombre.Tag != null)
                {
                    idCliente = Convert.ToInt32(txtClienteNombre.Tag);
                }
                else
                {
                    idCliente = ventaBll.RegistrarCliente(txtClienteId.Text.Trim(), txtClienteNombre.Text.Trim(), txtClienteApellido.Text.Trim(), txtClienteTelefono.Text.Trim());
                }

                int idEmpleado = Convert.ToInt32(cmbEmpleado.SelectedValue);
                int idMetodoPago = cmbMetodoPago.SelectedIndex + 1;
                string numComprobante = "FAC-MOB-" + DateTime.Now.ToString("yyyyMMddHHmmss");

                DataRowView filaModelo = (DataRowView)cmbModelo.SelectedItem;
                int idModelo = Convert.ToInt32(filaModelo["id_modelo"]);
                int stockActual = Convert.ToInt32(filaModelo["stock_actual"]);
                decimal precioBase = Convert.ToDecimal(filaModelo["precio_venta_sugerido"]);

                DataRowView filaPromo = (DataRowView)cmbPromocion.SelectedItem;
                decimal porcentajeDesc = Convert.ToDecimal(filaPromo["porcentaje_descuento"]);

                decimal descuentoAplicado = precioBase * (porcentajeDesc / 100m);
                decimal subtotalNeto = precioBase - descuentoAplicado;
                decimal iva = subtotalNeto * 0.15m;
                decimal total = subtotalNeto + iva;

                ventaBll.RegistrarVenta(idCliente, idEmpleado, idMetodoPago, numComprobante, subtotalNeto, descuentoAplicado, iva, total, idModelo, stockActual);

                MessageBox.Show("¡Venta procesada con éxito!\nComprobante: " + numComprobante + "\nTotal Pagado: $ " + total.ToString("N2"), "Éxito Comercial", MessageBoxButtons.OK, MessageBoxIcon.Information);

                pnlConfirmacion.Visible = false;
                txtClienteId.Clear();
                txtClienteNombre.Clear();
                txtClienteApellido.Clear();
                txtClienteTelefono.Clear();
                txtClienteNombre.Tag = null;

                CargarCatalogo();
                CargarPromociones();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Crítico en Transacción", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
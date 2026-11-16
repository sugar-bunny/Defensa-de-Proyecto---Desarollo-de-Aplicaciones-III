USE master;
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ElectroStoreBD')
BEGIN
    CREATE DATABASE ElectroStoreBD;
END
GO

USE ElectroStoreBD;
GO

IF OBJECT_ID('detalle_venta', 'U') IS NOT NULL DROP TABLE detalle_venta;
IF OBJECT_ID('venta', 'U') IS NOT NULL DROP TABLE venta;
IF OBJECT_ID('unidad_dispositivo', 'U') IS NOT NULL DROP TABLE unidad_dispositivo;
IF OBJECT_ID('compra', 'U') IS NOT NULL DROP TABLE compra;
IF OBJECT_ID('promocion', 'U') IS NOT NULL DROP TABLE promocion;
IF OBJECT_ID('modelo', 'U') IS NOT NULL DROP TABLE modelo;
IF OBJECT_ID('categoria', 'U') IS NOT NULL DROP TABLE categoria;
IF OBJECT_ID('metodo_pago', 'U') IS NOT NULL DROP TABLE metodo_pago;
IF OBJECT_ID('cliente', 'U') IS NOT NULL DROP TABLE cliente;
IF OBJECT_ID('empleado', 'U') IS NOT NULL DROP TABLE empleado;
IF OBJECT_ID('proveedor', 'U') IS NOT NULL DROP TABLE proveedor;
IF OBJECT_ID('marca', 'U') IS NOT NULL DROP TABLE marca;
GO

CREATE TABLE marca (
    id_marca INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE categoria (
    id_categoria INT IDENTITY(1,1) PRIMARY KEY,
    nombre_categoria VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE promocion (
    id_promocion INT IDENTITY(1,1) PRIMARY KEY,
    nombre_promo VARCHAR(80) NOT NULL,
    porcentaje_descuento DECIMAL(5,2) NOT NULL,
    activa BIT NOT NULL
);

CREATE TABLE proveedor (
    id_proveedor INT IDENTITY(1,1) PRIMARY KEY,
    nombre_empresa VARCHAR(120) NOT NULL,
    ruc_nit VARCHAR(25) NOT NULL UNIQUE,
    telefono VARCHAR(20) NOT NULL,
    email VARCHAR(80),
    direccion TEXT
);

CREATE TABLE empleado (
    id_empleado INT IDENTITY(1,1) PRIMARY KEY,
    num_identidad VARCHAR(25) NOT NULL UNIQUE,
    primer_nombre VARCHAR(20) NOT NULL,
    primer_apellido VARCHAR(20) NOT NULL,
    cargo VARCHAR(30) NOT NULL,
    activo BIT NOT NULL
);

CREATE TABLE cliente (
    id_cliente INT IDENTITY(1,1) PRIMARY KEY,
    num_identidad VARCHAR(25) NOT NULL UNIQUE,
    primer_nombre VARCHAR(20) NOT NULL,
    primer_apellido VARCHAR(20) NOT NULL,
    telefono VARCHAR(20) NOT NULL,
    fecha_registro DATE NOT NULL
);

CREATE TABLE metodo_pago (
    id_metodo_pago INT IDENTITY(1,1) PRIMARY KEY,
    tipo_pago VARCHAR(40) NOT NULL UNIQUE,
    activo BIT NOT NULL
);

CREATE TABLE modelo (
    id_modelo INT IDENTITY(1,1) PRIMARY KEY,
    id_marca INT NOT NULL FOREIGN KEY REFERENCES marca(id_marca),
    id_categoria INT NOT NULL FOREIGN KEY REFERENCES categoria(id_categoria),
    nombre_modelo VARCHAR(80) NOT NULL,
    almacenamiento_gb INT NULL,
    ram_gb INT NULL,
    color VARCHAR(40) NULL,
    precio_venta_sugerido DECIMAL(10,2) NOT NULL,
    stock_actual INT NOT NULL DEFAULT 0,
    stock_minimo INT NOT NULL
);

CREATE TABLE compra (
    id_compra INT IDENTITY(1,1) PRIMARY KEY,
    id_proveedor INT NOT NULL FOREIGN KEY REFERENCES proveedor(id_proveedor),
    num_factura_proveedor VARCHAR(50) NOT NULL,
    fecha_compra DATE NOT NULL,
    total_compra DECIMAL(12,2) NOT NULL
);

CREATE TABLE unidad_dispositivo (
    id_unidad INT IDENTITY(1,1) PRIMARY KEY,
    id_modelo INT NOT NULL FOREIGN KEY REFERENCES modelo(id_modelo),
    id_compra INT NOT NULL FOREIGN KEY REFERENCES compra(id_compra),
    imei_1 VARCHAR(25) NULL UNIQUE,
    estado_inventario VARCHAR(20) NOT NULL
);

CREATE TABLE venta (
    id_venta INT IDENTITY(1,1) PRIMARY KEY,
    id_cliente INT NOT NULL FOREIGN KEY REFERENCES cliente(id_cliente),
    id_empleado INT NOT NULL FOREIGN KEY REFERENCES empleado(id_empleado),
    id_metodo_pago INT NOT NULL FOREIGN KEY REFERENCES metodo_pago(id_metodo_pago),
    numero_comprobante VARCHAR(30) NOT NULL UNIQUE,
    fecha_venta DATETIME NOT NULL,
    subtotal DECIMAL(12,2) NOT NULL,
    descuento_aplicado DECIMAL(12,2) NOT NULL,
    impuesto_iva DECIMAL(12,2) NOT NULL,
    total DECIMAL(12,2) NOT NULL
);

CREATE TABLE detalle_venta (
    id_detalle_venta INT IDENTITY(1,1) PRIMARY KEY,
    id_venta INT NOT NULL FOREIGN KEY REFERENCES venta(id_venta),
    id_modelo INT NOT NULL FOREIGN KEY REFERENCES modelo(id_modelo),
    cantidad INT NOT NULL,
    precio_unitario DECIMAL(10,2) NOT NULL,
    descuento_unitario DECIMAL(10,2) NOT NULL
);
GO

INSERT INTO metodo_pago (tipo_pago, activo) VALUES 
('Efectivo', 1), 
('Tarjeta', 1), 
('Transferencia', 1);

INSERT INTO empleado (num_identidad, primer_nombre, primer_apellido, cargo, activo) VALUES 
('001-010100-0001A', 'Danilo', 'Rodriguez', 'Vendedor', 1),
('002-020200-0002B', 'Luis', 'Miguel', 'Gerente de Tienda', 1),
('003-030300-0003C', 'Kerry', 'Copito', 'Cajero Principal', 1),
('004-040400-0004E', 'LaLa', 'Joela', 'Vendedora', 1),
('005-050500-0005F', 'Israel', 'Lanuza', 'Asesor de Ventas', 1);

INSERT INTO cliente (num_identidad, primer_nombre, primer_apellido, telefono, fecha_registro) VALUES 
('401-200200-0002B', 'María', 'López', '88889999', GETDATE());

INSERT INTO marca (nombre) VALUES 
('Samsung'), 
('Apple'), 
('Xiaomi'), 
('Motorola');

INSERT INTO categoria (nombre_categoria) VALUES 
('Celulares'), 
('Accesorios'), 
('Reclamos y Devoluciones');

INSERT INTO promocion (nombre_promo, porcentaje_descuento, activa) VALUES 
('Sin Descuento', 0.00, 1),
('Especial Fin de Año', 10.00, 1),
('Super Promoción Navideña', 15.00, 1),
('Black Friday Store', 20.00, 1);

INSERT INTO proveedor (nombre_empresa, ruc_nit, telefono, direccion) VALUES 
('MegaTech Mobile S.A.', 'J031000001111', '22223333', 'Managua');

INSERT INTO compra (id_proveedor, num_factura_proveedor, fecha_compra, total_compra) VALUES 
(1, 'FAC-GENERAL-01', GETDATE(), 5000.00);

INSERT INTO modelo (id_marca, id_categoria, nombre_modelo, almacenamiento_gb, ram_gb, color, precio_venta_sugerido, stock_actual, stock_minimo) VALUES 
(1, 1, 'Galaxy S23', 256, 8, 'Negro', 650.00, 6, 2),
(2, 1, 'iPhone 15 Pro', 128, 8, 'Titanio Natural', 999.00, 4, 1),
(3, 1, 'Xiaomi 13', 256, 12, 'Blanco', 500.00, 5, 2),
(4, 1, 'Moto G84', 128, 8, 'Azul', 300.00, 0, 2),
(1, 2, 'Cargador Rápido USB-C 25W', NULL, NULL, 'Blanco', 25.00, 15, 5),
(2, 2, 'AirPods Pro', NULL, NULL, 'Blanco', 249.00, 8, 3);
GO
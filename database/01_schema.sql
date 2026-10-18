-- Sistema de Ventas: database schema
-- Matches the Entity Framework Core model in SistemaVenta.DAL/DBContext/ITventoryContext.cs.
-- Runs on SQL Server 2016+ and Azure SQL Database. Run 02_seed.sql afterwards.

CREATE TABLE Rol (
    idRol          INT IDENTITY(1,1) PRIMARY KEY,
    descripcion    VARCHAR(30),
    esActivo       BIT,
    fechaRegistro  DATETIME DEFAULT GETDATE()
);

CREATE TABLE Menu (
    idMenu         INT IDENTITY(1,1) PRIMARY KEY,
    descripcion    VARCHAR(30),
    idMenuPadre    INT REFERENCES Menu(idMenu),
    icono          VARCHAR(30),
    controlador    VARCHAR(30),
    paginaAccion   VARCHAR(30),
    esActivo       BIT,
    fechaRegistro  DATETIME DEFAULT GETDATE()
);

CREATE TABLE RolMenu (
    idRolMenu      INT IDENTITY(1,1) PRIMARY KEY,
    idRol          INT REFERENCES Rol(idRol),
    idMenu         INT REFERENCES Menu(idMenu),
    esActivo       BIT,
    fechaRegistro  DATETIME DEFAULT GETDATE()
);

CREATE TABLE Usuario (
    idUsuario      INT IDENTITY(1,1) PRIMARY KEY,
    nombre         VARCHAR(50),
    correo         VARCHAR(50),
    telefono       VARCHAR(50),
    idRol          INT REFERENCES Rol(idRol),
    urlFoto        VARCHAR(500),
    nombreFoto     VARCHAR(100),
    clave          VARCHAR(100),
    esActivo       BIT,
    fechaRegistro  DATETIME DEFAULT GETDATE()
);

CREATE TABLE Categoria (
    idCategoria    INT IDENTITY(1,1) PRIMARY KEY,
    descripcion    VARCHAR(50),
    esActivo       BIT,
    fechaRegistro  DATETIME DEFAULT GETDATE()
);

CREATE TABLE Producto (
    idProducto     INT IDENTITY(1,1) PRIMARY KEY,
    codigoBarra    VARCHAR(50),
    marca          VARCHAR(50),
    descripcion    VARCHAR(100),
    idCategoria    INT REFERENCES Categoria(idCategoria),
    stock          INT,
    urlImagen      VARCHAR(500),
    nombreImagen   VARCHAR(100),
    precio         DECIMAL(10,2),
    esActivo       BIT,
    fechaRegistro  DATETIME DEFAULT GETDATE()
);

CREATE TABLE NumeroCorrelativo (
    idNumeroCorrelativo INT IDENTITY(1,1) PRIMARY KEY,
    ultimoNumero        INT,
    cantidadDigitos     INT,
    gestion             VARCHAR(100),
    fechaActualizacion  DATETIME
);

CREATE TABLE TipoDocumentoVenta (
    idTipoDocumentoVenta INT IDENTITY(1,1) PRIMARY KEY,
    descripcion          VARCHAR(50),
    esActivo             BIT,
    fechaRegistro        DATETIME DEFAULT GETDATE()
);

CREATE TABLE Venta (
    idVenta               INT IDENTITY(1,1) PRIMARY KEY,
    numeroVenta           VARCHAR(6),
    idTipoDocumentoVenta  INT REFERENCES TipoDocumentoVenta(idTipoDocumentoVenta),
    idUsuario             INT REFERENCES Usuario(idUsuario),
    documentoCliente      VARCHAR(10),
    nombreCliente         VARCHAR(20),
    subTotal              DECIMAL(10,2),
    impuestoTotal         DECIMAL(10,2),
    Total                 DECIMAL(10,2),
    fechaRegistro         DATETIME DEFAULT GETDATE()
);

CREATE TABLE DetalleVenta (
    idDetalleVenta       INT IDENTITY(1,1) PRIMARY KEY,
    idVenta              INT REFERENCES Venta(idVenta),
    idProducto           INT,
    marcaProducto        VARCHAR(100),
    descripcionProducto  VARCHAR(100),
    categoriaProducto    VARCHAR(100),
    cantidad             INT,
    precio               DECIMAL(10,2),
    total                DECIMAL(10,2)
);

CREATE TABLE Negocio (
    idNegocio           INT PRIMARY KEY,
    urlLogo             VARCHAR(500),
    nombreLogo          VARCHAR(100),
    numeroDocumento     VARCHAR(50),
    nombre              VARCHAR(50),
    correo              VARCHAR(50),
    direccion           VARCHAR(50),
    telefono            VARCHAR(50),
    porcentajeImpuesto  DECIMAL(10,2),
    simboloMoneda       VARCHAR(5)
);

CREATE TABLE Configuracion (
    recurso    VARCHAR(50),
    propiedad  VARCHAR(50),
    valor      VARCHAR(60)
);

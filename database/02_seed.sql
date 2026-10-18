-- Sistema de Ventas: initial data
-- Roles, menus, document types, the sale counter, the business profile and the first administrator.
-- Choose the administrator's email and password below before running it.

DECLARE @AdminEmail    VARCHAR(50)  = 'admin@example.com';
DECLARE @AdminPassword VARCHAR(100) = 'change-me';

-- Roles
INSERT INTO Rol (descripcion, esActivo) VALUES
    ('Administrador', 1),
    ('Empleado', 1),
    ('Supervisor', 1);

-- Menus: a parent points to itself; its children point to the parent
INSERT INTO Menu (descripcion, icono, controlador, paginaAccion, esActivo) VALUES
    ('DashBoard',      'fas fa-fw fa-tachometer-alt', 'DashBoard', 'Index', 1);
INSERT INTO Menu (descripcion, icono, esActivo) VALUES
    ('Administración', 'fas fa-fw fa-cog', 1),
    ('Inventario',     'fas fa-fw fa-clipboard-list', 1),
    ('Ventas',         'fas fa-fw fa-tags', 1),
    ('Reportes',       'fas fa-fw fa-chart-area', 1);
UPDATE Menu SET idMenuPadre = idMenu;

INSERT INTO Menu (descripcion, idMenuPadre, controlador, paginaAccion, esActivo) VALUES
    ('Usuarios',          2, 'Usuario',   'Index',          1),
    ('Negocio',           2, 'Negocio',   'Index',          1),
    ('Categorias',        3, 'Categoria', 'Index',          1),
    ('Productos',         3, 'Producto',  'Index',          1),
    ('Nueva Venta',       4, 'Venta',     'NuevaVenta',     1),
    ('Historial Venta',   4, 'Venta',     'HistorialVenta', 1),
    ('Reporte de Ventas', 5, 'Reporte',   'Index',          1);

-- Administrator: everything
INSERT INTO RolMenu (idRol, idMenu, esActivo)
SELECT 1, idMenu, 1 FROM Menu;

-- Employee: dashboard and sales
INSERT INTO RolMenu (idRol, idMenu, esActivo) VALUES (2, 1, 1), (2, 10, 1), (2, 11, 1);

-- Supervisor: dashboard, inventory, sales and reports
INSERT INTO RolMenu (idRol, idMenu, esActivo) VALUES
    (3, 1, 1), (3, 8, 1), (3, 9, 1), (3, 10, 1), (3, 11, 1), (3, 12, 1);

-- First administrator (passwords are stored as lowercase SHA-256 hex)
INSERT INTO Usuario (nombre, correo, telefono, idRol, urlFoto, nombreFoto, clave, esActivo) VALUES
    ('Administrador', @AdminEmail, '', 1, '', '',
     LOWER(CONVERT(VARCHAR(64), HASHBYTES('SHA2_256', @AdminPassword), 2)), 1);

-- Sale counter used to number every sale
INSERT INTO NumeroCorrelativo (ultimoNumero, cantidadDigitos, gestion, fechaActualizacion) VALUES
    (0, 6, 'Venta', GETDATE());

-- Document types
INSERT INTO TipoDocumentoVenta (descripcion, esActivo) VALUES
    ('Boleta', 1),
    ('Factura', 1);

-- Business profile (editable from the app)
INSERT INTO Negocio (idNegocio, urlLogo, nombreLogo, numeroDocumento, nombre, correo, direccion, telefono, porcentajeImpuesto, simboloMoneda) VALUES
    (1, '', '', '00000000000', 'Mi Negocio', 'ventas@example.com', 'Av. Principal 123', '000-000-0000', 18, '$');

-- Email (SMTP) and Firebase Storage settings: fill the values to enable
-- password emails and photo uploads
INSERT INTO Configuracion (recurso, propiedad, valor) VALUES
    ('Servicio_Correo',  'correo',           ''),
    ('Servicio_Correo',  'clave',            ''),
    ('Servicio_Correo',  'alias',            'Sistema de Ventas'),
    ('Servicio_Correo',  'host',             'smtp.gmail.com'),
    ('Servicio_Correo',  'puerto',           '587'),
    ('FireBase_Storage', 'email',            ''),
    ('FireBase_Storage', 'clave',            ''),
    ('FireBase_Storage', 'ruta',             ''),
    ('FireBase_Storage', 'api_key',          ''),
    ('FireBase_Storage', 'carpeta_usuario',  'IMAGENES_USUARIO'),
    ('FireBase_Storage', 'carpeta_producto', 'IMAGENES_PRODUCTO'),
    ('FireBase_Storage', 'carpeta_logo',     'IMAGENES_LOGO');

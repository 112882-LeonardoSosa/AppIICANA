create database TEST6

GO

USE TEST6

GO


create table ROL(
IdRol int primary key identity,
Descripcion varchar(50),
FechaRegistro datetime default getdate()
)

go

create table PERMISO(
IdPermiso int primary key identity,
IdRol int references ROL(IdRol),
NombreMenu varchar(100),
FechaRegistro datetime default getdate()
)

go


create table CLIENTE(
IdCliente int primary key identity,
Documento varchar(50),
NombreCompleto varchar(50),
FechaNacimiento varchar(50),
Domicilio varchar(300),
Correo varchar(50),
Curso varchar(100),
Telefono varchar(100),
Sede varchar(100),
Estado bit,
FechaRegistro datetime default getdate()
)

go

create table USUARIO(
IdUsuario int primary key identity,
Documento varchar(50),
NombreCompleto varchar(50),
Correo varchar(50),
Clave varchar(50),
IdRol int references ROL(IdRol),
Estado bit,
FechaRegistro datetime default getdate()
)

go

create table CATEGORIA(
IdCategoria int primary key identity,
Descripcion varchar(100),
Estado bit,
FechaRegistro datetime default getdate()
)

go

create table PRODUCTO(
IdProducto int primary key identity,
Codigo varchar(50),
Nombre varchar(50),
Descripcion varchar(50),
IdCategoria int references CATEGORIA(IdCategoria),
Precio decimal(10,2) default 0,
Estado bit,
FechaRegistro datetime default getdate()
)

go


create table VENTA(
IdVenta int primary key identity,
IdUsuario int references USUARIO(IdUsuario),
TipoDocumento varchar(50),
NumeroDocumento varchar(50),
DocumentoCliente varchar(50),
NombreCliente varchar(100),
Alumnos varchar(300),
Concepto varchar(100),
Observaciones varchar(300),
MontoPago decimal(10,2),
MontoCambio decimal(10,2),
MontoTotal decimal(10,2),
FechaRegistro datetime default getdate()
)


go


create table DETALLE_VENTA(
IdDetalleVenta int primary key identity,
IdVenta int references VENTA(IdVenta),
IdProducto int references PRODUCTO(IdProducto),
PrecioVenta decimal(10,2),
Cantidad int,
SubTotal decimal(10,2),
FechaRegistro datetime default getdate()
)

go

create table NEGOCIO(
IdNegocio int primary key,
Nombre varchar(60),
CUIL varchar(60),
Direccion varchar(200),
Logo varbinary(max) NULL
)

go


/*************************** CREACION DE PROCEDIMIENTOS ALMACENADOS ***************************/
/*--------------------------------------------------------------------------------------------*/

go


create PROC SP_REGISTRARUSUARIO(
@Documento varchar(50),
@NombreCompleto varchar(100),
@Correo varchar(100),
@Clave varchar(100),
@IdRol int,
@Estado bit,
@IdUsuarioResultado int output,
@Mensaje varchar(500) output
)
as
begin
	set @IdUsuarioResultado = 0
	set @Mensaje = ''


	if not exists(select * from USUARIO where Documento = @Documento)
	begin
		insert into usuario(Documento,NombreCompleto,Correo,Clave,IdRol,Estado) values
		(@Documento,@NombreCompleto,@Correo,@Clave,@IdRol,@Estado)

		set @IdUsuarioResultado = SCOPE_IDENTITY()
		
	end
	else
		set @Mensaje = 'No se puede repetir el documento para más de un usuario'

end

go

create PROC SP_EDITARUSUARIO(
@IdUsuario int,
@Documento varchar(50),
@NombreCompleto varchar(100),
@Correo varchar(100),
@Clave varchar(100),
@IdRol int,
@Estado bit,
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
	set @Respuesta = 0
	set @Mensaje = ''


	if not exists(select * from USUARIO where Documento = @Documento and idusuario != @IdUsuario)
	begin
		update  usuario set
		Documento = @Documento,
		NombreCompleto = @NombreCompleto,
		Correo = @Correo,
		Clave = @Clave,
		IdRol = @IdRol,
		Estado = @Estado
		where IdUsuario = @IdUsuario

		set @Respuesta = 1
		
	end
	else
		set @Mensaje = 'No se puede repetir el documento para más de un usuario'

end
go

create PROC SP_ELIMINARUSUARIO(
@IdUsuario int,
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
	set @Respuesta = 0
	set @Mensaje = ''
	declare @pasoreglas bit = 1

	IF EXISTS (SELECT * FROM VENTA C 
	INNER JOIN USUARIO U ON U.IdUsuario = C.IdUsuario
	WHERE U.IDUSUARIO = @IdUsuario
	)
	BEGIN
		set @pasoreglas = 0
		set @Respuesta = 0
		set @Mensaje = @Mensaje + 'No se puede eliminar porque el usuario se encuentra relacionado a una COMPRA\n' 
	END

	IF EXISTS (SELECT * FROM VENTA V
	INNER JOIN USUARIO U ON U.IdUsuario = V.IdUsuario
	WHERE U.IDUSUARIO = @IdUsuario
	)
	BEGIN
		set @pasoreglas = 0
		set @Respuesta = 0
		set @Mensaje = @Mensaje + 'No se puede eliminar porque el usuario se encuentra relacionado a una VENTA\n' 
	END

	if(@pasoreglas = 1)
	begin
		delete from USUARIO where IdUsuario = @IdUsuario
		set @Respuesta = 1 
	end

end

go

/* ---------- PROCEDIMIENTOS PARA CATEGORIA -----------------*/


create PROC SP_RegistrarCategoria(
@Descripcion varchar(50),
@Estado bit,
@Resultado int output,
@Mensaje varchar(500) output
)as
begin
	SET @Resultado = 0
	IF NOT EXISTS (SELECT * FROM CATEGORIA WHERE Descripcion = @Descripcion)
	begin
		insert into CATEGORIA(Descripcion,Estado) values (@Descripcion,@Estado)
		set @Resultado = SCOPE_IDENTITY()
	end
	ELSE
		set @Mensaje = 'La categoria ingresada ya se encuentra registrada...'
	
end


go

create procedure sp_EditarCategoria(
@IdCategoria int,
@Descripcion varchar(50),
@Estado bit,
@Resultado bit output,
@Mensaje varchar(500) output
)
as
begin
	SET @Resultado = 1
	IF NOT EXISTS (SELECT * FROM CATEGORIA WHERE Descripcion =@Descripcion and IdCategoria != @IdCategoria)
		update CATEGORIA set
		Descripcion = @Descripcion,
		Estado = @Estado
		where IdCategoria = @IdCategoria
	ELSE
	begin
		SET @Resultado = 0
		set @Mensaje = 'No se puede repetir la descripcion de una categoria'
	end

end

go

create procedure sp_EliminarCategoria(
@IdCategoria int,
@Resultado bit output,
@Mensaje varchar(500) output
)
as
begin
	SET @Resultado = 1
	IF NOT EXISTS (
	 select *  from CATEGORIA c
	 inner join PRODUCTO p on p.IdCategoria = c.IdCategoria
	 where c.IdCategoria = @IdCategoria
	)
	begin
	 delete top(1) from CATEGORIA where IdCategoria = @IdCategoria
	end
	ELSE
	begin
		SET @Resultado = 0
		set @Mensaje = 'La categoria se encuentara relacionada a un producto'
	end

end

GO

/* ---------- PROCEDIMIENTOS PARA PRODUCTO -----------------*/

create PROC sp_RegistrarProducto(
@Codigo varchar(20),
@Nombre varchar(30),
@Descripcion varchar(30),
@IdCategoria int,
@Precio decimal(10,2),
@Estado bit,
@Resultado int output,
@Mensaje varchar(500) output
)as
begin
	SET @Resultado = 0
	IF NOT EXISTS (SELECT * FROM producto WHERE Codigo = @Codigo)
	begin
		insert into producto(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado) values (@Codigo,@Nombre,@Descripcion,@IdCategoria,@Precio,@Estado)
		set @Resultado = SCOPE_IDENTITY()
	end
	ELSE
	 SET @Mensaje = 'Ya existe un producto con el mismo codigo' 
	
end

GO

create procedure sp_ModificarProducto(
@IdProducto int,
@Codigo varchar(20),
@Nombre varchar(30),
@Descripcion varchar(30),
@IdCategoria int,
@Precio decimal(10,2),
@Estado bit,
@Resultado bit output,
@Mensaje varchar(500) output
)
as
begin	
		SET @Resultado = 1
		if not exists(Select * from PRODUCTO where Codigo = @Codigo and IdProducto != @IdProducto)
		update PRODUCTO set
		codigo = @Codigo,
		Nombre = @Nombre,
		Descripcion = @Descripcion,
		IdCategoria = @IdCategoria,
		Precio = @Precio,
		Estado = @Estado
		where IdProducto = @IdProducto		
	ELSE
	begin
		SET @Resultado = 0
		SET @Mensaje = 'Ya existe un producto con el mismo codigo' 
	end
end

go


create PROC SP_EliminarProducto(
@IdProducto int,
@Respuesta bit output,
@Mensaje varchar(500) output
)
as
begin
	set @Respuesta = 0
	set @Mensaje = ''
	declare @pasoreglas bit = 1

	IF EXISTS (SELECT * FROM DETALLE_VENTA dc 
	INNER JOIN PRODUCTO p ON p.IdProducto = dc.IdProducto
	WHERE p.IdProducto = @IdProducto
	)
	BEGIN
		set @pasoreglas = 0
		set @Respuesta = 0
		set @Mensaje = @Mensaje + 'No se puede eliminar porque se encuentra relacionado a una COMPRA\n' 
	END

	IF EXISTS (SELECT * FROM DETALLE_VENTA dv
	INNER JOIN PRODUCTO p ON p.IdProducto = dv.IdProducto
	WHERE p.IdProducto = @IdProducto
	)
	BEGIN
		set @pasoreglas = 0
		set @Respuesta = 0
		set @Mensaje = @Mensaje + 'No se puede eliminar porque se encuentra relacionado a una VENTA\n' 
	END

	if(@pasoreglas = 1)
	begin
		delete from PRODUCTO where IdProducto = @IdProducto
		set @Respuesta = 1 
	end

end
go

/* ---------- PROCEDIMIENTOS PARA CLIENTE -----------------*/

create PROC sp_RegistrarCliente(
@Documento varchar(50),
@NombreCompleto varchar(50),
@FechaNacimiento varchar(50),
@Domicilio varchar(300),
@Correo varchar(50),
@Curso varchar(100),
@Telefono varchar(50),
@Sede varchar(100),
@Estado bit,
@Resultado int output,
@Mensaje varchar(500) output
)as
begin
	SET @Resultado = 0
	DECLARE @IDPERSONA INT 
	IF NOT EXISTS (SELECT * FROM CLIENTE WHERE Documento = @Documento)
	begin
		insert into CLIENTE(Documento,NombreCompleto,FechaNacimiento,Domicilio,Correo,Curso,Telefono,Sede,Estado) values (
		@Documento,@NombreCompleto,@FechaNacimiento,@Domicilio,@Correo,@Curso,@Telefono,@Sede,@Estado)

		set @Resultado = SCOPE_IDENTITY()
	end
	else
		set @Mensaje = 'El numero de documento ya existe'
end

go

create PROC sp_ModificarCliente(
@IdCliente int,
@Documento varchar(50),
@NombreCompleto varchar(50),
@FechaNacimiento varchar(50),
@Domicilio varchar(300),
@Correo varchar(50),
@Curso varchar(100),
@Telefono varchar(50),
@Sede varchar(50),
@Estado bit,
@Resultado bit output,
@Mensaje varchar(500) output
)as
begin
	SET @Resultado = 1
	Declare @IdPersona int
	If not exists(select * from CLIENTE where Documento = @Documento and IdCliente != @IdCliente)
	begin
		update CLIENTE set
		Documento = @Documento,
		NombreCompleto = @NombreCompleto,
		FechaNacimiento = @FechaNacimiento,
		Domicilio = @Domicilio,
		Correo = @Correo,
		Curso = @Curso,
		Telefono = @Telefono,
		Sede = @Sede,
		Estado = @Estado
		where IdCliente = @IdCliente
	end
	else
	SET @Resultado = 0
	set @Mensaje = 'El numero de Documento ya existe'
end

GO


/* PROCESOS PARA REGISTRAR UNA VENTA */

CREATE TYPE [dbo].[EDetalle_Venta] AS TABLE(
	[IdProducto] int NULL,
	[PrecioVenta] decimal(18,2) NULL,
	[Cantidad] int NULL,
	[SubTotal] decimal(18,2) NULL
)


GO


create procedure sp_RegistrarVenta(
@IdUsuario int,
@TipoDocumento varchar(500),
@NumeroDocumento varchar(500),
@DocumentoCliente varchar(500),
@NombreCliente varchar(500),
@Alumnos varchar(300),
@Concepto varchar(200),
@Observaciones varchar(300),
@MontoPago decimal(18,2),
@MontoCambio decimal(18,2),
@MontoTotal decimal(18,2),
@DetalleVenta [EDetalle_Venta] READONLY,                                      
@Resultado bit output,
@Mensaje varchar(500) output
)
as
begin
	
	begin try

		declare @idventa int = 0
		set @Resultado = 1
		set @Mensaje = ''

		begin  transaction registro

		insert into VENTA(IdUsuario,TipoDocumento,NumeroDocumento,DocumentoCliente,NombreCliente,Alumnos,Concepto,Observaciones,MontoPago,MontoCambio,MontoTotal)
		values(@IdUsuario,@TipoDocumento,@NumeroDocumento,@DocumentoCliente,@NombreCliente,@Alumnos,@Concepto,@Observaciones,@MontoPago,@MontoCambio,@MontoTotal)

		set @idventa = SCOPE_IDENTITY()

		insert into DETALLE_VENTA(IdVenta,IdProducto,PrecioVenta,Cantidad,SubTotal)
		select @idventa,IdProducto,PrecioVenta,Cantidad,SubTotal from @DetalleVenta

		commit transaction registro

	end try
	begin catch
		set @Resultado = 0
		set @Mensaje = ERROR_MESSAGE()
		rollback transaction registro
	end catch

end

go

 create PROC sp_ReporteVentas(
 @fechainicio varchar(10),
 @fechafin varchar(10)
 )
 as
 begin
 SET DATEFORMAT dmy;  
 select 
 convert(char(10),v.FechaRegistro,103)[FechaRegistro],v.TipoDocumento,v.NumeroDocumento,v.MontoTotal,
 u.NombreCompleto[UsuarioRegistro],
 v.DocumentoCliente,v.NombreCliente,v.Alumnos,v.Concepto,v.Observaciones,
 p.Codigo[CodigoProducto],p.Nombre[NombreProducto],p.Descripcion,dv.PrecioVenta,dv.Cantidad,dv.SubTotal
 from VENTA v
 inner join USUARIO u on u.IdUsuario = v.IdUsuario
 inner join DETALLE_VENTA dv on dv.IdVenta = v.IdVenta
 inner join PRODUCTO p on p.IdProducto = dv.IdProducto
 inner join CATEGORIA ca on ca.IdCategoria = p.IdCategoria
 where CONVERT(date,v.FechaRegistro) between @fechainicio and @fechafin
end


/****************** INSERTAMOS REGISTROS A LAS TABLAS ******************/
/*---------------------------------------------------------------------*/

GO

 insert into rol (Descripcion)
 values('ADMINISTRADOR')

 GO

  insert into rol (Descripcion)
 values('EMPLEADO')

 GO

 insert into USUARIO(Documento,NombreCompleto,Correo,Clave,IdRol,Estado)
 values 
 ('101010','ADMIN','@GMAIL.COM','123',1,1)

  insert into USUARIO(Documento,NombreCompleto,Correo,Clave,IdRol,Estado)
 values 
 ('3103','Sede Valle Mall','iicanajardinbotanico@gmail.com','3103',1,1)

  insert into USUARIO(Documento,NombreCompleto,Correo,Clave,IdRol,Estado)
 values 
 ('ACAECE','Sede ACAECE','iicanajardinbotanico@gmail.com','123',1,1)

 GO


 insert into USUARIO(Documento,NombreCompleto,Correo,Clave,IdRol,Estado)
 values 
 ('20','EMPLEADO','@GMAIL.COM','456',2,1)

 GO

insert into PERMISO(IdRol,NombreMenu)
values(1,'menuUsuarios'),
(1,'menuMantenimiento'),
(1,'menuVentas'),
(1,'menuClientes'),
(1,'menuReportes'),
(1,'menuAcercaDe')

  GO

insert into PERMISO(IdRol,NombreMenu)
values
(2,'menuVentas'),
(2,'menuClientes'),
(2,'menuReportes'),
(2,'menuAcercaDe')

  GO

insert into NEGOCIO(IdNegocio,Nombre,CUIL,Direccion,Logo) values
(1,'IICANA Jardín Botánico','27-29967588-8','Av. República de China 1151 - Piso 1 Locales 15 a 18 Valle Mall',null)

GO
insert into CLIENTE (Documento,NombreCompleto,FechaNacimiento,Domicilio,Correo,Curso,Telefono,Sede,Estado)
values('333','Test','14/10/1991','test@','TestDom','Prep A','35344','Valle',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('Prep A',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('Prep B',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('1st Youngsters',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('2nd Youngsters',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('3rd Youngsters',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('4th Youngsters',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('2nd Youngsters',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('1st Intermediate',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('2nd Intermediate',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('3rd Intermediate',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('4th Intermediate',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('Low Teens',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('Mid Teens',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('High Teens',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('Acelerated A',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('Acelerated B',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('Acelerated C',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('Blended 1',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('Blended 2',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('Blended 3',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('Blended 4',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('Blended 5',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('Blended 6',1)
GO

insert into CATEGORIA(Descripcion,Estado)
values('Matricula',1)
GO

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('PA01','Prep A','Precio Lista',1,4890,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('PA02','Prep A','Precio Lista 10% Dcto Familiar',1,4400,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('PA03','Prep A','Precio Lista 20% Dcto Familiar',1,3910,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('PB01','Prep B','Precio Lista',2,4890,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('PB02','Prep B','Precio Lista 10% Dcto Familiar',2,4400,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('PB03','Prep B','Precio Lista 20% Dcto Familiar',2,3910,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('Y101','1st Youngsters','Precio Lista',3,4890,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('Y102','1st Youngsters','Precio Lista 10% Dcto Familiar',3,4400,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('Y103','1st Youngsters','Precio Lista 20% Dcto Familiar',3,3910,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('Y201','2nd Youngsters','Precio Lista',4,4890,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('Y202','2nd Youngsters','Precio Lista 10% Dcto Familiar',4,4400,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('Y203','2nd Youngsters','Precio Lista 20% Dcto Familiar',4,3910,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('Y301','3rd Youngsters','Precio Lista',5,5160,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('Y302','3rd Youngsters','Precio Lista 10% Dcto Familiar',5,4650,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('Y303','3rd Youngsters','Precio Lista 20% Dcto Familiar',5,4130,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('Y401','4th Youngsters','Precio Lista',6,5160,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('Y402','4th Youngsters','Precio Lista 10% Dcto Familiar',6,4650,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('Y403','4th Youngsters','Precio Lista 20% Dcto Familiar',6,4130,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('I101','1st Intermediate','Precio Lista',7,4880,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('I102','1st Intermediate','Precio Lista 10% Dcto Familiar',7,4390,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('I103','1st Intermediate','Precio Lista 20% Dcto Familiar',7,3900,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('I201','2nd Intermediate','Precio Lista',8,4980,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('I202','2nd Intermediate','Precio Lista 10% Dcto Familiar',8,4480,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('I203','2nd Intermediate','Precio Lista 20% Dcto Familiar',8,3900,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('I301','3rd Intermediate','Precio Lista',9,5080,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('I302','3rd Intermediate','Precio Lista 10% Dcto Familiar',9,4570,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('I303','3rd Intermediate','Precio Lista 20% Dcto Familiar',9,4070,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('I401','4th Intermediate','Precio Lista',10,5180,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('I402','4th Intermediate','Precio Lista 10% Dcto Familiar',10,4660,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('I403','4th Intermediate','Precio Lista 20% Dcto Familiar',10,4150,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('LT01','Low Teens','Precio Lista',11,5280,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('LT02','Low Teens','Precio Lista 10% Dcto Familiar',11,4750,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('LT03','Low Teens','Precio Lista 20% Dcto Familiar',11,4220,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('MT01','Mid Teens','Precio Lista',12,5500,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('MT02','Mid Teens','Precio Lista 10% Dcto Familiar',12,4960,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('MT03','Mid Teens','Precio Lista 20% Dcto Familiar',12,4400,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('HT01','High Teens','Precio Lista',13,5840,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('HT02','High Teens','Precio Lista 10% Dcto Familiar',13,5260,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('HT03','High Teens','Precio Lista 20% Dcto Familiar',13,4670,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('AA01','Acelerated A','Precio Lista',14,5280,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('AA02','Acelerated A','Precio Lista 10% Dcto Familiar',14,4750,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('AA03','Acelerated A','Precio Lista 20% Dcto Familiar',14,4230,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('AB01','Acelerated B','Precio Lista',15,5840,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('AB02','Acelerated B','Precio Lista 10% Dcto Familiar',15,5260,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('AB03','Acelerated B','Precio Lista 20% Dcto Familiar',15,4670,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('AC01','Acelerated C','Precio Lista',16,5840,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('AC02','Acelerated C','Precio Lista 10% Dcto Familiar',16,5260,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('AC03','Acelerated C','Precio Lista 20% Dcto Familiar',16,4670,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL101','Blended 1','Precio Lista',17,7120,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL102','Blended 1','Precio Lista 10% Dcto Familiar',17,6410,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL103','Blended 1','Precio Lista 20% Dcto Familiar',17,5700,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL201','Blended 2','Precio Lista',18,7120,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL202','Blended 2','Precio Lista 10% Dcto Familiar',18,6410,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL203','Blended 2','Precio Lista 20% Dcto Familiar',18,5700,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL301','Blended 3','Precio Lista',19,7120,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL302','Blended 3','Precio Lista 10% Dcto Familiar',19,6410,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL303','Blended 3','Precio Lista 20% Dcto Familiar',19,5700,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL401','Blended 4','Precio Lista',20,7120,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL402','Blended 4','Precio Lista 10% Dcto Familiar',20,6410,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL403','Blended 4','Precio Lista 20% Dcto Familiar',20,5700,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL501','Blended 5','Precio Lista',21,7120,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL502','Blended 5','Precio Lista 10% Dcto Familiar',21,6410,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL503','Blended 5','Precio Lista 20% Dcto Familiar',21,5700,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL601','Blended 6','Precio Lista',22,7120,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL602','Blended 6','Precio Lista 10% Dcto Familiar',22,6410,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('BL603','Blended 6','Precio Lista 20% Dcto Familiar',22,5700,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('MA01','Matricula','Precio Lista',23,4200,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('MA02','Matricula','Precio Lista 10% Dcto Familiar',23,3780,1)

insert into PRODUCTO(Codigo,Nombre,Descripcion,IdCategoria,Precio,Estado)
values ('MA03','Matricula','Precio Lista 20% Dcto Familiar',23,3360,1)




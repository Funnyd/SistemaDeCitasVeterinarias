
CREATE DATABASE SISTEMA_CITAS_VETERINARIAS

GO

USE SISTEMA_CITAS_VETERINARIAS

GO



CREATE TABLE CLIENTES (
    Id int PRIMARY KEY identity (1,1) not null,
    Nombre varchar(20) not null,
	Telefono varchar(20) not null,
	Correo varchar(30) not null,
);

CREATE TABLE VETERINARIOS (
    Id int PRIMARY KEY identity (1,1) not null,
    Nombre varchar(20) not null,
	Especialidad varchar(20) not null,
	Activo bit not null,
);

CREATE TABLE MASCOTAS (
    Id int PRIMARY KEY identity (1,1) not null,
    Nombre varchar(20) not null,
	Especie varchar(20) not null,
	Raza varchar(20) not null,
	ClienteId INT FOREIGN KEY REFERENCES Clientes(Id) not null
);

CREATE TABLE CITAS (
    Id int PRIMARY KEY identity (1,1) not null,
    MascotaId INT FOREIGN KEY REFERENCES Mascotas(Id) not null,
	VeterinarioId INT FOREIGN KEY REFERENCES Veterinarios(Id) not null,
    FechaHora DATETIME not null,
    Motivo NVARCHAR(200) not null,
    Estado NVARCHAR(20) DEFAULT 'Pendiente' not null,
	Diagnostico NVARCHAR(100) ,
    Tratamiento NVARCHAR(100)
);

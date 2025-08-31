CREATE DATABASE BANCO;
GO

USE BANCO;
GO

CREATE SCHEMA test;
GO

CREATE TABLE test.Persona
(
    PersonaId          BIGINT        IDENTITY(1,1) NOT NULL,
    Nombre             NVARCHAR(120)               NOT NULL,
    Genero             CHAR(1)                     NULL
        CONSTRAINT CK_Persona_Genero CHECK (Genero IN ('M','F','O') OR Genero IS NULL),
    Edad               INT                         NULL
        CONSTRAINT CK_Persona_Edad CHECK (Edad IS NULL OR (Edad >= 0 AND Edad <= 130)),
    Identificacion     VARCHAR(20)                 NOT NULL,
    Direccion          NVARCHAR(250)               NULL,
    Telefono           VARCHAR(25)                 NULL,
	EliminadoPersona          BIT           NOT NULL CONSTRAINT DF_Persona_Eliminado DEFAULT(0),
   
    CONSTRAINT PK_Persona PRIMARY KEY CLUSTERED (PersonaId)
);
GO

CREATE UNIQUE INDEX UX_Persona_Identificacion_NoEliminada
ON test.Persona (Identificacion)
WHERE EliminadoPersona = 0;
GO

CREATE TABLE test.Cliente
(
    ClienteIdPersona   BIGINT        NOT NULL,  
    CodigoCliente      VARCHAR(20)   NOT NULL,
    Contrasenia     NVARCHAR(256) NOT NULL,
    Estado            BIT           NOT NULL CONSTRAINT DF_Cliente_Eliminado DEFAULT(0)


    CONSTRAINT PK_Cliente PRIMARY KEY CLUSTERED (ClienteIdPersona),
    CONSTRAINT FK_Cliente_Persona FOREIGN KEY (ClienteIdPersona)
        REFERENCES test.Persona(PersonaId) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX UX_Cliente_Codigo_NoEliminado
ON test.Cliente (CodigoCliente)
WHERE Estado = 0;
GO

CREATE TABLE test.Cuenta
(
    CuentaId           BIGINT        IDENTITY(1,1) NOT NULL,
    ClienteIdPersona   BIGINT                      NOT NULL,
    NumeroCuenta       VARCHAR(20)                 NOT NULL,
    TipoCuenta         VARCHAR(15)                 NOT NULL
        CONSTRAINT CK_Cuenta_Tipo CHECK (TipoCuenta IN ('Ahorro','Corriente')),
    SaldoInicial       DECIMAL(18,2)               NOT NULL DEFAULT(0),
    Estado            BIT           NOT NULL CONSTRAINT DF_Cuenta_Eliminado DEFAULT(0),
    FechaApertura      DATETIME2(0)                NOT NULL DEFAULT (SYSUTCDATETIME()),

   

    CONSTRAINT PK_Cuenta PRIMARY KEY CLUSTERED (CuentaId),
    CONSTRAINT FK_Cuenta_Cliente FOREIGN KEY (ClienteIdPersona)
        REFERENCES test.Cliente(ClienteIdPersona) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX UX_Cuenta_Numero_NoElimnado
ON test.Cuenta (NumeroCuenta)
WHERE Estado = 0;
GO


CREATE TABLE test.Movimiento
(
    MovimientoId       BIGINT        IDENTITY(1,1) NOT NULL,
    CuentaId           BIGINT                      NOT NULL,
    Fecha              DATETIME2(0)                NOT NULL DEFAULT (SYSUTCDATETIME()),
    TipoMovimiento     CHAR(1)                     NOT NULL
        CONSTRAINT CK_Mov_Tipo CHECK (TipoMovimiento IN ('C','D')),
    Valor              DECIMAL(18,2)               NOT NULL
        CONSTRAINT CK_Mov_Valor_Pos CHECK (Valor > 0),
    MovimientoNeto      AS (CASE WHEN TipoMovimiento='C' THEN Valor ELSE -Valor END) PERSISTED,
   
    Anulado    BIT   NOT NULL CONSTRAINT DF_Mov_Anulado DEFAULT(0)
   
    CONSTRAINT PK_Movimiento PRIMARY KEY CLUSTERED (MovimientoId),
    CONSTRAINT FK_Movimiento_Cuenta FOREIGN KEY (CuentaId)
        REFERENCES test.Cuenta(CuentaId) ON DELETE CASCADE
);
GO

CREATE INDEX IX_Movimiento_Cuenta_Fecha_NoAnulado
ON test.Movimiento (CuentaId, Fecha)
WHERE Anulado = 0;
GO

CREATE TABLE test.ParametroSistema
(
    ParametroId   INT           IDENTITY(1,1) NOT NULL,
    Clave         VARCHAR(50)                 NOT NULL,
    ValorDecimal  DECIMAL(18,2)               NULL
   

    CONSTRAINT PK_Parametro PRIMARY KEY CLUSTERED (ParametroId)
);
GO

CREATE UNIQUE INDEX UX_Parametro_Clave
ON test.ParametroSistema (Clave);
GO


INSERT INTO test.ParametroSistema (Clave, ValorDecimal)
VALUES ('LIMITE_RETIRO_DIARIO', 1000.00);
GO
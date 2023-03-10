USE [master]
GO
CREATE DATABASE [VideoClub]
GO
ALTER DATABASE [VideoClub] SET COMPATIBILITY_LEVEL = 150
GO
USE [VideoClub]
GO
CREATE TABLE [dbo].[Account] (
    [login]        NVARCHAR (10) NOT NULL,
    [password]     NVARCHAR (10) NOT NULL,
    [is_president] BIT           CONSTRAINT [DF_Account_is_president] DEFAULT ((0)) NOT NULL,
    [is_sysadmin]  BIT           CONSTRAINT [DF_Account_is_sysadmin] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED ([login] ASC)
);
GO
CREATE TABLE [dbo].[Film] (
    [id]          INT            IDENTITY (1, 1) NOT NULL,
    [name]        NVARCHAR (50)  NOT NULL,
    [director]    NVARCHAR (50)  NOT NULL,
    [actor1]      NVARCHAR (50)  NULL,
    [actor2]      NVARCHAR (50)  NULL,
    [year]        INT            NOT NULL,
    [studio]      NVARCHAR (50)  NULL,
    [description] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Film] PRIMARY KEY CLUSTERED ([id] ASC)
);
GO
CREATE TABLE [dbo].[Cassette] (
    [id]         INT IDENTITY (1, 1) NOT NULL,
    [film_id]    INT NOT NULL,
    [views_left] INT CONSTRAINT [DF_Cassette_views_left] DEFAULT ((30)) NOT NULL,
    CONSTRAINT [PK_Cassette] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Cassette_Film] FOREIGN KEY ([film_id]) REFERENCES [dbo].[Film] ([id])
);
GO
CREATE TABLE [dbo].[Checking] (
    [id]            INT            IDENTITY (1, 1) NOT NULL,
    [cassette_id]   INT            NOT NULL,
    [checked]       DATETIME       CONSTRAINT [DF_Checking_checked] DEFAULT (getdate()) NOT NULL,
    [defects]       NVARCHAR (MAX) NULL,
    [defects_fixed] BIT            CONSTRAINT [DF_Checking_defects_fixed] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Checking] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Checking_Cassette] FOREIGN KEY ([cassette_id]) REFERENCES [dbo].[Cassette] ([id])
);
GO
CREATE TABLE [dbo].[Librarian] (
    [id]    INT           IDENTITY (1, 1) NOT NULL,
    [login] NVARCHAR (10) NOT NULL,
    [name]  NVARCHAR (50) NOT NULL,
    [phone] VARCHAR (15)  NOT NULL,
    CONSTRAINT [PK_Librarian] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Librarian_Account] FOREIGN KEY ([login]) REFERENCES [dbo].[Account] ([login])
);
GO
CREATE TABLE [dbo].[Member] (
    [id]    INT           IDENTITY (1, 1) NOT NULL,
    [login] NVARCHAR (10) NULL,
    [name]  NVARCHAR (50) NOT NULL,
    [phone] VARCHAR (15)  NOT NULL,
    [fund]  INT           CONSTRAINT [DF_Member_fund] DEFAULT ((50)) NOT NULL,
    CONSTRAINT [PK_Member] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Member_Account] FOREIGN KEY ([login]) REFERENCES [dbo].[Account] ([login])
);
GO
CREATE TABLE [dbo].[Order] (
    [id]            INT            IDENTITY (1, 1) NOT NULL,
    [cassette_id]   INT            NOT NULL,
    [member_id]     INT            NOT NULL,
    [ordered]       DATETIME       CONSTRAINT [DF_Order_ordered] DEFAULT (getdate()) NOT NULL,
    [issued]        DATETIME       NULL,
    [returned]      DATETIME       NULL,
    [defects]       NVARCHAR (MAX) NULL,
    [defects_fixed] BIT            CONSTRAINT [DF_Order_defects_fixed] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([id] ASC),
    CONSTRAINT [FK_Order_Cassette] FOREIGN KEY ([cassette_id]) REFERENCES [dbo].[Cassette] ([id]),
    CONSTRAINT [FK_Order_Member] FOREIGN KEY ([member_id]) REFERENCES [dbo].[Member] ([id])
);
GO
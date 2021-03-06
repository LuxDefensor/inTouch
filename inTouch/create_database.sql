USE [master]
GO
/****** Object:  Database [MessageHub]    Script Date: 06/29/2016 13:26:13 ******/
CREATE DATABASE [MessageHub] ON  PRIMARY 
( NAME = N'MessageHub', FILENAME = N'C:\DATA\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\MessageHub.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'MessageHub_log', FILENAME = N'C:\DATA\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\MessageHub_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
EXEC dbo.sp_dbcmptlevel @dbname=N'MessageHub', @new_cmptlevel=90
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [MessageHub].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [MessageHub] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [MessageHub] SET ANSI_NULLS OFF
GO
ALTER DATABASE [MessageHub] SET ANSI_PADDING OFF
GO
ALTER DATABASE [MessageHub] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [MessageHub] SET ARITHABORT OFF
GO
ALTER DATABASE [MessageHub] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [MessageHub] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [MessageHub] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [MessageHub] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [MessageHub] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [MessageHub] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [MessageHub] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [MessageHub] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [MessageHub] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [MessageHub] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [MessageHub] SET  DISABLE_BROKER
GO
ALTER DATABASE [MessageHub] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [MessageHub] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [MessageHub] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [MessageHub] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [MessageHub] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [MessageHub] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [MessageHub] SET  READ_WRITE
GO
ALTER DATABASE [MessageHub] SET RECOVERY FULL
GO
ALTER DATABASE [MessageHub] SET  MULTI_USER
GO
ALTER DATABASE [MessageHub] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [MessageHub] SET DB_CHAINING OFF
GO
EXEC sys.sp_db_vardecimal_storage_format N'MessageHub', N'ON'
GO
USE [MessageHub]
GO
/****** Object:  User [arm]    Script Date: 06/29/2016 13:26:13 ******/
CREATE USER [arm] FOR LOGIN [arm] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[Messages]    Script Date: 06/29/2016 13:26:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Messages](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Message] [nvarchar](max) NULL,
	[Stamp] [datetime] NOT NULL,
	[Address] [int] NULL,
 CONSTRAINT [PK_Messages] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 06/29/2016 13:26:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[UserList]    Script Date: 06/29/2016 13:26:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[UserList]
(	
)
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	SELECT Users.UserName FROM Users

)
GO
/****** Object:  StoredProcedure [dbo].[SetUserStatus]    Script Date: 06/29/2016 13:26:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[SetUserStatus]
	@add_user bit
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    IF @add_user=1
		INSERT INTO Users([UserName]) 
		SELECT SUSER_NAME()
	ELSE
		DELETE FROM Users
		WHERE [UserName]=SUSER_NAME()
END
GO

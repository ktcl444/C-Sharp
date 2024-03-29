
Use Master
if exists (select * from dbo.sysdatabases where name = N'miisGroupManagement')
DROP DATABASE miisGroupManagement
Go

CREATE DATABASE miisGroupManagement
Go

use [miisGroupManagement]
GO

CREATE TABLE [dbo].[attributeGroupDefinitions] (
	[objectUID] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[uniqueGroupID] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[displayName] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[attributeGroupType] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[attribute] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[linkAttribute] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[linkAttributeKey] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[groupType] [int] NULL ,
	[mailEnabled] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[clauseDefinitions] (
	[objectUID] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[clauseAutoUID] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[clauseType] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[clause] [varchar] (5120) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[exceptionDefinitions] (
	[objectUID] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[exceptType] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[mvObjectUID] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[groupDefinitions] (
	[objectUID] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[groupAutoUID] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[objectType] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[displayName] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[description] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[clauseLink] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[enabledFlag] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[maxExcept] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[preserveMembers] [int] NULL ,
	[groupType] [int] NULL ,
	[mailNickName] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[groupDefinitions_delta] (
	[objectUID] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[groupAutoUID] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[objectType] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[displayName] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[description] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[clauseLink] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[enabledFlag] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[maxExcept] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[preserveMembers] [int] NULL ,
	[groupType] [int] NULL ,
	[mailNickName] [varchar] (256) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[attributeName] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[changeTime] [datetime] NULL ,
	[changeType] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[memberDefinitions] (
	[objectUID] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[objectType] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[mvObjectUID] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[memberDefinitions_temp] (
	[objectUID] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL ,
	[objectType] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[mvObjectUID] [varchar] (64) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[stagingDefinitions] (
	[executeDateTime] [datetime] NULL ,
	[sqlCommand] [varchar] (5120) COLLATE SQL_Latin1_General_CP1_CI_AS NULL ,
	[comment] [varchar] (2048) COLLATE SQL_Latin1_General_CP1_CI_AS NULL 
) ON [PRIMARY]
GO

CREATE VIEW dbo.vw_groupDefinitions_delta
AS
SELECT     TOP 100 PERCENT *
FROM         dbo.groupDefinitions_delta
ORDER BY changeTime
GO

INSERT INTO [dbo].[attributeGroupDefinitions] (objectUID,uniqueGroupID,displayName,attributeGroupType,attribute,linkAttribute,linkAttributeKey,groupType,mailEnabled) VALUES ('{D8531EEE-9757-4269-8A87-1D6A1A4F4144}','titles','Title of {attributeValue}','single','title','','','8', 'true')
GO
INSERT INTO [dbo].[clauseDefinitions] (objectUID,clauseType,clause) VALUES ('{1DFF74BD-C59F-4DDF-940F-CA49EF919035}','clause','(displayName is not null and displayName <> '''')')
GO
INSERT INTO [dbo].[groupDefinitions] (objectUID,objectType,displayName,description,clauseLink,enabledFlag,maxExcept,preserveMembers,groupType,mailNickName) VALUES ('{1DFF74BD-C59F-4DDF-940F-CA49EF919035}','group','Sample Group','Sample Group Description','{1DFF74BD-C59F-4DDF-940F-CA49EF919035}','enabled','10','0','-2147483640','SampleGroup')
GO
INSERT INTO [dbo].[stagingDefinitions] (executeDateTime,sqlCommand,comment) VALUES ('1/1/2001','','successfulRun')
GO

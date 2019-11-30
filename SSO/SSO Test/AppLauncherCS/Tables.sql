/**** Applications  ****/
DROP TABLE esApps
GO
CREATE TABLE esApps
(
  iAppID int NOT NULL PRIMARY KEY,
  sAppName varchar(255) NOT NULL,
  sDesc varchar(1024) NULL,
  sURL varchar(1024) NOT NULL
)
GO

INSERT INTO esApps VALUES(1, 'Payroll', 'A Payroll Application', 'http://localhost/samples/AppTestCS/AppLogin.aspx')
GO
INSERT INTO esApps VALUES(2, 'Human Resources', 'A Human Resources Application', 'http://localhost/samples/AppTestCS/AppLogin.aspx')
GO
INSERT INTO esApps VALUES(3, 'Web Content', 'Content Management System for Intranet', 'http://localhost/samples/AppTestCS/AppLogin.aspx')
GO
/****************************************************************/

/**** Users  ****/
DROP TABLE esUsers
GO
CREATE TABLE esUsers
(
  iUserID int NOT NULL PRIMARY KEY,
  sLoginID varchar(255) NOT NULL
)
GO

INSERT INTO esUsers VALUES(1, 'PSheriff')
GO
INSERT INTO esUsers VALUES(2, 'KGetz')
GO
/****************************************************************/

/**** Application Users  ****/
DROP TABLE esAppsUsers
GO
CREATE TABLE esAppsUsers
(
  iAppUserID int NOT NULL PRIMARY KEY IDENTITY(1, 1),
  iAppID int NOT NULL FOREIGN KEY REFERENCES esApps(iAppID),
  iUserID int NOT NULL FOREIGN KEY REFERENCES esUsers(iUserID)
)
GO

INSERT INTO esAppsUsers(iAppID, iUserID) VALUES(1, 1)
GO
INSERT INTO esAppsUsers(iAppID, iUserID) VALUES(2, 1)
GO
INSERT INTO esAppsUsers(iAppID, iUserID) VALUES(2, 2)
GO
INSERT INTO esAppsUsers(iAppID, iUserID) VALUES(3, 2)
GO
/****************************************************************/

/**** Application Roles ****/
DROP TABLE esAppRoles
GO
CREATE TABLE esAppRoles
(
  iAppRoleID int NOT NULL PRIMARY KEY IDENTITY(1, 1),
  iAppID int NOT NULL FOREIGN KEY REFERENCES esApps(iAppID),
  sRole varchar(50) NOT NULL
)
GO
SET IDENTITY_INSERT esAppRoles ON

INSERT INTO esAppRoles(iAppRoleID, iAppID, sRole) VALUES(1, 1, 'Admin')
GO
INSERT INTO esAppRoles(iAppRoleID, iAppID, sRole) VALUES(2, 1, 'User')
GO
INSERT INTO esAppRoles(iAppRoleID, iAppID, sRole) VALUES(3, 2, 'Admin')
GO
INSERT INTO esAppRoles(iAppRoleID, iAppID, sRole) VALUES(4, 2, 'Supervisor')
GO
INSERT INTO esAppRoles(iAppRoleID, iAppID, sRole) VALUES(5, 3, 'Admin')
GO
INSERT INTO esAppRoles(iAppRoleID, iAppID, sRole) VALUES(6, 3, 'Supervisor')
GO
INSERT INTO esAppRoles(iAppRoleID, iAppID, sRole) VALUES(7, 3, 'Supervisor')
GO
/****************************************************************/

/**** Application Users Roles ****/
DROP TABLE esAppUserRoles
GO
CREATE TABLE esAppUserRoles
(
  iAppUserRoleID int NOT NULL PRIMARY KEY IDENTITY(1, 1),
  iAppID int NOT NULL FOREIGN KEY REFERENCES esApps(iAppID),
  iAppRoleID int NOT NULL FOREIGN KEY REFERENCES esAppRoles(iAppRoleID),
  iUserID int NOT NULL FOREIGN KEY REFERENCES esUsers(iUserID)
)
GO

INSERT INTO esAppUserRoles(iAppID, iAppRoleID, iUserID) VALUES(1, 1, 1)
GO
INSERT INTO esAppUserRoles(iAppID, iAppRoleID, iUserID) VALUES(1, 2, 2)
GO
INSERT INTO esAppUserRoles(iAppID, iAppRoleID, iUserID) VALUES(2, 3, 1)
GO
INSERT INTO esAppUserRoles(iAppID, iAppRoleID, iUserID) VALUES(2, 3, 2)
GO
INSERT INTO esAppUserRoles(iAppID, iAppRoleID, iUserID) VALUES(2, 4, 2)
GO
INSERT INTO esAppUserRoles(iAppID, iAppRoleID, iUserID) VALUES(3, 5, 1)
GO
INSERT INTO esAppUserRoles(iAppID, iAppRoleID, iUserID) VALUES(3, 6, 1)
GO
INSERT INTO esAppUserRoles(iAppID, iAppRoleID, iUserID) VALUES(3, 6, 2)
GO
INSERT INTO esAppUserRoles(iAppID, iAppRoleID, iUserID) VALUES(3, 7, 2)
GO

/****************************************************************/

/**** Application Token  ****/
DROP TABLE esAppToken
GO
CREATE TABLE esAppToken
(
  iAppTokenID int NOT NULL PRIMARY KEY IDENTITY(1, 1),
  sToken varchar(255) NOT NULL,
  sAppName varchar(255) NOT NULL,
  sLoginID varchar(255) NOT NULL,
  iUserID int NOT NULL FOREIGN KEY REFERENCES esUsers(iUserID),
  iAppID int NOT NULL FOREIGN KEY REFERENCES esApps(iAppID),
  dtCreated datetime NOT NULL
)
GO
/****************************************************************/
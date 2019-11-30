-- MySQL dump 10.13  Distrib 5.6.17, for Win64 (x86_64)
--
-- Host: 10.5.23.11    Database: test
-- ------------------------------------------------------
-- Server version	5.5.37-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `mytable`
--

DROP TABLE IF EXISTS `mytable`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mytable` (
  `tbname` varchar(1111) DEFAULT NULL,
  `colname` varchar(1111) DEFAULT NULL,
  `length` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `mytable`
--

LOCK TABLES `mytable` WRITE;
/*!40000 ALTER TABLE `mytable` DISABLE KEYS */;
INSERT INTO `mytable` VALUES ('c_membercard','token',50),('mdl_attribute','AttributeType',1020),('mdl_attribute','DbType',510),('mdl_attribute','DefaultValue',510),('mdl_attribute','DisplayFormat',100),('mdl_attribute','DisplayName',510),('mdl_attribute','LookupUrl',510),('mdl_attribute','MethodFromAttributeOfEntity',510),('mdl_attribute','MobileAttributeType',50),('mdl_attribute','Name',510),('mdl_attribute','OptionsArgs',510),('mdl_attribute','OptionsType',100),('mdl_attribute','RequiredLevel',510),('mdl_attributedesign','AttributeType',1020),('mdl_attributedesign','DbType',510),('mdl_attributedesign','DefaultValue',510),('mdl_attributedesign','DisplayFormat',100),('mdl_attributedesign','DisplayName',510),('mdl_attributedesign','LookupUrl',510),('mdl_attributedesign','MethodFromAttributeOfEntity',510),('mdl_attributedesign','MobileAttributeType',50),('mdl_attributedesign','Name',510),('mdl_attributedesign','OptionsArgs',510),('mdl_attributedesign','OptionsType',100),('mdl_attributedesign','RequiredLevel',510),('mdl_entity','Application',100),('mdl_entity','DisplayName',510),('mdl_entity','EntityClassAssemblyName',510),('mdl_entity','EntityClassTypeName',510),('mdl_entity','Name',510),('mdl_entity','ServiceClassAssemblyName',510),('mdl_entity','ServiceClassTypeName',510),('mdl_entitydesign','Application',100),('mdl_entitydesign','DisplayName',510),('mdl_entitydesign','EntityClassAssemblyName',510),('mdl_entitydesign','EntityClassTypeName',510),('mdl_entitydesign','Name',510),('mdl_entitydesign','ServiceClassAssemblyName',510),('mdl_entitydesign','ServiceClassTypeName',510),('mdl_gridview','Name',100),('mdl_mobpagedesign','Application',20),('mdl_mobpagedesign','PageCode',50),('mdl_mobpagedesign','PageName',200),('mdl_mobpagedesign','SourceTables',500),('mdl_mobuserpagedesign','mdl_mobPageDesignId',36),('mdl_mobuserpagedesign','WeixinId',200),('mdl_param','CustomizedUrl',1000),('mdl_param','GroupName',200),('mdl_param','IntervalUnit',510),('mdl_param','ParamCode',100),('mdl_param','ParamName',200),('mdl_paramapplication','Application',100),('mdl_paramdesign','CustomizedUrl',1000),('mdl_paramdesign','GroupName',200),('mdl_paramdesign','IntervalUnit',510),('mdl_paramdesign','ParamCode',100),('mdl_paramdesign','ParamName',200),('mdl_paramvaluetemplate','Code',508),('mdl_paramvaluetemplate','ParentCode',508),('mdl_paramvaluetemplate','Text',508),('mdl_paramvaluetemplate','Value',508),('mdl_paramvaluetemplatedesign','Code',508),('mdl_paramvaluetemplatedesign','ParentCode',508),('mdl_paramvaluetemplatedesign','Text',508),('mdl_paramvaluetemplatedesign','Value',508),('mdl_pluginassembly','Application',50),('mdl_pluginassemblydesign','Application',50),('mdl_serviceactionbasedesign','CustomEntityName',255),('mdl_serviceactionbasedesign','MethodName',255),('mdl_serviceactionbasedesign','ServiceName',255),('mdl_serviceactionbasedesign','ServiceType',255),('myapplication','AppFullName',50),('myapplication','Application',50),('myapplication','ApplicationIcon',50),('myapplication','ApplicationName',50),('myapplication','Comments',200),('myapplication','DllName',50),('myapplication','FunctionHelpUrl',200),('myapplication','HierarchyCode',100),('myapplication','IconClass',30),('myapplication','IconClassUrl',400),('myapplication','ParentHierarchyCode',100),('myapplication','ReportFilterType',16),('myapplication','Version',16),('myapplicationdesign','AppFullName',50),('myapplicationdesign','Application',50),('myapplicationdesign','ApplicationIcon',50),('myapplicationdesign','ApplicationName',50),('myapplicationdesign','ApplicationUrl',50),('myapplicationdesign','Comments',200),('myapplicationdesign','DllName',50),('myapplicationdesign','FunctionHelpUrl',200),('myapplicationdesign','HierarchyCode',100),('myapplicationdesign','IconClass',30),('myapplicationdesign','IconClassUrl',400),('myapplicationdesign','ParentHierarchyCode',100),('myapplicationdesign','ReportFilterType',16),('myapplicationdesign','Version',16),('mybusinessunit','AreaType',100),('mybusinessunit','BUCode',50),('mybusinessunit','BUFullName',100),('mybusinessunit','BUName',50),('mybusinessunit','Charter',50),('mybusinessunit','Comments',500),('mybusinessunit','CompanyAddr',100),('mybusinessunit','CorporationDeputy',20),('mybusinessunit','Fax',20),('mybusinessunit','HierarchyCode',500),('mybusinessunit','NamePath',1000),('mybusinessunit','OrderCode',20),('mybusinessunit','OrderHierarchyCode',500),('mybusinessunit','RefStationName',1000),('mybusinessunit','WebSite',50),('myfunction','Application',100),('myfunction','Assembly',100),('myfunction','Comments',400),('myfunction','FunctionBigIcon',200),('myfunction','FunctionHelpUrl',400),('myfunction','FunctionIcon',200),('myfunction','FunctionName',100),('myfunction','FunctionType',32),('myfunction','FunctionUrl',400),('myfunction','HierarchyCode',1000),('myfunction','IconClass',60),('myfunction','IconClassUrl',400),('myfunction','MainTable',100),('myfunction','ParentHierarchyCode',1000),('myfunctiondesign','Application',100),('myfunctiondesign','Assembly',100),('myfunctiondesign','Comments',400),('myfunctiondesign','FunctionBigIcon',200),('myfunctiondesign','FunctionHelpUrl',400),('myfunctiondesign','FunctionIcon',200),('myfunctiondesign','FunctionName',100),('myfunctiondesign','FunctionType',32),('myfunctiondesign','FunctionUrl',400),('myfunctiondesign','HierarchyCode',1000),('myfunctiondesign','IconClass',60),('myfunctiondesign','IconClassUrl',400),('myfunctiondesign','MainTable',100),('myfunctiondesign','ParentHierarchyCode',1000),('myorganizationlevel','Level',100),('myparamvalue','Code',508),('myparamvalue','Description',400),('myparamvalue','ParentCode',508),('myparamvalue','Text',508),('myparamvalue','Value',508),('myparamvaluehistory','Code',508),('myparamvaluehistory','Description',400),('myparamvaluehistory','ParentCode',508),('myparamvaluehistory','Text',508),('myparamvaluehistory','Value',508),('mysite','SiteName',40),('mysite','SitePath',400),('mystandardrole','Comments',1000),('mystandardrole','HierarchyCode',500),('mystandardrole','OrderCode',16),('mystandardrole','ProfessionalLine',50),('mystandardrole','StandardRoleCode',100),('mystandardrole','StandardRoleName',50),('mystandardrole','StandardRoleType',100),('mystandardroleright','ActionCode',16),('mystandardroleright','Application',50),('mystandardroleright','ObjectType',50),('mystandardroleuserright','ActionCode',16),('mystandardroleuserright','Application',50),('mystandardroleuserright','ObjectType',50),('myuser','ADAccount',50),('myuser','Comments',200),('myuser','DisabledReason',200),('myuser','Email',50),('myuser','HomePhone',20),('myuser','JobNumber',50),('myuser','JobTitle',50),('myuser','MobilePhone',20),('myuser','ModifiedOn',50),('myuser','OfficePhone',20),('myuser','Password',50),('myuser','PhotoUrl',50),('myuser','Position',50),('myuser','UserCode',16),('myuser','UserName',20),('myuser','UserProject',16),('p_project','ParentCode',40),('p_project','ProjCode',100),('p_project','ProjName',400),('p_project2erpproject','Erp_ProjectName',200);
/*!40000 ALTER TABLE `mytable` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `test1`
--

DROP TABLE IF EXISTS `test1`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `test1` (
  `id` int(11) DEFAULT NULL,
  `name` varchar(60) DEFAULT NULL,
  `name2` char(60) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `test1`
--

LOCK TABLES `test1` WRITE;
/*!40000 ALTER TABLE `test1` DISABLE KEYS */;
/*!40000 ALTER TABLE `test1` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `test2`
--

DROP TABLE IF EXISTS `test2`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `test2` (
  `idtest2` int(11) NOT NULL,
  PRIMARY KEY (`idtest2`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `test2`
--

LOCK TABLES `test2` WRITE;
/*!40000 ALTER TABLE `test2` DISABLE KEYS */;
INSERT INTO `test2` VALUES (1),(2);
/*!40000 ALTER TABLE `test2` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'test'
--
/*!50003 DROP PROCEDURE IF EXISTS `new_procedure` */;
/*!50003 SET @saved_cs_client      = @@character_set_client */ ;
/*!50003 SET @saved_cs_results     = @@character_set_results */ ;
/*!50003 SET @saved_col_connection = @@collation_connection */ ;
/*!50003 SET character_set_client  = utf8 */ ;
/*!50003 SET character_set_results = utf8 */ ;
/*!50003 SET collation_connection  = utf8_general_ci */ ;
/*!50003 SET @saved_sql_mode       = @@sql_mode */ ;
/*!50003 SET sql_mode              = 'STRICT_TRANS_TABLES,NO_AUTO_CREATE_USER,NO_ENGINE_SUBSTITUTION' */ ;
DELIMITER ;;
CREATE DEFINER=`sa`@`%` PROCEDURE `new_procedure`()
BEGIN
select 1;
END ;;
DELIMITER ;
/*!50003 SET sql_mode              = @saved_sql_mode */ ;
/*!50003 SET character_set_client  = @saved_cs_client */ ;
/*!50003 SET character_set_results = @saved_cs_results */ ;
/*!50003 SET collation_connection  = @saved_col_connection */ ;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2014-08-04  9:49:19

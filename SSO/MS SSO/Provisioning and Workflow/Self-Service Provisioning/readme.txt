General:
==============================

MIISWorkflow is a resource kit application that provides approval based provisioning functionality to MIIS 2003.


Requirements:
==============================

1) Windows Server 2003 Enterprise Edition
2) Microsoft Identity Integration Server (MIIS) 2003 
3) SQLServer 2000.
4) Internet Information Server (IIS) 6.0
5) ASP.NET windows component

Setup:
==============================

Creating the user on Windows:
---------------

- Open Computer Management.
- Create the new user "MIISWorkflowUser". This is a sample username. You can pick your own username if you want. If you select a different name, make sure you use it in other steps as well.
- Make sure the "User must change password at next logon" option is unchecked.
- Add the new user to IIS_WPG and MIISOperators group.
- Log off and log on again.

Setting up the database:
---------------

- Open Query Analyzer
- Load the SQL query file MIISWorkflowDB.sql.
- Execute the query. 

Assigning database access rights to the user:
---------------

- Open Enterprise Manager of SQL Server.
- Navigate to local SQL Server Group. Click on Security.
- Right click on Logins. Select New Login.
  - In General tab, select <domain>\MIISWorkflowUser as the name.
  - Select MIISWorkflow as the default Database.
- Go to Database Access tab
  - Select MIISWorkflow database.
  - Select public and db_owner as the database role.

Configuring IIS:
---------------

- Go to IIS Home directory (c:\InetPub\wwwroot)
- Create a new folder named "MIISWorkflow"
- Copy the content of MIISWorkflow\www in Resource Kit to the folder you just created.
- Open IIS Manager in Administrative Tools.
- Right click on Application Pools. Select New-Application Pool.
  - Type in "MIISWorkflow" as the Application pool ID.
  - Keep "Use default settings for new application pool" checked.
  - Click OK.
  - Right click on the MIISWorkflow application pool you just created and select Properties.
  - Go to Identity tab.
  - Select "Configurable" and type in <domain>\MIISWorkflowUser as the User Name and the password for that user.
  - After this change, make sure MIISWorkflow application pool is running.
- In IIS Manager, expand Web Sites\Default Web Site.
  - Right click on MIISWorkflow folder and select Properties.
  - In the Directory tab, click on "Create". Application name should become "MIISWorkflow".
  - Select "MIISWorkflow" as the Application Pool.
  - Click on Directory Security tab.
  - Click Edit on Authentication and access control.
  - Uncheck the "Enable anonymous access" option. Keep "Integrated Windows authentication" checked.
  - Click OK to return back to IIS Manager.
- In IIS Manager, right click on Web sites\Default Web Site.
  - Select "Stop" to stop the service.
  - Right click again. This time select "Start" to re-start the service.

Configuring MIIS:
---------------

- Open Identity Manager.
- Import MIISWorkflow MA from the file MIISWorkflowMA.xml.
- Copy MIISWorkflowMA.dll to the "extensions" folder in MIIS installation directory.


Running the application:
==============================

- Open Internet Explorer.
- Browse to http://localhost/MIISWorkflow/ContractorRequest.aspx


Notes:
==============================

1) When running the application the very first time, you might get a message when adding new contractors in ContractorRequest.aspx page saying that no designated approvers are found for the DEPARTMENT string you entered. To fix this, do the following:
  - Open Query Analyzer.
  - Insert your domain\alias and the department information you'd like to use into APPROVAL_PERMISSIONS table. Note that this makes you an approver for the DEPARTMENT you enter. That means you can approve any contractor request made to that DEPARTMENT.
    An example insert command will be as follows:
    insert into APPROVAL_PERMISSIONS values ('domain\alias','department')

2) If you'd like modify the MIISWorkflow application simply double click on the MIISWorkflow.csproj file under c:\inetpub\wwwroot\MIISWorkflow folder to open up the Visual Studio project.


/************************* MIISWorkflow database **************************/
if exists (select NAME from master.dbo.sysdatabases where NAME = N'MIISWorkflow')
  drop database MIISWorkflow
go

create database MIISWorkflow
go

use MIISWorkflow
go

/**************************** CONTRACTORS table ***************************/
create table CONTRACTORS
(
	CONTRACTOR_ID [nvarchar] (10) primary key,
	FIRST_NAME [nvarchar] (30) NOT NULL,
	LAST_NAME [nvarchar] (30) NOT NULL,
	DEPARTMENT [nvarchar] (30) NOT NULL,
	START_DATE [nvarchar] (10) NOT NULL,
	END_DATE [nvarchar] (10) NOT NULL,
	LOCATION [nvarchar] (30) NOT NULL,
	MANAGER_ID [nvarchar] (10) NULL,
	COMPANY [nvarchar] (10) NOT NULL,
	REQUESTER [nvarchar] (30) NOT NULL,
	LAST_MODIFIED [datetime] NULL,
	STATUS [nvarchar] (100) NOT NULL,
	UPDATE_COUNT [int] NOT NULL,
	EMPLOYEE_TYPE [int] NOT NULL, 
)
go


/************************* CONTRACTORS_DELTA table ************************/
create table CONTRACTORS_DELTA
(
	CONTRACTOR_ID [nvarchar] (10) primary key,
	FIRST_NAME [nvarchar] (30) NULL,
	LAST_NAME [nvarchar] (30) NULL,
	DEPARTMENT [nvarchar] (30) NULL,
	START_DATE [nvarchar] (10) NULL,
	END_DATE [nvarchar] (10) NULL,
	LOCATION [nvarchar] (30) NULL,
	MANAGER_ID [nvarchar] (10) NULL,
	COMPANY [nvarchar] (10) NULL,
	REQUESTER [nvarchar] (30) NULL,
	LAST_MODIFIED [datetime] NULL,
	STATUS [nvarchar] (100) NULL,
	UPDATE_COUNT [int] NULL,
	CHANGE_TYPE [nvarchar] (10) NULL,
	PASSWORD [nvarchar] (20) NULL,
	EMPLOYEE_TYPE [int] NOT NULL, 
)
go


/*********************** CONTRACTORS_HISTORY table ************************/
create table CONTRACTORS_HISTORY
(
	CONTRACTOR_ID [nvarchar] (10) NOT NULL,
	FIRST_NAME [nvarchar] (30) NOT NULL,
	LAST_NAME [nvarchar] (30) NOT NULL,
	DEPARTMENT [nvarchar] (30) NOT NULL,
	START_DATE [nvarchar] (10) NOT NULL,
	END_DATE [nvarchar] (10) NOT NULL,
	LOCATION [nvarchar] (30) NOT NULL,
	MANAGER_ID [nvarchar] (10) NULL,
	COMPANY [nvarchar] (10) NOT NULL,
	REQUESTER [nvarchar] (30) NOT NULL,
	LAST_MODIFIED [datetime] NULL,
	STATUS [nvarchar] (100) NOT NULL,
	UPDATE_COUNT [int] NOT NULL,
	timestamp
)
go


/********************** UPDATE trigger on CONTRACTORS *********************/
if exists (select NAME from sysobjects where NAME = 'UPDATE_CONTRACTOR' and TYPE = 'TR')
drop trigger UPDATE_CONTRACTOR
go

create trigger UPDATE_CONTRACTOR
on CONTRACTORS after update 
as

--This trigger should kick in only if the UPDATE_COUNT column is updated
--through an export run on MIIS.
if update (UPDATE_COUNT)
begin

  begin transaction

  declare @contractor_id nvarchar(10)
  declare @manager_id nvarchar(10)
  declare @manager_ref_id nvarchar(10)
  declare @last_modified datetime

  declare @rowcount_var int

  set @contractor_id = (select CONTRACTOR_ID from inserted)
  set @manager_id = (select MANAGER_ID from CONTRACTORS where CONTRACTOR_ID = @contractor_id)
  set @last_modified = (select LAST_MODIFIED from CONTRACTORS where CONTRACTOR_ID = @contractor_id)


  --This trigger is also responsible of doing some cleaning up.
  --Here we first look at the delta table if there are any entries
  --with status equals to 'delete'. If these entries are created before
  --the update operation of this very entry, we can safely say that
  --these 'delete' entries have been processed by MIIS and we could delete them.
  --We need to follow such a cleaning approach because MIIS does not export
  --the imported deletes and there is no timely way to go ahead and delete those
  --rows right away as we do in adds/modifies.
  delete from CONTRACTORS_DELTA
  where CHANGE_TYPE = 'Delete'
    and LAST_MODIFIED < @last_modified

  --This entry has been processed by MIIS. Update the status of the entry.
  update CONTRACTORS
  set STATUS = 'Provisioned'
  where CONTRACTOR_ID = @contractor_id

  --Remove the entry from the delta table because it's already been
  --processed by MIIS.
  delete from CONTRACTORS_DELTA
  where CONTRACTOR_ID = @contractor_id

  -- delete the row that contains the reference entry for the contractor's manager. 
  -- this row was used during import to create the association between contractor's manager_id and the manager object
  -- it is no longer needed once the object has been imported into the CS
  delete from CONTRACTORS_DELTA
  where CONTRACTOR_ID = @manager_id

  --Keep a record in the history table.
  insert into CONTRACTORS_HISTORY
    (
    CONTRACTOR_ID,
    FIRST_NAME,
    LAST_NAME,
    DEPARTMENT,
    START_DATE,
    END_DATE,
    LOCATION,
    MANAGER_ID,
    COMPANY,
    REQUESTER,
    LAST_MODIFIED,
    STATUS,
    UPDATE_COUNT
    )
  select
    CONTRACTOR_ID,
    FIRST_NAME,
    LAST_NAME,
    DEPARTMENT,
    START_DATE,
    END_DATE,
    LOCATION,
    MANAGER_ID,
    COMPANY,
    REQUESTER,
    LAST_MODIFIED,
    STATUS,
    UPDATE_COUNT
  from CONTRACTORS
  where 
    CONTRACTOR_ID = @contractor_id

  -- check and see if the manager reference is already in the 
  -- contractors table. This reference needs to be preserved in
  -- case the connector space or metaverse needs to be rebuilt 
  -- with a full import
  select 
    @manager_ref_id        = CONTRACTOR_ID
  from CONTRACTORS
  where CONTRACTOR_ID  = @manager_id

  --Check if the reference exists
  select @rowcount_var = @@ROWCOUNT
  if @rowcount_var = 0 
  begin
    insert into CONTRACTORS
      (
      CONTRACTOR_ID,
      FIRST_NAME,
      LAST_NAME,
      DEPARTMENT,
      START_DATE,
      END_DATE,
      LOCATION,
      MANAGER_ID,
      COMPANY,
      REQUESTER,
      LAST_MODIFIED,
      STATUS,
      UPDATE_COUNT,
      EMPLOYEE_TYPE
      )
    select
      CONTRACTOR_ID,
      FIRST_NAME,
      LAST_NAME,
      DEPARTMENT,
      START_DATE,
      END_DATE,
      LOCATION,
      MANAGER_ID,
      COMPANY,
      REQUESTER,
      LAST_MODIFIED,
      STATUS,
      UPDATE_COUNT,
      EMPLOYEE_TYPE
    from CONTRACTORS_DELTA
    where 
      CONTRACTOR_ID = @manager_id
  end

  commit transaction

end
go


/***************** Stored procedure : APPROVE_CONTRACTOR *****************/
if exists (select NAME from sysobjects where NAME = 'APPROVE_CONTRACTOR' and TYPE = 'P')
   drop procedure APPROVE_CONTRACTOR
go

create procedure APPROVE_CONTRACTOR
  @contractor_id nvarchar(10),
  @password nvarchar(20) 
as

begin

  declare @first_name nvarchar(30)
  declare @last_name nvarchar(30)
  declare @department nvarchar(30)
  declare @start_date nvarchar(10)
  declare @end_date nvarchar(10)
  declare @location nvarchar(30)
  declare @manager_id nvarchar(10)
  declare @company nvarchar(10)
  declare @requester nvarchar(30)
  declare @last_modified datetime
  declare @status nvarchar(100)
  declare @update_count int
  declare @employee_type int
  declare @manager_ref_id nvarchar(10)


  declare @rowcount_var int

  begin transaction
  
  select 
    @first_name        = FIRST_NAME,
    @last_name         = LAST_NAME,
    @department        = DEPARTMENT,
    @start_date        = START_DATE,
    @end_date          = END_DATE,
    @location          = LOCATION,
    @manager_id        = MANAGER_ID,
    @company           = COMPANY,
    @requester         = REQUESTER,
    @status            = 'Approved',
    @update_count      = UPDATE_COUNT,
    @employee_type     = 1
  from CONTRACTORS
  where CONTRACTOR_ID  = @contractor_id
           and STATUS  = 'Waiting for approval'

  --Check if this stored procedure is called for a valid contractor
  select @rowcount_var = @@ROWCOUNT
  if @rowcount_var = 0 
  begin
     --The above select statement returned 0 rows. That means such a contractor
     --does not exist or if it exists, its status is not 'Waiting for approval'
     rollback transaction
     return(1)
  end

  set @last_modified  = CURRENT_TIMESTAMP

  --Mark the status of the entry in the main table as "Approved"
  update CONTRACTORS
  set STATUS          = @status, 
      LAST_MODIFIED   = @last_modified
  where CONTRACTOR_ID = @contractor_id

  select employee_type = 2

  --..and add a copy to the delta table. This data will be picked up by MIIS.
  insert into CONTRACTORS_DELTA
  values(
    @contractor_id,
    @first_name,
    @last_name,
    @department,
    @start_date,
    @end_date,
    @location,
    @manager_id,
    @company,
    @requester,
    @last_modified,
    @status,
    @update_count,
    'Add',
    @password,
    @employee_type
    )

  -- check and see if the manager reference is already in the delta table
  -- note that the contract ID is overloaded for this purpose
  select 
    @manager_ref_id        = CONTRACTOR_ID
  from CONTRACTORS_DELTA
  where CONTRACTOR_ID  = @manager_id

  --Check if the reference exists
  select @rowcount_var = @@ROWCOUNT
  if @rowcount_var = 0 
  begin
    -- The above select statement returned 0 rows. That means 
    -- that the reference does not exist
    -- add a row to the delta table which is used for referencing
    -- the manager's id (only one such row is needed for each unique
    -- manager)
    insert into CONTRACTORS_DELTA
    values(
      @manager_id,
      null,
      null,
      null,
      null,
      null,
      null,
      null,
      null,
      null,
      null,
      null,
      null,
      'Add',
      null,
      @employee_type)

      
    end

  commit transaction

end
go


/***************** Stored procedure : DENY_CONTRACTOR *****************/
if exists (select NAME from sysobjects where NAME = 'DENY_CONTRACTOR' and TYPE = 'P')
   drop procedure DENY_CONTRACTOR
go

create procedure DENY_CONTRACTOR
  @contractor_id nvarchar(10) 
as

begin

  declare @first_name nvarchar(30)
  declare @last_name nvarchar(30)
  declare @department nvarchar(30)
  declare @start_date nvarchar(10)
  declare @end_date nvarchar(10)
  declare @location nvarchar(30)
  declare @manager_id nvarchar(10)
  declare @company nvarchar(10)
  declare @requester nvarchar(30)
  declare @last_modified datetime
  declare @status nvarchar(100)
  declare @update_count int

  declare @rowcount_var int

  begin transaction
  
  select 
    @first_name       = FIRST_NAME,
    @last_name        = LAST_NAME,
    @department       = DEPARTMENT,
    @start_date       = START_DATE,
    @end_date         = END_DATE,
    @location         = LOCATION,
    @manager_id       = MANAGER_ID,
    @company          = COMPANY,
    @requester        = REQUESTER,
    @update_count     = UPDATE_COUNT
  from CONTRACTORS
  where CONTRACTOR_ID = @contractor_id

  --Check if this stored procedure is called for a valid contractor
  select @rowcount_var = @@ROWCOUNT
  if @rowcount_var = 0 
  begin
     --The above select statement returned 0 rows. That means such a 
     --contractor does not exist
     rollback transaction
     return(1)
  end

  set @last_modified  = CURRENT_TIMESTAMP
  set @status         = 'Denied'

  --Mark the entry as 'Denied' in the main table.
  update CONTRACTORS
  set STATUS          = @status, 
      LAST_MODIFIED   = @last_modified
  where CONTRACTOR_ID = @contractor_id

  --Keep a copy in the history table.
  insert into CONTRACTORS_HISTORY
    (
    CONTRACTOR_ID,
    FIRST_NAME,
    LAST_NAME,
    DEPARTMENT,
    START_DATE,
    END_DATE,
    LOCATION,
    MANAGER_ID,
    COMPANY,
    REQUESTER,
    LAST_MODIFIED,
    STATUS,
    UPDATE_COUNT
    )
  values
    (
    @contractor_id,
    @first_name,
    @last_name,
    @department,
    @start_date,
    @end_date,
    @location,
    @manager_id,
    @company,
    @requester,
    @last_modified,
    @status,
    @update_count
    )

  commit transaction

end
go


/***************** Stored procedure : TERMINATE_CONTRACTOR *****************/
if exists (select NAME from sysobjects where NAME = 'TERMINATE_CONTRACTOR' and TYPE = 'P')
   drop procedure TERMINATE_CONTRACTOR
go

create procedure TERMINATE_CONTRACTOR
  @contractor_id nvarchar(10) 
as

begin

  declare @first_name nvarchar(30)
  declare @last_name nvarchar(30)
  declare @department nvarchar(30)
  declare @start_date nvarchar(10)
  declare @end_date nvarchar(10)
  declare @location nvarchar(30)
  declare @manager_id nvarchar(10)
  declare @company nvarchar(10)
  declare @requester nvarchar(30)
  declare @last_modified datetime
  declare @status nvarchar(100)
  declare @update_count int
  declare @password nvarchar(20)
  declare @employee_type int

  declare @rowcount_var int

  begin transaction
  
  select 
    @first_name       = FIRST_NAME,
    @last_name        = LAST_NAME,
    @department       = DEPARTMENT,
    @start_date       = START_DATE,
    @end_date         = END_DATE,
    @location         = LOCATION,
    @manager_id       = MANAGER_ID,
    @company          = COMPANY,
    @requester        = REQUESTER,
    @status           = STATUS,
    @update_count     = UPDATE_COUNT,
    @employee_type    = EMPLOYEE_TYPE
  from CONTRACTORS
  where CONTRACTOR_ID = @contractor_id

  --Check if this stored procedure is called for a valid contractor
  select @rowcount_var = @@ROWCOUNT
  if @rowcount_var = 0 
  begin
     --The above select statement returned 0 rows. That means such a 
     --contractor does not exist
     rollback transaction
     return(1)
  end

  set @last_modified  = CURRENT_TIMESTAMP

  if @status='Approved' or 
     @status='Terminated'
    begin
      --This entry is being processed by MIIS or already has been terminated.
      --Do not allow termination.
      rollback transaction
      return(1)
    end
  else
    begin
      --Check if there exists any history for this contractor that contains an approval.
      if (select count(*) from CONTRACTORS_HISTORY
          where CONTRACTOR_ID=@contractor_id
            and STATUS='Provisioned') > 0
        begin
          --This entry has been approved at least once. 
          --Hence it was submitted to MIIS. We should send an explicit 'delete' 
          --for this contractor to MIIS.
          insert into CONTRACTORS_DELTA
          values(
            @contractor_id,
            @first_name,
            @last_name,
            @department,
            @start_date,
            @end_date,
            @location,
            @manager_id,
            @company,
            @requester,
            @last_modified,
            @status,
            @update_count,
	    'Delete',
            @password,
            @employee_type)
        end
 
      --Mark this contractor as 'Terminated'.
      update CONTRACTORS
      set STATUS          = 'Terminated', 
          LAST_MODIFIED   = @last_modified
      where CONTRACTOR_ID = @contractor_id
      
      --Keep a copy in the history table.
      insert into CONTRACTORS_HISTORY
	(
	CONTRACTOR_ID,
	FIRST_NAME,
	LAST_NAME,
	DEPARTMENT,
	START_DATE,
	END_DATE,
	LOCATION,
	MANAGER_ID,
	COMPANY,
	REQUESTER,
	LAST_MODIFIED,
	STATUS,
	UPDATE_COUNT
	)
      values
        (
        @contractor_id,
        @first_name,
        @last_name,
        @department,
        @start_date,
        @end_date,
        @location,
        @manager_id,
        @company,
        @requester,
        @last_modified,
        'Terminated',
        @update_count
        )

      commit transaction

    end
end
go

/*********** View: CONTRACTORS_WITH_START_AND_END_DATE**********/
CREATE VIEW dbo.CONTRACTORS_WITH_START_AND_END_DATE
AS
SELECT     *
FROM         dbo.CONTRACTORS
WHERE     (CAST(START_DATE AS datetime) <= GETDATE()) AND (CAST(END_DATE AS datetime) > GETDATE())
go

/*********** View: CONTRACTOR_DELTA_GREATER_THEN_START_DATE**********/
CREATE VIEW dbo.CONTRACTOR_DELTA_GREATER_THEN_START_DATE
AS
SELECT     dbo.CONTRACTORS_DELTA.*
FROM         dbo.CONTRACTORS_DELTA
WHERE     (CAST(START_DATE AS DateTime) <= GETDATE())
go
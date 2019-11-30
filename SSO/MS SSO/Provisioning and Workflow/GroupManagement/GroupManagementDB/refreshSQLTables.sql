-- This file can be used to refresh the tables in a test environment.
-- It deletes all tables and rows other than the definintions.

truncate table exceptionDefinitions
truncate table stagingDefinitions
truncate table groupDefinitions_delta
truncate table memberDefinitions_temp
truncate table memberDefinitions
delete from groupDefinitions where objectType = 'person'
delete from clauseDefinitions where clauseType = 'clauseAuto'
delete from groupDefinitions where objectType = 'groupAuto'

using System;
using System.IO;
using MySql.Data.MySqlClient;

namespace MySqlOperation
{
    public class DatabaseRestoreProcessor
    {
        public void ThreadProcess(DatabaseRestoreArgument argument)
        {
            this.CreateRestorDataBase(argument);
            var command = string.Format("mysql.exe --host={0} --default-character-set=utf8 --port={1} --user={2} --password={3} {4}<\"{5}\"",
                                            argument.InstanceId, argument.Port, argument.UserId, argument.UserPassword, argument.RestoreDatabaseName, Path.Combine(argument.BackupUrl, argument.DatabaseName + ".sql"));

            var result = new CMD().StartCmd(System.Configuration.ConfigurationManager.AppSettings["MySqlAppPath"], command);

            var arr = result.Split('\n');

            if (arr.Length != 1)
            {
                for (var i = 0; i < arr.Length - 1; i++)
                {
                    if (!arr[i].StartsWith("Warning:"))
                    {
                        throw new Exception(result);
                    }
                }
            }
        }

        private void CreateRestorDataBase(DatabaseRestoreArgument argument)
        {
            var sql = string.Format("drop database if EXISTS {0};create database {0};", argument.RestoreDatabaseName);
            var connectionstring = string.Format("Data Source={0};Persist Security Info=yes;UserId={1}; PWD={2};",
                                                 argument.InstanceId, argument.UserId, argument.UserPassword);
            using (var conn = new MySqlConnection(connectionstring))
            {
                var cmd = new MySqlCommand(sql, conn);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}

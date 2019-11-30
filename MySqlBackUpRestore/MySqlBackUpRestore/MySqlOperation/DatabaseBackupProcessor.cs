using System;
using System.Configuration;
using System.IO;

namespace MySqlOperation
{
    public class DatabaseBackupProcessor 
    {
        public void ThreadProcess(DatabaseBackupArgument argument)
        {
            var filePath = this.InitBackUpFilePath(argument);
            var command = string.Format(@"mysqldump.exe -h{0} -P{1} -u{2} -p{3} --default-character-set=utf8 --routines {4} > ""{5}""",
                                              argument.InstanceId, argument.Port, argument.UserId, argument.UserPassword, argument.DatabaseName, filePath);

            var result = new CMD().StartCmd(ConfigurationManager.AppSettings["MySqlAppPath"], command);

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

        private string InitBackUpFilePath(DatabaseBackupArgument argument)
        {
            if(!Directory.Exists(argument.BackupUrl))
            {
                Directory.CreateDirectory(argument.BackupUrl);
            }
            var filePath = Path.Combine(argument.BackupUrl, argument.DatabaseName + ".sql");
            if(File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return filePath;
        }

    }
}

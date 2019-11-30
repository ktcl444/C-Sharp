using System.Diagnostics;

namespace MySqlOperation
{
    public class CMD
    {
        public string StartCmd(string workingDirectory, string command)
        {
            var p = new Process
                        {
                            StartInfo =
                                {
                                    FileName = "cmd.exe",
                                    WorkingDirectory = workingDirectory,
                                    UseShellExecute = false,
                                    RedirectStandardInput = true,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = true
                                },
                            EnableRaisingEvents = true
                        };

            p.Start();
            p.StandardInput.WriteLine(command);
            p.StandardInput.WriteLine("exit");

            // Warning: Using a password on the command line interface can be insecure.
            // mysqldump: Got error: 2003: Can't connect to MySQL server on '10.5.23.12' (10060) when trying to connect
            return p.StandardError.ReadToEnd();
        }
    }
}

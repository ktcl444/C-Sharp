using System;
using System.IO;

namespace MySqlBackUpRestore
{
    public partial class DBOperation : System.Web.UI.Page
    {
        private string _backupPath = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            _backupPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.Configuration.ConfigurationManager.AppSettings["BackupFileFolder"]);
        }

        protected void BtnBackUpClick(object sender, EventArgs e)
        {
            this.txtResult.Text = "数据库备份中...";
            var backupArgument = new MySqlOperation.DatabaseBackupArgument
                                     {
                                         InstanceId = this.txtServerName.Text.Trim(),
                                         Port = this.txtPort.Text.Trim(),
                                         UserId = this.txtUserName.Text.Trim(),
                                         UserPassword = this.txtPassword.Text.Trim(),
                                         DatabaseName = this.txtDataBaseName.Text.Trim(),
                                         BackupUrl = this._backupPath
                                     };

            try
            {
                new MySqlOperation.DatabaseBackupProcessor().ThreadProcess(backupArgument);
                this.txtResult.Text = "数据库备份成功.";
            }
            catch (Exception ex)
            {
                this.txtResult.Text = string.Format("数据库1备份失败:{0}", ex);
            }

            backupArgument.DatabaseName = this.txtDataBaseName0.Text.Trim();

            try
            {
                new MySqlOperation.DatabaseBackupProcessor().ThreadProcess(backupArgument);
                this.txtResult.Text = this.txtResult.Text + Environment.NewLine + "数据库2备份成功.";
            }
            catch (Exception ex)
            {
                this.txtResult.Text = string.Format("数据库备份失败:{0}", ex);
            }
        }

        protected void BtnRestoreClick(object sender, EventArgs e)
        {
            this.txtResult.Text = "数据库还原中...";
            var restoreArgument = new MySqlOperation.DatabaseRestoreArgument
                                      {
                                          InstanceId = this.txtServerName.Text.Trim(),
                                          Port = this.txtPort.Text.Trim(),
                                          UserId = this.txtUserName.Text.Trim(),
                                          UserPassword = this.txtPassword.Text.Trim(),
                                          DatabaseName = this.txtDataBaseName.Text.Trim(),
                                          RestoreDatabaseName = this.txtRestoreDataBaseName.Text.Trim(),
                                          BackupUrl = this._backupPath
                                      };

            try
            {
                new MySqlOperation.DatabaseRestoreProcessor().ThreadProcess(restoreArgument);
                this.txtResult.Text = "数据库还原成功.";
            }
            catch (Exception ex)
            {
                this.txtResult.Text = string.Format("数据库还原失败:{0}", ex);
            }
        }
    }
}
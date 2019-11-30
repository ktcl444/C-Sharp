namespace MySqlOperation
{
    /// <summary>
    /// 数据库备份参数
    /// </summary>
    public class DatabaseBackupArgument
    {
        /// <summary>
        /// 实例名称
        /// </summary>
        public string InstanceId { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        public string Port { get; set; }

        /// <summary>
        /// 数据库账户名称
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 数据库账户密码
        /// </summary>
        public string UserPassword { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// 备份路径
        /// </summary>
        public string BackupUrl { get; set; }
    }
}

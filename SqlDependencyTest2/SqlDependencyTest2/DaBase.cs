using System;

using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Caching;

/// <summary>
/// 要使用SqlServer2005 Service Broker，首先要在使用的数据库执行 ALTER DATABASE AdventureWorks SET ENABLE_BROKER 以启用该功能，执行时必须关闭所有可能锁表的操作和作业
/// 使用依赖的sql语句，不能用*，不能用top，不能用函数，包括聚合函数，不能用子查询，包括where后的子查询，
/// 不能用外连接，自连接，不能用临时表，不能用变量，不能用视图，不能垮库，表名之前必须加类似dbo数据库所有者这样的前缀
/// 依赖只针提供一次通知，所以重新发起某次查询则需要重新提供依赖sql语句
/// 
/// 优点：此应用比较适合访问次数大于更新次数的情况，访问次数比更新次数越多，速度提升越明显
/// 缺点：对服务器内存要求较高
/// </summary>
public abstract class DaBase
{
    private SqlDbHelper sqlDbHelper;
    //
    private Cache pageCache;

    public DaBase(Cache cache)
    {
        sqlDbHelper = SqlDbHelper.Instance();
        this.pageCache = cache;
    }

    /// <summary>
    /// 清除缓存
    /// </summary>
    /// <param name="cacheName">缓存名称</param>
    protected virtual void ClearCache(string cacheName)
    {
        System.Collections.IDictionaryEnumerator cacheEnum = pageCache.GetEnumerator();
        while (cacheEnum.MoveNext())
        {
            // 只清除与此业务相关的缓存，根据表名
            if (cacheEnum.Key.ToString().ToLower().IndexOf(cacheName.ToLower()) > 0)
                pageCache.Remove(cacheEnum.Key.ToString());
        }
    }

    /// <summary>
    /// 创建Service Borker通知(请确认Service Borker已开启)，自动响应程序表发生的更改，自动设定缓存机制
    /// </summary>
    /// <param name="pageCache">System.Web.Caching.Cache对象</param>
    /// <param name="selectSql">查询数据的sql语句</param>
    /// <param name="dbOwner">数据库表所有者</param>
    /// <param name="tableName">表名</param>
    /// <param name="column">列名，随意某个小列(最好是bit,tinyint,varchar(1),int)</param>
    /// <returns></returns>
    protected virtual DataTable GetDataTable(string selectSql, string dbOwner, string tableName, string column)
    {
        // 用于Service Broker跟踪的表范围sql
        string depSql = string.Format("select {0} from {1}.{2}", column, dbOwner, tableName);

        DataTable dt = new DataTable();
        if (pageCache[selectSql] != null)
            dt = pageCache[selectSql] as DataTable;
        else
        {
            // 触发行级依赖，如果该表的指定范围内的行被修改，则会收到SqlServer的通知，并且清空相应缓存
            SqlDependency sqlDep = sqlDbHelper.AddSqlDependency(depSql,
                delegate(object sender, SqlNotificationEventArgs e)
                {
                    if (e.Info == SqlNotificationInfo.Invalid)
                    {
                        // sqlDbHelper.ExecuteNonQuery("ALTER DATABASE AdventureWorksDW SET ENABLE_BROKER");
                        // 写文件，数据库未开启Service Broker或者提供了无法通知的语句，例如没有写包括数据库所有者的表名。
                    }
                    this.ClearCache(tableName);
                });
            dt = sqlDbHelper.GetDataTable(selectSql);
            pageCache[selectSql] = dt;
        }
        return dt;

    }


}
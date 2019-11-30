using System;

using System.Data;
using System.Web;
using System.Web.Caching;


public class DimCustomer : DaBase
{
    public DimCustomer(Cache pageCache) : base(pageCache) { }


    #region 分页查询顾客信息
    public DataTable SelectDimCustomer(int startIndex, int maxIndex)
    {
        // 用于查询的sql语句
        string strSql = string.Format("with t as ( "
            + "    select row_number() over(order by CustomerKey Desc) as rowNum, * "
            + "    from DimCustomer where '1' = '1' "
            + ") select * from t where rowNum between {0} and {1} ", startIndex, maxIndex);

        return base.GetDataTable(strSql, "dbo", "DimCustomer", "CustomerKey");
    }

    #endregion

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleApplication1.SyntaxEngineClass;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main()
        {
            string CONNECTING_STRING = @"Data Source=kongy-win7; Initial Catalog='Test';User='sa';Pwd='95938';";

            //string strIn = @"SELECT2 a.ParamName,a.ParamNameChn,b.GroupName as ParamGroup,b.ParamOrder,a.ShowType,a.ParamWidth,a.Options,a.Scope,a.ColSpan  FROM myBizParamRegist a LEFT JOIN myBizParamGroup b ON a.ParamName=b.ParamName  WHERE a.ParamType = '参数配置' AND b.Application ='销售业务' AND a.Scope = '项目' AND (a.ParamName NOT in ('cb_EnableHtSP','cb_EnableBgSP','cb_EnableFkSP')) AND (a.ParamName NOT in ('IsUseSale','h_UseMode2Sale') )" + Environment.NewLine + "GO";

            //string sql = @"CREATE TABLE Test (a int, b nvarchar(10))"+Environment.NewLine +"GO ";

            //try
            //{
            //    string result = SqlQueryProvider.CheckSyntaxBySql(CONNECTING_STRING, strIn);
            //    if (result.Length != 0)
            //    {
            //        Console.WriteLine(result);
            //    }
            //}
            //catch (Exception  ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            string [] filePaths = new string[2];
            
            //filePaths[0] = @"D:\MyWork\工作\2009\空库数据文件\20090923清理空库中垃圾数据.sql";
            //filePaths[1] = @"D:\MyWork\工作\2009\万科授权体系修改\SQLQuery1.sql";
            //Console.WriteLine(SqlQueryProvider.CheckSyntaxByFile(CONNECTING_STRING, filePaths));
            Console.WriteLine(SqlQueryProvider.ExcuteSql(CONNECTING_STRING, @"C:\Users\kongy\Desktop\Test.sql"));
            
            
            Console.ReadLine();
        }
    }
}

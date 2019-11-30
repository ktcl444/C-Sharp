using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Caching;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;


namespace SqlDependencyTest
{
    public partial class _Default3 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //DataTable dt = new DataTable();
            //DataRow[] drs = dt.Select("BUName = ''");
           //uint i = GetMemory();
            //int result = DBHelper.ExcuteSql("Update dbo.t_BusinessUnit set BuName = '11' where BUGUID = newid();", string.Empty);
          //int result = DBHelper.ExcuteSql("Delete dbo.t_BusinessUnit where BUGUID = newid();",string.Empty );


            //DataTable dt2 = DBHelper.GetDataTable(" select * FROM t_BusinessUnit", string.Empty);
        //DataTable dt = DBHelper.GetDataTable("SELECT BUGUID,BUName,BUCode,Level,ParentBUGUID FROM dbo.t_BusinessUnit", string.Empty);
        }

        private void GetDataTableSize(DataTable dt)
        { 
            long lngDataSize = 0;
            DataRow dr ;
            object obj;
            for (int i = 0; i < dt.Rows.Count ; i++)
			{
			    dr = dt.Rows[i];
                for (int j = 0; j < dr.ItemArray.Length ; i++)
			    {
			        obj = dr.ItemArray[j];
                    lngDataSize += obj.ToString().Length;
			    }
			}
        }
   //          Dim lngDataSize As Long = 0
   //' loop through the tables in the dataset
   //For Each dt As DataTable In ds.Tables
   //  ' loop through the rows of each table
   //  For Each dr As DataRow In dt.Rows
   //    ' loop through each row's columns
   //    For i As Integer = 0 To dt.Columns.Count - 1
   //      ' get this column value
   //      Dim obj As Object = dr.Item(i)

   //      ' if the column is an array, we need to get
   //      ' its length and multiply it by the size of
   //      ' its element type, which can be done by using
   //      ' the first element in the array
   //      If (TypeOf obj Is Array) Then
   //        Dim arr As Array = DirectCast(obj, Array)
   //        lngDataSize += (arr.Length * Len(arr(0)))
   //      Else
   //        ' otherwise we can just check the length
   //        ' of this value, since sql server should
   //        ' only be returning arrays or intrinsic
   //        ' datatypes len() will return the size
   //        lngDataSize += Len(obj)
   //      End If
   //    Next
   //  Next
   //Next

        protected void Button1_Click(object sender, EventArgs e)
        {
            string sql = TextBox1.Text.Trim();
            DataTable dt = DBHelper.GetDataTable(sql, "SQLDependencyTestConnectionString");
            try

            {
                HttpContext.Current.Cache.Insert(sql, dt);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            //select top 100 * from p_project
            //byte bt = Convert.ToByte(dt);

            //using (System.IO.MemoryStream memory = new System.IO.MemoryStream())
            //{
            //    BinaryFormatter b = new BinaryFormatter();
            //    b.Serialize(memory, dt); //系列化datatable,MS已经对datatable实现了系列化接口,如果你自定义的类要系列化,实现IFormatter 就可以类似做法 

            //    byte[] buff = memory.GetBuffer(); //这里就可你想要的byte[],可以使用它来传输 

            //}
        }
            public  struct MEMORYSTATUS1 //这个结构用于获得系统信息
        {
            internal uint dwLength;
            internal uint dwMemoryLoad;
            internal uint dwTotalPhys;
            internal uint dwAvailPhys;
            internal uint dwTotalPageFile;
            internal uint dwAvailPageFile;
            internal uint dwTotalVirtual;
            internal uint dwAvailVirtual;
        }
        [DllImport("kernel32.dll ")]//调用系统DLL
        public static extern void GlobalMemoryStatus(ref   MEMORYSTATUS1 lpBuffer); //获得系统DLL里的函数
        private uint  GetMemory()
        { 
        

            MEMORYSTATUS1 vBuffer = new MEMORYSTATUS1();//实例化结构
            GlobalMemoryStatus(ref   vBuffer);//给此结构赋值
            string useinfo=Convert.ToString(vBuffer.dwAvailPhys/1024/1024 + "MB");//获得已用内存量
            string allinfo = Convert.ToString(vBuffer.dwTotalPhys / 1024 / 1024 + "MB");//获得内存总量
            this.TextBox2.Text = useinfo;
            this.TextBox3.Text = allinfo;
            return vBuffer.dwAvailPhys;

        }
    }
}

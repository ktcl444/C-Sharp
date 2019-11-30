using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;

namespace localhost
{
	/// <summary>
	/// WebForm1 的摘要说明。
	/// </summary>
	public class WebForm1 : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.TextBox TextBox1;
		protected System.Web.UI.WebControls.Button Button1;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			// 在此处放置用户代码以初始化页面
			this.TextBox1.Text  ="";
		}

		#region Web 窗体设计器生成的代码
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{    
			this.Button1.Click += new System.EventHandler(this.Button1_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void Button1_Click(object sender, System.EventArgs e)
		{
			SqlConnection conn = DBHelper.GetSqlConnection();
			SqlCommand cmd = new SqlCommand("xTestPtoc",conn);
			cmd.CommandType = CommandType.StoredProcedure;
			SqlParameterCollection SqlParams = cmd.Parameters;
			SqlParams.Add(new  SqlParameter("@OutParam", SqlDbType.VarChar,8000));
			SqlParams["@OutParam"].Direction = ParameterDirection.Output;

			//conn.Open();
			if(conn.State ==ConnectionState.Closed)
			{
				conn.Open();
			}
			cmd.ExecuteNonQuery();
			if (SqlParams["@OutParam"] != null && SqlParams["@OutParam"].Value.ToString () != string.Empty )
			{
				int resultlength = SqlParams["@OutParam"].Value.ToString().Length ;
				this.TextBox1.Text = resultlength.ToString();
			}

		}
	}
}

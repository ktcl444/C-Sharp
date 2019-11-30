using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btn1_Click(object sender, EventArgs e)
    {
        DimCustomer da = new DimCustomer(this.Cache);
        this.gv1.DataSource = da.SelectDimCustomer(Convert.ToInt32(this.txt1.Text), Convert.ToInt32(this.txt2.Text));
        this.gv1.DataBind();

        //SqlDbHelper sqlDbHelper = SqlDbHelper.Instance();
        //DataTable dt = sqlDbHelper.GetDataTable("select top 10 * from DimCustomer");
        //this.gv1.DataSource = dt;
        //this.gv1.DataBind();
    }

}
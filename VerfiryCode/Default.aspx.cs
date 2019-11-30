using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;


public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Hashtable hs;
        HttpContext.Current.Cache.Insert("Test", hs);
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        if (Chkcode.Text.ToUpper()  == Session["Code"].ToString().ToUpper() )
        {
            Session["Admin"] = "Admin";
            Response.Write("<script>alert('验证成功！')</script>");
        }
        else
        {
            Response.Write("<script>alert('验证失败！')</script>");
        }
        //BCDFGHJKMPQRTVWXY2346789
        //'2', '3', '4', '5', '6', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'W', 'X', 'Y' 
    }


}

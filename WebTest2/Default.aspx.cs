using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        //Response.Write(RSAUtil1.Test());
        CopyFile();
    }
    private void CopyFile()
    {
        string s = string.Empty;
        string s2 = "1";
        RSAUtil1.GetHash(s2, ref s);
        string sourceFileName = @"D:\MyWork\代码\C#\Test\WebTest2\bin\ClassLibrary1.dll";
        string targetFileName = @"D:\MyWork\代码\C#\Test\WebTest1\bin\ClassLibrary1.dll";
        File.Copy(sourceFileName, targetFileName, true);
        //File.Delete(targetFileName);
        //File.Copy(sourceFileName,targetFileName.true);
    }
}

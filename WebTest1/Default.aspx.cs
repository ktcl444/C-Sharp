using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Xml;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //CheckCookie();
        //try
        //{
        //    TestCache();
        //}
        //catch (Exception ex)
        //{
        //    this.TextBox1.Text = ex.StackTrace;
        //}
        //string s = GetDomain("http://www.163.com");
        //this.TextBox1.Text = s;
        //s = GetDomain("http://mail.163.com");
        //this.TextBox1.Text += "\r\n" + s;
        WriteAppSettings("PrivateKey", "2222");
    }

    public static object WriteAppSettings(string key, string value)
    {
        string strFileName = System.Web.HttpContext.Current.Server.MapPath("\\") + "web.config";
        XmlDocument doc = new XmlDocument();
        doc.Load(strFileName);
        XmlElement appSection = doc.DocumentElement.SelectSingleNode("/configuration/appSettings");
        XmlElement item = appSection.SelectSingleNode("add[@key=\"" + key + "\"]");
        if (item == null)
        {
            XmlElement e = doc.CreateElement("add");
            e.SetAttribute("key", key);
            e.SetAttribute("value", value);
            appSection.AppendChild(e);
        }
        else
        {
            item.SetAttribute("value", value);
        }
        doc.Save(strFileName);
        
        return null;
    }

    private void TestStringLink()
    {
        this.TextBox1.Text = "222" + string.Empty + "33";
        this.TextBox1.Text = "Page".IndexOf("Page") > -1 ? "true" : "false";
    }

    private string GetDomain(string strHtmlPagePath)
    {
        string p = @"http://[^\.]*\.(?<domain>[^/]*)";
        Regex reg = new Regex(p, RegexOptions.IgnoreCase);
        Match m = reg.Match(strHtmlPagePath);
        return m.Groups["domain"].Value;
    }

    private string TestException()
    {
        try
        {
            throw new Exception("1");
        }
        catch
        {
            return string.Empty;
        }
        finally
        {
            string s = "2";
        }
        return "1";
    }

    private void TestCache()
    {
        Hashtable ht = new Hashtable();
        ht.Add("1", "1");
        Cache.Insert("Test", ht);

        object o = Cache.Get("Test");
        Hashtable ht2 = (Hashtable)o;
        ht2.Add("2","2");

        Hashtable ht3 = (Hashtable)Cache.Get("Test");
    }

    private void TestSplit()
    {
        string s1 = "1|2";
        string[] s = s1.Split('|');
        this.TextBox1.Text = s[0] + " | " + s[1];
        s1 = "1|";
        s = s1.Split('|');
        this.TextBox1.Text += "\r\n" + s[0] + " | " + s[1];
    }

    private void TestWriteHT()
    {
        System.Collections.Hashtable ht = new System.Collections.Hashtable();
        ht.Add("1", "1");
        ht.Add("2", "2");
        ht.Add("3", "3");
        ht.Add("4", "4");

        string returnValue = string.Empty;


        System.Collections.IDictionaryEnumerator myEnumerator = ht.GetEnumerator();
        while (myEnumerator.MoveNext())
        {
            returnValue += myEnumerator.Key + ":" + myEnumerator.Value + ",";
        }
        if (returnValue.Length  != 0)
        {
            returnValue = returnValue.Substring(0, returnValue.Length - 1);
        }
        this.TextBox1.Text = returnValue;
    }

    private void CopyFile()
    {
        //string sourceFileName = @"D:\MyWork\代码\C#\Test\WebTest1\Default.aspx";
        //string targetFileName = @"D:\MyWork\代码\C#\Test\WebTest2\Default.aspx";
        //File.Copy(sourceFileName,targetFileName.true);
    }

    private void Test1()
    {
        Test2();
    }
    private void Test2()
    {
        Test3();
    }
    private void Test3()
    {
        Test4();
    }
    private void Test4()
    {
        Test5();
    }
    private void Test5()
    {
        throw new Exception("Custom Exception");
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        Response.Write(RSAUtil1.Test());
        Response.Write(RSAUtil1.Test());
        //string s = string.Empty ;
        //string s2="1";
        //RSAUtil1.GetHash(s2, ref s);
        //string sourceFileName = @"D:\MyWork\代码\C#\Test\WebTest1\bin\ClassLibrary1.dll";
        //string targetFileName = @"D:\MyWork\代码\C#\Test\WebTest2\bin\ClassLibrary1.dll";
        //File.Copy(sourceFileName, targetFileName, true);
        //File.Delete(targetFileName);
        //HttpCookie cookie = new HttpCookie("Test");
        //cookie["Name"] = "1";
        //Response.Cookies.Add(cookie);

        //if (Request.Cookies["Test"] == null)
        //{
        //    Response.Write("Can't find cookie");
        //}
        //else
        //{
        //    Response.Write("Find cookie");
        //}
    }

    protected void Button2_Click(object sender, EventArgs e)
    {

        //if (Request.Cookies["Test"] == null)
        //{
        //    Response.Write("Can't find cookie");
        //}
        //else
        //{
        //    Response.Write("Find cookie");
        //}
    }

    private void CheckCookie()
    { 
        HttpCookie cookie = HttpContext.Current.Request.Cookies["Login_User"];
        if (cookie != null)
        {
            DateTime signTime = Convert.ToDateTime(cookie["SignTime"].ToString());
            Response.Write("signTime = " + signTime.ToString() + "  ");
            string userCode = cookie["UserCode"].ToString();
            Response.Write("userCode = " + userCode + "  ");
            string userCodeSign = cookie["UserSign"].ToString();
            Response.Write("userCodeSign = " + userCodeSign + "  ");
            if (signTime.AddDays(1) > DateTime.Now && RSAUtil1.SignatureDeformatter(userCode + signTime.ToString(), userCodeSign))
                Response.Write("Check OK");
            else
                Response.Write("Check Failed");
            return;
        }
        else {
            Response.Write("Can't find Cookie");
            return;
        }   

    }
}

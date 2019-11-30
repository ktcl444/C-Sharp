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
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

public partial class Validate : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string type = string.Empty;
        if (Request.QueryString["type"] == null)
        {
            type = "1";
        }
        else
        {
            type = Request.QueryString["type"].ToString();
        }
        if (type == "1")
        {
            BuildValidateCode1();
        }
        else if (type == "2")
        {
            BuildValidateCode2();
        }
        else
        {
            BuildValidateCode3();
        }
    }

    private void BuildValidateCode3()
    {
        String sRndStr = checkcode.rndStr(4);
        Session["code"] = sRndStr;
       // CreateCheckCodeImage(sRndStr);
        CreateCheckCodeImage(GenerateCheckCode());
    }

    private string GenerateCheckCode()
    {
        int number;
        char code;
        string checkCode = String.Empty;
        System.Random random = new Random();

        for (int i = 0; i < 4; i++)
        {
            number = random.Next();

            if (number % 2 == 0)
                code = (char)('0' + (char)(number % 10));
            else
                code = (char)('A' + (char)(number % 26));

            checkCode += code.ToString();
        }
        //Response.Cookies.Add(new HttpCookie("CheckCode", checkCode));
        Session["code"] = checkCode;
        return checkCode;
    }

    private void CreateCheckCodeImage(string checkCode)
    {
        if (checkCode == null || checkCode.Trim() == String.Empty)
            return;
        System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 12.5)), 22);
        Graphics g = Graphics.FromImage(image);
        try
        {
            //生成随机生成器
            Random random = new Random();
            //清空图片背景色
            g.Clear(Color.White);
            //画图片的背景噪音线
            for (int i = 0; i < 2; i++)
            {
                int x1 = random.Next(image.Width);
                int x2 = random.Next(image.Width);
                int y1 = random.Next(image.Height);
                int y2 = random.Next(image.Height);
                g.DrawLine(new Pen(Color.Black), x1, y1, x2, y2);
            }
            Font font = new System.Drawing.Font("Arial", 12, (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic));
            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Gray, Color.Gray, 1.2f, true);
            g.DrawString(checkCode, font, brush, 2, 2);
            //画图片的前景噪音点
            for (int i = 0; i < 100; i++)
            {
                int x = random.Next(image.Width);
                int y = random.Next(image.Height);
               // image.SetPixel(x, y, Color.FromArgb(random.Next()));
                image.SetPixel(x, y, Color.Gray  );
            }
            //画图片的边框线
            g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            Response.ClearContent();
            Response.ContentType = "image/Gif";
            Response.BinaryWrite(ms.ToArray());
        }
        finally
        {
            g.Dispose();
            image.Dispose();
        }
    }

    private void BuildValidateCode2()
    {
        String sRndStr = checkcode.rndStr(4);
        Session["code"] = sRndStr;
        checkcode.general(sRndStr);
    }

    private void BuildValidateCode1()
    {
        Session["code"] = RandNum(4);
        ValidateCode(Session["code"].ToString(), 40, 20, "黑体", 10, "#FFFFFF");  
    }

    /// <summary>
    /// 该方法用于生成指定位数的随机数
    /// </summary>
    /// <param name="VcodeNum">参数是随机数的位数</param>
    /// <returns>返回一个随机数字符串</returns>
    private string RandNum(int VcodeNum)
    {
        string Vchar = "0,1,2,3,4,5,6,7,8,9";
        string[] VcArray = Vchar.Split(',');//拆分成数组
        string VNum = "";
        int temp = -1;//记录上次随机数值，尽量避避免生产几个一样的随机数

        Random rand = new Random();
        //采用一个简单的算法以保证生成随机数的不同
        for (int i = 0; i < VcodeNum; i++)
        {
            if (temp != -1)
            {
                rand = new Random(i * temp * unchecked((int)DateTime.Now.Ticks));
            }

            int t = rand.Next(VcArray.Length - 1);
            if (temp != -1 && temp == t)
            {
                return RandNum(VcodeNum);

            }
            temp = t;
            VNum += VcArray[t];
        }
        return VNum;
    }

    /// <summary>
    /// 生成图片并写入字符
    /// </summary>
    /// <param name="VNum">目标字符</param>
    /// <param name="w">宽</param>
    /// <param name="h">高</param>
    /// <param name="font">字体文件</param>
    /// <param name="fontSize">字体大小</param>
    /// <param name="bgColor">图片背景颜色</param>
    private void ValidateCode(string VNum, int w, int h, string font, int fontSize, string bgColor)
    {
        Bitmap Img = new Bitmap(w, h);//生成图像的实例
        Graphics g = Graphics.FromImage(Img);//从Img对象生成新的Graphics对象
        g.Clear(ColorTranslator.FromHtml(bgColor));//背景颜色
        Font f = new Font(font, fontSize);//生成Font类的实例
        SolidBrush s = new SolidBrush(Color.Black);//生成笔刷类的实例
        g.DrawString(VNum, f, s, 3, 3);//将VNum写入图片中
        Img.Save(Response.OutputStream, ImageFormat.Jpeg);//将此图像以Jpeg图像文件的格式保存到流中
        Response.ContentType = "image/Jpeg";
        //回收资源
        g.Dispose();
        Img.Dispose();
        Response.End();
    }


    /*-----------------------------------------------------------------------------------*\
  * shawl.qiu c# .net checkcode class v1.0
  \*-----------------------------------------------------------------------------------*/
    //---------------------------------------------------------------------begin class checkcode
    public class checkcode
    {
        //-----------------------------------begin event
        public checkcode()
        {
        }
        ~checkcode()
        {
        }
        //-----------------------------------end event
        //-----------------------------------begin public constant
        //-----------------------begin about
        public const String auSubject = "shawl.qiu c# .net checkcode class";
        public const String auVersion = "v1.0";
        public const String au = "shawl.qiu";
        public const String auEmail = "shawl.qiu@gmail.com";
        public const String auBlog = "http://blog.csdn.net/btbtd";
        public const String auCreateDate = "2007-2-1";
        //-----------------------end about
        //-----------------------------------end public constant
        //-----------------------------------begin public static method
        public static void general(String sCc)
        {
            Int32 ccLen = sCc.Length;
            String ccFtFm = "Arial";
            Int32 ccFtSz = 12;
            Int32 ccWidth = ccLen * ccFtSz + 1;
            Int32 ccHeight = ccFtSz + 5;
            using (Bitmap oImg = new Bitmap(ccWidth, ccHeight))
            {
                using (Graphics oGpc = Graphics.FromImage(oImg))
                {
                    HatchBrush hBrush = new HatchBrush(HatchStyle.DashedVertical,
                    Color.Yellow, Color.Silver);
                    oGpc.FillRectangle(hBrush, 0, 0, ccWidth, ccWidth);
                    oGpc.DrawString(sCc, new System.Drawing.Font(ccFtFm, ccFtSz, FontStyle.Bold),
                    new System.Drawing.SolidBrush(Color.Black), 0, 0);
                    //-----------------------边框
                    Pen blackPen = new Pen(Color.Black, 1);
                    oGpc.DrawLine(blackPen, 0, ccHeight, 0, 0); // 左竖线
                    oGpc.DrawLine(blackPen, 0, 0, ccWidth, 0); // 顶横线
                    oGpc.DrawLine(blackPen, ccWidth - 1, 0, ccWidth - 1, 20); // 右竖线
                    oGpc.DrawLine(blackPen, 0, ccHeight - 1, ccWidth, ccHeight - 1); // 底横线
                    writeImg(oImg);
                }
            }
        } // end public static void general
        public static String rndStr(Int32 len)
        {
            String sTemp = "";
           // String sForRnd = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            String sForRnd = "2,3,4,5,6,8,9,A,B,C,D,E,F,G,H,J,K,L,M,N,P,R,S,T,W,X,Y";
            String[] aRnd = sForRnd.Split(',');
            Random oRnd = new Random();
            Int32 iArLen = aRnd.Length;
            for (Int32 i = 0; i < len; i++)
            {
                sTemp += aRnd[oRnd.Next(0, iArLen)];
            }
            return sTemp;
        } // end public static String rndStr
        //-----------------------------------end public static method
        //-----------------------------------begin private static method
        private static void writeImg(Bitmap oImg)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                oImg.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.ContentType = "image/Png";
                HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            }
        } // end private static void writeImg
    }
    //---------------------------------------------------------------------end class checkcode

    private void BuildValidateCode4()
    {
        Validate v = new Validate();
        v.Length = this.length;
        v.FontSize = this.fontSize;
        v.Chaos = this.chaos;
        v.BackgroundColor = this.backgroundColor;
        v.CodeSerial = this.codeSerial;
        v.Colors = this.colors;
        v.Fonts = this.fonts;
        v.Padding = this.padding;
        string code = v.CreateVerifyCode();                //取随机码
        v.CreateImageOnPage(code, this.Context);        // 输出图片
        Session["code"] = code;
        //Response.Cookies.Add(new HttpCookie("CheckCode", code.ToUpper()));// 使用Cookies取验证码的值
    }

    #region 验证码长度(默认6个验证码的长度)
    int length = 4;
    public int Length
    {
        get { return length; }
        set { length = value; }
    }
    #endregion
    #region 验证码字体大小(为了显示扭曲效果，默认40像素，可以自行修改)
    int fontSize = 12;
    public int FontSize
    {
        get { return fontSize; }
        set { fontSize = value; }
    }
    #endregion
    #region 边框补(默认1像素)
    int padding = 2;
    public int Padding
    {
        get { return padding; }
        set { padding = value; }
    }
    #endregion
    #region 是否输出燥点(默认不输出)
    bool chaos = true;
    public bool Chaos
    {
        get { return chaos; }
        set { chaos = value; }
    }
    #endregion

    #region 自定义背景色(默认白色)
    Color backgroundColor = Color.White;
    public Color BackgroundColor
    {
        get { return backgroundColor; }
        set { backgroundColor = value; }
    }
    #endregion
    #region 自定义随机颜色数组
    Color[] colors = { Color.Black, Color.Red, Color.DarkBlue, Color.Green, Color.Orange, Color.Brown, Color.DarkCyan, Color.Purple };
    public Color[] Colors
    {
        get { return colors; }
        set { colors = value; }
    }
    #endregion
    #region 自定义字体数组
    string[] fonts = { "Arial", "Georgia" };
    public string[] Fonts
    {
        get { return fonts; }
        set { fonts = value; }
    }
    #endregion
    #region 自定义随机码字符串序列(使用逗号分隔)
    string codeSerial = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
    public string CodeSerial
    {
        get { return codeSerial; }
        set { codeSerial = value; }
    }
    #endregion

    #region 生成校验码图片
    public Bitmap CreateImageCode(string code)
    {
        int fSize = FontSize;
        int fWidth = fSize + Padding;
        int imageWidth = (int)(code.Length * fWidth) + 4 + Padding * 2;
        int imageHeight = fSize * 2 + Padding;
        System.Drawing.Bitmap image = new System.Drawing.Bitmap(imageWidth, imageHeight);
        Graphics g = Graphics.FromImage(image);
        g.Clear(BackgroundColor);
        Random rand = new Random();
        //给背景添加随机生成的燥点
        if (this.Chaos)
        {
            //Pen pen = new Pen(ChaosColor, 0);
            int c = Length * 10;
            for (int i = 0; i < c; i++)
            {
                int x = rand.Next(image.Width);
                int y = rand.Next(image.Height);
                //g.DrawRectangle(pen, x, y, 1, 1);
            }
        }
        int left = 0, top = 0, top1 = 1, top2 = 1;
        int n1 = (imageHeight - FontSize - Padding * 2);
        int n2 = n1 / 4;
        top1 = n2;
        top2 = n2 * 2;
        Font f;
        Brush b;
        int cindex, findex;
        //随机字体和颜色的验证码字符
        for (int i = 0; i < code.Length; i++)
        {
            cindex = rand.Next(Colors.Length - 1);
            findex = rand.Next(Fonts.Length - 1);
            f = new System.Drawing.Font(Fonts[findex], fSize, System.Drawing.FontStyle.Bold);
            b = new System.Drawing.SolidBrush(Colors[cindex]);
            if (i % 2 == 1)
            {
                top = top2;
            }
            else
            {
                top = top1;
            }
            left = i * fWidth;
            g.DrawString(code.Substring(i, 1), f, b, left, top);
        }
        //画一个边框 边框颜色为Color.Gainsboro
        g.DrawRectangle(new Pen(Color.Gainsboro, 0), 0, 0, image.Width - 1, image.Height - 1);
        g.Dispose();
        ////产生波形（Add By 51aspx.com）
        //image = TwistImage(image, true, 8, 4);
        return image;
    }
    #endregion
    #region 将创建好的图片输出到页面
    public void CreateImageOnPage(string code, HttpContext context)
    {
        System.IO.MemoryStream ms = new System.IO.MemoryStream();
        Bitmap image = this.CreateImageCode(code);
        image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
        context.Response.ClearContent();
        context.Response.ContentType = "image/Jpeg";
        context.Response.BinaryWrite(ms.GetBuffer());
        ms.Close();
        ms = null;
        image.Dispose();
        image = null;
    }
    #endregion
    #region 生成随机字符码
    public string CreateVerifyCode(int codeLen)
    {
        if (codeLen == 0)
        {
            codeLen = Length;
        }
        string[] arr = CodeSerial.Split(',');
        string code = "";
        int randValue = -1;
        Random rand = new Random(unchecked((int)DateTime.Now.Ticks));
        for (int i = 0; i < codeLen; i++)
        {
            randValue = rand.Next(0, arr.Length - 1);
            code += arr[randValue];
        }
        return code;
    }
    public string CreateVerifyCode()
    {
        return CreateVerifyCode(0);
    }
    #endregion
}



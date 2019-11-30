<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Map.Web._Default" %>

<%@ Register Assembly="Mysoft.Map.WebControls" Namespace="Mysoft.Map.WebControls"    TagPrefix="cnt" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>测试</title>
<style>
.div1
{
    font-size:large;
    color:Red;
    font-weight:bolder;
    padding:5,0,15,0;
    height:50px;
}


</style>
    <script src="Scripts/jquery.js" type="text/javascript"></script>
    <script src="Scripts/common.js" type="text/javascript"></script>
</head>
<body >
    <form id="frm" runat="server">
    <span class="div1"> TextBox控件测试：</span>
    <br />
    <div runat="server" id="divTextBox"></div>
    <input type="button" value="显示" id="Button7" onclick="window.AppTextBox1.setDisplay(true)"/>
    <input type="button" value="隐藏" id="Button8" onclick="window.AppTextBox1.setDisplay(false)"/>
    <input type="button" value="只读" id="Button9" onclick="window.AppTextBox1.setReadOnly(true)" />
    <input type="button" value="非只读" id="Button10" onclick="window.AppTextBox1.setReadOnly(false)" />
    <input type="button" value="所有控件禁用" id="Button11" onclick="window.AppTextBox1.setDisabled(true)" />    
    <input type="button" value="所有控件非禁用" id="Button12" onclick="window.AppTextBox1.setDisabled(false)" />      
    <input type="button" value="按钮禁用" id="Button42" onclick="window.AppTextBox1.setIconDisabled(true)" />    
    <input type="button" value="按钮非禁用" id="Button43" onclick="window.AppTextBox1.setIconDisabled(false)" />     
    <input type="button" value="文本禁用" id="Button44" onclick="window.AppTextBox1.setTextDisabled(true)" />    
    <input type="button" value="文本非禁用" id="Button45" onclick="window.AppTextBox1.setTextDisabled(false)" />       
    <input type="button" value="读上一次值" id="Button41" onclick="alert(window.AppTextBox1.getPreviousValue())" />
    <input type="button" value="读值" id="Button13" onclick="alert(window.AppTextBox1.getValue())" />
    <input type="button" value="设值" id="Button14" onclick="window.AppTextBox1.setValue(window.prompt('请输入要设置的值',''))" />
     <input type="button" value="文本是否禁用" id="Button49" onclick="alert(window.AppTextBox1.getTextDisabled())" />
     <input type="button" value="按钮是否禁用" id="Button51" onclick="alert(window.AppTextBox1.getIconDisabled())" />
    <input type="button" value="复位" onclick="window.AppTextBox1.reset()" />
    <p/> 
    <span class="div1"> TextArea 控件测试：</span>
    <br />
    <div runat="server" id="divTextArea"></div>
    <input type="button" value="显示" id="Button17" onclick="window.AppTextArea1.setDisplay(true)"/>
    <input type="button" value="隐藏" id="Button18" onclick="window.AppTextArea1.setDisplay(false)"/>
    <input type="button" value="只读" id="Button19" onclick="window.AppTextArea1.setReadOnly(true)" />
    <input type="button" value="非只读" id="Button20" onclick="window.AppTextArea1.setReadOnly(false)" />
    <input type="button" value="禁用" id="Button21" onclick="window.AppTextArea1.setDisabled(true)" />    
    <input type="button" value="非禁用" id="Button22" onclick="window.AppTextArea1.setDisabled(false)" />      
    <input type="button" value="读上一次值" id="Button50" onclick="alert(window.AppTextArea1.getPreviousValue())" />   
    <input type="button" value="读值" id="Button23" onclick="alert(window.AppTextArea1.getValue())" />
    <input type="button" value="设值" id="Button24" onclick="window.AppTextArea1.setValue(window.prompt('请输入要设置的值',''))" />
    <input type="button" value="是否禁用" id="Button48" onclick="alert(window.AppTextArea1.getDisabled())" />
    <input type="button" value="是否显示" id="Button59" onclick="alert(window.AppTextArea1.getDisplay())" />
    <input type="button" value="复位" onclick="window.AppTextArea1.reset()" />
    <p/>     
    <span class="div1">  TextArea 控件测试：</span> 
    <br />
    <div runat="server" id="divTextArea1"></div>
    <input type="button" value="显示" id="Button25" onclick="window.AppTextArea2.setDisplay(true)"/>
    <input type="button" value="隐藏" id="Button26" onclick="window.AppTextArea2.setDisplay(false)"/>
    <input type="button" value="只读" id="Button27" onclick="window.AppTextArea2.setReadOnly(true)" />
    <input type="button" value="非只读" id="Button28" onclick="window.AppTextArea2.setReadOnly(false)" />
    <input type="button" value="禁用" id="Button29" onclick="window.AppTextArea2.setDisabled(true)" />    
    <input type="button" value="非禁用" id="Button30" onclick="window.AppTextArea2.setDisabled(false)" />      
    <input type="button" value="读上一次值" id="Button55" onclick="alert(window.AppTextArea2.getPreviousValue())" />     
    <input type="button" value="读值" id="Button31" onclick="alert(window.AppTextArea2.getValue())" />
    <input type="button" value="设值" id="Button32" onclick="window.AppTextArea2.setValue(window.prompt('请输入要设置的值',''))" />
    <input type="button" value="是否禁用" id="Button56" onclick="alert(window.AppTextArea2.getDisabled())" />
    <input type="button" value="是否显示" id="Button58" onclick="alert(window.AppTextArea2.getDisplay())" />
    <input type="button" value="复位" onclick="window.AppTextArea2.reset()" />
    <p/>          
       
    <span class="div1">  Password 控件测试：</span> 
    <br />

    <div runat="server" id="divPassword" ></div>
    <input type="button" value="显示" id="btnDisplay" onclick="window.AppPassword1.setDisplay(true)"/>
    <input type="button" value="隐藏" id="Button3" onclick="window.AppPassword1.setDisplay(false)"/>
    <input type="button" value="只读" id="btnDisable" onclick="window.AppPassword1.setReadOnly(true)" />
    <input type="button" value="非只读" id="Button4" onclick="window.AppPassword1.setReadOnly(false)" />
    <input type="button" value="禁用" id="Button5" onclick="window.AppPassword1.setDisabled(true)" />    
    <input type="button" value="非禁用" id="Button6" onclick="window.AppPassword1.setDisabled(false)" />      
    <input type="button" value="读上一次值" id="Button60" onclick="alert(window.AppPassword1.getPreviousValue())" />   
    <input type="button" value="读值" id="Button1" onclick="alert(window.AppPassword1.getValue())" />
    <input type="button" value="设值" id="Button2" onclick="window.AppPassword1.setValue(window.prompt('请输入要设置的值',''))" />
    <input type="button" value="是否禁用" id="Button52" onclick="alert(window.AppPassword1.getDisabled())" />
    <input type="button" value="复位" onclick="window.AppPassword1.reset()" />
    <br />
    <p/>
    <span class="div1">  Hyperlink 控件测试：</span> 
    <br />
    <div runat="server" id="divHyperlink"></div>
    <input type="button" value="读文本值" onclick="alert(window.AppHyperlink1.getValue())" />
    <input type="button" value="设文本值" onclick="window.AppHyperlink1.setValue(window.prompt('请输入要设置的值',''))" />
    <input type="button" value="禁用" id="Button15" onclick="window.AppHyperlink1.setDisabled(true)" />    
    <input type="button" value="非禁用" id="Button16" onclick="window.AppHyperlink1.setDisabled(false)" />      
    <input type="button" value="读上一次值" id="Button65"   onclick="alert(window.AppHyperlink1.getPreviousValue())" />    
    <input type="button" value="显示" onclick="window.AppHyperlink1.setDisplay(true)" />
    <input type="button" value="隐藏" onclick="window.AppHyperlink1.setDisplay(false)" />
    <input type="button" value="是否禁用" id="Button53" onclick="alert(window.AppHyperlink1.getDisabled())" />
    <input type="button" value="是否显示" id="Button54" onclick="alert(window.AppHyperlink1.getDisplay())" />
    <input type="button" value="设URL" id="Button47" onclick="window.AppHyperlink1.setHref(window.prompt('请输入要设置的URL',''))" />
    <input type="button" value="取URL" onclick="alert(window.AppHyperlink1.getHref())" />
    <input type="button" value="复位" onclick="window.AppHyperlink1.reset()" />
    <br />
    <p/>
   <span class="div1"> Label 控件测试：</span> 
    <br />
    <div runat="server" id="divLabel"></div>
    <input type="button" value="显示" onclick="window.AppHtmlLabel1.setDisplay(true)" />
    <input type="button" value="隐藏" onclick="window.AppHtmlLabel1.setDisplay(false)" />
    <input type="button" value="读文本值" onclick="alert(window.AppHtmlLabel1.getValue())" />
    <input type="button" value="设文本值" onclick="window.AppHtmlLabel1.setValue(window.prompt('请输入要设置的值',''))" />
    <input type="button" value="读上一次值" id="Button71"  onclick="alert(window.AppHtmlLabel1.getPreviousValue())" />  
    <input type="button" value="是否显示" id="Button57" onclick="alert(window.AppHtmlLabel1.getDisplay())" />
    <input type="button" value="复位" onclick="window.AppHtmlLabel1.reset()" />
    <br />
    <p/>
    <span class="div1"> Number 控件测试：</span> 
    <br />
    <div runat="server" id="divNumber"></div>
    <input type="button" value="显示" id="Button33" onclick="window.AppNumber1.setDisplay(true)"/>
    <input type="button" value="隐藏" id="Button34" onclick="window.AppNumber1.setDisplay(false)"/>
    <input type="button" value="只读" id="Button35" onclick="window.AppNumber1.setReadOnly(true)" />
    <input type="button" value="非只读" id="Button36" onclick="window.AppNumber1.setReadOnly(false)" />
    <input type="button" value="所有控件禁用" id="Button37" onclick="window.AppNumber1.setDisabled(true)" />    
    <input type="button" value="所有控件非禁用" id="Button38" onclick="window.AppNumber1.setDisabled(false)" />      
    <input type="button" value="按钮禁用" id="Button66" onclick="window.AppNumber1.setIconDisabled(true)" />    
    <input type="button" value="按钮非禁用" id="Button67" onclick="window.AppNumber1.setIconDisabled(false)" />     
    <input type="button" value="文本禁用" id="Button68" onclick="window.AppNumber1.setTextDisabled(true)" />    
    <input type="button" value="文本非禁用" id="Button69" onclick="window.AppNumber1.setTextDisabled(false)" />       
    <input type="button" value="读上一次值" id="Button70" onclick="alert(window.AppNumber1.getPreviousValue())" />  
    <input type="button" value="读值" id="Button39" onclick="alert(window.AppNumber1.getValue())" />
    <input type="button" value="读返回值" id="Button46" onclick="alert(window.AppNumber1.getReturnValue())" />
    <input type="button" value="设值" id="Button40" onclick="window.AppNumber1.setValue(window.prompt('请输入要设置的值',''))" /> 
    <input type="button" value="复位" onclick="window.AppNumber1.reset()" />
    <asp:Button ID="Button72" runat="server" onclick="Button72_Click" 
        Text="Button" />
    </form>
</body>
</html>

<script type="text/javascript">
//    Polygon=function(iSides)
//    {
//        this.sides = iSides;
//    }

//    Polygon.prototype.getArea = function()
//    {
//        return 0;
//    }


//    Triangle = function(iBase, iHeight)
//    {
//        Polygon.call(this, 3);
//        this.base = iBase;
//        this.height = iHeight;

//    }
//    Triangle.prototype.getArea = function()
//    {
//        return this.base * this.height * 0.5;
//    }

//    Rectangle = function(iWidth, iHeight)
//    {
//        Polygon.apply(this,4);

//        this.width = iWidth;
//        this.height = iHeight;
//    }
//    Rectangle.prototype.getArea = function()
//    {
//        return this.width * this.height;
//    }

//    window.onload = function()
//    {
//        var triangle = new Triangle(10, 5);
//        var rectangle = new Rectangle(3, 4);

//        alert("三角形的边为：" + triangle.sides + "面积为:" + triangle.getArea());
//        alert("矩形的边为：" + rectangle.sides + "面积为:" + rectangle.getArea());
//    }

</script>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppSelectTest.aspx.cs" Inherits="Map.Web.Test.AppSelectTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery.js" type="text/javascript"></script>

    <script src="../Scripts/Common.js" type="text/javascript"></script>
    <script type="text/javascript">
        function appSelectOnChange1() {
            alert("appSelectOnChange1()");
        }
        function appSelectOnChange2() {
            alert("appSelectOnChange2()");
        }
        function appSelectOnChange3() {
            alert("appSelectOnChange3()");
        }
        function appSelectOnChange4() {
            alert("appSelectOnChange4()");
        }
        function appSelectOnChange5() {
            alert("appSelectOnChange5()");
        }
        function appSelectOnChange6() {
            alert("appSelectOnChange6()");
        }
        function appSelectOnChange7() {
            alert("appSelectOnChange7()");
        }
        function appSelectOnChange8() {
            alert("appSelectOnChange8()");
        }
        function appSelectOnChange9() {
            alert("appSelectOnChange9()");
        }
        function appSelectOnChange10() {
            alert("appSelectOnChange10()");
        }
        function appSelectOnChange11() {
            alert("appSelectOnChange11()");
        }
        function appSelectOnChange12() {
            alert("appSelectOnChange12()");
        }
        function appSelectOnChange13() {
            alert("appSelectOnChange13()");
        }
        function appSelectOnChange14() {
            alert("appSelectOnChange14()");
        }
        function appSelectOnChange15() {
            alert("appSelectOnChange15()");
        }
        function appSelectOnChange16() {
            alert("appSelectOnChange16()");
        }
        function appSelectOnChange17() {
            alert("appSelectOnChange17()");
        }
        function appSelectOnChange18() {
            alert("appSelectOnChange18()");
        }
        function appSelectOnChange19() {
            alert("appSelectOnChange19()");
        }


        function setAppSelectDisplay(value) {
            section2_Select_Ctrl1.setDisplay(value);
            section2_Select_Ctrl2.setDisplay(value);
            section2_Select_Ctrl3.setDisplay(value);
            section2_Select_Ctrl4.setDisplay(value);
            section2_Select_Ctrl5.setDisplay(value);
            section2_Select_Ctrl6.setDisplay(value);
            section2_Select_Ctrl7.setDisplay(value);
            section2_Select_Ctrl8.setDisplay(value);
            section2_Select_Ctrl9.setDisplay(value);
            section2_Select_Ctrl10.setDisplay(value);
            section2_Select_Ctrl11.setDisplay(value);
            section2_Select_Ctrl12.setDisplay(value);
            section2_Select_Ctrl13.setDisplay(value);
            section2_Select_Ctrl14.setDisplay(value);
            section2_Select_Ctrl15.setDisplay(value);
            section2_Select_Ctrl16.setDisplay(value);
        }

        function setAppSelectDisabled(value) {
            section2_Select_Ctrl1.setDisabled(value);
            section2_Select_Ctrl2.setDisabled(value);
            section2_Select_Ctrl3.setDisabled(value);
            section2_Select_Ctrl4.setDisabled(value);
            section2_Select_Ctrl5.setDisabled(value);
            section2_Select_Ctrl6.setDisabled(value);
            section2_Select_Ctrl7.setDisabled(value);
            section2_Select_Ctrl8.setDisabled(value);
            section2_Select_Ctrl9.setDisabled(value);
            section2_Select_Ctrl10.setDisabled(value);
            section2_Select_Ctrl11.setDisabled(value);
            section2_Select_Ctrl12.setDisabled(value);
            section2_Select_Ctrl13.setDisabled(value);
            section2_Select_Ctrl14.setDisabled(value);
            section2_Select_Ctrl15.setDisabled(value);
            section2_Select_Ctrl16.setDisabled(value);

        }

        function setAppSelectRequired(value) {
            var showMessage = "";
            section2_Select_Ctrl1.setRequired(value);
            showMessage += "section2_Select_Ctrl1    Required：" + (section2_Select_Ctrl1.isValid() ? "True" : "False") + "\r\n";
            section2_Select_Ctrl2.setRequired(value);
            showMessage += "section2_Select_Ctrl2    Required：" + (section2_Select_Ctrl2.isValid() ? "True" : "False") + "\r\n";
            section2_Select_Ctrl3.setRequired(value);
            showMessage += "section2_Select_Ctrl3    Required：" + (section2_Select_Ctrl3.isValid() ? "True" : "False") + "\r\n";
            section2_Select_Ctrl4.setRequired(value);
            showMessage += "section2_Select_Ctrl4    Required：" + (section2_Select_Ctrl4.isValid() ? "True" : "False") + "\r\n";
            section2_Select_Ctrl5.setRequired(value);
            showMessage += "section2_Select_Ctrl5    Required：" + (section2_Select_Ctrl5.isValid() ? "True" : "False") + "\r\n";
            section2_Select_Ctrl6.setRequired(value);
            showMessage += "section2_Select_Ctrl6    Required：" + (section2_Select_Ctrl6.isValid() ? "True" : "False") + "\r\n";
            section2_Select_Ctrl7.setRequired(value);
            showMessage += "section2_Select_Ctrl7    Required：" + (section2_Select_Ctrl7.isValid() ? "True" : "False") + "\r\n";
            section2_Select_Ctrl8.setRequired(value);
            showMessage += "section2_Select_Ctrl8    Required：" + (section2_Select_Ctrl8.isValid() ? "True" : "False") + "\r\n";
            section2_Select_Ctrl9.setRequired(value);
            showMessage += "section2_Select_Ctrl9    Required：" + (section2_Select_Ctrl9.isValid() ? "True" : "False") + "\r\n";
            section2_Select_Ctrl10.setRequired(value);
            showMessage += "section2_Select_Ctrl10    Required：" + (section2_Select_Ctrl10.isValid() ? "True" : "False") + "\r\n";
            section2_Select_Ctrl11.setRequired(value);
            showMessage += "section2_Select_Ctrl11    Required：" + (section2_Select_Ctrl11.isValid() ? "True" : "False") + "\r\n";
            section2_Select_Ctrl12.setRequired(value);
            showMessage += "section2_Select_Ctrl12    Required：" + (section2_Select_Ctrl12.isValid() ? "True" : "False") + "\r\n";
            section2_Select_Ctrl13.setRequired(value);
            showMessage += "section2_Select_Ctrl13    Required：" + (section2_Select_Ctrl13.isValid() ? "True" : "False") + "\r\n";
            section2_Select_Ctrl14.setRequired(value);
            showMessage += "section2_Select_Ctrl14    Required：" + (section2_Select_Ctrl14.isValid() ? "True" : "False") + "\r\n";
            section2_Select_Ctrl15.setRequired(value);
            showMessage += "section2_Select_Ctrl15    Required：" + (section2_Select_Ctrl15.isValid() ? "True" : "False") + "\r\n";
            section2_Select_Ctrl16.setRequired(value);
            showMessage += "section2_Select_Ctrl16    Required：" + (section2_Select_Ctrl16.isValid() ? "True" : "False") + "\r\n";
            alert(showMessage);
        }

        function setAppSelectValue() {
            var value = $("#AppSelectSelectedValue").val();
            section2_Select_Ctrl1.setValue(value);
            section2_Select_Ctrl2.setValue(value);
            section2_Select_Ctrl3.setValue(value);
            section2_Select_Ctrl4.setValue(value);
            section2_Select_Ctrl5.setValue(value);
            section2_Select_Ctrl6.setValue(value);
            section2_Select_Ctrl7.setValue(value);
            section2_Select_Ctrl8.setValue(value);
            section2_Select_Ctrl9.setValue(value);
            section2_Select_Ctrl10.setValue(value);
            section2_Select_Ctrl11.setValue(value);
            section2_Select_Ctrl12.setValue(value);
            section2_Select_Ctrl13.setValue(value);
            section2_Select_Ctrl14.setValue(value);
            section2_Select_Ctrl15.setValue(value);
            section2_Select_Ctrl16.setValue(value);
        }
    </script>

    <style type="text/css">
         .Title
        {
        	font-size:xx-large;
        	font-weight:bolder;
        	color:Red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
     <div>
        <table id="AppSelect">
            <tr>
                <td class="Title">
                    AppSelect测试
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divAppSelect" runat="server">
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div>
                        <input id="Button7" type="button" value="DisplayTrue" onclick="setAppSelectDisplay(true)" />
                        <input id="Button8" type="button" value="DisplayFalse" onclick="setAppSelectDisplay(false)" />
                        <input id="AppSelectSelectedValue" type="text" value="1" />
                        <input id="Button9" type="button" value="setSelectedValue" onclick="setAppSelectValue()" />                        
                        <input id="Button11" type="button" value="DisabledTrue" onclick="setAppSelectDisabled(true)" />
                        <input id="Button12" type="button" value="DisabledFalse" onclick="setAppSelectDisabled(false)" />
                        <input id="Button1" type="button" value="RequireTrue" onclick="setAppSelectRequired(true)" />
                        <input id="Button2" type="button" value="RequireFalse" onclick="setAppSelectRequired(false)" />
                        <input id="Button3" type="button" value="3RemoveAll" onclick="section2_Select_Ctrl3.RemoveAll();" />
                        <input id="Button4" type="button" value="3Remove0" onclick="section2_Select_Ctrl3.RemoveAt(0)" />
                        <input id="Button5" type="button" value="3AppendAt100IN1" onclick="section2_Select_Ctrl3.AppendAt('100','100',1,true)" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
    </body>
</html>

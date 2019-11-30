<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppRadioTest.aspx.cs" Inherits="Map.Web.Test.AppRadioTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
        
     <script src="../Scripts/jquery.js" type="text/javascript"></script>

    <script src="../Scripts/Common.js" type="text/javascript"></script>
    <script type="text/javascript">
        function appRadioOnChange1() {
            alert("appRadioOnChange1()");
        }
        function appRadioOnChange2() {
            alert("appRadioOnChange2()");
        }
        function appRadioOnChange3() {
            alert("appRadioOnChange3()");
        }
        function appRadioOnChange4() {
            alert("appRadioOnChange4()");
        }
        function appRadioOnChange5() {
            alert("appRadioOnChange5()");
        }
        function appRadioOnChange6() {
            alert("appRadioOnChange6()");
        }
        function appRadioOnChange7() {
            alert("appRadioOnChange7()");
        }
        function appRadioOnChange8() {
            alert("appRadioOnChange8()");
        }
        function appRadioOnChange9() {
            alert("appRadioOnChange9()");
        }
        function appRadioOnChange10() {
            alert("appRadioOnChange10()");
        }
        function appRadioOnChange11() {
            alert("appRadioOnChange11()");
        }
        function appRadioOnChange12() {
            alert("appRadioOnChange12()");
        }
        function appRadioOnChange13() {
            alert("appRadioOnChange13()");
        }
        function appRadioOnChange14() {
            alert("appRadioOnChange14()");
        }
        function appRadioOnChange15() {
            alert("appRadioOnChange15()");
        }
        function appRadioOnChange16() {
            alert("appRadioOnChange16()");
        }
        function appRadioOnChange17() {
            alert("appRadioOnChange17()");
        }
        function appRadioOnChange18() {
            alert("appRadioOnChange18()");
        }
        function appRadioOnChange19() {
            alert("appRadioOnChange19()");
        }


        function setAppRadioDisplay(value) {
            section2_Radio_Ctrl1.setDisplay(value);
            section2_Radio_Ctrl2.setDisplay(value);
            section2_Radio_Ctrl3.setDisplay(value);
            section2_Radio_Ctrl4.setDisplay(value);
            section2_Radio_Ctrl5.setDisplay(value);
            section2_Radio_Ctrl6.setDisplay(value);
            section2_Radio_Ctrl7.setDisplay(value);
            section2_Radio_Ctrl8.setDisplay(value);
            section2_Radio_Ctrl9.setDisplay(value);
            section2_Radio_Ctrl10.setDisplay(value);
            section2_Radio_Ctrl11.setDisplay(value);
            section2_Radio_Ctrl12.setDisplay(value);
            section2_Radio_Ctrl13.setDisplay(value);
            section2_Radio_Ctrl14.setDisplay(value);
            section2_Radio_Ctrl15.setDisplay(value);
            section2_Radio_Ctrl16.setDisplay(value);
        }

        function setAppRadioDisabled(value) {
            section2_Radio_Ctrl1.setDisabled(value);
            section2_Radio_Ctrl2.setDisabled(value);
            section2_Radio_Ctrl3.setDisabled(value);
            section2_Radio_Ctrl4.setDisabled(value);
            section2_Radio_Ctrl5.setDisabled(value);
            section2_Radio_Ctrl6.setDisabled(value);
            section2_Radio_Ctrl7.setDisabled(value);
            section2_Radio_Ctrl8.setDisabled(value);
            section2_Radio_Ctrl9.setDisabled(value);
            section2_Radio_Ctrl10.setDisabled(value);
            section2_Radio_Ctrl11.setDisabled(value);
            section2_Radio_Ctrl12.setDisabled(value);
            section2_Radio_Ctrl13.setDisabled(value);
            section2_Radio_Ctrl14.setDisabled(value);
            section2_Radio_Ctrl15.setDisabled(value);
            section2_Radio_Ctrl16.setDisabled(value);

        }

        function setAppRadioRequired(value) {
            var showMessage = "";
            section2_Radio_Ctrl1.setRequired(value);
            showMessage += "section2_Radio_Ctrl1    Required：" + (section2_Radio_Ctrl1.isValid() ? "True" : "False") + "\r\n";
            section2_Radio_Ctrl2.setRequired(value);
            showMessage += "section2_Radio_Ctrl2    Required：" + (section2_Radio_Ctrl2.isValid() ? "True" : "False") + "\r\n";
            section2_Radio_Ctrl3.setRequired(value);
            showMessage += "section2_Radio_Ctrl3    Required：" + (section2_Radio_Ctrl3.isValid() ? "True" : "False") + "\r\n";
            section2_Radio_Ctrl4.setRequired(value);
            showMessage += "section2_Radio_Ctrl4    Required：" + (section2_Radio_Ctrl4.isValid() ? "True" : "False") + "\r\n";
            section2_Radio_Ctrl5.setRequired(value);
            showMessage += "section2_Radio_Ctrl5    Required：" + (section2_Radio_Ctrl5.isValid() ? "True" : "False") + "\r\n";
            section2_Radio_Ctrl6.setRequired(value);
            showMessage += "section2_Radio_Ctrl6    Required：" + (section2_Radio_Ctrl6.isValid() ? "True" : "False") + "\r\n";
            section2_Radio_Ctrl7.setRequired(value);
            showMessage += "section2_Radio_Ctrl7    Required：" + (section2_Radio_Ctrl7.isValid() ? "True" : "False") + "\r\n";
            section2_Radio_Ctrl8.setRequired(value);
            showMessage += "section2_Radio_Ctrl8    Required：" + (section2_Radio_Ctrl8.isValid() ? "True" : "False") + "\r\n";
            section2_Radio_Ctrl9.setRequired(value);
            showMessage += "section2_Radio_Ctrl9    Required：" + (section2_Radio_Ctrl9.isValid() ? "True" : "False") + "\r\n";
            section2_Radio_Ctrl10.setRequired(value);
            showMessage += "section2_Radio_Ctrl10    Required：" + (section2_Radio_Ctrl10.isValid() ? "True" : "False") + "\r\n";
            section2_Radio_Ctrl11.setRequired(value);
            showMessage += "section2_Radio_Ctrl11    Required：" + (section2_Radio_Ctrl11.isValid() ? "True" : "False") + "\r\n";
            section2_Radio_Ctrl12.setRequired(value);
            showMessage += "section2_Radio_Ctrl12    Required：" + (section2_Radio_Ctrl12.isValid() ? "True" : "False") + "\r\n";
            section2_Radio_Ctrl13.setRequired(value);
            showMessage += "section2_Radio_Ctrl13    Required：" + (section2_Radio_Ctrl13.isValid() ? "True" : "False") + "\r\n";
            section2_Radio_Ctrl14.setRequired(value);
            showMessage += "section2_Radio_Ctrl14    Required：" + (section2_Radio_Ctrl14.isValid() ? "True" : "False") + "\r\n";
            section2_Radio_Ctrl15.setRequired(value);
            showMessage += "section2_Radio_Ctrl15    Required：" + (section2_Radio_Ctrl15.isValid() ? "True" : "False") + "\r\n";
            section2_Radio_Ctrl16.setRequired(value);
            showMessage += "section2_Radio_Ctrl16    Required：" + (section2_Radio_Ctrl16.isValid() ? "True" : "False") + "\r\n";
            alert(showMessage);
        }

        function setAppRadioValue() {
            var value = $("#AppRadioSelectedValue").val();
            section2_Radio_Ctrl1.setValue(value);
            section2_Radio_Ctrl2.setValue(value);
            section2_Radio_Ctrl3.setValue(value);
            section2_Radio_Ctrl4.setValue(value);
            section2_Radio_Ctrl5.setValue(value);
            section2_Radio_Ctrl6.setValue(value);
            section2_Radio_Ctrl7.setValue(value);
            section2_Radio_Ctrl8.setValue(value);
            section2_Radio_Ctrl9.setValue(value);
            section2_Radio_Ctrl10.setValue(value);
            section2_Radio_Ctrl11.setValue(value);
            section2_Radio_Ctrl12.setValue(value);
            section2_Radio_Ctrl13.setValue(value);
            section2_Radio_Ctrl14.setValue(value);
            section2_Radio_Ctrl15.setValue(value);
            section2_Radio_Ctrl16.setValue(value);
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
        <table id="AppRadio">
            <tr>
                <td class="Title">
                    AppRadio测试
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divAppRadio" runat="server">
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div>
                        <input id="Button7" type="button" value="DisplayTrue" onclick="setAppRadioDisplay(true)" />
                        <input id="Button8" type="button" value="DisplayFalse" onclick="setAppRadioDisplay(false)" />
                        <input id="AppRadioSelectedValue" type="text" value="1" />
                        <input id="Button9" type="button" value="setSelectedValue" onclick="setAppRadioValue()" />                        
                        <input id="Button11" type="button" value="DisabledTrue" onclick="setAppRadioDisabled(true)" />
                        <input id="Button12" type="button" value="DisabledFalse" onclick="setAppRadioDisabled(false)" />
                        <input id="Button1" type="button" value="RequireTrue" onclick="setAppRadioRequired(true)" />
                        <input id="Button2" type="button" value="RequireFalse" onclick="setAppRadioRequired(false)" />
                        <input id="Button3" type="button" value="1RemoveAll" onclick="section2_Radio_Ctrl1.RemoveAll();" />
                        <input id="Button4" type="button" value="1Remove10" onclick="section2_Radio_Ctrl1.RemoveAt(10)" />
                        <input id="Button5" type="button" value="1AppendAt100IN10" onclick="section2_Radio_Ctrl1.AppendAt('100','100',10,true)" />
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>

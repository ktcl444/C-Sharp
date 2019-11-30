<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppCheckBoxTest.aspx.cs"
    Inherits="Map.Web.Test.AppCheckBoxTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <script src="../Scripts/jquery.js" type="text/javascript"></script>
    <script src="../Scripts/Common.js" type="text/javascript"></script>

    <script type="text/javascript">
        function appCheckBoxOnClick1() {
            alert("appCheckBoxOnClick1()");
        }
        function appCheckBoxOnClick2() {
            alert("appCheckBoxOnClick2()");
        }
        function appCheckBoxOnClick3() {
            alert("appCheckBoxOnClick3()");
        }
        function appCheckBoxOnClick4() {
            alert("appCheckBoxOnClick4()");
        }
        function appCheckBoxOnClick5() {
            alert("appCheckBoxOnClick5()");
        }
        function appCheckBoxOnClick6() {
            alert("appCheckBoxOnClick6()");
        }
        function appCheckBoxOnClick7() {
            alert("appCheckBoxOnClick7()");
        }
        function appCheckBoxOnClick8() {
            alert("appCheckBoxOnClick8()");
        }
        function appCheckBoxOnClick9() {
            alert("appCheckBoxOnClick9()");
        }
        function appCheckBoxOnClick10() {
            alert("appCheckBoxOnClick10()");
        }
        function appCheckBoxOnClick11() {
            alert("appCheckBoxOnClick11()");
        }
        function appCheckBoxOnClick12() {
            alert("appCheckBoxOnClick12()");
        }
        function appCheckBoxOnClick13() {
            alert("appCheckBoxOnClick13()");
        }
        function appCheckBoxOnClick14() {
            alert("appCheckBoxOnClick14()");
        }
        function appCheckBoxOnClick15() {
            alert("appCheckBoxOnClick15()");
        }
        function appCheckBoxOnClick16() {
            alert("appCheckBoxOnClick16()");
        }
        function appCheckBoxOnClick17() {
            alert("appCheckBoxOnClick17()");
        }
        function appCheckBoxOnClick18() {
            alert("appCheckBoxOnClick18()");
        }
        function appCheckBoxOnClick19() {
            alert("appCheckBoxOnClick19()");
        }


        function setAppCheckBoxDisplay(value) {
            section2_CheckBox_Ctrl1.setDisplay(value);
            section2_CheckBox_Ctrl2.setDisplay(value);
            section2_CheckBox_Ctrl3.setDisplay(value);
            section2_CheckBox_Ctrl4.setDisplay(value);
            section2_CheckBox_Ctrl5.setDisplay(value);
            section2_CheckBox_Ctrl6.setDisplay(value);
            section2_CheckBox_Ctrl7.setDisplay(value);
            section2_CheckBox_Ctrl8.setDisplay(value);
            section2_CheckBox_Ctrl9.setDisplay(value);
            section2_CheckBox_Ctrl10.setDisplay(value);
            section2_CheckBox_Ctrl11.setDisplay(value);
            section2_CheckBox_Ctrl12.setDisplay(value);
            section2_CheckBox_Ctrl13.setDisplay(value);
            section2_CheckBox_Ctrl14.setDisplay(value);
            section2_CheckBox_Ctrl15.setDisplay(value);
            section2_CheckBox_Ctrl16.setDisplay(value);

        }

        function setAppCheckBoxValue(value) {
            section2_CheckBox_Ctrl1.setValue(value);
            section2_CheckBox_Ctrl2.setValue(value);
            section2_CheckBox_Ctrl3.setValue(value);
            section2_CheckBox_Ctrl4.setValue(value);
            section2_CheckBox_Ctrl5.setValue(value);
            section2_CheckBox_Ctrl6.setValue(value);
            section2_CheckBox_Ctrl7.setValue(value);
            section2_CheckBox_Ctrl8.setValue(value);
            section2_CheckBox_Ctrl9.setValue(value);
            section2_CheckBox_Ctrl10.setValue(value);
            section2_CheckBox_Ctrl11.setValue(value);
            section2_CheckBox_Ctrl12.setValue(value);
            section2_CheckBox_Ctrl13.setValue(value);
            section2_CheckBox_Ctrl14.setValue(value);
            section2_CheckBox_Ctrl15.setValue(value);
            section2_CheckBox_Ctrl16.setValue(value);

        }

        function setAppCheckBoxDisabled(value) {
            section2_CheckBox_Ctrl1.setDisabled(value);
            section2_CheckBox_Ctrl2.setDisabled(value);
            section2_CheckBox_Ctrl3.setDisabled(value);
            section2_CheckBox_Ctrl4.setDisabled(value);
            section2_CheckBox_Ctrl5.setDisabled(value);
            section2_CheckBox_Ctrl6.setDisabled(value);
            section2_CheckBox_Ctrl7.setDisabled(value);
            section2_CheckBox_Ctrl8.setDisabled(value);
            section2_CheckBox_Ctrl9.setDisabled(value);
            section2_CheckBox_Ctrl10.setDisabled(value);
            section2_CheckBox_Ctrl11.setDisabled(value);
            section2_CheckBox_Ctrl12.setDisabled(value);
            section2_CheckBox_Ctrl13.setDisabled(value);
            section2_CheckBox_Ctrl14.setDisabled(value);
            section2_CheckBox_Ctrl15.setDisabled(value);
            section2_CheckBox_Ctrl16.setDisabled(value);

        }

        function setAppCheckBoxRequired(value) {
            section2_CheckBox_Ctrl1.setRequired(value);
            section2_CheckBox_Ctrl2.setRequired(value);
            section2_CheckBox_Ctrl3.setRequired(value);
            section2_CheckBox_Ctrl4.setRequired(value);
            section2_CheckBox_Ctrl5.setRequired(value);
            section2_CheckBox_Ctrl6.setRequired(value);
            section2_CheckBox_Ctrl7.setRequired(value);
            section2_CheckBox_Ctrl8.setRequired(value);
            section2_CheckBox_Ctrl9.setRequired(value);
            section2_CheckBox_Ctrl10.setRequired(value);
            section2_CheckBox_Ctrl11.setRequired(value);
            section2_CheckBox_Ctrl12.setRequired(value);
            section2_CheckBox_Ctrl13.setRequired(value);
            section2_CheckBox_Ctrl14.setRequired(value);
            section2_CheckBox_Ctrl15.setRequired(value);
            section2_CheckBox_Ctrl16.setRequired(value);
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

    <%--<style type="text/css">
    /* AppCheckBox */
/*未勾选，不必填*/
div.Checkbox_NoChecked_NoRequired
{
	background-color: Aqua;
}
.Checkbox_NoChecked_NoRequired input
{
	background-color: Green;
}
.Checkbox_NoChecked_NoRequired label
{
}
/*未勾选，必填*/
div.Checkbox_NoChecked_Required
{
	background-color: Aqua;
}
.Checkbox_NoChecked_Required input
{
	background-color: Green;
}
.Checkbox_NoChecked_Required label
{
	color: Red;	
}
/*勾选，不必填*/
div.Checkbox_Checked_NoRequired
{
	background-color: Aqua;
}
.Checkbox_Checked_NoRequired input
{
	background-color: blue;
}
.Checkbox_Checked_NoRequired label
{
}
/*勾选，必填*/
div.Checkbox_Checked_Required
{
	background-color: Aqua;
}
.Checkbox_Checked_Required input
{
	background-color: blue;
}
.Checkbox_Checked_Required label
{
	color: Red;
}
/* AppCheckBox */
    </style>--%>
   
</head>
<body>
    <form id="form1" runat="server">
    <table id="AppCheckBox">
        <tr>
            <td class="Title">
                AppCheckBox测试
            </td>
        </tr>
        <tr>
            <td>
                <div id="divAppCheckBox" runat="server">
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div>
                    <input id="Button7" type="button" value="DisplayTrue" onclick="setAppCheckBoxDisplay(true)" />
                    <input id="Button8" type="button" value="DisplayFalse" onclick="setAppCheckBoxDisplay(false)" />
                    <input id="Button9" type="button" value="CheckedTrue" onclick="setAppCheckBoxValue(true)" />
                    <input id="Button10" type="button" value="CheckedFalse" onclick="setAppCheckBoxValue(false)" />
                    <input id="Button11" type="button" value="DisabledTrue" onclick="setAppCheckBoxDisabled(true)" />
                    <input id="Button12" type="button" value="DisabledFalse" onclick="setAppCheckBoxDisabled(false)" />
                    <input id="Button1" type="button" value="RequireTrue" onclick="setAppCheckBoxRequired(true)" />
                    <input id="Button2" type="button" value="RequireFalse" onclick="setAppCheckBoxRequired(false)" />
                </div>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>

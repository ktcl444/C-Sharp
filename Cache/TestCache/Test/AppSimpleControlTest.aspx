<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppSimpleControlTest.aspx.cs"
    Inherits="Map.Web.Test.AppSimpleControlTest" %>

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

        function setAppCheckBoxDisplay(value) {
            window.section2_CheckBox_Ctrl1.setDisplay(value);
        }

        function setAppCheckBoxValue(value) {
            window.section2_CheckBox_Ctrl1.setValue(value);
        }

        function setAppCheckBoxDisabled(value) {
            window.section2_CheckBox_Ctrl1.setDisabled(value);
        }

        function setAppCheckBoxRequired(value) {
            window.section2_CheckBox_Ctrl1.setRequired(value);
        }

        function getAppCheckBoxReturnValue() {
            var tempValue = window.section2_CheckBox_Ctrl1.getReturnValue();
            alert(tempValue);
        }
        function getAppCheckBoxInitValue() {
            var tempValue = window.section2_CheckBox_Ctrl1.getInitValue();
            alert(tempValue);
        }
        function getAppCheckBoxPreviousValue() {
            var tempValue = window.section2_CheckBox_Ctrl1.getPreviousValue();
            alert(tempValue);
        }
        function getAppCheckBoxText() {
            var tempValue = window.section2_CheckBox_Ctrl1.getText();
            //alert($("#lblsection2_CheckBox_Ctrl1").text())
            alert(tempValue);
        }
        function getAppCheckBoxValue() {
            var tempValue = window.section2_CheckBox_Ctrl1.getValue();
            alert(tempValue);
        }
        function resetAppCheckBox() {
            window.section2_CheckBox_Ctrl1.reset();
            getAppCheckBoxValue();
        }
        
        
    </script>

    <script type="text/javascript">
        function setHiddenValue() {
            section2_Hidden_Ctrl.setValue($("#Text1").val());
        }
        function showHiddenValue() {
            alert(section2_Hidden_Ctrl.getValue());
        }
        function getHiddenReturnValue() {
            var tempValue = window.section2_Hidden_Ctrl.getReturnValue();
            alert(tempValue);
        }
        function getHiddenInitValue() {
            var tempValue = window.section2_Hidden_Ctrl.getInitValue();
            alert(tempValue);
        }
        function getHiddenPreviousValue() {
            var tempValue = window.section2_Hidden_Ctrl.getPreviousValue();
            alert(tempValue);
        }
        function getHiddenValue() {
            var tempValue = window.section2_Hidden_Ctrl.getValue();
            alert(tempValue);
        }
        function resetHidden() {
            window.section2_Hidden_Ctrl.reset();
            getHiddenValue();
        }
    </script>

    <script type="text/javascript">

        function setAppRadioDisplay(value) {
            section2_Radio_Ctrl1.setDisplay(value);
        }

        function setAppRadioDisabled(value) {
            section2_Radio_Ctrl1.setDisabled(value);
        }

        function setAppRadioRequired(value) {
            var showMessage = "";
            section2_Radio_Ctrl1.setRequired(value);
            showMessage += "section2_Radio_Ctrl1    Required：" + (section2_Radio_Ctrl1.isValid() ? "True" : "False") + "\r\n";
            alert(showMessage);
        }

        function setAppRadioValue() {
            var value = $("#AppRadioSelectedValue").val();
            section2_Radio_Ctrl1.setValue(value);
        }
        function getAppRadioReturnValue() {
            var tempValue = window.section2_Radio_Ctrl1.getReturnValue();
            alert(tempValue);
        }
        function getAppRadioInitValue() {
            var tempValue = window.section2_Radio_Ctrl1.getInitValue();
            alert(tempValue);
        }
        function getAppRadioPreviousValue() {
            var tempValue = window.section2_Radio_Ctrl1.getPreviousValue();
            alert(tempValue);
        }
                
        function getAppRadioText() {
            var tempValue = window.section2_Radio_Ctrl1.getText();
            alert(tempValue);
        }
        function getAppRadioValue() {
            var tempValue = window.section2_Radio_Ctrl1.getValue();
            alert(tempValue);
        }
        function resetAppRadio() {
            window.section2_Radio_Ctrl1.reset();
            getAppRadioValue();
        }
        function replaceAppRadioAllItem() {
            var allItem = eval("[['itemAAAA','0'],['item1','1'],['item2','2'],['item3','3'],['item4','4'],['item5','5'],['item6','6'],['item7','7'],['item8','8'],['item9','9'],['item10','10'],['item11','11'],['item12','12'],['item13','13'],['item14','14'],['item15','15'],['item16','16'],['item17','17'],['item18','18'],['item19','19']]");
            section2_Radio_Ctrl1.replaceAllItem(allItem);
        }
    </script>

    <script type="text/javascript">
        function appSelectOnChange1() {
            alert("appSelectOnChange1()");
        }
        function setAppSelectDisplay(value) {
            section2_Select_Ctrl1.setDisplay(value);

        }
        function setAppSelectDisabled(value) {
            section2_Select_Ctrl1.setDisabled(value);
        }

        function setAppSelectRequired(value) {
            var showMessage = "";
            section2_Select_Ctrl1.setRequired(value);
            showMessage += "section2_Select_Ctrl1    Required：" + (section2_Select_Ctrl1.isValid() ? "True" : "False") + "\r\n";
            alert(showMessage);
        }

        function setAppSelectValue() {
            var value = $("#AppSelectSelectedValue").val();
            section2_Select_Ctrl1.setValue(value);
        }
        
    </script>

    <style type="text/css">
        .Title
        {
            font-size: xx-large;
            font-weight: bolder;
            color: Red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div id="AppSimpleControl" runat="server">
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
                            <input id="Button7" type="button" value="显示" onclick="setAppCheckBoxDisplay(true)" />
                            <input id="Button8" type="button" value="隐藏" onclick="setAppCheckBoxDisplay(false)" />
                            <input id="Button9" type="button" value="选中" onclick="setAppCheckBoxValue(true)" />
                            <input id="Button10" type="button" value="非选中" onclick="setAppCheckBoxValue(false)" />
                            <input id="Button11" type="button" value="禁用" onclick="setAppCheckBoxDisabled(true)" />
                            <input id="Button12" type="button" value="非禁用" onclick="setAppCheckBoxDisabled(false)" />
                            <input id="Button1" type="button" value="必填" onclick="setAppCheckBoxRequired(true)" />
                            <input id="Button2" type="button" value="非必填" onclick="setAppCheckBoxRequired(false)" />
                            <input id="Button31" type="button" value="获取返回值" onclick="getAppCheckBoxReturnValue()" />
                            <input id="Button32" type="button" value="获取初始值" onclick="getAppCheckBoxInitValue()" />
                            <input id="Button33" type="button" value="获取前一个值" onclick="getAppCheckBoxPreviousValue()" />                            
                            <input id="Button36" type="button" value="获取值" onclick="getAppCheckBoxValue()" />  
                            <input id="Button35" type="button" value="复位" onclick="resetAppCheckBox()" />
                            <input id="Button34" type="button" value="获取文本" onclick="getAppCheckBoxText()" />                    
                            
                        </div>
                    </td>
                </tr>
            </table>
            <table id="AppHidden">
                <tr>
                    <td class="Title">
                        AppHidden测试
                    </td>
                </tr>
                <tr>
                    <td>
                        <div id="divAppHidden" runat="server">
                            <span>
                                <asp:Label ID="Label1" runat="server" Text="HiddenValue"></asp:Label>
                                <input id="Text1" type="text" value="1234567890<>,!@#$%^&&*()" style="width: 500px;" /></span>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div>
                            <input id="Button3" type="button" value="设值" onclick="setHiddenValue()" />
                            <input id="Button4" type="button" value="读值" onclick="showHiddenValue()" />
                            <input id="Button37" type="button" value="获取返回值" onclick="getHiddenReturnValue()" />
                            <input id="Button38" type="button" value="获取初始值" onclick="getHiddenInitValue()" />
                            <input id="Button41" type="button" value="获取前一个值" onclick="getHiddenPreviousValue()" />                            
                            <input id="Button42" type="button" value="获取值" onclick="getHiddenValue()" />  
                            <input id="Button43" type="button" value="复位" onclick="resetHidden()" />

                        </div>
                    </td>
                </tr>
            </table>
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
                            <input id="Button5" type="button" value="显示" onclick="setAppRadioDisplay(true)" />
                            <input id="Button6" type="button" value="隐藏" onclick="setAppRadioDisplay(false)" />
                            <input id="AppRadioSelectedValue" type="text" value="5" />
                            <input id="Button13" type="button" value="设值" onclick="setAppRadioValue()" />
                            <input id="Button14" type="button" value="禁用" onclick="setAppRadioDisabled(true)" />
                            <input id="Button15" type="button" value="非禁用" onclick="setAppRadioDisabled(false)" />
                            <input id="Button16" type="button" value="必填" onclick="setAppRadioRequired(true)" />
                            <input id="Button17" type="button" value="非必填" onclick="setAppRadioRequired(false)" />
                            <input id="Button18" type="button" value="移除所有选项" onclick="section2_Radio_Ctrl1.removeAll();" />
                            <input id="Button19" type="button" value="移除第10选项" onclick="section2_Radio_Ctrl1.removeAt(10)" />
                            <input id="Button20" type="button" value="将100选项添加在第10选位置" onclick="section2_Radio_Ctrl1.appendAt('100','100',10,true)" />
                            <input id="Button44" type="button" value="获取返回值" onclick="getAppRadioReturnValue()" />
                            <input id="Button45" type="button" value="获取初始值" onclick="getAppRadioInitValue()" />
                            <input id="Button46" type="button" value="获取前一个值" onclick="getAppRadioPreviousValue()" />                            
                            <input id="Button47" type="button" value="获取值" onclick="getAppRadioValue()" />  
                            <input id="Button48" type="button" value="复位" onclick="resetAppRadio()" />
                            <input id="Button49" type="button" value="替换所有选项" onclick="replaceAppRadioAllItem()" />
                            

                        </div>
                    </td>
                </tr>
            </table>
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
                            <input id="Button21" type="button" value="显示" onclick="setAppSelectDisplay(true)" />
                            <input id="Button22" type="button" value="隐藏" onclick="setAppSelectDisplay(false)" />
                            <input id="AppSelectSelectedValue" type="text" value="5" />
                            <input id="Button23" type="button" value="设值" onclick="setAppSelectValue()" />
                            <input id="Button24" type="button" value="禁用" onclick="setAppSelectDisabled(true)" />
                            <input id="Button25" type="button" value="非禁用" onclick="setAppSelectDisabled(false)" />
                            <input id="Button26" type="button" value="必填" onclick="setAppSelectRequired(true)" />
                            <input id="Button27" type="button" value="非必填" onclick="setAppSelectRequired(false)" />
                            <input id="Button28" type="button" value="移除所有选项" onclick="section2_Select_Ctrl1.RemoveAll();" />
                            <input id="Button29" type="button" value="移除第0选项" onclick="section2_Select_Ctrl1.RemoveAt(0)" />
                            <input id="Button30" type="button" value="将100选项添加在第1选位置" onclick="section2_Select_Ctrl1.AppendAt('100','100',1,true)" />
                        </div>
                    </td>
                </tr>
            </table>            
            <table id="AppDate">
            <tr>
                <td class="Title">
                    AppDate测试
                </td>
            </tr>
            <tr>
                <td>
                    <div id="divAppDate" runat="server"  style="width:200px">
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <div>
<%--                        <input id="Button31" type="button" value="DisplayTrue" onclick="setDisplay(true)" />
                        <input id="Button32" type="button" value="DisplayFalse" onclick="setDisplay(false)" />
                        <input id="Button33" type="button" value="CheckedTrue" onclick="setValue('1')" />
                        <input id="Button34" type="button" value="CheckedFalse" onclick="setValue('0')" />
                        <input id="Button35" type="button" value="DisabledTrue" onclick="setDisabled(true)" />
                        <input id="Button36" type="button" value="DisabledFalse" onclick="setDisabled(false)" />
                        <input id="Button37" type="button" value="RequireTrue" onclick="setRequired(true)" />
                        <input id="Button38" type="button" value="RequireFalse" onclick="setRequired(false)" />
--%>
 <input id="Button39" type="button" value="ReadOnlyFalse" onclick="ctl07.SetReadOnly(false)" />
 <input id="Button40" type="button" value="ReadOnlyFalse" onclick="ctl07.SetReadOnly(true)" />
</div>


                </td>
            </tr>
        </table>
        </div>
    </div>
    </form>
</body>
</html>

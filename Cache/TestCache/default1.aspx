<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default1.aspx.cs" Inherits="Map.Web.default1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
     <title>AppGrid控件HTML结构</title>
    <script src="Scripts/jquery.js" type="text/javascript"></script>
    <script src="Scripts/AppGrid.js" type="text/javascript"></script>
    
    <script type="text/javascript" language="javascript">
        window.onload = function InitGrid()
        {
            var grid_DivLeftContent = document.getElementById("grid_DivLeftContent");
            var grid_TbRightContent = document.getElementById("grid_TbRightContent");

            var fragment = document.createDocumentFragment();
            var strRow = grid_TbLeftContent.tBodies[0];


            var strTable = "<TABLE id=\"grid_TbLeftContent\" style=\"TABLE-LAYOUT: fixed\" cellSpacing=0 cellPadding=0 border=0>";
            strTable += "<TBODY >";
            for (var i = 0; i < 20; i++)
            {
                strTable += "<tr class=\"tr\" id=\"grid_LeftTr" + i + "\">\n";
                strTable += "    <td style=\"width: 20px;\" class=\"content\" id=\"grid_TdContent" + i + "_" + i + "\">\n";
                strTable += "          <input type=\"checkbox\" id=\"grid_Chk" + i + "\" />\n"
                strTable += "    </td>\n"
                strTable += "</tr>\n"
            }
            strTable += "</TBODY>";
            strTable += "</TABLE>";
            grid_DivLeftContent.innerHTML = strTable;



        }
    </script>

</head>
<body>
<form id="form1" runat="server" >
    <table cellpadding="0" cellspacing="0" border="0" id="tb">
        <tr style="height: 60px;">
            <td style="width: 12%; text-align: center; vertical-align: middle; border: 1px dashed orange;
                font-size: 18px; font-weight: bold; color: orange;">
                1
            </td>
            <td style="width: 88%; text-align: center; vertical-align: middle; border: 1px dashed orange;
                font-size: 18px; font-weight: bold; color: orange;">
                2
            </td>
        </tr>
        <tr id="tr">
            <td style="width: 12%; text-align: center; vertical-align: middle; border: 1px dashed orange;
                font-size: 18px; font-weight: bold; color: orange;">
                3
            </td>
            <td id="td" style="width: 100%; text-align: center; vertical-align: middle;">
                <div class="container">
                    <!-- AppGrid主表格 -->
                    <table class="main" style="height: 100%;" border="0" cellpadding="0"
                        cellspacing="0" id="grid" showfilter="true" showaggregation="true" pagesize="100"
                        leftcolumns="['grid_TdTitle_0']" rightcolumns="['grid_TdTitle_1', 'grid_TdTitle_2', 'grid_TdTitle_3', 'grid_TdTitle_4','grid_TdTitle_5','grid_TdTitle_6','grid_TdTitle_7','grid_TdTitle_8', 'grid_TdTitle_9']">
                        <!-- 表体 -->
                        <tr id="grid_TrContent">
                            <!-- 左边内容表格 -->
                            <td valign="top" id="grid_TdLeft">
                                <div style="overflow-x: hidden; overflow-y: hidden;" id="grid_DivLeftMain">
                                    <table border="0" cellpadding="0" cellspacing="0" style="table-layout: fixed;" id="grid_TbLeftMain">
                                        <tr style="height: 24px;" id="grid_TrLeftTitle">
                                            <td class="title" style="width: 20px;" id="grid_TdTitle_0">
                                                <input type="checkbox" id="grid_ChkAll" title="全选" />
                                            </td>
                                        </tr>
                                        <tr style="height: 24px;" id="grid_TrLeftFilter">
                                            <td class="filter" style="width: 20px;" id="grid_TdFilter_0">
                                                <img alt="高级查询" id="grid_ImgSearch" src="Image/find.gif"border="0" />
                                            </td>
                                        </tr>
                                        <tr id="grid_TrLeftContent">
                                            <td id="grid_TdLeftContent">
                                                <div id="grid_DivLeftContainer" style="position: relative; overflow: hidden">
                                                    <div id="grid_DivLeftContent" style="position: relative">
<%--                                                        <table id="grid_TbLeftContent" border="0" cellpadding="0" cellspacing="0" style="table-layout: fixed;">
                                                            <tr class="tr" id="grid_LeftTr0">
                                                                <td style="width: 20px;" class="content" id="grid_TdContent0_0">
                                                                    <input type="checkbox" id="grid_Chk0" />
                                                                </td>
                                                            </tr>
                                                            <tr class="tr" id="grid_LeftTr1">
                                                                <td style="width: 20px;" class="content" id="grid_TdContent1_0">
                                                                    <input type="checkbox" id="grid_Chk1" />
                                                                </td>
                                                            </tr>
                                                            <tr class="tr" id="grid_LeftTr2">
                                                                <td style="width: 20px;" class="content" id="grid_TdContent2_0">
                                                                    <input type="checkbox" id="grid_Chk2" />
                                                                </td>
                                                            </tr>
                                                            <tr class="tr" id="grid_LeftTr3">
                                                                <td style="width: 20px;" class="content" id="grid_TdContent3_0">
                                                                    <input type="checkbox" id="grid_Chk3" />
                                                                </td>
                                                            </tr>
                                                            <tr class="tr" id="grid_LeftTr4">
                                                                <td style="width: 20px;" class="content" id="grid_TdContent4_0">
                                                                    <input type="checkbox" id="grid_Chk4" />
                                                                </td>
                                                            </tr>
                                                        </table>--%>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr style="height: 24px;" id="grid_TrLeftAggregation">
                                            <td class="aggregation" style="width: 20px;" id="grid_TdAggregation_0">
                                                &nbsp;
                                            </td>
                                        </tr>
                                        <tr style="height: 17px;" id="grid_TrLeftScroll">
                                            <td class="scroll">
                                                &nbsp;
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                            <!-- 右边内容表格 -->
                            <td valign="top" id="grid_TdRight" >
                                <div style="overflow-x: hidden; overflow-y: hidden;" id="grid_DivRightMain">
                                    <table border="0" cellpadding="0" cellspacing="0" style="table-layout: fixed;" id="grid_TbRightMain">
                                        <!--表头部分-->
                                        <tr>
                                            <td>
                                                <div id="grid_DivRightTitleContainer" style="position: relative; overflow: hidden">
                                                    <div id="grid_DivRightTitleContent" style="position: relative">
                                                        <table id="grid_tbRightTitleContent" border="0" cellpadding="0" cellspacing="0" style="table-layout: fixed;">
                                                            <tr style="height: 24px;" id="grid_TrRightTitle">
                                                                <td class="title" style="width: 100px;" id="grid_TdTitle_1">
                                                                    Column1
                                                                </td>
                                                                <td class="title" style="width: 60px;" id="grid_TdTitle_2">
                                                                    Column2
                                                                </td>
                                                                <td class="title" style="width: 60px;" id="grid_TdTitle_3">
                                                                    Column3
                                                                </td>
                                                                <td class="title" style="width: 60px;" id="grid_TdTitle_4">
                                                                    Column4
                                                                </td>
                                                                <td class="title" style="width: 60px;" id="grid_TdTitle_5">
                                                                    Column5
                                                                </td>
                                                                <td class="title" style="width: 60px;" id="grid_TdTitle_6">
                                                                    Column6
                                                                </td>
                                                                <td class="title" style="width: 60px;" id="grid_TdTitle_7">
                                                                    Column7
                                                                </td>
                                                                <td class="title" style="width: 120px;" id="grid_TdTitle_8">
                                                                    Column8
                                                                </td>
                                                                <td class="title" style="width: 120px;" id="grid_TdTitle_9">
                                                                    Column9
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                        <!--筛选行-->
                                        <tr>
                                            <td>
                                                <div id="grid_DivRightFilterContainer" style="position: relative; overflow: hidden">
                                                    <div id="grid_DivRightFilterContent" style="position: relative; top: 0px; left: 0px;">
                                                        <table id="grid_TbRightFilterContent" border="0" cellpadding="0" cellspacing="0" style="table-layout: fixed;">
                                                            <tr style="height: 24px;" id="grid_TrRightFilter">
                                                                <td class="filter" style="width: 100px;" id="grid_TdFilter_1">
                                                                    <input value="f1" type="text" class="textbox" style="width: 90%;" />
                                                                </td>
                                                                <td class="filter" style="width: 60px;" id="grid_TdFilter_2">
                                                                    <input value="f2" type="text" class="textbox" style="width: 90%;" />
                                                                </td>
                                                                <td class="filter" style="width: 60px;" id="grid_TdFilter_3">
                                                                    <input value="f3" type="text" class="textbox" style="width: 90%;" />
                                                                </td>
                                                                <td class="filter" style="width: 60px;" id="grid_TdFilter_4">
                                                                    <input value="f4" type="text" class="textbox" style="width: 90%;" />
                                                                </td>
                                                                <td class="filter" style="width: 60px;" id="grid_TdFilter_5">
                                                                    <input value="f5" type="text" class="textbox" style="width: 90%;" />
                                                                </td>
                                                                <td class="filter" style="width: 60px;" id="grid_TdFilter_6">
                                                                    <input value="f6" type="text" class="textbox" style="width: 90%;" />
                                                                </td>
                                                                <td class="filter" style="width: 60px;" id="grid_TdFilter_7">
                                                                    <input value="f7" type="text" class="textbox" style="width: 90%;" />
                                                                </td>
                                                                <td class="filter" style="width: 120px;" id="grid_TdFilter_8">
                                                                    <input value="f8" type="text" class="textbox" style="width: 90%;" />
                                                                </td>
                                                                <td class="filter" style="width: 120px;" id="grid_TdFilter_9">
                                                                    <input value="f9" type="text" class="textbox" style="width: 90%;" />
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                        <!-- 右则表格主体 -->
                                        <tr>
                                            <td id="grid_TdRightContent"  style="width:100%" >
                                                <div id="grid_DivRightContainer" style="position: relative; overflow: hidden">
                                                    <div id="grid_DivRightContent" style="position: relative">
                                                        <table id="grid_TbRightContent" border="0" cellpadding="0" cellspacing="0" style="table-layout: fixed;">
                                                            <tr class="tr" id="grid_TrRightContent0">
                                                                <td style="width: 100px;" class="content" id="grid_TdContent0_1">
                                                                    123456789abcdefghijklmnopqrstuvwxyz
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent0_2">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent0_3">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent0_4">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent0_5">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent0_6">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent0_7">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 120px;" class="content" id="grid_TdContent0_8">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 120px;" class="content" id="grid_TdContent0_9">
                                                                    0
                                                                </td>
                                                            </tr>
                                                            <tr class="tr" id="grid_TrRightContent1">
                                                                <td style="width: 100px;" class="content" id="grid_TdContent1_1">
                                                                    1
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent1_2">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent1_3">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent1_4">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent1_5">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent1_6">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent1_7">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 120px;" class="content" id="grid_TdContent1_8">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 120px;" class="content" id="grid_TdContent1_9">
                                                                    1
                                                                </td>
                                                            </tr>
                                                            <tr class="tr" id="grid_TrRightContent2">
                                                                <td style="width: 100px;" class="content" id="grid_TdContent2_1">
                                                                    2
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent2_2">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent2_3">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent2_4">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent2_5">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent2_6">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent2_7">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 120px;" class="content" id="grid_TdContent2_8">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 120px;" class="content" id="grid_TdContent2_9">
                                                                    2
                                                                </td>
                                                            </tr>
                                                            <tr class="tr" id="grid_TrRightContent3">
                                                                <td style="width: 100px;" class="content" id="grid_TdContent3_1">
                                                                    3
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent3_2">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent3_3">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent3_4">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent3_5">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent3_6">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent3_7">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 120px;" class="content" id="grid_TdContent3_8">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 120px;" class="content" id="grid_TdContent3_9">
                                                                   3
                                                                </td>
                                                            </tr>
                                                            <tr class="tr" id="grid_TrRightContent4">
                                                                <td style="width: 100px;" class="content" id="grid_TdContent4_1">
                                                                    4
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent4_2">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent4_3">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent4_4">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent4_5">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent4_6">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 60px;" class="content" id="grid_TdContent4_7">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 120px;" class="content" id="grid_TdContent4_8">
                                                                    &nbsp;
                                                                </td>
                                                                <td style="width: 120px;" class="content" id="grid_TdContent4_9">
                                                                   4
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                        <!-- 聚合行 -->
                                        <tr>
                                            <td>
                                                <div id="grid_DivRightAggregationContainer" style="position: relative; overflow: hidden">
                                                    <div id="grid_DivRightAggregationContent" style="position: relative">
                                                        <table id="grid_TbRightAggregationContent" border="0" cellpadding="0" cellspacing="0"
                                                            style="table-layout: fixed;">
                                                            <tr style="height: 24px;" id="grid_TrRightAggregation">
                                                                <td class="aggregation" style="width: 100px;" id="grid_TdAggregation_1">
                                                                    合计：5050
                                                                </td>
                                                                <td class="aggregation" style="width: 60px;" id="grid_TdAggregation_2">
                                                                    &nbsp;
                                                                </td>
                                                                <td class="aggregation" style="width: 60px;" id="grid_TdAggregation_3">
                                                                    &nbsp;
                                                                </td>
                                                                <td class="aggregation" style="width: 60px;" id="grid_TdAggregation_4">
                                                                    &nbsp;
                                                                </td>
                                                                <td class="aggregation" style="width: 60px;" id="grid_TdAggregation_5">
                                                                    &nbsp;
                                                                </td>
                                                                <td class="aggregation" style="width: 60px;" id="grid_TdAggregation_6">
                                                                    &nbsp;
                                                                </td>
                                                                <td class="aggregation" style="width: 60px;" id="grid_TdAggregation_7">
                                                                    &nbsp;
                                                                </td>
                                                                <td class="aggregation" style="width: 120px;" id="grid_TdAggregation_8">
                                                                    &nbsp;
                                                                </td>
                                                                <td class="aggregation" style="width: 120px;" id="grid_TdAggregation_9">
                                                                    &nbsp;
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                        <!-- 水平滚动条 -->
                                        <tr style="height: 17px;" id="grid_TrHScroll">
                                            <td id="grid_TdHScroll" class="scroll">
                                                <div id="grid_DivHScroll" style="position: relative; overflow-x: scroll; overflow-y: hidden">
                                                    <div id="grid_DivHScrollContent" style="visibility: hidden; position: relative; height: 1px;">                                                    
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                            <td valign="top" id="grid_TdHelper">
                                <!-- 垂直滚动条表格 -->
                                <table style="width: 16px;" border="0" cellpadding="0" cellspacing="0">
                                    <tr style="height: 24px;">
                                        <td class="refresh">
                                            <img alt="刷新" src="Image/refresh.gif" border="0" />
                                        </td>
                                    </tr>
                                    <tr style="height: 24px;">
                                        <td class="refresh" id="grid_TdHideFilter">
                                            <img alt="过滤" src="Image/hide.gif" border="0" />
                                        </td>
                                    </tr>
                                    <!-- 垂直滚动条 -->
                                    <tr id="grid_TrVScroll">
                                        <td id="grid_TdVScroll" class="scroll">
                                            <div id="grid_DivVScroll" style="position: relative; overflow-x: hidden; overflow-y: scroll">
                                                <div id="grid_DivVScrollContent" style="position: relative; width: 1px; visibility: hidden">                                                
                                                </div>
                                            </div>
                                        </td>
                                    </tr>
                                     <!-- 聚合行的最后一格(空白处) -->
                                    <tr style="height: 24px;">
                                        <td class="refresh" id="grid_TdHideAggregation">
                                            &nbsp;
                                        </td>
                                    </tr>
                                    <!-- 横向滚动条空格（高？） -->
                                    <tr style="height: 17px;">
                                        <td class="scroll">
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <!-- 页脚 -->
                        <tr id="grid_TrPager" style="height: 20px;">
                            <td colspan="3" class="pager" id="grid_TdPager">
                                <table style="width: 100%;" cellpadding="0" cellspacing="0" border="0">
                                    <tr>
                                        <td style="width: 50%;">
                                            共100条记录 共10页当前第3页
                                        </td>
                                        <td style="width: 50%;" align="right">
                                            首页 上页 下页 末页
                                            <input type="button" value="转到" class="textbox" style="width: 40px;" />第<input type="text"
                                                class="textbox" style="width: 30px;" />页
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <!-- 拖动元素 -->
                    <div id="grid_dragItem" class="title" style="height: 24px; display: none; cursor: hand;
                        position: absolute; border: 1px solid #000000;">
                    </div>
                    <!-- 右键菜单 -->
                    <div id="grid_contextMemu" class="menu" style="display: none;">
                        <div id="grid_contextMemu_SFC">
                            冻结本列及前列</div>
                        <div id="grid_contextMemu_CFC">
                            取消冻结所有列</div>
                        <div id="grid_contextMemu_SFR">
                            显示过滤行</div>
                        <div id="grid_contextMemu_HFR">
                            隐藏过滤行</div>
                        <div id="grid_contextMemu_SAR">
                            显示统计行</div>
                        <div id="grid_contextMemu_HAR">
                            隐藏统计行</div>
                        <div id="grid_contextMemu_EXE">
                            导出Excel</div>
                        <div id="grid_contextMemu_EXP">
                            导出PDF</div>
                        <div id="grid_contextMemu_PRP">
                            打印预览</div>
                    </div>
                </div>
            </td>
        </tr>
    </table>
    <br />
    <br />
    <div id="info" style="font-size: 12pt; color: Red;">
    </div>
    <br />
    <div id="temp" style="border: dashed 2px green; width: 300px; height: 120px; position: absolute;
        top: -1500px; left: -1800px;">
    </div>
</form>
</html>

<script language="javascript">
    var mygrid = AppGrid("grid");
</script>


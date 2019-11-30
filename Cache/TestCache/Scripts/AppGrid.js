Array.prototype.AppendAt = function(index, value)
{
    var part1 = this.slice(0, index);
    var part2 = this.slice(index);
    part1.push(value);
    return (part1.concat(part2));
}
Array.prototype.RemoveAt = function(index)
{
    index++;
    var part1 = this.slice(0, index);
    var part2 = this.slice(index);
    part1.pop();
    return (part1.concat(part2));
}
Array.prototype.ChangeBefore = function(value1, value2)
{
    var idx1 = -1;
    var idx2 = -1;
    for (var i = 0; i < this.length; i++)
    {
        if (this[i] == value1) { idx1 = i; }
        if (this[i] == value2) { idx2 = i; }
    }
    for (var i = idx1; i > idx2; i--)
    {
        var tmp = this[i];
        this[i] = this[i - 1];
        this[i - 1] = tmp;
    }
}
Array.prototype.ChangeAfter = function(value1, value2)
{
    var idx1 = -1;
    var idx2 = -1;
    for (var i = 0; i < this.length; i++)
    {
        if (this[i] == value1) { idx1 = i; }
        if (this[i] == value2) { idx2 = i; }
    }
    if (idx1 >= 0 && idx2 >= 0)
    {
        for (var i = idx1; i < idx2; i++)
        {
            var tmp = this[i + 1];
            this[i + 1] = this[i];
            this[i] = tmp;
        }
    }
}

function AppGrid(id)
{
    var el = document.getElementById(id);
    var showFilter = (el.showfilter != undefined && el.showfilter.toLowerCase() == "true") ? true : false;
    var showAggregation = (el.showaggregation != undefined && el.showaggregation.toLowerCase() == "true") ? true : false;
    var pagesize = (el.pagesize != undefined) ? parseInt(el.pagesize) : 10;
    var allowMulitSelect = (el.allowmulitselect != undefined && el.allowmulitselect.toLowerCase() == "false") ? false : true;

    var gridWidth = $(el.parentNode).width();
    var gridHeight = $(el.parentNode).height();
    var titleHeight = $("#" + id + "_TrRightTitle").height();
    var scrollHeight = $("#" + id + "_TrHScroll").height();
    var contentHeight = $("#" + id + "_TbRightContent").height();
    var filterHeight = showFilter ? $("#" + id + "_TrRightFilter").height() : 0;
    var aggregationHeight = showAggregation ? $("#" + id + "_TrRightAggregation").height() : 0;
    var pagerHeight = $("#" + id + "_TrPager").height();
    var scrollWidth = $("#" + id + "_TdHelper").width();
    var mainHeight = gridHeight - pagerHeight;
    var leftWidth = $("#" + id + "_TbLeftMain").width();
    var rightWidth = $("#" + id + "_TbRightMain").width();

    function AdjustGridHeight()
    {
        var ht = mainHeight - titleHeight - filterHeight - aggregationHeight - scrollHeight - 4;
        $("#" + id + "_DivLeftContainer").height(ht);
        $("#" + id + "_DivRightContainer").height(ht);
        $("#" + id + "_DivVScroll").height(ht);
        $("#" + id + "_DivVScrollContent").height(contentHeight);
    }

    function AdjustGridWidth(wr)
    {
        var wc = $("#" + id + "_TbRightContent").width();
        var wl = $("#" + id + "_TbLeftMain").width();
        $("#" + id + "_DivLeftMain").width(wl);
        $("#" + id + "_DivRightMain").width(wr);
        $("#" + id + "_DivLeftContainer").width(wl);
        $("#" + id + "_DivRightContainer").width(wr);
        $("#" + id + "_DivLeftContent").width(wl);
        $("#" + id + "_DivRightContent").width(wr);
        $("#" + id + "_DivHScroll").width(wr);
        $("#" + id + "_DivHScrollContent").width(wc);
    }

    function InitGridSize()
    {
        AdjustGridHeight();

        AdjustGridWidth(rightWidth);
    }

    InitGridSize();

    var leftColumns = (el.leftcolumns != undefined) ? el.leftcolumns : "";
    var leftColumnList = eval(leftColumns);

    var rightColumns = (el.rightcolumns != undefined) ? el.rightcolumns : "";
    var rightColumnList = eval(rightColumns);

    function Sync_ScrollX()
    {
        document.getElementById(id + "_DivRightContent").style.posLeft = -document.getElementById(id + "_DivHScroll").scrollLeft;
        document.getElementById(id + "_DivRightTitleContent").style.posLeft = -document.getElementById(id + "_DivHScroll").scrollLeft;
        document.getElementById(id + "_DivRightFilterContent").style.posLeft = -document.getElementById(id + "_DivHScroll").scrollLeft;
        document.getElementById(id + "_DivRightAggregationContent").style.posLeft = -document.getElementById(id + "_DivHScroll").scrollLeft;
    }

    $("#" + id + "_DivHScroll").scroll(Sync_ScrollX);

    function Sync_SrcollY()
    {
        document.getElementById(id + "_DivLeftContent").style.posTop = -document.getElementById(id + "_DivVScroll").scrollTop;
        document.getElementById(id + "_DivRightContent").style.posTop = -document.getElementById(id + "_DivVScroll").scrollTop;
    }

    $("#" + id + "_DivVScroll").scroll(Sync_SrcollY);

    var dragResize = false, dragChange = false;
    var firstTitleCellX = 0, firstTitleCellY = 0;
    var dragSrcPos, dragDstPos, dragWidth, dragIndex, dragItem, dragObject, dragSrcObject, dragDstObject, dragTmpObject;
    var rightColumnListPosition = null;
    var dragToRight = true;
    var isFreeze = false;
    var contextObject = null;
    var idxCtm = 0;
    var frzCnt = $("#" + id + "_TrLeftTitle")[0].cells.length;

    function ResizeCell(cell, width)
    {
        cell.style.pixelWidth += width;
        cell.parentNode.parentNode.style.pixelWidth += width;
    }

    function DoMouseMove()
    {
        if (event.offsetX > event.srcElement.offsetWidth - 6 && event.offsetX < event.srcElement.offsetWidth)
        {
            event.srcElement.style.cursor = 'e-resize';
        }
        else
        {
            event.srcElement.style.cursor = 'hand';
        }

        if (dragResize)
        {
            dragDstPos = window.event.x;
            dragWidth = dragDstPos - dragSrcPos;
            if (dragWidth > 0 || dragObject.style.pixelWidth > 10)
            {
                dragIndex = dragObject.cellIndex;

                ResizeCell(dragObject, dragWidth);

                ResizeCell(document.getElementById(id + "_TbRightFilterContent").rows[0].cells[dragIndex], dragWidth);

                var tb = document.getElementById(id + "_TbRightContent");
                for (var i = 0; i < tb.rows.length; i++)
                {
                    ResizeCell(tb.rows[i].cells[dragIndex], dragWidth);
                }

                ResizeCell(document.getElementById(id + "_TbRightAggregationContent").rows[0].cells[dragIndex], dragWidth);

                document.getElementById(id + "_DivHScrollContent").style.pixelWidth += dragWidth;

                dragSrcPos = dragDstPos;
            }
        }
    }

    function DoDrag()
    {
        if (dragChange)
        {
            dragItem.style.left = window.event.x - dragItem.style.pixelWidth / 2;
            dragItem.style.top = window.event.y - dragItem.style.pixelHeight / 2;

            dragTmpObject = dragDstObject;
            dragDstObject = null;
            if (rightColumnListPosition != null && rightColumnListPosition.length > 0)
            {
                for (var i = 0; i < rightColumnListPosition.length; i++)
                {
                    if (window.event.screenX > rightColumnListPosition[i][0] && window.event.screenX < rightColumnListPosition[i][1] && window.event.screenY > rightColumnListPosition[i][2] && window.event.screenY < rightColumnListPosition[i][3])
                    {
                        dragDstObject = document.getElementById(rightColumnList[i]);
                        break;
                    }
                }
            }

            if (dragTmpObject != null && dragTmpObject != dragDstObject)
            {
                dragTmpObject.className = "title";
            }
            if (dragDstObject != null && dragSrcObject != dragDstObject)
            {
                if (parseInt(dragSrcObject.cellIndex) > parseInt(dragDstObject.cellIndex))
                {
                    dragToRight = false;
                }
                else
                {
                    dragToRight = true;
                }
                dragDstObject.className = "titlechange";
                dragItem.style.cursor = "move";
                dragItem.title = "把" + dragSrcObject.innerText + "列移动到" + dragDstObject.innerText + "列" + ((dragToRight) ? "后" : "前");
            }
            else
            {
                dragItem.style.cursor = "not-allowed";
            }
        }
    }

    function ChangeCell(tr, tdSrc, tdDst)
    {
        tr.removeChild(tdSrc);
        if (dragToRight)
        {
            if (tdDst.nextSibling)
            {
                tr.insertBefore(tdSrc, tdDst.nextSibling);
            }
            else
            {
                tr.appendChild(tdSrc);
            }
        }
        else
        {
            tr.insertBefore(tdSrc, tdDst);
        }
    }

    function DoDragCancle()
    {
        if (dragChange)
        {
            dragChange = false;
            dragItem.style.display = "none";
            dragItem.style.cursor = "hand";
            if (dragDstObject != null && dragDstObject != undefined)
            {
                dragDstObject.className = "title";
            }

            dragSrcObject = null;
            dragDstObject = null;

            $(document).unbind("mousemove", DoDrag);
        }
    }

    function DoDragEnd()
    {
        if (dragChange)
        {
            if (dragSrcObject != null && dragSrcObject != undefined && dragDstObject != null && dragDstObject != undefined)
            {
                var idxSrc = dragSrcObject.cellIndex;
                var idxDst = dragDstObject.cellIndex;

                var tr = document.getElementById(id + "_TrRightTitle");
                var tdSrc = dragSrcObject;
                var tdDst = dragDstObject;
                ChangeCell(tr, tdSrc, tdDst);

                if (dragToRight)
                {
                    rightColumnList.ChangeAfter(dragSrcObject.id, dragDstObject.id);
                }
                else
                {
                    rightColumnList.ChangeBefore(dragSrcObject.id, dragDstObject.id);
                }

                tr = document.getElementById(id + "_TrRightFilter");
                tdSrc = tr.cells[idxSrc];
                tdDst = tr.cells[idxDst];
                ChangeCell(tr, tdSrc, tdDst);

                var tb = document.getElementById(id + "_TbRightContent");
                for (var i = 0; i < tb.rows.length; i++)
                {
                    tr = tb.rows[i];
                    tdSrc = tr.cells[idxSrc];
                    tdDst = tr.cells[idxDst];
                    ChangeCell(tr, tdSrc, tdDst);
                }

                tr = document.getElementById(id + "_TrRightAggregation");
                tdSrc = tr.cells[idxSrc];
                tdDst = tr.cells[idxDst];
                ChangeCell(tr, tdSrc, tdDst);

                firstTitleCellX = 0;
                firstTitleCellY = 0;
            }

            dragChange = false;
            dragItem.style.display = "none";
            dragItem.style.cursor = "hand";
            if (dragDstObject != null && dragDstObject != undefined)
            {
                dragDstObject.className = "title";
            }

            dragSrcObject = null;
            dragDstObject = null;

            $(document).unbind("mousemove", DoDrag);
        }
    }

    function DoMouseDown()
    {
        if (window.event.button == 2) return;

        if (event.offsetX > event.srcElement.offsetWidth - 6 && event.offsetX < event.srcElement.offsetWidth)
        {
            dragResize = true;
            dragChange = false;
            dragSrcPos = window.event.x;
            dragObject = event.srcElement;
            dragObject.setCapture();
            dragObject.style.cursor = 'e-resize';
        }
        else
        {
            dragChange = true;
            dragResize = false;
            dragDstObject = null;
            dragSrcObject = window.event.srcElement;

            if (firstTitleCellX == 0 && firstTitleCellY == 0)
            {
                var idx = window.event.srcElement.cellIndex;
                var x = window.event.screenX - window.event.offsetX;
                var y = window.event.screenY - window.event.offsetY;
                var tw = 0;
                if (rightColumnList != undefined && rightColumnList.length > 0)
                {
                    for (var i = 0; i < idx; i++)
                    {
                        tw += $("#" + rightColumnList[i])[0].offsetWidth;
                    }
                }
                firstTitleCellX = x - tw;
                firstTitleCellY = y;
                var str = "[";
                if (rightColumnList != undefined && rightColumnList.length > 0)
                {
                    tw = firstTitleCellX;
                    for (var i = 0; i < rightColumnList.length; i++)
                    {
                        str += "[" + tw + "," + ($("#" + rightColumnList[i])[0].offsetWidth + tw) + "," + firstTitleCellY + "," + (firstTitleCellY + $("#" + rightColumnList[i])[0].offsetHeight) + "],";
                        tw += $("#" + rightColumnList[i])[0].offsetWidth;
                    }
                }
                str = str.substr(0, str.length - 1);
                str += "]";

                rightColumnListPosition = eval(str);
            }

            dragItem = document.getElementById(id + "_dragItem");
            dragItem.style.display = "";
            dragItem.innerText = dragSrcObject.innerText;
            dragItem.style.width = dragSrcObject.style.width;
            dragItem.style.left = window.event.clientX - dragItem.style.pixelWidth / 2;
            dragItem.style.top = window.event.clientY - dragItem.style.pixelHeight / 2;

            $(document).bind("mousemove", DoDrag);
        }
    }

    function DoMouseUp()
    {
        if (dragResize)
        {
            dragResize = false;
            dragObject.releaseCapture();
            dragObject.style.cursor = 'hand';
            dragObject = null;

            firstTitleCellX = 0;
            firstTitleCellY = 0;
        }
    }

    function AppendCell(trSrc, trDst, cellIndex)
    {
        var cnt = 0;
        for (var i = cellIndex; i >= 0; i--)
        {
            var cell = trSrc.cells[i];
            trSrc.removeChild(cell);
            if (cnt == 0)
            {
                trDst.appendChild(cell);
            }
            else
            {
                trDst.insertBefore(cell, trDst.cells[trDst.cells.length - cnt]);
            }
            cnt++;
        }
    }

    function RemoveCell(trSrc, trDst, cellIndex)
    {
        for (var i = cellIndex; i >= frzCnt; i--)
        {
            var cell = trSrc.cells[i];
            trSrc.removeChild(cell);
            if (trDst.cells.length > 0)
            {
                trDst.insertBefore(cell, trDst.cells[0]);
            }
            else
            {
                trDst.appendChild(cell);
            }
        }
    }

    function BindColumnEvent()
    {
        if (rightColumnList != undefined && rightColumnList.length > 0)
        {
            for (var i = 0; i < rightColumnList.length; i++)
            {
                $("#" + rightColumnList[i]).bind("contextmenu", ShowContextMenu);
            }
            $("#" + id + "_dragItem").mouseup(DoDragEnd).mouseout(DoDragCancle);
        }
    }

    function BindRightColumnEvent()
    {
        if (rightColumnList != undefined && rightColumnList.length > 0)
        {
            for (var i = 0; i < rightColumnList.length; i++)
            {
                $("#" + rightColumnList[i]).bind("mousedown", DoMouseDown).bind("mousemove", DoMouseMove).bind("mouseup", DoMouseUp);
            }
        }
    }

    function UnbindLeftColumnEvent()
    {
        if (leftColumnList != undefined && leftColumnList.length > 0)
        {
            for (var i = 0; i < leftColumnList.length; i++)
            {
                $("#" + leftColumnList[i]).unbind("mousedown", DoMouseDown).unbind("mousemove", DoMouseMove).unbind("mouseup", DoMouseUp);
            }
        }
    }

    function HideContextMenu()
    {
        $("#" + id + "_contextMemu").hide();
        contextObject = null;
        $(document).unbind("click", HideContextMenu);
    }
    
    //冻结
    function DoFreeze()
    {
        window.event.cancelBubble = true;
        if (!isFreeze)
        {
            if (contextObject != null)
            {
                idxCtm = contextObject.cellIndex;

                for (var i = 0; i <= idxCtm; i++)
                {
                    leftColumnList = leftColumnList.AppendAt(leftColumnList.length, rightColumnList[0]);
                    rightColumnList = rightColumnList.RemoveAt(0);
                }

                UnbindLeftColumnEvent();

                firstTitleCellX = 0;
                firstTitleCellY = 0;

                var trSrc, trDst;

                var trSrc = document.getElementById(id + "_TrRightTitle");
                var trDst = document.getElementById(id + "_TrLeftTitle");
                AppendCell(trSrc, trDst, idxCtm);

                trSrc = document.getElementById(id + "_TrRightFilter");
                trDst = document.getElementById(id + "_TrLeftFilter");
                AppendCell(trSrc, trDst, idxCtm);

                var tbSrc = document.getElementById(id + "_TbRightContent");
                var tbDst = document.getElementById(id + "_TbLeftContent");
                for (var i = 0; i < tbSrc.rows.length; i++)
                {
                    trSrc = tbSrc.rows[i];
                    trDst = tbDst.rows[i];
                    AppendCell(trSrc, trDst, idxCtm);
                }

                trSrc = document.getElementById(id + "_TrRightAggregation");
                trDst = document.getElementById(id + "_TrLeftAggregation");
                AppendCell(trSrc, trDst, idxCtm);

                var appendCells = "";
                for (var i = 0; i <= idxCtm; i++)
                {
                    appendCells += "<td class='scroll'>&nbsp;</td>";
                }
                document.getElementById()
                $("#" + id + "_TrLeftScroll").append(appendCells);

                var cols = ($("#" + id + "_TdLeftContent").attr("colspan") == null || $("#" + id + "_TdLeftContent").attr("colspan") == undefined) ? 1 : parseInt($("#" + id + "_TdLeftContent").attr("colspan"));
                cols = cols + idxCtm + 1;
                $("#" + id + "_TdLeftContent").attr("colspan", cols);

                AdjustGridWidth($("#" + id + "_TbRightMain").width() - $("#" + id + "_TbLeftMain").width() + leftWidth);

                isFreeze = true;
            }

            HideContextMenu();
        }
    }
    
    //取消冻结
    function DoCancel()
    {
        window.event.cancelBubble = true;
        if (isFreeze)
        {
            for (var i = idxCtm; i >= 0; i--)
            {
                rightColumnList = rightColumnList.AppendAt(0, leftColumnList[leftColumnList.length - 1]);
                leftColumnList = leftColumnList.RemoveAt(leftColumnList.length - 1);
            }

            BindRightColumnEvent();

            firstTitleCellX = 0;
            firstTitleCellY = 0;

            var trSrc, trDst;

            var trSrc = document.getElementById(id + "_TrLeftTitle");
            var trDst = document.getElementById(id + "_TrRightTitle");
            RemoveCell(trSrc, trDst, idxCtm + frzCnt);

            trSrc = document.getElementById(id + "_TrLeftFilter");
            trDst = document.getElementById(id + "_TrRightFilter");
            RemoveCell(trSrc, trDst, idxCtm + frzCnt);

            var tbSrc = document.getElementById(id + "_TbLeftContent");
            var tbDst = document.getElementById(id + "_TbRightContent");
            for (var i = 0; i < tbSrc.rows.length; i++)
            {
                trSrc = tbSrc.rows[i];
                trDst = tbDst.rows[i];
                RemoveCell(trSrc, trDst, idxCtm + frzCnt);
            }

            trSrc = document.getElementById(id + "_TrLeftAggregation");
            trDst = document.getElementById(id + "_TrRightAggregation");
            RemoveCell(trSrc, trDst, idxCtm + frzCnt);

            $("#" + id + "_TrLeftScroll").empty();
            var appendCells = "";
            for (var i = 0; i < frzCnt; i++)
            {
                appendCells += "<td class='scroll'>&nbsp;</td>";
            }
            $("#" + id + "_TrLeftScroll").append(appendCells);

            var cols = ($("#" + id + "_TdLeftContent").attr("colspan") == null || $("#" + id + "_TdLeftContent").attr("colspan") == undefined) ? 1 : parseInt($("#" + id + "_TdLeftContent").attr("colspan"));
            cols = cols - idxCtm - 1;
            $("#" + id + "_TdLeftContent").attr("colspan", cols);

            AdjustGridWidth(rightWidth);

            isFreeze = false;
        }

        HideContextMenu();
    }
    
    //显示筛选
    function DoShowFilter()
    {
        window.event.cancelBubble = true;
    }
    
    //隐藏筛选
    function DoHideFilter()
    {
        window.event.cancelBubble = true;
    }
    
    //显示聚合
    function DoShowAggregation()
    {
        window.event.cancelBubble = true;
    }
    
    //隐藏聚合
    function DoHideAggregation()
    {
        window.event.cancelBubble = true;
    }
    
    //显示右键菜单
    function ShowContextMenu()
    {
        contextObject = window.event.srcElement;
        window.event.cancelBubble = true;
        window.event.returnValue = false;

        $("#" + id + "_contextMemu").css("left", window.event.clientX);
        $("#" + id + "_contextMemu").css("top", window.event.clientY);
        $("#" + id + "_contextMemu").show();

        if (isFreeze)
        {
            $("#" + id + "_contextMemu_CFC").attr("class", "menuitemenable");
            $("#" + id + "_contextMemu_CFC").bind("click", DoCancel);
            $("#" + id + "_contextMemu_SFC").attr("class", "menuitemdisable");
            $("#" + id + "_contextMemu_SFC").unbind("click", DoFreeze);
        }
        else
        {
            $("#" + id + "_contextMemu_SFC").attr("class", "menuitemenable");
            $("#" + id + "_contextMemu_SFC").bind("click", DoFreeze);
            $("#" + id + "_contextMemu_CFC").attr("class", "menuitemdisable");
            $("#" + id + "_contextMemu_CFC").unbind("click", DoCancel);
        }

        if (showFilter)
        {
            $("#" + id + "_contextMemu_HFR").attr("class", "menuitemenable");
            $("#" + id + "_contextMemu_HFR").bind("click", DoHideFilter);
            $("#" + id + "_contextMemu_SFR").attr("class", "menuitemdisable");
            $("#" + id + "_contextMemu_SFR").unbind("click", DoShowFilter);
        }
        else
        {
            $("#" + id + "_contextMemu_SFR").attr("class", "menuitemenable");
            $("#" + id + "_contextMemu_SFR").bind("click", DoShowFilter);
            $("#" + id + "_contextMemu_HFR").attr("class", "menuitemdisable");
            $("#" + id + "_contextMemu_HFR").unbind("click", DoHideFilter);
        }

        if (showAggregation)
        {
            $("#" + id + "_contextMemu_HAR").attr("class", "menuitemenable");
            $("#" + id + "_contextMemu_HAR").bind("click", DoHideAggregation);
            $("#" + id + "_contextMemu_SAR").attr("class", "menuitemdisable");
            $("#" + id + "_contextMemu_SAR").unbind("click", DoShowAggregation);
        }
        else
        {
            $("#" + id + "_contextMemu_SAR").attr("class", "menuitemenable");
            $("#" + id + "_contextMemu_SAR").bind("click", DoShowAggregation);
            $("#" + id + "_contextMemu_HAR").attr("class", "menuitemdisable");
            $("#" + id + "_contextMemu_HAR").unbind("click", DoHideAggregation);
        }

        $("#" + id + "_contextMemu_EXE").attr("class", "menuitemenable");
        $("#" + id + "_contextMemu_EXP").attr("class", "menuitemenable");
        $("#" + id + "_contextMemu_PRP").attr("class", "menuitemenable");

        $("#" + id + "_contextMemu > div").hover(function()
        {
            if ($(this).attr("class").indexOf("enable") > 0)
            {
                $(this).attr("class", "menuitemselectedenable");
            }
            else
            {
                $(this).attr("class", "menuitemselecteddisable");
            }
        }, function()
        {
            if ($(this).attr("class").indexOf("enable") > 0)
            {
                $(this).attr("class", "menuitemenable");
            }
            else
            {
                $(this).attr("class", "menuitemdisable");
            }
        });

        $(document).bind("click", HideContextMenu);
    }

    BindColumnEvent();

    BindRightColumnEvent();

    function GetData() { }

    function GetSelectedData() { }

    function GetSelectedKey() { }

    function GetCurrentData() { }

    function GetCurrentKey() { }

    function GetValue(row, cell) { }

    function SetValue(row, cell, val) { }

    var selectedRow = null;
    var currentRow = null;
    
    //绑定行事件
    function BindRowEvent()
    {
        $("#" + id + "_TbLeftContent tr").click(OnRowClick).dblclick(OnRowDblClick);
        $("#" + id + "_TbRightContent tr").click(OnRowClick).dblclick(OnRowDblClick);
        if (allowMulitSelect)
        {
            $("#" + id + "_TbLeftContent tr td > input").click(OnChkClick);
            $("#" + id + "_TrLeftTitle td > input").click(OnChkAllClick);
        }
    }

    //行是否被选择
    function IsRowSelected(row)
    {
        if (allowMulitSelect)
        {
            return $("#" + id + "_TbLeftContent tr td > input")[row.rowIndex].checked;
        }
        else
        {
            return false;
        }
    }
    
    //是否当前行
    function IsRowCurrent(row)
    {
        return ($(row).attr("class").toLowerCase() == "tr_current");
    }
    
    //选择单行
    function SelectRow(i)
    {
        if (currentRow == null || currentRow.rowIndex != i)
        {
            document.getElementById(id + "_TbLeftContent").rows[i].className = "tr_selected";
            document.getElementById(id + "_TbRightContent").rows[i].className = "tr_selected";
            //                    $("#" + id + "_TbLeftContent tr:nth-child(" + [i + 1] + ")").attr("class", "tr_selected");
            //                    $("#" + id + "_TbRightContent tr:nth-child(" + [i + 1] + ")").attr("class", "tr_selected");
        }
        document.getElementById(id + "_TbLeftContent").rows[i].cells[0].childNodes[0].checked = true;
        //                $("#" + id + "_TbLeftContent tr td > input")[i].checked = true;
    }
    
    //取消行选择
    function UnSelectRow(i)
    {
        if (currentRow == null || currentRow.rowIndex != i)
        {
            $("#" + id + "_TbLeftContent tr:nth-child(" + [i + 1] + ")").attr("class", "tr");
            $("#" + id + "_TbRightContent tr:nth-child(" + [i + 1] + ")").attr("class", "tr");
        }
        $("#" + id + "_TbLeftContent tr td > input")[i].checked = false;
    }
    
    //选择行记
    function SelectRows(beginIdx, endIdx)
    {
        for (var i = beginIdx; i <= endIdx; i++)
        {
            SelectRow(i);
        }
    }
    
    //取消批量行选择
    function UnSelectRows(beginIdx, endIdx)
    {
        for (var i = beginIdx; i <= endIdx; i++)
        {
            UnSelectRow(i);
        }
    }
    
    //全选操作
    function SelectAllRows()
    {
        SelectRows(0, $("#" + id + "_TbLeftContent")[0].rows.length - 1);
    }
    
    //取消全部选择行
    function UnSelectAllRows()
    {
        UnSelectRows(0, $("#" + id + "_TbLeftContent")[0].rows.length - 1);
    }
    
    //设置当前行
    function SetRowCurrent(row)
    {
        if (currentRow != null && currentRow.rowIndex != row.rowIndex)
        {
            if (IsRowSelected(currentRow))
            {
                $("#" + id + "_TbLeftContent tr:nth-child(" + [currentRow.rowIndex + 1] + ")").attr("class", "tr_selected");
                $("#" + id + "_TbRightContent tr:nth-child(" + [currentRow.rowIndex + 1] + ")").attr("class", "tr_selected");
            }
            else
            {
                $("#" + id + "_TbLeftContent tr:nth-child(" + [currentRow.rowIndex + 1] + ")").attr("class", "tr");
                $("#" + id + "_TbRightContent tr:nth-child(" + [currentRow.rowIndex + 1] + ")").attr("class", "tr");
            }
        }
        $("#" + id + "_TbLeftContent tr:nth-child(" + [row.rowIndex + 1] + ")").attr("class", "tr_current");
        $("#" + id + "_TbRightContent tr:nth-child(" + [row.rowIndex + 1] + ")").attr("class", "tr_current");
        currentRow = row;
    }
    
    //行单击事件
    function OnRowClick()
    {
        var row = (window.event.srcElement.tagName.toLowerCase() == "td") ? window.event.srcElement.parentNode : window.event.srcElement;
        SetRowCurrent(row);
    }
    
    //行双击事件
    function OnRowDblClick()
    {
        var row = (window.event.srcElement.tagName.toLowerCase() == "td") ? window.event.srcElement.parentNode : window.event.srcElement;
        SetRowCurrent(row);
    }
    
    //行选择控件处理
    function OnChkClick()
    {
        window.event.cancelBubble = true;
        var chk = window.event.srcElement;
        var row = chk.parentNode.parentNode;
        if ((window.event.shiftKey || window.event.ctrlKey) && allowMulitSelect && selectedRow != null)
        {
            var selectedRowIdx = selectedRow.rowIndex;
            var rowIdx = row.rowIndex;
            if (selectedRowIdx < rowIdx) { chk.checked ? SelectRows(selectedRowIdx, rowIdx) : UnSelectRows(selectedRowIdx, rowIdx); }
            if (selectedRowIdx > rowIdx) { chk.checked ? SelectRows(rowIdx, selectedRowIdx) : UnSelectRows(rowIdx, selectedRowIdx); }
            if (selectedRowIdx == rowIdx) { chk.checked ? SelectRow(row.rowIndex) : UnSelectRow(row.rowIndex); }
        }
        else
        {
            selectedRow = row;
            chk.checked ? SelectRow(row.rowIndex) : UnSelectRow(row.rowIndex);
        }
    }
    
    //全选控件处理
    function OnChkAllClick()
    {
        window.event.cancelBubble = true;
        var chk = window.event.srcElement;
        chk.checked ? SelectAllRows() : UnSelectAllRows();
    }
    
    //绑定行事件
    BindRowEvent();

    //打开搜索窗口
    function ShowSearchDialog()
    {
        window.event.cancelBubble = true;
        var h = 125;
        var w = 200;
        var arg;
        for (var i = 0; i < 1000; i++)
        {
            arg += "abcdefghijklmnopqrstuvwxyz";
        }
        var featrues = "dialogTop:220;dialogLeft:220;dialogHeight:" + h + "px;dialogWidth:" + w + "px;help:0;status:0;resizable:1;";
        var filter = window.showModalDialog("search.htm", arg, featrues);
    }

    //绑定搜索事件
    function BindSearchEvent()
    {
        $("#" + id + "_ImgSearch").css("cursor", "hand");
        $("#" + id + "_ImgSearch").click(ShowSearchDialog);
    }

    BindSearchEvent();
}
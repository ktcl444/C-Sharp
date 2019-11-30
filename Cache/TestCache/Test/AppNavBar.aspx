<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AppNavBar.aspx.cs" Inherits="Map.Web.Test.AppNavBar" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table cellpadding="0" cellspacing="0" border="0" width="100%" height="100%">
            <colgroup>
                <col width="100" />
                <col width="7" />
                <col />
            </colgroup>
            <tr>
                <td style="BACKGROUND-COLOR:#c9c7ba"><!--左导航-->
                <table width="100%" height="100%" border="0" cellspacing="0" cellpadding="0">
					<tr height="15">
						<td>sdfsadfasdfas</td>
					</tr>
					<tr>
						<td align="center" valign="top">
							<div style="width:100%; height:100%; OVERFLOW: auto;">
								<div style="MARGIN-TOP:5px;margin-left:5px;">
									<table width="148" border="0" cellspacing="0" cellpadding="0" style="TABLE-LAYOUT:fixed;border:1px solid #a4a493;">
										<tr height="23">
											<td style="BACKGROUND-IMAGE:url(/AppNavBar/nav0_function_tab.gif);border-bottom:1px solid #a4a493;">
												<table border="0" cellspacing="0" cellpadding="0" style="FONT-SIZE:9pt;PADDING-TOP:3px;width:100%" onclick="showNav(this)" id="tblNav0">
													<tr style="cursor:hand">
														<td class="nav_title">企业知识门户(EKP)</td>
														<td width="20" align="10">
															<img src="/AppNavBar/nav_arrow_right.gif" id="imgBar" />
														</td>
													</tr>
												</table>         
											</td>
										</tr>
										<tr id="trBar" style="display:none">
											<td style="background-color:white">
												<table width="100%" border="0" cellspacing="0" cellpadding="0" style="FONT-SIZE:9pt">
													<col width="30" align="right" />
													<col width="5" align="center" />
													<col />
													<tr height="4">
														<td />
														<td />
														<td />
													</tr>
													<tr height="28">
														<td>
															<img src="/images/workflow/WorkFlow_16_white.gif" />
														</td>
														<td />
														<td style="CURSOR:hand" onmouseover="this.style.color=&quot;blue&quot;" onmouseout="this.style.color=&quot;&quot;" onclick="openFunc(this.innerText,this.Application,this.IsSelectCompany,this.SelectBULevel)" url="FuncNav.aspx" sys="工作协同" Application="1001" IsSelectCompany="True" SelectBULevel="0">工作协同</td>
													</tr>
													<tr height="28">
														<td>
															<img src="/images/SalesSystem_16_wihte.gif" />
														</td>
														<td />
														<td style="CURSOR:hand" onmouseover="this.style.color=&quot;blue&quot;" onmouseout="this.style.color=&quot;&quot;" onclick="openFunc(this.innerText,this.Application,this.IsSelectCompany,this.SelectBULevel)" url="FuncNav.aspx" sys="门户管理" Application="1090" IsSelectCompany="True" SelectBULevel="0">门户管理</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</div>
								<div style="MARGIN-TOP:5px;margin-left:5px;">
									<table width="148" border="0" cellspacing="0" cellpadding="0" style="TABLE-LAYOUT:fixed;border:1px solid #a4a493;">
										<tr height="23">
											<td style="BACKGROUND-IMAGE:url(/AppNavBar/nav0_function_tab.gif);border-bottom:1px solid #a4a493;">
												<table border="0" cellspacing="0" cellpadding="0" style="FONT-SIZE:9pt;PADDING-TOP:3px;width:100%" onclick="showNav(this)" id="tblNav1">
													<tr style="cursor:hand">
														<td class="nav_title">项目运营管理(POM)</td>
														<td width="20" align="10">
															<img src="/AppNavBar/nav_arrow_right.gif" id="imgBar" />
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr id="trBar" style="display:none">
											<td style="background-color:white">
												<table width="100%" border="0" cellspacing="0" cellpadding="0" style="FONT-SIZE:9pt">
													<col width="30" align="right" />
													<col width="5" align="center" />
													<col />
													<tr height="4">
														<td />
														<td />
														<td />
													</tr>
													<tr height="28">
														<td>
															<img src="/cbgl/images/CostControlcb_16_wihte.gif" />
														</td>
														<td />
														<td style="CURSOR:hand" onmouseover="this.style.color=&quot;blue&quot;" onmouseout="this.style.color=&quot;&quot;" onclick="openFunc(this.innerText,this.Application,this.IsSelectCompany,this.SelectBULevel)" url="FuncNav.aspx" sys="投资分析" Application="0203" IsSelectCompany="True" SelectBULevel="1">投资分析</td>
													</tr>
													<tr height="28">
														<td>
															<img src="/Xmjd/images/ItemManage_16_white.gif" />
														</td>
														<td />
														<td style="CURSOR:hand" onmouseover="this.style.color=&quot;blue&quot;" onmouseout="this.style.color=&quot;&quot;" onclick="openFunc(this.innerText,this.Application,this.IsSelectCompany,this.SelectBULevel)" url="FuncNav.aspx" sys="项目进度管理" Application="0202" IsSelectCompany="True" SelectBULevel="1">项目进度管理</td>
													</tr>
													<tr height="28">
														<td>
															<img src="cbgl/images/MaterialListcb_16_wihte.gif" />
														</td>
														<td />
														<td style="CURSOR:hand" onmouseover="this.style.color=&quot;blue&quot;" onmouseout="this.style.color=&quot;&quot;" onclick="openFunc(this.innerText,this.Application,this.IsSelectCompany,this.SelectBULevel)" url="FuncNav.aspx" sys="采购招投标管理" Application="0204" IsSelectCompany="True" SelectBULevel="2">采购招投标管理</td>
													</tr>
													<tr height="28">
														<td>
															<img src="/Cbgl/images/CostManagment_16_white.gif" />
														</td>
														<td />
														<td style="CURSOR:hand" onmouseover="this.style.color=&quot;blue&quot;" onmouseout="this.style.color=&quot;&quot;" onclick="openFunc(this.innerText,this.Application,this.IsSelectCompany,this.SelectBULevel)" url="FuncNav.aspx" sys="成本管理" Application="0201" IsSelectCompany="True" SelectBULevel="2">成本管理</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</div>
								<div style="MARGIN-TOP:5px;margin-left:5px;">
									<table width="148" border="0" cellspacing="0" cellpadding="0" style="TABLE-LAYOUT:fixed;border:1px solid #a4a493;">
										<tr height="23">
											<td style="BACKGROUND-IMAGE:url(/AppNavBar/nav0_function_tab.gif);border-bottom:1px solid #a4a493;">
												<table border="0" cellspacing="0" cellpadding="0" style="FONT-SIZE:9pt;PADDING-TOP:3px;width:100%" onclick="showNav(this)" id="tblNav2">
													<tr style="cursor:hand">
														<td class="nav_title">客户关系管理(CRM)</td>
														<td width="20" align="10">
															<img src="/AppNavBar/nav_arrow_right.gif" id="imgBar" />
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr id="trBar" style="display:none">
											<td style="background-color:white">
												<table width="100%" border="0" cellspacing="0" cellpadding="0" style="FONT-SIZE:9pt">
													<col width="30" align="right" />
													<col width="5" align="center" />
													<col />
													<tr height="4">
														<td />
														<td />
														<td />
													</tr>
													<tr height="28">
														<td>
															<img src="/images/SalesSystem_16_wihte.gif" />
														</td>
														<td />
														<td style="CURSOR:hand" onmouseover="this.style.color=&quot;blue&quot;" onmouseout="this.style.color=&quot;&quot;" onclick="openFunc(this.innerText,this.Application,this.IsSelectCompany,this.SelectBULevel)" url="FuncNav.aspx" sys="销售管理" Application="0101" IsSelectCompany="True" SelectBULevel="1">销售管理</td>
													</tr>
													<tr height="28">
														<td>
															<img src="/images/location_home.gif" />
														</td>
														<td />
														<td style="CURSOR:hand" onmouseover="this.style.color=&quot;blue&quot;" onmouseout="this.style.color=&quot;&quot;" onclick="openFunc(this.innerText,this.Application,this.IsSelectCompany,this.SelectBULevel)" url="FuncNav.aspx" sys="租赁管理" Application="0104" IsSelectCompany="True" SelectBULevel="1">租赁管理</td>
													</tr>
													<tr height="28">
														<td>
															<img src="/images/CustomerService_16_wihte.gif" />
														</td>
														<td />
														<td style="CURSOR:hand" onmouseover="this.style.color=&quot;blue&quot;" onmouseout="this.style.color=&quot;&quot;" onclick="openFunc(this.innerText,this.Application,this.IsSelectCompany,this.SelectBULevel)" url="FuncNav.aspx" sys="客户服务管理" Application="0102" IsSelectCompany="True" SelectBULevel="1">客户服务管理</td>
													</tr>
													<tr height="28">
														<td>
															<img src="/images/MemberSystem_16_wihte.gif" />
														</td>
														<td />
														<td style="CURSOR:hand" onmouseover="this.style.color=&quot;blue&quot;" onmouseout="this.style.color=&quot;&quot;" onclick="openFunc(this.innerText,this.Application,this.IsSelectCompany,this.SelectBULevel)" url="FuncNav.aspx" sys="会员管理" Application="0103" IsSelectCompany="True" SelectBULevel="1">会员管理</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</div>
								<div style="MARGIN-TOP:5px;margin-left:5px;">
									<table width="148" border="0" cellspacing="0" cellpadding="0" style="TABLE-LAYOUT:fixed;border:1px solid #a4a493;">
										<tr height="23">
											<td style="BACKGROUND-IMAGE:url(/AppNavBar/nav0_function_tab.gif);border-bottom:1px solid #a4a493;">
												<table border="0" cellspacing="0" cellpadding="0" style="FONT-SIZE:9pt;PADDING-TOP:3px;width:100%" onclick="showNav(this)" id="tblNav3">
													<tr style="cursor:hand">
														<td class="nav_title">决策分析</td>
														<td width="20" align="10">
															<img src="/AppNavBar/nav_arrow_right.gif" id="imgBar" />
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr id="trBar" style="display:none">
											<td style="background-color:white">
												<table width="100%" border="0" cellspacing="0" cellpadding="0" style="FONT-SIZE:9pt">
													<col width="30" align="right" />
													<col width="5" align="center" />
													<col />
													<tr height="4">
														<td />
														<td />
														<td />
													</tr>
													<tr height="28">
														<td>
															<img src="/images/ManageDrvying_16_wihte.gif" />
														</td>
														<td />
														<td style="CURSOR:hand" onmouseover="this.style.color=&quot;blue&quot;" onmouseout="this.style.color=&quot;&quot;" onclick="openFunc(this.innerText,this.Application,this.IsSelectCompany,this.SelectBULevel)" url="FuncNav.aspx" sys="管理驾驶舱" Application="0301" IsSelectCompany="True" SelectBULevel="0">管理驾驶舱</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</div>
								<div style="MARGIN-TOP:5px;margin-left:5px;">
									<table width="148" border="0" cellspacing="0" cellpadding="0" style="TABLE-LAYOUT:fixed;border:1px solid #a4a493;">
										<tr height="23">
											<td style="BACKGROUND-IMAGE:url(/AppNavBar/nav0_function_tab.gif);border-bottom:1px solid #a4a493;">
												<table border="0" cellspacing="0" cellpadding="0" style="FONT-SIZE:9pt;PADDING-TOP:3px;width:100%" onclick="showNav(this)" id="tblNav4">
													<tr style="cursor:hand">
														<td class="nav_title">系统管理</td>
														<td width="20" align="10">
															<img src="/AppNavBar/nav_arrow_right.gif" id="imgBar" />
														</td>
													</tr>
												</table>
											</td>
										</tr>
										<tr id="trBar" style="display:none">
											<td style="background-color:white">
												<table width="100%" border="0" cellspacing="0" cellpadding="0" style="FONT-SIZE:9pt">
													<col width="30" align="right" />
													<col width="5" align="center" />
													<col />
													<tr height="4">
														<td />
														<td />
														<td />
													</tr>
													<tr height="28">
														<td>
															<img src="/images/SyestemPara_16_white.gif" />
														</td>
														<td />
														<td style="CURSOR:hand" onmouseover="this.style.color=&quot;blue&quot;" onmouseout="this.style.color=&quot;&quot;" onclick="openFunc(this.innerText,this.Application,this.IsSelectCompany,this.SelectBULevel)" url="FuncNav.aspx" sys="系统设置" Application="0000" IsSelectCompany="False" SelectBULevel="0">系统设置</td>
													</tr>
												</table>
											</td>
										</tr>
									</table>
								</div>
							</div>
						</td>
					</tr>
				</table>
                </td>
                <td valign="middle" style="BACKGROUND-COLOR:#c9c7ba">
					<img id="imgSwitch" src="/AppNavBar/nav0_hidealter_arrow.gif" style="CURSOR:hand" onclick="clickSwitch()" onmouseout="mouseOver()" onmousemove="mouseOut()">
				</td>
                <td><!--f内容区域-->
                fff
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>

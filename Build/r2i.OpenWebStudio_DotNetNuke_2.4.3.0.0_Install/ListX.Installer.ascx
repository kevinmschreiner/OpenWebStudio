<%@ Control Language="vb" AutoEventWireup="false" Codebehind="Installer.ascx.vb" Inherits="Bi4ce.Modules.xList.Installer" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<hr/>
<center><a class=NormalRed href='<%#me.ResolveUrl("../OWS/IM.aspx")%>?_OWS_=<%#r2i.OWS.UI.OpenControlBase.qAction%>:UPGRADE,<%#r2i.OWS.UI.OpenControlBase.qUpgradeModule%>:<%#Me.ModuleId%>,<%#r2i.OWS.UI.OpenControlBase.qUpgradePage%>:<%#Me.PortalSettings.ActiveTab.TabID%>' onclick="return window.confirm('Are you sure?');">Upgrade to Open Web Studio</a></center>
<hr/>
<table width="100%" border="0" cellpadding="0" cellspacing="0">
	<TR>
		<TD colspan="2">ListX Package Installation<BR>
			<SPAN class="normal"><IMG 
src='<%# ModuleImageURL("biglp.gif")%>' align=left>Providing a quick, painless, and easy way of 
				setting up a ListX instance, whether starting from an existing ListX package, 
				or just setting up from an existing configuration, everything is made possible 
				through our Installation utility. Select the ListX Package (LP)&nbsp;or XML 
				file you wish to use for this configuration from a local resource, and press 
				the Install button. Once installed, you may administer ListX throught the 
				standard View Options interface.</SPAN>
		</TD>
	</TR>
	<tr>
		<td><span style="WIDTH: 100px">File:</span></td>
		<td style="WIDTH: 100%"><input type="file" style="WIDTH: 100%" runat="server" id="flUpload"></td>
	</tr>
	<tr>
		<td colspan="2" align="center"><asp:LinkButton ID="lnkUpload" Runat="server"> Install Package</asp:LinkButton>
			&nbsp;|&nbsp;<asp:LinkButton id="lnkBuildPackage" runat="server">Create Package</asp:LinkButton></td>
	</tr>
	<tr>
		<td colspan="2"><asp:CheckBoxList id="lstTabs" runat="server" rows="22" datatextfield="TabName" datavaluefield="TabId"
				cssclass="NormalTextBox" width="400px" Visible="False"></asp:CheckBoxList><br>
		</td>
	</tr>
	<tr>
		<td colspan="2" align="center">
			<asp:LinkButton id="lnkPackager" runat="server" Visible="False">Build Package</asp:LinkButton></td>
	</tr>
	<tr>
		<td colspan="2"><asp:Literal ID="ltlResults" Runat="server"></asp:Literal></td>
	</tr>
</table>
<script language=javascript>try {lxInit_RichText(false);} catch(ex) { /*do nothing*/ }</script>

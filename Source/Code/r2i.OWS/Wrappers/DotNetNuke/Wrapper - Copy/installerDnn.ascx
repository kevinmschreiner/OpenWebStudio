<%@ Control Language="vb" AutoEventWireup="false" Codebehind="installerDnn.ascx.vb" Inherits="r2i.OWS.Wrapper.DNN.installerDnn" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register Assembly="r2i.OWS.Wrapper.DotNetNuke" Namespace="r2i.OWS.Wrapper.DNN" TagPrefix="cc1" %>
<asp:Panel ID="pnlOWS" runat="server" Width="100%">
OpenWebStudio utilizes preconfigured settings, known simply as <b>Configurations</b> which provide the full functionality of each interface.<br />
For a standard module two possible configurations can be executed: One for the default View, the other for the Module Settings. <br />
Select the runtime of your choice from the options provided below.
<table width="100%">
	<tr><td class="SubHead" width="20%">View</td><td>
        <asp:DropDownList ID="ddlConfigLst" runat="server" Width="80%" visible="<%# DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.IsSuperUser %>"></asp:DropDownList>&nbsp; 
        <asp:Label ID="lblEditLink" runat="server" Visible="False"></asp:Label>
        <asp:Label ID="lblNoConfig" runat="server" Visible="False"></asp:Label></td></tr>
	<tr><td class="SubHead" width="20%">Settings</td><td>
        <asp:DropDownList ID="ddlConfigLstSettings" runat="server" Width="80%" visible="<%# DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo.IsSuperUser %>"></asp:DropDownList>&nbsp; 
        <asp:Label ID="lblEditLinkSettings" runat="server" Visible="False"></asp:Label>
        <asp:Label ID="lblNoConfigSettings" runat="server" Visible="False"></asp:Label></td></tr>        
    <tr><td class="SubHead" width="20%">Options</td><td>
        <asp:CheckBox id="chkInvisible" runat="server" Text="Hide All OpenWebStudio interfaces" /><br /><i>To re-enable the Configuration Options, you must manually remove the OpenWebStudio.Visible module setting.</i>
        </td></tr>             
</table>
<script type="text/javascript">
function openconfig(basepath,id) {
    var dd = document.getElementById(id);
    var url =  basepath + "#config/" + dd[dd.selectedIndex].value;
    window.open(url);
}
</script>
</asp:Panel>

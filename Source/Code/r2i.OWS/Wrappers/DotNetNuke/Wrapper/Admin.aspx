<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Admin.aspx.vb" Inherits="r2i.OWS.Wrapper.DNN.Admin" %>


<html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <%=PageTitle(True)%>
        <%=CodeMirrorCss()%>
        <%=CSSLibrary(True)%>
        <%=JavascriptLibrary(True)%>
    </head>
    <body>
		<input id="KeyCatch" name="KeyCatch" style="height:1px;width:1px;float:left;position:absolute;left:-1;top:0;background:white;border:0px solid white;" />
		<form name="form1" id="form1" method="post" action="admin.aspx">
        <map id="" name="move">
            <area alt="Move Up" id="area1" coords="20,0,55,20" href="javascript:ActionMove('UP');" />
            <area alt="Outdent" id="area2" coords="0,21,21,52" href="javascript:ActionMove('LEFT');" />
            <area alt="" id="area3" coords="24,23,52,50" href="javascript:ActionMove('');" />
            <area alt="Indent" id="area4" coords="54,21,75,52" href="javascript:ActionMove('RIGHT');" />	
            <area alt="Move Down" id="area5" coords="20,54,55,75" href="javascript:ActionMove('DOWN')" />
        </map>

        <div class="HRow"><div><div><img alt="" src="images/spacer.gif" width="1" height="6" /></div></div></div>
        <table width="100%" cellpadding="0" cellspacing="0" border="0" class="HFile">
	        <tr>
		        <td style="vertical-align: top;width: 32px;background: url(images/mnuback.gif) bottom left;" rowspan="2">
			        <img alt="" src="images/mnubock.gif" border="0" />
		        </td>
		        <td width="50" nowrap="nowrap" class="HFileA" align="left" valign="center" style="padding: 4px 0px 0px 4px; width: 75px; vertical-align: middle;background: url(images/mnuback2.gif) top left;">
			        <img alt="Open" src="images/open.gif" onclick="onClick_Conf_Open(-1)" />
			        <img alt="New" src="images/item.gif" onclick="onClick_Conf_New(-1)" />				
			        <img alt="Save" src="images/saved.gif" onclick="onClick_Conf_Save(-1)"/>
			        <img alt="Publish" src="images/publish.gif" onclick="onClick_Conf_Publish(-1)"/>
		        </td>
		        <td class="HFileA" valign="center" style="width:32px; vertical-align: middle;background: url(images/mnuback9.gif) repeat bottom left; background-position:1px 0px;">
			        <img alt="" src="images/mnuback4.gif" valign="bottom"/>
			    </td>
		        <td width="100%" valign="middle" align="right" style="vertical-align: center;background: url(images/mnuback9.gif) repeat bottom left; background-position:1px 0px;">
			        <span id="HConfigName"></span><span id="HAppName">Open Web Studio <%=Version()%></span>
		        </td>		
	        </tr>
	        <tr class="HFileB">
		        <td colspan="3" valign="bottom">
			        <div id="HMenu"></div>
		        </td>
	        </tr>
        </table>
        <div class="HStrip" id="HStrip"></div>
        <div id="HFilter" style="display: block;"></div>
        <div id="HMain">
            <div id="HListTitle" class="HTitle"></div>
            <div id="HList" style="display: block;  overflow: auto;">
                <div id="HListHeader">
                </div>
                <div id="HListContainer">
                    <div id="HContent" onmouseover="disableSelection();" onmouseout="enableSelection();">
                    </div>
                </div>
            </div>
        </div>
        <div id="HPropertySizer">
            <div id="HSplitter">
                <br />
            </div>
            <div id="HSplitter2">
                <div class="FRow"><div><div><img alt="" src="images/spacer.gif" width="1" height="6" /></div></div></div>
                <div class="Splitter"><img alt="" src="images/splitter.gif" /></div>
            </div>
        </div>
        <div id="HProperty">
            <a name="Property"></a>
            <div class="HRow"><div><div><img alt="" src="images/spacer.gif" width="1" height="6" /></div></div></div> 
            <div id="HPropertyDetail" style="display: block;"></div>
            <div class="FRow"><div><div><img alt="" src="images/spacer.gif" width="1" height="6" /></div></div></div>
        </div>
		</form>
        <textarea cols="0" rows="0" style="display: none;" id="newConfiguration">
        {"Name":"New Configuration","recordsPerPage":0,"enableAlphaFilter":false,"enablePageSelection":false,"enableRecordsPerPage":false,"enableCustomPaging":false,"enableExcelExport":false,"enableHide_OnNoQuery":false,"enableHide_OnNoResults":false,"enableAdvancedParsing":true,"enableCompoundIIFConditions":true,"enableQueryDebug":false,"enableQueryDebug_Edit":false,"enableQueryDebug_Admin":false,"enableQueryDebug_Super":false,"enableQueryDebug_Log":false,"enableQueryDebug_ErrorLog":false,"autoRefreshInterval":"","skipRedirectActions":false,"skipSubqueryDebugging":false,"enableAdmin_Edit":true,"enableAdmin_Admin":false,"enableAdmin_Super":false,"enableAJAX":false,"enableAJAXCustomPaging":false,"enableAJAXCustomStatus":false,"enableAJAXManual":false,"includeJavascriptUtilities":false,"includeJavascriptValidation":false,"javascriptOnComplete":"","enableMultipleColumnSorting":false,"ModuleCommunicationMessageType":"","showAll":true,"useExplicitSystemVariables":false,"enabledForcedQuerySplit":false,"searchItems":[],"queryItems":[],"listItems":[],"messageItems":[{"Index":1,"Level":0,"Parameters":{"Name":"OnLoad","RenderType":"0"},"ActionType":"Action-Region","ChildActions":[{"Index":2,"Level":1,"Parameters":{"Value":"Place all general operations within this region as a starting point. This includes Template and Variable assignments."},"ActionType":"Action-Comment","ChildActions":[]},{"Index":3,"Level":0,"Parameters":{"Name":"Query Variables","RenderType":"0"},"ActionType":"Action-Region","ChildActions":[{"Index":4,"Level":0,"Parameters":{"VariableType":"<Action>","QuerySource":"Source Value","QueryTarget":"New Query Variable","QueryTargetLeft":"","QueryTargetRight":"","QueryTargetEmpty":"","EscapeListX":"0","Protected":"true","EscapeHTML":"true"},"ActionType":"Template-Variable","ChildActions":[]},{"Index":5,"Level":0,"Parameters":{"VariableType":"<Action>","QuerySource":"Source Value","QueryTarget":"New Query Variable","QueryTargetLeft":"","QueryTargetRight":"","QueryTargetEmpty":"","EscapeListX":"0","Protected":"true","EscapeHTML":"true"},"ActionType":"Template-Variable","ChildActions":[]},{"Index":6,"Level":0,"Parameters":{"VariableType":"<Action>","QuerySource":"Source Value","QueryTarget":"New Query Variable","QueryTargetLeft":"","QueryTargetRight":"","QueryTargetEmpty":"","EscapeListX":"0","Protected":"true","EscapeHTML":"true"},"ActionType":"Template-Variable","ChildActions":[]}]}]},{"Index":7,"Level":0,"Parameters":{"Name":"OnRender","RenderType":"0"},"ActionType":"Action-Region","ChildActions":[{"Index":8,"Level":1,"Parameters":{"Value":"Default Region OnRender, used for the purpose of any runtime which will change the course of the outcome of the module, or handling of the general interaction."},"ActionType":"Action-Comment","ChildActions":[]},{"Index":9,"Level":0,"Parameters":{"Type":"Query-Query","GroupStatement":"","GroupIndex":"","Value":"New Template"},"ActionType":"Template","ChildActions":[{"Index":10,"Level":0,"Parameters":{"Type":"Group-Header","GroupStatement":"","GroupIndex":"0","Value":"Group Header"},"ActionType":"Template","ChildActions":[{"Index":11,"Level":0,"Parameters":{"Type":"Detail-Detail","GroupStatement":"","GroupIndex":"","Value":"General Detail"},"ActionType":"Template","ChildActions":[]},{"Index":12,"Level":0,"Parameters":{"Type":"Detail-Alternate","GroupStatement":"","GroupIndex":"","Value":"Alternate Detail"},"ActionType":"Template","ChildActions":[]}]},{"Index":13,"Level":0,"Parameters":{"Type":"Group-Footer","GroupStatement":"","GroupIndex":"0","Value":"Group Footer"},"ActionType":"Template","ChildActions":[]}]},{"Index":14,"Level":0,"Parameters":{"Type":"Detail-NoResults","GroupStatement":"","GroupIndex":"","Value":"Standard No Results"},"ActionType":"Template","ChildActions":[]},{"Index":15,"Level":0,"Parameters":{"Type":"Detail-NoQuery","GroupStatement":"","GroupIndex":"","Value":"Standard No Query"},"ActionType":"Template","ChildActions":[]}]}],"query":"","filter":"","customConnection":"","listItem":"","listAItem":"","defaultItem":"","noqueryItem":"","SearchQuery":"","SearchTitle":"","SearchLink":"","SearchAuthor":"","SearchDate":"","SearchKey":"","SearchContent":"","SearchDescription":"","Version":20}
        </textarea>

        <%=CodeMirrorConfig()%>
        <%=JavascriptLibrary(False)%>
    </body>
</html>
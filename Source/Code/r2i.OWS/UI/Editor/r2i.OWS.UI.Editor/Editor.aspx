<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Editor.aspx.cs" Inherits="r2i.OWS.UI.Editor.Editor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
	<head>
		<link rel="stylesheet" href="_js/jquery/themes/base/jquery.ui.all.css"></link>
		
		<script type="text/javascript" src="_js/jquery/jquery-1.4.4.js"></script>
		<script type="text/javascript" src="http://ajax.microsoft.com/ajax/jquery.templates/beta1/jquery.tmpl.min.js"></script>

		<script type="text/javascript" src="_js/jquery/ui/jquery.ui-latest.js"></script>
		<script type="text/javascript" src="_js/jquery/ui/jquery.layout-latest.js"></script>
		<script type="text/javascript" src="_js/jquery/ui/jquery.ui.button.js"></script>
		<script type="text/javascript" src="_js/jquery/ui/jquery.ui.slider.js"></script> 
		<script type="text/javascript" src="_js/OWS.jQuery.Extended.Editor.js"></script>
		
		<script type="text/javascript" src="_js/jquery/jquery.cookie.js"></script>
		<script type="text/javascript" src="_js/jquery/jquery.hotkeys.js"></script>		
		<script type="text/javascript" src="_js/jquery/jquery.jstree.js"></script>
		
		<script type="text/javascript"  src="_js/jquery/jquery.themeswitcher.min.js"></script>

		<script type="text/javascript" src="_js/OWS.js?v=1"></script>
		<script type="text/javascript" src="_js/OWS.Services.js?v=1"></script>	

		<!--Docking Manager-->
		<link rel="stylesheet" href="_css/OWS.Ide.css?v=1"></link>		
		<script type="text/javascript" src="_js/OWS.Ide.js?v=4"></script>
		<!--/Docking Manager-->
	</head>
	<body class="ui-widget-content" style="border:0px;" >
		<div id="dlgError" style="display:none;" title="" default="Looks like we've got a problem!"><p></p></div>
		<script type="text/html" id="tmp_ideActions_" onbind="$(that).accordion({event:'hoverintent click'});">
			<h3><a href="#" action="${Code}" onclick="ows.admin.configurations.active().add($(this).attr('action'));return false;">${Name}</a></h3>
			<div><p>${Description}</p></div>
		</script>
		<!-- #include file="_templates/ide.actions.html" -->
		<!-- #include file="_templates/ide.community.chat.html" -->
		<!-- Actions -->
		<!-- /Actions -->
		<div id="ideMain" class="section" >
			<!--Headline-->
				<div id="ideHeadline" class=""></div>
			<!--/Headline-->	
			<!--Center-->
				<div id="ideCenter" class="clear section">
					<!--Top-->
						<div id="ideTop"  class="clear section">
							<div id="ideTopLeft" class="pane" ></div>
							<div id="ideTopRight" class="pane">
							</div>
						</div>
					<!--/Top-->
					<!--Bottom-->
						<div id="ideBottom"  class="clear section">
							<div id="ideBottomLeft" class="pane" >
								<div id="ide_plugins">
									<h3><a href="#">Tokens</a></h3>
									<div id="ide_tokens" class="toolitems"></div>
									<h3><a href="#">Formats</a></h3>
									<div id="ide_formats" class="toolitems"></div>		
									<h3><a href="#">Queries</a></h3>
									<div id="ide_queries" class="toolitems"></div>										
									<h3><a href="#">Html</a></h3>
									<div id="ide_html" class="toolitems"></div>										
								</div>
							</div>
							<div id="ideBottomRight" class="pane">
							</div>
						</div>
					<!--/Bottom-->
				</div>
			<!--/Center-->
			<!--Community-->
			<div id="ideCommunity" class="pane"></div>
			<!--/Community-->
			<!--Footer-->
				<div id="ideFooter"  class="ui-widget-content clear section"></div>
			<!--/Footer-->
		</div>
		<!-- #include file="_templates/ide.about.html" -->	
        <script type="text/javascript">
            function register_ide_callback(value) {
                if (typeof ide == 'undefined') {
                    var cburl = value;
                    window.setTimeout(function () { register_ide_callback(cburl); }, 13);
                } else {
                    ide.callback_url = value;
                }
            }
            register_ide_callback('<%=this.Request.RawUrl.ToLower().Replace("editor.aspx", "")%>');
        </script>
		<script type="text/javascript" src="_js/OWS.Admin.js"></script>				
	</body>
</html>
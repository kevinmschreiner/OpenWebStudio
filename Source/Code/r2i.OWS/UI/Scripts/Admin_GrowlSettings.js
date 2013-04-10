( function($) {
$.growl.settings.displayTimeout = 4000;
$.growl.settings.dockCss.width = '225px';
$.growl.settings.defaultImage = 'Images/forum.gif';
$.growl.settings.noticeTemplate = ''
+ '<div class="growlbody">'
+ '	<div class="header">'
+ '			    <img src="%image%" />'
+ '			    %title%'
+ '	</div>'
+ '	<div class="message">'
+ '			%message%'
+ '	</div>'
+ '	<div class="footer">'
+ '			<a href="" onclick="return false;" rel="close">Close</a>'
+ '	</div>'
+ '</div>';
$.growl.settings.noticeCss = { position: 'relative' };
})(jQuery);
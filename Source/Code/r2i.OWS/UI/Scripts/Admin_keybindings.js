jQuery(function($)
{
	var pressed = {};
	$(document).keydown(function(event)
	{
		var keychar;var numcheck;var shiftCheck;var ctrlCheck;var altCheck;
		shiftCheck = event.shiftKey;
		ctrlCheck = event.ctrlKey;
		altCheck = event.altCheck;
		var keyCode = event.keyCode;
		if (!(keyCode==null))
		{
			switch(keyCode)
			{
				case 83: //Save
			        if (ctrlCheck) {
				        if ( sysProperty_ObjectItem && sysProperty_ListItem ) {
					        SaveProperty(true);
				        } else {
					        saveConfiguration(true);
				        }
				        event.preventDefault(); // prevent the default save dialog
			        }				
				    break;
				case 78: //New
			        if (ctrlCheck) {
				        setTimeout(function() {
				          if ( confirm('Are you sure you want to close this configuration open a new configuration?') ) {
				            onClick_Conf_New(-1);
				          }
				        },20);
				        event.preventDefault(); // prevent the default save dialog
			        }
			        break;
			    case 79: //Open
			        if (ctrlCheck) {
				        setTimeout(function() {
				          if ( confirm('Are you sure you want to close this configuration and open a different one?') ) {
					        onClick_Conf_Open(-1);
				          }
				        },20);
				        event.preventDefault(); // prevent the default save dialog
			        }
			        break;
			    case 27: //ESCAPE
			    	if ( sysProperty_ObjectItem && sysProperty_ListItem ) {
				        setTimeout(function() {
				          if ( confirm('Dismiss changes to this action?') ) {
					        SaveProperty(false);
				          }
				        },20);
			        }
			        break;
	        }
		}
	});
});

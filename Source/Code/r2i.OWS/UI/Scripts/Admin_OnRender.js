RibbonMain.Initialize($('HStrip'),$('HMenu'));
var owsSelect;

//CLIPBOARD EVENTS
function onClick_Clip_Copy(o)
{
	if (selectedActionObj!=undefined&&selectedActionObj!=null)
	{
		//clipboard_Action=CopyAction(selectedActionObj);
		showClipboard(ows.JSON.stringify(CopyAction(selectedActionObj)),false);
	}
}
function onClick_Clip_Paste(o)
{
	//if (clipboard_Action!=undefined && clipboard_Action!=null)
	//	ActionMove('PASTE');
	showClipboard('',true);
}
var ClipboardVisible = false;
function showClipboard(displayValue,isPaste)
{
    if (!isPaste||!ClipboardVisible)
    {
        var info = '';
	    var buttons = '<button onclick="onClick_Clipboard_Close();return false;">Close</button>';
	    if (isPaste)
	    {
		    info = 'Paste the action JSON block into the textarea provided and click "Paste".';
		    buttons = '<button onclick="onClick_Clipboard_Paste();return false;">Paste</button>' + buttons;
	    }
	    else
		    info = 'Copy the action JSON block provided by selecting and pressing Control+C. Click "Close" when complete.';
	    HFilter.innerHTML = '<div class="HTitle"><center><table border="0" cellpadding="2" style="width:100%;" cellspacing="2"><tr>' +
						    '<td class="HStrip" style="width:200px;"><b>Clipboard</b>&nbsp;The clipboard allows you to copy and paste actions and their child action structures from within the same configuration or one configuration to another. '+info+'</td>'+
						    '<td class="HStrip"><textarea name="frmClipboard_Content" id="frmClipboard_Content" style="width:100%; height:100px;"></textarea></td>'+
						    '<td class="HStrip" style="width:80px;">'+buttons+'</td>'+
						    '</tr></table></center></div>';
	    showBlock(HFilter);	
	    ClipboardVisible = true;
	    sysSetText($('frmClipboard_Content'),displayValue);
	}
	else
	{
	    onClick_Clipboard_Paste();
	}
}
function onClick_Clipboard_Paste()
{
	var displayValue = sysGetText($('frmClipboard_Content'));
	if (displayValue.length>0)
	{
		try
		{
			eval('clipboard_Action = '+displayValue);
			if (clipboard_Action!=undefined && clipboard_Action!=null)
				ActionMove('PASTE');
		}
		catch(ex)
		{
			alert('The operation failed due to a javascript error, please verify that your syntax is correct.');
		}
	}
}
function onClick_Clipboard_Close()
{
        ClipboardVisible = false;
		HFilter.innerHTML = '';
		hideBlock(HFilter);
}

//CONFIGURATION EVENTS
function onClick_Conf_Save(o)
{
saveConfiguration(false);
}
function onClick_Conf_Cancel(o)
{
}
function onClick_Conf_Import(o)
{
importConfiguration();
}
function onClick_Conf_Export(o)
{
exportConfiguration();
}

function onClick_Conf_Publish(o)
{
saveConfiguration(true);
}

function onClick_Conf_Save(o)
{
// false means save as draft
saveConfiguration(false);
}
function onClick_Conf_New(o)
{
// change the configid to a new one
loadConfiguration(document.getElementById('newConfiguration').value);
}
function onClick_Conf_Open(o)
{
LoadProperty('Open');
}
function onClick_Conf_Filter(o)
{
/* mod r.metlinskiy */
HFilter.innerHTML = '<div class="HTitle" id="ConfigSearch"><center><table border="0" cellpadding="2" cellspacing="2"><tr>' +
					'<td class="HStrip">Name/Guid:&nbsp;<input id="frmSearchBy" name="frmSearchBy" type="text" style="width:200px" autocomplete="off" /></td>'+
					'<td class="HStrip">&nbsp;<b>Date</b>:&nbsp;From&nbsp;<input id="frmSearchByDateFrom" name="frmSearchByDateFrom" type="text" style="width:80px" title="MM/dd/yyyy" /></td>'+
					'<td class="HStrip">To&nbsp;<input id="frmSearchByDateTo" name="frmSearchByDateTo" type="text" style="width:80px" title="MM/dd/yyyy" /></td>'+
					'<td class="HStrip">&nbsp;Created&nbsp;<input id="frmSearchByDate" name="frmSearchByDate" type="radio" value="Created" checked="checked"  /></td>'+
					'<td class="HStrip">Update&nbsp;<input id="frmSearchByDate" name="frmSearchByDate" type="radio" value="LastUpdate" /></td>'+
					'<td class="HStrip">&nbsp;<input type="button" onclick="onClick_Conf_Search(-1);" value="Search" /></td>'+
					'<td class="HStrip"><input type="button" onclick="onClick_Conf_SearchCancel(-1);" value="Cancel" /></td>'+
					'</tr></table></center></div>';
showBlock(HFilter);

$jq("#frmSearchBy")
.keydown(function(event){
	var keyCode = event.keyCode;
	if (!(keyCode==null))
	{
		switch(keyCode)
		{
			case 13:
				onClick_Conf_Search(-1);
				return false;
				break;
		}
	}
})
.val(lxGetCookie("SearchBy"))
.focus();
LoadProperty('Open');
}
function onClick_Conf_SearchCancel(o)
{
lxSetCookie("SearchBy",'');
$jq("#ConfigSearch").remove();
ows.Fetch('ConfigurationList',-1,'');
return ows.History('ConfigurationList',-1);
//LoadProperty('Open');
}
function onClick_Conf_Search(o)
{
LoadProperty('Open');
lxSetCookie("SearchBy",$jq("#frmSearchBy").val());
//LoadProperty('Open');
ows.Fetch('ConfigurationList',-1,'');
ows.History('ConfigurationList',-1);
return false;
}
//SEARCH EVENTS
function onClick_Find_Find(o)
{
}
function onClick_Find_Replace(o)
{
}
//SECURITY EVENTS
function onClick_Lock_Lock(o)
{
}
function onClick_Lock_Unlock(o)
{
}
function onClick_Lock_Encrypt(o)
{
}
function onClick_Lock_Decrypt(o)
{
}
//EVENT EVENTS
function onClick_Event_Enable(o)
{
}
function onClick_Event_Disable(o)
{
}
function onClick_Event_View(o)
{
}
function onClick_Event_Clear(o)
{
}
function onClick_Event_Import(o)
{
}
function onClick_Event_Export(o)
{
}



Initialize();

if (window.location.hash.substring(0,8) == '#config/')
{
  configurationId = window.location.hash.substring(8,window.location.hash.length);
  getJsonConfig(configurationId);
}

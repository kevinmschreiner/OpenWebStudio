//ADD OUR ACTION INTO THE RIBBON
RibbonMain.configuration.Menu[3].Groups.push('Events');
RibbonMain.configuration.Groups.Events = {
				"Name":"Events",
				"Abbr":"event",
				"onClick":"",
				"Tabs": 
					[
						{
							"Image":"images/event.gif",
							"ImageMap":"",
							"Items":["View","Filter"]
						},
						{
							"Items":["Clear","Clear All","Delete"]
						}
					]			
			};
			
//THIS IS REQUIRED TO PHYSICALLY LOAD THE PROPERTY LIST INTO THE ADMIN REGION - onClick_Repo_View DEMONSTRATES THE CALL TO LOAD.
sysAdmin.addProperty(
	'Events',
	{
	   "Name" : "<center>Event Log</center>",
	   "Code" : "Events",
	   "Description" : "Provides the ability for a developer to log and debug.",
	   "Display":"List",
	   "onLoad" : "viewEvents",
	   "onSave" : "",
	   "Template" :
	    "<div id=\"EventView\" class=\"PropertyList\"></div>" +
		"<div id=\"EventViewPager\" class=\"PropertyListPager\"></div><input id=\"frmConfigurationId\" name=\"frmConfigurationId\" type=\"hidden\"/>"
	}
);	
sysAdmin.addProperty(
	'Event',
	{
	   "Name" : "<center>Event</center>",
	   "Code" : "Event",
	   "Description" : "Provides the ability for a developer to view the event log entry.",
	   "Display":"Sheet",	   
	   "Header": "<div class=HTitle><center>Event Detail</center></div>",
	   "Footer": "<div class=HCommand><center><a href=\"#\" onclick=\"SaveProperty(false);\">Cancel</a></center></div>",
	   "onLoad" : "viewEvent",
	   "onSave" : "",
	   "Template" :
		"<div id=\"EventEntryView\"></div>"
	}
);	
			
function viewEvents(template,action)
{
    configurationId = configuration.ConfigurationID;
	sysSetText($('frmConfigurationId'),configurationId);
	ows.Create('EventView',0,20,_OWS_.CModuleId+':EventView,'+_OWS_.ResourceFile+':Admin.aspx.resx,'+_OWS_.ResourceKey+':OWS.Configuration.Events','','',false,-1,false,'EventView','EventViewPager');
}
var event_rowid = '';
function viewEvent(template,action)
{
	ows.Fetch('EventView',-1,'LogID='+event_rowid+'&Action=Detail','EventEntryView');
}	
function loadEvent(mid,rowid)
{
	event_rowid = rowid;
	LoadProperty('Event');
}
		
// EVENTS
function onClick_event_Filter(o)
{
	HFilter.innerHTML = '<div class="HTitle"><center><table border="0" cellpadding="2" cellspacing="2"><tr>' +
						'<td class="HStrip"><b>Event Filter</b>&nbsp;&nbsp;&nbsp;Session:&nbsp;<input name="frmEvent_Session"></td>'+
						'<td class="HStrip">Name:&nbsp;<input name="frmEvent_Name"></td>'+
						'<td class="HStrip">Date:&nbsp;From&nbsp;<input name="frmEvent_DateFrom">&nbsp;To&nbsp;<input name="frmEvent_DateTo"></td>'+
						'<td class="HStrip"><button onclick="onClick_event_View();return false;">Apply</button><button onclick="onClick_event_Filter_off();return false;">Cancel</button></td>'+
						'</tr></table></center></div>';
	showBlock(HFilter);
	onClick_event_View(o);
}
function onClick_event_Filter_off(o)
{
	HFilter.innerHTML = '';
	hideBlock(HFilter);
	onClick_event_View(o);
}
function onClick_event_Clear(o)
{
	clearEvent();
}
function onClick_event_Clear_All(o)
{
	clearAllEvent();
}

function onClick_event_View(o)
{
LoadProperty('Events');
}
function onClick_event_Delete(o)
{
deleteEvent();
}

var AjaxServerPageName;
AjaxServerPageName = "IM.aspx";
var configurationId = '6fafcdec-11c3-4348-aebd-079c6c12c178';  //'65EFA916-21D0-4829-A30C-2EC4E74ACF90';
var XmlHttp;

var srcConfiguration;
var configNameChoices = 0;
var configRegions = 0;

function getJsonConfig(configurationId) {
	var requestUrl = AjaxServerPageName + "?_OWS_=" + _OWS_.Action + ":LISTXCONFIG," + _OWS_.Type + ":JSON," + _OWS_.Actions + ":Get," + _OWS_.ConfigurationID + ":" + configurationId;
	$jq.ajax({
                url: requestUrl,
                dataType : 'json',
                data : {},
                success : function(data, textStatus){
			loadOWSConfig(data);
                },
                error : function(x, txt, e){
                    alert('Failed to complete query, please try again.');
                }
        });	
}

function getJsonExec(exec,cnn) {
    var conn = cnn;
    if (typeof cnn == 'undefined'||cnn==null)
        cnn = '';
    if (cnn.length>0)
        cnn = '&c='+escape(cnn);
    else
        cnn = '';
	var requestUrl = AjaxServerPageName + "?_OWS_=" + _OWS_.Action + ":LISTXCONFIG," + _OWS_.Type + ":JSON," + _OWS_.Actions + ":Exec&q=" + escape(exec) + cnn;
	$jq.growl("Execute", 'The Query is attempting execution on the server.','Images/forum.gif');
	$jq.ajax({
                url: requestUrl,
                dataType : 'json',
                data : {},
                success : function(data, textStatus){
			loadExecResult(data);
                },
                error : function(x, txt, e){
                    alert('Failed to complete query, please try again.');
                }
        });
}

function SetJsonByPostRequest(configId, publish, configurationSrc) 
{
	var requestUrl;
	if (publish){
		requestUrl = AjaxServerPageName + "?_OWS_=" + _OWS_.Action + ":LISTXCONFIG," + _OWS_.Actions + ":Set," + _OWS_.ConfigurationID + ":" + configId;
	} else {
		requestUrl = AjaxServerPageName + "?_OWS_=" + _OWS_.Action + ":LISTXCONFIG," + _OWS_.Actions + ":SetDraft," + _OWS_.ConfigurationID + ":" + configId;
	}
	if (configuration!=null && (configuration.enableQueryDebug=='true'||configuration.enableQueryDebug_Edit=='true'||configuration.enableQueryDebug_Admin=='true'||configuration.enableQueryDebug_Super=='true'))
	{
	    $jq.growl("<span class='warning'>Debugging Enabled</span>", '<div class="warning">Debugging is currently enabled. Remember to turn off debugging in production environments.','Images/srotator.gif');
	}
	$jq.growl("Publishing", 'Your changes are being saved.','Images/saved.gif');
	var d = {};
	d[configId] = configurationSrc;
	$jq.ajax({
                url: requestUrl,
                dataType : 'json',
		type : 'post',
                data : d,
                success : function(data, textStatus){
			if (data.Code=='ACK')
				//alert('The configuration has been published.');
				$jq.growl("Success", 'The configuration has been published.','Images/publish.gif');
			else
				alert('The publish request has failed, please check your current authentication rights and try again.');
                },
                error : function(x, txt, e){
                    alert('Failed to complete query, please try again.');
                }
        });
}

function getJsonConfigFromXml(xmlSrc) {
	var requestUrl = AjaxServerPageName + "?_OWS_=" + _OWS_.Action + ":LXIMPORT," + _OWS_.Type + ":JSON," + _OWS_.Actions + ":Get" ;
	$jq.ajax({
                url: requestUrl,
                dataType : 'json',
		type : 'post',
                data : {'Import':xmlSrc},
                success : function(data, textStatus){
			data.Name = 'Imported configuration';
			loadOWSConfig(data);
                },
                error : function(x, txt, e){
                    alert('Failed to complete query, please try again.');
                }
        });
}

/* loads configuration into the interface */
function loadOWSConfig(data) {
	loadConfiguration(data);
	sysProperty_ListItem = false;
	sysProperty_ObjectItem = false;
	displayProperty('',null);
	if (typeof configuration!='undefined'&&configuration!=null) showBlock(HMain);
	if (configuration!=null && (configuration.enableQueryDebug=='true'||configuration.enableQueryDebug_Edit=='true'||configuration.enableQueryDebug_Admin=='true'||configuration.enableQueryDebug_Super=='true'))
	{
	    $jq.growl("<span class='warning'>Debugging Enabled</span>", '<div class="warning">Debugging is currently enabled. Remember to turn off debugging in production environments.','Images/srotator.gif');
	}
}

/*Loading a List of Configurations*/



function loadConfigurationOptions() {
	var requestUrl = AjaxServerPageName + "?_OWS_=" + _OWS_.Action + ":LISTXCONFIG,"  + _OWS_.Type + ":JSON," + _OWS_.Actions + ":GetConfigList";
	$jq.ajax({
                url: requestUrl,
                dataType : 'json',
                data : {},
                success : function(data, textStatus){
			HandleConfigOptionsResponse(data);
                },
                error : function(x, txt, e){
                    alert('Failed to complete query, please try again.');
                }
        });
}


    //Called when response comes back from server
    function HandleConfigOptionsResponse(data)
    {
	    if (data!=null)
		    configNameChoices = data;
	    else
		    alert('Either no configurations exist or you do not have Administrative access. Please try again later.');
    }
    
    
    function loadConfigurationRegions(configID,callback) {
	var requestUrl = AjaxServerPageName + "?_OWS_=" + _OWS_.Action + ":LISTXCONFIG,"  + _OWS_.Type + ":JSON," + _OWS_.Actions + ":GetRegions," + _OWS_.ConfigurationID + ":" + configID;
	$jq.ajax({
                url: requestUrl,
                dataType : 'json',
                data : {},
                success : function(data, textStatus){
			HandleConfigRegionsResponse(data,callback);
                },
                error : function(x, txt, e){
                    alert('Failed to complete query, please try again.');
                }
        });
}

   
//Called when response comes back from server
function HandleConfigRegionsResponse(data,callback)
{
	configRegions = data;
	eval(callback);
}
$jq(document).ready(function() {
if (typeof(console)!='undefined'&&console!=null&&typeof(console.trace)!='undefined'&&typeof(console.trace)=='function')
{
    $jq.growl.settings.displayTimeout = 10000;
    $jq.growl("<span class='warning'>Firebug Enabled</span>", '<div class="warning">Firebug causes OpenWebStudio to operate poorly in most environments. Heavy memory consumption is normally the side effect of this. We recommend disabling firebug within the admin.','Images/srotator.gif');
    $jq.growl.settings.displayTimeout = 4000;
}
});	
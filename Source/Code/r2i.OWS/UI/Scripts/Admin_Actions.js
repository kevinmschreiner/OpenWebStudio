/*
function onAction_(ActionTypeTemplate,ActionElement)
{

}
*/
var qryResults = null;
function QueryExecute() {
var qry2 = $('frmActionQuery_Query');
var cnn = $('frmActionQuery_Connection');
getJsonExec(qry2.value,cnn.value);
}
function loadExecResult(rslt) {
var qry2 = $('qry2');
qryResults = rslt;
var output = new Array();
	if (qryResults!=null&&qryResults.Columns!=undefined&&qryResults.Columns!=null&&qryResults.Columns.length > 0)
	{
		output.push('<table width=100% cellpadding=0 cellspacing=0 class=HTbl>');
		output.push('<tr class=HTblHdr>');
		var x = 0;
		for (x=0;x<qryResults.Columns.length;x++)
		{
			output.push('<td>');
			output.push(sysShortenSummary(qryResults.Columns[x].Name,50));
			output.push('</td>');
		}
		output.push('</tr>');
		var x = 0;
		var alt = false;
		for (x=0;x<qryResults.Rows.length;x++)
		{
			var cname = 'HTblRow';
			alt = !alt;
			if (alt)
				cname = 'HTblRowAlt';
			output.push('<tr class=' + cname + '>');
			var y = 0;
			for (y=0;y<qryResults.Rows[x].Columns.length;y++)
			{
				output.push('<td>');
				output.push(sysShortenSummary(qryResults.Rows[x].Columns[y],200));
				output.push('</td>');
			}
			output.push('</tr>');
		}		
		output.push('</table>');
	}
	if (output.length==0)
	{
		qry2.innerHTML = "<center class=NormalRed>No Results, or a query error occurred. Try again.</center>";
		hideBlock($('qry1'));showBlock($('qry2'));
	}
	else
	{
		qry2.innerHTML = output.join('');
		showBlock($('qry1'));showBlock($('qry2'));
	}
}

function encode(v,c) {
     v= v.replace(/\//g,"\\");
	 v= v.replace(/'/g,"\'");
	//v=v.replace(/</g,"&lt;");
	//v=v.replace(/>/g,"&gt;");
	if (c!=undefined&&c==true)
		v = encode(v,false);
	return v;
}

function ColumnClip() {
	if (qryResults!=null&&qryResults.Columns!=undefined&&qryResults.Columns!=null&&qryResults.Columns.length > 0)
	{

		if (window.confirm('Would you like to clear out the current Clip?'))
		{
		RibbonEditor.configuration.Groups.ClipTags.Tabs[0].Items = new Array();
		RibbonEditor.configuration.Groups.ClipTags.Tabs[0].ItemEditor = new Array();
		RibbonEditor.configuration.Groups.ClipTags.Tabs[0].ItemHelp = new Array();
		}
		if (typeof RibbonEditor.configuration.Groups.ClipTags.Tabs[0].ItemHelp == 'undefined' || RibbonEditor.configuration.Groups.ClipTags.Tabs[0].ItemHelp == null)
			RibbonEditor.configuration.Groups.ClipTags.Tabs[0].ItemHelp = new Array();
		
		var qryName = $('frmActionQuery_Name');
		var prefix = prompt('If you need a prefix on the column names in the \nClip, please enter it in the box below:');
		if (prefix==null)
			prefix = '';
		if (qryName!=null)
		{
			if (qryName.value.length>0)
				qryName = ',' + qryName.value;
			else
				qryName = '';
		}
		else
		{
			qryName = '';
		}
		var postfix = prompt('If you need a postfix on the colum name in the \nClip, please enter it in the box below:',qryName);
		if (postfix==null)
			postfix = '';
		var x = 0;
		for (x=0;x<qryResults.Columns.length;x++)
		{
			RibbonEditor.configuration.Groups.ClipTags.Tabs[0].Items.push('[' + prefix + encodehtml(qryResults.Columns[x].Name) + postfix + ']');
			RibbonEditor.configuration.Groups.ClipTags.Tabs[0].ItemEditor.push('['  + prefix + encodehtml(qryResults.Columns[x].Name)  + postfix + ']');
			RibbonEditor.configuration.Groups.ClipTags.Tabs[0].ItemHelp.push('<b>['  + prefix + encodehtml(qryResults.Columns[x].Name) + postfix + ']</b> - Inserts the field tag into your content');
		}
		vquery = $('frmActionQuery_Query');
		RibbonEditor.configuration.Menu[RibbonEditor.configuration.Menu.length-1].Groups=["ClipTags"];
		RibbonEditor.Regenerate(vquery);
	}
}
function onActionPrint_Comment(template,action) { 
	return sysActionSummary(template.Name,' - ' + action.Parameters.Value,'Comment',actionDisplay(action));			
}

function onActionLoad_Comment(template,action) { 
	//AVAILABLE RESOURCES
	/*
		"Value":"Apples"' +
	*/	
	//ASSIGN PROPERTIES
	vvalue = $('frmActionComment');
	sysSetText(vvalue,action.Parameters.Value);
	RibbonEditor.Generate(vvalue);
}
function onActionSave_Comment(template,action) { 
	//AVAILABLE RESOURCES
	/*
		"Value":"Apples"' +
	*/	
	//ASSIGN PROPERTIES
	vvalue = $('frmActionComment');

	action.Parameters.Value = sysGetText(vvalue);
	return true;
}


//REGION-CONFIG SWITCHER
function onActionPrint_Goto(template,action) { 
	var summary = '';
	var configheader = "Configuration:";
	var regionheader = "Region:";
	if (typeof action.Parameters.ConfigurationID != 'undefined' && action.Parameters.ConfigurationID.length > 0 && action.Parameters.ConfigurationID != '00000000-0000-0000-0000-000000000000') {
		summary += configheader + action.Parameters.Name + ' ';
		configheader = '';
	}
	if (typeof action.Parameters.ConfigurationDyn != 'undefined' && action.Parameters.ConfigurationDyn.length > 0) {
		summary += configheader +'(dynamic) ' + action.Parameters.ConfigurationDyn + ' ';
	}

	if (action.Parameters.Region.length > 0) {
		summary += regionheader + action.Parameters.Region + ' ';
		regionheader = '';
	}
	if (typeof action.Parameters.RegionDyn != 'undefined' && action.Parameters.RegionDyn.length > 0) {
		summary += regionheader + '(dynamic) ' + action.Parameters.RegionDyn + ' ';
	}

	return sysActionSummary(template.Name,summary,null,actionDisplay(action));
}

function onActionLoad_Goto(template,action) { 
	var vname = $('frmActionGoto_Name');
	var vid = $('frmActionGoto_ConfigurationID');
	var vregion = $('frmActionGoto_Region');
	var vnameDyn = $('frmActionGoto_ConfigurationDyn');
	var vregionDyn = $('frmActionGoto_RegionDyn');
	
	sysSetText(vname,action.Parameters.Name);
	
	if (action.Parameters.Name.length > 0)
	{
		vid.options[vid.options.length]=new Option(action.Parameters.Name,action.Parameters.ConfigurationID);
		vregion.options[vregion.options.length]=new Option(action.Parameters.Region,action.Parameters.Region);
	}
	else
	{
	    loadConfigurationChoices_Goto();
	}
	sysSetSelect(vid,action.Parameters.ConfigurationID);
	sysSetSelect(vregion,action.Parameters.Region);

	sysSetText(vnameDyn, '');
	sysSetText(vregionDyn, '');
	if (action.Parameters.ConfigurationDyn != null) {
		sysSetText(vnameDyn, action.Parameters.ConfigurationDyn);
	}
	if (action.Parameters.RegionDyn != null) {
		sysSetText(vregionDyn, action.Parameters.RegionDyn);
	}	
}
function onActionSave_Goto(template,action) { 
	var vid = $('frmActionGoto_ConfigurationID');
	var vregion = $('frmActionGoto_Region');
	var vnameDyn = $('frmActionGoto_ConfigurationDyn');
	var vregionDyn = $('frmActionGoto_RegionDyn');
	
	action.Parameters.Name = sysGetSelectText(vid);
	action.Parameters.ConfigurationID = sysGetSelect(vid);
	action.Parameters.Region = sysGetSelect(vregion);
	action.Parameters.ConfigurationDyn = sysGetText(vnameDyn);
	action.Parameters.RegionDyn = sysGetText(vregionDyn);
return true;
}
function openConfig_Goto(cid) {
    if (cid != '00000000-0000-0000-0000-000000000000') {
        var url = '/DesktopModules/OWS/Admin.aspx#config/' + cid;
        window.open(url, '_blank');
    }
}
function loadConfigurationChoices_Goto(cid)
{

	if (configNameChoices == 0)
	{
		configNameChoices = 1;
		loadConfigurationOptions();
		
		setTimeout("loadConfigurationChoices_Goto('"+(typeof cid!='undefined'?(cid!=null?cid:''):'')+"')",200);
	}
	else if (configNameChoices!=1)
	{	
		var selList = $('frmActionGoto_ConfigurationID');
		var selected = sysGetSelect(selList);
		if (typeof selList.options.length != 'undefined')
		{
			while (selList.options.length > 0)
				selList.options[selList.options.length-1]=null;
		}
		selList.options[selList.options.length]=new Option('Select Configuration','00000000-0000-0000-0000-000000000000');
		for (var configId in configNameChoices)
		{
			selList.options[selList.options.length]=new Option(configNameChoices[configId],configId);
		}
		if (typeof selected!='undefined'&&selected!=null&&selected.length>0)
			sysSetSelect(selList,selected);
		else
		{
			configRegions=0;
			loadConfigurationRegions_Goto(configuration.ConfigurationID);
		}
	}
}
function loadConfigurationRegions_Goto(cid,load)
{
	var selList = $('frmActionGoto_Region');
	selList.onFocus = null;
	
	if (typeof cid != 'undefined' && cid!=null && cid.length > 0 && cid!='00000000-0000-0000-0000-000000000000')
	{
		if (typeof load == 'undefined' || load==null || load==0)
		{
			loadConfigurationRegions(cid,"loadConfigurationRegions_Goto('"+cid+"',1)");
		}
		else if (load==1)
		{	
			var selected = sysGetSelect(selList);
			if (typeof selList.options.length != 'undefined')
			{
				while (selList.options.length > 0)
					selList.options[selList.options.length-1]=null;
			}
			selList.options[selList.options.length]=new Option('Select a Region','');
			for (var i=0;i<configRegions.Regions.length;i++)
			{
				selList.options[selList.options.length]=new Option(configRegions.Regions[i],configRegions.Regions[i]);
			}
			if (typeof selected!='undefined'&&selected!=null)
				sysSetSelect(selList,selected);
		}
	}
	else
	{
		if (typeof selList.options.length != 'undefined')
		{
			while (selList.options.length > 0)
				selList.options[selList.options.length-1]=null;
		}
		selList.options[selList.options.length]=new Option('Select a Region','');
	}
}
function onGotoConfigChanged()
{
	configRegions = 0;
    loadConfigurationRegions_Goto(sysGetSelect($('frmActionGoto_ConfigurationID')));
}

//DELAY
function onActionPrint_Delay(template,action) { 
	return sysActionSummary(template.Name,' for ' + action.Parameters.Value + ' ' + action.Parameters.Type + (action.Parameters.Value!=1?'s':''),null,actionDisplay(action));
}
function onActionLoad_Delay(template,action) { 
	vname = document.getElementsByName('frmActionDelay_Type');
	vregion = $('frmActionDelay_Value');
	
	sysSetRadio(vname,action.Parameters.Type);
	sysSetText(vregion,action.Parameters.Value);
}
function onActionSave_Delay(template,action) { 
	vname = document.getElementsByName('frmActionDelay_Type');
	vregion = $('frmActionDelay_Value');
	
	action.Parameters.Type = sysGetRadio(vname);
	action.Parameters.Value = sysGetText(vregion);
	return true;
}

//REDIRECT
function onActionPrint_Redirect(template,action) { return sysActionSummary(template.Name,' to ' + action.Parameters.Type + ' ' + action.Parameters.Link,null,actionDisplay(action));}
function onActionLoad_Redirect(template,action) { 
	vname = document.getElementsByName('frmActionRedirect_Type');
	vlink = $('frmActionRedirect_Value');
	
	sysSetRadio(vname,action.Parameters.Type);
	sysSetText(vlink,action.Parameters.Link);
}
function onActionSave_Redirect(template,action) { 
	vname = document.getElementsByName('frmActionRedirect_Type');
	vlink = $('frmActionRedirect_Value');
	
	action.Parameters.Type = sysGetRadio(vname);
	action.Parameters.Link = sysGetText(vlink);
	return true;
}

//FILTER OPTIONS
function onActionPrint_Filter(template,action) { 
var summary = '';
	summary += ' Options ';
return sysActionSummary(template.Name,summary,null,actionDisplay(action));}

function onActionLoad_Filter(template,action) { 
	currentFilterAction=action;
	onActionBind_Filter_Options();
}

function onActionBind_Filter_Options() {
	var val = new Array();
	val.push('<table width=100% border=0 cellpadding=1 class=HTbl>');	
	val.push('<tr class=HTblHdr><td class=subhead><a href="javascript:onActionLoad_Filter_Option(-1);"><img src=images/add.gif border=0></a></td><td class=subhead>Option</td><td class=subhead>Field</td>')
	if (typeof currentFilterAction!='undefined' && typeof currentFilterAction.Parameters.Options!='undefined')
	{
		var p = 0;
		var alt = false;
		for (p=0;p<currentFilterAction.Parameters.Options.length;p++)
		{
			var cname = 'HTblRow';
			alt = !alt;
			if (alt)
				cname = 'HTblRowAlt';
			var xp = currentFilterAction.Parameters.Options[p];
			val.push('<tr class=' + cname + '><td><a href="javascript:onActionLoad_Filter_Option(' + p + ');"><img src=images/edit.gif border=0></a><a href="javascript:onActionDelete_Filter_Option(' + p + ');"><img border=0 src=images/delete.gif></a></td><td>'+sysShortenSummary(xp.Option)+'</td><td>'+sysShortenSummary(xp.Field)+'</td></tr>');
		}
	}
	val.push('</table><div id=fi15NCM></div>')
	$('fi15').innerHTML = val.join('');
}
function onActionSave_Filter_Option(i) {
	if (currentFilterAction!=undefined && currentFilterAction.Parameters.Options!=undefined && currentFilterAction.Parameters.Options.length>i&& i >= 0)
	{
		var cfm = null;
		cfm = currentFilterAction.Parameters.Options[i];
		cfm.Option = sysGetText($('frmFOOption'));
		cfm.Field = sysGetText($('frmFOField'));
	}
	var objT = $('fi15NCM');
	objT.innerHTML = '';
	onActionBind_Filter_Options();
}
function onActionDelete_Filter_Option(i) {
	if (currentFilterAction!=undefined && currentFilterAction.Parameters.Options!=undefined && i<currentFilterAction.Parameters.Options.length)
	{
		currentFilterAction.Parameters.Options.splice(i,1);
	}
	var objT = $('fi15NCM');
	objT.innerHTML = '';
	onActionBind_Filter_Options();
}
function onActionLoad_Filter_Option(i) {
	if (i==-1)
	{
		if (currentFilterAction!=undefined)
		{
			if ((currentFilterAction.Parameters.Options==undefined) || (currentFilterAction.Parameters.Options.length == 0))
			{
				currentFilterAction.Parameters.Options=new Array();
			}
			i = currentFilterAction.Parameters.Options.length;
			currentFilterAction.Parameters.Options.push({"Option":"New Option","Filter":""});
		}
	}
	if (currentFilterAction!=undefined && currentFilterAction.Parameters.Options!=undefined && i<currentFilterAction.Parameters.Options.length)
	{
		var objT = $('fi15NCM');
		objT.innerHTML = sysFilterOptionTemplate.Template + 
		'<div class=HFile><center><a href="javascript:onActionSave_Filter_Option(' + i + ');">Save</a>&nbsp;|&nbsp;<a href="javascript:onActionSave_Filter_Option(-1);">Cancel</a></center></div>';
		var cfm = null;
		cfm = currentFilterAction.Parameters.Options[i];
		sysSetText($('frmFOOption'),cfm.Option);
		sysSetText($('frmFOField'),cfm.Field);
	}
}
var currentFilterAction = null;
function onActionSave_Filter(template,action) { 
	return true;
}

//FILE
function onActionPrint_File(template,action) { 
var summary = '';
	summary += action.Parameters.SourceType + ' source ';
	if (action.Parameters.Source!=null && action.Parameters.Source.length>0)
		summary += '"' + action.Parameters.Source + '" ';
	if (action.Parameters.DestinationType!=null && action.Parameters.DestinationType.length>0)
		summary += action.Parameters.DestinationType + ' destination ';
	if (action.Parameters.Destination!=null && action.Parameters.Destination.length>0)
		summary += '"' + action.Parameters.Destination + '" ';		
	if (action.Parameters.TransformType!=null && action.Parameters.TransformType.length>0)
		summary += action.Parameters.TransformType + ' with ' + action.Parameters.TransformType + ' transform';	
return sysActionSummary(template.Name,summary,null,actionDisplay(action));}

function ActionParameter_ClearFormatting(value)
{
    if (typeof value != 'undefined' && value!=null) {
	value = value.replace(/</g, "&lt;");
	value = value.replace(/>/g, "&gt;"); }
	return value;
}
function onActionLoad_File(template,action) { 
	currentFileAction=action;

	sysSetSelect($('frmSourceType'),action.Parameters.SourceType);
	
	sysSetSelect($('frmSourceVariableType'),ActionParameter_ClearFormatting(action.Parameters.SourceVariableType));
	sysSetText($('frmSource'),action.Parameters.Source);
	sysSetText($('frmSourceQuery'),action.Parameters.SourceQuery);
	sysSetText($('frmSourceConnection'),action.Parameters.SourceConnection);
	RibbonEditor.Generate($('frmSourceQuery'));
	sysSetSelect($('frmDestinationType'),action.Parameters.DestinationType);
	
	sysSetSelect($('frmResultVariableType'),ActionParameter_ClearFormatting(action.Parameters.DestinationVariableType));
	sysSetText($('frmResponseType'),action.Parameters.DestinationResponseType);
	sysSetCheck($('frmChkFirstRow'),action.Parameters.SQLFirstRow);
	sysSetText($('frmDestinationQuery'),action.Parameters.DestinationQuery);
	RibbonEditor.Generate($('frmDestinationQuery'));
	sysSetCheck($('frmChkRunAsProcess'),action.Parameters.RunAsProcess);
	sysSetText($('frmFileProcessName'),action.Parameters.ProcessName);
	sysSetText($('frmDestination'),action.Parameters.Destination);
	sysSetSelect($('frmTransformation'),action.Parameters.TransformType);
	sysSetText($('frmImageWidth'),action.Parameters.ImageWidth);
	sysSetSelect($('frmImageWidthType'),action.Parameters.ImageWidthType);
	sysSetText($('frmImageHeight'),action.Parameters.ImageHeight);
	sysSetSelect($('frmImageHeightType'),action.Parameters.ImageHeightType);
	sysSetText($('frmImageQuality'),action.Parameters.ImageQuality);
	sysSetSelect($('frmFileOperation'), action.Parameters.FileTask);
	sysSetText($('frmImageRotation'), action.Parameters.ImageRotation);
	
	if (typeof action.Parameters.ImageTransformType == 'undefined' || action.Parameters.ImageTransformType == null)
	{
	    action.Parameters.ImageTransformType = 'Size';
	    action.Parameters.ImageFont = '';
	    action.Parameters.ImageColor = '';
	    action.Parameters.ImageBGColor = '';
	    action.Parameters.ImageWarp = '';
	}
	if (typeof action.Parameters.ImageRotation == 'undefined' || action.Parameters.ImageRotation == null) {
	    action.Parameters.ImageRotation = '';
	}
	if (typeof action.Parameters.ImageWarp == 'undefined' || action.Parameters.ImageWarp == null)
	{
	    action.Parameters.ImageWarp = '';
	}
	if (typeof action.Parameters.ImageText == 'undefined' || action.Parameters.ImageText == null)
	{
	    action.Parameters.ImageText = '';
	}
	if (typeof action.Parameters.ImageX == 'undefined' || action.Parameters.ImageX == null) {
	    action.Parameters.ImageX = '0';
	}
	if (typeof action.Parameters.ImageY == 'undefined' || action.Parameters.ImageY == null) {
	    action.Parameters.ImageY = '0';
	}

	sysSetSelect($('frmImageTransformType'),action.Parameters.ImageTransformType);
	sysSetText($('frmImageFont'),action.Parameters.ImageFont);
	sysSetText($('frmImageSize'),action.Parameters.ImageSize);
	sysSetSelect($('frmImageSizeType'),action.Parameters.ImageSizeType);
	sysSetText($('frmImageColor'),action.Parameters.ImageColor);
	sysSetText($('frmImageBGColor'),action.Parameters.ImageBGColor);
	sysSetText($('frmImageText'),action.Parameters.ImageText);
	sysSetText($('frmImageX'),action.Parameters.ImageX);
	sysSetText($('frmImageY'), action.Parameters.ImageY);
	sysSetText($('frmImageRotation'), action.Parameters.ImageRotation);
	//sysSetRadio(document.getElementsByName('frmImageWarp'),action.Parameters.ImageWarp);
	
	
	var chkItems = document.getElementsByName('frmImageWarp');
	for (var chkI=0;chkI < chkItems.length;chkI++)
	{
	    if ((','+action.Parameters.ImageWarp+',').match(','+chkItems[chkI].value+',')!=null)
	       chkItems[chkI].checked=true;
	}
    chkItems = null;
    	
	onfrmTransformation_Toggle();
	onfrmImageTransformType_Toggle();
	onActionBind_File_Columns();
	onfrmDestinationType_Toggle();
	onfrmSourceType_Toggle();
}
//sysFileColumnMapTemplate
function onActionBind_File_Columns() {
	var val = new Array();
	val.push('<table width=100% border=0 cellpadding=1 class=HTbl>');	
	val.push('<tr class=HTblHdr><td class=subhead><a href="javascript:onActionLoad_File_Column(-1);"><img src=images/add.gif border=0></a></td><td class=subhead>Position</td><td class=subhead>Name</td><td class=subhead>Target</td><td class=subhead>Type</td>')
	if (currentFileAction!=undefined && currentFileAction.Parameters.ColumnMappings!=undefined)
	{
		var p = 0;
		var alt = false;
		for (p=0;p<currentFileAction.Parameters.ColumnMappings.length;p++)
		{
			var cname = 'HTblRow';
			alt = !alt;
			if (alt)
				cname = 'HTblRowAlt';
			var xp = currentFileAction.Parameters.ColumnMappings[p];
			val.push('<tr class=' + cname + '><td><a href="javascript:onActionLoad_File_Column(' + p + ');"><img src=images/edit.gif border=0></a><a href="javascript:onActionDelete_File_Column(' + p + ');"><img border=0 src=images/delete.gif></a></td><td>'+sysShortenSummary(xp.Position)+'</td><td>'+sysShortenSummary(xp.Name)+'</td><td>'+sysShortenSummary(xp.Target)+'</td><td>'+sysShortenSummary(xp.Type)+'</td></tr>');
		}
	}
	val.push('</table><div id=fi15NCM></div>')
	$('fi15').innerHTML = val.join('');
}
function onActionSave_File_Column(i) {
	if (currentFileAction!=undefined && currentFileAction.Parameters.ColumnMappings!=undefined && currentFileAction.Parameters.ColumnMappings.length>i&& i >= 0)
	{
		var cfm = null;
		cfm = currentFileAction.Parameters.ColumnMappings[i];
		cfm.FileType = sysGetSelect($('frmCMType'));
		cfm.Position = sysGetText($('frmCMPosition'));
		cfm.StartColumn = sysGetText($('frmCMStartingColumn'));
		cfm.EndColumn = sysGetText($('frmCMEndingColumn'));
		cfm.Name = sysGetText($('frmCMName'));
		cfm.Target = sysGetText($('frmCMTarget'));
		cfm.Type = sysGetSelect($('frmCMDataType'));
		cfm.Format = sysGetText($('frmCMFormat'));
		cfm.DefaultValue = sysGetText($('frmCMDefault'));
		cfm.NullValue = sysGetText($('frmCMNull'));
	}
	var objT = $('fi15NCM');
	objT.innerHTML = '';
	onActionBind_File_Columns();
}
function onActionDelete_File_Column(i) {
	if (currentFileAction!=undefined && currentFileAction.Parameters.ColumnMappings!=undefined && i<currentFileAction.Parameters.ColumnMappings.length)
	{
		currentFileAction.Parameters.ColumnMappings.splice(i,1);
	}
	var objT = $('fi15NCM');
	objT.innerHTML = '';
	onActionBind_File_Columns();
}
function onActionLoad_File_Column(i) {
	if (i==-1)
	{
		if (currentFileAction!=undefined)
		{
			if ((currentFileAction.Parameters.ColumnMappings==undefined) || (currentFileAction.Parameters.ColumnMappings.length == 0))
			{
				currentFileAction.Parameters.ColumnMappings=new Array();
			}
			i = currentFileAction.Parameters.ColumnMappings.length;
			currentFileAction.Parameters.ColumnMappings.push({"FileType":"CSV","Position":"","StartColumn":"","EndColumn":"","Name":"","Target":"","Type":"","Format":"","DefaultValue":"","NullValue":""});
		}
	}
	if (currentFileAction!=undefined && currentFileAction.Parameters.ColumnMappings!=undefined && i<currentFileAction.Parameters.ColumnMappings.length)
	{
		var objT = $('fi15NCM');
		objT.innerHTML = sysFileColumnMapTemplate.Template + 
		'<div class=HFile><center><a href="javascript:onActionSave_File_Column(' + i + ');">Save</a>&nbsp;|&nbsp;<a href="javascript:onActionSave_File_Column(-1);">Cancel</a></center></div>';
		var cfm = null;
		cfm = currentFileAction.Parameters.ColumnMappings[i];
		sysSetSelect($('frmCMType'),cfm.FileType);
		sysSetText($('frmCMPosition'),cfm.Position);
		sysSetText($('frmCMStartingColumn'),cfm.StartColumn);
		sysSetText($('frmCMEndingColumn'),cfm.EndColumn);
		sysSetText($('frmCMName'),cfm.Name);
		sysSetText($('frmCMTarget'),cfm.Target);
		sysSetSelect($('frmCMDataType'),cfm.Type);
		sysSetText($('frmCMFormat'),cfm.Format);
		sysSetText($('frmCMDefault'),cfm.DefaultValue);
		sysSetText($('frmCMNull'),cfm.NullValue);
		
		onfrmCMType_Toggle();
	}
}
var currentFileAction = null;
function onfrmSourceType_Toggle()
{
	vmeth = $('frmSourceType');
	switch(sysGetSelect(vmeth).toLowerCase())
	{
		case '':
			hideBlock($('fi0'));hideBlock($('fi1'));showBlock($('fi2'));showBlock($('fi3'));hideBlock($('fi4'));hideBlock($('fi5'));hideBlock($('fy4'));hideBlock($('fy5'));
			break;
		case 'path':
			hideBlock($('fi0'));hideBlock($('fi1'));showBlock($('fi2'));showBlock($('fi3'));hideBlock($('fi4'));hideBlock($('fi5'));hideBlock($('fy4'));hideBlock($('fy5'));
			break;
		case 'variable':
			showBlock($('fi0'));showBlock($('fi1'));showBlock($('fi2'));showBlock($('fi3'));hideBlock($('fi4'));hideBlock($('fi5'));hideBlock($('fy4'));hideBlock($('fy5'));
			break;
		case 'sql':
			hideBlock($('fi0'));hideBlock($('fi1'));hideBlock($('fi2'));hideBlock($('fi3'));showBlock($('fi4'));showBlock($('fy4'));showBlock($('fi5'));showBlock($('fy5'));
			break;
	}
}
function onfrmCMType_Toggle()
{
	vmeth = $('frmCMType');
	switch(sysGetSelect(vmeth).toLowerCase())
	{
		case '':
			hideBlock($('cm0'));hideBlock($('cm1'));hideBlock($('cm2'));hideBlock($('cm3'));
			hideBlock($('cm4'));hideBlock($('cm5'));
			break;
		case 'csv':
			showBlock($('cm0'));showBlock($('cm1'));hideBlock($('cm2'));hideBlock($('cm3'));
			hideBlock($('cm4'));hideBlock($('cm5'));
			break;
		case 'fixed':
			hideBlock($('cm0'));hideBlock($('cm1'));showBlock($('cm2'));showBlock($('cm3'));
			showBlock($('cm4'));showBlock($('cm5'));
			break;
	}
}
function onfrmTransformation_Toggle()
{
	vmeth = $('frmTransformation');
	switch(sysGetSelect(vmeth).toLowerCase())
	{
		case '':
		    for(var i=18;i<=45;i++) {
		    	hideBlock($('fi'+i)); }
			break;
		case 'image':
		    for(var i=18;i<=45;i++) {
		    	hideBlock($('fi'+i)); }
			showBlock($('fi20'));showBlock($('fi21'));
			onfrmImageTransformType_Toggle();
			break;
		case 'file':
		    for(var i=18;i<=45;i++) {
		    	hideBlock($('fi'+i)); }
			showBlock($('fi18'));showBlock($('fi19'));
			break;
	}
}
function onfrmImageTransformType_Toggle()
{
	var tmeth = $('frmTransformation');
	if (sysGetSelect(tmeth).toLowerCase()=='image')
	{
	    vmeth = $('frmImageTransformType');
	    switch(sysGetSelect(vmeth).toLowerCase())
	    {
		    case '':
			    //hideBlock($('fi18'));hideBlock($('fi19'));hideBlock($('fi20'));hideBlock($('fi21'));
			    //hideBlock($('fi22'));hideBlock($('fi23'));hideBlock($('fi24'));hideBlock($('fi25'));
		        //break;
            case 'smartcrop':
		    case 'size':
		        for(var i=22;i<=45;i++) {
		    	    hideBlock($('fi'+i)); }
		        for(var i=22;i<=27;i++) {
		            showBlock($('fi' + i));
		        }
		        break;
		    case 'crop':
		        for (var i = 22; i <= 45; i++) {
		            hideBlock($('fi' + i));
		        }
		        for (var i = 22; i <= 27; i++) {
		            showBlock($('fi' + i));
		        }
		        for (var i = 30; i <= 33; i++) {
		            showBlock($('fi' + i));
		        }
		        break;
		    case 'draw.text':
		        for(var i=22;i<=45;i++) {
		    	    hideBlock($('fi'+i)); }
		        for(var i=28;i<=43;i++) {
		            showBlock($('fi' + i));
		        }
		    	break;
		 case 'rotate':
		     for (var i = 22; i <= 45; i++) {
		         hideBlock($('fi' + i));}
		     for (var i = 44; i <= 45; i++) {
		         showBlock($('fi' + i));
		     }
		     break;
	    }
	}
}

function onfrmDestinationType_Toggle()
{
	vmeth = $('frmDestinationType');
	switch(sysGetSelect(vmeth).toLowerCase())
	{
		case '':
			hideBlock($('fi6'));hideBlock($('fi7'));hideBlock($('fi8'));hideBlock($('fi9'));hideBlock($('fi10'));hideBlock($('fi11'));
			hideBlock($('fi12'));hideBlock($('fi13'));hideBlock($('fi14'));hideBlock($('fi15'));hideBlock($('fi16'));hideBlock($('fi17'));
			break;
		case 'path':
			hideBlock($('fi6'));hideBlock($('fi7'));hideBlock($('fi8'));hideBlock($('fi9'));hideBlock($('fi10'));hideBlock($('fi11'));
			hideBlock($('fi12'));hideBlock($('fi13'));hideBlock($('fi14'));hideBlock($('fi15'));hideBlock($('fi16'));hideBlock($('fi17'));
			break;
		case 'response':
			showBlock($('fi6'));showBlock($('fi7'));hideBlock($('fi8'));hideBlock($('fi9'));hideBlock($('fi10'));hideBlock($('fi11'));
			hideBlock($('fi12'));hideBlock($('fi13'));hideBlock($('fi14'));hideBlock($('fi15'));hideBlock($('fi16'));hideBlock($('fi17'));		
			break;
		case 'emailattachment':
			showBlock($('fi6'));showBlock($('fi7'));hideBlock($('fi8'));hideBlock($('fi9'));hideBlock($('fi10'));hideBlock($('fi11'));
			hideBlock($('fi12'));hideBlock($('fi13'));hideBlock($('fi14'));hideBlock($('fi15'));hideBlock($('fi16'));hideBlock($('fi17'));
			break;
		case 'variable':
			hideBlock($('fi6'));hideBlock($('fi7'));showBlock($('fi8'));showBlock($('fi9'));hideBlock($('fi10'));hideBlock($('fi11'));
			hideBlock($('fi12'));hideBlock($('fi13'));hideBlock($('fi14'));hideBlock($('fi15'));hideBlock($('fi16'));hideBlock($('fi17'));
			break;
		case 'sql':
			hideBlock($('fi6'));hideBlock($('fi7'));hideBlock($('fi8'));hideBlock($('fi9'));showBlock($('fi10'));showBlock($('fi11'));
			showBlock($('fi12'));showBlock($('fi13'));showBlock($('fi14'));showBlock($('fi15'));showBlock($('fi16'));showBlock($('fi17'));
			onActionBind_File_Columns();
			break;
	}
}
function onActionSave_File(template,action) { 
	action.Parameters.SourceType = sysGetSelect($('frmSourceType'));
	action.Parameters.SourceVariableType = sysGetSelect($('frmSourceVariableType'));
	action.Parameters.Source = sysGetText($('frmSource'));
	action.Parameters.SourceQuery = sysGetText($('frmSourceQuery'));
	action.Parameters.SourceConnection = sysGetText($('frmSourceConnection'));
	action.Parameters.DestinationType = sysGetSelect($('frmDestinationType'));
	action.Parameters.DestinationVariableType = sysGetSelect($('frmResultVariableType'));
	action.Parameters.DestinationResponseType = sysGetText($('frmResponseType'));
	action.Parameters.SQLFirstRow = sysGetCheck($('frmChkFirstRow'));
	action.Parameters.DestinationQuery = sysGetText($('frmDestinationQuery'));
	action.Parameters.RunAsProcess = sysGetCheck($('frmChkRunAsProcess'));
	action.Parameters.ProcessName = sysGetText($('frmFileProcessName'));
	action.Parameters.Destination = sysGetText($('frmDestination'));
	action.Parameters.TransformType = sysGetSelect($('frmTransformation'));
	action.Parameters.ImageRotation = sysGetText($('frmImageRotation'));
	action.Parameters.ImageWidth = sysGetText($('frmImageWidth'));
	action.Parameters.ImageWidthType = sysGetSelect($('frmImageWidthType'));
	action.Parameters.ImageHeight = sysGetText($('frmImageHeight'));
	action.Parameters.ImageHeightType = sysGetSelect($('frmImageHeightType'));
	action.Parameters.ImageQuality = sysGetText($('frmImageQuality'));
	action.Parameters.FileTask = sysGetSelect($('frmFileOperation'));
	action.Parameters.ImageTransformType = sysGetSelect($('frmImageTransformType'));	
	action.Parameters.ImageFont = sysGetText($('frmImageFont'));
	action.Parameters.ImageSize = sysGetText($('frmImageSize'));
	action.Parameters.ImageSizeType = sysGetSelect($('frmImageSizeType'));
	action.Parameters.ImageColor = sysGetText($('frmImageColor'));
	action.Parameters.ImageBGColor = sysGetText($('frmImageBGColor'));
	action.Parameters.ImageText = sysGetText($('frmImageText'));
	action.Parameters.ImageX = sysGetText($('frmImageX'));
	action.Parameters.ImageY = sysGetText($('frmImageY'));
	
	var chks = new Array();
	var chkItems = document.getElementsByName('frmImageWarp');
	for (var chkI=0;chkI < chkItems.length;chkI++)
	{
	    if (chkItems[chkI].checked)
	        chks.push(chkItems[chkI].value);
	}
    chkItems = null;	    	    	
	action.Parameters.ImageWarp = chks.join(',');
	return true;
}

//INPUT
function onActionPrint_Input(template,action) { 
return sysActionSummary(template.Name,' From ' + action.Parameters.URL,null,actionDisplay(action));
}
function onActionLoad_Input(template,action) { 
/*
"URL":"URL",
"Querystring":"Querystring",
"Data":"Data",
"ContentType":"ContentType",
"Method":"Method",
"VariableType":"VariableType",
"VariableName":"VariableName",
"InputFormat":"InputFormat",
"XPath":"XPath",
"AuthenticationType":"AuthenticationType",
"Headers":"Headers",
"Domain":"Domain",
"Username":"Username",
"Password":"Password",
"SoapAction":"SoapAction",
"SoapResult":"SoapResult"
*/
	vurl = $('frmActionInput_URL');
	vauth = document.getElementsByName('frmActionInput_Auth');
	vusername = $('frmActionInput_UserName');
	vpass = $('frmActionInput_Password');
	vdomain = $('frmActionInput_Domain');
	vctype = $('frmActionInput_ContentType');
	vqparams = $('frmActionInput_QueryParameters');
	vdata = $('frmActionInput_Data');
	vheaders = $('frmActionInput_Headers');
	vmeth = $('frmActionInput_Method');
	vsoapa = $('frmActionInput_SoapAction');
	vsoapr = $('frmActionInput_SoapResult');
	vresponse = $('frmActionInput_ResponseFormat');
	vtype = $('frmActionInput_VariableType');
	vname = $('frmActionInput_Name');
	vxmlp = $('frmActionInput_XMLPath');
	
	sysSetText(vurl,action.Parameters.URL);
	sysSetRadio(vauth,action.Parameters.AuthenticationType);
	sysSetText(vusername,action.Parameters.Username);
	sysSetText(vpass,action.Parameters.Password);
	sysSetText(vdomain,action.Parameters.Domain);
	sysSetText(vctype,action.Parameters.ContentType);
	sysSetText(vqparams,action.Parameters.Querystring);
	sysSetText(vdata,action.Parameters.Data);
	sysSetText(vheaders,action.Parameters.Headers);
	sysSetSelect(vmeth,action.Parameters.Method);
	sysSetText(vsoapa,action.Parameters.SoapAction);
	sysSetText(vsoapr,action.Parameters.SoapResult);
	sysSetSelect(vresponse,action.Parameters.InputFormat);
	sysSetSelect(vtype,action.Parameters.VariableType);
	sysSetText(vname,action.Parameters.VariableName);
	sysSetText(vxmlp,action.Parameters.XPath);
	
	onfrmActionInput_AuthToggle();
	onfrmActionInput_MethodToggle();
	onfrmActionInput_ResponseFormatToggle();
	
	RibbonEditor.Generate(vheaders);
	RibbonEditor.Generate(vdata);
}
function onfrmActionInput_ResponseFormatToggle()
{	vmeth = $('frmActionInput_ResponseFormat');
	if (sysGetSelect(vmeth).toLowerCase()=='xml')
	{showBlock($('dfrm10'));showBlock($('dfrm11'));}
	else
	{hideBlock($('dfrm10'));hideBlock($('dfrm11'));}
}
function onfrmActionInput_AuthToggle()
{	vauth = document.getElementsByName('frmActionInput_Auth');
	if (sysGetRadio(vauth)==1||sysGetRadio(vauth)==3)
	{showBlock($('dfrm0'));showBlock($('dfrm1'));showBlock($('dfrm2'));showBlock($('dfrm3'));showBlock($('dfrm4'));showBlock($('dfrm5'));}
	else
	{hideBlock($('dfrm0'));hideBlock($('dfrm1'));hideBlock($('dfrm2'));hideBlock($('dfrm3'));hideBlock($('dfrm4'));hideBlock($('dfrm5'));}
}
function onfrmActionInput_MethodToggle()
{	vmeth = $('frmActionInput_Method');
	if (sysGetSelect(vmeth).toLowerCase()=='soap')
	{showBlock($('dfrm6'));showBlock($('dfrm7'));showBlock($('dfrm8'));showBlock($('dfrm9'));}
	else
	{hideBlock($('dfrm6'));hideBlock($('dfrm7'));hideBlock($('dfrm8'));hideBlock($('dfrm9'));}
}
function onActionSave_Input(template,action) { 
	vurl = $('frmActionInput_URL');
	vauth = document.getElementsByName('frmActionInput_Auth');
	vusername = $('frmActionInput_UserName');
	vpass = $('frmActionInput_Password');
	vdomain = $('frmActionInput_Domain');
	vctype = $('frmActionInput_ContentType');
	vqparams = $('frmActionInput_QueryParameters');
	vdata = $('frmActionInput_Data');
	vheaders = $('frmActionInput_Headers');
	vmeth = $('frmActionInput_Method');
	vsoapa = $('frmActionInput_SoapAction');
	vsoapr = $('frmActionInput_SoapResult');
	vresponse = $('frmActionInput_ResponseFormat');
	vtype = $('frmActionInput_VariableType');
	vname = $('frmActionInput_Name');
	vxmlp = $('frmActionInput_XMLPath');
	
	action.Parameters.URL = sysGetText(vurl);
	action.Parameters.AuthenticationType = sysGetRadio(vauth);
	action.Parameters.Username = sysGetText(vusername);
	action.Parameters.Password = sysGetText(vpass);
	action.Parameters.Domain = sysGetText(vdomain);
	action.Parameters.ContentType = sysGetText(vctype);
	action.Parameters.Querystring = sysGetText(vqparams);
	action.Parameters.Data = sysGetText(vdata);
	action.Parameters.Headers = sysGetText(vheaders);
	action.Parameters.Method = sysGetSelect(vmeth);
	action.Parameters.SoapAction = sysGetText(vsoapa);
	action.Parameters.SoapResult = sysGetText(vsoapr);
	action.Parameters.InputFormat = sysGetSelect(vresponse);
	action.Parameters.VariableType = sysGetSelect(vtype);
	action.Parameters.VariableName = sysGetText(vname);
	action.Parameters.XPath = sysGetText(vxmlp);

	return true;
}


//OUTPUT
function onActionPrint_Output(template,action) { 
return sysActionSummary(template.Name,action.Parameters.OutputType + (action.Parameters.Filename.length>0?' to file ' + action.Parameters.Filename:''),null,actionDisplay(action));
}

function onActionLoad_Output(template,action) { 
	vtype = $('frmActionOutput_Type');
	vdelim = $('frmActionOutput_Delimiter');
	vfile = $('frmActionOutput_Filename');
	
	sysSetSelect(vtype,action.Parameters.OutputType);
	sysSetText(vfile,action.Parameters.Filename);
	sysSetText(vdelim,action.Parameters.Delimiter);
	
	onfrmActionOutput_TypeToggle();
}
function onfrmActionOutput_TypeToggle()
{	vmeth = $('frmActionOutput_Type');
	if (sysGetSelect(vmeth).toLowerCase()=='delimited'||sysGetSelect(vmeth).toLowerCase()=='complete delimited')
	{showBlock($('dl0'));showBlock($('dl1'));}
	else
	{hideBlock($('dl0'));hideBlock($('dl1'));}
}
function onActionSave_Output(template,action) { 
	vtype = $('frmActionOutput_Type');
	vdelim = $('frmActionOutput_Delimiter');
	vfile = $('frmActionOutput_Filename');
	
	action.Parameters.OutputType = sysGetSelect(vtype);
	action.Parameters.Filename = sysGetText(vfile);
	action.Parameters.Delimiter = sysGetText(vdelim);
	return true;
}

//QUERY
function onActionPrint_Query(template,action) { 
return sysActionSummary((action.Parameters.IsProcess.toUpperCase()=='TRUE'?'(Process)&nbsp;&nbsp;':'') + template.Name + '[' + action.Parameters.Name + ']',action.Parameters.Query,null,actionDisplay(action));
}
function onActionLoad_Query(template,action) {  
	//AVAILABLE RESOURCES
	/*
	"Name":"Delete",  ' +
	"Query":"DELETE FROM Lists WHERE EntryID=@EntryID",  ' +
	"IsProcess":"False",' +
	"Connection":"provider=sqloledb;Server=(local);Database=DotNetNuke;uid=sa;pwd=bi4ce;"' +
	*/
	//ASSIGN PROPERTIES
	var vname = $('frmActionQuery_Name');
	var vconnection = $('frmActionQuery_Connection');
	var vquery = $('frmActionQuery_Query');
	var vprocess = $('frmActionQuery_IsProcess');
	var vcachetime = $('frmActionQuery_CacheTime');
		var vcachename = $('frmActionQuery_CacheName');
	var vcs = $('frmActionQuery_CacheShared');
	
	sysSetText(vname,action.Parameters.Name);
	sysSetText(vconnection,action.Parameters.Connection);
	sysSetText(vquery,action.Parameters.Query);
	sysSetText(vcachetime,action.Parameters.CacheTime);
	sysSetText(vcachename,action.Parameters.CacheName);	
	sysSetCheck(vcs,action.Parameters.CacheShared);
	sysSetCheck(vprocess,action.Parameters.IsProcess);
	
	hideBlock($('qry1'));hideBlock($('qry2'));
	/*hideBlock($('qry3'));
	hideBlock($('qry4'));hideBlock($('qry5'));hideBlock($('qry6'));
	hideBlock($('qry7'));hideBlock($('qry8'));hideBlock($('qry9'));
	hideBlock($('qry10'));hideBlock($('qry11'));hideBlock($('qry12'));*/
	
	RibbonEditor.Generate(vquery);
}
function onActionSave_Query(template,action) { 
	//AVAILABLE RESOURCES
	/*
	"Name":"Delete",  ' +
	"Query":"DELETE FROM Lists WHERE EntryID=@EntryID",  ' +
	"IsProcess":"False",' +
	"Connection":"provider=sqloledb;Server=(local);Database=DotNetNuke;uid=sa;pwd=bi4ce;"' +
	*/
	//ASSIGN PROPERTIES
	var vname = $('frmActionQuery_Name');
	var vconnection = $('frmActionQuery_Connection');
	var vquery = $('frmActionQuery_Query');
	var vprocess = $('frmActionQuery_IsProcess');
	var vcachetime = $('frmActionQuery_CacheTime');
	var vcachename = $('frmActionQuery_CacheName');
	var vcs = $('frmActionQuery_CacheShared');

	action.Parameters.Name = sysGetText(vname);
	action.Parameters.Connection = sysGetText(vconnection);
	action.Parameters.Query = sysGetText(vquery);
	action.Parameters.IsProcess = sysGetCheck(vprocess);
	action.Parameters.CacheName = sysGetText(vcachename);	
	action.Parameters.CacheTime = sysGetText(vcachetime);
	action.Parameters.CacheShared = sysGetCheck(vcs);

	return true;
}

//MESSAGE
function onActionPrint_Message(template,action) { 
	if (action.Parameters.Type.length > 0 )
	{	if (action.Parameters.Value.length > 0)
			return sysActionSummary(template.Name,'Awaiting Incoming Event with Type \'' + action.Parameters.Type + '\' and Value \'' + action.Parameters.Value + '\'',null,actionDisplay(action));			
		else
			return sysActionSummary(template.Name,'Awaiting Incoming Event with Type \'' + action.Parameters.Type + '\'',null,actionDisplay(action));		
	}
	else
	{	if (action.Parameters.Value.length > 0)
			return sysActionSummary(template.Name,'Awaiting Incoming Event with Value \'' + action.Parameters.Value + '\'',null,actionDisplay(action));			
		else
			return sysActionSummary(template.Name,'Awaiting Incoming Event with any Type and value',null,actionDisplay(action));		
	}
}

function onActionLoad_Message(template,action) { 
	//AVAILABLE RESOURCES
	/*
		"Type":"MODULETITLE",  ' +
		"Value":"Apples"' +
	*/	
	//ASSIGN PROPERTIES
	vtype = $('frmActionMessage_Type');
	vvalue = $('frmActionMessage_Value');

	sysSetText(vtype,action.Parameters.Type);
	sysSetText(vvalue,action.Parameters.Value);
}
function onActionSave_Message(template,action) { 
	//AVAILABLE RESOURCES
	/*
		"Type":"MODULETITLE",  ' +
		"Value":"Apples"' +
	*/	
	//ASSIGN PROPERTIES
	vtype = $('frmActionMessage_Type');
	vvalue = $('frmActionMessage_Value');

	action.Parameters.Type = sysGetText(vtype);
	action.Parameters.Value = sysGetText(vvalue);
	return true;
}

//ASSIGNMENT
function onActionPrint_Assignment(template,action) { 
return sysActionSummary(template.Name,action.Parameters.Type + '.' + action.Parameters.Name + ' to ' + action.Parameters.Value,null,actionDisplay(action));
}


function onActionLoad_Assignment(template,action) { 
	//AVAILABLE RESOURCES
	/*
		"Type":"MODULETITLE",  ' +
		"Name":"WhatILove",  ' +
		"Value":"Apples",' +
		"SkipProcessing":"True",' +
		"AssignmentType":"1"' +	
	*/	
	//ASSIGN PROPERTIES
	vtype = $('frmActionAssignment_VariableType');
	vname = $('frmActionAssignment_Name');
	vignore = $('frmActionAssignment_IgnoreTags');
	vvalue = $('frmActionAssignment_Value');
	vatype = document.getElementsByName('frmActionAssignment_AssignmentType');

	sysSetSelect(vtype,ActionParameter_ClearFormatting(action.Parameters.Type));
	sysSetText(vvalue,action.Parameters.Value);
	sysSetText(vname,action.Parameters.Name);
	sysSetCheck(vignore,action.Parameters.SkipProcessing);
	sysSetRadio(vatype,action.Parameters.AssignmentType);
	
	RibbonEditor.Generate(vvalue);
}
function onActionSave_Assignment(template,action) { 
	//AVAILABLE RESOURCES
	/*
		"Type":"MODULETITLE",  ' +
		"Name":"WhatILove",  ' +
		"Value":"Apples",' +
		"SkipProcessing":"True",' +
		"AssignmentType":"1"' +	
	*/	
	//ASSIGN PROPERTIES
	vtype = $('frmActionAssignment_VariableType');
	vname = $('frmActionAssignment_Name');
	vignore = $('frmActionAssignment_IgnoreTags');
	vvalue = $('frmActionAssignment_Value');
	vatype = document.getElementsByName('frmActionAssignment_AssignmentType');

	action.Parameters.Type = sysGetSelect(vtype);
	action.Parameters.Name = sysGetText(vname);
	action.Parameters.Value = sysGetText(vvalue);
	action.Parameters.SkipProcessing = sysGetCheck(vignore);
	action.Parameters.AssignmentType = sysGetRadio(vatype);
	return true;
}


//EMAIL
function onActionPrint_Email(template,action) { 
return sysActionSummary(template.Name,(action.Parameters.Format) + ' ' + action.Parameters.Subject + ' from ' + action.Parameters.From + ' to ' + action.Parameters.To,null,actionDisplay(action));
}
function onActionLoad_Email(template,action) { 
	vfrom = $('frmActionEmail_From');
	vto = $('frmActionEmail_To');
	vcc = $('frmActionEmail_Cc');
	vbcc = $('frmActionEmail_Bcc');
	vformat = document.getElementsByName('frmActionEmail_Format');
	vsubject = $('frmActionEmail_Subject');
	vbody = $('frmActionEmail_Body');
	vtype = $('frmActionEmail_VariableType');
	vname = $('frmActionemail_Name');
	
	vsserver = $('frmActionEmail_SServer');
	vsauth = document.getElementsByName('frmActionEmail_SAuth');
	vsuser = $('frmActionEmail_SUser');
	vspass = $('frmActionEmail_SPass');
	vsssl = $('frmActionEmail_SSSL');
	
	sysSetText(vfrom,action.Parameters.From);
	sysSetText(vto,action.Parameters.To);
	sysSetText(vcc,action.Parameters.Cc);
	sysSetText(vbcc,action.Parameters.Bcc);
	sysSetRadio(vformat,action.Parameters.Format);
	sysSetText(vsubject,action.Parameters.Subject);
	sysSetText(vbody,action.Parameters.Body);
	sysSetSelect(vtype,action.Parameters.ResultVariableType);
	sysSetText(vname,action.Parameters.ResultVariableName);
	
	sysSetText(vsserver,action.Parameters.SMTPServer);
	sysSetRadio(vsauth,action.Parameters.SMTPAuthType);
	sysSetText(vsuser,action.Parameters.SMTPUsername);
	sysSetText(vspass,action.Parameters.SMTPPassword);
	sysSetCheck(vsssl,action.Parameters.SMTPSSL);
	
	RibbonEditor.Generate(vbody);
}
function onActionSave_Email(template,action) { 
/*
                  "From":"eryy",
                  "To":"tyrtytry",
                  "Cc":"rtyrty",
                  "Bcc":"rtyrtyrt",
                  "Format":"text",
                  "Subject":"rtyrty",
                  "Body":"rtyrtyrty",
                  "ResultVariableType":"<Session>",
                  "ResultVariableName":"rtyrty"
*/
	vfrom = $('frmActionEmail_From');
	vto = $('frmActionEmail_To');
	vcc = $('frmActionEmail_Cc');
	vbcc = $('frmActionEmail_Bcc');
	vformat = document.getElementsByName('frmActionEmail_Format');
	vsubject = $('frmActionEmail_Subject');
	vbody = $('frmActionEmail_Body');
	vtype = $('frmActionEmail_VariableType');
	vname = $('frmActionemail_Name');
	
	vsserver = $('frmActionEmail_SServer');
	vsauth = document.getElementsByName('frmActionEmail_SAuth');
	vsuser = $('frmActionEmail_SUser');
	vspass = $('frmActionEmail_SPass');
	vsssl = $('frmActionEmail_SSSL');
	
	action.Parameters.From = sysGetText(vfrom);
	action.Parameters.To = sysGetText(vto);
	action.Parameters.Cc = sysGetText(vcc);
	action.Parameters.Bcc = sysGetText(vbcc);
	action.Parameters.Format = sysGetRadio(vformat);
	action.Parameters.Subject = sysGetText(vsubject);
	action.Parameters.Body = sysGetText(vbody);
	action.Parameters.ResultVariableType = sysGetSelect(vtype);
	action.Parameters.ResultVariableName = sysGetText(vname);	
	
	action.Parameters.SMTPServer = sysGetText(vsserver);
	action.Parameters.SMTPUsername =sysGetText(vsuser);
	action.Parameters.SMTPPassword = sysGetText(vspass);
	action.Parameters.SMTPAuthType = sysGetRadio(vsauth);
	action.Parameters.SMTPSSL = sysGetCheck(vsssl);
	
	return true;
}

//LOG
function onActionPrint_Log(template,action) { 
return sysActionSummary(template.Name,action.Parameters.Name + ' with a value of ' + action.Parameters.Value,null,actionDisplay(action));
}
function onActionLoad_Log(template,action) { 
	vname = $('frmActionLog_Name');
	vvalue = $('frmActionLog_Value');

	sysSetText(vname,action.Parameters.Name);
	sysSetText(vvalue,action.Parameters.Value);
	
	RibbonEditor.Generate(vvalue);
}
function onActionSave_Log(template,action) { 
	//AVAILABLE RESOURCES
	/*
		"Name":"",  ' +
		"Value":"",' +
	*/	
	//ASSIGN PROPERTIES
	vname = $('frmActionLog_Name');
	vvalue = $('frmActionLog_Value');

	action.Parameters.Name = sysGetText(vname);
	action.Parameters.Value = sysGetText(vvalue);
	return true;
}

//SEARCH
function onActionPrint_Search(template,action) { 
return sysActionSummary(template.Name,'Search Integration',null,actionDisplay(action));
}
function onActionLoad_Search(template,action) { 
	v1 = $('txtSearchQuery'); //rt
	v2 = $('txtSearchTitle');
	v3 = $('txtSearchAuthor');
	v4 = $('txtSearchDate');
	v5 = $('txtSearchQuerystring');
	v6 = $('txtSearchKey');
	v7 = $('txtSearchDescription'); //rt
	v8 = $('txtSearchContent'); //rt

	sysSetText(v1,action.Parameters.Query);
	sysSetText(v2,action.Parameters.Title);
	sysSetText(v3,action.Parameters.Author);
	sysSetText(v4,action.Parameters.Date);
	sysSetText(v5,action.Parameters.Querystring);
	sysSetText(v6,action.Parameters.Key);
	sysSetText(v7,action.Parameters.Description);
	sysSetText(v8,action.Parameters.Content);
	//RibbonEditor.Generate(v1);
	//RibbonEditor.Generate(v7);
	//RibbonEditor.Generate(v8);
}
function onActionSave_Search(template,action) { 
	//ASSIGN PROPERTIES
	v1 = $('txtSearchQuery'); //rt
	v2 = $('txtSearchTitle');
	v3 = $('txtSearchAuthor');
	v4 = $('txtSearchDate');
	v5 = $('txtSearchQuerystring');
	v6 = $('txtSearchKey');
	v7 = $('txtSearchDescription'); //rt
	v8 = $('txtSearchContent'); //rt

	action.Parameters.Query = sysGetText(v1);
	action.Parameters.Title = sysGetText(v2);
	action.Parameters.Author = sysGetText(v3);
	action.Parameters.Date = sysGetText(v4);
	action.Parameters.Querystring = sysGetText(v5);
	action.Parameters.Key = sysGetText(v6);
	action.Parameters.Description = sysGetText(v7);
	action.Parameters.Content = sysGetText(v8);
	return true;
}

//TEMPLATE
function onActionPrint_Template(template,action) { 
var summary = '';
var tname = '';
switch(action.Parameters.Type)
{
	case 'Query-Query':
		tname='Query';
		break;
	case 'Detail-Detail':
		tname='Detail';
	break;
	case 'Group-Header':
		if (action.Parameters.GroupIndex==null)
			action.Parameters.GroupIndex='';
		if (action.Parameters.GroupStatement==null)
			action.Parameters.GroupStatement='';	
		tname='Header [' + action.Parameters.GroupIndex + '] ' + (action.Parameters.GroupStatement.length > 0?' "' + action.Parameters.GroupStatement + '"':'');
	break;
	case 'Group-Footer':
		if (action.Parameters.GroupIndex==null)
			action.Parameters.GroupIndex='';
		if (action.Parameters.GroupStatement==null)
			action.Parameters.GroupStatement='';	
		tname='Footer [' + action.Parameters.GroupIndex + '] ' + (action.Parameters.GroupStatement.length > 0?' "' + action.Parameters.GroupStatement + '"':'');
	break;
	case 'Detail-NoQuery':
		tname='No Query';
	break;
	case 'Detail-NoResults':
		tname='No Results';
	break;
	case 'Detail-Alternate':
		tname='Detail (Alternate)';
	break;
}
summary+=' - ' + action.Parameters.Value;
return sysActionSummary(tname + ' ' + template.Name,summary,null,actionDisplay(action));
}

function onActionLoad_Template(template,action) { 
	//ASSIGN PROPERTIES
	var vtype = $('frmActionTemplate_Type');
	var vgs = $('frmActionTemplate_GroupStatement');
	var vgi = $('frmActionTemplate_GroupIndex');
	var vvalue = $('frmActionTemplate_Value');
	var vconn = $('frmActionTemplate_QueryConnection');
	var vfilt = $('frmActionTemplate_QueryFilter');
    var vct = $('frmActionTemplate_QueryCacheTime');
    var vcn = $('frmActionTemplate_QueryCacheName');
    var vcs = $('frmActionTemplate_QueryCacheShared');
    
	sysSetSelect(vtype,action.Parameters.Type);
	sysSetText(vgs,action.Parameters.GroupStatement);
	sysSetText(vgi,action.Parameters.GroupIndex);
	sysSetText(vvalue,action.Parameters.Value);
	sysSetText(vconn,action.Parameters.Connection);
	sysSetText(vfilt,action.Parameters.Filter);
	sysSetText(vct,action.Parameters.CacheTime);
	sysSetText(vcn,action.Parameters.CacheName);
	sysSetCheck(vcs,action.Parameters.CacheShared);
		
	onfrmActionTemplate_TypeToggle();
	
	RibbonEditor.Generate(vvalue);
}
function onfrmActionTemplate_TypeToggle()
{
	var vmeth = $('frmActionTemplate_Type');
	switch(sysGetSelect(vmeth).toLowerCase().substring(0,1))
	{
		case 'g':
			showBlock($('dGroup0'));showBlock($('dGroup1'));showBlock($('dGroup2'));showBlock($('dGroup3'));
			hideBlock($('dGroup4'));hideBlock($('dGroup5'));hideBlock($('dGroup6'));hideBlock($('dGroup7'));hideBlock($('dGroup8'));hideBlock($('dGroup9'));
			break;
		case 'q':
			hideBlock($('dGroup0'));hideBlock($('dGroup1'));hideBlock($('dGroup2'));hideBlock($('dGroup3'));
			showBlock($('dGroup4'));showBlock($('dGroup5'));showBlock($('dGroup6'));showBlock($('dGroup7'));showBlock($('dGroup8'));showBlock($('dGroup9'));
			break;
		default:
			hideBlock($('dGroup0'));hideBlock($('dGroup1'));hideBlock($('dGroup2'));hideBlock($('dGroup3'));
			hideBlock($('dGroup4'));hideBlock($('dGroup5'));hideBlock($('dGroup6'));hideBlock($('dGroup7'));hideBlock($('dGroup8'));hideBlock($('dGroup9'));
			break;
	}
}
function onActionSave_Template(template,action) { 
	//ASSIGN PROPERTIES
	var vtype = $('frmActionTemplate_Type');
	var vgs = $('frmActionTemplate_GroupStatement');
	var vgi = $('frmActionTemplate_GroupIndex');
	var vvalue = $('frmActionTemplate_Value');
	var vconn = $('frmActionTemplate_QueryConnection');
	var vfilt = $('frmActionTemplate_QueryFilter');
    var vct = $('frmActionTemplate_QueryCacheTime');
    var vcn = $('frmActionTemplate_QueryCacheName');
    var vcs = $('frmActionTemplate_QueryCacheShared');
    
	action.Parameters.Type = sysGetSelect(vtype);
	action.Parameters.GroupStatement = sysGetText(vgs);
	action.Parameters.GroupIndex = sysGetText(vgi);
	action.Parameters.Value = sysGetText(vvalue);
	action.Parameters.Connection = sysGetText(vconn);
	action.Parameters.Filter = sysGetText(vfilt);	
	action.Parameters.CacheTime = sysGetText(vct);
	action.Parameters.CacheName = sysGetText(vcn);
	action.Parameters.CacheShared = sysGetCheck(vcs);
		
	return true;
}
function onActionPrint_Variable(template,action) { 
return sysActionSummary(template.Name,action.Parameters.QueryTarget + ' from ' + action.Parameters.VariableType + ' ' + action.Parameters.QuerySource,null,actionDisplay(action));
}
function onActionLoad_Variable(template,action) { 
	//ASSIGN PROPERTIES
	sysSetSelect($('frmActionVariable_VariableType'),action.Parameters.VariableType);
	sysSetSelect($('frmActionVariable_VariableDataType'),action.Parameters.VariableDataType);
	sysSetText($('frmActionVariable_Formatters'),action.Parameters.Formatters);	
	sysSetText($('frmActionVariable_Source'),action.Parameters.QuerySource);
	sysSetText($('frmActionVariable_Target'),action.Parameters.QueryTarget);
	sysSetText($('frmActionVariable_Left'),action.Parameters.QueryTargetLeft);
	sysSetText($('frmActionVariable_Right'),action.Parameters.QueryTargetRight);
	sysSetText($('frmActionVariable_Empty'),action.Parameters.QueryTargetEmpty);
	sysSetRadio(document.getElementsByName('frmActionVariable_PCode'),action.Parameters.EscapeListX);
	sysSetCheck($('frmActionVariable_PSQL'),action.Parameters.Protected);
	sysSetCheck($('frmActionVariable_PHTML'),action.Parameters.EscapeHTML);
}
function onActionSave_Variable(template,action) { 
	//ASSIGN PROPERTIES
	action.Parameters.VariableType = sysGetSelect($('frmActionVariable_VariableType'));
	action.Parameters.VariableDataType = sysGetSelect($('frmActionVariable_VariableDataType'));
    action.Parameters.Formatters = sysGetText($('frmActionVariable_Formatters'));
	action.Parameters.QuerySource = sysGetText($('frmActionVariable_Source'));
	action.Parameters.QueryTarget = sysGetText($('frmActionVariable_Target'));
	action.Parameters.QueryTargetLeft = sysGetText($('frmActionVariable_Left'));
	action.Parameters.QueryTargetRight = sysGetText($('frmActionVariable_Right'));
	action.Parameters.QueryTargetEmpty = sysGetText($('frmActionVariable_Empty'));
	action.Parameters.EscapeListX = sysGetRadio(document.getElementsByName('frmActionVariable_PCode'));
	action.Parameters.Protected = sysGetCheck($('frmActionVariable_PSQL'));
	action.Parameters.EscapeHTML = sysGetCheck($('frmActionVariable_PHTML'));

	return true;
}

//CONDITION IF
function onActionPrint_Condition_If(template,action) { 
	if (action.Parameters.IsAdvanced.toUpperCase()=='TRUE')
	{
		return sysActionSummary(template.Name,action.Parameters.LeftCondition,null,actionDisplay(action));
	}
	else
	{
		return sysActionSummary(template.Name,action.Parameters.LeftCondition + '&nbsp;' + action.Parameters.Operator + '&nbsp;' + action.Parameters.RightCondition,null,actionDisplay(action));
	}
}

function onActionLoad_Condition_If(template,action) { 
	//AVAILABLE RESOURCES
	/*
	"LeftCondition":"\'[EntryID,Q]\'",  ' +
	"RightCondition":"\'\'",  ' +
	"Operator":"<>",  ' +
	"IsAdvanced":"False"  ' +	
	*/	
	
	//ASSIGN PROPERTIES
	vlft = $('frmActionCondition_ConditionLeft');
	vrgt = $('frmActionCondition_ConditionRight');
	vopr = $('frmActionCondition_ConditionOperator');
	vatype = document.getElementsByName('frmActionCondition_ConditionType');

	sysSetText(vlft,action.Parameters.LeftCondition);
	var operatorTemp = action.Parameters.Operator;
	//operatorTemp = operatorTemp.replace(/</g, "&lt;");
	//operatorTemp = operatorTemp.replace(/>/g, "&gt;");
	sysSetSelect(vopr,operatorTemp);
	sysSetText(vrgt,action.Parameters.RightCondition);
	sysSetRadio(vatype,action.Parameters.IsAdvanced);
	
	onfrmActionCondition_ConditionTypeToggle(action.Parameters.IsAdvanced);
}
function onfrmActionCondition_ConditionTypeToggle(selectedvalue) {
	lftCL = $('lfthandLabel');
	lftCP = $('lfthandPrompt');
	rhtCL = $('rhthandLabel');
	rhtCP = $('rhthandPrompt');
	oprCL = $('operatorLabel');
	oprCP = $('operatorPrompt');	
	if (selectedvalue.toUpperCase()!='TRUE')
	{
		showBlock(lftCL);
		showBlock(lftCP);
		showBlock(rhtCL);
		showBlock(rhtCP);
		showBlock(oprCL);
		showBlock(oprCP);
		lftCL.innerHTML = 'Left Condition';
	}
	else
	{
		showBlock(lftCL);
		showBlock(lftCP);
		hideBlock(rhtCL);
		hideBlock(rhtCP);
		hideBlock(oprCL);
		hideBlock(oprCP);
		lftCL.innerHTML = 'Condition';	
	}
}
function onActionSave_Condition_If(template,action) { 
	//AVAILABLE RESOURCES
	/*
	"LeftCondition":"\'[EntryID,Q]\'",  ' +
	"RightCondition":"\'\'",  ' +
	"Operator":"<>",  ' +
	"IsAdvanced":"False"  ' +	
	*/	
	
	//ASSIGN PROPERTIES
	vlft = $('frmActionCondition_ConditionLeft');
	vrgt = $('frmActionCondition_ConditionRight');
	vopr = $('frmActionCondition_ConditionOperator');
	vatype = document.getElementsByName('frmActionCondition_ConditionType');

	action.Parameters.IsAdvanced = sysGetRadio(vatype);
	action.Parameters.LeftCondition = sysGetText(vlft);
	action.Parameters.Operator = sysGetSelect(vopr);
	action.Parameters.RightCondition = sysGetText(vrgt);
	return true;
}

function onActionSave_Condition_Else(template,action) { 
	//ASSIGN PROPERTIES
	return true;
}

//CONDITION ELSE-IF
function onActionPrint_Condition_Else(template,action) {

//CONDITION ELSE
		return sysActionSummary(template.Name,"",null,actionDisplay(action));
 }


//CONDITION SELECT
function onActionPrint_Condition_Select(template,action) { 
return sysActionSummary(template.Name,' "' + action.Parameters.Value + '"',null,actionDisplay(action));
}
function onActionLoad_Condition_Select(template,action) { 
	vvalue = $('frmActionSelect_Value');
	sysSetText(vvalue,action.Parameters.Value);
}
function onActionSave_Condition_Select(template,action) { 
	vvalue = $('frmActionSelect_Value');
	action.Parameters.Value = sysGetText(vvalue);
	return true;
}

//CONDITION CASE
function onActionPrint_Condition_Case(template,action) { 
return sysActionSummary(template.Name,'[' + action.Parameters.Condition + ']',null,actionDisplay(action));
}
function onActionLoad_Condition_Case(template,action) { 
	vvalue = $('frmActionCase_Condition');
	sysSetText(vvalue,action.Parameters.Condition);
}
function onActionSave_Condition_Case(template,action) { 
	vvalue = $('frmActionCase_Condition');
	action.Parameters.Condition = sysGetText(vvalue);
	return true;
}

function onActionDelete(template,action) { 
return window.confirm('Are you sure you want to delete this action?');
}


//LOOP UNTIL
function onActionPrint_Loop(template,action) { 
	if (action.Parameters.IsAdvanced.toUpperCase()=='TRUE')
	{
		return sysActionSummary(template.Name + " Until",action.Parameters.LeftCondition,null,actionDisplay(action));
	}
	else
	{
		return sysActionSummary(template.Name + " Until",action.Parameters.LeftCondition + '&nbsp;' + action.Parameters.Operator + '&nbsp;' + action.Parameters.RightCondition,null,actionDisplay(action));
	}
}

function onActionLoad_Loop(template,action) { 
	//AVAILABLE RESOURCES
	/*
	"LeftCondition":"\'[EntryID,Q]\'",  ' +
	"RightCondition":"\'\'",  ' +
	"Operator":"<>",  ' +
	"IsAdvanced":"False"  ' +	
	*/	
	
	//ASSIGN PROPERTIES
	vlft = $('frmActionCondition_ConditionLeft');
	vrgt = $('frmActionCondition_ConditionRight');
	vopr = $('frmActionCondition_ConditionOperator');
	vatype = document.getElementsByName('frmActionCondition_ConditionType');

	sysSetText(vlft,action.Parameters.LeftCondition);
	var operatorTemp = action.Parameters.Operator;
	//operatorTemp = operatorTemp.replace(/</g, "&lt;");
	//operatorTemp = operatorTemp.replace(/>/g, "&gt;");
	sysSetSelect(vopr,operatorTemp);
	sysSetText(vrgt,action.Parameters.RightCondition);
	sysSetRadio(vatype,action.Parameters.IsAdvanced);
	
	onfrmActionCondition_ConditionTypeToggle(action.Parameters.IsAdvanced);
}

function onActionSave_Loop(template,action) { 
	//AVAILABLE RESOURCES
	/*
	"LeftCondition":"\'[EntryID,Q]\'",  ' +
	"RightCondition":"\'\'",  ' +
	"Operator":"<>",  ' +
	"IsAdvanced":"False"  ' +	
	*/	
	
	//ASSIGN PROPERTIES
	vlft = $('frmActionCondition_ConditionLeft');
	vrgt = $('frmActionCondition_ConditionRight');
	vopr = $('frmActionCondition_ConditionOperator');
	vatype = document.getElementsByName('frmActionCondition_ConditionType');

	action.Parameters.IsAdvanced = sysGetRadio(vatype);
	action.Parameters.LeftCondition = sysGetText(vlft);
	action.Parameters.Operator = sysGetSelect(vopr);
	action.Parameters.RightCondition = sysGetText(vrgt);
	return true;
}

//REGION
function onActionPrint_Region(template,action)
{
var cdbg = '';
var csea = '';
var csex = '';
var csei = ''
if (typeof action.Parameters.skipDebug == 'undefined' || action.Parameters.skipDebug == null)
    action.Parameters.skipDebug = 'False';
if (action.Parameters.skipDebug.toLowerCase()=='true')
    cdbg = '(Debug Disabled) ';
if (typeof action.Parameters.includeSearch == 'undefined' || action.Parameters.includeSearch == null)
    action.Parameters.includeSearch = 'False';
if (action.Parameters.includeSearch.toLowerCase()=='true')
    csea = '(Include In Search) ';
if (typeof action.Parameters.includeExport == 'undefined' || action.Parameters.includeExport == null)
    action.Parameters.includeExport = 'False';
if (action.Parameters.includeExport.toLowerCase()=='true')
    csex = '(Include In Export) ';
if (typeof action.Parameters.includeImport == 'undefined' || action.Parameters.includeImport == null)
    action.Parameters.includeImport = 'False';
if (action.Parameters.includeImport.toLowerCase()=='true')
    csei = '(Include In Import) ';

return sysActionSummary(template.Name,' ' + cdbg + csea + csex + csei + '- ' + action.Parameters.Name + (action.Parameters.RenderType=='1'?'(Page Load Only)':'') + (action.Parameters.RenderType=='2'?'(AJAX Request Only)':''),"Region",actionDisplay(action));
}
function onActionSave_Region(template,action)
{
	vname = $('frmActionRegion_Name');
	vcdbg = $('frmActionRegion_skipDebug');
	vinse = $('frmActionRegion_includeSearch');
	vinex = $('frmActionRegion_includeExport');
	vinim = $('frmActionRegion_includeImport');
	vrtype = document.getElementsByName('frmActionRegion_Type');
	
	action.Parameters.Name = sysGetText(vname);
	action.Parameters.RenderType = sysGetRadio(vrtype);
	action.Parameters.skipDebug = sysGetCheck(vcdbg);
	action.Parameters.includeSearch = sysGetCheck(vinse);
	action.Parameters.includeExport = sysGetCheck(vinex);
	action.Parameters.includeImport = sysGetCheck(vinim);
	return true;
}
function onActionLoad_Region(template,action)
{
	vname = $('frmActionRegion_Name');
	vrtype = document.getElementsByName('frmActionRegion_Type');
    vcdbg = $('frmActionRegion_skipDebug');
    vinse = $('frmActionRegion_includeSearch');
	vinex = $('frmActionRegion_includeExport');
	vinim = $('frmActionRegion_includeImport');
    
	sysSetText(vname,action.Parameters.Name);
	sysSetRadio(vrtype,action.Parameters.RenderType);
	sysSetCheck(vcdbg,action.Parameters.skipDebug);
	sysSetCheck(vinse,action.Parameters.includeSearch);
	sysSetCheck(vinex,action.Parameters.includeExport);
	sysSetCheck(vinim,action.Parameters.includeImport);
}

/*ON CLICK HANDLERS*/
function onClick_Actions_Assign() {
	//ADD A NEW ACTION
	AddAction("Action-Assignment",{"Type":"","Name":"Variable Name","Value":"Enter Value Here","SkipProcessing":"False","AssignmentType":"0"});
}
function onClick_Actions_Loop() {
	//ADD A NEW ACTION
	AddAction("Action-Loop",{"LeftCondition":"Left Condition","RightCondition":"Right Condition","Operator":"=","IsAdvanced":"False"});
}
function onClick_Actions_If() {
	//ADD A NEW ACTION
	AddAction("Condition-If",{"LeftCondition":"Left Condition","RightCondition":"Right Condition","Operator":"=","IsAdvanced":"False"});
}
function onClick_Actions_Else_If() {
	//ADD A NEW ACTION
	AddAction("Condition-ElseIf",{"LeftCondition":"Left Condition","RightCondition":"Right Condition","Operator":"=","IsAdvanced":"False"});
}
function onClick_Actions_Else() {
	//ADD A NEW ACTION
	AddAction("Condition-Else",{});
}
 function ReloadProperty()
 {
	_LoadProperty(sysProperty_ListItem);
	owsSelect.expand(sysProperty_ObjectItem.GUID);	
 }
function onClick_Action_Convert(castTo) {
	var canChange = false;
    switch(castTo) {
        case 'Condition-If':
		case 'Condition-ElseIf':
			switch(sysProperty_ObjectItem.ActionType)
            {
                case 'Condition-If':
				case 'Condition-ElseIf':
					canChange = true;
					break;
				case 'Condition-Else':
					canChange = true;
					sysProperty_ObjectItem.Parameters = {"LeftCondition":"Left Condition","RightCondition":"Right Condition","Operator":"=","IsAdvanced":"False"};
					break;
                break;
            }
			break;
		case 'Condition-Else':
			switch(sysProperty_ObjectItem.ActionType)
            {
                case 'Condition-If':
				case 'Condition-ElseIf':
				case 'Condition-Else':
					canChange = true;
					sysProperty_ObjectItem.Parameters = {};
					break;
                break;
            }
			break;
        break;
    }
	if (canChange)
	{
		sysProperty_ObjectItem.ActionType = castTo;
		ReloadProperty();	
	}
}
function onClick_Actions_Comment() {
	//ADD A NEW ACTION
	AddAction("Action-Comment",{"Value":"Place Comment Here"});
}
function onClick_Actions_Delay() {
	//ADD A NEW ACTION
	AddAction("Action-Delay",{"Type":"second","Value":"0"});
}
function onClick_Actions_Message() {
	//ADD A NEW ACTION
	AddAction("Message",{"Type":"Message Type","Value":"Enter Value Here"});
}
function onClick_Actions_Query() {
	//ADD A NEW ACTION
	AddAction("Action-Execute",{"Name":"New Query","Query":"Enter Query Here","IsProcess":"False","Connection":""});
}
function onClick_Actions_Log() {
	//ADD A NEW ACTION
	AddAction("Action-Log",{"Name":"New Event","Value":"Enter Event Information Here"});
}
function onClick_Actions_Redirect() {
	//ADD A NEW ACTION
	AddAction("Action-Redirect",{"Type":"Link","Link":"http://Link","PageID":""});
}
function onClick_Actions_Email() {
	//ADD A NEW ACTION
	AddAction("Action-Email",{"From":"From Address","To":"To Address","Cc":"Cc Address","Bcc":"Bcc Address","Format":"HTML","Subject":"Enter Subject Here","Body":"Place Body Content Here","ResultVariableType":"","ResultVariableName":"","SMTPServer":"","SMTPUsername":"","SMTPPassword":"","SMTPAuthType":"","SMTPSSL":""});
}
function onClick_Actions_File() {
	//ADD A NEW ACTION
	AddAction("Action-File",{"SourceType":"","SourceVariableType":"","Source":"","SourceQuery":"","SourceConnection":"","DestinationType":"","DestinationVariableType":"","DestinationResponseType":"","SQLFirstRow":"False","ColumnMappings": "","DestinationQuery":"","RunAsProcess":"False","ProcessName":"","Destination":"","TransformType":"","ImageTransformType":"","ImageFont":"","ImageSize":"","ImageSizeType":"","ImageColor":"","ImageBGColor":"","ImageWidth":"","ImageWidthType":"px","ImageRotation":"","ImageHeight":"","ImageHeightType":"px","ImageQuality":"90","XMLReadPath":"","XMLWritePath":"","FileTask":""});
}
function onClick_Actions_Filter() {
	//ADD A NEW ACTION
	AddAction("Action-Filter",{"Options": ""});
}
function onClick_Actions_Output() {
	//ADD A NEW ACTION
	AddAction("Action-Output",{"OutputType":"complete delimited","Filename":"New Output File","Delimiter":","});
}
function onClick_Actions_Input() {
	//ADD A NEW ACTION
	AddAction("Action-Input",{"URL":"New Input","Querystring":"","Data":"","Headers":"","ContentType":"","Method":"","VariableType":"","VariableName":"","InputFormat":"","XPath":"","AuthenticationType":"","Domain":"","Username":"","Password":"","SoapAction":"","SoapResult":""});
}
function onClick_Actions_Select() {
	//ADD A NEW ACTION
	AddAction("Condition-Select",{"Value":"Select Value to compare"});
}
function onClick_Actions_Case() {
	//ADD A NEW ACTION
	AddAction("Condition-Case",{"Condition":"Condition to evaluate"});
}
function onClick_Actions_Search() {
	//ADD A NEW ACTION
	AddAction("Action-Search",{"Query":"Query to obtain the search items","Title":"The Title of the Items","Author":"The User ID of the author","Date":"Recent Updated Date","Querystring":"Specific Querystring parameters to return to this result","Key":"Primary Key value of the item","Description":"Visible description of the item","Content":"Searched content of the item"});
}
function onClick_Actions_Region() {
	AddAction("Action-Region",{"Name":"New Region","RenderType":"0","skipDebug":"False","includeSearch":"False"});
}
function onClick_Actions_Goto() {
	AddAction("Action-Goto",{"Name":"","Region":"Target Region"});
}
function onClick_Actions_Template() {
	AddAction("Template",{"Type":"Query-Query","GroupStatement":"","GroupIndex":"","Value":"New Template"});
}
function onClick_Actions_Header() {
	AddAction("Template",{"Type":"Group-Header","GroupStatement":"","GroupIndex":"","Value":"New Template"});
}
function onClick_Actions_Footer() {
	AddAction("Template",{"Type":"Group-Footer","GroupStatement":"","GroupIndex":"","Value":"New Template"});
}
function onClick_Actions_Detail() {
	AddAction("Template",{"Type":"Detail-Detail","GroupStatement":"","GroupIndex":"","Value":"New Template"});
}
function onClick_Actions_Variable() {
	AddAction("Template-Variable",{"VariableType":"<Action>","VariableDataType":"Any","Formatters":"","QuerySource":"Source Value","QueryTarget":"New Query Variable","QueryTargetLeft":"'","QueryTargetRight":"'","QueryTargetEmpty":"","EscapeListX":"0","Protected":"true","EscapeHTML":"true"});
}

/*GENERAL PROPERTIES*/
function onExport(template,action)
{
	var vtxt = $('txtConfigSrc');
	var src = configurationToString();
	//src = src.replace(/&lt;/g,"&amp;lt;");
	//src = src.replace(/&gt;/g,"&amp;gt;");
	sysSetText(vtxt,src);
	RibbonEditor.Generate(vtxt);
}

function onImportSave(template,action)
{
 var vtxt = $('txtConfigSrc');
 var IsNewConfigId = document.getElementById('preserveConfigIdNo').checked;

 src = sysGetText(vtxt);
 var xmlFormat = CheckXml(src.substring(0,10));
 if (xmlFormat){
  getJsonConfigFromXml(src); 
  return false;
 }
 else
 {
	 try
	 {
	  eval('configuration = ' + src + ';');
	  if (configuration.ConfigurationID == undefined || configuration.ConfigurationID.length == 0)
	  {
	     IsNewConfigId = true;
	  }
	  if (IsNewConfigId)
      {
         configuration.ConfigurationID = Bi4ce.GenerateGuid.newGuid('N').toString()
      }
	 }
	 catch(ex)
	 {
	  alert('This configuration failed to import properly');
	  return false;
	 }
 }
 return true;
}

function CheckXml(src)
{
 var filter =  /^(<)/;
 if (filter.test(src)){
  return true;
 }
 else{
  return false;
 }
}


function onImport(template,action)
{
	var vtxt = $('txtConfigSrc');
	sysSetText(vtxt,"Place Configuration Here");
	RibbonEditor.Generate(vtxt);
}
function onGeneralLoad(template,action)
{
	//ASSIGN PROPERTIES
	var vrpp = $('txtRPP');
	var vshowall = $('chkShowAll');
	var vcrpp = $('chkRPP');
	var vpageselect = $('chkPageSelection');
	var vcustompage = $('chkCustomPaging');
	var vcra = $('chkAlphabet');
	var vMulti = $('chkMultiSort');
	var vAjax = $('chkEnableAjax');
	var vAjaxM = $('chkEnableAjaxManual');
	var vAjaxPH = $('chkAjaxPageHistory');
	var vAjaxCPH = $('txtAjaxPageHistory');
	var vnoOWSCreate = $('chknoOWSCreate');
	var vAjaxCS = $('chkAjaxCustomStatus');
	var vAjacCP = $('chkAjaxCustomPaging');
	var vAjaxAuto = $('txtAjaxAutoRefresh');
	var vAjaxPaging = $('chkEnableAjaxPaging');
	var vju = $('chkJavascriptUtilities');
	var vjv = $('chkJavascriptValidation');
	var vmodulemessage = $('txtModuleMessageType');
	var vjsoncomplete = $('txtJavascriptOnComplete');
	var vdebugall = $('chkDebugging_All');
	var vdebugeditors = $('chkDebugging_Editors');
	var vdebugadministrators = $('chkDebugging_Administrators');
	var vdebugsupers = $('chkDebugging_SuperUsers');
	var vdebugignorer = $('chkDebugging_IgnoreRedirections');
	var vdebugignores = $('chkDebugging_IgnoreSubquery');
	var vedituser =  document.getElementsByName('chkAdmin');
	var vexplicit = $('chkUseExplicit');
	var vadvanced = $('chkUseAdvancedParsing');
	var vcompound = $('chkUseCompoundIIFConditions');
	var vexcel = $('chkExcelExport');
	var vmaster = $('chkMasterTemplate');
	var vgo = $('chkUseGoSplit');
	var vname = $('txtName');
	var vlistx = $('chkUseListX');
	var vsilverlight = $('chkUseSilverlight');
	var vlblConfigId = $('lblConfigurationId');
	
	sysSetText(vname,configuration.Name);
	vlblConfigId.innerHTML = '<span id="lblConfigIDValue">'+configuration.ConfigurationID+'</span> <a href="#" onclick="onShowConfigIDHelp(this);return false;">(How to use this?)</a><span id="hlpConfigId"></span>';
	sysSetText(vrpp,configuration.recordsPerPage);
	sysSetCheck(vshowall,configuration.showAll);
	sysSetCheck(vcrpp,configuration.enableRecordsPerPage);
	sysSetCheck(vpageselect,configuration.enablePageSelection);
	sysSetCheck(vcustompage,configuration.enableCustomPaging);
	sysSetCheck(vcra,configuration.enableAlphaFilter);
	sysSetCheck(vMulti,configuration.enableMultipleColumnSorting);
	sysSetCheck(vAjax,configuration.enableAJAX);
	sysSetCheck(vAjaxM,configuration.enableAJAXManual);
	sysSetCheck(vAjaxPaging,configuration.enableAJAXPaging);
	sysSetCheck(vAjaxPH,configuration.enableAJAXPageHistory);
	sysSetText(vAjaxCPH, configuration.customAJAXPageHistory);
	sysSetText(vnoOWSCreate, configuration.noOWSCreate);
	sysSetCheck(vAjaxCS,configuration.enableAJAXCustomStatus);
	sysSetCheck(vAjacCP,configuration.enableAJAXCustomPaging);
	sysSetText(vAjaxAuto,configuration.autoRefreshInterval);
	sysSetCheck(vju,configuration.includeJavascriptUtilities);
	sysSetCheck(vjv,configuration.includeJavascriptValidation);
	sysSetText(vmodulemessage,configuration.ModuleCommunicationMessageType);
	sysSetText(vjsoncomplete,configuration.javascriptOnComplete);
	sysSetCheck(vdebugall,configuration.enableQueryDebug);
	sysSetCheck(vdebugeditors,configuration.enableQueryDebug_Edit);
	sysSetCheck(vdebugadministrators,configuration.enableQueryDebug_Admin);
	sysSetCheck(vdebugsupers,configuration.enableQueryDebug_Super);
	sysSetCheck(vdebugignorer,configuration.skipRedirectActions);
	sysSetCheck(vdebugignores,configuration.skipSubqueryDebugging);	
    
	sysSetCheck(vexplicit,configuration.useExplicitSystemVariables);
	sysSetCheck(vadvanced,configuration.enableAdvancedParsing);
	sysSetCheck(vcompound,configuration.enableCompoundIIFConditions);
	sysSetCheck(vexcel,configuration.enableExcelExport);
	
	sysSetCheck(vlistx,configuration.disableOpenScript);
	sysSetCheck(vsilverlight,configuration.enableSilverlight);
	sysSetCheck(vgo,configuration.enabledForcedQuerySplit);
	
	if (typeof configuration.javascriptInclude != 'undefined' && configuration.javascriptInclude!=null)
	{
	    var vjinclude = $('name=chkJavascript');
	    for (var ji = 0;ji<configuration.javascriptInclude.length;ji++)
	    {
	         sysSetCheck(vjinclude,'true',configuration.javascriptInclude[ji]);
	    }
	}
	if (configuration!=null && (configuration.enableQueryDebug=='true'||configuration.enableQueryDebug_Edit=='true'||configuration.enableQueryDebug_Admin=='true'||configuration.enableQueryDebug_Super=='true'))
	{
	    $jq.growl("<span class='warning'>Debugging Enabled</span>", '<div class="warning">Debugging is currently enabled. Remember to turn off debugging in production environments.','Images/srotator.gif');
	}	
	/*
		if (configuration.disableOpenScript==undefined)
		configuration.disableOpenScript = true;
	
	sysSetCheck(vlistx,configuration.disableOpenScript);
	sysSetCheck(vsilverlight,configuration.enableSilverlight);
	*/
	//sysSetCheck(vmaster,configuration.recordsPerPage);		
}
function onShowConfigIDHelp(obj)
{
    var hlpConfigId=$('hlpConfigId');
    var lblConfigIdx=$('lblConfigIDValue');
    var lblConfigIdv=lblConfigIdx.innerHTML;
    var glb = "<table border='0'>" +
    "<tr class='HTblRow'><td width='80' class='SubHead'>Embedded Control</td><td class='Normal'>{OPENCONTROL,[ModuleID,System],configurationid,"+lblConfigIdv+",span}</td></tr>" +
    "<tr class='HTblRowAlt'><td width='80' class='SubHead'>DNN&nbsp;Module</td><td class='Normal'>&lt;%@ Control Language=\"vb\" AutoEventWireup=\"false\" Inherits=\"r2i.OWS.Wrapper.DNN.Module\" TargetSchema=\"http://schemas.microsoft.com/intellisense/ie5\" %&gt;<br/>"+
    "&lt;%@ Register Assembly=\"r2i.OWS.Wrapper.DotNetNuke\" Namespace=\"r2i.OWS.Wrapper.DNN\" TagPrefix=\"cc1\" %&gt;<br/><br/>"+
    "&lt;cc1:OpenControl id=\"ows\" ConfigurationID=\""+lblConfigIdv+"\" runat=\"server\"&gt;"+
    "&lt;/cc1:OpenControl&gt;"+
    "</td></tr>" +
    "<tr class='HTblRow'><td width='80' class='SubHead'>DNN&nbsp;Module&nbsp;Settings</td><td class='Normal'>&lt;%@ Control Language=\"vb\" AutoEventWireup=\"false\" Inherits=\"r2i.OWS.Wrapper.DNN.ModuleSettings\" TargetSchema=\"http://schemas.microsoft.com/intellisense/ie5\" %&gt;<br/>"+
    "&lt;%@ Register Assembly=\"r2i.OWS.Wrapper.DotNetNuke\" Namespace=\"r2i.OWS.Wrapper.DNN\" TagPrefix=\"cc1\" %&gt;<br/><br/>"+
    "&lt;cc1:OpenControl id=\"ows\" ConfigurationID=\""+lblConfigIdv+"\" runat=\"server\"&gt;"+
    "&lt;/cc1:OpenControl&gt;"+
    "</td></tr>" +    
    "<tr class='HTblRowAlt'><td width='80' class='SubHead'>DNN&nbsp;Page</td><td class='Normal'>&lt;%@ Page Language=\"vb\" AutoEventWireup=\"false\" Inherits=\"r2i.OWS.Wrapper.DNN.OpenPage\" %&gt;<br/>"+
    "&lt;%@ Register Assembly=\"r2i.OWS.Wrapper.DotNetNuke\" Namespace=\"r2i.OWS.Wrapper.DNN\" TagPrefix=\"cc1\" %&gt;<br/><br/>"+
    "&lt;cc1:OpenControl id=\"ows\" ConfigurationID=\""+lblConfigIdv+"\" runat=\"server\"&gt;"+
    "&lt;/cc1:OpenControl&gt;"+
    "</td></tr>" +       
    "</table>";
    hlpConfigId.innerHTML = glb;
}
function onGeneralSave(template,action)
{
	//ASSIGN PROPERTIES
	var vname = $('txtName');
	var vrpp = $('txtRPP');
	var vshowall = $('chkShowAll');
	var vcrpp = $('chkRPP');
	var vpageselect = $('chkPageSelection');
	var vcustompage = $('chkCustomPaging');
	var vcra = $('chkAlphabet');
	var vMulti = $('chkMultiSort');
	var vAjax = $('chkEnableAjax');
	var vAjaxPaging = $('chkEnableAjaxPaging');
	var vAjaxM = $('chkEnableAjaxManual');
	var vAjaxPH = $('chkAjaxPageHistory');
	var vnoOWSCreate = $('chknoOWSCreate');
	var vAjaxCPH = $('txtAjaxPageHistory');
	var vAjaxCS = $('chkAjaxCustomStatus');
	var vAjacCP = $('chkAjaxCustomPaging');
	var vAjaxAuto = $('txtAjaxAutoRefresh');
	var vju = $('chkJavascriptUtilities');
	var vjv = $('chkJavascriptValidation');
	var vmodulemessage = $('txtModuleMessageType');
	var vjsoncomplete = $('txtJavascriptOnComplete');
	var vdebugall = $('chkDebugging_All');
	var vdebugeditors = $('chkDebugging_Editors');
	var vdebugadministrators = $('chkDebugging_Administrators');
	var vdebugsupers = $('chkDebugging_SuperUsers');
	var vdebugignorer = $('chkDebugging_IgnoreRedirections');
	var vdebugignores = $('chkDebugging_IgnoreSubquery');
	var vedituser =  document.getElementsByName('chkAdmin');
	var vexplicit = $('chkUseExplicit');
	var vadvanced = $('chkUseAdvancedParsing');
	var vcompound = $('chkUseCompoundIIFConditions');
	var vexcel = $('chkExcelExport');
	var vgo = $('chkUseGoSplit');
	var vmaster = $('chkMasterTemplate');
	var vlistx = $('chkUseListX');
	var vsilverlight = $('chkUseSilverlight');
	
	configuration.Name = sysGetText(vname);
	configuration.recordsPerPage = sysGetText(vrpp);
	configuration.showAll = sysGetCheck(vshowall);
	configuration.enableRecordsPerPage = sysGetCheck(vcrpp);
	configuration.enablePageSelection = sysGetCheck(vpageselect);
	configuration.enableCustomPaging = sysGetCheck(vcustompage);
	configuration.enableAlphaFilter = sysGetCheck(vcra);
	configuration.enableMultipleColumnSorting = sysGetCheck(vMulti);
	configuration.enableAJAX = sysGetCheck(vAjax);
	configuration.enableAJAXPaging = sysGetCheck(vAjaxPaging);
	configuration.enableAJAXManual = sysGetCheck(vAjaxM);
	configuration.enableAJAXPageHistory = sysGetCheck(vAjaxPH);
	configuration.noOWSCreate = sysGetCheck(vnoOWSCreate);
	configuration.customAJAXPageHistory = sysGetText(vAjaxCPH);
	configuration.enableAJAXCustomStatus = sysGetCheck(vAjaxCS);
	configuration.enableAJAXCustomPaging = sysGetCheck(vAjacCP);
	configuration.autoRefreshInterval = sysGetText(vAjaxAuto);
	configuration.includeJavascriptUtilities = sysGetCheck(vju);
	configuration.includeJavascriptValidation = sysGetCheck(vjv);
	configuration.ModuleCommunicationMessageType = sysGetText(vmodulemessage);
	configuration.javascriptOnComplete = sysGetText(vjsoncomplete);
	configuration.enableQueryDebug = sysGetCheck(vdebugall);
	configuration.enableQueryDebug_Edit = sysGetCheck(vdebugeditors);
	configuration.enableQueryDebug_Admin = sysGetCheck(vdebugadministrators);
	configuration.enableQueryDebug_Super = sysGetCheck(vdebugsupers);
	configuration.skipRedirectActions = sysGetCheck(vdebugignorer);
	configuration.skipSubqueryDebugging = sysGetCheck(vdebugignores);	

	configuration.useExplicitSystemVariables = sysGetCheck(vexplicit);
	configuration.enableAdvancedParsing = true; //sysGetCheck(vadvanced);
	configuration.enableCompoundIIFConditions = sysGetCheck(vcompound);
	configuration.enableExcelExport = false; //sysGetCheck(vexcel);
	configuration.enabledForcedQuerySplit = sysGetCheck(vgo);
		
	//configuration.... = setGetRadio(vedituser);
	configuration.disableOpenScript = false; //sysGetCheck(vlistx);
	configuration.enableSilverlight = false; //sysGetCheck(vsilverlight);
	
	// don't know why this property is here but clear it off!!!
	configuration.Index = undefined;
	
	configuration.javascriptInclude =  new Array();
	
    var vjinclude = $('name=chkJavascript');
    for (var ji = 0;ji<vjinclude.length;ji++)
    {
        var val = sysGetCheck(vjinclude,vjinclude[ji].value);
        if (val=='true')
            configuration.javascriptInclude.push(vjinclude[ji].value);
    }

	if (configuration!=null && (configuration.enableQueryDebug=='true'||configuration.enableQueryDebug_Edit=='true'||configuration.enableQueryDebug_Admin=='true'||configuration.enableQueryDebug_Super=='true'))
	{
	    $jq.growl("<span class='warning'>Debugging Enabled</span>", '<div class="warning">Debugging is currently enabled. Remember to turn off debugging in production environments.','Images/srotator.gif');
	}

	return true;	
}


function onOpenConfigLoad(template,action)
{
	ows.Create('ConfigurationList',0,20,_OWS_.CModuleId+':ConfigurationList,'+_OWS_.ResourceFile+':Admin.aspx.resx,'+_OWS_.ResourceKey+':OWS.Configuration.List','','',false,-1,false,'ConfigurationList','ConfigListPager');
}

function onOpenConfigSave(template,action)
{
	//AVAILABLE RESOURCES
	
	
	//ASSIGN PROPERTIES
	vConfigId = $('frmConfigName');
	onOpenConfig(sysGetSelect(vConfigId));
	return true;	
}
function onOpenConfig(configId)
{
        window.location.hash = "#config/"+configId;
		//configurationId = configId;	
		getJsonConfig(configId);
}
function actionDisplay(action)
{
return (typeof action._showAll == 'undefined'?false:action._showAll);
}

//MIGRATE TO SEPERATE FILE::::
var sysExportTemplate = { 
   "Name" : "Configuration Import/Export",
   "Code" : "Configuration Export",
   "Description" : "Provide the ability for a developer to place Comments within their Action script",
   "onLoad" : "onExport",
   "onSave" : "onImportSave",
   "Template" :
    "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    " <tr>" +
    "  <td class=\"SubHead\">Configuration&nbsp;</td>" +
	" </tr><tr>" +
	"<td><textarea wrap=offname=txtConfigSrc id=txtConfigSrc style=\"height: 150px; width:100%;\" richtext=true ></textarea></td>" +
    " </tr>" +
    "</table>" 
}
var sysImportTemplate = { 
   "Name" : "Configuration Import/Export",
   "Code" : "Configuration Export",
   "Description" : "Provide the ability for a developer to place Comments within their Action script",
   "onLoad" : "onImport",
   "onSave" : "onImportSave",
   "Template" :
    "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    " <tr>" +
    "  <td class=\"SubHead\">Configuration&nbsp;</td>" +
	" </tr><tr>" +
	"<td><textarea wrap=offname=txtConfigSrc id=txtConfigSrc style=\"height: 150px; width:100%;\" richtext=true ></textarea></td>" +
    " </tr>" +
    " <tr>" +
    "  <td class=\"SubHead\">Preserve Configuration ID&nbsp;</td>" +
	" </tr><tr>" +
	"<td><input type='radio' id='preserveConfigIdYes' name='preserveConfigId' value='yes' checked='checked' />Yes&nbsp;<input type='radio' id='preserveConfigIdNo' name='preserveConfigId' value='no' />No&nbsp;</td>" +
    " </tr>" +
    "</table>"
}
//sysImportTemplate.Template = sysExportTemplate.Template;

var sysFilterOptionTemplate = {
	"onLoad":"onActionLoad_Filter_Option",
	"onSave":"onActionSave_Filter_Option",
	"Template":
	"<center><div class=HSubEditor><table class=Normal style=\"width:300px;\" border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=cm0>Option Text</div></td>" +
     "<td><div id=cm1>" +
     "<input id=\"frmFOOption\" style=\"width:100px;\">" + 
     "</div>" +
     "</td>" +
    "</tr>" + 		
    "<tr>" +
     "<td class=SubHead width=151><div id=cm2>Field</div></td>" +
     "<td><div id=cm3>" +
     "<input id=\"frmFOField\" style=\"width:100px;\">" + 
     "</div>" +
     "</td>" +
    "</tr>" + 	
	"</table></div></center>"
}
var sysFileColumnMapTemplate = {
	"onLoad":"onActionLoad_File_ColumnMap",
	"onSave":"onActionSave_File_ColumnMap",
	"Template":
	"<center><div class=HSubEditor><table class=Normal style=\"width:300px;\" border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Mapping Type</td>" +
     "<td>" +
     "<select name=frmCMType id=frmCMType onchange=\"onfrmCMType_Toggle()\">" +
      "<option value=\"CSV\">Comma Delimited</option>" +
      "<option value=\"FIXED\">Fixed Width</option>" +
     "</select>" +
     "</td>" +
    "</tr>" + 	
    "<tr>" +
     "<td class=SubHead width=151><div id=cm0>Position</div></td>" +
     "<td><div id=cm1>" +
     "<input id=\"frmCMPosition\" style=\"width:100px;\">" + 
     "</div>" +
     "</td>" +
    "</tr>" + 		
    "<tr>" +
     "<td class=SubHead width=151><div id=cm2>Start Column</div></td>" +
     "<td><div id=cm3>" +
     "<input id=\"frmCMStartingColumn\" style=\"width:100px;\">" + 
     "</div>" +
     "</td>" +
    "</tr>" + 	
    "<tr>" +
     "<td class=SubHead width=151><div id=cm4>End Column</div></td>" +
     "<td><div id=cm5>" +
     "<input id=\"frmCMEndingColumn\" style=\"width:100px;\">" + 
     "</div>" +
     "</td>" +
    "</tr>" + 	
    " <TR>" +
    "  <TD class=\"SubHead\" width=\"200\">Name:&nbsp;</TD>" +
    "  <TD class=\"Normal\"><input id=\"frmCMName\" style=\"width:100%;\"></TD>" +
    " </TR>" +		
    " <TR>" +
    "  <TD class=\"SubHead\" width=\"200\">Target:&nbsp;</TD>" +
    "  <TD class=\"Normal\"><input id=\"frmCMTarget\" style=\"width:100%;\"></TD>" +
    " </TR>" +		
    " <TR>" +
    "  <TD class=\"SubHead\" width=\"200\">Type:&nbsp;</TD>" +
    "  <TD class=\"Normal\">" +
     "<select name=frmCMDataType id=frmCMDataType>" +
      "<option value=\"Number\">Number</option>" +
      "<option value=\"Decimal\">Decimal</option>" +
      "<option value=\"Text\">Text</option>" +
      "<option value=\"Date\">Date</option>" +
      "<option value=\"Time\">Time</option>" +
      "<option value=\"DateTime\">DateTime</option>" +	  
     "</select>" +	
	"  </TD>" +
    " </TR>" +	
    " <TR>" +
    "  <TD class=\"SubHead\" width=\"200\">Format&nbsp;</TD>" +
    "  <TD class=\"Normal\"><input id=\"frmCMFormat\" style=\"width:100%;\"></TD>" +
    " </TR>" +		
    " <TR>" +
    "  <TD class=\"SubHead\" width=\"200\">Default&nbsp;</TD>" +
    "  <TD class=\"Normal\"><input id=\"frmCMDefault\" style=\"width:100%;\"></TD>" +
    " </TR>" +		
    " <TR>" +
    "  <TD class=\"SubHead\" width=\"200\">Null&nbsp;</TD>" +
    "  <TD class=\"Normal\"><input id=\"frmCMNull\" style=\"width:100%;\"></TD>" +
    " </TR>" +			
	"</table></div></center>"
}
function sysGeneralTemplate() {
return {
   "Name" : "General Properties",
   "Code" : "General Properties",
   "Description" : "Provide the ability for a developer to place Comments within their Action script",
   "onLoad" : "onGeneralLoad",
   "onSave" : "onGeneralSave",
   "Template" :
    "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    " <TR>" +
    "  <TD class=\"SubHead\" align=\"right\" width=\"200\">Name:&nbsp;</TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input id=\"txtName\" style=\"width:100%;\"></TD>" +
    " </TR>" +	
    " <TR>" +
    "  <TD class=\"SubHead\" align=\"right\" width=\"200\">Id:&nbsp;</TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><i><span id=\"lblConfigurationId\"></span></i></TD>" +
    " </TR>" +	    
    " <TR>" +
    "  <TD class=\"SubHead\" align=\"right\" width=\"200\">Paging Properties:&nbsp;</TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input id=\"txtRPP\" Width=\"50px\">&nbsp;Records per Page</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkShowAll\">Show All Records</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkRPP\" style=\"left-margin: -2px\" >Show Records Per Page</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkPageSelection\" style=\"left-margin: -2px\" >Show Page Selection</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkCustomPaging\" style=\"left-margin: -2px\">Use Custom Paging</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"></TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD class=\"SubHead\" align=\"right\" width=\"200\">Filters and Sorting:&nbsp;</TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkAlphabet\" style=\"left-margin: -2px\">Show Alphabetic Filter</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkMultiSort\" style=\"left-margin: -2px\">Allow Multi-column Sorting</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"></TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD class=\"SubHead\" align=\"right\" width=\"200\">Ajax Integration:&nbsp;</TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox onclick=\"if(this.checked) $('chkEnableAjaxPaging').checked=false;\" id=\"chkEnableAjax\" style=\"left-margin: -2px\">Enable Ajax Interaction</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox onclick=\"if(this.checked) $('chkEnableAjax').checked=false;\" id=\"chkEnableAjaxPaging\" style=\"left-margin: -2px\">Enable Ajax Interaction (Progressive Enhancement)</TD>" +
    " </TR>" +	    
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkEnableAjaxManual\" style=\"left-margin: -2px\">Ajax Interaction - Manual Load</TD>" +
    " </TR>" +	
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkAjaxCustomStatus\" style=\"left-margin: -2px\">Ajax Interaction - Custom Status</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkAjaxCustomPaging\" style=\"left-margin: -2px\">Ajax Interaction - Custom Paging</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkAjaxPageHistory\" style=\"left-margin: -2px\">Ajax Interaction - Enable Ajax Page History</TD>" +
    " </TR>" +    
    " <TR>" +
    "  <TD class=\"SubHead\" align=\"right\" width=\"200\">&nbsp;</TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input id=\"txtAjaxPageHistory\" Width=\"50px\">&nbsp;Custom Page History Anchor Text</TD>" +
    " </TR>" +      
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input id=\"txtAjaxAutoRefresh\"  Width=\"150px\">&nbsp;Milliseconds " +
    "   between Automatic Refresh</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD class=\"SubHead\" align=\"right\" width=\"200\"></TD>" +
    "  <TD class=\"SubHead\" style=\"HEIGHT: 19px\"></TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD class=\"SubHead\" align=\"right\" width=\"200\">Include:&nbsp;</TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkJavascriptUtilities\" style=\"left-margin: -2px\">Utilities Javascript</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkJavascriptValidation\" style=\"left-margin: -2px\">Validation Javascript</TD>" +
    " </TR>" +
    sysGeneralTemplate_Javascript() +   
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"SubHead\" style=\"HEIGHT: 19px\"></TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD class=\"SubHead\" align=\"right\" width=\"200\">Integration:&nbsp;</TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input id=\"txtModuleMessageType\" Width=\"150px\">&nbsp;Message " +
    "   Type for External Module Communication Listener</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD class=\"SubHead\" align=\"right\" width=\"200\"></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input id=\"txtJavascriptOnComplete\" Width=\"150px\">&nbsp;Javascript " +
    "   Function (Name) to execute after Load completes</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD class=\"SubHead\" align=\"right\" width=\"200\"></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"></TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD class=\"SubHead\" align=\"right\" width=\"200\">Debugging:&nbsp;</TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkDebugging_All\" style=\"left-margin: -2px\">Display for All Users</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkDebugging_Editors\" style=\"left-margin: -2px\">Display for Edit Users</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkDebugging_Administrators\" style=\"left-margin: -2px\">Display for Admin Users</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkDebugging_SuperUsers\" style=\"left-margin: -2px\">Display for Super Users</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"></TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkDebugging_IgnoreRedirections\" style=\"left-margin: -2px\">Skip Redirect Actions</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox id=\"chkDebugging_IgnoreSubquery\" style=\"left-margin: -2px\">Skip Subquery Debugging</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"></TD>" +
    " </TR>" +
    " <TR style=\"display:none;\">" +
    "  <TD class=\"SubHead\" align=\"right\" width=\"200\">Administration:&nbsp;</TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=radio name=\"chkAdmin\" value=\"Edit\" style=\"left-margin: -2px\">Edit Users</TD>" +
    " </TR>" +
    " <TR style=\"display:none;\">" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=radio name=\"chkAdmin\" value=\"Admin\" style=\"left-margin: -2px\">Admin Users</TD>" +
    " </TR>" +
    " <TR style=\"display:none;\">" +
    "  <TD></TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=radio name=\"chkAdmin\" value=\"Super\" style=\"left-margin: -2px\">Super Users</TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD></TD>" +
    "  <TD class=\"SubHead\" style=\"HEIGHT: 19px\"></TD>" +
    " </TR>" +
    " <TR>" +
    "  <TD class=\"SubHead\" vAlign=\"top\" align=\"right\" width=\"200\">Options:&nbsp;</TD>" +
    "  <TD class=\"Normal\" style=\"HEIGHT: 19px\">" +
    "   <div style=\"display: none;\"><input type=checkbox id=\"chkUseListX\" style=\"left-margin: -2px\">Use the ListX Script tags, in place of Open Script</div>" +
    "   <div><input type=checkbox id=\"chkUseExplicit\" style=\"left-margin: -2px\">Use Explicit System Variables [Name,System] or [*Name]</div>" +
    "   <div><input type=checkbox id=\"chknoOWSCreate\" style=\"left-margin: -2px\">Suppress creation of ows tokens and ows.create calls. OWS will only operate with standard callbacks and no ows.js support.</div>" +
    "   <div style=\"display: none;\"><input type=checkbox checked=\"checked\" id=\"chkUseAdvancedParsing\" style=\"left-margin: -2px\">Use Advanced Parsing (Not Backwards Compatible)</div>" +
    "   <div><input type=checkbox id=\"chkUseCompoundIIFConditions\" style=\"left-margin: -2px\">Use Compound Conditions and Math in IIF tags</div>" +
    "   <div><input type=checkbox id=\"chkUseGoSplit\" style=\"left-margin: -2px\">Use Query Command Separation on GO</div>" +
    "   <div style=\"display: none;\"><input type=checkbox id=\"chkExcelExport\" style=\"left-margin: -2px\" >Enable Excel Export (depricated)</div>" +
    "   <div style=\"display: none;\"><input type=checkbox id=\"chkMasterTemplate\" style=\"left-margin: -2px\">Master Template&nbsp;</div>" +    
	"   <div style=\"display: none;\"><input type=checkbox id=\"chkUseSilverlight\" style=\"left-margin: -2px\">Use Silverlight for rendering</div>" +
    "  </TD>" +
    " </TR>" +
    "</TABLE>"
  };
}
function sysGeneralTemplate_Javascript()
{
    var str = '';
	for (var i = 0;i<adminAbout.UI.length;i++)
	{
	    if (adminAbout.UI[i].Required=='false' && adminAbout.UI[i].Name != 'ows.utilities' && adminAbout.UI[i].Name != 'ows.validation' )
	    {
	    str =   str + " <TR>" +
                "  <TD></TD>" +
                "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox name=\"chkJavascript\" value=\""+adminAbout.UI[i].Name+"\" style=\"left-margin: -2px\">" + adminAbout.UI[i].Title + "</TD>" +
                " </TR>";
        }
    }
	if (typeof configuration.javascriptInclude != 'undefined' && configuration.javascriptInclude!=null)
	{
	    for (var ji = 0;ji<configuration.javascriptInclude.length;ji++)
	    {
	        var b = false;
	        for (var i = 0;i<adminAbout.UI.length;i++)
	        {
	            if (adminAbout.UI[i].Name==configuration.javascriptInclude[ji])
	            {
                    b = true;
                }
            }
            if (!b)
            {
           	            str =   str +  " <TR>" +
                        "  <TD></TD>" +
                        "  <TD class=\"Normal\" style=\"HEIGHT: 19px\"><input type=checkbox name=\"chkJavascript\" value=\""+configuration.javascriptInclude[ji]+"\" style=\"left-margin: -2px\">Missing Reference: " + configuration.javascriptInclude[ji] + "</TD>" +
                        " </TR>"; 
            }
	    }
	}
    return str;
}  
var sysOpenTemplate =   {
   "Name" : "<center>Open Configuration</center>",
   "Code" : "Open Configuration",
   "Description" : "Provides the ability for a developer to open a configuration.",
   "onLoad" : "onOpenConfigLoad",
   "onSave" : "onOpenConfigSave",
   "Template" :
    "<div id=\"ConfigurationList\"></div>" +
	"<div id=\"ConfigListPager\"></div>"
  };  

var sysAction_Types = [
  {
   "Name" : "Comment",
   "Code" : "Action-Comment",
   "Description" : "Provide the ability for a developer to place Comments within their Action script",
   "onLoad" : "onActionLoad_Comment",
   "onSave" : "onActionSave_Comment",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Comment",
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151 align=left>Comment</td>" +
    "</tr>" +
    "<tr>" +
     "<td><textarea wrap=off name=frmActionComment id=frmActionComment style=\"height: 150px; width:100%;\" richtext=true ></textarea></td>" +
    "</tr>" +
   "</table>"
  },
  {
   "Name" : "Goto",
   "Code" : "Action-Goto",
   "Description" : "Provide the ability to jump from one configuration to a point in another configuration",
   "onLoad" : "onActionLoad_Goto",
   "onSave" : "onActionSave_Goto",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Goto",   
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    " <tr>" +
    "  <td class=\"SubHead\" width=\"151\">Configuration (static)</td>" +
    "  <td class=\"Normal\" style=\"HEIGHT: 19px\"><input type=\"hidden\" name=frmActionGoto_Name id=frmActionGoto_Name /><div id=fi1>" +
    "	<select name=frmActionGoto_ConfigurationID id=frmActionGoto_ConfigurationID onchange=\"onGotoConfigChanged();\">" +
    "	<option value=\"\">Select a Configuration</option>" +
    "	</select>&nbsp;<a href=\"#\" onclick=\"loadConfigurationChoices_Goto(sysGetSelect($('frmActionGoto_ConfigurationID')));return false;\">refresh</a>&nbsp;<a href=\"#\" onclick=\"openConfig_Goto(sysGetSelect($('frmActionGoto_ConfigurationID')));return false;\">edit</a></div></td>" +
	" </tr>" +
	" <tr>" +
	" <td class=\"SubHead\" width=\"151\">Region (static)</td>" +
	" <td class=\"Normal\" style=\"HEIGHT: 19px\"><div id=fi1>" +
	"	<select name=frmActionGoto_Region id=frmActionGoto_Region>" +
	"		<option value=\"\">Select a Region</option>" +
	"	</select>&nbsp;<a href=\"#\" onclick=\"configRegions=0;loadConfigurationRegions_Goto(sysGetSelect($('frmActionGoto_ConfigurationID')));return false;\">refresh</a></div></td>" +
	"</tr>" +
	"<tr><td colspan=\"2\"><hr></td></tr>"+
	"<tr>" +
	   "<td class=\"SubHead\" width=\"151\">Configuration (dynamic)</td>" +
	   "<td class=\"Normal\" style=\"HEIGHT: 19px\"><input style=\"width:500px\" type=\"text\" name=frmActionGoto_ConfigurationDyn id=frmActionGoto_ConfigurationDyn /></td>" +
	 " </tr>" +
	 "<tr>" +
	 "  <td class=\"SubHead\" width=\"151\">Region (dynamic)</td>" +
	 "  <td class=\"Normal\" style=\"HEIGHT: 19px\">" +
	 "<input style=\"width:500px\" type=\"text\" name=frmActionGoto_RegionDyn id=frmActionGoto_RegionDyn>" +
	 " </td></tr>" +
   "</table>"
  },  
  {
    "Name" : "Query",
   "Code" : "Action-Execute",
   "Description" : "Execute a query to gain the results and user them as a loop, or as a reference point",
   "onLoad" : "onActionLoad_Query",
   "onSave" : "onActionSave_Query",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Query",
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Name</td>" +
     "<td><input name=frmActionQuery_Name id=frmActionQuery_Name type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Connection</td>" +
     "<td><input name=frmActionQuery_Connection id=frmActionQuery_Connection type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Cache (seconds)</td>" +
     "<td>" + 
     "<input name=frmActionQuery_CacheTime id=frmActionQuery_CacheTime type=textbox style=\"width:200px;\">" +
     "<span class=SubHead>&nbsp;Cache (Name)&nbsp;</span>" +
     "<input name=frmActionQuery_CacheName id=frmActionQuery_CacheName type=textbox style=\"width:200px;\">" +  
     "&nbsp;<input type=checkbox id=\"frmActionQuery_CacheShared\" style=\"left-margin: -2px\">Shared" +   
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Run as Process</td>" +
     "<td><input name=frmActionQuery_IsProcess id=frmActionQuery_IsProcess type=\"checkbox\"/></td>" +
    "</tr>" +		
    "<tr>" +
     "<td class=SubHead width=151>" +
	 "Query<br><a href='javascript:QueryExecute();'>(Execute&nbsp;Query)</a>" + 
     "</td>" +
     "<td id=cActionQuery_Query><textarea wrap=off name=frmActionQuery_Query id=frmActionQuery_Query style=\"height: 150px; width:100%;\" richtext=true ></textarea></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>" + 
	 "<div id=qry1>Results<br><a href='javascript:QuickBuilder();'>(Quick&nbsp;Builder)</a><br><a href='javascript:ColumnClip();'>(Column&nbsp;Clip)</a><br></div>" + 	 
	 "</td>" +
     "<td><div id=qry2>" +
	 "</div>" +
	 "</td>" +
    "</tr>" +		
    "<tr>" +	
	"<td colspan=2><div id=\"qryQB\"></div></td></tr>" +	
   "</table>"
  },  
  {
   "Name" : "Message",
   "Code" : "Message",
   "Description" : "Event driven logic, based on a message sender and receiver. Upon receipt of the defined message, this action will execute.",
   "onLoad" : "onActionLoad_Message",
   "onSave" : "onActionSave_Message",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Message",
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Event Type</td>" +
     "<td><input name=frmActionMessage_Type id=frmActionMessage_Type type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Event Value</td>" +
     "<td><input name=frmActionMessage_Value id=frmActionMessage_Value type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
   "</table>"
  },
  {
   "Name" : "Search",
   "Code" : "Action-Search",
   "Description" : "Integration within the core systems search functionality.",
   "onLoad" : "onActionLoad_Search",
   "onSave" : "onActionSave_Search",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Search",
   "Template" :
	"<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" style=\"font-size: 12px;\">" +
	"	<tr>" +
	"		<td>Query - <i><i>The SQL Statement to execute against your database to obtain the list of all searchable content</i></i></td>" +
	"	</tr>" +
	"	<tr>" +
	"		<td class=\"SubHead\">" +
	"			<textarea wrap=off onfocus=\"RibbonEditor.Toggle('Search',this);\" name=txtSearchQuery id=txtSearchQuery style=\"height: 150px; width:100%;\" richtext=true></textarea>" +
	"		</td>" +
	"	</tr>" +
	"	<tr>" +
	"		<td class=\"SubHead\">" +
	"			<table cellpadding=\"0\" border=\"0\" width=\"100%\">" +
	"				<tbody><tr>" +
	"					<td nowrap=\"\" width=\"150\" class=\"SubHead\">Title</td>" +
	"					<td nowrap=\"\" colspan=\"3\" class=\"Normal\">" +
	"						<input type=\"text\" style=\"width: 100%;\" id=\"txtSearchTitle\"/></td>" +
	"				</tr>" +
	"				<tr>" +
	"					<td nowrap=\"\" width=\"150\" class=\"SubHead\">Author (UserID)</td>" +
	"					<td nowrap=\"\" class=\"Normal\">" +
	"						<input type=\"text\" style=\"width: 100%;\" id=\"txtSearchAuthor\"/></td>" +
	"					<td nowrap=\"\" width=\"150\" class=\"SubHead\">Published Date</td>" +
	"					<td nowrap=\"\" width=\"150\" class=\"Normal\">" +
	"						<input type=\"text\" style=\"width: 100%;\" id=\"txtSearchDate\"/></td>" +
	"				</tr>" +
	"				<tr>" +
	"					<td nowrap=\"\" width=\"150\" class=\"SubHead\">Link Parameters</td>" +
	"					<td nowrap=\"\" class=\"Normal\">" +
	"						<input type=\"text\" style=\"width: 100%;\" id=\"txtSearchQuerystring\"/></td>" +
	"					<td nowrap=\"\" width=\"150\" class=\"SubHead\">Search Key</td>" +
	"					<td nowrap=\"\" width=\"150\" class=\"Normal\">" +
	"						<input type=\"text\" style=\"width: 100%;\" id=\"txtSearchKey\"/></td>" +
	"				</tr>" +
	"			</tbody></table>" +
	"		</td>" +
	"	</tr>" +
	"	<tr>" +
	"		<td>Description - <i>The format used to display and describe the description of the search result items.</i></td>" +
	"	</tr>" +
	"	<tr>" +
	"		<td class=\"SubHead\">" +
	"			<textarea wrap=off onfocus=\"RibbonEditor.Toggle('Search',this);\" name=txtSearchDescription id=txtSearchDescription style=\"height: 150px; width:100%;\" richtext=true></textarea>" +
	"		</td>" +
	"	</tr>" +
	"	<tr>" +
	"		<td>Content - <i>The format used for the content of the search items, this content is used for searching directly within the search interface.</i></td>" +
	"	</tr>" +
	"	<tr>" +
	"		<td class=\"SubHead\">" +
	"			<textarea wrap=off onfocus=\"RibbonEditor.Toggle('Search',this);\" name=txtSearchContent id=txtSearchContent style=\"height: 150px; width:100%;\" richtext=true></textarea>" +
	"		</td>" +
	"	</tr>" +
	"</table>"
  },    
  {
   "Name" : "Delay",
   "Code" : "Action-Delay",
   "Description" : "Provide the ability for a developer to force a specific length Delay",
   "onLoad" : "onActionLoad_Delay",
   "onSave" : "onActionSave_Delay",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Delay",
   "Template" : 
    "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Delay Metric</td>" +
     "<td>" +
     "<input type=\"radio\" id=frmActionDelay_Type name=frmActionDelay_Type value=\"Millisecond\">Milliseconds" +
	 "<input type=\"radio\" id=frmActionDelay_Type name=frmActionDelay_Type value=\"Second\">Seconds" +     
	 "<input type=\"radio\" id=frmActionDelay_Type name=frmActionDelay_Type value=\"Minute\">Minutes" +     	 
	 "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Value</td>" +
     "<td><input name=frmActionDelay_Value id=frmActionDelay_Value type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
   "</table>"
  },  
  {
   "Name" : "Redirect",
   "Code" : "Action-Redirect",
   "Description" : "Redirect the browser to another location",
   "onLoad" : "onActionLoad_Redirect",
   "onSave" : "onActionSave_Redirect",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Redirect",
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Type</td>" +
     "<td>" +
      "<input type=\"radio\" id=frmActionRedirect_Type name=frmActionRedirect_Type value=\"Link\">URL Link" +     
      "<input type=\"radio\" id=frmActionRedirect_Type name=frmActionRedirect_Type value=\"Tab\">Page" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Value</td>" +
     "<td><input name=frmActionRedirect_Value id=frmActionRedirect_Value style=\"width:100%;\"></td>" +
    "</tr>" +
   "</table>"
  },  
  {
   "Name" : "File",
   "Code" : "Action-File",
   "Description" : "Load Files or Import and Export data to and from your systems",
   "onLoad" : "onActionLoad_File",
   "onSave" : "onActionSave_File",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_File",
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Source Type</td>" +
     "<td>" +
     "<select name=frmSourceType id=frmSourceType onchange=\"onfrmSourceType_Toggle()\">" +
	  "<option value=\"\"></option>" +	 
      "<option value=\"Path\">Directory / File Path</option>" +
      "<option value=\"Variable\">Runtime Value</option>" +
      "<option value=\"SQL\">SQL Query</option>" +
     "</select>" +
     "</td>" +
    "</tr>" +   
    "<tr>" +
     "<td width=151><div id=fi0 class=SubHead>Variable Type</div></td>" +
     "<td><div id=fi1>" +
     "<select name=frmSourceVariableType id=frmSourceVariableType>" +
      "<option value=\"&amp;lt;Form&amp;gt;\">&lt;Form&gt;</option>" +
	  "<option value=\"&amp;lt;Session&amp;gt;\">&lt;Session&gt;</option>" +
      "<option value=\"&amp;lt;ViewState&amp;gt;\">&lt;ViewState&gt;</option>" +
      "<option value=\"&amp;lt;Action&amp;gt;\">&lt;Action&gt;</option>" +
      "<option value=\"&amp;lt;Cookie&amp;gt;\">&lt;Cookie&gt;</option>" +
      "<option value=\"&amp;lt;QueryString&amp;gt;\">&lt;QueryString&gt;</option>" +
      "<option value=\"&amp;lt;Context&amp;gt;\">&lt;Context&gt;</option>" +
      "<option value=\"&amp;lt;Custom&amp;gt;\">&lt;Custom&gt;</option>" +
     "</select></div>" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=fi2>Source</div></td>" +
     "<td><div id=fi3><input name=frmSource id=frmSource type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +
    "<tr>" +
     "<td width=151>" +
     "<div class=SubHead id=fi4>Source Query</div>" +
     "</td>" +
     "<td><div id=fi5><textarea wrap=off name=frmSourceQuery id=frmSourceQuery style=\"height: 150px; width:100%;\" richtext=true></textarea></div></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=fy4>Custom Connection</div></td>" +
     "<td><div id=fy5><input name=frmSourceConnection id=frmSourceConnection type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151>Destination Type</td>" +
     "<td>" +
     "<select name=frmDestinationType id=frmDestinationType onchange=\"onfrmDestinationType_Toggle()\">" +
	  "<option value=\"\"></option>" +	 
      "<option value=\"Path\">Directory / File Path</option>" +
      "<option value=\"Variable\">Runtime Value</option>" +
      "<option value=\"SQL\">SQL Query</option>" +
	  "<option value=\"Response\">Outgoing Response</option>" +
	  "<option value=\"EmailAttachment\">Email Attachment</option>" +
     "</select>" +
     "</td>" +
    "</tr>" +  
    "<tr>" +
     "<td class=SubHead width=151><div id=fi6>Response Type</div></td>" +
     "<td><div id=fi7><input name=frmResponseType id=frmResponseType type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" + 	
    "<tr>" +
     "<td width=151><div id=fi8 class=SubHead>Variable Type</div></td>" +
     "<td><div id=fi9>" +
     "<select name=frmResultVariableType id=frmResultVariableType>" +
	  "<option value=\"&amp;lt;Session&amp;gt;\">&lt;Session&gt;</option>" +
      "<option value=\"&amp;lt;ViewState&amp;gt;\">&lt;ViewState&gt;</option>" +
      "<option value=\"&amp;lt;Action&amp;gt;\">&lt;Action&gt;</option>" +
      "<option value=\"&amp;lt;Cookie&amp;gt;\">&lt;Cookie&gt;</option>" +
      "<option value=\"&amp;lt;Context&amp;gt;\">&lt;Context&gt;</option>" +
     "</select></div>" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td width=151><div id=fi10 class=SubHead>" + 
		"<input type=checkbox id=\"frmChkRunAsProcess\" style=\"left-margin: -2px\">Run As Process"	 +
	 "</div></td>" +
	  "<td><div id=fi11><input name=frmFileProcessName id=frmFileProcessName type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" + 	
    "<tr>" +
     "<td width=151><div id=fi12 class=SubHead>" + 
		"&nbsp;" +	 
	 "</div></td>" +
	  "<td><div id=fi13><input type=checkbox id=\"frmChkFirstRow\" style=\"left-margin: -2px\">First Row Contains Column Names</div></td>" +
    "</tr>" +
    "<tr>" +
     "<td width=151>" +
     "<div class=SubHead id=fi14>Column Mapping</div>" +
     "</td>" +
     "<td><div id=fi15></div></td>" +
    "</tr>" +
    "<tr>" +
     "<td width=151>" +
     "<div class=SubHead id=fi16>Destination Query</div>" +
     "</td>" +
     "<td><div id=fi17><textarea wrap=off name=frmDestinationQuery id=frmDestinationQuery style=\"height: 150px; width:100%;\" richtext=true></textarea></div></td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151>Destination</td>" +
     "<td><input name=frmDestination id=frmDestination type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
    "<tr>" +
     "<td width=151>Transformation</td>" +
     "<td>" +
     "<select name=frmTransformation id=frmTransformation onchange=\"onfrmTransformation_Toggle()\">" +
	  "<option value=\"\"></option>" +
	  "<option value=\"Image\">Image</option>" +
	  "<option value=\"File\">File</option>" +	  
     "</select>" +
     "</td>" +
    "</tr>" +	
    "<tr>" +
     "<td width=151><div id=fi18>File Operation</div></td>" +
     "<td><div id=fi19>" +
     "<select name=frmFileOperation id=\"frmFileOperation\">" +
	  "<option value=\"copy\">Copy Source to Target</option>" +
	  "<option value=\"move\">Move Source to Target</option>" +
	  "<option value=\"delete\">Delete Source</option>" +	  
	  "<option value=\"extract\">Decompress Source into Target</option>" +		  
      "<option value=\"compress\">Compress Source into Target</option>" +	  
      "<option value=\"csv\">CSV Delimited (SQL Source Only)</option>" +
      "<option value=\"tab\">Tab Delimited (SQL Source Only)</option>" +
      "<option value=\"semicolon\">Semicolon Delimited (SQL Source Only)</option>" +
     "</select></div>" +
     "</td>" +
    "</tr>" +	
    "<tr>" +
     "<td width=151><div id=fi20>Image Operation</div></td>" +
     "<td><div id=fi21>" +
     "<select name=frmImageTransformType id=\"frmImageTransformType\"  onchange=\"onfrmImageTransformType_Toggle()\">" +
	  "<option value=\"Size\">Resize</option>" +
      "<option value=\"Crop\">Crop</option>" +
      "<option value=\"SmartCrop\">Smart Crop</option>" +
      "<option value=\"Rotate\">Rotate</option>" +
	  "<option value=\"Draw.Text\">Draw Text</option>" +
     "</select></div>" +
     "</td>" +
    "</tr>" +	    
    "<tr>" +
     "<td width=151><div id=fi22>Image Width</div></td>" +
     "<td><div id=fi23>" +
     "<input name=frmImageWidth id=frmImageWidth type=textbox style=\"width:100px;\">&nbsp;<select name=frmImageWidthType id=\"frmImageWidthType\">" +
	  "<option value=\"px\">pixels</option>" +
	  "<option value=\"%\">percent</option>" +
     "</select></div>" +
     "</td>" +
    "</tr>" +		
    "<tr>" +
     "<td width=151><div id=fi24>Image Height</div></td>" +
     "<td><div id=fi25>" +
     "<input name=frmImageHeight id=frmImageHeight type=textbox style=\"width:100px;\">&nbsp;<select name=frmImageHeightType id=\"frmImageHeightType\">" +
	  "<option value=\"px\">pixels</option>" +
	  "<option value=\"%\">percent</option>" +
     "</select></div>" +
     "</td>" +
    "</tr>" +	
    "<tr>" +
     "<td width=151><div id=fi26>Image Quality</div></td>" +
     "<td><div id=fi27>" +
     "<input name=frmImageQuality id=frmImageQuality type=textbox style=\"width:100px;\"></div>" +
     "</td>" +
    "</tr>" +	
    "<tr>" +
     "<td width=151><div id=fi28>Text</div></td>" +
     "<td><div id=fi29>" +
     "<input name=frmImageText id=frmImageText type=textbox  style=\"width:100%;\"></div>" +
     "</td>" +
    "</tr>" +	   
    "<tr>" +
     "<td width=151><div id=fi30>X</div></td>" +
     "<td><div id=fi31>" +
     "<input name=frmImageX id=frmImageX type=textbox style=\"width:100px;\"></div>" +
     "</td>" +
    "</tr>" +	 
    "<tr>" +
     "<td width=151><div id=fi32>Y</div></td>" +
     "<td><div id=fi33>" +
     "<input name=frmImageY id=frmImageY type=textbox style=\"width:100px;\"></div>" +
     "</td>" +
    "</tr>" +	        	
    "<tr>" +
     "<td width=151><div id=fi34>Font Family</div></td>" +
     "<td><div id=fi35>" +
     "<input name=frmImageFont id=frmImageFont type=textbox style=\"width:100px;\"></div>" +
     "</td>" +
    "</tr>" +		
    "<tr>" +
     "<td width=151><div id=fi36>Font Size</div></td>" +
     "<td><div id=fi37>" +
     "<input name=frmImageSize id=frmImageSize type=textbox style=\"width:100px;\">&nbsp;<select name=frmImageSizeType id=\"frmImageSizeType\">" +
	  "<option value=\"px\">pixels</option>" +
	  "<option value=\"pt\">points</option>" +
     "</select></div>" +
     "</td>" +
    "</tr>" +	
    "<tr>" +
     "<td width=151><div id=fi38>Fore Color</div></td>" +
     "<td><div id=fi39>" +
     "<input name=frmImageColor id=frmImageColor type=textbox style=\"width:100px;\"></div>" +
     "</td>" +
    "</tr>" +    
    "<tr>" +
     "<td width=151><div id=fi40>Background Color</div></td>" +
     "<td><div id=fi41>" +
     "<input name=frmImageBGColor id=frmImageBGColor type=textbox style=\"width:100px;\"></div>" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td width=151><div id=fi42>Warp Text</div></td>" +
     "<td><div id=fi43>" +
      "<input type=\"checkbox\" name=frmImageWarp value=\"1\">Skew / Warp<br/>" +     
      "<input type=\"checkbox\" name=frmImageWarp value=\"2\">Distort / Ripple<br/>" +
      "<input type=\"checkbox\" name=frmImageWarp value=\"3\">Jitter<br/>" +
      "<input type=\"checkbox\" name=frmImageWarp value=\"4\">Splinter / Thrash<br/>" +
      "<input type=\"checkbox\" name=frmImageWarp value=\"5\">Warp Background (using selected warps)" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td width=151><div id=fi44>Rotation</div></td>" +
     "<td><div id=fi45>" +
     "<input name=frmImageRotation id=frmImageRotation type=textbox style=\"width:100px;\"></div>" +
     "</td>" +
    "</tr>" +	
   "</table>"
  },    
 {
   "Name" : "Filter",
   "Code" : "Action-Filter",
   "Description" : "Load Filter options for providing built in filtering for you query",
   "onLoad" : "onActionLoad_Filter",
   "onSave" : "onActionSave_Filter",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Filter",
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td width=151>" +
     "<div class=SubHead id=fi14>Filter Options</div>" +
     "</td>" +
     "<td><div id=fi15></div></td>" +
    "</tr>" +
   "</table>"
  },
  {
   "Name" : "Assignment",
   "Code" : "Action-Assignment",
   "Description" : "Assign values to your runtime parameters",
   "onLoad" : "onActionLoad_Assignment",
   "onSave" : "onActionSave_Assignment",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Assignment",
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Variable Type</td>" +
     "<td>" +
     "<select name=frmActionAssignment_VariableType id=frmActionAssignment_VariableType style=\"width:100%;\">" +
      "<option value=\"&amp;lt;Session&amp;gt;\">&lt;Session&gt;</option>" +
      "<option value=\"&amp;lt;ViewState&amp;gt;\">&lt;ViewState&gt;</option>" +
      "<option value=\"&amp;lt;Action&amp;gt;\">&lt;Action&gt;</option>" +
      "<option value=\"&amp;lt;Cache&amp;gt;\">&lt;Cache&gt;</option>" +
      "<option value=\"&amp;lt;Context&amp;gt;\">&lt;Context&gt;</option>" +
      "<option value=\"&amp;lt;Cookie&amp;gt;\">&lt;Cookie&gt;</option>" +
      "<option value=\"&amp;lt;Module Setting&amp;gt;\">&lt;Module Setting&gt;</option>" +
      "<option value=\"&amp;lt;Tab Module Setting&amp;gt;\">&lt;Tab Module Setting&gt;</option>" +
      "<option value=\"&amp;lt;UserInfo&amp;gt;\">&lt;UserInfo&gt;</option>" +
      "<option value=\"&amp;lt;SharedCache&amp;gt;\">&lt;SharedCache&gt;</option>" +
      "<option value=\"&amp;lt;System&amp;gt;\">&lt;System&gt;</option>" +
      "<option value=\"CONTROL.HEADER\">Control.Header</option>" +   
      "<option value=\"CONTROL.FOOTER\">Control.Footer</option>" +   
      "<option value=\"FILTER\">Filter</option>" +      
      "<option value=\"MODULETITLE\">ModuleTitle</option>" +
      "<option value=\"PAGETITLE\">PageTitle</option>" +
      //"<option value=\"PAGEDESCRIPTION\">PageDescription</option>" +
      //"<option value=\"PAGEKEYWORDS\">PageKeywords</option>" +
      //"<option value=\"PAGEAUTHOR\">PageAuthor</option>" +
      //"<option value=\"PAGECOPYRIGHT\">PageCopyright</option>" +
      //"<option value=\"PAGEGENERATOR\">PageGenerator</option>" +
      "<option value=\"PAGESIZE\">PageSize</option>" +
      "<option value=\"PAGENUMBER\">PageNumber</option>" +
      "<option value=\"CACHECONTROL\">Response.CacheControl</option>" +
      "<option value=\"CONTENTDISPOSITION\">Response.ContentDisposition</option>" +
      "<option value=\"REDIRECTLOCATION\">Response.RedirectLocation</option>" +
      "<option value=\"CONTENTTYPE\">Response.ContentType</option>" +
      "<option value=\"PRAGMA\">Response.Pragma</option>" +
      "<option value=\"STATUS\">Response.Status</option>" +
      "<option value=\"STATUSCODE\">Response.StatusCode</option>" +
      "<option value=\"STATUSDESCRIPTION\">Response.StatusDescription</option>" +
      "<option value=\"SEARCH\">Search</option>" +
      "<option value=\"SORT\">Sort</option>" +
     "</select>" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Name</td>" +
     "<td><input name=frmActionAssignment_Name id=frmActionAssignment_Name type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>" +
     "<input  name=frmActionAssignment_IgnoreTags id=frmActionAssignment_IgnoreTags type=\"checkbox\"/>" +
     "<label for=\"frmActionAssignment_IgnoreTags\">Value (Ignore Tags)</label>" +
     "</td>" +
     "<td><textarea wrap=off name=frmActionAssignment_Value id=frmActionAssignment_Value style=\"height: 150px; width:100%;\" richtext=true></textarea></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Assignment Type</td>" +
     "<td>" +
      "<input type=\"radio\" id=frmActionAssignment_AssignmentType name=frmActionAssignment_AssignmentType value=\"0\">Assign" +     
      "<input type=\"radio\" id=frmActionAssignment_AssignmentType name=frmActionAssignment_AssignmentType value=\"1\">Append" +
      "<input type=\"radio\" id=frmActionAssignment_AssignmentType name=frmActionAssignment_AssignmentType value=\"3\">Prepend" +
      "<input type=\"radio\" id=frmActionAssignment_AssignmentType name=frmActionAssignment_AssignmentType value=\"2\">Increment" +     
     "</td>" +
    "</tr>" +
   "</table>"
  },
  {
   "Name" : "Email",
   "Code" : "Action-Email",
   "Description" : "Send email from the actions",
   "onLoad" : "onActionLoad_Email",
   "onSave" : "onActionSave_Email",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Email",   
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>From</td>" +
     "<td><input name==frmActionEmail_From id=frmActionEmail_From type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>To</td>" +
     "<td><input name==frmActionEmail_To id=frmActionEmail_To type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Cc</td>" +
     "<td><input name==frmActionEmail_Cc id=frmActionEmail_Cc type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Bcc</td>" +
     "<td><input name==frmActionEmail_Bcc id=frmActionEmail_Bcc type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +  
    "<tr>" +
     "<td class=SubHead width=151>Subject</td>" +
     "<td><input name==frmActionEmail_Subject id=frmActionEmail_Subject type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +  
    "<tr>" +
     "<td class=SubHead width=151>Format</td>" +
     "<td>" +
      "<input type=\"radio\" id=frmActionEmail_Format name=frmActionEmail_Format value=\"text\">Text" +     
      "<input type=\"radio\" id=frmActionEmail_Format name=frmActionEmail_Format value=\"html\">Html" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>" +
     "Body" +
     "</td>" +
     "<td><textarea wrap=off name=frmActionEmail_Body id=frmActionEmail_Body style=\"height: 150px; width:100%;\" richtext=true></textarea></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Result</td>" +
     "<td class=Normal><I>When the Email attempt is completed, the result can be identified as either successful (True) or as a failure (Message). In the option below, specify the type and name of the target variable which will house the actual result response of the email attempt.</I>" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Variable Type</td>" +
     "<td>" +
     "<select name=frmActionEmail_VariableType id=frmActionEmail_VariableType style=\"width:100%;\">" +
      "<option value=\"&amp;lt;Session&amp;gt;\">&lt;Session&gt;</option>" +
      "<option value=\"&amp;lt;ViewState&amp;gt;\">&lt;ViewState&gt;</option>" +
      "<option value=\"&amp;lt;Action&amp;gt;\">&lt;Action&gt;</option>" +
      "<option value=\"&amp;lt;Cookie&amp;gt;\">&lt;Cookie&gt;</option>" +
     "</select>" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Name</td>" +
     "<td><input name=frmActionEmail_Name id=frmActionemail_Name type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
   "</table>" +
   "<div id='divServer' style='border:1px solid #ccc;text-align:center;'><a href='#' onclick=\"this.style.display='none';document.getElementById('objEmailServer').style.display='block';return false;\">Click here to Adjust SMTP Server Settings</a></div>"+
   "<table id='objEmailServer' style='display:none;' class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +   
    "<tr>" +
     "<td class=SubHead width=151>Server</td>" +
     "<td><input name=frmActionEmail_SServer id=frmActionEmail_SServer type=textbox style=\"width:100%;\"><br/><i>Provide the Address of the Server. If a specific port is required, add a colon followed by the port (ie. pop.gmail.com:995)</td>" +
    "</tr>" +   
    "<tr>" +
     "<td class=SubHead width=151>Authentication Type</td>" +
     "<td>" +
      "<input type=\"radio\" id=frmActionEmail_SAuth name=frmActionEmail_SAuth value=\"0\">Anonymous<br>" +
      "<input type=\"radio\" id=frmActionEmail_SAuth name=frmActionEmail_SAuth value=\"1\">Basic Authentication<br>" +   
	  "<input type=\"radio\" id=frmActionEmail_SAuth name=frmActionEmail_SAuth value=\"2\">NTLM Authentication" +
     "</td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151>User Name</td>" +
     "<td><input name=frmActionEmail_SUser id=frmActionEmail_SUser type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151>Password</td>" +
     "<td ><input name=frmActionEmail_SPass id=frmActionEmail_SPass type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151>Use SSL</td>" +
     "<td ><input type=checkbox id=\"frmActionEmail_SSSL\" style=\"left-margin: -2px\">Server requires SSL<br></td>" +
    "</tr>" +	
   "</table>"       
  },
  {
   "Name" : "Input",
   "Code" : "Action-Input",
   "Description" : "Retrieve data from an external resource",
   "onLoad" : "onActionLoad_Input",
   "onSave" : "onActionSave_Input",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Input",
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>URL</td>" +
     "<td><input name=frmActionInput_URL id=frmActionInput_URL type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +   
    "<tr>" +
     "<td class=SubHead width=151>Authentication Type</td>" +
     "<td>" +
      "<input type=\"radio\" id=frmActionInput_Auth name=frmActionInput_Auth onclick=\"onfrmActionInput_AuthToggle()\" value=\"0\">No Authentication<br>" +     
      "<input type=\"radio\" id=frmActionInput_Auth name=frmActionInput_Auth onclick=\"onfrmActionInput_AuthToggle()\" value=\"1\">Basic Authentication<br>" +
      "<input type=\"radio\" id=frmActionInput_Auth name=frmActionInput_Auth onclick=\"onfrmActionInput_AuthToggle()\" value=\"3\">Digest Authentication<br>" +   
	  "<input type=\"radio\" id=frmActionInput_Auth name=frmActionInput_Auth onclick=\"onfrmActionInput_AuthToggle()\" value=\"2\">Windows Authentication" +
     "</td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151><div id=dfrm0>User Name</div></td>" +
     "<td><div id=dfrm1><input name=frmActionInput_UserName id=frmActionInput_UserName type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151><div id=dfrm2>Password</div></td>" +
     "<td ><div id=dfrm3><input name=frmActionInput_Password id=frmActionInput_Password type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151><div id=dfrm4>Domain<br></td>" +
     "<td ><div id=dfrm5><input name=frmActionInput_Domain id=frmActionInput_Domain type=textbox style=\"width:100%;\"><br></div></td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151>Content Type</td>" +
     "<td><input name=frmActionInput_ContentType id=frmActionInput_ContentType type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151>Headers (Request)</td>" +
     "<td><textarea wrap=offname=frmActionInput_Headers id=frmActionInput_Headers style=\"height: 150px; width:100%;\" richtext=true ></textarea></td>" +
    "</tr>" +	    
    "<tr>" +
     "<td class=SubHead width=151>Query Parameters</td>" +
     "<td><input name=frmActionInput_QueryParameters id=frmActionInput_QueryParameters type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151>Data (Body)</td>" +
     "<td><textarea wrap=offname=frmActionInput_Data id=frmActionInput_Data style=\"height: 150px; width:100%;\" richtext=true ></textarea></td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151>Method</td>" +
     "<td>" +
     "<select name=frmActionInput_Method id=frmActionInput_Method style=\"width:100%;\" onchange=\"onfrmActionInput_MethodToggle()\">" +
      "<option Value=\"get\">Get</option>" +
      "<option Value=\"put\">Put</option>" +
      "<option Value=\"post\">Post</option>" +            
      "<option Value=\"delete\">Delete</option>" +
      "<option Value=\"soap\">Soap</option>" +
     "</select>" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=dfrm6>Soap Action</div></td>" +
     "<td><div id=dfrm7><input name=frmActionInput_SoapAction id=frmActionInput_SoapAction type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151><div id=dfrm8>Soap Result Tag</div></td>" +
     "<td><div id=dfrm9><input name=frmActionInput_SoapResult id=frmActionInput_SoapResult type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151>Response Format</td>" +
     "<td>" +
     "<select name=frmActionInput_ResponseFormat id=frmActionInput_ResponseFormat style=\"width:100%;\" onchange=\"onfrmActionInput_ResponseFormatToggle()\">" +
      "<option Value=\"text\">Text</option>" +
      "<option Value=\"xml\">XML</option>" +
      "<option Value=\"binary\">Binary</option>" +
     "</select>" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Variable Type</td>" +
     "<td>" +
     "<select name=frmActionInput_VariableType id=frmActionInput_VariableType style=\"width:100%;\">" +
      "<option value=\"&amp;lt;Session&amp;gt;\">&lt;Session&gt;</option>" +
      "<option value=\"&amp;lt;Action&amp;gt;\">&lt;Action&gt;</option>" +
      "<option value=\"&amp;lt;Cookie&amp;gt;\">&lt;Cookie&gt;</option>" +
     "</select>" +
     "</td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151>Name</td>" +
     "<td><input name=frmActionInput_Name id=frmActionInput_Name type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=dfrm10>XML Path</div></td>" +
     "<td><div id=dfrm11><input name=frmActionInput_XMLPath id=frmActionInput_XMLPath type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +
   "</table>"
  },    
  {
   "Name" : "Output",
   "Code" : "Action-Output",
   "Description" : "Change the desired output type from the standard rendering to an alternative format",
   "onLoad" : "onActionLoad_Output",
   "onSave" : "onActionSave_Output",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Output",   
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Variable Type</td>" +
     "<td>" +
     "<select name=frmActionOutput_Type id=frmActionOutput_Type style=\"width:100%;\" onchange=\"onfrmActionOutput_TypeToggle()\">" +
      "<option Value=\"Delimited\">Delimited File</option>" +
      "<option Value=\"Complete Delimited\">Delimited File (Complete)</option>" +
      "<option Value=\"Excel\">Excel</option>" +
      "<option Value=\"Complete Excel\">Excel (Complete)</option>" +
      "<option Value=\"Word\">Word</option>" +
      "<option Value=\"Complete Word\">Word (Complete)</option>" +
     "</select>" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td width=151><div id=dl0 class=SubHead>Delimiter</div></td>" +
     "<td><div  id=dl1><input name=frmActionOutput_Delimiter id=frmActionOutput_Delimiter type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Filename</td>" +
     "<td><input name=frmActionOutput_Filename id=frmActionOutput_Filename type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151></td>" +
     "<td><input type=hidden name=frmActionOutput_ControlID id=frmActionOutput_ControlID type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
   "</table>"
  },
  {
   "Name" : "Log",
   "Code" : "Action-Log",
   "Description" : "Write an event into the Log",
   "onLoad" : "onActionLoad_Log",
   "onSave" : "onActionSave_Log",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Log",   
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Name</td>" +
     "<td><input name=frmActionLog_Name id=frmActionLog_Name type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Detail</td>" +
     "<td><textarea wrap=offname=frmActionLog_Value id=frmActionLog_Value style=\"height: 150px; width:100%;\" richtext=true ></textarea></td>" +
    "</tr>" +
   "</table>"
  },    
  {
   "Name" : "Template",
   "Code" : "Template",
   "Description" : "Assign to the global Template property used for this configuration",
   "onLoad" : "onActionLoad_Template",
   "onSave" : "onActionSave_Template",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Template",   
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Template</td>" +
     "<td>" +
     "<select name=frmActionTemplate_Type id=frmActionTemplate_Type onchange=\"onfrmActionTemplate_TypeToggle()\" style=\"width:100%;\">" +
      "<option value=\"Query-Query\">Query</option>" +
      "<option value=\"Group-Header\">Header</option>" +
      "<option value=\"Group-Footer\">Footer</option>" +
      "<option value=\"Detail-Detail\">Detail</option>" +
      "<option value=\"Detail-Alternate\">Detail (Alternate)</option>" +
      "<option value=\"Detail-NoResults\">Detail (No Results)</option>" +
      "<option value=\"Detail-NoQuery\">Detail (No Query)</option>" +
     "</select>" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=dGroup0>Group Statement</div></td>" +
     "<td><div id=dGroup1><input name=frmActionTemplate_GroupStatement id=frmActionTemplate_GroupStatement type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=dGroup2>Index</div></td>" +
     "<td><div id=dGroup3><input name=frmActionTemplate_GroupIndex id=frmActionTemplate_GroupIndex type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>" +
	 "Content" +
     "</td>" +
     "<td><textarea wrap=off name=frmActionTemplate_Value id=frmActionTemplate_Value style=\"height: 150px; width:100%;\" richtext=true></textarea></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=dGroup8>Cache (seconds)</div></td>" +
     "<td><div id=dGroup9>" + 
     "<input name=frmActionTemplate_QueryCacheTime id=frmActionTemplate_QueryCacheTime type=textbox style=\"width:200px;\">" +
     "<span class=SubHead>&nbsp;Cache (Name)&nbsp;</span>" +
     "<input name=frmActionTemplate_QueryCacheName id=frmActionTemplate_QueryCacheName type=textbox style=\"width:200px;\">" +  
     "&nbsp;<input type=checkbox id=\"frmActionTemplate_QueryCacheShared\" style=\"left-margin: -2px\">Shared" +   
     "</div></td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151><div id=dGroup4>Filter</div></td>" +
     "<td><div id=dGroup5><input name=frmActionTemplate_QueryFilter id=frmActionTemplate_QueryFilter type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151><div id=dGroup6>Custom Connection</div></td>" +
     "<td><div id=dGroup7><input name=frmActionTemplate_QueryConnection id=frmActionTemplate_QueryConnection type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +	
	"</table>"
  },
    {
   "Name" : "Variable",
   "Code" : "Template-Variable",
   "Description" : "Create a SQL Variable for use throughout the queries within the configuration",
   "onLoad" : "onActionLoad_Variable",
   "onSave" : "onActionSave_Variable",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Variable",   
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Variable Type</td>" +
     "<td colspan=3>" +
     "<select name=frmActionVariable_VariableType id=frmActionVariable_VariableType style=\"width:100%;\">" +
      "<option value=\"&lt;Action&gt;\">&lt;Action&gt;</option>" +
      "<option value=\"&lt;Cookie&gt;\">&lt;Cookie&gt;</option>" +
      "<option value=\"&lt;Cache&gt;\">&lt;Cache&gt;</option>" +
      "<option value=\"&lt;Context&gt;\">&lt;Context&gt;</option>" +      
      "<option value=\"&lt;Custom&gt;\">&lt;Custom&gt;</option>" +
      "<option value=\"&lt;Form&gt;\">&lt;Form&gt;</option>" +
      "<option value=\"&lt;Message&gt;\">&lt;Message&gt;</option>" +
	  "<option value=\"&lt;QueryString&gt;\">&lt;QueryString&gt;</option>" +
      "<option value=\"&lt;Session&gt;\">&lt;Session&gt;</option>" +
      "<option value=\"&lt;SharedCache&gt;\">&lt;SharedCache&gt;</option>" +
      "<option value=\"&lt;ViewState&gt;\">&lt;ViewState&gt;</option>" +
      "<option value=\"TabId\">TabId</option>" +
      "<option value=\"ModuleId\">ModuleId</option>" +
      "<option value=\"Owner\">Owner</option>" +
      "<option value=\"PageNumber\">PageNumber</option>" +
      "<option value=\"PageSize\">PageSize</option>" +
      "<option value=\"PortalAlias\">PortalAlias</option>" +
      "<option value=\"PortalId\">PortalId</option>" +
      "<option value=\"Qualifier\">Qualifier</option>" +
      "<option value=\"UserId\">UserId</option>" +	  
     "</select>" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Source</td>" +
     "<td colspan=3><input name=frmActionVariable_Source id=frmActionVariable_Source type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Data Type</td>" +
     "<td colspan=3>" +
     "<select name=frmActionVariable_VariableDataType id=frmActionVariable_VariableDataType style=\"width:100%;\">" +
      "<option value=\"Any\">Any</option>" +
      "<option value=\"Boolean\">Boolean</option>" +
      "<option value=\"Date\">Date</option>" +
      "<option value=\"Guid\">Guid</option>" +
      "<option value=\"Number\">Number</option>" +
     "</select>" +
     "</td>" +
    "</tr>" +    
    "<tr>" +
     "<td class=SubHead width=151>Formatters</td>" +
     "<td colspan=3><input name=frmActionVariable_Formatters id=frmActionVariable_Formatters type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +    
    "<tr>" +
     "<td class=SubHead width=151>Target</td>" +
     "<td colspan=3><input name=frmActionVariable_Target id=frmActionVariable_Target type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>" +
	 "Target Left" +
     "</td>" +
     "<td><input name=frmActionVariable_Left id=frmActionVariable_Left type=textbox style=\"width:100%;\"></td>" +
     "<td class=SubHead width=100>" +
	 "Target Right" +
     "</td>" +	 
     "<td><input name=frmActionVariable_Right id=frmActionVariable_Right type=textbox style=\"width:100%;\"></td>" +	 
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Empty Target</td>" +
     "<td colspan=3><input name=frmActionVariable_Empty id=frmActionVariable_Empty type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +	
    "<tr>" +
     "<td class=SubHead width=151 valign=top>Security</td>" +
     "<td colspan=3>" +
	 "Protect Against SQL Injection<br>" +
	 "<input type=checkbox id=\"frmActionVariable_PSQL\" style=\"left-margin: -2px\">Escape Single Quotes<br>" +
	 "Protect Against HTML Injection<br>" +
	 "<input type=checkbox id=\"frmActionVariable_PHTML\" style=\"left-margin: -2px\">Escape HTML Symbols<br>" +
	 "Protect Against CODE Injection<br>" +
      "<input type=\"radio\" id=frmActionVariable_PCode name=frmActionVariable_PCode value=\"0\">Do not escape code tags<br>" +     
	  "<input type=\"radio\" id=frmActionVariable_PCode name=frmActionVariable_PCode value=\"1\">Escape code tags ONCE - Rendering tags on OUTPUT<br>" +
      "<input type=\"radio\" id=frmActionVariable_PCode name=frmActionVariable_PCode value=\"2\">Escape code tags TWICE - Displaying tags on OUTPUT" +
	 "</td>" +
    "</tr>" +		
   "</table>"
  },
  {
   "Name" : "Loop",
   "Code" : "Action-Loop",
   "Description" : "Provide a simple conditional Looping operation",
   "onLoad" : "onActionLoad_Loop",
   "onSave" : "onActionSave_Loop",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Loop",   
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Condition Type</td>" +
     "<td>" +
      "<input type=\"radio\" id=frmActionCondition_ConditionType name=frmActionCondition_ConditionType value=\"False\" onclick=\"onfrmActionCondition_ConditionTypeToggle( this.value);\">Standard" +     
      "<input type=\"radio\" id=frmActionCondition_ConditionType name=frmActionCondition_ConditionType value=\"True\" onclick=\"onfrmActionCondition_ConditionTypeToggle( this.value);\">Advanced (Compound)" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=lfthandLabel>Left Condition</div></td>" +
     "<td><div id=lfthandPrompt><input name=frmActionCondition_ConditionLeft id=frmActionCondition_ConditionLeft type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=operatorLabel>Comparison</div></td>" +
     "</td>" +
     "<td><div id=operatorPrompt>" +
     "<select name=frmActionCondition_ConditionOperator id=frmActionCondition_ConditionOperator>" +
      "<option value=\"=\">=</option>" +
      "<option value=\"&lt;&gt;\">&lt;&gt;</option>" +
      "<option value=\"&lt;\">&lt;</option>" +
      "<option value=\"&gt;\">&gt;</option>" +
      "<option value=\"&lt;=\">&lt;=</option>" +
      "<option value=\"&gt;=\">&gt;=</option>" +
     "</select>" +     
     "</div></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=rhthandLabel>Right Condition</div></td>" +
     "<td><div id=rhthandPrompt><input name=frmActionCondition_ConditionRight id=frmActionCondition_ConditionRight type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +
   "</table>"
  }, 
  {
   "Name" : "If",
   "Code" : "Condition-If",
   "Description" : "Provide a simple conditional If statement",
   "onLoad" : "onActionLoad_Condition_If",
   "onSave" : "onActionSave_Condition_If",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Condition_If",   
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Condition Type</td>" +
     "<td><div id=TypePrompt>" +
     "<select name=frmActionCondition_LogicType id=frmActionCondition_LogicType onchange='onClick_Action_Convert(this.options[this.selectedIndex].value);'>" +
      "<option value=\"Condition-If\" selected>If</option>" +
      "<option value=\"Condition-ElseIf\">Else-If</option>" +
      "<option value=\"Condition-Else\">Else</option>" +
     "</select>" +     
      "<input type=\"radio\" id=frmActionCondition_ConditionType name=frmActionCondition_ConditionType value=\"False\" onclick=\"onfrmActionCondition_ConditionTypeToggle( this.value);\">Standard" +     
      "<input type=\"radio\" id=frmActionCondition_ConditionType name=frmActionCondition_ConditionType value=\"True\" onclick=\"onfrmActionCondition_ConditionTypeToggle( this.value);\">Advanced (Compound)" +
	       "</div>" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=lfthandLabel>Left Condition</div></td>" +
     "<td><div id=lfthandPrompt><input name=frmActionCondition_ConditionLeft id=frmActionCondition_ConditionLeft type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=operatorLabel>Comparison</div></td>" +
     "</td>" +
     "<td><div id=operatorPrompt>" +
     "<select name=frmActionCondition_ConditionOperator id=frmActionCondition_ConditionOperator>" +
      "<option value=\"=\">=</option>" +
      "<option value=\"&lt;&gt;\">&lt;&gt;</option>" +
      "<option value=\"&lt;\">&lt;</option>" +
      "<option value=\"&gt;\">&gt;</option>" +
      "<option value=\"&lt;=\">&lt;=</option>" +
      "<option value=\"&gt;=\">&gt;=</option>" +
     "</select>" +     
     "</div></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=rhthandLabel>Right Condition</div></td>" +
     "<td><div id=rhthandPrompt><input name=frmActionCondition_ConditionRight id=frmActionCondition_ConditionRight type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +
   "</table>"
  },  
  {
   "Name" : "Else If",
   "Code" : "Condition-ElseIf",
   "Description" : "Provide a simple conditional Else If statement",
   "onLoad" : "onActionLoad_Condition_If",
   "onSave" : "onActionSave_Condition_If",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Condition_If",   
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Condition Type</td>" +
     "<td><div id=TypePrompt>" +
     "<select name=frmActionCondition_LogicType id=frmActionCondition_LogicType onchange='onClick_Action_Convert(this.options[this.selectedIndex].value);'>" +
      "<option value=\"Condition-If\">If</option>" +
      "<option value=\"Condition-ElseIf\" selected>Else-If</option>" +
      "<option value=\"Condition-Else\">Else</option>" +
     "</select>" +  
      "<input type=\"radio\" id=frmActionCondition_ConditionType name=frmActionCondition_ConditionType value=\"False\" onclick=\"onfrmActionCondition_ConditionTypeToggle( this.value);\">Standard" +     
      "<input type=\"radio\" id=frmActionCondition_ConditionType name=frmActionCondition_ConditionType value=\"True\" onclick=\"onfrmActionCondition_ConditionTypeToggle( this.value);\">Advanced (Compound)" +
	  "</div>" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=lfthandLabel>Left Condition</div></td>" +
     "<td><div id=lfthandPrompt><input name=frmActionCondition_ConditionLeft id=frmActionCondition_ConditionLeft type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=operatorLabel>Comparison</div></td>" +
     "</td>" +
     "<td><div id=operatorPrompt>" +
     "<select name=frmActionCondition_ConditionOperator id=frmActionCondition_ConditionOperator>" +
      "<option value=\"=\">=</option>" +
      "<option value=\"&lt;&gt;\">&lt;&gt;</option>" +
      "<option value=\"&lt;\">&lt;</option>" +
      "<option value=\"&gt;\">&gt;</option>" +
      "<option value=\"&lt;=\">&lt;=</option>" +
      "<option value=\"&gt;=\">&gt;=</option>" +
     "</select>" +     
     "</div></td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=rhthandLabel>Right Condition</div></td>" +
     "<td><div id=rhthandPrompt><input name=frmActionCondition_ConditionRight id=frmActionCondition_ConditionRight type=textbox style=\"width:100%;\"></div></td>" +
    "</tr>" +
   "</table>"
  },  
  {
   "Name" : "Else",
   "Code" : "Condition-Else",
   "Description" : "Provide a simple conditional Else statement",
   "onLoad" : "",
   "onSave" : "onActionSave_Condition_Else",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Condition_Else",   
   "Template" : 
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151><div id=TypeLabel>Logic Type</div></td>" +
     "</td>" +
     "<td><div id=TypePrompt>" +
     "<select name=frmActionCondition_LogicType id=frmActionCondition_LogicType onchange='onClick_Action_Convert(this.options[this.selectedIndex].value);'>" +
      "<option value=\"Condition-If\">If</option>" +
      "<option value=\"Condition-ElseIf\">Else-If</option>" +
      "<option value=\"Condition-Else\" selected>Else</option>" +
     "</select>" +     
     "</div></td>" +
    "</tr>" +
   "</table>"   
  },    
  {
   "Name" : "Select",
   "Code" : "Condition-Select",
   "Description" : "Provide a select conditional statement",
   "onLoad" : "onActionLoad_Condition_Select",
   "onSave" : "onActionSave_Condition_Select",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Condition_Select",   
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Value</td>" +
     "<td><input name=frmActionSelect_Value id=frmActionSelect_Value type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
   "</table>"
  },    
  {
   "Name" : "Case",
   "Code" : "Condition-Case",
   "Description" : "Provide a simple conditional Case statement",
   "onLoad" : "onActionLoad_Condition_Case",
   "onSave" : "onActionSave_Condition_Case",
   "onDelete" : "onActionDelete",
   "onPrint" : "onActionPrint_Condition_Case",  
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Condition</td>" +
     "<td><input name=frmActionCase_Condition id=frmActionCase_Condition type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +
   "</table>"
  },
  {
   "Name" : "Region",
   "Code" : "Action-Region",
   "Description" : "",
   "onLoad" : "onActionLoad_Region",
   "onSave" : "onActionSave_Region",
   "onPrint" : "onActionPrint_Region",   
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td class=SubHead width=151>Region</td>" +
     "<td><input name=frmActionRegion_Name id=frmActionRegion_Name type=textbox style=\"width:100%;\"></td>" +
    "</tr>" +  
    "<tr>" +
     "<td class=SubHead width=151>Condition Type</td>" +
     "<td>" +
      "<input type=\"radio\" id=frmActionRegion_Type name=frmActionRegion_Type value=\"0\">Both" +     
	  "<input type=\"radio\" id=frmActionRegion_Type name=frmActionRegion_Type value=\"1\">Page Load (only)" +
      "<input type=\"radio\" id=frmActionRegion_Type name=frmActionRegion_Type value=\"2\">AJAX Request (only)" +
     "</td>" +
    "</tr>" +
    "<tr>" +
     "<td class=SubHead width=151>Debug</td>" +
     "<td><input name=\"frmActionRegion_skipDebug\" id=\"frmActionRegion_skipDebug\" type=\"checkbox\">Disable Debug for all children of this region</td>" +
    "</tr>" +      
    "<tr>" +
     "<td class=SubHead width=151>Search</td>" +
     "<td><input name=\"frmActionRegion_includeSearch\" id=\"frmActionRegion_includeSearch\" type=\"checkbox\">Include Action Sequence in Search Integration</td>" +
    "</tr>" +    
    "<tr>" +
     "<td class=SubHead width=151>Portable</td>" +
     "<td><input name=\"frmActionRegion_includeImport\" id=\"frmActionRegion_includeImport\" type=\"checkbox\">Include Action Sequence in Import<br/>" +
     "<input name=\"frmActionRegion_includeExport\" id=\"frmActionRegion_includeExport\" type=\"checkbox\">Include Action Sequence in Export</td>" +
    "</tr>" +    
   "</table>"
  }   
  ];

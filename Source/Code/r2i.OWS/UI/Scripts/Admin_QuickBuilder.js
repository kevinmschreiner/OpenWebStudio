
function QuickBuilder() {
showBlock($('qryQB'));
$('qryQB').innerHTML=QuickBuilderTemplate;
}

function genQBSQL(grp){
if (document.getElementById('QBchk'+grp).checked) {
	switch(grp.toUpperCase())
	{case 'GRID':
		getQBQuery('Grid','LIST',qryResults,document.getElementById('QBtSelLst'));
		getQBQuery('Grid','DELETE',qryResults,document.getElementById('QBtDelete')); break;
	case 'VIEW':
		getQBQuery('View','VIEW',qryResults,document.getElementById('QBtSel')); break;
	case 'EDIT':
		getQBQuery('Edit','VIEW',qryResults,document.getElementById('QBtFetch'));
		getQBQuery('Edit','ADD',qryResults,document.getElementById('QBtInsert'));
		getQBQuery('Edit','EDIT',qryResults,document.getElementById('QBtUpdate')); break;
	}
}
}
function genQBSQLAll(){
genQBSQL('Grid');genQBSQL('View');genQBSQL('Edit');
}

function genQB(){
try {
	var qbArr = new Array();
	var Qparams = false;
	var Vparams = false;
	var Eparams = false;
	if (document.getElementById('QBchk'+'Grid').checked)
		Qparams = QBGetJSON('Grid',qryResults);
	if (document.getElementById('QBchk'+'View').checked)
		Vparams = QBGetJSON('View',qryResults);
	if (document.getElementById('QBchk'+'Edit').checked)
		Eparams = QBGetJSON('Edit',qryResults);	


// SECURITY AREA
	var onSRegion = null;
	var qbSIf = null;
	
	if (! document.getElementById('QBradSecureAll').checked) {
		onSRegion = upgradeRegion(onSRegion,'Security Level',sysProperty_ObjectItem);
		var secureString = "[FORMAT,,{ISSUPERUSER}]";
		if (document.getElementById('QBradSecureRole').checked && document.getElementById('QBtxtSecureRoleName').value.length > 0) {
			var rolename = document.getElementById('QBtxtSecureRoleName').value;
			rolename = rolename.replace(/\"/,"\\\"");
			secureString = "[FORMAT,\"" + rolename + "\",{ISINROLE}]";
		}
		qbSIf = new Action("Condition-If",{"LeftCondition":secureString,"RightCondition":"True","Operator":"<>","IsAdvanced":"False"});
		onSRegion.ChildActions.push(qbSIf);
		qbSIf.ChildActions.push(new Action("Action-Redirect",{"Type":"Link","GroupStatement":"","GroupIndex":"","Link":"/","PageID":""}));
	}

 //ADD the general PROPERTIES FIRST---
 //Assign, Assign and Control Header
 //CREATE a keyed array with the handlers contained within - switching to Condition-ElseIf when necessary.

	var ifKey = '';
	var elseKey = '';
	var dForm = false;
	var onGRegion = null;
	var btnObj = false;
	var btnDisabled = '';
	var onQBGenRegion = null;
	onQBGenRegion = upgradeRegion(onQBGenRegion,'Quick Builder Generated');
	
	onGRegion = upgradeRegion(onGRegion,'Global Variables',onQBGenRegion);
	//ADD THE PROMPT DIV ASSIGNMENT
	onGRegion.ChildActions.push(new Action("Action-Assignment",{"Type":"<Action>","Name":"PromptID","Value":"divPrompt[ModuleID,System]","SkipProcessing":"false","AssignmentType":"0"}));
	//ADD THE ADD/EDIT DIV ASSIGNMENT
	onGRegion.ChildActions.push(new Action("Action-Assignment",{"Type":"<Action>","Name":"AddEditID","Value":"divAdd[ModuleID,System]","SkipProcessing":"false","AssignmentType":"0"}));
	//ADD THE LIST DIV ASSIGNMENT
	onGRegion.ChildActions.push(new Action("Action-Assignment",{"Type":"<Action>","Name":"ListID","Value":"divList[ModuleID,System]","SkipProcessing":"false","AssignmentType":"0"}));
	//ADD THE GRID DIV ASSIGNMENT
	onGRegion.ChildActions.push(new Action("Action-Assignment",{"Type":"<Action>","Name":"GridID","Value":"divGrid[ModuleID,System]","SkipProcessing":"false","AssignmentType":"0"}));
	
	var onRPageHeader = null;
	onRPageHeader = upgradeRegion(onRPageHeader,'Control Header',onQBGenRegion);
	onRPageHeader.Parameters.RenderType = 1; //PAGE LOAD ONLY

    configuration.javascriptInclude = ['jquery','jquery.noconflict','jquery.thickbox','jquery.thickbox.css'];
	
	//HEADER
	dForm = new Array();
	dForm.push('<style>\n\t.HTbl {border: 1px solid #cccccc; font-family: arial; font-size: 11px; color: #000000;}\n\t.HTblHdr {border-bottom: 1px solid #cccccc; margin-left: 6px;margin-right: 6px; background: #e8e8e8; font-weight: bold;}\n\t.HTblRow {background:#f6f6f6;}\n\t.HTblRowAlt{background: #e8e8e8;}');
	dForm.push('</style>');
	dForm.push('<div id=\"rsltHandler\"></div>');
	//IF A GRID, CREATE THE ADD FORM AND ANIMATION
	if (Qparams) {
		dForm.push('<div id="[PromptID,Action]" style="display:none;">');
		btnObj = QBGetBySysIndex(Qparams,-2);
		if (!btnObj.Click)
			btnDisabled = ' disabled="disabled" ';
		else
			btnDisabled = '';
		if (btnObj.Show) {

			dForm.push('\t<table width="100%" border="0">');
			dForm.push('\t\t<tr>');
			dForm.push('\t\t\t<td width="100%" class="Normal" align="center">');
			dForm.push('\t\t\t\tManage your data in the box provided below, click Add to create a new record.');
			dForm.push('\t\t\t</td>');
			dForm.push('\t\t\t<td>');
			if (Qparams.Transitions)
			{
				dForm.push('\t\t\t\t<button ' + btnDisabled + ' onclick="$jq(\'#[AddEditID,A]\').slideUp(\'slow\',function(){ows.Fetch(\'[ModuleID,System]\',-1,\'Action=Add\',\'[AddEditID,A]\')});return false;">Add</button>');
			}
			else
			{
				dForm.push('\t\t\t\t<button ' + btnDisabled + ' onclick="ows.Fetch(\'[ModuleID,System]\',-1,\'Action=Add\',\'[AddEditID,A]\');return false;">Add</button>');
			}
			dForm.push('\t\t\t</td>');
			dForm.push('\t\t</tr>');
			dForm.push('\t</table>');
		
		}
		dForm.push('</div>');
		dForm.push('<div id="[AddEditID,Action]"></div>');		
		dForm.push('<hr/>');
		if (Qparams.Transitions)
			dForm.push('<div id="[GridID,Action]" style="display:none;">');		
	}
	onRPageHeader.ChildActions.push(new Action("Action-Assignment",{"Type":"CONTROL.HEADER","Name":"","SkipProcessing":"False","AssignmentType":"0","Value":dForm.join('\n')}));
	
	//FOOTER
	dForm = new Array();
	if (Qparams && Qparams.Transitions) {
		dForm.push('</div>');		
		dForm.push('<script type="text/javascript">');
		dForm.push('\t$jq(\'#[PromptID,Action]\').slideDown(\'slow\');');
		dForm.push('\t$jq(\'#[GridID,Action]\').slideDown(\'slow\');');
		dForm.push('</script>');
	}
	onRPageHeader.ChildActions.push(new Action("Action-Assignment",{"Type":"CONTROL.FOOTER","Name":"","SkipProcessing":"False","AssignmentType":"0","Value":dForm.join('\n')}));	
		
	var onRRegion = null;
	onRRegion = upgradeRegion(onRRegion,'Template onRender',onQBGenRegion);

	//ALL GRID FUNCTIONALITY
	dForm = new Array();
		
	//ALL EDIT FUNCTIONALITY
	if (Eparams) {
		elseKey = 'EDIT';
		ifKey = 'UPDATE';
		qbArr['EDIT'] = new Action("Condition-ElseIf",{"LeftCondition":"'[Action,Querystring]'","RightCondition":"'Edit'","Operator":"=","IsAdvanced":"False"});
		qbArr['ADD'] =  new Action("Condition-ElseIf",{"LeftCondition":"'[Action,Querystring]'","RightCondition":"'Add'","Operator":"=","IsAdvanced":"False"});
		qbArr['UPDATE'] = new Action("Condition-ElseIf",{"LeftCondition":"'[Action,Querystring]'","RightCondition":"'Update'","Operator":"=","IsAdvanced":"False"});
		qbArr['SAVE'] = new Action("Condition-If",{"LeftCondition":"'[Action,Querystring]'","RightCondition":"'Save'","Operator":"=","IsAdvanced":"False"});	
			
		var kName = getQBItems(Eparams.Keys,'[[COLUMN]]','',false);
		var qkName = getQBItems(Eparams.Keys,'['+Eparams.Prefix+'[COLUMN]'+Eparams.Postfix+',Querystring]','',false);
		var kQLst = getQBItems(Eparams.Keys,Eparams.Prefix+'[COLUMN]'+Eparams.Postfix+'=[[COLUMN]]','&',false)
		
		if (Qparams)
		{
			ifKey = 'CANCEL';
			qbArr['CANCEL'] = new Action("Condition-ElseIf",{"LeftCondition":"'[Action,Querystring]'","RightCondition":"'Cancel'","Operator":"=","IsAdvanced":"False"});
		
			var cForm = new Array();
			
			if (Eparams.Transitions) {
				cForm.push('<script type="text/javascript">');
				cForm.push('$jq(\'#[PromptID,A]\').slideDown(\'slow\');');				
				cForm.push('$jq(\'#[AddEditID,A]\').slideUp(\'slow\');');
				cForm.push('<\/script>');
			}
			
			qbArr['CANCEL'].ChildActions.push(new Action("Template",{"Type":"Detail-NoQuery","GroupStatement":"","GroupIndex":"","Value":cForm.join('\n')}));
		}
		
		var eForm = new Array();
		//ASSEMBLE THE EDIT FORM
			eForm.push('<table width="100%" border="0">');

			for (var i=0;i<Eparams.Items.length;i++)
			{
				if (Eparams.Items[i].Key >= 0 && Eparams.Items[i].Column!=null)
				{
				eForm.push('\t<tr>');
				eForm.push('\t\t<td class="SubHead">'+Eparams.Items[i].Label+'</td>');
				eForm.push('\t\t<td class="Normal">');
				if (Eparams.Items[i].Column.Type.toLowerCase()=='boolean')
				{eForm.push('\t\t\t<input type="checkbox" name="'+Eparams.FormPrefix+Eparams.Items[i].Column.Name+Eparams.FormPostfix+kName+'" value="1" {IIF, "\''+Eparams.Items[i].Value+'\' = \'True\'", "checked", ""}/>');}
				else
				{eForm.push('\t\t\t<input type="textbox" name="'+Eparams.FormPrefix+Eparams.Items[i].Column.Name+Eparams.FormPostfix+kName+'" value="'+Eparams.Items[i].Value+'"/>');}
				eForm.push('\t\t</td>');
				eForm.push('\t</tr>');
				}
			}
			eForm.push('<tr><td colspan="2" align="center">');
		
		//SAVE		
			btnObj = QBGetBySysIndex(Eparams,-5);
			if (!btnObj.Click)
				btnDisabled = ' disabled="disabled" ';
			else
				btnDisabled = '';
			if (btnObj.Show) {
				eForm.push('\t\t<button type="button" onclick="');
			if (Eparams.Transitions) {
				eForm.push('\t\t\t$jq(\'#o[ModuleID,System]erow'+kName+'\').fadeOut(\'slow\',function(){ows.Fetch(\'[ModuleID,System]\',-1,\'Action=Update&'+kQLst+'\',\'rsltHandler\')});');}
			else
				{eForm.push('\t\t\tdocument.getElementById(\'o[ModuleID,System]erow'+kName+'\').style.display=\'none\';ows.Fetch(\'[ModuleID,System]\',-1,\'Action=Update&'+kQLst+'\',\'rsltHandler\')');}
			eForm.push('\t\t" ' + btnDisabled + '>Save</button>');
			}
			
		//CANCEL
			btnObj = QBGetBySysIndex(Eparams,-4);
			if (!btnObj.Click)
				btnDisabled = ' disabled="disabled" ';
			else
				btnDisabled = '';			
			if (btnObj.Show) {
			if (Eparams.Transitions) {
				eForm.push('\t\t<button ' + btnDisabled + ' type="button" onclick="$jq(\'#o[ModuleID,System]erow'+kName+'\').fadeOut(\'slow\',function() {$jq(\'#o[ModuleID,System]row'+kName+'\').fadeIn(\'slow\');});return false;">Cancel</button>');
				}
			else 
				eForm.push('\t\t<button ' + btnDisabled + ' type="button" onclick="document.getElementById(\'o[ModuleID,System]erow'+kName+'\').style.display=\'none\';document.getElementById(\'o[ModuleID,System]row'+kName+'\').style.display=\'block\';return false;">Cancel</button>');
			}
			
			eForm.push('</td></tr>');
			eForm.push('</table>');
			
			
			if (Eparams.Transitions)
			{
				eForm.push('<script type="text\/javascript">');
				 eForm.push('\t$jq(\'#o[ModuleID,System]row'+kName+'\').fadeOut(\'slow\',function() {  $jq(\'#o[ModuleID,System]erow'+kName+'\').fadeIn(\'slow\'); });');
				eForm.push('<\/script>');
			}
			else
			{
				eForm.push('<script type="text/javascript">');
				 eForm.push('\tdocument.getElementById(\'o[ModuleID,System]row'+kName+'\').style.display=\'none\';document.getElementById(\'o[ModuleID,System]erow'+kName+'\').style.display=\'block\';');
				eForm.push('<\/script>');
			}
			
			//ADD FORM
			var aForm = new Array();
			aForm.push('<table width="100%" border="0">');
			for (var i=0;i<Eparams.Items.length;i++)
			{
				if (Eparams.Items[i].Key >= 0 && Eparams.Items[i].Column!=null)
				{
				aForm.push('\t<tr>');
				aForm.push('\t\t<td class="SubHead">'+Eparams.Items[i].Label+'</td>');
				aForm.push('\t\t<td class="Normal">');
				if (Eparams.Items[i].Column.Type.toLowerCase()=='boolean')
				{aForm.push('\t\t\t<input type="checkbox" name="'+Eparams.FormPrefix+Eparams.Items[i].Column.Name+Eparams.FormPostfix+'" value="1"/>');}
				else
				{aForm.push('\t\t\t<input type="textbox" name="'+Eparams.FormPrefix+Eparams.Items[i].Column.Name+Eparams.FormPostfix+'"  />');}
				aForm.push('\t\t</td>');
				aForm.push('\t</tr>');
				}
			}
			
			aForm.push('<tr><td colspan="2" align="center">');
			//SAVE
			btnObj = QBGetBySysIndex(Eparams,-5);
			if (!btnObj.Click)
				btnDisabled = ' disabled="disabled" ';
			else
				btnDisabled = '';
			if (btnObj.Show) {			
			aForm.push('\t<button type="button" onclick="');
			if (Eparams.Transitions) {
				aForm.push('\t\t$jq(\'#[AddEditID,A]\').slideUp(\'slow\');$jq(\'#[ListID,A]\').slideUp(\'slow\',function(){ows.Fetch(\'[ModuleID,System]\',0,\'Action=Save\');});');}
			else
				{aForm.push('\t\tows.Fetch(\'[ModuleID,System]\',0,\'Action=Save\');');}
			aForm.push('\t" ' + btnDisabled + '>Save</button>');
			}
			
			//CANCEL
			btnObj = QBGetBySysIndex(Eparams,-4);
			if (!btnObj.Click)
				btnDisabled = ' disabled="disabled" ';
			else
				btnDisabled = '';
			if (btnObj.Show) {
			if (Eparams.Transitions) {
				aForm.push('\t<button ' + btnDisabled + ' type="button" onclick="$jq(\'#[AddEditID,A]\').slideUp(\'slow\',function(){ows.Fetch(\'[ModuleID,System]\',-1,\'Action=Cancel\',\'[AddEditID,A]\');});return false;">Cancel</button>');
				}
			else 
				aForm.push('\t<button ' + btnDisabled + ' type="button" onclick="ows.Fetch(\'[ModuleID,System]\',-1,\'Action=Cancel\',\'[AddEditID,A]\');">Cancel</button>');
			}
			
			aForm.push('</td></tr>');
			aForm.push('</table>');
			
			if (Eparams.Transitions)
			{
				aForm.push('<script type="text\/javascript">');
				 aForm.push('\t $jq(\'#[AddEditID,A]\').slideDown(\'slow\');');
				aForm.push('<\/script>\n');
			}
			
			qbArr['ADD'].ChildActions.push(new Action("Template",{"Type":"Detail-NoQuery","GroupStatement":"","GroupIndex":"","Value":aForm.join('\n')}));
			
			
			for (var i=0;i<Eparams.Keys.length;i++)
			{
				qbArr['EDIT'].ChildActions.push(new Action("Template-Variable",{"VariableType":"<QueryString>","QuerySource":Eparams.Prefix + Eparams.Keys[i].Column.Name + Eparams.Postfix,"QueryTarget":'@' + Eparams.Prefix +Eparams.Keys[i].Column.Name + Eparams.Postfix,"QueryTargetLeft":"\'","QueryTargetRight":"\'","QueryTargetEmpty":"NULL","EscapeListX":"1","Protected":"true","EscapeHTML":"true"}));
			}
			qbArr['EDIT'].ChildActions.push(new Action("Template",{"Type":"Query-Query","GroupStatement":"","GroupIndex":"","Value":document.getElementById('QBtFetch').value}));
			qbArr['EDIT'].ChildActions.push(new Action("Template",{"Type":"Detail-Detail","GroupStatement":"","GroupIndex":"","Value":eForm.join('\n')}));
			
			for (var i=0;i<Eparams.Keys.length;i++)
			{
				qbArr['UPDATE'].ChildActions.push(new Action("Template-Variable",{"VariableType":"<QueryString>","QuerySource":Eparams.Prefix +Eparams.Keys[i].Column.Name+ Eparams.Postfix,"QueryTarget":'@' + Eparams.Prefix +Eparams.Keys[i].Column.Name + Eparams.Postfix,"QueryTargetLeft":"\'","QueryTargetRight":"\'","QueryTargetEmpty":"NULL","EscapeListX":"1","Protected":"true","EscapeHTML":"true"}));
			}
			for (var i=0;i<Eparams.Items.length;i++)
			{
				if (Eparams.Items[i].System>=0)
					qbArr['UPDATE'].ChildActions.push(new Action("Template-Variable",{"VariableType":"<Form>","QuerySource":Eparams.FormPrefix +Eparams.Items[i].Column.Name+ Eparams.FormPostfix+qkName,"QueryTarget":'@' + Eparams.FormPrefix +Eparams.Items[i].Column.Name + Eparams.FormPostfix,"QueryTargetLeft":"\'","QueryTargetRight":"\'","QueryTargetEmpty":"NULL","EscapeListX":"1","Protected":"true","EscapeHTML":"true"}));
				if (Eparams.Items[i].System>=0)
					qbArr['SAVE'].ChildActions.push(new Action("Template-Variable",{"VariableType":"<Form>","QuerySource":Eparams.FormPrefix +Eparams.Items[i].Column.Name+ Eparams.FormPostfix,"QueryTarget":'@' + Eparams.FormPrefix +Eparams.Items[i].Column.Name + Eparams.FormPostfix,"QueryTargetLeft":"\'","QueryTargetRight":"\'","QueryTargetEmpty":"NULL","EscapeListX":"1","Protected":"true","EscapeHTML":"true"}));
			}
			qbArr['UPDATE'].ChildActions.push(new Action("Action-Execute",{"Name":"UPDATE","Query":document.getElementById('QBtUpdate').value,"IsProcess":"False","Connection":""}));
			qbArr['SAVE'].ChildActions.push(new Action("Action-Execute",{"Name":"SAVE","Query":document.getElementById('QBtInsert').value,"IsProcess":"False","Connection":""}));
			qbArr['UPDATE'].ChildActions.push(new Action("Template",{"Type":"Query-Query","GroupStatement":"","GroupIndex":"","Value":document.getElementById('QBtFetch').value}));
			
			if (Qparams)
			{
				eForm = new Array();
				eForm.push('<script type="text/javascript">');
				var iShow = 0;
				for (var i=0;i<Qparams.Items.length;i++)
				{
					if (Qparams.Items[i].System>=0 && !Qparams.Items[i].Key && Qparams.Items[i].Show)
					{
						eForm.push('\t$jq(\'#o[ModuleID,System]row'+kName+' td:eq('+i+')\').html(\''+Qparams.Items[i].Value+'\');');
					}
					if (Qparams.Items[i].Show)
						iShow++;
				}
				if (Eparams.Transitions)
				eForm.push('\t$jq(\'#o[ModuleID,System]erow'+kName+'\').fadeOut(\'slow\',function() {  $jq(\'#o[ModuleID,System]row'+kName+'\').fadeIn(\'slow\'); });');
				eForm.push('</script>');
				qbArr['UPDATE'].ChildActions.push(new Action("Template",{"Type":"Detail-Detail","GroupStatement":"","GroupIndex":"","Value":eForm.join('\n')}));
			}
			else
				qbArr['UPDATE'].ChildActions.push(new Action("Template",{"Type":"Detail-Detail","GroupStatement":"","GroupIndex":"","Value":eForm.join('\n')}));
		}	
	
		//ALL VIEW FUNCTIONALITY
	
	if (Vparams) {
		elseKey = 'VIEW';
		qbArr['VIEW'] = new Action("Condition-ElseIf",{"LeftCondition":"'[Action,Querystring]'","RightCondition":"'View'","Operator":"=","IsAdvanced":"False"});
		
		var dForm = new Array();
		var kName = getQBItems(Vparams.Keys,'[[COLUMN]]','',false);
		var kQLst = getQBItems(Vparams.Keys,Vparams.Prefix+'[COLUMN]'+Vparams.Postfix+'=[[COLUMN]]','&',true)
		dForm.push('<table width="100%" border="0">');
		for (var i=0;i<Vparams.Items.length;i++)
		{
			if (Vparams.Items[i].Key >= 0 && Vparams.Items[i].Column!=null)
			{
			dForm.push('\t<tr>');
			dForm.push('\t\t<td class="SubHead">'+Vparams.Items[i].Label+'</td>');
			dForm.push('\t\t<td class="Normal">');
			if (Vparams.Items[i].Column.Type.toLowerCase()=='boolean')
			{dForm.push('\t\t\t'+Vparams.Items[i].Value+'');}
			else
			{dForm.push('\t\t\t'+Vparams.Items[i].Value+'');}
			dForm.push('\t\t</td>');
			dForm.push('\t</tr>');
			}
		}
		
		dForm.push('<tr><td colspan="2" align="center">');
		
		//CANCEL
		btnObj = QBGetBySysIndex(Vparams,-4);
		if (!btnObj.Click)
			btnDisabled = ' disabled="disabled" ';
		else
			btnDisabled = '';
		if (btnObj.Show) {
		if (Vparams.Transitions)
			dForm.push('<button ' + btnDisabled + ' type="button" onclick="tb_remove();return false;">Cancel</button>');
		else 
			dForm.push('<button ' + btnDisabled + ' type="button" onclick="document.getElementById(\'#o[ModuleID,System]erow'+kName+'\').style.display=\'none\';document.getElementById(\'#o[ModuleID,System]row'+kName+'\').style.display=\'block\';return false;">Cancel</button>');
		}
			
		dForm.push('</td></tr>');
		dForm.push('</table>');
		
		if (!Vparams.Transitions)
		{
			dForm.push('<script type="text/javascript">');
			 dForm.push('\tdocument.getElementById(\'o[ModuleID,System]row'+kName+'\').style.display=\'none\';document.getElementById(\'o[ModuleID,System]erow'+kName+'\').style.display=\'block\';');
			dForm.push('<\/script>');
		}	
	
		for (var i=0;i<Vparams.Keys.length;i++)
		{
			qbArr['VIEW'].ChildActions.push(new Action("Template-Variable",{"VariableType":"<QueryString>","QuerySource":Vparams.Prefix +Vparams.Keys[i].Column.Name + Vparams.Postfix,"QueryTarget":'@' + Vparams.Prefix +Vparams.Keys[i].Column.Name + Vparams.Postfix,"QueryTargetLeft":"\'","QueryTargetRight":"\'","QueryTargetEmpty":"NULL","EscapeListX":"1","Protected":"true","EscapeHTML":"true"}));
		}
	
		var vqaction = new Action("Template",{"Type":"Query-Query","GroupStatement":"","GroupIndex":"","Value":document.getElementById('QBtSel').value});
		qbArr['VIEW'].ChildActions.push(vqaction);
		vqaction.ChildActions.push(new Action("Template",{"Type":"Detail-Detail","GroupStatement":"","GroupIndex":"","Value":dForm.join('\n')}));
		
		dForm=new Array();
		dForm.push('<table width="100%" border="0">');
		dForm.push('\t<tr>');
		dForm.push('\t\t<td width="100%" class="Normal" align="center">');
		dForm.push('\t\t\tThere are currently no details for this record.');
		dForm.push('\t\t</td>');
		dForm.push('\t</tr>');
		dForm.push('</table>');
		qbArr['VIEW'].ChildActions.push(new Action("Template",{"Type":"Detail-NoQuery","GroupStatement":"","GroupIndex":"","Value":dForm.join('\n')}));
		
	}	
	if (Qparams) {
		elseKey = 'GRID';
		if (ifKey.length==0) ifKey = 'DELETE';
		
		qbArr['DELETE'] = new Action("Condition-ElseIf",{"LeftCondition":"'[Action,Querystring]'","RightCondition":"'Delete'","Operator":"=","IsAdvanced":"False"});
		qbArr['GRID'] = new Action("Condition-ElseIf",{"LeftCondition":"'[Action,Querystring]'","RightCondition":"'Grid'","Operator":"=","IsAdvanced":"False"});
		
		var qForm = new Array();
		var kName = getQBItems(Qparams.Keys,'[[COLUMN]]','',false);
		var kQLst = getQBItems(Qparams.Keys,Qparams.Prefix+'[COLUMN]'+Qparams.Postfix+'=[[COLUMN]]','&',false)

		qForm.push('<div id="[ListID,A]">');
		qForm.push('\t<table cellspacing="0" cellpadding="0" width="100%" class="HTbl">');
		qForm.push('\t\t<tbody>');
		qForm.push('\t\t<tr class="HTblHdr">');
		
		for (var i=0;i<Qparams.Items.length;i++)
		{
			if (Qparams.Items[i].Label.length > 0)
			{
				if (Qparams.Items[i].System*1>=0 && Qparams.isSorted)
				{
					qForm.push('\t\t\t<td><a {SORT,"'+Qparams.Items[i].Column.Name+'","'+Qparams.Items[i].Label+'","'+Qparams.Items[i].Label+' (asc)","'+Qparams.Items[i].Label+' (desc)",,"'+i+'"}>{SORTHEADER,"'+i+'"}</a></td>');
				}
				else
					qForm.push('\t\t\t<td>'+Qparams.Items[i].Label+'</td>');
			}
			else
			{
				qForm.push('\t\t\t<td>&nbsp;</td>');
			}
		}
		qForm.push('\t\t</tr>');
		//APPEND TO NEW ACTION AND THEN BUILD DETAIL
		var qGridHeader = new Action("Template",{"Type":"Group-Header","GroupStatement":"","GroupIndex":"0","Value":qForm.join('\n')});
		
		qForm = new Array(); //CLEAR
		
		qForm.push('\t\t<tr class="HTblRow{ALTERNATE,classes,,Alt}" id="o[ModuleID,System]row'+kName+'">');
		var colI = 0;
		for (var i=0;i<Qparams.Items.length;i++)
		{
			var isA = false;
			qForm.push('\t\t\t<td>');
			if (Qparams.Items[i].Click) {
				switch(Qparams.Items[i].System*1)
				{
					case -3: //VIEW
						if (Qparams.Transitions)
							qForm.push('\t\t\t\t<a href="#TB_inline?height=255&width=700&inlineId=hiddenModalContent" onclick="ows.Fetch(\'[ModuleID,System]\',-1,\'Action=View&'+kQLst+'\',\'TB_ajaxContent\');" class="thickbox">');
						else
							qForm.push('\t\t\t\t<a href="#" onclick="ows.Fetch(\'[ModuleID,System]\',-1,\'Action=View&'+kQLst+'\',\'o[ModuleID,System]erow'+kName+'\');return false;">');
						isA=true;
						break;
					case -2: //EDIT
						qForm.push('\t\t\t\t<a href="#" onclick="\t');
						qForm.push('\t\t\t\t\tows.Fetch(\'[ModuleID,System]\',-1,\'Action=Edit&'+kQLst+'\',\'o[ModuleID,System]erow'+kName+'\');return false;');
						qForm.push('\t\t\t\t">');
						isA=true;
						break;					
					case -1: //DELETE
						qForm.push('\t\t\t\t<a href="#" onclick="\t');
						if (Qparams.Transitions)
							qForm.push('\t\t\t\t\t$jq(\'#o[ModuleID,System]row'+kName+'\').fadeOut(\'slow\',function() {ows.Fetch(\'[ModuleID,System]\',-1,\'Action=Delete&'+kQLst+'\',\'null\')});return false;');
						else
							qForm.push('\t\t\t\t\tdocument.getElementById(\'o[ModuleID,System]row'+kName+'\').style.display=\'none\';ows.Fetch(\'[ModuleID,System]\',-1,\'Action=Delete&'+kQLst+'\',\'null\');return false;');
						qForm.push('"\t\t\t\t>');
						isA=true;
						break;
				}
				if (Vparams && !isA)
				{
							if (Qparams.Transitions)
								qForm.push('\t\t\t\t<a href="#TB_inline?height=255&width=700&inlineId=hiddenModalContent" onclick="ows.Fetch(\'[ModuleID,System]\',-1,\'Action=View&'+kQLst+'\',\'TB_ajaxContent\');" class="thickbox">');
							else
								qForm.push('\t\t\t\t<a href="#" onclick="ows.Fetch(\'[ModuleID,System]\',-1,\'Action=View&'+kQLst+'\',\'o[ModuleID,System]erow'+kName+'\');">');
							isA=true;
				}
			}
	
			qForm.push('\t\t\t\t\t'+Qparams.Items[i].Value+'');

			if (isA) qForm.push('\t\t\t\t</a>');
				qForm.push('\t\t\t</td>');
			colI++;
		}
		qForm.push('\t\t</tr>');
		qForm.push('\t\t<tr><td  id="o[ModuleID,System]erow'+kName+'" style="display:none;" colspan="'+ colI +'">&nbsp;</td></tr>');

		var qQuery = new Action("Template",{"Type":"Query-Query","GroupStatement":"","GroupIndex":"","Value":document.getElementById('QBtSelLst').value});
		qQuery.ChildActions.push(qGridHeader);
		qGridHeader.ChildActions.push(new Action("Template",{"Type":"Detail-Detail","GroupStatement":"","GroupIndex":"","Value":qForm.join('\n')}));

		//APPEND FOOTER ACTION
		qForm = new Array(); //CLEAR
		qForm.push('\t</tbody></table>');
		qForm.push('</div>');
		qForm.push('<hr/>');
		if (Qparams.Transitions) {
			qForm.push('<script type="text/javascript">');
			qForm.push('\t$jq(\'#[ListID,A]\').slideDown(\'slow\');');
			qForm.push('\t$jq(\'#[PromptID,A]\').slideDown(\'slow\');');
			qForm.push('tb_init(\'a.thickbox, area.thickbox, input.thickbox\');');
			qForm.push('<\/script>');
		}
		
		qQuery.ChildActions.push(new Action("Template",{"Type":"Group-Footer","GroupStatement":"","GroupIndex":"0","Value":qForm.join('\n')}));
		qbArr['GRID'].ChildActions.push(qQuery);
		
		qForm=new Array();
		if (Qparams.Transitions)
			qForm.push('<div id="[ListID,A]">');
		else
			qForm.push('<div id="[ListID,A]">');		
		qForm.push('<table width="100%" border="0">');
		qForm.push('\t<tr>');
		qForm.push('\t\t<td width="100%" class="Normal" align="center">');
		qForm.push('\t\t\tThere are currently no details for this record.');
		qForm.push('\t\t</td>');
		qForm.push('\t</tr>');
		qForm.push('</table></div>');
		qForm.push('<hr/>');
		if (Qparams.Transitions){
		    qForm.push('<script type="text/javascript">');
		    qForm.push('\t$jq(document).ready(function() {');
		    qForm.push('\t$jq(\'#[ListID,A]\').slideDown(\'slow\');');
		    qForm.push('\t$jq(\'#[PromptID,A]\').slideDown(\'slow\');');
		    qForm.push('});');
		    qForm.push('<\/script>');
		}
		
		qbArr['GRID'].ChildActions.push(new Action("Template",{"Type":"Detail-NoResults","GroupStatement":"","GroupIndex":"","Value":qForm.join('\n')}));
		
		
		
		for (var i=0;i<Qparams.Keys.length;i++)
		{
			qbArr['DELETE'].ChildActions.push(new Action("Template-Variable",{"VariableType":"<QueryString>","QuerySource":Qparams.Prefix +Qparams.Keys[i].Column.Name + Qparams.Postfix,"QueryTarget":'@' + Qparams.Prefix +Qparams.Keys[i].Column.Name + Qparams.Postfix,"QueryTargetLeft":"\'","QueryTargetRight":"\'","QueryTargetEmpty":"NULL","EscapeListX":"1","Protected":"true","EscapeHTML":"true"}));
		}
		qbArr['DELETE'].ChildActions.push(new Action("Action-Execute",{"Name":"DELETE","Query":document.getElementById('QBtDelete').value,"IsProcess":"False","Connection":""}));
	}

	//ASSEMBLE THE ACTIONS
	if (ifKey.length > 0)
		qbArr[ifKey].ActionType='Condition-If';
	if (elseKey.length > 0)
	{
		qbArr[elseKey].ActionType='Condition-Else';
		qbArr[elseKey].Parameters = {};
	}
		
	if (ifKey.length > 0)
	{
		//ADD THE IF, THEN THE ELSE-IF's THEN THE ELSE to Regin
		onRRegion.ChildActions.push(qbArr[ifKey]);
		if (ifKey!='GRID' && elseKey!='GRID' && qbArr['GRID']!=null) onRRegion.ChildActions.push(qbArr['GRID']);
		if (ifKey!='VIEW' && elseKey!='VIEW' && qbArr['VIEW']!=null) onRRegion.ChildActions.push(qbArr['VIEW']);
		if (ifKey!='EDIT' && elseKey!='EDIT' && qbArr['EDIT']!=null) onRRegion.ChildActions.push(qbArr['EDIT']);
		if (ifKey!='CANCEL' && elseKey!='CANCEL' && qbArr['CANCEL']!=null) onRRegion.ChildActions.push(qbArr['CANCEL']);
		if (ifKey!='ADD' && elseKey!='ADD' && qbArr['ADD']!=null) onRRegion.ChildActions.push(qbArr['ADD']);
		if (ifKey!='UPDATE' && elseKey!='UPDATE' && qbArr['UPDATE']!=null) onRRegion.ChildActions.push(qbArr['UPDATE']);
		if (ifKey!='DELETE' && elseKey!='DELETE' && qbArr['DELETE']!=null) onRRegion.ChildActions.push(qbArr['DELETE']);
		if (ifKey!='SAVE' && elseKey!='SAVE' && qbArr['SAVE']!=null) qbArr[elseKey].ChildActions.insert(qbArr['SAVE'],0);
		onRRegion.ChildActions.push(qbArr[elseKey]);
	}
	else
	{
		//LOOP THROUGH THE CHILD ACTIONS OF elseKey, appending directly to Region
		for (var i = 0; i<qbArr[elseKey].ChildActions.length; i++)
		{
			onRRegion.ChildActions.push(qbArr[elseKey].ChildActions[i]);
		}
	}
	owsSelect.reindex(null);
	owsSelect.expand(onQBGenRegion.GUID);
	alert('All actions have been generated, the builder will remain\n open so that it can be executed again if needed.');
	}
	catch (err)
	{
	    alert('The process failed to generate the action structure: \n' + err.description);
	}
}

function genQBRow(arr,instance,rowindex,i,column,maxcolumn,fname) {
	/*Actions:View|Edit|Delete
	<tr class="HTblRow">
		<td id="qbgr1t1"><a href="#">Up</a>&nbsp;|&nbsp;<a href="#">Down</a></td>
		<td id="qbgr1t2"><input type="checkbox" id="chkQBGs1" name="chkQBGs1"/></td>
		<td id="qbgr1t3">Edit<input name="gbgc" type="hidden" value="-1"/></td>
		<td id="qbgr1t4"><input type="text" id="txtQBGl1" name="txtQBGl1"/></td>
		<td id="qbgr1t5"><input type="text" id="txtQBGc1" name="txtQBGc1"/></td>
		<td id="qbgr1t6"><input type="checkbox" id="chkQBGc1" name="chkQBGc1"/></td>
		<td id="qbgr1t7"><input type="checkbox" id="chkQBGk1" name="chkQBGk1"/></td>
	</tr>*/
	var name = '';
	var sDisplay = 'none';
	var cChecked = 'checked="checked"';
	var s1fname = "";
	var s2fname = fname;
	if (typeof fname=='undefined')
	{
		fname = column.Name;
		name = column.Name + ' (<i>' + column.Type + '</i>)';
		cChecked='';
		s1fname = fname;
		s2fname = '['+fname+']';
		sDisplay = 'block';
	}
	else
		name = fname;
	arr.push('<tr class="HTblRow'+(rowindex%2==0?'':'Alt')+'">');
	arr.push('<td id="qbgr1'+instance+rowindex+'" align="center"><a href="#" onclick="QBMove(\'U\',\''+instance+'\','+rowindex+','+maxcolumn+');genQBSQL(\''+instance+'\');return false;">Up</a>&nbsp;|&nbsp;<a href="#" onclick="QBMove(\'D\',\''+instance+'\','+rowindex+','+maxcolumn+');return false;">Down</a></td>');
	arr.push('<td id="qbgr2'+instance+rowindex+'"><input onclick="genQBSQL(\''+instance+'\');" checked="checked" type="checkbox" id="vQB2'+instance+rowindex+'" value="'+i+'" name="vQB2'+instance+'"/></td>');
	arr.push('<td id="qbgr3'+instance+rowindex+'">'+name+'<input name="vQB'+instance+'" type="hidden" value="'+i+'"/></td>');
	arr.push('<td id="qbgr4'+instance+rowindex+'"><input type="text" id="vQB4'+instance+rowindex+'" name="vQB4'+instance+'" value="'+s1fname+'"/></td>');
	arr.push('<td id="qbgr5'+instance+rowindex+'"><input type="text" id="vQB5'+instance+rowindex+'" name="vQB5'+instance+'" value="'+s2fname+'"/></td>');
	arr.push('<td id="qbgr6'+instance+rowindex+'"><input type="checkbox" '+cChecked+' id="vQB6'+instance+rowindex+'" value="'+i+'" name="vQB6'+instance+'"/></td>');
	arr.push('<td id="qbgr7'+instance+rowindex+'"><input onclick="genQBSQL(\''+instance+'\');" style="display:'+sDisplay+';" type="checkbox" id="vQB7'+instance+rowindex+'" value="'+i+'" name="vQB7'+instance+'"/></td>');

	arr.push('</tr>');
}

function getQBItems(src,tmpl,join,showonly,s,onlyone)
{
	if (typeof onlyone=='undefined')
		onlyone = false;
	if (typeof s=='undefined'||s==null)
		s = new Array();
	for (var i = 0;i < src.length && i >= 0; i++)
	{
		if (src[i].Column!=null&&(((showonly&&src[i].Show&&!src[i].Key)||!showonly)))
		{
			s.push(tmpl.replace(/\[COLUMN\]/g,src[i].Column.Name));
			if (onlyone)
				i = -10;
		}
	}
	if (join!=null)
		return s.join(join);
	else
		return s;
}

function getQBQuery(instance,name,src,trg)
{
	var str = new Array();
	var params = QBGetJSON(instance,src);
	switch(name.toUpperCase())
	{
		case 'LIST': 
			str.push('SELECT\n');
			if (params.Keys.length+params.Items.length>0)
				str.push(getQBItems(params.Keys,'\t[COLUMN]',',\n',false,getQBItems(params.Items,'\t[COLUMN]',null,true))+'\n');
			else
				str.push('--NO ITEM LIST DEFINED, CHECK SHOW IN THE COLUMN GRID\n');
			str.push('');
			str.push('FROM [' + params.Name + ']\n');		
			if (params.isSorted)
			{
				var strArr = getQBItems(params.Keys,'[COLUMN]','',false,null,true);
				if (strArr.length==0)
					strArr = '0';
				str.push('\tORDER BY [SORTTAG,' + strArr + ']'); 
			}
			break;
		case 'VIEW': 
			str.push('SELECT\n');
			if (params.Keys.length+params.Items.length>0)
				str.push(getQBItems(params.Keys,'\t[COLUMN]',',\n',false,getQBItems(params.Items,'\t[COLUMN]',null,true))+'\n');
			else
				str.push('--NO ITEM LIST DEFINED, CHECK SHOW IN THE COLUMN GRID\n');
			str.push('FROM [' + params.Name + ']\n');
			if (params.Keys.length > 0)
			{
				str.push('WHERE\n');
				str.push(getQBItems(params.Keys,'\t[COLUMN]=@'+params.Prefix+'[COLUMN]'+params.Postfix,' AND\n',false)+'\n');
			}	
			else
				str.push('--WHERE\n--NO KEY LIST DEFINED, CHECK KEY IN THE COLUMN GRID\n');			
			break;
		case 'EDIT': 
			str.push('UPDATE\n');
			str.push('\t[' + params.Name + ']\n');
			str.push('SET\n');
			str.push(getQBItems(params.Items,'\t[COLUMN]=@'+params.FormPrefix+'[COLUMN]'+params.FormPostfix,',\n',true)+'\n');
			if (params.Keys.length > 0)
			{
				str.push('WHERE\n');
				str.push(getQBItems(params.Keys,'\t[COLUMN]=@'+params.Prefix+'[COLUMN]'+params.Postfix,' AND\n',false)+'\n');
			}	
			else
				str.push('--WHERE\n--NO KEY LIST DEFINED, CHECK KEY IN THE COLUMN GRID\n');
			break;				
		case 'ADD':
			str.push('INSERT INTO [' + params.Name + ']\n');
			if (params.Keys.length+params.Items.length>0)
				str.push('(\n'+getQBItems(params.Items,'\t[COLUMN]',',\n',true)+'\n)\n');
			else
				str.push('--NO ITEM LIST DEFINED, CHECK SHOW IN THE COLUMN GRID\n');
			str.push('VALUES(\n'+getQBItems(params.Items,'\t@'+params.FormPrefix+'[COLUMN]'+params.FormPostfix,',\n',true)+'\n)\n');
			break;			
		case 'DELETE': 
			str.push('DELETE\n');
			str.push('FROM [' + params.Name + ']\n');
			if (params.Keys.length > 0)
			{
				str.push('WHERE\n');
				str.push(getQBItems(params.Keys,'\t[[COLUMN]]=@'+params.Prefix+'[COLUMN]'+params.Postfix,' AND\n',false)+'\n');
			}
			else
				str.push('--WHERE\n--NO KEY LIST DEFINED, CHECK KEY IN THE COLUMN GRID\n');
			break;
	}
	trg.value = str.join('');
}
function QBGetJSON(instance,rslt)
{
	var ColS=QBJsArray('vQB2'+instance);
	var ColP=QBJsArray('vQB'+instance);
	var ColL=QBJsArray('vQB4'+instance);
	var ColV=QBJsArray('vQB5'+instance);
	var ColC=QBJsArray('vQB6'+instance);
	var ColK=QBJsArray('vQB7'+instance);
	
	var sArray = new Array();
	var kArray = new Array();
	for (var i = 0; i<ColP.length; i++)
	{
		var c = {"Column":null,"System":null,"Label":null,"Value":null,"Show":false,"Click":false,"Key":false};
			if (ColP[i]>=0)
			{
			c.Column = rslt.Columns[ColP[i]];
			c.System = i;
			c.Key = (ColK.indexOf(ColP[i])>=0?true:false);
			}
			else
			{
			c.Column = null;
			c.System = ColP[i];
			c.Key = false;
			}
			c.Label = ColL[i];
			c.Value = ColV[i];
			c.Show = (ColS.indexOf(ColP[i])>=0?true:false);
			c.Click = (ColC.indexOf(ColP[i])>=0?true:false);
			if (c.Key)
				kArray.push(c);
			if (c.Show)
				sArray.push(c);
	}
	return ({"Keys":kArray,"Items":sArray,"Name":document.getElementById('QBtxtName').value,"Transitions":document.getElementById('QBchkTransition').checked,"isSorted":document.getElementById('QBchkSort').checked,"Prefix":document.getElementById('QBtxtPrefix').value,"Postfix":document.getElementById('QBtxtPostfix').value,"FormPrefix":document.getElementById('QBtxtFPrefix').value,"FormPostfix":document.getElementById('QBtxtFPostfix').value});
}
function QBGetBySysIndex(obj,index)
{
for (var i=0;i<obj.Items.length;i++)
		{
			var isA = false;
			if (obj.Items[i].System*1 == index)
			{
				return obj.Items[i];
			}
		}			
	return {"Click":false,"Show":false};
}
function QBJsArray(name,dep)
{
	var sA = document.getElementsByName(name);
	var rV=new Array();
	for (var i = 0; i < sA.length; i++)
	{
		if (sA[i].type.toUpperCase()=='CHECKBOX')
		{if(sA[i].checked) rV.push(sA[i].value);}
		else
		{rV.push(sA[i].value);}
	}
	return rV;
}
function QBMove(direction,instance,rowindex,maxcolumn)
{
	var aI = rowindex;
	var bI = -1;
	switch(direction)
	{
		case 'U':
			if (aI > 0)
			{
				bI = aI-1;
			}
		break;
		default:
			if (aI < maxcolumn && aI >= 0)
			{
				bI = aI+1;
			}
	}
	if (bI>=0)
	{
		var a = new Array();
		var b = new Array();
		QBGetRow(a,instance,aI);
		QBGetRow(b,instance,bI);
		QBSetRow(a,instance,bI);
		QBSetRow(b,instance,aI);
	}
}
function QBGetRow(arr,instance,index)
{	
	arr[0]=document.getElementById('vQB2'+instance+index).checked;
	arr[1]=document.getElementById('vQB2'+instance+index).value;
	arr[2]=document.getElementById('qbgr3'+instance+index).innerHTML;
	arr[3]=document.getElementById('vQB4'+instance+index).value;
	arr[4]=document.getElementById('vQB5'+instance+index).value;
	arr[5]=document.getElementById('vQB6'+instance+index).checked;
	arr[6]=document.getElementById('vQB6'+instance+index).value;
	arr[7]=document.getElementById('vQB7'+instance+index).checked;
	arr[8]=document.getElementById('vQB7'+instance+index).value;
	arr[9]='none';
	if (document.getElementById('vQB7'+instance+index).style.display!='none')
		arr[9]='block';
}
function QBSetRow(arr,instance,index)
{	
	document.getElementById('vQB2'+instance+index).checked=		arr[0];
	document.getElementById('vQB2'+instance+index).value=		arr[1];
	document.getElementById('qbgr3'+instance+index).innerHTML=	arr[2];
	document.getElementById('vQB4'+instance+index).value=		arr[3];
	document.getElementById('vQB5'+instance+index).value=		arr[4];
	document.getElementById('vQB6'+instance+index).checked=		arr[5];
	document.getElementById('vQB6'+instance+index).value=		arr[6];
	document.getElementById('vQB7'+instance+index).checked=		arr[7];
	document.getElementById('vQB7'+instance+index).value=		arr[8];
	document.getElementById('vQB7'+instance+index).style.display=arr[9];

	}
function genQBQuery(name,id,arr)
{
arr.push('<hr/>');
arr.push('<table width="100%" border="0" cellpadding="1" cellspacing="1" class="HTbl">');
	arr.push('<tr>');
		arr.push('<td style="width:60px;">'+name+'</td>');
		arr.push('<td><textarea id="'+id+'" name="'+id+'" style="width:100%; height: 250px;"></textarea></td>');
	arr.push('</tr>');
arr.push('</table>');
}
function genQBTable(instance,rslt,trg,view,edit,del,save,cancel,last){
	var arr = new Array();
	arr.push('<table width="100%" border="0" cellpadding="1" cellspacing="1" class="HTbl">');
	arr.push('<tr class="HTblHdr">'); 
	arr.push('<th>Move</th>'); arr.push('<th>Show</th>'); arr.push('<th>Name</th>'); arr.push('<th>Label</th>'); 	arr.push('<th>Content</th>');	arr.push('<th>Click</th>');	arr.push('<th>isKey</th>');	
	arr.push('</tr>');
	//instance,i,rowindex,column,fname
	var len = rslt.Columns.length-1 + (view+edit+del+save+cancel);
	var rI = 0;
	if (!last) {
	if (save) 	{arr.push(genQBRow(arr,instance,rI,-5,null,len,'Save'));	rI++;}
	if (cancel)	{arr.push(genQBRow(arr,instance,rI,-4,null,len,'Cancel'));	rI++;}		
	if (view)	{arr.push(genQBRow(arr,instance,rI,-3,null,len,'View'));	rI++;}
	if (edit)	{arr.push(genQBRow(arr,instance,rI,-2,null,len,'Edit'));	rI++;}
	if (del)	{arr.push(genQBRow(arr,instance,rI,-1,null,len,'Delete'));	rI++;}
	}
	for (var i = 0; i<rslt.Columns.length; i++)
	{
		genQBRow(arr,instance,rI,i,rslt.Columns[i],len);
		rI+=1;
	}
	if (last) {
	if (save) 	{arr.push(genQBRow(arr,instance,rI,-5,null,len,'Save'));	rI++;}
	if (cancel)	{arr.push(genQBRow(arr,instance,rI,-4,null,len,'Cancel'));	rI++;}		
	if (view)	{arr.push(genQBRow(arr,instance,rI,-3,null,len,'View'));	rI++;}
	if (edit)	{arr.push(genQBRow(arr,instance,rI,-2,null,len,'Edit'));	rI++;}
	if (del)	{arr.push(genQBRow(arr,instance,rI,-1,null,len,'Delete'));	rI++;}
	}
	arr.push('</table>');
	
	switch(instance)
	{
		case 'Grid':
			genQBQuery('Select (List)','QBtSelLst',arr);
			genQBQuery('Delete','QBtDelete',arr);
		break;
		case 'View':
			genQBQuery('Select (Single)','QBtSel',arr);
		break;
		case 'Edit':
			genQBQuery('Select (Edit)','QBtFetch',arr);
			genQBQuery('Insert','QBtInsert',arr);
			genQBQuery('Update','QBtUpdate',arr);
		break;
	}
	
	document.getElementById(trg).innerHTML = arr.join('');
	genQBSQL(instance);
}

  var QuickBuilderTemplate = ""+
"<table width=\"100%\" border=\"0\" cellpadding=\"1\" cellspacing=\"1\" class=\"HTbl\">"+
"	<tr>"+
"		<td>Table</td>"+
"		<td>Name</td>"+
"		<td><input onchange=\"genQBSQLAll();\" type=\"text\" id=\"QBtxtName\" style=\"width: 150px;\" /></td>"+
"		<td>JQuery Transitions</td>"+
"		<td><input type=\"checkbox\" id=\"QBchkTransition\"/></td>"+
"	</tr>"+
"	<tr>"+
"		<td>Security Level</td>"+
"		<td>Role</td>"+
"		<td>    <input type=\"radio\" name=\"QBradSecure\" id=\"QBradSecureAll\" value=\"all\" checked=\"checked\" /> Everyone <br />" +
"			<input type=\"radio\" name=\"QBradSecure\" id=\"QBradSecureHost\" value=\"host\" /> Host <br />"+
"			<input type=\"radio\" name=\"QBradSecure\" id=\"QBradSecureRole\" value=\"role\" />" +
"			<input type=\"text\" name=\"QBtxtSecureRoleName\" id=\"QBtxtSecureRoleName\" value=\"\" style=\"width: 126px;\" /> </td>" +
"		<td>Include Sorting</td>"+
"		<td><input onclick=\"genQBSQLAll();\" type=\"checkbox\" id=\"QBchkSort\"/></td>"+
"	</tr>"+
"	<tr>"+
"		<td>Query Variable</td>"+
"		<td>Prefix</td>"+
"		<td><input onchange=\"genQBSQLAll();\" type=\"text\" id=\"QBtxtPrefix\" style=\"width: 150px;\" /></td>"+
"		<td>Postfix</td>"+
"		<td><input onchange=\"genQBSQLAll();\" type=\"text\" id=\"QBtxtPostfix\" style=\"width: 150px;\" /></td>"+
"	</tr>"+
"	<tr>"+
"		<td>Form Variable</td>"+
"		<td>Prefix</td>"+
"		<td><input onchange=\"genQBSQLAll();\" type=\"text\" id=\"QBtxtFPrefix\" style=\"width: 150px;\" /></td>"+
"		<td>Postfix</td>"+
"		<td><input onchange=\"genQBSQLAll();\" type=\"text\" id=\"QBtxtFPostfix\" style=\"width: 150px;\" /></td>"+
"	</tr>"+
"</table>"+
"<hr/>"+
"<table width=\"100%\" border=\"0\" cellpadding=\"1\" cellspacing=\"1\" class=\"HTbl\">"+
"<tr>"+
"<td valign=\"top\"><input type=\"checkbox\" id=\"QBchkGrid\" onclick=\"if (this.checked) {genQBTable('Grid',qryResults,'QBgTbl',true,true,true,false,false,false);} else {document.getElementById('QBgTbl').innerHTML='';}\" style=\"width:40px;\">Grid</td>"+
"<td id=\"QBgTbl\">&nbsp;</td></tr>"+
"</table>"+
"<hr style=\"border: 0px dotted gray;\"/>"+
"<table width=\"100%\" border=\"0\" cellpadding=\"1\" cellspacing=\"1\" class=\"HTbl\">"+
"<tr>"+
"<td valign=\"top\"><input type=\"checkbox\" id=\"QBchkView\" onclick=\"if (this.checked) {genQBTable('View',qryResults,'QBvTbl',false,false,false,false,true,true);} else {document.getElementById('QBvTbl').innerHTML='';}\" style=\"width:40px;\">View</td>"+
"<td id=\"QBvTbl\">&nbsp;</td></tr>"+
"</table>"+
"<hr style=\"border: 0px dotted gray;\"/>"+
"<table width=\"100%\" border=\"0\" cellpadding=\"1\" cellspacing=\"1\" class=\"HTbl\">"+
"<tr>"+
"<td valign=\"top\"><input type=\"checkbox\" id=\"QBchkEdit\" onclick=\"if (this.checked) {genQBTable('Edit',qryResults,'QBeTbl',false,false,false,true,true,true);} else {document.getElementById('QBeTbl').innerHTML='';}\" style=\"width:40px;\">Edit</td>"+
"<td id=\"QBeTbl\">&nbsp;</td></tr>"+
"</table>"+
"<hr style=\"border: 0px dotted gray;\"/>"+
"<center><button id=\"btnQBBuild\" onclick=\"genQB();return false;\">Generate</button></center>"+
"<hr style=\"border: 0px dotted gray;\"/>";
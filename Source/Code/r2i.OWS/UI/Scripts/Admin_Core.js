//window.onbeforeunload = function(e) { return "You are about to leave the Administration Interface. Any unsaved work will be lost permanently. Are you sure you want to do this?"; }

var HList = false;
var HMain = false;
var HContent = false;
var HListHeader = false;
var HProperty = false;
var HPropertyDetail = false;
var HFilter = false;
var HSplitter = false;
var HConfigName = false;
var HListContainer = false;
var HListTitle = false;
var splitController = false;

var selectedActionObj = false;
var selectedObj = false;
var sysShowPrintVersion = false;
var adminAbout = {"Actions":[],"Tokens":[],"Queries":[],"Formats":[],"Plugins":[],"Admin":[],"UI":[],"Issues":[]};

String.prototype.trim = function() { return this.replace(/^\s+|\s+$/, ''); };

if( typeof Array.prototype.copy==='undefined' ) {
 Array.prototype.copy = function() {
  var a = [], i = this.length;
  while( i-- ) {
   a[i] = typeof this[i].copy!=='undefined' ? this[i].copy() : this[i];
  }
  return a;
 };
}

Array.prototype.swap = function(index1,index2)
{ var temp = this[index1];
this[index1] = this[index2];
this[index2] = temp;
return;
};

// Array.pop() - Remove and return the last element of an array
if( typeof Array.prototype.pop==='undefined' ) {
 Array.prototype.pop = function() {
  var b = this[this.length-1];
  this.length--;
  return b;
 };
}

// Array.push() - Add an element to the end of an array, return the new length
if( typeof Array.prototype.push==='undefined' ) {
 Array.prototype.push = function() {
  for( var i = 0, b = this.length, a = arguments, l = a.length; i<l; i++ ) {
   this[b+i] = a[i];
  }
  return this.length;
 };
}

// Array.shift() - Remove and return the first element
if( typeof Array.prototype.shift==='undefined' ) {
 Array.prototype.shift = function() {
  for( var i = 0, b = this[0], l = this.length-1; i<l; i++ ) {
   this[i] = this[i+1];
  }
  this.length--;
  return b;
 };
}

// Array.slice() - Copy and return several elements
if(typeof Array.prototype.slice=='undefined')
Array.prototype.slice=function(a,c){
var i=0,b,d=[];
if(!c)
c=this.length;
if(c<0)
c=this.length+c;
if(a<0)
a=this.length-a;
if(c<a){b=a;a=c;c=b}
for(i;i<c-a;i++)
	d[i]=this[a+i];
return d
};


// Array.splice() - Remove or replace several elements and return any deleted elements

if(typeof Array.prototype.splice=='undefined')
Array.prototype.splice=function(a,c){
var i=0,e=arguments,d=this.copy(),f=a;
if(!c)
c=this.length-a;
for(i;i<e.length-2;i++)
this[a+i]=e[i+2];
for(a;a<this.length-c;a++)
this[a+e.length-2]=d[a-c];
this.length-=c-e.length+2;
return d.slice(f,f+c)
};

if(typeof Array.prototype.remove=='undefined')
Array.prototype.remove = function(from, to) {
  if (typeof to == 'undefined' || to == null)
 { from = this.indexOf(from);to=from; }
  var rest = this.slice((to || from) + 1 || this.length);
  this.length = from < 0 ? this.length + from : from;
  return this.push.apply(this, rest);
};

// Array.unshift() - Add an element to the beginning of an array
if( typeof Array.prototype.unshift==='undefined' ) {
 Array.prototype.unshift = function() {
  this.reverse();
  var a = arguments, i = a.length;
  while(i--) { this.push(a[i]); }
  this.reverse();
  return this.length;
 };
}

// -- 4umi additional functions

// Array.forEach( function ) - Apply a function to each element
Array.prototype.forEach = function( f ) {
 var i = this.length, j, l = this.length;
 for( i=0; i<l; i++ ) { if( ( j = this[i] ) ) { f( j ); } }
};

// Array.indexOf( value, begin, strict ) - Return index of the first element that matches value
Array.prototype.indexOf = function( v, b, s ) {
 for( var i = +b || 0, l = this.length; i < l; i++ ) {
  if( this[i]===v || s && this[i]==v ) { return i; }
 }
 return -1;
};

// Array.insert( index, value ) - Insert value at index, without overwriting existing keys
Array.prototype.insert = function(newValue,position) {
if (typeof position != 'undefined' && position!=null && position>-1)
this.splice(position,0,newValue);
else
this.push(newValue);
};


// Array.lastIndexOf( value, begin, strict ) - Return index of the last element that matches value
Array.prototype.lastIndexOf = function( v, b, s ) {
 b = +b || 0;
 var i = this.length; while(i-->b) {
  if( this[i]===v || s && this[i]==v ) { return i; }
 }
 return -1;
};

// Array.random( range ) - Return a random element, optionally up to or from range
Array.prototype.random = function( r ) {
 var i = 0, l = this.length;
 if( !r ) { r = this.length; }
 else if( r > 0 ) { r = r % l; }
 else { i = r; r = l + r % l; }
 return this[ Math.floor( r * Math.random() - i ) ];
};

// Array.shuffle( deep ) - Randomly interchange elements
Array.prototype.shuffle = function( b ) {
 var i = this.length, j, t;
 while( i ) {
  j = Math.floor( ( i-- ) * Math.random() );
  t = b && typeof this[i].shuffle!=='undefined' ? this[i].shuffle() : this[i];
  this[i] = this[j];
  this[j] = t;
 }
 return this;
};

// Array.unique( strict ) - Remove duplicate values
Array.prototype.unique = function( b ) {
 var a = [], i, l = this.length;
 for( i=0; i<l; i++ ) {
  if( a.indexOf( this[i], 0, b ) < 0 ) { a.push( this[i] ); }
 }
 return a;
};

// Array.walk() - Change each value according to a callback function
Array.prototype.walk = function( f ) {
 var a = [], i = this.length;
 while(i--) { a.push( f( this[i] ) ); }
 return a.reverse();
};

function _() {
	var elements = new Array();
	var splitter = "|";
	if (arguments.length==2)
	{
		splitter = arguments[1];
	}
	if (typeof arguments[0] == 'string')
	{	
		elements = arguments[0].split(splitter);
	}
	return elements;
}
function $() {
	var elements = new Array();
	for (var i = 0; i < arguments.length; i++) {
		var element = arguments[i];
		if (typeof element == 'string')
		{	
			if (element.toLowerCase().indexOf('name=')==0)
			{
			    var sval = element.substring(5);
				element = document.getElementsByName(sval);
			}
			else
			{
				if (element.toLowerCase().indexOf('class=')==0)
				    { var sval = element.substring(6);
					    element = getElementsByClass(sval);
					}
				else
					element = document.getElementById(element);
			}
		}
		if (typeof element == 'array')
		{ elements.push(element); }
		else
			elements.push(element);
		if (arguments.length == 1)
			return element;
	}
	return elements;
}

 var sysProperty_actionsInvalidated = true;
 var sysProperty_ListItem = false;
 var sysProperty_ObjectItem = false;
 var sysProperty_ObjectType = false;
 var sysHeader = "headercontent = '<div class=HTitle><center>' + sysProperty_ObjectType.Name + ' ' + (typeof sysProperty_ObjectItem.Index!='undefined'?'['+sysProperty_ObjectItem.Index+']':'') + '</center></div>'";
 var sysFooter = "footercontent = '<div class=HCommand><center><a href=\"#\" onclick=\"SaveProperty(true);\">Save</a>&nbsp;|&nbsp;<a href=\"#\" onclick=\"SaveProperty(false);\">Cancel</a></center></div>'"; 
 var sysLoadConfigFooter = "footercontent = '<div class=HCommand><center><a href=\"#\" onclick=\"SaveProperty(true);\">Load</a>&nbsp;|&nbsp;<a href=\"#\" onclick=\"SaveProperty(false);\">Cancel</a></center></div>'"; 
 
 function owsAdmin() {
	this.properties = new Array();
	this.keys = new Array();
	this.removeProperty =  function(key) { this.keys.remove(key); properties[key]=null; }
	this.addProperty = function(key,value) { this.keys.push(key); this.properties[key]=value; }
 }
var sysAdmin = new owsAdmin();
sysAdmin.addProperty('About',	{
	   "Name" : "<center>About</center>",
	   "Code" : "About",
	   "Description" : "Provides the runtime information universal to the instance.",
	   "onLoad" : "viewAbout",
	   "onSave" : "",
	   "Template" : "<div id=\"divAbout\" class=\"PropertyList\"></div>"
	});

function disableSelection()
{
document.onselectstart=new Function("return false");document.onmousedown = function () { return false; };
}
function enableSelection()
{
document.onselectstart=null;document.onmousedown = null;
}
window.browser = (document.all==undefined?'FF':'IE');
window.browser = (window.opera!=undefined?'OP':window.browser);
//window.browser = (navigator.userAgent.toLowerCase().indexOf('chrome') > -1?'CM':window.browser);

function getParent(obj)
{
if (obj!=undefined&&obj!=null)
{
	if (typeof obj.parentNode != 'undefined')
	{
		return obj.parentNode;
	}
	return obj.parentElement;
}
else
	return null;
}
function getX(obj,until)
  {
   var x = 0;
   var xo = obj;
   x = xo.offsetLeft;
   while (getParent(xo)!=null) {
			x = x + xo.offsetLeft;
		 	xo = getParent(xo);
	}
   return x;
}

function getY(obj,until)  {
   var x = 0;
    var xo = obj;
    x = xo.offsetTop;
	if (obj==until)
		return x;
   while (getParent(xo)!=null) {
			x = x + xo.offsetTop;
		 	xo = getParent(xo);
	if (xo==until || getParent(xo)==until)
		return x;			
		}
   return x;
 }
 
 function UnloadProperty()
 {
	//CHECK SAVE
	if (window.frames.length > 0)
	{
		var i = 0;
		/*
		while(window.frames.length>0)
		{
		
			var obj = $(window.frames[0].name);
			var pobj = getParent(obj);
			alert(i);
			pobj.removeChild(obj);
			i++;
			if (i>10)
			{
			return true;
			}
		}
				*/
	}
	//alert('remove');
	HPropertyDetail.innerHTML = '<br><Br><br><Br><br><Br>';
	hideBlock(HProperty);
	showBlock(HList);
	return true;
 }
 function LoadProperty(obj)
 {
 	//CHECK SAVE
	UnloadProperty();
	if (_LoadProperty(obj)==true)
	{	showH(HProperty);
		document.location = '#Property';
	}
	return true;
 }
 function SaveProperty(canContinue)
 {
	if (canContinue && sysProperty_ObjectItem && sysProperty_ListItem)
	{
	var objid = (sysProperty_ListItem.id==undefined?sysProperty_ListItem:sysProperty_ListItem.id);	
	 switch (objid.substring(0,1).toLowerCase())
	 {
		case 'a': //ACTION
			objId = sysProperty_ListItem.id.substring(1,sysProperty_ListItem.id.length);
			canContinue = sysActionHandleCommand(sysProperty_ObjectItem,null,'SAVE');
			if (canContinue)
			{
				owsSelect.expand(sysProperty_ObjectItem.GUID);
			}
			break;
		case 'g': //GENERAL
			canContinue = sysConfigHandleCommand('SAVE',sysProperty_ObjectType);
			if (canContinue)
			{
			    document.title = configuration.Name + ' - OWS Admin';
				HConfigName.innerHTML = configuration.Name + ' - ';
				showBlock(HMain);
			}
			break;
		case 'o': //OPEN
			sysConfigHandleCommand('SAVE',sysProperty_ObjectType);
			//needs to be set to false so the rest is not loaded.
			canContinue = false;
			/*
			if (canContinue)
			{
				HConfigName.innerHTML = configuration.Name + ' - ';
				showBlock(HMain);
			}
			*/
			break;
		case 'c': //CONFIGURATION
			break;
		case 'r': //REPOSITORY
			break;
		case 'e': //EVENT
			break;
		case 'x': //EXPORT
			canContinue = sysConfigHandleCommand('SAVETEXT',sysProperty_ObjectType);
			if (canContinue)
			{
				loadConfiguration();
				showBlock(HMain);
			}		
			break;
		case 'i': //IMPORT
			canContinue = sysConfigHandleCommand('SAVETEXT',sysProperty_ObjectType);
			if (canContinue)
			{
				loadConfiguration();
				showBlock(HMain);
			}		
			break;
		case '*': //ACTION
			canContinue = sysCommunityHandleCommand('SAVE',sysProperty_ObjectType,sysProperty_ObjectItem);
			break;			
	 }
	}
	else
	{
		canContinue = true;
	}
	if (canContinue)
	{
		sysProperty_ListItem = false;
		sysProperty_ObjectItem = false;
		displayProperty('',null);
		if (typeof configuration != 'undefined' && configuration!=null) {
				showBlock(HMain);
		}
	}
}

function viewAbout()
{
	var out = $('divAbout');
	var outS = new Array();
	outS.push('<table class="OWSAdminTable" cellspacing="0" cellpadding="0" width="100%">');
	outS.push('<tr><th class="LeftCell" width="80">Type</th><th class="Primary" width="150">Name</th><th class="Primary">Library</th></tr>');
	for (var i = 0;i<adminAbout.Versions.length;i++)
	{
	    outS.push('<tr><td class="LeftCell" width="80">Plugins</td><td class="Primary" width="150">'+adminAbout.Versions[i].Name+'</td><td class="Primary" width="150">'+adminAbout.Versions[i].Version+((adminAbout.Versions[i].Name.toLowerCase()=='r2i.ows.engine')?'&nbsp;<a href="http://www.openwebstudio.com" target="_blank"><img src="http://www.openwebstudio.com/version.aspx?v='+adminAbout.Versions[i].Version+'" border="0" /></a>':'')+'</td></tr>');
	}
    for (var i = 0;i<adminAbout.Issues.length;i++)
		outS.push('<tr><td class="LeftCell" width="80">Issues</td><td class="Primary" width="150">'+adminAbout.Issues[i].Name+'</td><td class="Primary" width="150">'+adminAbout.Issues[i].Value+'</td></tr>');
	outS.push('<tr><td colspan="4">'+'<hr />'+'</td></tr>');
	for (var i = 0;i<adminAbout.Actions.length;i++)
		outS.push('<tr><td class="LeftCell" width="80">Action</td><td class="Primary" width="150">'+adminAbout.Actions[i].Name+'</td><td class="Primary" width="150">'+adminAbout.Actions[i].Version+'</td></tr>');
	for (var i = 0;i<adminAbout.Tokens.length;i++)
		outS.push('<tr><td class="LeftCell" width="80">Tokens</td><td class="Primary" width="150">'+adminAbout.Tokens[i].Name+'</td><td class="Primary" width="150">'+adminAbout.Tokens[i].Version+'</td></tr>');
	for (var i = 0;i<adminAbout.Formats.length;i++)
		outS.push('<tr><td class="LeftCell" width="80">Formats</td><td class="Primary" width="150">'+adminAbout.Formats[i].Name+'</td><td class="Primary" width="150">'+adminAbout.Formats[i].Version+'</td></tr>');
	for (var i = 0;i<adminAbout.Queries.length;i++)
		outS.push('<tr><td class="LeftCell" width="80">Queries</td><td class="Primary" width="150">'+adminAbout.Queries[i].Name+'</td><td class="Primary" width="150">'+adminAbout.Queries[i].Version+'</td></tr>');
	outS.push('<tr><td colspan="4">'+'<hr />'+'</td></tr>');
	for (var i = 0;i<adminAbout.Admin.length;i++)
		outS.push('<tr><td class="LeftCell" width="80">Admin</td><td class="Primary" width="150">'+adminAbout.Admin[i].Name+'</td><td class="Primary" width="150">'+adminAbout.Admin[i].Version+'</td></tr>');
	for (var i = 0;i<adminAbout.UI.length;i++)
		outS.push('<tr><td class="LeftCell" width="80">UI</td><td class="Primary" width="150">'+adminAbout.UI[i].Name+'</td><td class="Primary" width="150">'+adminAbout.UI[i].Version+'</td></tr>');
	outS.push('</table>');
	out.innerHTML = outS.join('');
}
 function _LoadProperty(obj)
 {
	var returnvalue = true;
	var handled  = false;
	sysProperty_ListItem = obj;
	sysProperty_ObjectItem = false;
	var objid = (obj.id==undefined?obj:obj.id);
	
	var phandle = sysAdmin.properties[objid];
	if (typeof phandle != 'undefined' && phandle != null)
	{
		sysProperty_actionsInvalidated=true;
		sysProperty_ObjectItem = false;
		if (typeof phandle.Display != 'undefined' && phandle.Display.toUpperCase()=='SHEET')
		{
			if (typeof phandle.Header!='undefined'&&typeof phandle.Footer!='undefined')
				displayProperty(phandle.Template,phandle.Header,phandle.Footer); 
			else
				displayProperty(phandle.Template,eval(sysHeader),eval(sysFooter)); 
		}
		else
		{	
			displayList(phandle.Template,'',phandle.Name); 
			returnvalue=false;
		}
		eval(phandle.onLoad+'();');	
	}
	else
	{
 	 switch (objid.substring(0,1).toLowerCase())
	 {
		case 'a': //ACTION
			handled=true;
			objId = sysProperty_ListItem.id.substring(1,sysProperty_ListItem.id.length);
			sysProperty_ObjectItem = owsSelect.getItem(objId,configuration.messageItems);
			sysProperty_ObjectType = sysActionGetObject(sysProperty_ObjectItem.ActionType);
			if (sysProperty_ObjectType.Template!=undefined && sysProperty_ObjectType.Template.length > 0)
				displayProperty(sysProperty_ObjectType.Template,eval(sysHeader),eval(sysFooter));
			else
				returnvalue=false;
			sysActionHandleCommand(sysProperty_ObjectItem,null,'LOAD');
			break;
		case 'g': //GENERAL PROPERTIES
			handled=true;
			sysProperty_ObjectItem = configuration;
			sysProperty_ObjectItem.Index = configuration.Name;
			sysProperty_ObjectType = sysGeneralTemplate();
			hideBlock(HMain);
			displayProperty(sysProperty_ObjectType.Template,eval(sysHeader),eval(sysFooter));
			sysConfigHandleCommand('LOAD',sysProperty_ObjectType);	
			break;
		case 'd': //DISPLAY
			handled=true;
			showBlock(HMain);
			if (sysProperty_actionsInvalidated)
				owsSelect.render();
			returnvalue=false;
			break;
		case 'o': //OPEN
			handled=true;
			sysProperty_actionsInvalidated=true;
			sysProperty_ObjectItem = false;
			displayList(sysOpenTemplate.Template,'',sysOpenTemplate.Name);
			eval(sysOpenTemplate.onLoad+'();');	
			returnvalue=false;			
			break;
		case 'c': //CONFIGURATION
			handled=true;
			break;
		case 'e': //EVENT
			handled=true;
			break;
		case 'i': //EXPORT/IMPORT CONFIG
			handled=true;
			sysProperty_ObjectItem = {"id":"import"};
			sysProperty_ObjectType = sysImportTemplate;
			hideBlock(HMain);
			displayProperty(sysProperty_ObjectType.Template,eval(sysHeader),eval(sysFooter));
			sysConfigHandleCommand('IMPORT',sysProperty_ObjectType);
			break;			
		case 'x': //EXPORT/IMPORT CONFIG
			handled=true;
			sysProperty_ObjectItem = {"id":"xport"};
			sysProperty_ObjectType = sysExportTemplate;
			hideBlock(HMain);
			displayProperty(sysProperty_ObjectType.Template,eval(sysHeader),eval(sysFooter));
			sysConfigHandleCommand('EXPORT',sysProperty_ObjectType);
			break;			
		}
	}
	 return returnvalue;
 }
 
  function GetPropertyItem(obj)
 {
	var propObj = false;

	var objid = (obj.id==undefined?obj:obj.id);
 	 switch (objid.substring(0,1).toLowerCase())
	 {
		case 'a': //ACTION
			objId = obj.id.substring(1,obj.id.length);
			propObj = owsSelect.getItem(objId,configuration.messageItems);
			//sysProperty_ObjectType = sysActionGetObject(sysProperty_ObjectItem.ActionType);
			break;
		case 'c': //CONFIGURATION
			break;
		case 'r': //REPOSITORY
			break;
		case 'e': //EVENT
			break;
	 }
	 return propObj;
 }
  function displayProperty(content,headercontent,footercontent)
 {
 if (content!=undefined && content.length > 0)
 {
	var layout = new Array();
	if (headercontent!=undefined && headercontent.length > 0)
		layout.push(headercontent);
	layout.push(content);
	if (headercontent!=undefined && headercontent.length > 0)
		layout.push(footercontent);
	HPropertyDetail.innerHTML = layout.join('');
 }
 else
	UnloadProperty();
 }
 function displayList(content,headercontent,titlecontent)
 {
	if (content!=undefined && content.length > 0)
	{
		showBlock(HMain);
		if (headercontent!=undefined)
			HListHeader.innerHTML = headercontent;
		if (titlecontent!=undefined)
			HListTitle.innerHTML = titlecontent;
		HContent.innerHTML = content;
	}
	else
	{
		hideBlock(HMain);
		HContent.innerHTML = '';
		HListHeader.innerHTML = '';
	}
 }
 function displayFilter(content)
 {
 }

 function RemoveAction(objId)
 {
	if (onActionDelete()) {
	var parentGUID = null;
 	ObjectItem = owsSelect.getItem(objId,configuration.messageItems);
	//GET THE PARENT OF THE ACTION
	if (ObjectItem!=undefined)
		{ 
			if (ObjectItem.parentAction!=undefined)
				{
					var pAction = ObjectItem.parentAction;
					parentGUID = pAction.GUID;
					if (ObjectItem.parentIndex>=0)
					{
						pAction.ChildActions.splice(ObjectItem.parentIndex,1);
					}
					else
					{
						pAction.ChildActions = new Array();
					}
				}
			else {
					if (ObjectItem.parentIndex>=0)
					{
						configuration.messageItems.splice(ObjectItem.parentIndex,1);
					}
					else
					{
						configuration.messageItems = new Array();
					}				
				}
		}
	//FIND THE INDEX WITHIN THE PARENT
	//DELETE THE ITEM
	//REINDEX
	//REFRESH
		selectedActionObj=null;
		owsSelect.sharedIndex = 1;
		owsSelect.reindex(null);
		owsSelect.expand(parentGUID);	
		}
 }
 var clipboard_Action = null;
 function ActionMove(command,shiftCheck)
 {
	if (typeof shiftCheck == 'undefined')
		shiftCheck = false;
	var nfirst = null;
	var nsecond = null;
	var nSourceParent = null;
	var nTargetParent = null;
	var nSourceIndex = null;
	var nTargetIndex = null;
	var canContinue = true;
	var sourceFirst = true;
	var nAction = selectedActionObj;
	if (nAction!=undefined && nAction!=null && nAction.ActionType != undefined)
		{
			if (nAction.parentAction!=undefined)
			{
				nSourceParent = nAction.parentAction;
			}
			if (nSourceParent!=undefined&&nSourceParent!=null)
			{
				nSourceIndex = nSourceParent.ChildActions.indexOf(nAction);
			}
			else
			{
				nSourceIndex = configuration.messageItems.indexOf(nAction);
			}
			switch (command)
			{
				case 'PASTE':
					if (clipboard_Action!=null)
					{
						nSourceIndex=-1;
						nTargetIndex = 0;
						nTargetParent = nAction;
						nAction = CopyAction(clipboard_Action);
					}
				break;
				case 'UP':
					owsSelect.move(selectedActionObj,null,-1,shiftCheck);
					return;

					break;
				case 'DOWN':
					owsSelect.move(selectedActionObj,null,1,shiftCheck);
					return;
			
					break;
				case 'LEFT':
						if (nSourceParent!=undefined)
						{
							if (nSourceParent.parentAction!=undefined)
							{
								nTargetIndex = nSourceParent.parentAction.ChildActions.indexOf(nSourceParent);
								nTargetParent = nSourceParent.parentAction;
							}
							else
							{
								nTargetIndex = configuration.messageItems.indexOf(nSourceParent);
								nTargetParent = undefined;
							}
							
						}
						else
						{
							canContinue = false;
						}
						sourceFirst=false;
					break;
				case 'RIGHT':
					if (nSourceIndex>0 && nSourceParent!=undefined)
					{
						nTargetIndex = nSourceIndex - 1;
						nTargetParent = nSourceParent.ChildActions[nTargetIndex]; nTargetIndex = 0;
					}
					else if (nSourceIndex > 0 && nSourceParent==undefined)
					{
						nTargetIndex = nSourceIndex - 1;
						nTargetParent = configuration.messageItems[nTargetIndex]; nTargetIndex = 0; 
					}
					else
					{
						canContinue = false;
					}
					sourceFirst=true;
						break;
				case '':
					canContinue=false;
					break;
			}
			if (canContinue) {
				if (nTargetParent!=undefined && nTargetParent.ChildActions!=undefined && nTargetParent.ChildActions.length==undefined)
				{
					var messageItems = new Array();
					messageItems.push(nTargetParent.ChildActions);
					nTargetParent.ChildActions = messageItems;
				}
				
				if (nSourceIndex>-1)
				{
					if (nSourceParent!=undefined)
						nSourceParent.ChildActions.splice(nSourceIndex,1);
					else
						configuration.messageItems.splice(nSourceIndex,1);
				}
					
				if (nTargetParent!=undefined)
					{ nTargetParent.ChildActions.insert(nAction,nTargetIndex); nAction.parentAction=nTargetParent;}
				else
					{ configuration.messageItems.insert(nAction,nTargetIndex); nAction.parentAction=null;}
				
				owsSelect.reindex(null);
				
				if (sourceFirst) 
				{
					if (nSourceParent!=undefined)
						{owsSelect.expand(nSourceParent.GUID);}
					else
						{owsSelect.expand(null);}
					if (nTargetParent!=undefined)
						{owsSelect.expand(nTargetParent.GUID);}
					else if (nSourceParent!=undefined)
						{owsSelect.expand(null);}
				}
				else
				{
					if (nTargetParent!=undefined)
						owsSelect.expand(nTargetParent.GUID);
					else
						owsSelect.expand(null);
					if (nSourceParent!=undefined)
						owsSelect.expand(nSourceParent.GUID);
					else if (nTargetParent!=undefined)
						owsSelect.expand(null);						
				}
			}				
		}	
 }
 function AddAction(ActionType,ActionInformation)
 {
	var nAction = new Action(ActionType,ActionInformation);
	if (selectedActionObj!=undefined && selectedActionObj!=null && selectedActionObj.ActionType != undefined)
	{
		if (selectedActionObj.ChildActions!=undefined && selectedActionObj.ChildActions.length==undefined)
		{
			var messageItems = new Array();
			messageItems.push(selectedActionObj.ChildActions);
			selectedActionObj.ChildActions = messageItems;
		}
		selectedActionObj.ChildActions.push(nAction);
		owsSelect.sharedIndex = 1;
		owsSelect.reindex(null);
		owsSelect.expand(selectedActionObj.GUID);
	}
	else
	{
		if (configuration.messageItems==undefined)
			configuration.messageItems = new Array();
		if (configuration.messageItems.length==undefined)
		{
			var messageItems = new Array();
			messageItems.push(configuration.messageItems);
			configuration.messageItems = messageItems;
		}
		configuration.messageItems.push(nAction);
		owsSelect.sharedIndex = 1;
		owsSelect.reindex(null);	
		owsSelect.render();
	}
		//showBlock($('testConfiguration'));
		//$('testConfiguration').value = configuration.toJSONString();
		//loadActionTree(sysProperty_ObjectItem);
		//buildList();
 }
 function Action(ActionType,ActionInformation,Index,Level,ChildActions)
 {
	if (ActionType!=undefined&&ActionType.ActionType!=undefined)
	{
		ActionInformation = ActionType.Parameters;
		Index = ActionType.Index;
		Level = ActionType.Level;
		ChildActions = ActionType.ChildActions;
		ActionType = ActionType.ActionType;
	}
 
	this.ActionType = ActionType;
	this.Parameters = ActionInformation;
	if (Index==undefined)
		this.Index = 0;
	else
		this.Index = Index;
	if (Level==undefined)
			this.Level = 0;
		else
			this.Level = Level;		
	if (ChildActions==undefined)
			this.ChildActions = new Array();
		else
			this.ChildActions = ChildActions;	
 }
 
 function configurationToString() {
	configuration.Version = "20";
					if (configuration.ConfigurationID==undefined) {
						configuration.ConfigurationID = Bi4ce.GenerateGuid.newGuid('N').toString();
                                                configurationId = configuration.ConfigurationID;
					}
	JSONBINDActionItems(configuration.messageItems);
	return ows.JSON.stringify(configuration);
 }
 function JSONBINDActionItems(obj) {
	if (obj!=undefined)
	{
		if (obj.length!=undefined)
		{
			var xi=0;
			for (xi=0;xi<obj.length;xi++)
				JSONBINDActionItems(obj[xi]);
		}
		else
		{
			obj.toJSON = function (w) { x = new Object(); x.Index=this.Index; x.Level=this.Level; x.Parameters=this.Parameters; x.ActionType=this.ActionType; x.ChildActions=this.ChildActions; return x; };
			JSONBINDActionItems(obj.ChildActions);
		}
	}
}
function ActionClearGUID(obj)
{
	if (obj!=undefined)
	{
		if (obj.length!=undefined)
		{
			var xi=0;
			for (xi=0;xi<obj.length;xi++)
				ActionClearGUID(obj[xi]);
		}
		else
		{
			obj.GUID = undefined;
			ActionClearGUID(obj.ChildActions);
		}
	}	
}
function CopyAction(obj)
{
	JSONBINDActionItems(obj);
	var strValue = ows.JSON.stringify(obj);
	var newValue = null;
	eval('newValue = ' + strValue + ';');
	ActionClearGUID(newValue);
	return newValue;
}
 //WINDOW EVENTS AND HANDLERS
 function Initialize()
 {
	HMain = $('HMain');
	HList = $('HList');
	HContent = $('HContent');
	HListHeader = $('HListHeader');
	HProperty = $('HProperty');
	HPropertyDetail = $('HPropertyDetail');
	HFilter = $('HFilter');
	HSplitter = $('HSplitter');
	HListContainer = $('HListContainer');
	HConfigName = $('HConfigName');
	HListTitle = $('HListTitle');
	
	showSplitter();
	hideBlock(HProperty);


	hideBlock(HMain);	
	HPropertyDetail.innerHTML = '<br><Br><br><Br><br><Br>';
	
	var configid=getQueryVariable('configurationId')
	if (configid!='') {
		onOpenConfig(configid);
	}
 }
function getQueryVariable(name) {
		var QRY='';  var cleanURL; var urlParts; var recordElement = false; var isKey = true; var pathQuery = "";var isSkipped=false;
		
		if (document.location.search.length > 0)
			{
			  //trim the question mark
			  QRY = document.location.search.substr(1);
			}
		
		var QRYpairs = QRY.split('&');
		QRY = new Array();
		for (var i=0;i<QRYpairs.length;i++)
		{
			var QRYKey = QRYpairs[i].split('=');
			if (QRYKey[0]==name)
				return QRYKey[1];
		}
		return '';
}
function handleResize(eventObj)
 {
	eventObj = eventObj ? eventObj : window.event;
	if (selectedObj)
	{
		owsSelect.show(selectedObj.id);
	}
	//IE:Force Refresh
	if ($('HTreeRoot')!=null)
	    $('HTreeRoot').className=$('HTreeRoot').className;
 }

window.onresize = handleResize; 

 
//SPLITTER FUNCTIONS 
 function showSplitter()
{
	if (!splitController)
	{
		HList.style.height = '400px';
		var splitController = new dragObject(HSplitter,null,null,null,null,splitterMove,splitterEnd,null);
		splitController.initialize();
	}
}

//GENERAL DISPLAY FUNCTIONS
function showBlock(obj)
{
	obj.style.display = 'block';
	if (window.browser!='IE') obj.style.visibility = 'visible';
}
function hideBlock(obj)
{
	obj.style.display = 'none';
	if (window.browser!='IE') obj.style.visibility = 'invisible';
}
function showH(obj)
{
	switch (obj)
	{
		case HList:
			break;
		case HProperty:
			showBlock(HProperty);
			showSplitter();
			break;
	}
}
function exportConfiguration()
{
LoadProperty('XPort');
}
function importConfiguration()
{
LoadProperty('Import');
}
function loadConfiguration(src)
{
	if (src!=undefined)
	{
		if (src.length == 0)
		{
			loadConfiguration(document.getElementById('newConfiguration').value);
			return;
		}
		else
		{
                        if (src.Name != undefined && src.Name != null) {
                            // src is json object already
                            configuration = src;
                        } else if (src.length>2)
				eval('configuration = ' + src);
			else
			{
				alert('You do not have access to change this configuration. Please authenticate and try again.');
				return false;
			}
		}
	}
	RibbonMain.state='load';
	RibbonMain.Render();
	document.title = configuration.Name + ' - OWS Admin';
	HConfigName.innerHTML = configuration.Name + ' - ';
	upgradeConfiguration();
	if (typeof owsSelect != 'undefined')
		owsSelect.clear();
	//nameOfSource,nameOfRootElements,nameOfChildElements,rootCId,rootCss
	owsSelect = new OWSCodeTree('owsSelect','configuration','messageItems','ChildActions','HTreeRoot','HTree','cItem','cItemSel','cItemActive','cItemActive','KeyCatch');
	//buildTree();
	owsSelect.render();
}
function upgradeConfiguration()
{
	if (configuration.Version < 20)
	{
		//var lRegion = null;
		//var uRegion = null;
		//var vRegion = null;
		
		//REGION: Query Variables and Default Formatting
			//REGION: Query Variables
			//REGION: List Item Formats (Templates)
		//REGION: Page Rendering and Action Execution
		var qvRegion = null;
		var dfRegion = null;
		var aRegion = null;
		var actions = null;
		
		/*
		if (configuration.disableOpenScript==undefined)
			configuration.disableOpenScript = true;
		*/
		
		//SETUP THE DEFAULT STRUCTURE (LISTX MODE)
		if (configuration.messageItems==null)
			{	configuration.messageItems=new Array();
				actions = new Array();}
		else
			{ actions = configuration.messageItems;
			  configuration.messageItems=new Array(); }

		qvRegion = upgradeRegion(qvRegion,'Query Variable Definitions');
		aRegion = upgradeRegion(aRegion,'Actions and Runtime');
		dfRegion = upgradeRegion(dfRegion,'List Item Formatting (Templates)');
		
		if (configuration.queryItems!=undefined&&configuration.queryItems.length > 0)
		{
			var l = 0;
			for (l=0;l<configuration.queryItems.length;l++)
			{
				var obj = new Action("Template-Variable",configuration.queryItems[l]);
				obj.Parameters.Value = configuration.query;
				qvRegion.ChildActions.push(obj);
			}
			configuration.queryItems=null;
		}
		if (configuration.query!=undefined&&configuration.query!=null&&configuration.query.length>0)
		{
			var obj = new Action("Template",{"Type":"Query-Query","GroupStatement":"","GroupIndex":"","Value":"New Template","Connection":"","Filter":""});
			obj.Parameters.Value = configuration.query;
			obj.Parameters.Connection = configuration.customConnection;
			obj.Parameters.Filter = configuration.filter;
			configuration.query = null;
			configuration.customConnection = null;
			configuration.filter = null;
			dfRegion.ChildActions.push(obj);
		}
		var liParent = dfRegion;
		if (configuration.listItems!=undefined&&configuration.listItems.length > 0)
		{
			var l = 0;
			for (l=0;l<configuration.listItems.length;l++)
			{
				var objH = new Action("Template",{"Type":"Group-Header","GroupStatement":"","GroupIndex":"","Value":"New Template"});
					objH.Parameters.Value = configuration.listItems[l].ListHeader;
					objH.Parameters.GroupStatement = configuration.listItems[l].GroupStatement;
					objH.Parameters.GroupIndex = configuration.listItems[l].Index;
					liParent.ChildActions.push(objH);
					
				var objF = new Action("Template",{"Type":"Group-Footer","GroupStatement":"","GroupIndex":"","Value":"New Template"});
					objF.Parameters.Value = configuration.listItems[l].ListFooter;
					objF.Parameters.GroupStatement = configuration.listItems[l].GroupStatement;
					objF.Parameters.GroupIndex = configuration.listItems[l].Index;
					liParent.ChildActions.push(objF);	
				
				liParent = objH;
			}
			configuration.listItems=null;
		}
		if (configuration.listItem!=undefined&&configuration.listItem!=null&&configuration.listItem.length>0)
		{
			var obj = new Action("Template",{"Type":"Detail-Detail","GroupStatement":"","GroupIndex":"","Value":"New Template"});
			obj.Parameters.Value = configuration.listItem;
			configuration.listItem = null;
			liParent.ChildActions.push(obj);
		}
		if (configuration.listAItem!=undefined&&configuration.listAItem!=null&&configuration.listAItem.length>0)
		{
			var obj = new Action("Template",{"Type":"Detail-Alternate","GroupStatement":"","GroupIndex":"","Value":"New Template"});
			obj.Parameters.Value = configuration.listAItem;
			configuration.listAItem = null;
			liParent.ChildActions.push(obj);
		}	
		if (configuration.defaultItem!=undefined&&configuration.defaultItem!=null&&configuration.defaultItem.length>0)
		{
			var obj = new Action("Template",{"Type":"Detail-NoResults","GroupStatement":"","GroupIndex":"","Value":"New Template"});
			obj.Parameters.Value = configuration.defaultItem;
			configuration.defaultItem = null;
			dfRegion.ChildActions.push(obj);
		}		
		if (configuration.noqueryItem!=undefined&&configuration.noqueryItem!=null&&configuration.noqueryItem.length>0)
		{
			var obj = new Action("Template",{"Type":"Detail-NoQuery","GroupStatement":"","GroupIndex":"","Value":"New Template"});
			obj.Parameters.Value = configuration.noqueryItem;
			configuration.noqueryItem = null;
			dfRegion.ChildActions.push(obj);
		}		
		if (configuration.searchItems!=undefined&&configuration.searchItems!=null&&configuration.searchItems.length>0)
		{
			var obj = new Action("Action-Filter",{"Options":[]});
			for (var i = 0; i < configuration.searchItems.length;i++)
			{
				var objI = {"Option":"","Field":""};
				objI.Option = configuration.searchItems[i].SearchOption;
				objI.Field = configuration.searchItems[i].SearchField;
				obj.Parameters.Options.push(objI);
			}
			configuration.searchItems = null;
			dfRegion.ChildActions.push(obj);
		}
		if (actions!=undefined&&actions.length > 0)
		{
			var act = 0;
			for (act=0;act<actions.length;act++)
			{
				aRegion.ChildActions.push(actions[act]);
			}
		}
		configuration.Version=20;
	}
	//"customConnection":"",
}
function upgradeRegion(obj,name,p)
{
	if (obj==null)
	{
		eval('obj = new Action("Action-Region",{"Name":"' + name + '","RenderType":"0"});');
		if (p==undefined)
		{ if (configuration.messageItems==null)
			configuration.messageItems=new Array();
		configuration.messageItems.push(obj);
		}
		else
		{
		 if (p.ChildActions==null)
			p.ChildActions=new Array();
		p.ChildActions.push(obj);
		}
		obj.ChildActions = new Array();
	}
	return obj;
}
function onClick_Actions_Template() {
	AddAction("Template",{"Type":"Query-Query","GroupStatement":"","GroupIndex":"","Value":"New Template"});
}
function onClick_Actions_Variable() {
	AddAction("Template-Variable",{"VariableType":"<Action>","QuerySource":"Source Value","QueryTarget":"New Query Variable","QueryTargetLeft":"","QueryTargetRight":"","QueryTargetEmpty":"","EscapeListX":"0","Protected":"true","EscapeHTML":"true"});
}
function verifyConfiguration()
{
	if (configuration.recordsPerPage==null||isNaN(configuration.recordsPerPage))
		configuration.recordsPerPage=0;
}

function saveConfiguration(publish)
{
    verifyConfiguration();
    var configurationSrc = configurationToString();
    SetJsonByPostRequest(configuration.ConfigurationID, publish, configurationSrc);
}

function unloadConfiguration()
{
RibbonMain.state='';
RibbonMain.Render();
}

function sysActionGetObject(Name)
{
	for (var i=0;i<sysAction_Types.length;i++)
	{
		if (sysAction_Types[i].Code==Name)
			return sysAction_Types[i];
	}
}
function sysActionSummary(header,content,overrideClass,showAll)
{
	if (overrideClass==undefined || overrideClass.length==0)
	{
		return '<b>' + header + '</b>' + '<i>' + (showAll?'<pre>':'') + sysShortenSummary(content,(showAll?-1:200)) + (showAll?'</pre>':'') + '</i>';
	}
	else
	{
		return '<b class="' + overrideClass + '">' + header + '</b>' + '<i class="' + overrideClass + '">' + (showAll?'<pre>':'') + sysShortenSummary(content,(showAll?-1:500)) + (showAll?'</pre>':'') + '</i>';
	}
}
function encodehtml(v)
{
	v = v.replace(/</g,"&lt;");
	v = v.replace(/>/g,"&gt;");
	return v;
}
function sysConfigHandleCommand(command,aType)
{
	switch(command.toUpperCase())
	{
		case "LOAD":
			return eval(aType.onLoad + '(aType,null);');
			break;
		case "SAVE":
			return eval(aType.onSave + '(aType,null);');
			break;
		case "EXPORT":
			return eval(aType.onLoad + '(aType,null);');
			break;
		case "IMPORT":
			return eval(aType.onLoad + '(aType,null);');
			break;			
		case "SAVETEXT":
			return eval(aType.onSave + '(aType,null);');
			break;		
	}
}
function sysActionHandleCommand(action,typename,command)
{
	var aType = false;
	if (action!=null)
	{
		aType = sysActionGetObject(action.ActionType);	        
	}
	else
	{
		aType = sysActionGetObject(typename);
	}
	if (!aType)
	{
		var p = {"Value":""};
		if (typeof action.ActionType != 'undefined' && action.ActionType != null)
		{
			p.Value = 'Undefined Action - ' + action.ActionType;
		}
		else
		{
			p.Value = 'Undefined Action';
		}
		action.ActionType = 'Action-Comment';
		action.Parameters = p;
		aType = sysActionGetObject(action.ActionType);
	}
	if (aType)
	{
		switch(command.toUpperCase())
		{
			case "LOAD":
				return eval(aType.onLoad + '(aType,action);');
				break;
			case "SAVE":
				return eval(aType.onSave + '(aType,action);');
				break;
			case "DELETE":
				//return eval(aType.onDelete + '(aType,action);');
				//UNSELECT
				selectedActionObj = false;
				//action
				//if (UnloadProperty(selectedEventObj))
				if (UnloadProperty(action))
					setSelector_unselect(action);
				selectedEventObj=false;
				selectedEventState=0;
				
				break;	
			case "PRINT":
				//debugger;
				return eval(aType.onPrint + '(aType,action);');
		}
	}
	
}
function sysSetRadio(objTarget,currentValue)
{
	if (typeof currentValue=='undefined'||currentValue==null)
		currentValue='';
	for	(var i=0;i<objTarget.length;i++)
	{
		if (objTarget[i].value ==	currentValue)
			objTarget[i].checked	= true;
		else
			objTarget[i].checked	= false;
	}
}
function sysSetSelect(objDrop,currentValue)
{
	if (typeof currentValue=='undefined'||currentValue==null)
		currentValue='';
	for	(var i=0;i<objDrop.options.length;i++)
	{
		if (objDrop.options[i].value ==	currentValue)
			objDrop.options[i].selected	= true;
		else
			objDrop.options[i].selected	= false;
	}
}
function sysSetCheck(objTarget,currentValue,byvalue)
{
    if (typeof byvalue == 'undefined' || byvalue == null)
    {
	    if ((typeof currentValue) == 'boolean')
		    objTarget.checked = currentValue;
	    else {
	        if (typeof currentValue!='undefined'&&currentValue!=null&&currentValue.toUpperCase()=='TRUE')
		        objTarget.checked = true;
	        else
		        objTarget.checked = false;
	    }
	}
	else
	{
	    for	(var i=0;i<objTarget.length;i++)
	    {
		    if (objTarget[i].value==byvalue)
			{
                if ((typeof currentValue) == 'boolean')
		            objTarget[i].checked = currentValue;
	            else {
	                if (typeof currentValue!='undefined'&&currentValue!=null&&currentValue.toUpperCase()=='TRUE')
		                objTarget[i].checked = true;
	                else
		                objTarget[i].checked = false;
	            }			
			}
	    }
	}
}
function sysSetText(objTarget,currentValue)
{
	if (typeof currentValue=='undefined'||currentValue==null)
		currentValue='';
	if (objTarget.tagName.toUpperCase()=='TEXTAREA')
	{	objTarget.value = currentValue;
		//CHECK FOR RICHTEXT ATTRIBUTE
	}
	else
		objTarget.value = currentValue;
}
function sysGetRadio(objTarget)
{
	for	(var i=0;i<objTarget.length;i++)
	{
		if (objTarget[i].checked ==	true)
			return objTarget[i].value;
	}
	return '';
}
function sysGetSelect(objDrop)
{
//	for	(i=0;i<objDrop.options.length;i++)
//	{
//		if (objDrop.options[i].selected == true)
//			return objDrop.options[i].value;
//	}
//	return '';
    return (objDrop.selectedIndex >= 0 ? objDrop.options[objDrop.selectedIndex].value : '');
}
function sysGetSelectText(objDrop)
{
    return (objDrop.selectedIndex >= 0 ? objDrop.options[objDrop.selectedIndex].text : '');
}
function sysGetCheck(objTarget,byvalue)
{
    if (typeof byvalue == 'undefined' || byvalue == null)
    {
	    if (objTarget.checked==true)
		    return 'true';
	    else
		    return 'false';
    }
    else
    {
	    for	(var i=0;i<objTarget.length;i++)
	    {
		    if (objTarget[i].value==byvalue)
		    {
		        if (objTarget[i].checked ==	true)
			        return 'true';
			    else
			        return 'false';
			}
	    }
	    return null;        
    }
}
function sysGetText(objTarget)
{
	//CHECK FOR RICHTEXT ATTRIBUTE
	if (objTarget.tagName.toUpperCase()=='TEXTAREA')
	{	
		return objTarget.value;
	}
	else
		return objTarget.value;
}
function sysShortenSummary(value,length)
{

	//COMMENT
	 if (value==undefined)
		value = '';
	 if (length==undefined)
		length=50;
	if (value.length > length && length > 0)
	{
		value = value.substring(0,length-1);
	}
	//ENCODE HTML...
	value=encodehtml(value);

	return value;
}
function showPrint()
{
	sysShowPrintVersion=(!sysShowPrintVersion);
	owsSelect.render();
}
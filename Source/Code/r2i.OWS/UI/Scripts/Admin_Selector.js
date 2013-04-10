
function OWSCodeTree(variablename,nameOfSource,nameOfRootElements,nameOfChildElements,rootCId,rootCss,normalCss,hoverCss,selectedCss,selectedHoverCss,keyDownControl)
{
	this.name = variablename;
	this.className_normal = normalCss;
	this.className_hover = hoverCss;
	this.className_selected = selectedCss;
	this.className_selectedHover = selectedHoverCss;
	this.className_root = rootCss;
	this.element_root = rootCId;
	this.EventState = 0;
	this.Timeout = false;
	this.nameOfSrc = nameOfSource;
	this.nameOfSrcRootChildren = nameOfRootElements;
	this.nameOfSrcItemChildren = nameOfChildElements;
	this.selectedItemId = false;
	this.hover = function hover(obj,over)
	{
			obj.className = (obj.id!=this.selectedItemId)?((over)?this.className_hover:this.className_normal):((over)?this.className_selectedHover:this.className_selected);
			//this.setInputFocus();
	}
	this.keycontrol = document.getElementById(keyDownControl);
	this.keycontrol.onkeydown=function(e) {owsSelect.onkeydown(e)}; //window.onbeforeunload = function(e) { return "You are about to leave the Administration Interface. Any unsaved work will be lost permanently. Are you sure you want to do this?"; }
	this.remove = function(id)
	{
		if (selectedObj!=undefined&&selectedObj.id!=undefined)
		{
		 switch (selectedObj.id.substring(0,1).toLowerCase())
		 {
			case 'a': //ACTION
			if (id!=null&&id.length>0)
			{
				var objId;
				objId = id.substring(1,id.length);
				RemoveAction(objId);
	/*			if (this.selectedItemId==id)
				{
					this.selectedItemId=null;
					selectedObj = undefined;
					selectedActionObj = undefined;
				}
	*/
				this.abortClick = false;
				this.selectedItemId = false;
				this.hover(selectedObj,false);
				selectedObj = false;	
			}
				break;
			case 'c': //CONFIGURATION
				break;
			case 'r': //REPOSITORY
				break;
			case 'e': //EVENT
				break;
		 }
		}
 	}
	this.clear = function clear()
	{
		// do nothing yet.
	}
	
	this.timeout = false;
	this.select = function select(obj)
	{
		if (!this.abortClick)
		{
			window.clearTimeout(this.timeout);
			if (this.selectedItemId!=obj.id)
			{
					this.EventState=1;	
					this.show(obj.id);
					selectedObj = false;
					this.timeout = window.setTimeout(this.name + '.handle()',250);
			}
			else
			{
				this.EventState++;
				this.timeout = window.setTimeout(this.name + '.handle()',250);
			}
		}
		this.abortClick = false;
	}	
	this.selectedGUID = function () { return this.selectedItemId.substring(1,this.selectedItemId.length);};
	this.onkeydown = function onkeydown(e)
	{
		var keynum;var keychar;var numcheck;var shiftCheck;
		keynum = (window.event)?window.event.keyCode:((e.which)?e.which:null);
		shiftCheck = (window.event)?window.event.shiftKey:e.shiftKey;
		if (!(keynum==null))
		{
			switch(keynum)
			{
				case 10:
				case 13:
					//EDIT
					this.EventState=2;
					this.handle();
					break;
				case 32:
					//SELECT
					this.EventState=0;
					this.handle();
					break;
				case 46:
					//DELETE
					this.remove(this.selectedItemId);
					break;
				case 61:
				case 107:
					//EXPAND:
					var s = this.selectedGUID();
					this.expand(s);
					break;
				case 109:
					//COLLAPSE:
					var s = this.selectedGUID();
					this.collapse(s);
					break;
				case 37:
				case 100:
					//LEFT
					ActionMove('LEFT');
					break;
				case 38:
				case 104:
					//UP
					ActionMove('UP',shiftCheck);
					break;
				case 39:
				case 102:
					//RIGHT
					ActionMove('RIGHT');
					break;
				case 40:
				case 98:
					//DOWN
					ActionMove('DOWN',shiftCheck);
					break;
				case 191:
				case 111:
				case 220:
					var s = this.selectedGUID();
					this.describe(s);
					break;
				default:
					break;
			}
		}
	}
	this.show = function show(objectId)
	{
		var item = false;
		if (this.selectedItemId)
		{
			item = $(this.selectedItemId);
			if (item)
				item.className = this.className_normal;
		}
		if (objectId!=null)
		{
			this.selectedItemId = objectId;
			item = $(this.selectedItemId);
			if (item)
			{
				item.className = this.className_selected;
				item.outClassName = this.className_selected;		
			}
		}
		
	}
	this.setInputFocus = function setInputFocus()
	{
		if (this.keycontrol)
		this.keycontrol.focus();
	}
	this.abortClick = false;
	this.handle = function handle()
	{
		if (!this.abortClick)
		{
			var item = false;	
			if (this.selectedItemId)
			{
				item = $(this.selectedItemId);
				if (this.EventState>1)
				{ 
					//DOUBLE CLICK
					this.show(this.selectedItemId);
					//TODO:EXTERNAL
					if (LoadProperty(item))//selectedEventObj))
					{
						selectedObj = item;
					}
				}
				else
				{
					//SINGLE CLICK
					if (item.id!=selectedObj.id)
					{
						//TODO:EXTERNAL
/*						
						if (UnloadProperty(selectedObj))
						{
							this.unselect(selectedObj);
						}	
*/	
	//SELECT
							//TODO:EXTERNAL
							selectedActionObj = GetPropertyItem(item);
							selectedObj = item;
							this.setInputFocus();
						
					}
					else
					{
						//UNSELECT
						selectedActionObj = false;
						//TODO:EXTERNAL
						if (UnloadProperty(item))
							this.unselect(item);
					}
				}
			}
			this.EventState = 0;
		}
		this.abortClick = false;
	}
	this.unselect = function unselect(obj)
	{
		this.selectedItemId = false;
		this.hover(selectedObj,false);
		selectedObj = false;		
	}

	this.render = function render()
	{
		//TODO: EXTERNAL
		sysProperty_actionsInvalidated=false;

		var str = new Array();
		str.push('<UL id="' + this.element_root + '" class="' + this.className_root + '">');
		
		var src = false;
		eval('src = ' + this.nameOfSrc + '.' + this.nameOfSrcRootChildren);
		this.renderItem(src,str,null,false,true,true,true);
		
		str.push('</UL>');
		var strH = new Array();
		//TODO: EXTERNAL
		HListTitle.innerHTML = ('<center>Actions</center>');
		//TODO: EXTERNAL
		strH.push('<table width=100% border=0 cellpadding=0 cellspacing=0 class=HListHeader>');
		strH.push('<tr>');
		//TODO: EXTERNAL
		strH.push('<td class=HListHeaderLeft style=\'width: 50px;float:left;\'>Index</td >');
		//TODO: EXTERNAL
		strH.push('<td align=left>Action</td >');
		//TODO: EXTERNAL
		//var Print = '<a href="#" onclick="showPrint(this);return false;">Print</a>';
		var Print = '&nbsp;';
		strH.push('<td class=HListHeaderRight>' + Print + '</td>');
		strH.push('</tr>');
		strH.push('</table>');
		
		//TODO:EXTERNAL
		displayList(str.join(''),strH.join(''));
		this.mapItems(src);
		
		this.sharedIndex = 1;
		this.reindex(null);	
	}
	this.renderItem = function renderItem(actions,str,parentAction,loopChildren,renderParent,isRoot,renderLI)
	{
		var hasChildren = false;
		if (typeof renderParent=='undefined')
			renderParent = false;
		if (typeof loopChildren=='undefined')
			loopChildren = false;
		if (typeof renderLI=='undefined')
			renderLI = false;		
		var i = 0;
		if (actions!=undefined&&actions.length==undefined)
		{
			eval('if ((actions.' + this.nameOfSrcItemChildren + '!=null && actions.' + this.nameOfSrcItemChildren + '!=undefined && actions.' + this.nameOfSrcItemChildren + '.length > 0)) hasChildren=true;')
		
		
				str.push(renderLI?'<li>':'');
				if (navigator.userAgent.indexOf("Firefox")!=-1)
					str.push('<div class="cItem" onmouseover="' + this.name + '.hover(this,true);" onmouseout="' + this.name + '.hover(this,false);" id=a' + actions.GUID + ' onmousedown="' + this.name + '.select(this);">');
				else
					str.push('<div class="cItem" onmouseover="' + this.name + '.hover(this,true);" onmouseout="' + this.name + '.hover(this,false);" id=a' + actions.GUID + ' onmouseup="' + this.name + '.select(this);">');
					str.push('<u class="LineNumber">');
						str.push('<img  onmousedown="' + this.name + '.remove(\'a' + actions.GUID + '\');" src="images/delete.gif">');			
						str.push(actions.Index + '.');
					str.push('</u>'); 
					if (hasChildren)
					{
						str.push('<div onmousedown="' + this.name + '.');
						//TODO:External
						str.push(loopChildren?'collapse':'expand');
						str.push('(\'' + actions.GUID + '\');"');
						str.push(' class="');
						str.push(loopChildren?'Collapse':'Expand');
						str.push('">&nbsp;</div>');
					}
					//TODO: External
					str.push(sysActionHandleCommand(actions,null,'PRINT'));
				str.push('</div>');

				if (hasChildren && loopChildren) 
					this.renderItem(this.getChildren(actions),str,actions,false,true,false,true);
			
				str.push(renderLI?'</li>':'');
		}
		else
		{
			if ((actions!=null && actions!=undefined && actions.length > 0))
				hasChildren=true;		
				
			if (hasChildren)
			{		
				
				if (!isRoot)
				{
					str.push('<ul class="');
					//TODO:External
					str.push(loopChildren?'Contract':'Expand');
					str.push('">');
				}

				for (i=0;i<actions.length;i++)
				{
						//TODO:External
						if (actions[i]!=null) {
							if (actions[i].GUID==undefined)
								actions[i].GUID = Bi4ce.GenerateGuid.newGuid().toString('N');
							
							//TODO:External 
							actions[i].parentIndex = i;
							actions[i].parentAction = parentAction;
							
							var iloopchildren = false;
							if (typeof actions[i]._isExpanded != 'undefined')
								iloopchildren = actions[i]._isExpanded;

							eval('this.renderItem(actions[i],str,parentAction,iloopchildren,true,true,true);');
						}
				}
				
				if (!isRoot)
					str.push('</ul>');
			}
		}
	}
	
	this.sharedIndex = 0;
	this.setIndex = function setIndex(obj)
	{
		if (obj.Node!=undefined)
		{
			var a;
			a = obj.Node.getElementsByTagName('U');
			if (a!=undefined&&a.length>0)
			{ 
				a[0].innerHTML = '<img  onclick="' + this.name + '.remove(\'a' + obj.GUID + '\');" src="images/delete.gif">' + obj.Index + '.';
			}
		}
	}
	this.describe = function describe(id)
	{
		var target = null;
		var list = null;
		var str = new Array();
		
		if (id!=undefined&&id!=null)
		{
			target = (getParent($('a' + id)));
			//eval('list = ' + this.name + 'getItem(id,' + this.nameOfSrc + '.' + this.nameOfSrcRootChildren + ');');
			list = this.getItem(id,this.getRoot());
		}
		if (list!=null)
		{
			list._showAll = (typeof list._showAll == 'undefined'?true:!list._showAll);
			this.renderItem(list,str,true,(typeof list._isExpanded == 'undefined'?false:list._isExpanded),false,false,false);
			target.innerHTML = str.join('');
			//eval(this.name + '.mapItems(' + this.nameOfSrc + '.' + this.nameOfSrcRootChildren + ');');
			//this.mapItems(this.getRoot());
		}
		//TODO:EXTERNAL
		handleResize();	
	}
	this.expand = function loadTree(id)
	{
		this.abortClick = true;
		var target = null;
		var list = null;
		var str = new Array();
		
		if (id!=undefined&&id!=null)
		{
			target = (getParent($('a' + id)));
			//eval('list = ' + this.name + 'getItem(id,' + this.nameOfSrc + '.' + this.nameOfSrcRootChildren + ');');
			list = this.getItem(id,this.getRoot());
		}
		if (list!=null)
		{
			list._isExpanded = true;
			this.renderItem(list,str,true,true,false,false,false);
			target.innerHTML = str.join('');
			//eval(this.name + '.mapItems(' + this.nameOfSrc + '.' + this.nameOfSrcRootChildren + ');');
			this.mapItems(this.getRoot());
		}
		if (list==null && (id==undefined||id==null))
		{
			this.render();
		}
		//TODO:EXTERNAL
		handleResize();
	}
	
	this.collapse = function hideActionTree(id)
	{
			this.abortClick = true;
			var target = getParent($('a' + id));
			var list = null;
			list = this.getItem(id,this.getRoot());
			var str = new Array();
			if (list!=null)
			{
				//buildListItem(list,str,false);
				//actions,str,parentAction,loopChildren,renderParent,isRoot,renderLI
				list._isExpanded = false;
				this.renderItem(list,str,null,false,false,false,false);
				target.innerHTML = str.join('');
			}
			//TODO:EXTERNAL
			handleResize();
	}
	
	
	this.reindex = function reindexActionTree(obj,parentObj)
	{
		this.sequence(this.getRoot(),null);
	}
	
	this.sequence = function ActionSequence(obj,parentObj,src)
	{
		if (src==undefined)
		{	src = new Array(); src.push(1); }
		if (obj!=undefined&&obj!=null)
		{
			if (obj.length!=undefined)
			{
				var xi=0;
				for (xi=0;xi<obj.length;xi++)
					this.sequence(obj[xi],parentObj,src);
			}
			else
			{
				obj.Index = src[0];
				obj.parentAction = parentObj;
				this.setIndex(obj,parentObj);
				src[0]++;
				//arrayObj = obj.' + this.nameOfSrcItemChildren + ';');
				arrayObj = this.getChildren(obj);
				this.sequence(arrayObj,obj,src);
			}
		}	
	}
	
	this.getRoot = function getRootItems()
	{
		var obj = null;
		eval('obj = ' + this.nameOfSrc + '.' + this.nameOfSrcRootChildren + ';');
		return obj;
	}
	this.getChildren = function getChildren(item)
	{
		var obj = null;
		if (typeof item != 'undefined' && item != null)
			eval('if (typeof item.' + this.nameOfSrcItemChildren + ' != \'undefined\') obj = item.' + this.nameOfSrcItemChildren + ';');
		return obj;	
	}
	this.getItem_Previous = function getItem_Previous(item,allowjump)
	{
		var parentArray = this.getItem_ParentArray(item);
		var myIndex = parentArray.indexOf(item);
		if (myIndex > 0)
		{
			var currentPrevious = parentArray[myIndex-1];
			while (allowjump)
			{
				var children = this.getChildren(currentPrevious);
				var iloopchildren = false;
				if (children!=null && typeof currentPrevious._isExpanded != 'undefined')
					iloopchildren = currentPrevious._isExpanded;
				if (iloopchildren && children.length > 0)
					currentPrevious = children[children.length-1];
				else
					allowjump = false;
			}
			return currentPrevious;
		}
		else
		{
			if (allowjump && typeof item.parentAction != 'undefined' && item.parentAction!=null)
				return item.parentAction;
		}
	}
	this.getItem_Next = function(item,allowjump)
	{
		var parentArray = this.getItem_ParentArray(item);
		var myIndex = parentArray.indexOf(item);
		if (myIndex < parentArray.length)
		{
			return parentArray[myIndex+1];
		}
		else
		{
			if (allowjump && typeof item.parentAction != 'undefined' && item.parentAction!=null)
			{
				return this.getItem_Next(item.parentAction,allowjump);
			}
			else
				return null;
		}		
	}
		
	this.move = function(src,target,direction,shift)
{
	if (src==null)
		return 0;
	var isgroup = false;
	var refreshArray = new Array();
	if (typeof src.length != 'undefined')
		isgroup = true;
		
	for (var i=0; isgroup && src.length > i;i++)
	{
		if (refreshArray.indexOf(src[i].GUID)==-1)
			 refreshArray.push(src[i].GUID);
		this.getItem_ParentArray(src[i]).remove(src[i]);
	}
	
	if (!isgroup)
	{	
		
		if (src.parentAction!=null&&refreshArray.indexOf(src.parentAction.GUID)==-1)
			 refreshArray.push(src.parentAction.GUID);
		else if (src.parentAction==null)
			 refreshArray.push(null);
		if (direction<0)
		{ //MOVE UP
			var previous = this.getItem_Previous(src,shift);
			if (previous!=null)
			{
				var startIndex = this.getItem_ParentArray(src).indexOf(src);
				this.getItem_ParentArray(src).remove(src);
				if (shift && previous.parentAction==src.parentAction)
				{
					//last child of previous
					this.getChildren(previous).push(src);
					src.parentAction = previous;
					if (refreshArray.indexOf(previous.GUID)==-1)
						refreshArray.push(previous.GUID);
				}
				else 
				{
					//above previous
					var index = this.getItem_ParentArray(previous).indexOf(previous);
					
					if (shift&&startIndex>0)
						index+=1;
					if (previous.parentAction!=null&&refreshArray.indexOf(previous.parentAction.GUID)==-1)
						refreshArray.push(previous.parentAction.GUID);
					this.getItem_ParentArray(previous).insert(src,index);
				}
			}
		}
		else
		{ //MOVEDOWN
			var next = this.getItem_Next(src,shift);
			if (shift&&next==null&&src.parentAction!=null)
			{
				this.getItem_ParentArray(src).remove(src);
				//last child of next parent
				next = src.parentAction;
				src.parentAction = next.parentAction;
				if (next.parentAction!=null&&refreshArray.indexOf(next.parentAction.GUID)==-1)
					refreshArray.push(next.parentAction.GUID);
				else if (next.parentAction==null&&refreshArray.indexOf(null)==-1)
					refreshArray.push(null);
				if (next==null)
					return 0;
				this.getItem_ParentArray(next).push(src);
			}
			else if (next!=null) {
				this.getItem_ParentArray(src).remove(src);
				if (shift&&next.parentAction==src.parentAction)
				{
					//first child of next
					src.parentAction = next;
					if (refreshArray.indexOf(next.GUID)==-1)
						refreshArray.push(next.GUID);
					this.getChildren(next).insert(src,0);
				}
				else if (next.parentAction==src.parentAction)
				{
					//after next
					if (next.parentAction!=null && refreshArray.indexOf(next.parentAction.GUID)==-1)
						refreshArray.push(next.parentAction.GUID);
					this.getItem_ParentArray(next).insert(src,this.getItem_ParentArray(next).indexOf(next)+1);
				}
				else if (next.parentAction!=null)
				{
					//last child of next parent
					src.parentAction = next.parentAction;
					if (refreshArray.indexOf(next.parentAction.GUID)==-1)
						refreshArray.push(next.parentAction.GUID);
					this.getChildren(next.parentAction).push(src);
				}
			}
			else
				return 0;
		}
	}
	else
	{
	}
	this.reindex(null);
	var mustRefresh = false;
	for (var i = refreshArray.length-1;i>=0;i--)
	{
		if (refreshArray[i]==null)
			mustRefresh=true;
		this.expand(refreshArray[i]);
	}
	if (mustRefresh||src.parentAction==null)
		this.expand(null);
	//handleResize();	
}
this.expandItem = function(item)
	{
	if (item == null)
		this.expand(null);
	else
		this.expand(item.GUID);
	}	
	this.getItem_ParentArray = function(item)
	{
		if (typeof item.parentAction != 'undefined' && item.parentAction!=null)
			return this.getChildren(item.parentAction)
		else
			return this.getRoot();
	}
	this.getIndex = function getItem_Index(item)
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
	}
	this.getItem = function getListItem(id,actions)
	{
		var i = 0;
		var trg = null;
		if (actions!=undefined&&actions.length==undefined)
		{
			if (actions.GUID==id)
			{
				return actions;
			}
			else
				return null;
		}
		else
		{
			for (i=0;i<actions.length;i++)
			{
				if (actions[i]==null||actions[i].GUID!=id)
				{
					if (actions[i]==null)
						return null;
					var children = this.getChildren(actions[i]);
					if (children!=undefined&&children.length!=undefined && children.length>0)
					{
						trg = this.getItem(id,children);
						if (trg!=null)
							return trg;
					}
					//NEW?
					else if (children!=undefined&&children.length==undefined)
					{
						trg = getItem(id,children);
						if (trg!=null)
							return trg;
					}
				}
				else
				{
					return actions[i];
				}
			}
		}
		return trg;
	}
	
	this.mapItems = function mapListItems(actions)
	{
	    try {
		var i = 0;
		if (actions!=undefined&&actions.length==undefined)
		{
			if (actions.Node==undefined)
				actions.Node = $('a' + actions.GUID);
			var children = this.getChildren(actions);
			if ((children!=null && children!=undefined || children.length > 0))
				this.mapItems(children);
		}
		else
		{
			for (i=0;i<actions.length;i++)
			{
				if (actions[i].Node==undefined)
					actions[i].Node = $('a' + actions[i].GUID);
			
				var children = this.getChildren(actions[i]);
				if ((children!=null && children!=undefined || children.length > 0))
					this.mapItems(children);
			}
		}	
		}
		catch(ex)
		{
		
		}
	}


}
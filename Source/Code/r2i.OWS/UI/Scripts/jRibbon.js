function Ribbon(callerobj,Config)
{
	this.configuration = Config;
	this.caller = callerobj;
	this.target = null;
	this.menutarget = null;
	this.selected = null;
	this.state=null;
	this.Editors = new Array();
	
	function RibbonEditor(editor,helper)
	{
		this.cEditor = editor;
		this.cHelper = helper;
		this.RibbonTab = false;
		this.RibbonMenu = false;
		this.Editor = function()
		{
			if (this.cEditor!=undefined && this.cEditor.id==undefined)
			{
				this.cEditor = $(this.cEditor);
			}
			return this.cEditor;
		}
		this.Helper = function()
		{
			if (this.cHelper!=undefined && this.cHelper.id==undefined)
			{
				this.cHelper = $(this.cHelper);
			}
			return this.cHelper;
		}
	}
	
	this.Initialize = function (target,menutarget,editorIndex)
	{
		this.target = target;
		this.menutarget = menutarget;
		if (this.selected==null&&this.configuration.Menu!=undefined)
		{
			for (var i=0;i<this.configuration.Menu.length;i++)
			{
				if (this.configuration.Menu[i].Default!=undefined && this.configuration.Menu[i].Default.toUpperCase() == 'TRUE')
				{
					this.selected = this.configuration.Menu[i];
					i = this.configuration.Menu.length+1;
				}
			}
		}
		this.Render(editorIndex);
	}
	this.Regenerate = function(txtobj) {
		this.Render(txtobj.EditorIndex);
	}
	this.Toggle = function(groupname,txtobj) {
		var canLoad = true;
		if (typeof this.Editors[groupname] != 'undefined')
		{
			if (txtobj.id == this.Editors[groupname].cEditor.id)
			{
				canLoad = false;
			}
			else
				this.Remove(this.Editors[groupname].cEditor);
		}
		if (canLoad) this.Editors[groupname]=this.Generate(txtobj);
	}
	this.Remove = function(txtobj) {
			if (typeof txtobj.EditorIndex != 'undefined' && txtobj.EditorIndex != null)
			{
				var rEditor = this.Editors[txtobj.EditorIndex];
				this.Editors[txtobj.EditorIndex]=null;
				txtobj.EditorIndex = null;
				txtobj.onkeydown=false;
				txtobj.setAttribute('isLoaded','false');
				txtobj.parentNode.removeChild(rEditor.RibbonTab);
				txtobj.parentNode.removeChild(rEditor.RibbonMenu);
				rEditor.RibbonTab = false;
				rEditor.RibbonMenu = false;
			}
	}
	this.Generate = function(txtobj) {
		var randomnumber=Math.floor(Math.random()*32767)
		//this.HelpTabID='RibbonHelp' + randomnumber;
		
		var editorIndex = 0;
		
		var rEditor = new RibbonEditor(txtobj,'RibbonHelp' + randomnumber);
		this.Editors.push(rEditor);
		editorIndex=this.Editors.length-1;
		txtobj.EditorIndex = editorIndex;
				
		var override=true;
		var rtext = true;
		var isLoadedObj = txtobj.getAttribute('isLoaded');
		if (typeof isLoadedObj=='undefined' || isLoadedObj==null)
			isLoadedObj = false;
		else
			if (isLoadedObj =='false')
				isLoaded=false;
			else
				isLoaded=true;
			
		if(isLoadedObj==true)
			rtext=false;
			
		if(rtext){
			txtobj.setAttribute('isLoaded','true');
			this.EditorSetup(txtobj);
			eval('txtobj.onkeydown=function(event){return window.' + this.caller + '.EditorKeyDown(this,event)};');
			var dv=document.createElement('div');
			dv.className=this.configuration.MainClass;

			txtobj.parentNode.insertBefore(dv,txtobj);
			var dvm = document.createElement('div');
			dvm.className=this.configuration.EditorMenuBarClass;
			
			this.Initialize(dv,dvm,editorIndex);
			
			txtobj.parentNode.insertBefore(dvm,dv);		
			
			rEditor.RibbonTab = dv;
			rEditor.RibbonMenu = dvm;
		}
		return rEditor;
	}		
	this.EditorSetup = function(item)
	{
		var iMinHeight=50;var iHitHeight=50;var iMinWidth=50;var iHitWidth=50;
		var cookiei=0;
		if(lxGetCookie(this.caller)!=null)
		cookiei=parseInt(lxGetCookie(this.caller).replace('px',''));
		var itemi=parseInt(item.style.height.replace('px',''));
		if(itemi<iMinHeight||cookiei<iMinHeight){itemi=iMinHeight;lxtxtUp(item);}
		else{item.style.height=lxGetCookie(this.caller);}		
	}
	this.EditorReplaceSelection = function (input,replaceString,replaceValue,itemCursor){
		if (itemCursor==null)
				itemCursor = 0;	
		if (!isNaN(input))
			input = this.Editors[input].Editor();
	input.focus();		
		if(input.setSelectionRange)
			{	
				var selectionStart=input.selectionStart;
				var selectionEnd=input.selectionEnd;
				var scrollTop=input.scrollTop;
				if(replaceValue!=undefined&&selectionEnd>selectionStart){
					var result=input.value.substring(selectionStart,selectionEnd);
					replaceString=this.EditorReplace(replaceValue,result);
				}
				input.value=input.value.substring(0,selectionStart)+replaceString+input.value.substring(selectionEnd);
				input.scrollTop=scrollTop;
				if(selectionStart !=selectionEnd){
					this.EditorSelectionRange(input,selectionStart,selectionStart+replaceString.length-itemCursor);
				}
				else
				{
					this.EditorSelectionRange(input,selectionStart+replaceString.length,selectionStart+replaceString.length-itemCursor);
				}
			}
		else if(document.selection)
				{ 
					var range=document.selection.createRange();
					if(range.parentElement()==input)
					{
						if(replaceValue!=undefined)
						{
							var result=range.text;
							replaceString=this.EditorReplace(replaceValue,result);
						}
						var isCollapsed=range.text=='';
						range.text=replaceString;
						if(!isCollapsed){
							range.moveStart('character',-replaceString.length-itemCursor);range.select();
						}
					}
				}
	}	
	this.EditorGetSelection = function (input){
	
		if (!isNaN(input))
			input = this.Editors[input].Editor();
	input.focus();		
		if(input.setSelectionRange)
			{	
				var selectionStart=input.selectionStart;
				var selectionEnd=input.selectionEnd;
				var scrollTop=input.scrollTop;
					var result=input.value.substring(selectionStart,selectionEnd);
					return result;
			}
		else if(document.selection)
				{ 
					var range=document.selection.createRange();
					if(range.parentElement()==input)
					{
							var result=range.text;
							return result;
					}
				}
	}	
	this.EditorReplace = function (replaceValue,input)
		{
			if (!isNaN(input))
				input = this.Editors[input].Editor();
				
			var result=input;
			if(replaceValue==true)
			{	var re=new RegExp("\\\\+","g");var s='';var lindex=0;var m=re.exec(result);
				while(m!=null){
					if(m.index>lindex)
					{s=s+result.substring(lindex,m.index);}
					s=s+result.substring(m.index,m.index+m.length);
					lindex=m.index+m.length;
					m=re.exec(result);
				}
				if(lindex<result.length)
					{s=s+result.substring(lindex);}
				result=s;
				re=new RegExp("[\"]","g");
				result=result.replace(re,"\\\"");
				re=new RegExp("[\[]","g");
				result=result.replace(re,"\\[");
				re=new RegExp("[\\]]","g");
				result=result.replace(re,"\\]");
				re=new RegExp("[\{]","g");
				result=result.replace(re,'\\{');
				re=new RegExp("[\}]","g");
				result=result.replace(re,'\\}');
			}
			else
			{
				var re=new RegExp("\\\\+","g");
				var s='';
				var lindex=0;
				var m=re.exec(result);
				while(m!=null){
					if(m.index>lindex)
						{s=s+result.substring(lindex,m.index);}
					s=s+result.substring(m.index,m.index+m.length-1);lindex=m.index+m.length;m=re.exec(result);
				}
				if(lindex<result.length){s=s+result.substring(lindex);}
				result=s;
			}
		return result;
	}
	this.EditorSelectionRange = function(input,selectionStart,selectionEnd)
	{
		if (!isNaN(input))
			input = this.Editors[input].Editor();
		if(input.setSelectionRange)
		{
			input.focus();
			input.setSelectionRange(selectionStart,selectionEnd);
		}
		else if(input.createTextRange)
		{
			var range=input.createTextRange();
			range.collapse(true);
			range.moveEnd('character',selectionEnd);
			range.moveStart('character',selectionStart);
			range.select();
		}
	}	
	this.EditorKeyDown = function (item,e)
	{
	
			if(!e)
				e=window.event;
			c=e.which?e.which:e.keyCode;
			if(c==9){
				return this.EditorKeyDownHandler(item,'TAB');}
			else if(c==40&&e.ctrlKey){
				return this.EditorKeyDownHandler(item,'UP');}
			else if(c==38&&e.ctrlKey){
				return this.EditorKeyDownHandler(item,'DOWN');}
			else if(c==39&&e.ctrlKey&&e.altKey){
				return this.EditorKeyDownHandler(item,'RIGHT');}
			else if(c==37&&e.ctrlKey&&e.altKey){
				return this.EditorKeyDownHandler(item,'LEFT');}
			else if(c==69&&e.ctrlKey){
				return this.EditorKeyDownHandler(item,'ESCAPE');}
			else if(c==82&&e.ctrlKey){
				return this.EditorKeyDownHandler(item,'UNESCAPE');}
			else if(c==192&&e.ctrlKey){
				return this.EditorKeyDownHandler(item,'AUTOPASTE');}
			else if(c==192&&e.altKey){
				return this.EditorKeyDownHandler(item,'AUTOCOPY');}
		}
	this.onItemClick = function (input,group,tab,item)
	{
		if (!isNaN(input))
		input = this.Editors[input].Editor();
		
		tab = eval('this.configuration.Groups.' + group + '.Tabs[' + tab + ']');
		if (tab!=undefined && tab!=null)
		{
			item = tab.ItemEditor[item];
			if (item.length!=undefined && item.length > 0)
			{
				this.EditorReplaceSelection(input,item);
			}
			else
			{
				if (item.Cursor!=undefined)
				{
					var itemCursor = item.Cursor;
					item = item.Text;
					if (item.length!=undefined && item.length > 0)
					{
						this.EditorReplaceSelection(input,item,null,itemCursor);
					}					
				}
			}
		}
		return false;
	}
	var txtClip = '';
	var txtPasteItem = null;
	this.PasteHandler = function (lastPosition, sl)
	{
		/* settings values */
		var delayMax = 100;
		var ta; 
		ta = this.txtPasteItem;
		var mt = "";
		//var sl = 0;
		if (sl == undefined || sl==null)
		{
			sl = 0;
		}
		var charWidth = 7;
		var tabWidth = 8 * charWidth;
		/* this should be charWidth * the width of the textarea */
		var slStart = - ta.clientWidth;//-30 * charWidth;

		
		
		var c;
		c = this.txtClip[lastPosition];
		ta.value = ta.value + c
		
		if (c=="\n")
		{
			sl = slStart;
		}
		else if (c=="\t")
		{
			sl+= tabWidth;
		}
		else
		{
			sl+= charWidth;
		}
		ta.scrollLeft = (sl>0?sl:0);
		ta.scrollTop = 5000;	
		
		
		lastPosition++;
		if (lastPosition < this.txtClip.length)
		{
			var waiter = Math.round(delayMax*Math.random());
			//window.setTimeout('RibbonEditor.PasteHandler(' + lastPosition + ');',waiter);
			window.setTimeout('RibbonEditor.PasteHandler(' + lastPosition + ',' + sl + ');',waiter);
		}
	}
	this.EditorKeyDownHandler = function(item,code)
	{
		var iMinHeight=50;var iHitHeight=50;var iMinWidth=50;var iHitWidth=50;
		switch(code.toUpperCase())
		{
			case 'TAB':
				this.EditorReplaceSelection(item,String.fromCharCode(9));
				return false;
				break;
			case 'AUTOPASTE':
				this.txtPasteItem = item;
				this.PasteHandler(0);
				return false;
				break;				
			case 'AUTOCOPY':
				this.txtClip = this.EditorGetSelection(item);
				return false;
				break;				
			case 'UP':
				var itemi=parseInt(item.style.height.replace('px',''));
				if(itemi>=iMinHeight){item.style.height=(itemi+iHitHeight)+'px';}
				else{item.style.height=iMinHeight+'px';}
				lxSetCookie(this.caller,item.style.height);
				return false;
				break;
			case 'DOWN':
				var itemi=parseInt(item.style.height.replace('px',''));
				if(itemi>iMinHeight){item.style.height=(itemi-iHitHeight)+'px';}
				else{item.style.height=iMinHeight+'px';}
				lxSetCookie(this.caller,item.style.height);
				return false;
				break;
			case 'RIGHT':
				var itemi=parseInt(item.style.width.replace('px',''));
				if(itemi>=iMinWidth){item.style.width=(itemi+iHitWidth)+'px';}
				else{item.style.width=iMinWidth+'px';}
				break;
			case 'LEFT':
				var itemi=parseInt(item.style.width.replace('px',''));
				if(itemi>iMinWidth){item.style.width=(itemi-iHitWidth)+'px';}
				else{item.style.width=iMinWidth+'px';}
				return false;
				break;
			case 'ESCAPE':
				this.EditorReplaceSelection(item,'',true);
				return false;
				break;
			case 'UNESCAPE':
				this.EditorReplaceSelection(item,'',false);
				return false;
				break;
		}
	}
	this.onMenu = function (menuindex,editorIndex)
	{
		if (this.configuration.Menu!=undefined&&this.configuration.Menu.length > menuindex)
		{
			this.selected = this.configuration.Menu[menuindex];
			if (this.selected.onShow!=undefined && this.selected.onShow.length>0)
			{
				eval(this.selected.onShow);
			}
		}	
		this.Render(editorIndex);
	}
	this.onMenuHover = function (o)
	{
		o.xclassName = o.className;
		if (o.className!=this.configuration.SelectedClass)
			o.className= this.configuration.HoverClass;
	}
	this.onMenuBlur = function (o)
	{
	 o.className=o.xclassName;
	}	
	this.onItemHover = function (o,group,iindex,tindex,editorIndex)
	{
		o.xclassName = o.className;
		o.className=this.configuration.ItemHoverClass;
		if (group!=undefined && group!=null)
		{
			var helptxt = eval('this.configuration.Groups.' + group + '.Tabs[' + tindex + '].ItemHelp[' + iindex + ']');
			if (helptxt!=null && this.Editors[editorIndex]!=null && typeof this.Editors[editorIndex].Helper != 'undefined' && this.Editors[editorIndex].Helper !=null && this.Editors[editorIndex].Helper() != null)
				this.Editors[editorIndex].Helper().innerHTML = helptxt;
		}
	}
	this.onItemBlur = function (o,group,iindex,tindex)
	{
		o.className=o.xclassName;
	}
	this.Render = function (editorIndex)
	{
		arr = new Array();
		if (this.menutarget!=null && this.configuration!=null)
		{
			if (this.configuration.Menu!=null && this.configuration.Menu.length > 0)
				this.renderMenu(arr,editorIndex);
			this.menutarget.innerHTML = arr.join('');
		}
		arr = new Array();
		if (this.target!=null  && this.configuration!=null && this.selected!=null)
		{
			var groups = this.selected.Groups;
			arr.push('<table cellpadding=0 cellspacing=0>');
			if (groups!=null && groups.length > 0)
			{
				arr.push('<TR>');			
				for (var i=0;i<groups.length;i++)
				{
					//RENDER GROUP 
					var group = false;
					eval('group=this.configuration.Groups.' + groups[i]);
					this.renderGroup(this.configuration.Rows,group,arr,editorIndex);
					if (i<groups.length-1)
					{
						arr.push('<TD class="' + this.configuration.GapClass + '"></TD>');
					}
				}
				arr.push('</TR>');							
				arr.push('<TR>');			
				for (var i=0;i<groups.length;i++)
				{
					//RENDER FOOTER
					this.renderGroupFooter(this.configuration.Rows,eval('this.configuration.Groups.' + groups[i]),arr,editorIndex);
					if (i<groups.length-1)
					{
						arr.push('<TD class="' + this.configuration.GapClass + '"></TD>');
					}
				}
				arr.push('</TR>');							
			}
			arr.push('</table>');	
			this.target.innerHTML = arr.join('');
		}
	}
	this.renderGroupFooter = function (rows,group,arr,editorIndex)
	{
		colspan = 0;
		if (group.Tabs != null && group.Tabs.length > 0)
		{
			for (var tti = 0; tti < group.Tabs.length; tti++)
			{
				tabcolumns = 0;
				itemcounter = 0;			
				if (group.Tabs[tti].Columns!=undefined)
					tabcolumns=1;
				else
				{
					if (group.Tabs[tti].Image!=undefined && group.Tabs[tti].Image.length > 0)
					{	itemcounter++;itemcounter++; }
					if (group.Tabs[tti].ImageMap!=undefined && group.Tabs[tti].ImageMap.length > 0)
						itemcounter++;
					if (group.Tabs[tti].Items!=undefined)
						itemcounter = itemcounter + group.Tabs[tti].Items.length;
					tabcolumns = parseInt(itemcounter / rows);
					if (itemcounter / rows > tabcolumns)
						tabcolumns++;				
				}
				colspan = colspan + tabcolumns;
			}
		}		
		if (colspan > 0)
		{
			arr.push('<td class="' + this.configuration.GroupClass + '" colspan=' + colspan + '>' + group.Name + '</td>');
		}
	}
	this.renderMenu = function (arr,editorIndex)
	{
		if (this.configuration.Menu != null && this.configuration.Menu.length > 0)
		{
			arr.push('<div class="' + this.configuration.MenuBarClass + '">');
			for (var mnuI = 0; mnuI < this.configuration.Menu.length; mnuI++)
			{
				if (this.state!=null && this.configuration.Menu[mnuI].show==this.state || this.configuration.Menu[mnuI].show=='')
					this.renderMenuItem(this.configuration.Menu[mnuI],mnuI,arr,editorIndex);
			}
			arr.push('</div>');
		}	
	}
	this.renderMenuItem = function (group,index,arr,editorIndex)
	{
		arr.push('<div ' + (this.selected!=null&&group.Name==this.selected.Name?'class="' + this.configuration.SelectedClass+'"':'class=' + this.configuration.MenuClass + '"') + ' onclick="' + this.caller + '.onMenu(' + index + ',' + (editorIndex==null?'null':editorIndex)  + ');" onmouseover="' + this.caller + '.onMenuHover(this);" onmouseout="' + this.caller + '.onMenuBlur(this);">' + group.Name + '</div>');
	}
	this.renderGroup = function (rows,group,arr,editorIndex)
	{
		if (group.Tabs != null && group.Tabs.length > 0)
		{
			for (var tabI = 0; tabI < group.Tabs.length; tabI++)
			{
				group.Tabs[tabI].Index=tabI;
				this.renderTab(rows,group,group.Tabs[tabI],(tabI==0?true:false),(tabI==group.Tabs.length-1?true:false),arr,editorIndex,tabI);
			}
		}
	}
	this.renderTab = function (rows,group,tab,firsttab,lasttab,arr,editorIndex)
	{
		var thisEditorIndex = -1;
		var classname = '';
		var isLastTab = lasttab;
		var crow = 0;
		if (tab.Image!=undefined && tab.Image.length > 0)
		{ crow+=2; 
		  if (tab.ImageMap!=undefined && tab.ImageMap.length > 0)
			crow++;	
		}
		if ((tab.Columns==undefined && rows-crow < (tab.Items!=undefined?tab.Items.length:0)) || tab.Columns!=undefined && rows-crow < tab.Columns )
		{
			lasttab=false;
		}	
		crow = 0;
		if (editorIndex!=undefined)
			thisEditorIndex = editorIndex;	
		
		if (lasttab && firsttab)
			classname = this.configuration.SingleClass;
		else if (lasttab)
			classname = this.configuration.RightClass;
		else if (firsttab)
			classname = this.configuration.LeftClass;
		else
			classname = this.configuration.CenterClass;
		if (tab.Items==undefined || tab.Items.length == 0)
			classname = this.configuration.CenterImageClass;
			
		var tdc = '<div>';
		var tde = '</div>';
		var tc = 'div';
		if (tab!=null)
		{
			var style = '';
			if (tab.Style!=undefined)
				style = ' style="' + tab.Style + '" ';
				
			if (tab.Image!=undefined && tab.Image.length > 0)
			{
				arr.push('<td class="' + classname + '"' + style + '>' + tdc);
				crow = crow + 2;
				if (tab.ImageMap!=undefined && tab.ImageMap.length > 0)
				{
					arr.push('<img src="' + tab.Image + '" border=0 useMap="' + tab.ImageMap + '">');
					crow++;
				}
				else
				{
					arr.push('<img src="' + tab.Image + '">');
				}
				if (crow == rows)
				{ arr.push(tde + '</td>'); crow = 0; }
			}
			else
			{
				//tdc = '<UL>';
				//tde = '</UL>';
				//tc = 'li';
			}

			if (tab.Columns!=undefined)
			{
				arr.push('<td class="' + classname + '"' + style + '>');
				if (tab.Help==undefined || tab.Help.toUpperCase()!='TRUE')
				{
					arr.push('<div class="' + tab.ColumnClass + '">');
					arr.push('<table width=100% border=0 cellpadding=0 cellspacing=0>');
				}
				else
				{
					if (editorIndex!=undefined)
					{
						var chelper=this.Editors[editorIndex].cHelper;

						arr.push('<div class="' + tab.ColumnClass + '" id="' + chelper + '">');
						arr.push(tab.DefaultHelp);
					}
				}
			}
			if (tab.Items!=undefined && tab.Items.length > 0)
			{
				for (var ti = 0;tab.Items!=undefined && ti < tab.Items.length; ti++)
				{
					lasttab=false;
					if (tab.Columns==undefined && isLastTab && rows-crow > tab.Items.length-ti-1)
						lasttab=true;
					if (lasttab && firsttab)
						classname = this.configuration.SingleClass;
					else if (lasttab)
						classname = this.configuration.RightClass;
					else if (firsttab && tab.Columns==undefined)
						classname = this.configuration.LeftClass;
					else
						classname = this.configuration.CenterClass;
						
					if (tab.Columns!=undefined)
					{
						if (crow==0)
						{arr.push('<tr>');}
						if (crow >=0)
						{arr.push('<td class="' + classname + '">' + tdc); }
						classname = this.configuration.CenterClass;
					}
					else
					{
						if (crow==0)
						{arr.push('<td class="' + classname + '"' + style + '>' + tdc); }
					}
						
					var item = tab.Items[ti];
					var onclick = '';
					var onclickparameters = '()';
					
					if (tab.onclick==undefined||tab.onclick==null||tab.onclick.length==0)
					{
						if (this.configuration.onItem!=undefined && this.configuration.onItem!=null)
						{
							onclick= eval(this.configuration.onItem);
							onclickparameters =	'(' + thisEditorIndex + ')';
						}
					}
					else
					{
						onclick=  eval(tab.onclick);
						onclickparameters = '(' + thisEditorIndex + ')';
					}
					onclick = onclick.replace(' ','_').replace('-','_').replace('\'','_');
					
					if (editorIndex!=undefined && tab.ItemEditor!=undefined && tab.ItemEditor!=null && tab.ItemEditor.length == tab.Items.length)
					{
						if (tab.ItemEditor[ti]!=null && (tab.ItemEditor[ti].length != undefined || tab.ItemEditor[ti].Text != undefined))
						{
							onclick = this.caller + '.onItemClick';
							onclickparameters = '(' + editorIndex + ',\'' + group.Abbr + '\',' + tab.Index + ',' + ti + ')';
						}
					}
					else
					{
						if (eval('!window.' + onclick) && this.configuration.debug!=undefined)
							alert('The Ribbon control requires a definition for function: ' + onclick);
					}

					var image = null;
					if (this.configuration.defaultItemImage!=undefined)
						image = this.configuration.defaultItemImage;
					if (tab.ItemImages!=undefined && tab.ItemImages[ti].length > 0)
						image = tab.ItemImages[ti];
						
					if (tab.ItemHelp==undefined)
						arr.push('<' + tc + ' onclick="' + onclick + onclickparameters + '" onmouseover="' + this.caller + '.onItemHover(this);" onmouseout="' + this.caller + '.onItemBlur(this);">' + (image!=null?'<img src=' + image + '>':'') + tab.Items[ti] + '</' + tc + '>');	
					else
						arr.push('<' + tc + ' onclick="' + onclick + onclickparameters + '" onmouseover="' + this.caller + '.onItemHover(this,\'' + group.Abbr + '\',' + ti + ',' + tab.Index + ',' + thisEditorIndex + ');" onmouseout="' + this.caller + '.onItemBlur(this,\'' + group.Name + '\',' + ti + ',' + tab.Index + ');">' + (image!=null?'<img src=' + image + '>':'') + tab.Items[ti] + '</' + tc + '>');	
					crow++;
					if (tab.Columns!=undefined)
					{ arr.push(tde + '</td>'); }
					
					if (crow == rows && tab.Columns==undefined)
					{ arr.push(tde + '</td>'); crow = 0; firsttab=false;}
					else if (tab.Columns!=undefined && crow==tab.Columns)
					{ arr.push('</tr>'); crow = 0; firsttab=false; }
				}
			}
			if (crow!=0 && tab.Columns==undefined)
				arr.push(tde + '</td>');
			else if (tab.Columns!=undefined)
			{	
				if (tab.Help==undefined || tab.Help.toUpperCase()!='TRUE')
				{
					var bcrow = false;
					while (crow != tab.Columns && crow > 0)
					{
						bcrow=true;
						arr.push('<td>&nbsp;</td>');
						crow++;
					}
					if (bcrow)
						arr.push('</tr>');
					arr.push('</table>');
				}
				arr.push('</div>');
				arr.push('</td>');
			}
		}
	}
}


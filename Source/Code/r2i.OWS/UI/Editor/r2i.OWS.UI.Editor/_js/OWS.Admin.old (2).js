/*General Object Manipulations*/
		Array.prototype.filter=function(f,me) 		{
			if (typeof me == 'undefined')
			{	me = false; }
			var that = [];
			if (this.length>0) {
				var min = me?this.length:0;
				var max = me?0:this.length;
				var inc = me?-1:1;
				for (var i = min; i!=max; i=i+inc) {
					var b = false;
					if (typeof f == 'function') {
						b = f(this[i]);
					}
					else {
						b = true;
						for (var name in f) {
							if ((this[i])[name]!=f[name])
							{ b = false; }
						}
					}
					if (b) {
						if (me) {
							this.splice(i,1);
						} else {
							that.push(this[i]);
						}
					}
				}
			}
			return (me?this:that);
		};
		Array.prototype.sortBy=function(type,name,direction) {
			if (typeof type!='undefined' && typeof type.push!='undefined' && type.length==3) {
				name = type[1];
				direction = type[2];
				type = type[0];
			}
			if (typeof direction=='undefined'||direction==null||direction.length==0) {return this};
			var f = null;
			var lpa = 'a[';
			var rpa = ']';
			var lpb = 'b[';
			var rpb = ']';
			if (name.indexOf('.')>0){
				lpa = 'GetChainObjectValue(a,';
				lpb = 'GetChainObjectValue(b,';
				rpa = ')';
				rpb = ')';
			}
			switch(type.toLowerCase()) {
				case 'number':
				case 'date':
					f=new Function("a","b","return "+lpa+"'"+name+"'"+rpa+"-"+lpb+"'"+name+"'"+rpb+";");
					break;
				case 'boolean':
					f=new Function("a","b","a="+lpa+"'"+name+"'"+rpa+"; b="+lpb+"'"+name+"'"+rpb+"; if (a==null) {a=false;} if (b==null) {b=false;} return (a?0:1)-(b?0:1);");
					break;
				default:
					f=new Function("a","b","return ("+lpa+"'"+name+"'"+rpa+".toLowerCase()<="+lpb+"'"+name+"'"+rpb+".toLowerCase()?("+lpa+"'"+name+"'"+rpa+".toLowerCase()=="+lpb+"'"+name+"'"+rpb+".toLowerCase()?0:-1):1)");
			}
			this.sort(f);
			var direction = direction.toLowerCase()[0];
			if (direction=='d') { this.reverse() }
			f=null;
			return this;
		}

		var hoverintent_config = ($.hoverintent = {
			sensitivity: 7,
			interval: 300
		});
		$.event.special.hoverintent = {
			setup: function() {
				$( this ).bind( "mouseover", jQuery.event.special.hoverintent.handler );
			},
			teardown: function() {
				$( this ).unbind( "mouseover", jQuery.event.special.hoverintent.handler );
			},
			handler: function( event ) {
				event.type = "hoverintent";
				var self = this,
					args = arguments,
					target = $( event.target ),
					cX, cY, pX, pY;
				
				function track( event ) {
					cX = event.pageX;
					cY = event.pageY;
				};
				pX = event.pageX;
				pY = event.pageY;
				function clear() {
					target
						.unbind( "mousemove", track )
						.unbind( "mouseout", arguments.callee );
					clearTimeout( timeout );
				}
				function handler() {
					if ( ( Math.abs( pX - cX ) + Math.abs( pY - cY ) ) < hoverintent_config.sensitivity ) {
						clear();
						jQuery.event.handle.apply( self, args );
					} else {
						pX = cX;
						pY = cY;
						timeout = setTimeout( handler, hoverintent_config.interval );
					}
				}
				var timeout = setTimeout( handler, hoverintent_config.interval );
				target.mousemove( track ).mouseout( clear );
				return true;
			}
		};	
/*---*/
/* Global Functions */
	
		function GetChainObjectValue(o,p) {
			try {
				var s = p.split('.');
				for (var i = 0; i < s.length; i++)
				{
					o=o[s[i]];
				}
				} catch (ex) {
				}	
			return o;
		}
		function FormatDouble(v) {
			return ows.Utilities.Format.Number(v,1,',','.','','','-','');
		}
		function FormatSingle(v) {
			return ows.Utilities.Format.Number(v*100,0,',','','','','-','');
		}	
/*---*/

if (typeof ows=='undefined' || ows==null) {ows={}};
if (typeof openwebstudio=='undefined' || openwebstudio==null) {openwebstudio=ows};
function OWSConfigurationClass(o) {
	this._ = null;
	this.load = function(o) {
		this._ = o;
	}
	this.add = function(a) {
		var act = ows.admin.plugins.items({Code:a});
		if (act.length>0) {
			act=act[0];
			ows.admin.console.write('Action "'+act.Code+'" added to the current configuration.');
			$('#tabs-1').append('<p>'+act.Code+'</p>');
		}
	}
	if (typeof o!='undefined') { this.load(o); }
}
function OWSAdminConfigurationsClass() {
	this._ = [];
	this._$= null;
	this.active = function() {		
		if (this.length>0) { 
			if (this._$==null || this._$ >= this.length) { this._$=0 }
			return this[this._$];
		}
		return new OWSConfigurationClass();
	}
	this.add = function(o) {
		this.push(OWSConfigurationClass(o))
	}
}
function OWSAdminPluginsClass() {
	this._ = [];
	this.add = function(o) {
		if (o.length>0) {
			for (var i = 0 ; i < o.length; i++) {
				if (typeof o[i].type=='undefined' || o[i].type==null) {o[i].type='Action'}
				this._.push(o[i]);
				ows.admin.console.write(o[i].type + ' plugin "'+o[i].Code+'" enabled.');
			}
		} else {
			if (typeof o.type=='undefined' || o.type==null) {o.type='Action'}
			this._.push(o);
		}	
	}
	this.actions = function() {
		return this._.filter({type:'Action'});
	}
	this.items = function(f) {
		if (typeof f=='undefined' || f == null) {
			return this._;
		}
		else {
			return this._.filter(f);
		}
	}
}
function OWSAdminConsole(c) {
	this.index = 1;
	this.length = 20;
	this.direction = 'down';
	this.target = {scroll:null,remove:null};
	$(c).append(document.createElement('div'));
	$(c).append(document.createElement('div'));
	if (this.direction == 'down') {
		this.target.scroll = $(c).children(':first');
		this.target.remove = $(c).children(':last');
	} else { 
		this.target.scroll = $(c).children(':last');
		this.target.remove = $(c).children(':first');
	}
	this.write = function(s) {
		var el=$(document.createElement('div'));
		el.addClass('consoleline');
		el.hide();
		el.html(this.index+'. '+s);
		if (this.direction=='down') {
			this.target.scroll.prepend(el);
			el.slideDown('slow');
		} else {
			this.target.scroll.append(el);
			el.slideUp('slow');
		}
		this.cleanup();
		this.index++;
	}
	this.cleanup = function() {
		if (this.target.scroll.children().length>this.length) {
			var d = null;
			if (this.direction=='down') {
				d = this.target.scroll.children(':last').detach();
				this.target.remove.prepend(d);
				d.slideUp('slow',function(){$(this).remove()});
			} else {
				d = this.target.children(':first').detach();
				this.target.remove.append(d);
				d.slideDown('slow',function(){$(this).remove()});
			}
		}
	}
}
function OWSAdminUtilityClass() {
	this.encodehtml = function(v) {
		v = v.replace(/</g,"&lt;");
		v = v.replace(/>/g,"&gt;");
		return v;
	}
	this.shorten = function(value,length) {
		//COMMENT
		 if (value==undefined)
			value = '';
		 if (length==undefined)
			length=75;
		if (value.length > length && length > 0)
		{
			value = value.substring(0,length-1);
		}
		//ENCODE HTML...
		value=this.encodehtml(value);

		return value;	
	}
	this.describe = function(i,t,s,c) {
		var t = t.split('-');
		t = t[t.length-1];
		if (typeof c=='undefined' || c==null || c.length==0) {
			return '<ins class="linenumber">'+i+'.</ins>'+'<strong>'+t+'</strong><i>'+this.shorten(s)+'</i>';
		} else {
			return '<ins class="linenumber">'+i+'.</ins>'+'<strong class="'+c+'">'+t+'</strong><i class="'+c+'">'+this.shorten(s)+'</i>';
		}
	}
}
function OWSAdminClass(d) {
	if (typeof d=='undefined') {d=false};
	openwebstudio.admin=this;
	if (d) {
		this.console = new OWSAdminConsole('#ideFooter');
	} else {
		this.console = {};
		this.console.write=function(v){};
	}

	this.configurations = new OWSAdminConfigurationsClass();
	this.plugins = new OWSAdminPluginsClass();
	this.utility = new OWSAdminUtilityClass();
	
	this.bind = function(target,clear,prepend,template,data,append) {
		this.console.write('Template('+template+') bound to target "'+target+'".');
		var t = $(target);
		if (clear) { t.empty(); };
		if (prepend!=null) { t.append(prepend); }; 
		$(template).tmpl(data).appendTo(t);
		if (typeof append != 'undefined' && append!=null) { t.append(append); }
		if ($(template).attr('onbind')!=null) {
			var tmp = $(template).get()[0];			
			if (typeof tmp.onbind!='function') {
				tmp.onbind = new Function("that","data",$(template).attr('onbind'))
			}
			tmp.onbind(t,data);
		}
		t=null;
	}
	
	this.load = function() {
		//this.bind("#ideTopLeft",true,false,"#tmp_ideActions",this.plugins.actions(),true);
	}
}


OWSAdminClass(true);
ows$cfg=openwebstudio.admin.configurations;
ows$plg=openwebstudio.admin.plugins;
ows$plg.add([
  {
   "Name" : "Comment",
   "Code" : "Action-Comment",
   "Description" : "Provide the ability for a developer to place Comments within their Action script",
   "onLoad" : "onActionLoad_Comment",
   "onSave" : "onActionSave_Comment",
   "onDelete" : "onActionDelete",
   "onPrint" : function(a) {return ows.admin.utility.describe(a.Index,a.ActionType,' - ' + a.Parameters.Value,'Comment')},
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
   "onPrint" : function(a) {
			var s = '';
			if (typeof a.Parameters.ConfigurationID!='undefined' && a.Parameters.ConfigurationID.length > 0 && a.Parameters.ConfigurationID!='00000000-0000-0000-0000-000000000000')
					s += 'Configuration:' + a.Parameters.Name + ' ';
			if (a.Parameters.Region.length > 0)
					s += 'Region:' + a.Parameters.Region + ' ';
			return ows.admin.utility.describe(a.Index,a.ActionType,s)
		},
   "Template" :
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    " <tr>" +
    "  <td class=\"SubHead\" width=\"151\">Configuration</td>" +
    "  <td class=\"Normal\" style=\"HEIGHT: 19px\"><input type=\"hidden\" name=frmActionGoto_Name id=frmActionGoto_Name /><div id=fi1>" +
     "<select name=frmActionGoto_ConfigurationID id=frmActionGoto_ConfigurationID onchange=\"onGotoConfigChanged();\">" +
      "<option value=\"\">Select a Configuration</option>" +
     "</select>&nbsp;<a href=\"#\" onclick=\"loadConfigurationChoices_Goto(sysGetSelect($('frmActionGoto_ConfigurationID')));return false;\">refresh</a></div></td>" +
    " </tr>" +
    " <tr>" +
    "  <td class=\"SubHead\" width=\"151\">Region</td>" +
    "  <td class=\"Normal\" style=\"HEIGHT: 19px\"><div id=fi1>" +
     "<select name=frmActionGoto_Region id=frmActionGoto_Region>" +
      "<option value=\"\">Select a Region</option>" +
     "</select>&nbsp;<a href=\"#\" onclick=\"configRegions=0;loadConfigurationRegions_Goto(sysGetSelect($('frmActionGoto_ConfigurationID')));return false;\">refresh</a></div></td>" +
    " </tr>" +
   "</table>"
  },  
  {
    "Name" : "Query",
   "Code" : "Action-Execute",
   "Description" : "Execute a query to gain the results and user them as a loop, or as a reference point",
   "onLoad" : "onActionLoad_Query",
   "onSave" : "onActionSave_Query",
   "onDelete" : "onActionDelete",
   "onPrint" : function(a) {return ows.admin.utility.describe(a.Index,(a.Parameters.IsProcess.toUpperCase()=='TRUE'?'(Process)&nbsp;&nbsp;':'') + a.ActionType + '[' + a.Parameters.Name + ']',' - ' + a.Parameters.Query,'Comment')},
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
   "onPrint" : function(a) {
		var s = '';
		if (a.Parameters.Type.length > 0 )
		{	if (a.Parameters.Value.length > 0)
				s='Awaiting Incoming Event with Type \'' + a.Parameters.Type + '\' and Value \'' + a.Parameters.Value + '\'';		
			else
				s='Awaiting Incoming Event with Type \'' + a.Parameters.Type + '\'';		
		}
		else
		{	if (a.Parameters.Value.length > 0)
				s='Awaiting Incoming Event with Value \'' + a.Parameters.Value + '\'';		
			else
				s='Awaiting Incoming Event with any Type and value';	
		}   
		return ows.admin.utility.describe(a.Index,a.ActionType,s,'Comment')
	},
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
   "onPrint" : function(a) {return ows.admin.utility.describe(a.Index,a.ActionType,'Search Integration')},
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
   "onPrint" : function(a) {return ows.admin.utility.describe(a.Index,a.ActionType,' for ' + a.Parameters.Value + ' ' + a.Parameters.Type + (a.Parameters.Value!=1?'s':''))},
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
   "onPrint" : function(a) {return ows.admin.utility.describe(a.Index,a.ActionType,' to ' + a.Parameters.Type + ' ' + a.Parameters.Link)},
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
   "onPrint" : function(a) {
		var s = '';
		s += a.Parameters.SourceType + ' source ';
		if (a.Parameters.Source!=null && a.Parameters.Source.length>0)
			s += '"' + a.Parameters.Source + '" ';
		if (a.Parameters.DestinationType!=null && a.Parameters.DestinationType.length>0)
			s += a.Parameters.DestinationType + ' destination ';
		if (a.Parameters.Destination!=null && a.Parameters.Destination.length>0)
			s += '"' + a.Parameters.Destination + '" ';		
		if (a.Parameters.TransformType!=null && a.Parameters.TransformType.length>0)
			s += a.Parameters.TransformType + ' with ' + a.Parameters.TransformType + ' transform';	   
		return ows.admin.utility.describe(a.Index,a.ActionType,s)
	},
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
     "</select></div>" +
     "</td>" +
    "</tr>" +	
    "<tr>" +
     "<td width=151><div id=fi20>Image Operation</div></td>" +
     "<td><div id=fi21>" +
     "<select name=frmImageTransformType id=\"frmImageTransformType\"  onchange=\"onfrmImageTransformType_Toggle()\">" +
	  "<option value=\"Size\">Resize</option>" +
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
   "</table>"
  },    
  {
   "Name" : "Filter",
   "Code" : "Action-Filter",
   "Description" : "Load Filter options for providing built in filtering for you query",
   "onLoad" : "onActionLoad_Filter",
   "onSave" : "onActionSave_Filter",
   "onDelete" : "onActionDelete",
   "onPrint" : function(a) {return ows.admin.utility.describe(a.Index,a.ActionType,' Options ')},
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
   "onPrint" : function(a) {return ows.admin.utility.describe(a.Index,a.ActionType,a.Parameters.Type + '.' + a.Parameters.Name + ' to ' + a.Parameters.Value)},
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
   "onPrint" : function(a) {return ows.admin.utility.describe(a.Index,a.ActionType,(a.Parameters.Format) + ' ' + a.Parameters.Subject + ' from ' + a.Parameters.From + ' to ' + a.Parameters.To)},
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
   "onPrint" : function(a) {return ows.admin.utility.describe(a.Index,a.ActionType,' From ' + a.Parameters.URL)},
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
   "onPrint" : function(a) {return ows.admin.utility.describe(a.Index,a.ActionType,a.Parameters.OutputType + (a.Parameters.Filename.length>0?' to file ' + a.Parameters.Filename:''))},
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
   "onPrint" : function(a) {return ows.admin.utility.describe(a.Index,a.ActionType,a.Parameters.Name + ' with a value of ' + a.Parameters.Value)},
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
   "onPrint" : function(a) {
		var s = '';
		var tname = '';
		switch(a.Parameters.Type)
		{
			case 'Query-Query':
				tname='Query';
				break;
			case 'Detail-Detail':
				tname='Detail';
			break;
			case 'Group-Header':
				if (a.Parameters.GroupIndex==null)
					a.Parameters.GroupIndex='';
				if (a.Parameters.GroupStatement==null)
					a.Parameters.GroupStatement='';	
				tname='Header [' + a.Parameters.GroupIndex + '] ' + (a.Parameters.GroupStatement.length > 0?' "' + a.Parameters.GroupStatement + '"':'');
			break;
			case 'Group-Footer':
				if (a.Parameters.GroupIndex==null)
					a.Parameters.GroupIndex='';
				if (a.Parameters.GroupStatement==null)
					a.Parameters.GroupStatement='';	
				tname='Footer [' + a.Parameters.GroupIndex + '] ' + (a.Parameters.GroupStatement.length > 0?' "' + a.Parameters.GroupStatement + '"':'');
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
		s+=' - ' + a.Parameters.Value;   
		return ows.admin.utility.describe(a.Index,tname + ' ' + a.ActionType,s)
	},
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
   "onPrint" : function(a) {return ows.admin.utility.describe(a.Index,a.ActionType,a.Parameters.QueryTarget + ' from ' + a.Parameters.VariableType + ' ' + a.Parameters.QuerySource)},
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
   "onPrint" : function(a) {
		if (a.Parameters.IsAdvanced.toUpperCase()=='TRUE')
		{
			s = a.Parameters.LeftCondition;
		}
		else
		{
			s = a.Parameters.LeftCondition + '&nbsp;' + a.Parameters.Operator + '&nbsp;' + a.Parameters.RightCondition;
		}   
		return ows.admin.utility.describe(a.Index,a.ActionType + ' Until ',s)
	},
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
   "onPrint" : function(a) {
   		if (a.Parameters.IsAdvanced.toUpperCase()=='TRUE')		{
			s = a.Parameters.LeftCondition;		}
		else	{
			s = a.Parameters.LeftCondition + '&nbsp;' + a.Parameters.Operator + '&nbsp;' + a.Parameters.RightCondition;		}   
		return ows.admin.utility.describe(a.Index,a.ActionType,s)
   },
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
   "onPrint" : function(a) {
   		if (a.Parameters.IsAdvanced.toUpperCase()=='TRUE')		{
			s = a.Parameters.LeftCondition;		}
		else	{
			s = a.Parameters.LeftCondition + '&nbsp;' + a.Parameters.Operator + '&nbsp;' + a.Parameters.RightCondition;		}   
		return ows.admin.utility.describe(a.Index,a.ActionType,s)
   },
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
   "onPrint" : function(a) {return ows.admin.utility.describe(a.Index,a.ActionType,"")},
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
   "onPrint" : function(a) {return ows.admin.utility.describe(a.Index,a.ActionType,' "' + a.Parameters.Value + '"')},
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
   "onPrint" : function(a) {return ows.admin.utility.describe(a.Index,a.ActionType,'['+a.Parameters.Condition+']')},
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
   "onPrint" : function(a) {
		var cdbg = '';
		var csea = '';
		var csex = '';
		var csei = ''
		if (typeof a.Parameters.skipDebug == 'undefined' || a.Parameters.skipDebug == null)
			a.Parameters.skipDebug = 'False';
		if (a.Parameters.skipDebug.toLowerCase()=='true')
			cdbg = '(Debug Disabled) ';
		if (typeof a.Parameters.includeSearch == 'undefined' || a.Parameters.includeSearch == null)
			a.Parameters.includeSearch = 'False';
		if (a.Parameters.includeSearch.toLowerCase()=='true')
			csea = '(Include In Search) ';
		if (typeof a.Parameters.includeExport == 'undefined' || a.Parameters.includeExport == null)
			a.Parameters.includeExport = 'False';
		if (a.Parameters.includeExport.toLowerCase()=='true')
			csex = '(Include In Export) ';
		if (typeof a.Parameters.includeImport == 'undefined' || a.Parameters.includeImport == null)
			a.Parameters.includeImport = 'False';
		if (a.Parameters.includeImport.toLowerCase()=='true')
			csei = '(Include In Import) ';   
		return ows.admin.utility.describe(a.Index,a.ActionType,' ' + cdbg + csea + csex + csei + '- ' + a.Parameters.Name + (a.Parameters.RenderType=='1'?'(Page Load Only)':'') + (a.Parameters.RenderType=='2'?'(AJAX Request Only)':''),'Region')
	},
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
  } ]  );
  
  ows.admin.load();
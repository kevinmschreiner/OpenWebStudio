			function OWSIdeToolbarClass(c,o) {
				this.selector = c;
				this.target = {parent:$(c),toolbar:null,tabs:null};
				this.options = o;
				this.index=1;
				this.addTab = function(op) {
					var li = $(document.createElement('li'));
					var dv = $(document.createElement('div'));
					li.append('<a href="'+this.selector+'-'+this.index+'">'+op.name+'</a>');
					dv.attr('id',this.selector.substr(1)+'-'+this.index);
					this.target.tabs.append(li);
					this.target.parent.append(dv);
					this.index++;
					return dv;
				}
				this.addSection = function(tab,op) {
					var sp = null;
					if (typeof op.custom=='undefined') {
						sp=$(document.createElement('span'));
						sp.addClass('buttons');
						tab.append(sp);
						return sp;
					} else {
						sp=$(document.createElement(op.tag));
						sp.html(op.custom);
						if (typeof op.css!='undefined') {
							sp.css(op.css);
						}
						tab.append(sp);
						if (typeof op.onload!='undefined') {
							op.onload(sp);
						}								
					}
				}
				this.addButton = function(section,op,tb,se) {
					if (typeof op.custom=='undefined') {
						var bt = $(document.createElement('button'));
						if (typeof op.onclick!='undefined')
						{
							bt.bind('click',op.onclick);
						} else {
							if (this.options.onclick!=null) {
								bt.attr('tab',tb.name);
								bt.attr('btn',op.name);
								bt.bind('click',this.options.onclick);
							}
						}
						bt.html(op.name);
						section.append(bt);
					} else {
						var bt = $(document.createElement('span'));
						bt.html(op.custom);
						section.append(bt);
					}
					if (typeof op.onload!='undefined') {
						op.onload(bt);
					}
					if (typeof op.icons!='undefined') {
						var options = {icons:{primary:"",secondary:""}};
						var s=op.icons.split(',');
						options.icons.primary = s[0];
						if (s.length>1) {
							options.icons.secondary = s[s.length-1];
						}
						bt.button(options);
					} else {bt.button()}
					return bt;
				}
				this.initialize = function() {
					var tb = $(document.createElement('div'));
					tb.addClass('toolbar');
					tb.attr('id',this.selector.substr(1)+'_tb');
					this.selector = this.selector+'_tb';
					this.target.parent.append(tb);
					this.target.parent = tb;
					this.target.tabs = $(document.createElement('ul'));
					this.target.parent.append(this.target.tabs);
					if (typeof this.options.onclick=='undefined') {
						this.options.onclick=null;
					}
					for (var i=0;i<this.options.tabs.length;i++)
					{
						var tab = this.addTab(this.options.tabs[i]);
						for (var ii=0; ii<this.options.tabs[i].sections.length; ii++)
						{
							var section = this.addSection(tab,this.options.tabs[i].sections[ii]);
							if (section!=null) {
								for (var iii=0; iii<this.options.tabs[i].sections[ii].buttons.length;iii++)
								{
									var button = this.addButton(section,this.options.tabs[i].sections[ii].buttons[iii],this.options.tabs[i],this.options.tabs[i].sections[ii]);
								}
							}
						}
					}
					
					$(this.target.parent).tabs();
					$(this.target.parent).parent().find('.buttons').buttonset();					
				}
				this.initialize();
			}		
			function owsIdeClass() {
			    window['ide'] = ide = this;
			    ide.callback_url = null;
				this.System = {};
				this.System.Variable=function(part){
					if (typeof part=='undefined') {
						return ide.System._Variable;
					} else {
						var r =
						{
							_:ide.System._Variable._,
							get:function(n,v){
								if (typeof n.push!='undefined') { for (var i = 0; i < n.length; i++) {n[i]=part+'.'+n[i]}}
								return ide.System._Variable.get(n,v);
							},
							set:function(n,v){
								if (typeof n.push!='undefined') { for (var i = 0; i < n.length; i++) {n[i]=part+'.'+n[i]}}
								ide.System._Variable.set(n,v);
								return this;
							}
						}
						return r;
					}
				}
				this.System._Variable={
					_:[],
					get:function(n,v){
							if (typeof n!='undefined' && typeof n.push!='undefined')
							{
								var r=[];
								for (var i = 0; i < n.length; i++) {
									if (typeof ide.System._Variable._[n[i]]=='undefined'||ide.System._Variable._[n[i]]==null) {
										if(typeof v!='undefined'){
											r[i]=v[i];
										} else {
											r[i]=null;
										}
									} else {
										r[i]=ide.System._Variable._[n[i]]
									}
								}
								return function(c){
									if (typeof c=='undefined') return r;
									if (c.length==r.length) {
										for (var i = 0; i < c.length; i++) {
											if (r[i]!=c[i]){return false;}
										}
										return true;
									} else {
										return false;
									}
								};
							}
							else {
								if (typeof v=='undefined') {v=null};
								if (typeof ide.System._Variable._[n]=='undefined'||ide.System._Variable._[n]==null) {return v } else {return ide.System._Variable._[n]}
							}
					},
					set:function(n,v){
						if (typeof n.push!='undefined' && n.length==v.length) {
							for (var i = 0; i < n.length;i++) {
								ide.System._Variable._[n[i]]=v[i];
							}
						} else
						{
							ide.System._Variable._[n]=v;
						}
						return this;
					}
				}				
				this.System.load = function() {
					//init any system objects
				}
				this.System.error = function(ex,title) {
					var msg = '';
					if (typeof ex=='string' || typeof ex.Error=='string') {
						if (typeof ex=='string') {
							msg = ex;
						}
						else {
							msg = ex.Error;
						}
					}
					else {
						var sb = [];
						for (var name in ex.Error) {
							sb.push(ex.Error[name]);
						}
						msg = sb.join('\n');
					}
					if (typeof title!='undefined') {
						ide.System.message(msg,title);
					} else {
						ide.System.message(msg);
					}
				}
				this.System.confirm = function(msg,title,buttons) {
					ide.System.message(msg,title,buttons);
				}
				this.System.message = function(msg,title,buttons) {
					jQuery( "#dlgError p").html(msg);
					if (typeof title=='undefined') {
						title = jQuery( "#dlgError").attr('default');
					}
					if (typeof buttons=='undefined') {
						jQuery( "#dlgError" ).dialog({ modal: true, buttons:null, title:title });		
					}
					else
					{
						jQuery( "#dlgError" ).dialog({ modal: true, buttons:buttons, title:title });		
					}
				}
				this.Community={}
				this.Community.Authentic={
					onEvent:null,
					check:function(next){
						if (typeof next!='undefined') {
							ide.Community.Authentic.onEvent=next;
						}
						ows.Services.request({
							url:'http://openwebstudio.com/Services/Authenticate.aspx',
							callback:function(data){
								if (data.length>0) {
									//we are authenticated
									if (ide.Community.Authentic.onEvent!=null) {
										ide.Community.Authentic.onEvent();
									}
									ide.Community.Authentic.onEvent=null;
								} else {
									//prompt to authentication
									ide.System.message('<table><tr><td>Username</td><td><input value=""/></td></tr><tr><td>Password</td><td><input type="password" value=""/></td></tr></table>','Authenticate',{'Login using these credentials':function(){ide.Community.Authentic.passthrough(jQuery(this).parent().find('input'));jQuery(this).dialog('close')},'Cancel':function(){ide.Community.Authentic.onEvent=null;jQuery(this).dialog('close')}});
								}
							}
						});
					},
					passthrough:function(u,p){
						//ows.Service.request
						if (typeof p=='undefined' && u.length==2) {
							p = u[1].value;
							u = u[0].value;
						}
						ows.Services.request({
							url:'http://openwebstudio.com/Services/Authenticate.aspx',
							params:[
								{type:'post',name:'u',value:u},
								{type:'post',name:'p',value:p},
								{name:'respond',value:escape(ide.callback_url)}
							],
							callback:function(data){
								if (data.length>0) {
									//we are authenticated
									if (ide.Community.Authentic.onEvent!=null) {
										ide.Community.Authentic.onEvent();
									}
									ide.Community.Authentic.onEvent=null;
								} else {
									//prompt to authentication
									ide.Community.Authentic.check(ide.Community.Authentic.onEvent);
								}
							}
						});						
					}
				}
				this.Community.Chat={
					lastId:null,
					length:0,
					add:function(){
						//Check Authentication first...
						ide.Community.Authentic.check(ide.Community.Chat._add);
						//if authenticated - do this...
					},
					_add:function(){
						ide.System.message('<textarea style="width:270px;height:100px;" maxlength="500"></textarea>','Add a Comment',{'Send':function(){ide.Community.Chat.post(jQuery(this).parent().find('textarea'));jQuery(this).dialog('close')},'Cancel':function(){jQuery(this).dialog('close')}});
					},
					post:function(o){
						var msg = o.val();
						ows.Services.request({
							url:'http://openwebstudio.com/Services/Chat.aspx',
							params:[{name:'ModuleId',value:'436'},
									{name:'Action',value:'Add'},
									{name:'frmBody',type:'post',value:msg},
									{name:'respond',value:escape(ide.callback_url)}
									],
							callback:function(data){
								ide.Community.Chat.ping();
							}});						
					},
					ping:function(){
						ows.Services.request({
							url:'http://openwebstudio.com/Services/Chat.aspx',
							params:[{name:'ModuleId',value:'436'},{name:'LastPostID',value:ide.Community.Chat.lastId}],
							callback:function(data){
								if (data.length>0) {
									ide.Community.Chat.lastId=data[data.length-1].id;
								}
								ide.Community.Chat._list(data,true)
							}})},
					list:function(){
						ows.Services.request({
							url:'http://openwebstudio.com/Services/Chat.aspx',
							params:[{name:'ModuleId',value:'436'}],
							callback:function(data){
								if (data.length>0) {
									ide.Community.Chat.lastId=data[data.length-1].id;
								}
								ide.Community.Chat._list(data,false)
							}})},
					_list:function(data,append){
						ide.Community.Chat.length=ide.Community.Chat.length+data.length;
						if (append && ide.Community.Chat.length>10) {
								var r = ide.Community.Chat.length-10;
								$("#dvCommunityChat").children('div:lt('+r+')').remove();
								ide.Community.Chat.length=ide.Community.Chat.length-r;
						}
						$( "#tmp_ideCommunity_Chat_" ).tmpl( data ).appendTo( "#dvCommunityChat" );
						$("#dvCommunityChat .invisible").removeClass('invisible').fadeIn('slow');
						if (data.length>0) {
							$("#ideCommunity").animate({ scrollTop: $("#ideCommunity").attr("scrollHeight") }, 3000);
						}
						window.setTimeout(function(){ide.Community.Chat.ping()},10000);
					}
				}
				this.toggler = {horizontal:{open:'<span class="ui-layout-toggler-icon-horizontal ui-icon ui-icon-triangle-1-n">&nbsp;</span>',closed:'<span class="ui-layout-toggler-icon-horizontal ui-icon ui-icon-triangle-1-s">&nbsp;</span>'},
								vertical:{open:'<span class="ui-layout-toggler-icon-vertical ui-icon ui-icon-triangle-1-w">&nbsp;</span>',closed:'<span class="ui-layout-toggler-icon-vertical ui-icon ui-icon-triangle-1-e">&nbsp;</span>'}};
								this.click = function (t, b) {
								    switch (t.toLowerCase()) {
								        case 'file':
								            switch (b.toLowerCase()) {
								                case 'open':
								                    //ows.Manager.open('33114253-8499-3fba-d2ea-70c16eacdb21');
								                    ows.Manager.open('12996742-4455-434c-650e-50eaf142fc58');
								                    break;
								                case 'new':
								                    ows.Manager.open();
								                    break;
								                case 'save':
								                    ows.Manager.save();
								                    break;
								                case 'save as...':
								                    break;
								                case 'close':
								                    ide.System.confirm('Any unsaved data will be lost, are you sure you want to close this configuration?', 'Close Configuration', { 'Yes, close this configuration': function () { ide.close(false); jQuery(this).dialog('close') }, 'Cancel': function () { jQuery(this).dialog('close') } });
								                    break;
								                case 'close all':
								                    ide.System.confirm('Any unsaved data will be lost, are you sure you want to close all these configurations?', 'Close All Configurations', { 'Yes, close all configurations': function () { ide.close(true); jQuery(this).dialog('close') }, 'Cancel': function () { jQuery(this).dialog('close') } });
								                    break;
								                case 'import':
								                    break;
								                case 'export':
								                    break;
								                case 'about':
								                    this.about.show(function () { ide.about.hide() }, 400)
								                    break;
								            }
								            break;
								        case 'edit':
								            switch (b.toLowerCase()) {
								                case 'cut':
								                    break;
								                case 'copy':
								                    break;
								                case 'paste':
								                    break;
								                case 'expand':
								                    ows.Manager.active().config().expand(false);
								                    break;
								                case 'collapse':
								                    ows.Manager.active().config().contract(false);
								                    break;
								                case 'expand all':
								                    ows.Manager.active().config().expand(true);
								                    break;
								                case 'collapse all':
								                    ows.Manager.active().config().contract(true)
								                    break;
								            }
								            break;
								        case 'repository':
								            break;
								        case 'events':
								            break;
								        case 'community':
								            this.panels.main.layout.open('east');
								            //this.panels.community.element.html('<iframe src="http://www.openwebstudio.com" width="100%" height="100%"></iframe>');
								            switch (b.toLowerCase()) {
								                case 'chat':
								                    this.panels.community.element.html('<div id="dvCommunityChat"></div><div id="dvCommentBox"></div>');

								                    var bx = $('#dvCommentBox');
								                    var bt = $(document.createElement('button'));
								                    bt.html('Add a Comment');
								                    bt.css({ width: '100%' });
								                    bt.bind('click', ide.Community.Chat.add);
								                    bx.append(bt);
								                    bt.button({ icons: { primary: 'ui-icon-comment', secondary: 'ui-icon-triangle-1-n'} });

								                    this.Community.Chat.list('');
								                    break;
								            }
								            break;
								    }
								}
				this.close=function(closeAll) { if(closeAll){ows.Manager.closeAll();}else{ows.Manager.close();} }
				this.toolbar= 
					{
						tabs:[
							{name:"File",
							 sections:[
								{buttons:[{id:"Open",name:"Open",icons:"ui-icon-folder-open"},
										  {id:"New",name:"New",icons:"ui-icon-plus"}]},
								{buttons:[{id:"Save",name:"Save",icons:"ui-icon-disk"},{id:"SaveAs",name:"Save As..."}]},
								{buttons:[{id:"Close",name:"Close",icons:"ui-icon-circle-close"},{id:"CloseAll",name:"Close All"}]},
								{buttons:[{id:"Import",name:"Import",icons:"ui-icon-arrowthickstop-1-w"},{id:"Export",name:"Export",icons:"ui-icon-arrowthickstop-1-e"}]},
								{buttons:[{id:"About",name:"About",icons:"ui-icon-info"}]}
							]},
							{name:"Edit",
							 sections:[
								{buttons:[{id:"Cut",name:"Cut",icons:"ui-icon-scissors"},{id:"Copy",name:"Copy",icons:"ui-icon-clipboard"},{id:"Paste",name:"Paste",icons:"ui-icon-copy"}]},
								{buttons:[{id:"Expand",name:"Expand",icons:"ui-icon-circle-triangle-s"},{id:"Collapse",name:"Collapse",icons:"ui-icon-circle-triangle-n"},{id:"ExpandAll",name:"Expand All",icons:"ui-icon-circle-arrow-s"},{id:"CollapseAll",name:"Collapse All",icons:"ui-icon-circle-arrow-n"}]},
								{tag:'div',css:{width:'143px',display:'inline',position:'absolute','margin-top':'3px'},custom:'',onload:function(that){that.themeswitcher({imgpath:"_js/jQuery/images/",initialText:'Theme'});}}
								]},
							{name:"Repository",
							 sections:[
								{buttons:[{id:"View",name:"View",icons:"ui-icon-search"}]},
								{buttons:[{id:"Label",name:"Label",icons:"ui-icon-flag"},{id:"Rollback",name:"Rollback",icons:"ui-icon-arrowreturnthick-1-s"},{id:"Revert",name:"Revert",icons:"ui-icon-arrowreturn-1-s"}]},
								{buttons:[{id:"Clear",name:"Clear",icons:"ui-icon-transfer-e-w"},{id:"Delete",name:"Delete",icons:"ui-icon-trash"}]}]},
							{name:"Events",
							 sections:[
								{buttons:[{id:"View",name:"View",icons:"ui-icon-search"}]},
								{buttons:[{id:"Filter",name:"Filter",icons:"ui-icon-circle-zoomout"}]},
								{buttons:[{id:"Clear",name:"Clear",icons:"ui-icon-transfer-e-w"},{id:"ClearAll",name:"Clear All",icons:"ui-icon-transferthick-e-w"},{id:"Delete",name:"Delete",icons:"ui-icon-trash"}]}]},
							{name:"Community",
							 sections:[
								{buttons:[{id:"Chat",name:"Chat",icons:"ui-icon-comment"}]},
								{buttons:[{id:"Help",name:"Help",icons:"ui-icon-help"},{id:"Tickets",name:"Tickets",icons:"ui-icon-tag"}]},
								{buttons:[{id:"Questions",name:"Questions",icons:"ui-icon-alert"},{id:"Forum",name:"Forum",icons:"ui-icon-person"},{id:"Wiki",name:"Wiki",icons:"ui-icon-script"},{id:"Blog",name:"Blog",icons:"ui-icon-note"}]}]}								
								
						],
						onclick:function(){var b = $(this);ide.click(b.attr('tab'),b.attr('btn'))}
					}

				this.panels=
				{
					footer:	{target:'#ideFooter',		dock:'bottom'},
					actions:{target:'#ideTopLeft',		dock:'left'},
					content:{target:'#ideTopRight',		dock:'fill'},
					community: {target:'#ideCommunity', dock:'right'},
					plugins:{target:'#ideBottomLeft',	dock:'left'},
					editor: {target:'#ideBottomRight',	dock:'fill'},						
					main:	{target:'#ideMain',			
								layout:{
									top:{paneSelector:"#ideHeadline",size:70,closable:false,resizable:false,slidable:false,toggler:ide.toggler.horizontal,spacing_open:1,togglerLength_open:-1,toggler_Length_closed:-1},
									center:{paneSelector:"#ideCenter"},
									bottom:{paneSelector:"#ideFooter",initClosed:true,toggler:ide.toggler.horizontal},
									right:{paneSelector:"#ideCommunity",initClosed:true,toggler:ide.toggler.vertical}
							}},
					body: 	{target:'#ideCenter',
								layout:{
									center:{paneSelector:"#ideTop"},
									bottom:{paneSelector:"#ideBottom",initClosed:true,size:400,minSize:150,toggler:ide.toggler.horizontal}
							}},
					bodyT:{target:"#ideTop",
								layout:{
									left:{paneSelector:"#ideTopLeft",resizable:false,slidable:false,size:160,minSize:160,toggler:ide.toggler.vertical},
									center:{paneSelector:"#ideTopRight"}
							}},
					bodyE:{target:"#ideBottom",
								layout:{
									left:{paneSelector:"#ideBottomLeft",resizable:false,slidable:false,size:160,minSize:160,toggler:ide.toggler.vertical},
									center:{paneSelector:"#ideBottomRight"}
							}}							
				}
				this._init_docking = function() {
					for (var name in ide.panels) {
						ide.panels[name].element=$(ide.panels[name].target);
						if (typeof ide.panels[name].dock!='undefined' && ide.panels[name].dock!=null) {
							//add the initial docking classes - these must be assigned to all sub panels prior to being consumed by the parent panel
							var cname = 'ui-layout-';
							switch(ide.panels[name].dock.toLowerCase()) {
								case 'top':
									cname+='north';
									break;
								case 'bottom':
									cname+='south';
									break;
								case 'left':
									cname+='west';
									break;
								case 'right':
									cname+='east';
									break;
								default:
									cname+='center';
									break;
							}
							ide.panels[name].element.addClass(cname);
						}
					}				
				}
				this._dup_fk_layout = function(s,t,p){
					for (var k in s) { t[p+k]=s[k]};
				}
				this._init_layouts = function() {
					for (var name in ide.panels) {
						if (typeof ide.panels[name].layout!='undefined') {
							//initialize the layout
							//rename...
							if (typeof ide.panels[name].layout.top!='undefined') 	{ ide.panels[name].layout.north = 	ide.panels[name].layout.top }
							if (typeof ide.panels[name].layout.bottom!='undefined') { ide.panels[name].layout.south = 	ide.panels[name].layout.bottom }
							if (typeof ide.panels[name].layout.left!='undefined') 	{ ide.panels[name].layout.west = 	ide.panels[name].layout.left }
							if (typeof ide.panels[name].layout.right!='undefined') 	{ ide.panels[name].layout.east = 	ide.panels[name].layout.right }
							
							if (typeof ide.panels[name].layout.north!='undefined' && typeof ide.panels[name].layout.north.toggler!='undefined') {
								ide.panels[name].layout.north.togglerContent_closed=ide.panels[name].layout.north.toggler.closed;
								ide.panels[name].layout.north.togglerContent_open=ide.panels[name].layout.north.toggler.open;
								ide.panels[name].layout.north.togglerLength_open=20;
								ide.panels[name].layout.north.togglerLength_closed=20;
							}
							if (typeof ide.panels[name].layout.south!='undefined' && typeof ide.panels[name].layout.south.toggler!='undefined') {
								ide.panels[name].layout.south.togglerContent_closed=ide.panels[name].layout.south.toggler.open;
								ide.panels[name].layout.south.togglerContent_open=ide.panels[name].layout.south.toggler.closed;
								ide.panels[name].layout.south.togglerLength_open=20;
								ide.panels[name].layout.south.togglerLength_closed=20;
							}
							if (typeof ide.panels[name].layout.west!='undefined' && typeof ide.panels[name].layout.west.toggler!='undefined') {
								ide.panels[name].layout.west.togglerContent_closed=ide.panels[name].layout.west.toggler.closed;
								ide.panels[name].layout.west.togglerContent_open=ide.panels[name].layout.west.toggler.open;
								ide.panels[name].layout.west.togglerLength_open=20;
								ide.panels[name].layout.west.togglerLength_closed=20;
								
							}
							if (typeof ide.panels[name].layout.east!='undefined' && typeof ide.panels[name].layout.east.toggler!='undefined') {
								ide.panels[name].layout.east.togglerContent_closed=ide.panels[name].layout.east.toggler.open;
								ide.panels[name].layout.east.togglerContent_open=ide.panels[name].layout.east.toggler.closed;
								ide.panels[name].layout.east.togglerLength_open=20;
								ide.panels[name].layout.east.togglerLength_closed=20;
							}
							if (typeof ide.panels[name].layout.north!='undefined'){this._dup_fk_layout(ide.panels[name].layout.north,ide.panels[name].layout,'north__')}
							if (typeof ide.panels[name].layout.south!='undefined'){this._dup_fk_layout(ide.panels[name].layout.south,ide.panels[name].layout,'south__')}
							if (typeof ide.panels[name].layout.east!='undefined'){this._dup_fk_layout(ide.panels[name].layout.east,ide.panels[name].layout,'east__')}
							if (typeof ide.panels[name].layout.west!='undefined'){this._dup_fk_layout(ide.panels[name].layout.west,ide.panels[name].layout,'west__')}
							//if (typeof ide.panels[name].dock!='undefined') {
								ide.panels[name].layout=ide.panels[name].element.layout(ide.panels[name].layout);
							//}
						}
					}				
				}
				this.about = {
					show:function(next,delay){
						if (typeof delay=='undefined') {
							$('#dvAbout').fadeIn(500,next);
						} else {
							$('#dvAbout').fadeIn(500,function(){window.setTimeout(next,delay)});
						}
					},
					hide:function(next,delay){
						if (typeof delay=='undefined') {
							$('#dvAbout').fadeOut(500,next);
						} else {
							$('#dvAbout').fadeOut(500,function(){window.setTimeout(next,delay)});
						}					
					}
				}
				this._init_toolbar = function() {
					this.toolbar = new OWSIdeToolbarClass('#ideHeadline',this.toolbar);
				}
				this.actions = {
				    toggled: null,
				    toggle: function (o) {
				        var c = true;
				        if (this.actions.toggled != null) {
				            this.actions.toggled.parent().children('.help').slideUp('slow');
				            if ($(o).attr('toggled') == 'true') {
				                c = false;
				            }
				            this.actions.toggled.attr('toggled', false);
				            this.actions.toggled = null;
				        }
				        if (c) {
				            this.actions.toggled = $(o);
				            $(o).attr('toggled', true);
				            this.actions.toggled.parent().children('.help').slideDown('slow');
				        }
				    },
				    add: function (a) {
				        ows.Manager.addAction(ows.admin.plugins.items({ Code: a }), null, ows.Manager.active());
				        //ows.admin.configurations.active().add($(this).attr('action'));return false;
				        /*
				        var act = ows.admin.plugins.items({Code:a});
				        if (act.length>0) {
				        act=act[0];
				        ows.admin.console.write('Action "'+act.Code+'" added to the current configuration.');
				        $('#tabs-1').append('<p>'+act.Code+'</p>');
				        }
				        */
				    },
				    edit: function (o, e, c) {
				        ows.Manager.editAction(o, e, c);
				    },
				    save: function (id) {
				        ows.Manager.saveAction(id);
				    },
				    cancel: function (id) {
				        ows.Manager.cancelAction(id);
				    }
				}
				this.tokens = {
					toggled:null,
					toggle:function(o) {
						var c=true;
						if (this.tokens.toggled!=null) {
							this.tokens.toggled.parent().children('.help').slideUp('slow');
							if ($(o).attr('toggled')=='true') {
								c=false;
							} 
							this.tokens.toggled.attr('toggled',false);
							this.tokens.toggled=null;
						}
						if (c) {
							this.tokens.toggled = $(o);
							$(o).attr('toggled',true);
							this.tokens.toggled.parent().children('.help').slideDown('slow');
						}
					},
					add:function(a){
						var o = ows.Manager.activeElement();
						if (typeof o!='undefined' && o!=null) {
							var act = ows.admin.plugins.items({Code:a});
							if (act.length>0) {
								act=act[0];
								$(ows.Manager.activeElement()).texty.insert(ows.Manager.activeElement(),act.Template);
							}
						}
						/*
						var act = ows.admin.plugins.items({Code:a});
						if (act.length>0) {
							act=act[0];
							ows.admin.console.write('Token "'+act.Code+'" added to the current configuration.');
							$('#tabs-1').append('<p>'+act.Code+'</p>');
						}
						*/
					},
					edit:function(o,e,c){
						ows.Manager.editAction(o,e,c);
					},
					save:function(id) {
						ows.Manager.saveAction(id);
					},
					cancel:function(id) {
						ows.Manager.cancelAction(id);
					}
				}
				this.initialize = function() {
					this._init_toolbar();
					this._init_docking();
					this._init_layouts();
				}
			}
			owsIdeClass();
			function loadAdmin() {
				//window.setTimeout(function(){$('#dvAbout').fadeOut(500,_loadAdmin);},400)
				ide.about.show(function(){ide.about.hide(_loadAdmin)},400);
			}
			function _loadAdmin() {
				ide.initialize();
				ide.panels.body.layout.hide('south');				
			}
			$(document).ready(function(){
				loadAdmin();
			});
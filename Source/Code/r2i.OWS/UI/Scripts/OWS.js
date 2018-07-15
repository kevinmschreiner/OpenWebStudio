//<![CDATA[
if (typeof jQuery!='undefined')
	var $jq=jQuery;

if (typeof ows == 'undefined' || ows==null)
{
function OWSParameters() {
this.ConfigurationID = 'id';
this.Source = 'lxSrc';
this.ResourceKey = 'key';
this.ResourceFile = 'file';
this.PageId = 'p';
this.ModuleId = 'm';
this.Download = 'download';
this.CPageId = 'cp';
this.CModuleId = 'cm';
this.Type = 'type';
this.Sort = 'sort';
this.SortState = 'sortstate';
this.Action = 'lxA';
this.Actions = 'xActions';
this.UpgradeModule = 'uM';
this.UpgradePage = 'uT';
this.Filename = 'filename';
this.CheckIndex = 'lxIx';
this.CheckModuleID = 'lxM';
this.CheckGroup = 'lxG';
this.CheckItem = 'lxI';
this.CheckValue = 'lxV';
this.CheckRemove = 'lxR';
this.Name = 'lxN';
this.RecordsPerPage = 'lxC';
this.PageNumber = 'lxP';
}
var _OWS_ = new OWSParameters();

function OWSControl(id,ipage,rpp,requestparameters,wurl,onload,forcepager,historypager,statusMessages)
{
    this.id = id+''; //added +'' to cast as a string.
    this.page = ipage;
    this.length = 0;
    this.tbl = false;
    this.sort = null;
    this.sortstate = null;
    this.xml = false;
    this.data = false;
    this.pgs = false;
    this.status = false;
    /*
    {
	                    "Render":"Rendering Data...",
	                    "Fetch":"Fetching Data...",
	                    "Error":{"General":"Data Failure.",
	                             "Data":"There was a problem retrieving the XML data:\n",
	                             "Ajax":"AJAX: Asynchronous XML with Javascript is not supported by your browser."},
	                    "Header":'<div style="text-align:center;">',
	                    "Footer":"</div>"
	};
	*/
    if (typeof statusMessages!='undefined')
        this.StatusMessages = statusMessages;
    else
        this.StatusMessages = null;    
	this.forcepager=forcepager;
	if (typeof historypager!='undefined' && historypager!=null && historypager.length > 0)
	{	this.historypagerDisplay = historypager;
		this.historypager = true; }
	else
	{   this.historypagerDisplay = 'Page'+id;
	    this.historypager = false; }
	    
    this.request = requestparameters;
    this.url = wurl;
    this.hide = false;
	if (this.id!=null)
	{	
		if (this.forcepager)
			this.pgsn = this.forcepager;
		else
			this.pgsn = 'lxP' + this.id;
	    this.statusn = 'lxS' + this.id;
	}
	else
	{
		this.pgsn = '';
		this.statusn = '';
	}
    this.rpp = rpp;
    this.onLoad = onload;
}
/*
function OWSJState()
{
    this.properties = new Object();
    this.buffer = new Array();
    this.buffering = true;
    this.source = null;
    this.property = function(name,value,save)
    {
        if (typeof value == 'undefined')
        {
            if (!this.buffering)
            {
                return this.properties[name];
            }
            else
            {
                alert('unable to retrieve state properties until the page has loaded');
            }
        }
        else {
            if (!this.buffering)
            {
                this.properties[name]=value
            }
            else
            {
                this.buffer.push( {"name":name,"value":value} );
            }
        }
        if (typeof save=='undefined'||save==null||save==true)
            this.save();
        return value;
    }
    this.load = function(src)
    {
       if (src!=null)
        this.source=src;
       if (this.source!=null&&typeof this.source.value!='undefined'&&this.source.value.lengt>0)
       {
        this.properties=eval('('+this.source.value+')');
       }
       this.buffering = false;
       while (this.buffer.length>0)
       {
            var item = this.buffer.pop();
            this.property(item.name,item.value,false);
       }
       this.save();
    }
    this.save = function()
    {
        if (!this.buffering)
        {
            var out = new Array();
            for (var name in this.properties)
            {
                out.push('"'+escape(name)+'":"'+escape((this.properties[name]==null)?'':this.properties[name])+'"');
            }   
            if (out.length>0)
                this.source.value = '{'+out.join(',')+'}';
            else
                this.source.value ='';
        }
    }
}
*/
function OWS()
{
    /*
    this.pagestate = new OWSJState();
    try
    {
        if (document.getElementById('__OWS_PAGESTATE__')==null)
        {
            var elem = document.createElement('input');
            elem.setAttribute('type','hidden');
            elem.setAttribute('name','__OWS_PAGESTATE__');
            elem.setAttribute('id','__OWS_PAGESTATE__');
            if (typeof document.forms!='undefined'&&document.forms.length>0)
                document.forms[0].appendChild(elem);
            else
                document.getElementsByTagName('html')[0].appendChild(elem);
            this.pagestate.load(elem);
        }
        else
            this.pagestate.load(document.getElementById('__OWS_PAGESTATE__'));
    } catch(ex) {}
    this.property = function(name,value)
    {
        return this.pagestate.property(name,value);
    }
    */
    
    this.items = new Array();
    this.urlbase = '';
	this.DETECT = navigator.userAgent.toLowerCase();
	this.SPINWAIT = 250;
	this.STARTDELAY = 1000;
	
	this.LOCALE_PAGEFIRST	= 'First';
	this.LOCALE_PAGELAST	= 'Last';
	this.LOCALE_PAGENEXT	= 'Next';
	this.LOCALE_PAGEBACK	= 'Back';
	/*
	this.LOCALE_STATUSH     = '<div style="text-align:center;">';
	this.LOCALE_STATUS1		= 'Rendering Data...';
	this.LOCALE_STATUS2		= 'Fetching Data...';
	this.LOCALE_STATUS3		= 'Data Failure.';
	this.LOCALE_STATUS4		= 'AJAX: Asynchronous XML with Javascript is not supported by your browser.';
	this.LOCALE_STATUS5		= 'There was a problem retrieving the XML data:\n';
	this.LOCALE_STATUSF     = '</div>';
	*/
	this.LOCALE_STATUS = {
	                    "Render":"Rendering Data...",
	                    "Fetch":"Fetching Data...",
	                    "Error":{"General":"Data Failure.",
	                             "Data":"There was a problem retrieving the XML data:\n",
	                             "Ajax":"AJAX: Asynchronous XML with Javascript is not supported by your browser."},
	                    "Header":'<div style="text-align:center;">',
	                    "Footer":"</div>"
	                     };
       
	this.Create=function Create(id,page,rpp,request,wurl,url,enable,refresh,historypager,targetobjectid,forcepager,qparams,onload,StatusMessages){
	    this.urlbase = url;
	    if (typeof id!='undefined'&&id!=null) //added to force id to a string.
	        id = id+'';
		if (page<=0)
		    page=0;
        var skip=false;
        var loadpages=null;
		if (typeof onload=='undefined')
			onload = null;  
		if (typeof StatusMessages=='undefined')
		    StatusMessages = this.LOCALE_STATUS;      
        if (typeof(enable) != "boolean" && !isNaN(enable))
        {
            loadpages = enable;
            skip=true;
        }
        else
        {
            skip=enable;
        }
	    this.items['o'+id]=new OWSControl(id,page,rpp,request,wurl,onload,forcepager,historypager,StatusMessages);
	    var cPage = this.PageGet(id);
		if (cPage!=null)
			page=cPage;
		if(skip&&cPage!=null)
			skip=false;		
		this.items['o'+id].page = page;
		if (typeof qparams==undefined||qparams==null)
			qparams = '';
		this.Pagers['o'+id]=false;
		this.Init(id,this.items['o'+id].page,skip,qparams,targetobjectid);
		if (refresh > -1)
		{
		    var starget = '';
			if (typeof qparams!= 'undefined' && qparams!=null && qparams.length>0)
			    starget='\'' + qparams + '\'';
			else
			    starget='\'\'';
			if (typeof targetobjectid!= 'undefined' && targetobjectid!=null && targetobjectid.length>0)
				starget=starget+',\'' + targetobjectid + '\'';
			window.setInterval("ows.Fetch(" + id + ",ows.items['o" + id + "'].page,"+starget+");",refresh);
		}
		if (loadpages!=null&&loadpages>0)
        {
            this.items['o'+id].length=loadpages;
            this._Page(id);
        }    
	}
	this._Page = function(id)
	{
	    if (typeof id!='undefined'&&id!=null) //added to force id to a string.
	        id = id+'';	
		window.setTimeout('ows.Page(\'' + id + '\');', this.SPINWAIT) 
	}
	this.PageGet = function(id)
	{
	    if (typeof id!='undefined'&&id!=null) //added to force id to a string.
	        id = id+'';	
	    var historyPageText = this.items['o'+id].historypagerDisplay;
		var strLocation = document.location+'';
		var lsplit = strLocation.split('#'+historyPageText+':');
		if (lsplit.length > 1)
		{
			var nsplit = lsplit[lsplit.length-1].split('#');
			return nsplit[0];
		}
		return null;
	}
	this.PageSet = function(id,page)
	{
	    if (typeof id!='undefined'&&id!=null) //added to force id to a string.
	        id = id+'';	
	    var historyPageText = this.items['o'+id].historypagerDisplay;
		var strLocation = document.location+'';
		var value = '#'+historyPageText+':'+page;
		var lsplit = strLocation.split('#'+historyPageText+':');
		var locLeft = lsplit[0];
		if (lsplit.length > 1)
		{
			var nsplit = lsplit[lsplit.length-1].split('#');
			nsplit[0]='#'+historyPageText+':'+page;
			locLeft+=nsplit.join('#');
		}
		else
		{
			locLeft+=value;
		}
		return locLeft;
	}	
	this.BrowserType = function BrowserType(string)
	{
		return this.DETECT.indexOf(string) + 1;
	}
	this.Init = function Init(TM,page,skip,qparams,targetobjectid)
	{
	    if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	        TM = TM+'';	
		if (qparams==undefined || qparams==null && qparams.length == 0)
			qparams = '';
		if (targetobjectid==undefined || targetobjectid==null && targetobjectid.length == 0)
			targetobjectid = '';
			
		if (!skip && this.BrowserType('msie')  && document.readyState != 'complete')
		{ 
			var exparams = '';
			if (qparams.length > 0)
				exparams = ",'" + qparams + "'";
			else
				exparams = ",null";
			
			if (targetobjectid.length>0)
				exparams = exparams + ",'" + targetobjectid + "'";
			else
				exparams = exparams + ",null";
			
			if (page)
			{
				window.setTimeout('ows.Init(\'' + TM + '\',' + page + ',' + skip + exparams + ');', this.SPINWAIT) 
			}
			else
			{
				window.setTimeout('ows.Init(\'' + TM + '\',0,' + skip + exparams + ');', this.SPINWAIT) 
			}
		}
		else
		{ 
			if (targetobjectid.length==0)
				targetobjectid = null;
			this.lxOverride();
			this.onLoad(TM,targetobjectid);
			if (!page)
				page=0;
			window['CURRENTPAGE'+TM]=page;
			if (!skip)
			{
				if (targetobjectid!=null)
					this.Fetch(TM,page,qparams,targetobjectid);
				else
					this.Fetch(TM,page,qparams);
			}
		}
	}
	this.onLoad = function onLoad(TM,targetobjectid)
	{
	    if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	        TM = TM+'';		
		if (targetobjectid==undefined||targetobjectid==null)
			this.items['o'+TM].tbl = document.getElementById('lxT' + TM);
		else
			this.items['o'+TM].tbl = document.getElementById(targetobjectid);
	}
	this.Status = function Status(TM,value)
	{
		try
		{
	    if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	        TM = TM+'';			
		if (!this.items['o'+TM].status)
		{
			this.items['o'+TM].status	= document.getElementById(this.items['o'+TM].statusn);	
		}
		if (this.items['o'+TM].status)
		{
			this.items['o'+TM].status.innerHTML = this.items['o'+TM].StatusMessages.Header + value + this.items['o'+TM].StatusMessages.Footer;
		}
		}
		catch (ex)
		{
		}
	}

	this.Load = function Load(TM,targetobjectid)
	{
		var parse = 0;
	    if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	        TM = TM+'';			
		if (this.items['o'+TM].tbl && this.items['o'+TM].data)
		{
			parse=1;
			this.Status(TM,this.items['o'+TM].StatusMessages.Render);
			this.HASSTARTED = false;
			this.Render(TM,targetobjectid);
		}
		this.Status(TM,'');
		if (targetobjectid==null)
		{
			this.Page(TM);
		}
		else
		{
			if(this.items['o'+TM].forcepager)
			{this.Page(TM);}
		}
		if (parse==1)
		{
			if (typeof targetobjectid!=undefined && targetobjectid!=null)
			{
				var targetobject = document.getElementById(targetobjectid);
				if (targetobject!=undefined)
				{
					this.Parse(targetobject);
				}
			}
			else
			{	this.Parse(this.items['o'+TM].tbl); }
		}
		this.CompleteLoad(TM);
	}
	this.Parse = function Parse(obj)
	{
			var objects = new Array();
			objects = obj.getElementsByTagName('SCRIPT');
			for (i=0;i<objects.length;i++)
			{
				//try
				//{	
					this.InstallScript(objects[i]);
				//}
				//catch (ex)
				//{	//DO NOTHING	
				//	alert('This AJAX request contains a javascript error: ' + ex.message);
				//}
			}
	}
	this.InstallScript = function InstallScript(script)
	{
		if (!script)
			return;

		if (script.src)
		{
			var head = document.getElementsByTagName("head")[0];
			var scriptObj = document.createElement("script");

			scriptObj.setAttribute("type", "text/javascript");
			scriptObj.setAttribute("src", script.src);  

			head.appendChild(scriptObj);

		}
		else if (script.innerHTML)
		{
			if (window.execScript)
			{
				window.execScript( script.innerHTML );
			}
			else
				window.eval(script.innerHTML);
		}
	}
	this.CompleteLoad = function CompleteLoad(TM)
	{
	    if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	        TM = TM+'';		
		try {
			if (this.items['o'+TM].onLoad!=null && this.items['o'+TM].onLoad.length > 0) {
				if (this.items['o'+TM].onLoad.indexOf("(")>0)
					eval(this.items['o'+TM].onLoad);
				else
					eval(this.items['o'+TM].onLoad + '();');
			}			
		}
		catch (errObj) {}
	}
	
	//-----------------
	//MANAGER FUNCTIONS
	//-----------------
	//DATA BUILDING FUNCTIONALITY
	//GET THE DATA POINTS FROM THE DOM OBJECT
	this.Content = function Content(TM,src,ignoreResultStats) {
	    if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	        TM = TM+'';	
		if (src && src.length >= 20)
		{
			var strRecordCount;
			strRecordCount = src.substring(0,20);
			if (isNaN(strRecordCount))
			{
				if (typeof ignoreResultStats == 'undefined' || ignoreResultStats == 'false')
					this.items['o'+TM].length = 0;
				return src;
			}
			else
			{
				if (typeof ignoreResultStats == 'undefined' || ignoreResultStats == 'false')
					this.items['o'+TM].length = strRecordCount;
				return src.substring(20);
			}
			
		}
	}
this.GetFormElementValue = function GetFormElementValue(formId,name)
{
    var elements;
    if (typeof name!='undefined'&&name!=null)
        elements = document.forms[formId].elements[name];
    else
        elements = document.forms[formId].elements;
    var str = '';
    if (elements!=null&&elements.length>0)
    {
        for(var i = 0;i < elements.length;i++) 
		   { 
			var value = "";
			   switch(elements[i].type) 
			   { 
					case 'text':
					case 'password':
					case 'textarea':
						value += this.GetElementName(elements[i].name) + "=" + encodeURIComponent(elements[i].value);
						break;
					case 'select-one':
						if (elements[i].options.length > 0 && elements[i].selectedIndex >= 0) {
							value += this.GetElementName(elements[i].name) + "=" + encodeURIComponent(elements[i].options[elements[i].selectedIndex].value);
						}
						break;
					case 'select-multiple':
						if (elements[i].length > 0) {
							var sSelValues = '';
							try
							{
								for (var iSel=0; iSel<fobj.elements[i].length; iSel++ )
								{
									if (elements[i].options[iSel].selected == true)
									{
										if (sSelValues != '')
											   sSelValues += "&" + this.GetElementName(elements[i].name) + '=';
										sSelValues += encodeURIComponent(elements[i].options[iSel].value);
									}
								}
							}
							catch (err)
							{
							}
							value += this.GetElementName(elements[i].name) + "=" + sSelValues;
						}
						break;
					case 'hidden':
						if (elements[i].name != '__VIEWSTATE')
						{
							value += this.GetElementName(elements[i].name) + "=" + encodeURIComponent(elements[i].value); 
						}
						break;
					case 'radio':
						if (elements[i].checked)
						{
							value += this.GetElementName(elements[i].name) + "=" + encodeURIComponent(elements[i].value); 
						}
						break;
					case 'checkbox':
						if (elements[i].checked)
						{
							value += this.GetElementName(elements[i].name) + "=" + encodeURIComponent(elements[i].value); 
						}
						break;
					default:
						//alert(fobj.elements[i].type);
			   } 
				if (value != "")  
				{ if (str!="")
					  str += "&";
				 str += value;
				}
		   }         
    }
    return str;
}
this.GetForm = function GetForm(fobj) 
	{ 
		   var str = ""; 
		   for(var i = 0;i < fobj.elements.length;i++) 
		   { 
			var value = "";
			   switch(fobj.elements[i].type) 
			   { 
					case 'text':
					case 'password':
					case 'textarea':
						value += this.GetElementName(fobj.elements[i].name) + "=" + encodeURIComponent(fobj.elements[i].value);
						break;
					case 'select-one':
						if (fobj.elements[i].options.length > 0 && fobj.elements[i].selectedIndex >= 0) {
							value += this.GetElementName(fobj.elements[i].name) + "=" + encodeURIComponent(fobj.elements[i].options[fobj.elements[i].selectedIndex].value);
						}
						break;
					case 'select-multiple':
						if (fobj.elements[i].length > 0) {
							var sSelValues = '';
							try
							{
								for (var iSel=0; iSel<fobj.elements[i].length; iSel++ )
								{
									if (fobj.elements[i].options[iSel].selected == true)
									{
										if (sSelValues != '')
											   sSelValues += "&" + this.GetElementName(fobj.elements[i].name) + '=';
										sSelValues += encodeURIComponent(fobj.elements[i].options[iSel].value);
									}
								}
							}
							catch (err)
							{
							}
							value += this.GetElementName(fobj.elements[i].name) + "=" + sSelValues;
						}
						break;
					case 'hidden':
						if (fobj.elements[i].name != '__VIEWSTATE')
						{
							value += this.GetElementName(fobj.elements[i].name) + "=" + encodeURIComponent(fobj.elements[i].value); 
						}
						break;
					case 'radio':
						if (fobj.elements[i].checked)
						{
							value += this.GetElementName(fobj.elements[i].name) + "=" + encodeURIComponent(fobj.elements[i].value); 
						}
						break;
					case 'checkbox':
						if (fobj.elements[i].checked)
						{
							value += this.GetElementName(fobj.elements[i].name) + "=" + encodeURIComponent(fobj.elements[i].value); 
						}
						break;
					default:
						//alert(fobj.elements[i].type);
			   } 
				if (value != "")  
				{ if (str!="")
					  str += "&";
				 str += value;
				}
		   } 
		   return str; 
	}
	this.GetQuery = function GetQuery(appendQuery)
	{
		var QRY='';  var cleanURL; var urlParts; var recordElement = false; var isKey = true; var pathQuery = "";var isSkipped=false;
		
		if(appendQuery==null)
			appendQuery='';
		appendQuery='&'+appendQuery.toLowerCase();

		if (document.location.search.length > 0)
			{
			  //trim the question mark
			  QRY = document.location.search.substr(1);
			}
		
		var QRYpairs = QRY.split('&');
		QRY = new Array();
		for (i=0;i<QRYpairs.length;i++)
		{
			var QRYKey = QRYpairs[i].split('=');
			if(appendQuery.indexOf('&' + QRYKey[0].toLowerCase() + '=')==-1)
			{
				QRY.push(QRYpairs[i]);
			}
		}
		QRY = QRY.join('&');

		cleanURL = document.location.pathname;
		urlParts = cleanURL.split("/");
		//looping to leave last item which is page name (<.length-1)
		for (var i=0;i<urlParts.length-1;i++) 
		{
		   if (!recordElement && urlParts[i].toLowerCase()=="tabid") 
		   { recordElement =true; }
		   if (recordElement)
		   {
				if (isKey) 
				{ 
					isSkipped=false;
					if(appendQuery.indexOf('&' + urlParts[i].toLowerCase() + '=')>0)
						isSkipped=true;
					else
						pathQuery += "&" + urlParts[i] + "="; 
				}
				else
				{	if (!isSkipped)
						pathQuery += urlParts[i]; 
				}
				isKey = !isKey;
		   }
		}
		if (pathQuery.length>0)
		{ if (QRY.length>0)
		  { QRY += pathQuery; }
		  else
		  { // remove first amp;
			QRY = pathQuery.substr(1); }
		}

		return QRY;
	}
	
	this.GetElementName=function GetElementName(name)
	{
		if (name.length > 1 && name.substr(0,1) == '_')
			return '"' + name + '"';
		else
			return name;
	}

	this.Sort=function (TM,page,sortIndex,appendQuery,targetobjectid,state)
	{
	    this.items['o'+TM].sort = sortIndex;
	    if (typeof state!='undefined' && state != null)
	        this.items['o'+TM].sortstate = state;
	    else
	        this.items['o'+TM].sortstate = state;
	        
	    if (typeof appendQuery  == 'undefined' || appendQuery == null)
	        appendQuery = '';
	    if (typeof targetobjectid  == 'undefined' || targetobjectid == null)  
		    this.Fetch(TM,this.items['o'+TM].page,appendQuery);
		else
		    this.Fetch(TM,this.items['o'+TM].page,appendQuery,targetobjectid);
	}
	//DATA AJAX FUNCTIONALITY
	this.Fetch=function (TM,page,appendQuery,targetobjectid)
	{
	    if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	        TM = TM+'';		
		var ignoreResultStats = false;
		this.Status(TM,this.items['o'+TM].StatusMessages.Fetch);
		if (!page)
		{
			page = 0;
		}
		
		if (!this.items['o'+TM].page)
		{
			this.items['o'+TM].page=0
		}
		if (page >= 0 && ((this.items['o'+TM].length == 0 && page == 0) || ((page) <= Math.round((this.items['o'+TM].length/this.items['o'+TM].rpp) + 0.5))))
		{
				window['CURRENTPAGE'+TM]=page;		
		        this.items['o'+TM].page = page;
		        this.items['o'+TM].data = false;
		}
		if (page < 0)
		{
			ignoreResultStats = true;
			this.items['o'+TM].data = false;		
		}
		
		if (!this.items['o'+TM].data)
		{
			this.FetchStart(TM,appendQuery,targetobjectid,ignoreResultStats);	
		}
		else
		{
			this.Load(TM);
		}
	}
	this.Include = function(type,url)
	{
        var body = document.getElementsByTagName('body').item(0);
        var obj = null;
        switch(type.toUpperCase())
        {
            case 'JS':
            case 'JAVASCRIPT':
            case 'JSCRIPT':
            case 'TEXT/JAVASCRIPT':
            case 'TEXT\JAVASCRIPT':  
                obj = document.createElement('script');
                obj.src = url;
                obj.type = 'text/javascript';                      
                break;
            case 'CSS':
            case 'STYLE':
            case 'STYLESHEET':
            case 'TEXT/CSS':
            case 'TEXT\CSS':
                obj = document.createElement('link');
                obj.setAttribute('href',url);
                obj.type = 'text/css';                      
                obj.setAttribute('rel','stylesheet');
                break;
        }
        if (obj!=null)
            body.appendChild(obj)
	}
    this.CleanQuery=function CleanQuery(value)
	{
		var query = value;
		//window.location.search.substring(1); <BR>
		var vars = query.split('&'); 
		var result = '';
		var tabid = false;
		var mid = false;
		var tmid = false;
		for (var i=0;i<vars.length;i++) 
		{ 
			var pair = vars[i].split('=');
			switch (pair[0].toLowerCase())
			{
				case 'tabid':
					tabid = pair[1];
					break;
				case 'mid':
					mid = pair[1];
					break;
				case 'tmid':
					tmid = pair[1];
					break;
				default:
					if (result.length > 0)
						result += '&'
					result += pair[0] + '=' + pair[1]
			}
		}
		if (tabid)
		{
			if (result.length > 0)
				result += '&'
			result += 'tabid=' + tabid;
		}
		if (mid)
		{
			if (result.length > 0)
				result += '&'
			result += 'mid=' + mid;
		}
		if (tmid)
		{
			if (result.length > 0)
				result += '&'
			result += 'tmid=' + tmid;
		}		
		return result;
	}
	this.FetchStart=function FetchStart(TM,appendQuery,targetobjectid,ignoreResultStats)
	{			
	
		var isIframe = false;
		var pgValue = this.items['o'+TM].page;
		if (ignoreResultStats)
		    pgValue = -1;
		
		var qParams = {"QueryString":""}
		var oParams = {"Default":this.items['o'+TM].request,"RecordsPerPage":this.items['o'+TM].rpp,"PageNumber":pgValue}
		
		if (typeof this.items['o'+TM].sort != 'undefined' && this.items['o'+TM].sort!=null)
		{
		    oParams.sort = this.items['o'+TM].sort;
		    this.items['o'+TM].sort=null; //THIS MUST AUTOMATICALLY RESET
		    
		    if (typeof this.items['o'+TM].sortstate != 'undefined' && this.items['o'+TM].sortstate!=null)
		    {
		        oParams.sortstate = this.items['o'+TM].sortstate;
		        this.items['o'+TM].sortstate=null; //THIS MUST AUTOMATICALLY RESET
		    }
		}
		
		if (appendQuery.length>=1)
		  qParams.QueryString = qParams.QueryString + (qParams.QueryString.length>0?'&':'') + appendQuery;

		var getQuery = this.GetQuery(qParams.QueryString);
		if (getQuery.length>0)
		    qParams.QueryString = qParams.QueryString + '&' + getQuery;
		
		url = this.CallbackUrl(oParams,qParams);

		if (window.XMLHttpRequest)
		{
			try {
					this.items['o'+TM].xml = new XMLHttpRequest();
				}
			catch(e)
			{
			    this.items['o'+TM].xml = false;
			}
		}
		else if (window.ActiveXObject) {
			try {
				this.items['o'+TM].xml =  new ActiveXObject("Msxml2.XMLHTTP");
				}
			catch(e) 
				{
					try {
					    this.items['o'+TM].xml = new ActiveXObject("Microsoft.XMLHTTP");
					} 
					catch(e) 
					{
						//eval('XML' + TM + ' = false;');
						isIframe = true;
					}
				}
		}


		if (this.items['o'+TM].xml || isIframe)
		{
			this.Status(TM,this.items['o'+TM].StatusMessages.Fetch);
			try {
				if (document.forms.length>0)
					var fstr = this.GetForm(document.forms[0]);
				else
					var fstr = ""
				var random_num = (Math.round((Math.random()*100000000)+1))
				if (!isIframe) {

                    var _tm=TM;
                    var _targetobjectid=targetobjectid;
                    var _ignoreresultstats=ignoreResultStats;
                    this.items['o'+TM].xml.onreadystatechange = function(){
                        if (_targetobjectid!=null) ows.FetchEnd(_tm,_targetobjectid,_ignoreresultstats);
                        else ows.FetchEnd(_tm);
                    }

					this.items['o'+TM].xml.open("POST", url + '&RA=' + random_num, true);
					this.items['o'+TM].xml.setRequestHeader("Content-Type","application/x-www-form-urlencoded; charset=UTF-8");
					this.items['o'+TM].xml.send(fstr);
				}
				else
				{
					new iframerequest({extendedId:TM,targetObjectId:targetobjectid,onEndResponse:'ows.FetchEndIFrame',formdata:fstr,url:url + '&RA=' + random_num}); 	
				}
			} 
			catch(e)
			{
				this.Status(TM,this.items['o'+TM].StatusMessages.Error.General + e.message);
			}
		}
		else
		{
			this.Status(TM,this.items['o'+TM].StatusMessages.Error.Ajax);
		}
	}
	this.FetchEndIFrame=function FetchEndIFrame(TM,src,targetobjectid)
	{
		DATA = this.Content(TM,src);
		this.Status(TM,'');
		if (DATA)
		{
    		this.items['o'+TM].DATA = DATA;
			this.Load(TM,targetobjectid);
		}		
	}
	
	this.FetchEnd=function FetchEnd(TM,targetobjectid,ignoreResultStats)
	{
	    var DATA = false;
		if (this.items['o'+TM].xml)
		{
			if (this.items['o'+TM].xml.readyState == 4) 
			{
				// only if "OK"
				if (this.items['o'+TM].xml.status == 200) 
				{
					    if (this.items['o'+TM].xml.responseText)
						{
							DATA = this.Content(TM,this.items['o'+TM].xml.responseText,ignoreResultStats);
							this.Status(TM,'');
							if (DATA)
							{
							    this.items['o'+TM].data = DATA;
								this.Load(TM,targetobjectid);
							}
						}
				} else {
					this.Status(TM,this.items['o'+TM].StatusMessages.Error.Data + this.items['o'+TM].xml.statusText);
				}
				
				//CLEAN UP -- AVOID MEMORY LEAKS (IN IE)
				this.items['o'+TM].xml.onreadystatechange = new function() {};
				this.CleanUp(TM);
			}
		}
	}
	
	this.CleanUp=function CleanUp(TM)
	{
    	this.items['o'+TM].data = null;
	    this.items['o'+TM].xml = null;
	}
		
	//RENDER THE RESULTING TABLE
	this.Render_ScriptParseIE = '<br style="display: none;">'
	this.Render = function Render(TM,targetobjectid)
	{
	    if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	        TM = TM+'';		
		var strvalue = '';
		if (this.items['o'+TM].tbl && this.items['o'+TM].data) {
			if (targetobjectid!=undefined)
			{
				var targetobject = document.getElementById(targetobjectid);
				if (targetobject!=undefined)
				{
					targetobject.innerHTML = this.Render_ScriptParseIE + this.items['o'+TM].data;
				}
			}
			else
			{
				this.items['o'+TM].tbl.innerHTML = this.Render_ScriptParseIE + this.items['o'+TM].data;			
			}
		}
		if (this.items['o'+TM].hide==true)
		{
			if (!this.items['o'+TM].data || this.items['o'+TM].data.length==0)
			{
				this.Module(TM,false);
			}
			else
			{
				this.Module(TM,true);
			}
		}
	}
	
	this.MaxPage=function MaxPage(TM)
	{
	    if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	        TM = TM+'';		
		var lastPage = 1;
		if (this.items['o'+TM].length > 2)
		{
			if (this.items['o'+TM].rpp > 0)
			{
				lastPage = Math.ceil((this.items['o'+TM].length/this.items['o'+TM].rpp));
			}
		}
		
		return lastPage;
	}

	
	//BUILD PAGING CONTROL FOR THE RENDERED TABLE
	this.Pagers = new Array();
	this.History = function(TM,page)
	{
	    if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	        TM = TM+'';		
	try {
		if (this.items['o'+TM].historypager)
			window.setTimeout('ows.SetHistory(\''+TM+'\','+page+');',100);
			}
	catch (ex)
	{
	}
		return false;
	}
	this.SetHistory = function(TM,page)
	{
	    if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	        TM = TM+'';		
		document.location.replace(this.PageSet(TM,page));
	}
	this.Page = function Page(TM)//
	{
	    if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	        TM = TM+'';		
		this.items['o'+TM].pgs = document.getElementById(this.items['o'+TM].pgsn);	
		var CURRENTPAGE = 0;
		var DATALENGTH = 0;
		var RPP = 0;
		var PGS = false;
		var PGSa = false;
		var PGSb = new Array();
		var PGStext = '';
		var minPage = 0;
		var maxPage = 0;
		var lastPage = 0;
		
		var PGSHeader = '';
		var PGSFooter = '';
		var PGSPages = 10;
		var PGSPageHalf = 0;
		var PGSPage = 'PAGENUMBER';
		var PGSQueryString = '';
		var PGSTargetId = '';
		var PGSTargetIdValue = '';
		var PGSBack = this.LOCALE_PAGEBACK;
		var PGSNext = this.LOCALE_PAGENEXT;
		var PGSFirst = this.LOCALE_PAGEFIRST;
		var PGSLast = this.LOCALE_PAGELAST;
		var PGSSeparator = '|';
		var PGSPageSeparator = '&nbsp;';
		var PGSBackSeparator = '...';
		var PGSNextSeparator = '...';
		var HistoryPageAnchor;

        CURRENTPAGE = Number(this.items['o'+TM].page);
        DATALENGTH = this.items['o'+TM].length*1;
        RPP = this.items['o'+TM].rpp;
        PGS = this.items['o'+TM].pgs;
        HistoryPageAnchor = this.items['o'+TM].historypagerDisplay;
		PGSa = this.Pagers['o'+TM];
		if (!!PGSa)
		{
			if (PGSa.length>0)
			{
				if (PGSa[0].parentNode==null)
					PGSa = false;
			}
			
		}
		if (!PGSa)
		{
			if (!PGS)
			{
				PGSa = document.getElementsByTagName('lxPager' + TM );
			}
			else
			{
				PGSa = new Array();
				PGSa[0] = PGS;
				PGSHeader = '<div style="width: 100%; text-align: center;">';
				PGSFooter = '</div>';
				PGSa[0].setAttribute('Header',PGSHeader);
				PGSa[0].setAttribute('Footer',PGSFooter);
			}
			this.Pagers['o'+TM] = PGSa;
		}

		if (PGSa.length > 0)
		{
			if(PGSa[0].getAttribute('Header')!=null) PGSHeader = PGSa[0].getAttribute('Header');
			if(PGSa[0].getAttribute('Footer')!=null) PGSFooter = PGSa[0].getAttribute('Footer');
			if(PGSa[0].getAttribute('Pages')!=null) PGSPages = PGSa[0].getAttribute('Pages');
			if(PGSa[0].getAttribute('Page')!=null) PGSPage = PGSa[0].getAttribute('Page');
			if(PGSa[0].getAttribute('Back')!=null) PGSBack = PGSa[0].getAttribute('Back');
			if(PGSa[0].getAttribute('Next')!=null) PGSNext = PGSa[0].getAttribute('Next');
			if(PGSa[0].getAttribute('First')!=null) PGSFirst = PGSa[0].getAttribute('First');
			if(PGSa[0].getAttribute('Last')!=null) PGSLast = PGSa[0].getAttribute('Last');
			if(PGSa[0].getAttribute('Querystring')!=null) PGSQueryString = PGSa[0].getAttribute('Querystring');
			if(PGSa[0].getAttribute('TargetId')!=null) PGSTargetId = PGSa[0].getAttribute('TargetId');
			if(PGSa[0].getAttribute('Separator')!=null) PGSSeparator = PGSa[0].getAttribute('Separator');
			if(PGSa[0].getAttribute('PageSeparator')!=null) PGSPageSeparator = PGSa[0].getAttribute('PageSeparator');
			if(PGSa[0].getAttribute('BackSeparator')!=null) PGSBackSeparator = PGSa[0].getAttribute('BackSeparator');
			if(PGSa[0].getAttribute('NextSeparator')!=null) PGSNextSeparator = PGSa[0].getAttribute('NextSeparator');
		}
		if (DATALENGTH > 1)
		{
			PGSPageHalf = PGSPages/2;
			minPage = (CURRENTPAGE + 1) - (PGSPageHalf-1); //4
			
			if (RPP > 0)
			{
				lastPage = Math.ceil((DATALENGTH/RPP));
			}
			else
			{
				lastPage = minPage;
			}
			
			if (minPage < 0)
			{
				minPage = 0;
			}
			maxPage = minPage + (PGSPageHalf+1); //6
			if (maxPage > lastPage)
			{
				maxPage = lastPage;
			}

			if (DATALENGTH==RPP)
			{
				maxPage = minPage;
				lastPage = minPage;
			}
			if (PGSTargetId!=null&&PGSTargetId.length>0)
			    PGSTargetIdValue = ',\''+PGSTargetId+'\''
			if ((lastPage-1) > 0)
			{
				if (CURRENTPAGE > 0)
				{
					if (PGSBack.length > 0)
					{
						PGStext += '<a href="#'+HistoryPageAnchor+':'+(CURRENTPAGE-1)+'" onclick="ows.Fetch(\'' + TM + '\',' + (CURRENTPAGE - 1) + ',\''+PGSQueryString+'\''+PGSTargetIdValue+');' + 'return ows.History(\''+TM+'\','+(CURRENTPAGE-1)+');' + '">' + PGSBack + '</a>' + PGSPageSeparator + '' + PGSBackSeparator + '&nbsp;';
					}
					if (PGSFirst.length > 0)
					{
						PGStext += '<a href="#'+HistoryPageAnchor+':'+0+'" onclick="ows.Fetch(\'' + TM + '\',' + 0 + ',\''+PGSQueryString+'\''+PGSTargetIdValue+');' + 'return ows.History(\''+TM+'\','+0+');' + '">' + PGSFirst + '</a>' + PGSPageSeparator + '' + PGSSeparator + '' + PGSPageSeparator + '';
					}
				}
				else
				{
					if (PGSBack.length > 0)
					{
						PGStext += '' + PGSBack + '' + PGSPageSeparator + '' + PGSBackSeparator + '' + PGSPageSeparator + '';
					}
					if (PGSFirst.length > 0)
					{
						PGStext += '' + PGSFirst + '' + PGSPageSeparator + '' + PGSSeparator + '' + PGSPageSeparator + '';
					}
				}
				//PGStext += PGSHeader;
				for (x=minPage;x<maxPage;x++)
				{	
					if (x==CURRENTPAGE)
						PGStext +=  PGSPage.replace(/PAGENUMBER/,(x+1));
					else
						PGStext += '<a href="#'+HistoryPageAnchor+':'+(x)+'" onclick="ows.Fetch(\'' + TM + '\',' + x + ',\''+PGSQueryString+'\''+PGSTargetIdValue+');' + 'return ows.History(\''+TM+'\','+x+');' + '">' + PGSPage.replace(/PAGENUMBER/,(x+1)) + '</a>';
					
					PGStext += '' + PGSPageSeparator + '';
				}
				//PGStext += PGSFooter;
				if (CURRENTPAGE < (lastPage-1))
				{
					if (PGSLast.length > 0)
					{
						PGStext += '' + PGSSeparator + '' + PGSPageSeparator + '<a href="#'+HistoryPageAnchor+':'+(lastPage-1)+'" onclick="ows.Fetch(\'' + TM + '\',' + (lastPage - 1) + ',\''+PGSQueryString+'\''+PGSTargetIdValue+');' + 'return ows.History(\''+TM+'\','+(lastPage-1)+');' + '">' + PGSLast + '</a>' + PGSPageSeparator + '' + PGSNextSeparator + '' + PGSPageSeparator + '';
					}
					
					if (PGSNext.length > 0)
					{
						PGStext += '<a href="#'+HistoryPageAnchor+':'+(CURRENTPAGE+1)+'" onclick="ows.Fetch(\'' + TM + '\',' + (CURRENTPAGE + 1) + ',\''+PGSQueryString+'\''+PGSTargetIdValue+');' + 'return ows.History(\''+TM+'\','+(CURRENTPAGE+1)+');' + '">' + PGSNext + '</a>';
					}
				}
				else
				{
					if (PGSLast.length > 0)
					{
						PGStext += '' + PGSSeparator + ' ' + PGSLast + ' ' + PGSNextSeparator + ' ';
					}
					if (PGSNext.length > 0)
					{
						PGStext += '' + PGSNext  + '';
					}
				}
				PGStext = PGSHeader + PGStext + PGSFooter;
			}
		}
		//PGStext = PGSHeader + PGStext + PGSFooter;
		
		if (PGSa.length > 0)
		{
			for(ii=0;ii<PGSa.length;ii++)
			{
				try
				{
					PGSa[ii].innerHTML = PGStext;
				}
				catch(x)
				{
					xt = null;
					xt = document.createElement('div');
					xt.className='CommandButton';
					xt.innerHTML = PGStext;
					xt.Header = PGSHeader;
					xt.Footer = PGSFooter;
					xt.Pages = PGSPages;
					xt.Page = PGSPage;
					xt.Querystring = PGSQueryString;
					xt.TargetId = PGSTargetId;
					xt.Back = PGSBack;
					xt.Next = PGSNext;
					xt.First = PGSFirst;
					xt.Last = PGSLast;
					xt.Separator = PGSSeparator;
					xt.PageSeparator = PGSPageSeparator;
					xt.BackSeparator = PGSBackSeparator;
					xt.NextSeparator = PGSNextSeparator;
					PGSa[ii].parentNode.insertBefore(xt,PGSa[ii]);
					PGSb[ii] = xt;
				}
			}
		}
		if (PGSb.length>0)
		{
			this.Pagers['o'+TM] = PGSb;
		}
	}
this.CurrentPage=function(TM)
{
	    if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	        TM = TM+'';	
	return this.items['o'+TM].page;
}
this.Module=function Module(moduleid,display)
{

	var anchor = false;
	for(i=0;i<document.anchors.length;i++)
	{
		if (document.anchors[i].name == moduleid)
		{
			anchor = document.anchors[i];
			i = document.anchors.length;
		}
	}
	if (anchor!=null)
	{
	    var sibling = anchor.nextSibling; 
		var displaytext = '';
		if (display)
		{
			displaytext = 'block';
		}
		else
		{
			displaytext = 'none';
		}
		while (sibling!=null)
		{
			if (sibling!=null && (sibling.tagName!='A'||(sibling.tagName=='A' && !isNaN(sibling.Name))))
			{
				if (sibling.style!=null && sibling.tagName!='SCRIPT')
				{
					sibling.style.display = displaytext;
				}
				sibling=sibling.nextSibling;
			}
			else
			{
				sibling=null;
			}
		}
	}
}

this.CheckArray=new Array();
this.CheckArrayIndex = 0;
this.onGroupCheckChange=function onGroupCheckChange(moduleid,value)
{
	    if (typeof moduleid!='undefined'&&moduleid!=null) //added to force id to a string.
	        moduleid = moduleid+'';	
	var form = document.forms[0];
	for (i=0;i<form.length;i++)
	{if (form[i].id.substr(0,value.id.length) == value.id && form[i].id != value.id)
			{	form[i].checked = value.checked;
				if (form[i].getAttribute('name'))
				{form[i].onclick();}}}
}
this.CallbackUrl=function CallbackUrl(oObj,qObj)
{
    var qParams = new Array();
    var oParams = new Array();
    var dParams = '';
    for (prop in oObj)
    {
        if (prop.toUpperCase()!='DEFAULT')
        for (Oprop in _OWS_)
        {
            if (Oprop.toUpperCase()==prop.toUpperCase())
            {
                oParams.push(_OWS_[Oprop]+':'+oObj[prop]);
                break;
            }
        }
        else
            dParams = oObj[prop];
    }
    for (prop in qObj)
    {
        switch(prop.toUpperCase())
        {
            case 'QUERYSTRING':
                qParams.push(qObj[prop]);
                break;
            default:
                qParams.push(prop + '=' + escape(qObj[prop]));
        }
    }
    var op = oParams.join(',');
    if (op.length > 0 && dParams.length > 0)
        op += ','
    op += dParams;
    qParams.push('_OWS_='+op);
    return this.urlbase + 'IM.aspx?' + qParams.join('&');
}
this.onCheck=function onCheckChange(moduleid,group,item,value)
{
	    if (typeof moduleid!='undefined'&&moduleid!=null) //added to force id to a string.
	        moduleid = moduleid+'';	
	var val;
	val = 0;
	if (value.checked)
	{
	  val=1;
	}
	
	
	this.CheckArray[this.CheckArrayIndex] = new Image();
	this.CheckArray[this.CheckArrayIndex].src=this.urlbase + 'IM.aspx?_OWS_=' + _OWS_.Action + ':C,' + _OWS_.CheckIndex + ':' + this.CheckArrayIndex + ',' + _OWS_.CheckModuleID + ':' + moduleid + ',' + _OWS_.CheckGroup + ':' + group + ',' + _OWS_.CheckItem + ':' + item + ',' + _OWS_.CheckValue + ':' + val + '&' + this.items['o'+moduleid].request;

	this.CheckArrayIndex = this.CheckArrayIndex + 1;
}
this.onUncheck=function onUncheck(moduleid,group,item,value)
{
	    if (typeof moduleid!='undefined'&&moduleid!=null) //added to force id to a string.
	        moduleid = moduleid+'';	
	var val;
	val = 0;
	if (value.checked)
	{
	  val=1;
	}
	
	
	this.CheckArray[this.CheckArrayIndex] = new Image();
    this.CheckArray[this.CheckArrayIndex].src=this.urlbase + 'IM.aspx?_OWS_=' + _OWS_.Action + ':U,' + _OWS_.CheckIndex + ':' + this.CheckArrayIndex + ',' + _OWS_.CheckModuleID + ':' + moduleid + ',' + _OWS_.CheckGroup + ':' + group + ',' + _OWS_.CheckItem + ':' + item + ',' + _OWS_.CheckValue + ':' + val + ',' + _OWS_.CheckRemove + ':1&' + this.items['o'+moduleid].request;

	this.CheckArrayIndex = this.CheckArrayIndex + 1;
}
this.onSet=function onSet(moduleid,name,value)
{
	    if (typeof moduleid!='undefined'&&moduleid!=null) //added to force id to a string.
	        moduleid = moduleid+'';	
	var val;
	val = 0;
	if (value.checked)
	{
	  val=1;
	}
	
	
	this.CheckArray[this.CheckArrayIndex] = new Image();
	//this.CheckArray[this.CheckArrayIndex].src=this.urlbase + 'IM.aspx?lxA=V&' + this.items['o'+moduleid].request + '&lxIx=' + this.CheckArrayIndex + '&lxM=' + moduleid + '&lxV=' + value + '&lxN=' + sessionVariable;
    this.CheckArray[this.CheckArrayIndex].src=this.urlbase + 'IM.aspx?_OWS_=' + _OWS_.Action + ':V,' + _OWS_.CheckIndex + ':' + this.CheckArrayIndex + ',' + _OWS_.CheckModuleID + ':' + moduleid + ',' + _OWS_.CheckValue + ':' + val + ',' + _OWS_.Name + ':' + sessionVariable + '&' + this.items['o'+moduleid].request;

	this.CheckArrayIndex = this.CheckArrayIndex + 1;
}
this.lxiRequests = {};
this.iframerequest=function iframerequest(options)
{
	this.createForm = function(doc,fstr,url)
	{ 
		var frm = doc.createElement('FORM');
		frm.setAttribute('action',url);
		frm.setAttribute('method','post');
		frm.setAttribute('enctype','multipart/form-data');

		var items = new Array();
		items = fstr.split('&');
		for(i=0;i<items.length;i++)
		{
			var pair = items[i].split('=');
			var name = '';
			var value = '';
			name = pair[0];
			if (pair.length==2)
			{
				value = pair[1];
			}
			var elem = doc.createElement('INPUT');
				elem.setAttribute('type','HIDDEN');
				elem.setAttribute('name',name);
				elem.setAttribute('value',value);
			frm.appendChild(elem);
		}
				var elem = doc.createElement('INPUT');
				elem.setAttribute('type','HIDDEN');
				elem.setAttribute('name','lxiAJAXRESPONSE');
				elem.setAttribute('value','1');
				frm.appendChild(elem);
		doc.documentElement.appendChild(frm);			
		return frm;
	};


	this.onLoad = function(){
		this.frame = document.getElementById('lxi_'+this.uniqueId);

		try {   var data = this.frame.contentDocument.document.body.innerHTML; this.frame.contentDocument.document.close(); }
		catch (e){ 
			try{ var data = this.frame.contentWindow.document.body.innerHTML; this.frame.contentWindow.document.close(); }
			 catch (e){
				 try { var data = this.frame.document.body.innerHTML; this.frame.document.body.close(); }
					catch (e) {
						try	{ var data = window.frames['lxi_'+this.uniqueId].document.body.innerText; } 
						catch (e) { } 
				 }
			}
		}
		if (this.onEndResponse) eval(this.onEndResponse + '(this.extendedId,this.parseContent(data),this.targetObjectId);');
	};		

	this.padLeft = function(str, pad, count) { while(str.length<count) str=pad+str; return str; };
	this.parseContent = function(src)
	{
		if (src && src.length >= 20)
		{
			var strRecordCount;
			var iOffset = 0;
			var ltOffset = src.indexOf('<');
			strRecordCount = src.substring(0,ltOffset);
			if (!isNaN(strRecordCount))
			{ iOffset=ltOffset;	}
			if (src.substring(iOffset,16 + iOffset).toUpperCase()=='<AJAX><NOSCRIPT>')
			{ if (iOffset > 0) { return this.padLeft(strRecordCount,' ',20) + src.substring(16 + iOffset,src.length-18);	} }
			else { return src;	}

		}
	}

	this.createFrame = function() 
	{
		var divElm = document.createElement('DIV');
	    	divElm.style.position = "absolute";
        	divElm.style.top = "0";
        	divElm.style.marginLeft = "-10000px";
		divElm.style.display = "";

		if (navigator.userAgent.indexOf('MSIE') > 0 && navigator.userAgent.indexOf('Opera') == -1) {
		 divElm.innerHTML = '<iframe  name=\"lxi_'+this.uniqueId+'\" id=\"lxi_'+this.uniqueId+'\" src=\"about:blank\" onload=\"setTimeout(function(){document.lxiRequests['+this.uniqueId+'].onLoad()},20);"></iframe>';
		} else {
			var frame = document.createElement("iframe");
			frame.setAttribute("name", "lxi_"+this.uniqueId);
			frame.setAttribute("id", "lxi_"+this.uniqueId);
			eval('frame.onload = function() {document.ows.lxiRequests['+this.uniqueId+'].onLoad()};');
			divElm.appendChild(frame);
		}
		document.documentElement.appendChild(divElm);
		return divElm;
	}
		if (!options) options = {};
		this.form = this.createForm(document,options.formdata,options.url);
		this.uniqueId = new Date().getTime();
		document.lxiRequests[this.uniqueId] = this;
		this.frame = this.createFrame();
		this.onEndResponse = options.onEndResponse || null;
		this.extendedId = options.extendedId || null;
		this.targetObjectId = options.targetObjectId || null;
		this.form.target= 'lxi_'+this.uniqueId;
		this.form.setAttribute("target", 'lxi_'+this.uniqueId);
		this.form.submit();
}
this.override = false;
this.lxFetch = false;
this.lxSort = false;
this._lxFetch = false;
this._lxSort = false;
this._lxModule = false;
this.lxModule = false;
this.lxOverride = function lxOverride()
{

		if (!this.override && !typeof lxFetch == 'function' && document.readyState != 'complete')
		{ 
			window.setTimeout('ows.lxOverride();', this.SPINWAIT) 
		}
		else if (!this.override)
		{
				this.override = true;
				if (typeof window['lxFetch'] == 'function')
				{ eval('ows._lxFetch = ' + window['lxFetch'].toString());
				  eval('ows._lxSort = ' + window['lxSort'].toString());
				  eval('ows._lxModule = ' + window['lxModule'].toString());
				}
				else
				{
				    ows._lxFetch = ows.Fetch;
				    ows._lxSort = ows.Sort;
				    ows._lxModule = ows.Module;
				}
				ows.fetch = ows.Fetch;
				ows.lxFetch = ows.Fetch;
				ows.lxSort = ows.Sort;
				ows.lxModule = ows.Module;
				window['lxFetch'] = function lxFetch(TM,page,appendQuery,targetobjectid)
				{
	                if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	                    TM = TM+'';					
					if (typeof ows.items['o'+TM] == 'undefined')
						ows._lxFetch(TM,page,appendQuery,targetobjectid);
					else
						ows.Fetch(TM,page,appendQuery,targetobjectid);
				};
				window['lxSort'] = function lxSort(TM,page,sortIndex)
				{
	                if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	                    TM = TM+'';						
					if (typeof ows.items['o'+TM] == 'undefined')
						ows._lxSort(TM,page,sortIndex);
					else
						ows.Sort(TM,page,sortIndex);
				};
				window['lxModule'] = function lxModule(TM,display)
				{
	                if (typeof TM!='undefined'&&TM!=null) //added to force id to a string.
	                    TM = TM+'';						
					if (typeof ows.items['o'+TM] == 'undefined')
						ows._lxModule(TM,display);
					else
						ows.Module(TM,display);
				};
		}
}
}
//]]>
var ows = new OWS();
}
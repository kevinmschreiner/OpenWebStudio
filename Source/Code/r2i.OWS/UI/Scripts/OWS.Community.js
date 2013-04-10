function ows_community(){
 this.ObjectType = {
 "Name" : "Submit a Ticket",
 "Code" : "Submit a Ticket",
 "Header": "",
 "Footer": "<div class=HCommand><center><a href=\"#\" onclick=\"SaveProperty(false);\">Cancel</a></center></div>",
 "Description" : "Provides the ability for a community member to submit a trouble ticket.",
 "Display":"Sheet",	 
 "onLoad" : "ows_comm_object.onLoad",
 "onSave" : "",
 "Template" : "<iframe style='border:0px;' src='http://www.openwebstudio.com/' width='100%' height='100%' id='ctlCommunity'></iframe>"
 }; 
 this.onLoad=function(){ };
 this.onSave=function(){ };
 }
 var ows_comm_object = new ows_community();
 
function community_load(url,ex)
{

/*
 var sc=document.createElement('script');
 sc.type='text/javascript';
 sc.id="jsReq"+Math.random();
 sc.src=url+'?community='+ex+'&r='+sc.id;
 document.getElementsByTagName('head')[0].appendChild(sc);
 */
 sysProperty_ObjectItem = {"id":"*Ticket*"};
 ows_comm_object.ObjectType.Template = "<iframe src='http://www.openwebstudio.com/" + url + "' width='100%' height='600' id='ctlCommunity'></iframe>";
 ows_comm_object.ObjectType.Name = ex;
 sysProperty_ObjectType = ows_comm_object.ObjectType;
 LoadProperty(sysProperty_ObjectItem)
 hideBlock(HMain);
 displayProperty(ows_comm_object.ObjectType.Template,eval(sysHeader),eval(sysFooter));
 var info = {"Parameters":{"Name":"Ticket"}};
 sysCommunityHandleCommand('LOAD',sysProperty_ObjectType,info);
 sysProperty_ObjectItem = {"id":"*Ticket*"};
}
function sysCommunityHandleCommand(command,cType,info)
{
	switch(command.toUpperCase())
	{
		case "LOAD":
			return eval(cType.onLoad + '(cType,info);');
			break;
		case "SAVE":
			return eval(cType.onSave + '(cType,info);');
			break;
	}	
}
function onClick_Comm_Login()
{
community_load('IDE.aspx?ctl=login','Login');
}
function onClick_Comm_Ticket()
{
community_load('IDE/TroubleTickets.aspx','Trouble Tickets');
}
function onClick_Comm_I_m_Online()
{
community_load('IDE/Online.aspx','Online');
}
function onClick_Comm_Help()
{
community_load('IDE/Help.aspx','Help');
}
function onClick_Comm_Questions()
{
community_load('IDE/FAQ.aspx','Questions');
}
function onClick_Comm_Wiki()
{
community_load('IDE/Topics.aspx','Wiki');
}
function onClick_Comm_Blogs()
{
community_load('IDE/Blogs.aspx','Blogs');
}
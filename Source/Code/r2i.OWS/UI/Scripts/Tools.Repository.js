//ADD OUR ACTION INTO THE RIBBON
RibbonMain.configuration.Menu[3].Groups.push('Repository');
RibbonMain.configuration.Groups.Repository = {
				"Name":"Repository",
				"Abbr":"Repo",
				"onClick":"",
				"Tabs": 
					[
						{
							"Image":"images/repo.gif",
							"ImageMap":"",
							"Items":["View","Label"]
						},
						{
							"Items":["Clear","Delete","Rollback","Revert"]
						}
					]			
			};
			
//THIS IS REQUIRED TO PHYSICALLY LOAD THE PROPERTY LIST INTO THE ADMIN REGION - onClick_Repo_View DEMONSTRATES THE CALL TO LOAD.
sysAdmin.addProperty(
	'Repository',
	{
	   "Name" : "<center>Configuration Repository</center>",
	   "Code" : "Configuration Repository",
	   "Description" : "Provides the ability for a developer to open a configuration.",
	   "Display":"List",	   
	   "onLoad" : "viewRepo",
	   "onSave" : "",
	   "Template" :
	    "<div id=\"RepositoryView\" class=\"PropertyList\"></div>" +
		"<div id=\"RepositoryViewPager\" class=\"PropertyListPager\"></div><input id=\"frmConfigurationId\" name=\"frmConfigurationId\" type=\"hidden\"/>"
	}
);	
		
function viewRepo(template,action)
{
	sysSetText($('frmConfigurationId'),configurationId);
	ows.Create('RepositoryView',0,20,_OWS_.CModuleId+':RepositoryView,'+_OWS_.ResourceFile+':Admin.aspx.resx,'+_OWS_.ResourceKey+':OWS.Configuration.Repository','','',false,-1,false,'RepositoryView','RepositoryViewPager');
}
		
		
//REPOSITORY EVENTS
function onClick_Repo_Label(o)
{
try{labelRepo();} catch(ex){}
}
function onClick_Repo_Clear(o)
{
try{clearRepo();} catch(ex){}
}
function onClick_Repo_View(o)
{
LoadProperty('Repository');
}
function onClick_Repo_Rollback(o)
{
try{rollbackRepo();} catch(ex){}
}
function onClick_Repo_Delete(o)
{
try{deleteRepo();} catch(ex){}
}
function onClick_Repo_Revert(o)
{
try{revertRepo();} catch(ex){}
}
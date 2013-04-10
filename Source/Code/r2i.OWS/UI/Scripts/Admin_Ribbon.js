var RibbonMain = new Ribbon('RibbonMain',{
	"Rows":"4",
	"defaultItemImage":"images/item.gif",
	"MainClass":"HStrip",
	"GroupClass":"HStripB",
	"LeftClass":"HStripL",
	"CenterClass":"HStripP",
	"CenterImageClass":"HStripPC",
	"RightClass":"HStripR",
	"SingleClass":"HStripLR",
	"SelectedClass":"selected",
	"HoverClass":"hover",
	"ItemHoverClass":"hover",
	"GapClass":"HStripS",
	"MenuBarClass":"HMenuParent",
	"MenuClass":"HMenuItem",
	"onItem":"'onClick_' + (group.Abbr!=undefined?group.Abbr:group.Name) + '_' + item",
	"Menu":[
		{
			"Name":"Home",
			"Groups":["Clipboard","Configuration"],
			"show":"",
			"onShow":"",
			"onHide":"",
			"Default":"true"
		},
		{
			"Name":"General",
			"Groups":["Clipboard","Configuration"],
			"show":"load",
			"onShow":"LoadProperty('General');",
			"onHide":""
		},
		{
			"Name":"Actions",
			"Groups":["Clipboard","Configuration","Actions"],
			"show":"load",
			"onShow":"LoadProperty('Display');",
			"onHide":""
		},
		{
			"Name":"Tools",
			"Groups":["Clipboard","Configuration"],
			"show":"",
			"onShow":"",
			"onHide":""
		},
		{
			"Name":"About",
			"Groups":"",
			"show":"",
			"onShow":"LoadProperty('About');",
			"onHide":""
		}
		,{
			"Name":"Community",
			"Groups":["Community","Support"],
			"show":"",
			"onShow":"",
			"onHide":""
		}		
		],
	"Groups":
		{
			"Clipboard": 
			{
				"Name":"Clipboard",
				"Abbr":"Clip",
				"onClick":"",
				"Tabs": 
					[
						{
							"Image":"images/clip.gif",
							"ImageMap":"",
							"Items":["Copy","Paste"],
							"ItemImages":["images/copy.gif","images/paste.gif"]
						}
					]
			},
			"Configuration": 
			{
				"Name":"Configuration",
				"Abbr":"Conf",
				"onClick":"",
				"Tabs": 
					[
						{
							"Image":"images/save.gif",
							"Items":["Open","New"],
							"ItemImages":["images/open.gif",""]
						},
						{
							"Items":["Filter","Import","Export","Publish"],
							"ItemImages":["","images/import.gif","images/export.gif","images/publish.gif"]
						}
					]
			},			
			"Search": 
			{
				"Name":"Search",
				"Abbr":"Find",
				"onClick":"",
				"Tabs": 
					[
						{
							"Image":"images/find.gif",
							"Items":["Find","Replace"]
						}
					]
			},		
			"Security": 
			{
				"Name":"Security",
				"Abbr":"Lock",
				"onClick":"",
				"Tabs": 
					[
						{
							"Image":"images/lock.gif",
							"Items":["Lock","Unlock"]
						}
					]
			},	
			"Actions":
			{
				"Name":"Actions",
				"onClick":"",
				"Tabs": 
					[
						/*{
							"Image":"images/code.gif",
							"ImageMap":"",
							"Items":["Add","Edit"],
							"ItemImages":["images/add.gif","images/edit.gif"]
						},*/
						{
							"Image":"images/action_move.gif",
							"ImageMap":"#Move"
						},
						{
							"Items":[
								"Template",
								"Header",
								"Footer",
								"Detail",
								"Region",								
								"Variable",
								"Assign",
								"Query",
								"If",
								"Else-If",
								"Else",
								"Comment",					
								"Delay",
								"Email",
								"File",
								"Filter",
								"Goto",
								"Input",
								"Log",
								"Loop",
								"Message",
								"Output",							
								"Redirect",
								"Search",
								]
						}
					]			
			},
			/*"Repository":
			{
				"Name":"Repository",
				"Abbr":"Repo",
				"onClick":"",
				"Tabs": 
					[
						{
							"Image":"images/repo.gif",
							"ImageMap":"",
							"Items":["Enable","Disable"]
						},
						{
							"Items":["View","Clear","Label","Roll-Back"]
						}
					]			
			},*/
			"Events":
			{
				"Name":"Event Log",
				"Abbr":"Event",
				"onClick":"",
				"Tabs": 
					[
						{
							"Image":"images/event.gif",
							"ImageMap":"",
							"Items":["Enable","Disable"]
						},
						{
							"Items":["View","Clear","Import","Export"]
						}
					]			
			},
			"Community":
			{
				"Name":"Community",
				"Abbr":"Comm",
				"onClick":"",
				"Tabs": 
					[
						{
							"Image":"images/ows.gif",
							"ImageMap":"",
							"Items":["Login","I'm Online"],
							"ItemImages":["images/login.gif","images/me.gif"]
						}
					]			
			},
			"Support":
			{
				"Name":"Support",
				"Abbr":"Comm",
				"onClick":"",
				"Tabs": 
					[
						{
							"Image":"images/support.gif",
							"Items":["Help","Questions"],
							"ItemImages":["images/help.gif","images/questions.gif"]
						},
						{
							"Items":["Ticket","Wiki","Blogs"],
							"ItemImages":["images/ticket.gif","images/wiki.gif","images/blogs.gif"]
						}
					]			
			}
		}
});

var RibbonEditor = new Ribbon('RibbonEditor',{
	"Rows":"3",
/*	
	"defaultItemImage":"images/item.gif",
	"MainClass":"HStripE",
	"GroupClass":"HStripB",
	"LeftClass":"HStripL",
	"CenterClass":"HStripP",
	"CenterImageClass":"HStripPC",
	"RightClass":"HStripR",
	"SingleClass":"HStripLR",
	"SelectedClass":"selected",
	"HoverClass":"hover",
	"ItemHoverClass":"hover",
	"GapClass":"HStripS",
*/
	"defaultItemImage":"images/item.gif",
	"MainClass":"HStrip",
	"GroupClass":"HStripB",
	"LeftClass":"HStripL",
	"CenterClass":"HStripP",
	"CenterImageClass":"HStripPC",
	"RightClass":"HStripR",
	"SingleClass":"HStripLR",
	"SelectedClass":"selected",
	"HoverClass":"hover",
	"ItemHoverClass":"hover",
	"GapClass":"HStripS",
	"EditorMenuBarClass":"HMenuBarEditor",
	"MenuBarClass":"HMenuParent",
	"MenuClass":"HMenuItem",
	"onItem":"'onClick_' + (group.Abbr!=undefined?group.Abbr:group.Name) + '_' + item",
	"Menu":[
				{
					"Name":"Script",
					"Groups":["ScriptTags","Filters","Help"],
					"show":"",
					"onShow":"",
					"onHide":"",
					"Default":"true"
				},
                {
					"Name":"Query",
					"Groups":["OWS","Help"],
					"show":"",
					"onShow":"",
					"onHide":"",
					"Default":"false"
				},				
				{
					"Name":"Html",
					"Groups":["HtmlTags","HtmlTemplates"],
					"show":"",
					"onShow":"",
					"onHide":"",
					"Default":"false"
				},
				{
					"Name":"Silverlight",
					"Groups":["XamlTags"],
					"show":"",
					"onShow":"",
					"onHide":"",
					"Default":"false"
				},
				{
					"Name":"Clip",
					"Groups":[],
					"show":"",
					"onShow":"",
					"onHide":"",
					"Default":"false"
				}
				/*,
				{
					"Name":"FaceBook",
					"Groups":[],
					"show":"",
					"onShow":"",
					"onHide":"",
					"Default":"false"
				}*/		
		],
	"Groups":
		{
			"Clipboard": 
			{
				"Name":"Clipboard",
				"Abbr":"Clip",
				"onClick":"",
				"Tabs": 
					[
						{
							"Image":"images/clip.gif",
							"ImageMap":"",
							"Items":["Copy","Paste"],
							"ItemImages":["images/copy.gif","images/paste.gif"]
						}
					]
			},
			"ScriptTags": 
			{
				"Name":"Script Tags",
				"Abbr":"ScriptTags",
				"onClick":"",
				"Tabs": 
					[
						{
							"Style":"width: 290px;",						
							"Columns":"2",
							"ColumnClass":"ScrollTab",
							"ItemHelp":[
							"ACTION : <b>Link Tag</b><br/>A dynamic link tag", 
							"ACTIONS : <b>Advanced Link Tag</b><br/>A multi part dynamic link tag", 
							"ALTERNATE : <b>Alternator</b><br/>Returns the current alternate value, based on the iteration of requests from the provided list. First request returns the first item, the next request the second and so on.", 
							"CHECKLISTITEM : <b>A single item in a check list</b><br/>", 
							"CHECKLIST : <b>A Check List Group</b><br/>",  
							"COLUMNS : <b>Column Layout Tag</b><br/>Quick layout of query data", 
							"COUNT : <b>Count Aggregate</b><br/>Counts the numbers to a Bock Variable",
							"FORMAT (pre): <b>Post Format Value</b><br/>Formats the provided value using Pre formatting, meaning the value is formatted prior to rendering into the tag.<br><br><i>[$Value:Source,Formatter]</i>",														
							"FORMAT (post): <b>Pre Format Value</b><br/>Formats the provided value using Post formatting, meaning the value is formatted after rendering into the tag.<br><br><i>[FORMAT,Value,Formatter]",							
							"IIF : <b>Conditional:</b><br/>This is a conditional tag used to handle If and only If", 
							"LOCALE: <b>Localize</b><br/>Reads resource values directly from the provided file name for the provided key.",
							"MATH : <b>Display Literal Evaluation</b><br/>A Mathematical Expression",  
							"RADIO : <b>Radio Button</b><br/>Convenient radio button method", 
							"SET : <b>SET</b><br/>Set a variable to a temporary Action variable",  
							"SORT : <b>Sort a column</b><br/>Provides a convenient way to provide sorting features", 
							"SUBQUERY : <b>Quick Query</b><br/>Allows a query agains the database",   
							"SUM : <b>Sum Aggregate</b><br/>Adds numbers to a Bock Variable",
							"TEXTEDITOR : <b>Rich Text Editor</b><br/>Adds the Rich Text Editor configured as the default for your environment."
							],
							"ItemEditor":[
							"{ACTION, columnName, sessionVariableName, destination, variableType, renderHREF}",
							"{ACTIONS, columnName1, sessionVariableName1, variableType1, columnName2, sessionVariableName2, variableType2, destination}",
							"{ALTERNATE,Name,Value1,Value2,Value3...}", 
							"{CHECKLISTITEM, groupName, value, default}",
							"{CHECKLIST, groupName, sessionVariableName}",
							"{COLUMNS, colTemplate, separatorTemplate, separatePrePost, ignoreColumnList}",
							"{COUNT,myVariable, Value}",
							"[$Name:Source,FORMATTER]",
							"[FORMAT,\"Value\",FORMATTER]",
							"{IIF, \"Condition\", \"True Part\", \"False Part\"}",
							"[LOCALE,ResourceFilePath,Key]",
							"{MATH, \"Expression\"}",
							"{RADIO, sessionVariableName, columnName}",
							"{SET, myVariable, Value, Collection}",  
							"{SORT, columnName, standardText, ascendingText, descendingText, defaultOrder, sortIndex, optional sortQuerystring, optional sortTargetId}", 
							"{SUBQUERY, Name=\"\", Query=\"\" , Header=\"\", Footer=\"\", NoResultFormat=\"\", NoQueryFormat=\"\", Format=\"\", SelectedFormat=\"\", Value=\"\", SelectedField=\"\", SelectedItems=\"\", UseCache=\"\"}",
							"{SUM,myVariable, Value}",
							"{TEXTEDITOR,id,source name,source collection,width,height}"
							],
							"Items":[
							"ACTION", 
							"ACTIONS", 
							"ALTERNATE", 
							"CHECKLISTITEM", 
							"CHECKLIST",  
							"COLUMNS", 
							"COUNT",
							"FORMAT (Pre)",
							"FORMAT (Post)",
							"IIF", 
							"LOCALE",
							"MATH",  
							"RADIO", 
							"SET",  
							"SORT", 
							"SUBQUERY",   
							"SUM",
							"TEXTEDITOR"
								]
						}
					]
			},
			"Filters": 
			{
				"Name":"Filters",
				"Abbr":"Filters",
				"onClick":"",
				"Tabs": 
					[
						{
							"Style":"width: 170px;",
							"Columns":"1",
							"HelpTab":"Help",
							"ColumnClass":"ScrollTab",
							"ItemHelp":[
								"CANEDIT: <b>Check Edit Authorization</b><br>returns true or false depending on whether the provided user has Edit access.",
								"CONTAINS: <b>Text Contains</b><br>returns true when the provided value contains the specified parameter. Otherwise, false is returned.",
								"DATE: <b>Date Format</b><br>Provides the initial foundation for the Date formatter, many other formatters for date are supported.",
								"DECODEHTML: <b>HTML Decoding</b><br>Decode the value from HTML transport format.",
								"DECODEURI: <b>URI Decoding</b><br>Decode the value from URI transport format.",
								"DECRYPT: <b>Text Decrypt</b><br>Decrypt the text value using the provided key parameter.",
								"ENCODEHTML: <b>HTML Encoding</b><br>Encode the value in HTML transport format. Replacing all appropriate symbols and characters.",
								"ENCODEURI: <b>URI Encoding</b><br>Encode the value for URI compatibility. Replacing all spaces, symbols and other characters with URL appropriate formats.",
								"ENCRYPT: <b>Text Encrypt</b><br>Encrypt the text value using the provided key parameter.",
								"ENDSWITH: <b>Text Ends With</b><br>returns true when the provided value contains the specified parameter. Otherwise, false is the result.",
								"ESCAPE: <b>Tag Escape</b><br>Encodes the value with Tag Escaping for means of transport and communication.",
								"EXISTS: <b>File Exists</b><br>Returns true when the provided value (filename) exists within the path.",
								"FRIENDLYURL: <b>Friendly URL</b><br>Returns the appropriate Friendly URL based on the provided value.",
								"INDEXOF: <b>Text Character Index</b><br>Returns the first position of the provided parameter within the value. If the parameter search fails, the result is -1.",
								"ISDATE: <b>Is a Date</b><br>Returns true when the value is a date.",
								"ISEMPTY: <b>Is Empty</b><br>Returns true when the value is empty, meaning the length of the value is zero.",
								"ISINROLE: <b>Is In Role</b><br>Returns true when the current user appears within the provided value (role)",
								"ISNUMERIC: <b>Is a Number</b><br>Returns true when the value is a numeric format.",
								"ISSUPERUSER: <b>Is Super User</b>Returns true when the user is a member of the super users",
								"LASTINDEXOF: <b>Text Last Character Index</b><br>Returns the last position of the provided parameter within the value. If the parameter search fails, the result is -1.",
								"LEFT: <b>Text Left</b><br>Returns the first [n] number of characters from your value. If [n] is greater than the length of your value the result is your value.",
								"LENGTH: <b>Text Length</b><br>Returns the length of your value.",
								"LIST: <b>Convert to List</b><br>Converts the provided value into list of a different format.",
								"LOWER: <b>Text to Lowercase</b><br>Casts the text value into lowercase.",
								"MAPPATH: <b>File Map Path</b><br>Returns the physical path of the provided relative path.",
								"MID: <b>Text Substring (Mid)</b><br>Returns the value from the middle of your text, starting at position [x] and ending at position [y].",
								"NUMBER: <b>Numeric Format</b><br>Provides the initial foundation for the Number formatter, many other formatters for numbers are supported.",
								"PADLEFT: <b>Text Left Padding</b><br>Fills your text value to a desired width, replacing the missing values with the provided parameter.",
								"PADRIGHT: <b>Text Right Padding</b><br>Fills your text value to a desired width, replacing the missing values with the provided parameter.",
								"REPLACE: <b>Text Replacement</b><br>Replaces a specific character value within your provided text with an alternate value.",
								"RIGHT: <b>Text Right</b><br>Returns the last [n] number of characters from your value. If [n] is greater than the length of your value the result is your value.",
								"SQLFIND: <b>SQL Lookup</b><br>Provided with the table name, column to review, value to find and column to return.",
								"STARTSWITH: <b>Text Begins With</b><br>returns true when the provided value starts with the specific paramter. Otherwise, false is the result.",
								"TABDESCRIPTION: <B>Page Description</b><br>returns the provided page description for the page matching the ID or Name of the specified value.",
								"TABID: <B>Page ID</b><br>returns the provided page id for the page matching the Name of the specified value.",
								"TABID by Column: <B>Page ID Lookup</b><br>returns the provided page id for the page matching the specific column and column value.",
								"TABNAME: <b>Page Name</b><br>returns the page name for the page matching the ID provided.",
								"TABTITLE: <b>Page Title</b><br>returns the page title for the page matching the ID provided.",
								"TRIM: <b>Text Trim</b><br>Trims whitespace characters from the beginning and ending of the text value.",
								"TRIMLEFT: <b>Text Trim Left</b><br>Trims whitespace characters from the beginning of the text value.",
								"TRIMRIGHT: <b>Text Trim Right</b><br>Trims whitespace characters from the ending of the text value.",
								"UNESCAPE: <b>Tag Unescape</b><br>Removes the escaping of tags from the provided text value.",
								"UPPER: <b>Text to Uppercase</b><br>Casts the text value into uppercase",
								"URL: <b>Page URL</b><br>Returns the URL specific to the website from the provided page ID or page Name."
							],						
							"ItemEditor":[
								"{CANEDIT}",
								"{CONTAINS:searchValue}",
								"{0:d}",
								"{DECODEHTML}",
								"{DECODEURI}",
								"{DECRYPT:key}",
								"{ENCODEHTML}",
								"{ENCODEURI}",
								"{ENCRYPT:key}",
								"{ENDSWITH:searchValue}",
								"{ESCAPE}",
								"{EXISTS}",
								"{FRIENDLYURL}",
								"{INDEXOF:searchValue}",
								"{ISDATE}",
								"{ISEMPTY:default}",
								"{ISINROLE}",
								"{ISNUMERIC}",
								"{ISSUPERUSER}",
								"{LASTINDEXOF:searchValue}",
								"{LEFT:length}",
								"{LENGTH}",
								"{LIST:prefix,postfix,delimiter,originalDelimiter}",
								"{LOWER}",
								"{MAPPATH}",
								"{MID:from,to}",
								"{0:##0.00}",
								"{PADLEFT:length,character}",
								"{PADRIGHT:length,character}",
								"{REPLACE:lookFor,replaceWith}",
								"{RIGHT:length}",
								"{SQLFIND:tableName,searchColumn,resultColumn}",
								"{STARTSWITH:searchValue}",
								"{TABDESCRIPTION}",
								"{TABID}",
								"{TABID:column}",
								"{TABNAME}",
								"{TABTITLE}",
								"{TRIM}",
								"{TRIMLEFT}",
								"{TRIMRIGHT}",
								"{UNESCAPE}",
								"{UPPER}",
								"{URL}"							
							],
							"Items":[
								"CANEDIT",
								"CONTAINS",
								"DATE",
								"DECODEHTML",
								"DECODEURI",
								"DECRYPT",
								"ENCODEHTML",
								"ENCODEURI",
								"ENCRYPT",
								"ENDSWITH",
								"ESCAPE",
								"EXISTS",
								"FRIENDLYURL",
								"INDEXOF",
								"ISDATE",
								"ISEMPTY",
								"ISINROLE",
								"ISNUMERIC",
								"ISSUPERUSER",
								"LASTINDEXOF",
								"LEFT",
								"LENGTH",
								"LIST",
								"LOWER",
								"MAPPATH",
								"MID",
								"NUMBER",
								"PADLEFT",
								"PADRIGHT",
								"REPLACE",
								"RIGHT",
								"SQLFIND",
								"STARTSWITH",
								"TABDESCRIPTION",
								"TABID",
								"TABID by Column",
								"TABNAME",
								"TABTITLE",
								"TRIM",
								"TRIMLEFT",
								"TRIMRIGHT",
								"UNESCAPE",
								"UPPER",
								"URL"							
							]
						}
					]
			},
			"OWS": 
			{
				"Name":"OWS",
				"Abbr":"OWS",
				"onClick":"",
				"Tabs": 
					[
						{
							"Style":"width: 170px;",
							"Columns":"1",
							"HelpTab":"Help",
							"ColumnClass":"ScrollTab",
							"ItemHelp":[
								"DIRECTORY: <b>Query the File System</b><br>returns a data table containing the results of the file query.",
								"DOTNETNUKE.PORTALS: <b>Create Portal/Alias</b><br>Execute a portal or alias creation against the DotNetNuke framework, returning the Portal and PortalAlias ID's.",																
								"JSON: <b>JSON Query</b><br>Executes a request which results in a JSON response, mapping the results to a data table structure.",
								"SYSTEM: <b>System Query</b><br>Executes a Query against the system runtime parameters, returning the results in a data table structure.",								
								"XML: <b>XML Query</b><br>Executes a request which results in an XML response, mapping the results to a data table structure."
							],						
							"ItemEditor":[
								"<DIRECTORY>\n\t<PATH></PATH>\n\t<DIRECTORYSEARCH></DIRECTORYSEARCH>\n\t<FILESEARCH></FILESEARCH>\n\t<FLAGS></FLAGS>\n\t<QUERY></QUERY>\n\t<SORT></SORT>\n</DIRECTORY>",
								"<DOTNETNUKE.PORTALS>\n\t<METHOD>Create OR Alias</METHOD>\n\t<PARAMETERS>\n\t<!--THE FOLLOWING ARE USED FOR CREATE-->\n \t\t<PORTALNAME></PORTALNAME>\n\t\t<PORTALALIAS></PORTALALIAS>\n\t\t<ISCHILD></ISCHILD>\n\t\t<HOMEDIRECTORY></HOMEDIRECTORY>\n\t\t<TITLE></TITLE>\n\t\t<KEYWORDS></KEYWORDS>\n\t\t<DESCRIPTION></DESCRIPTION>\n\t\t<FIRSTNAME></FIRSTNAME>\n\t\t<LASTNAME></LASTNAME>\n\t\t<PASSWORD></PASSWORD>\n\t\t<EMAIL></EMAIL>\n\t\t<TEMPLATE></TEMPLATE>\n\t\t<USERNAME></USERNAME>\n\n\t\t<!--THE FOLLOWING ARE USED FOR ALIAS (AND CAN BE COMMA DELIMITED)-->\n\t\t<ALIAS></ALIAS>\n\t</PARAMETERS>\n</DOTNETNUKE.PORTALS>",
								"<JSON>\n\t<PATH></PATH>\n\t<!---USE ONE OF THE FOLLOWING: VALUE,POST,GET,PUT,DELETE-->\n\t<VALUE>\n\t\t<CONTENTTYPE></CONTENTTYPE>\n\t\t<NAME></NAME>\n\t</VALUE>\n\t<POST>\n\t\t<CONTENTTYPE></CONTENTTYPE>\n\t\t<QUERY></QUERY>\n\t\t<HEADERS></HEADERS>\n\t\t<BODY></BODY>\n\t</POST>\n\t<GET>\n\t\t<CONTENTTYPE></CONTENTTYPE>\n\t\t<QUERY></QUERY>\n\t\t<HEADERS></HEADERS>\n\t\t<BODY></BODY>\n\t</GET>\n\t<PUT>\n\t\t<CONTENTTYPE></CONTENTTYPE>\n\t\t<QUERY></QUERY>\n\t\t<HEADERS></HEADERS>\n\t\t<BODY></BODY>\n\t</PUT>\n\t<DELETE>\n\t\t<CONTENTTYPE></CONTENTTYPE>\n\t\t<QUERY></QUERY>\n\t\t<HEADERS></HEADERS>\n\t\t<BODY></BODY>\n\t</DELETE>\n\t<!-- --->\n\t\n\t<AUTHENTICATION>\n\t\t<TYPE></TYPE>\n\t\t<USERNAME></USERNAME>\n\t\t<PASSWORD></PASSWORD>\n\t\t<DOMAIN></DOMAIN>\n\t</AUTHENTICATION>\n\t<ROOT></ROOT>\n\t<TRIM>\n\t\t<START></START>\n\t\t<END></END>\n\t</TRIM>\n\t<COLUMNS>\n\t\t<!---REPEAT AS MANY COLUMNS AS NEEDED-->\n\t\t<COLUMN>\n\t\t\t<NAME></NAME>\n\t\t\t<XPATH></XPATH>\n\t\t</COLUMN>\n\t</COLUMNS>\n</JSON>",
								"<SYSTEM>\n\t<PATH>Cache or Form or Querystring or Session or Cookie or User or Portal or Context or ViewState or Action or Messages or Settings or Headers</PATH>\n</SYSTEM>",
								"<XML>\n\t<PATH></PATH>\n\t<!---USE ONE OF THE FOLLOWING: VALUE,POST,GET,PUT,DELETE,SOAP-->\n\t<VALUE>\n\t\t<CONTENTTYPE></CONTENTTYPE>\n\t\t<NAME></NAME>\n\t</VALUE>\n\t<POST>\n\t\t<CONTENTTYPE></CONTENTTYPE>\n\t\t<QUERY></QUERY>\n\t\t<HEADERS></HEADERS>\n\t\t<BODY></BODY>\n\t</POST>\n\t<GET>\n\t\t<CONTENTTYPE></CONTENTTYPE>\n\t\t<QUERY></QUERY>\n\t\t<HEADERS></HEADERS>\n\t\t<BODY></BODY>\n\t</GET>\n\t<PUT>\n\t\t<CONTENTTYPE></CONTENTTYPE>\n\t\t<QUERY></QUERY>\n\t\t<HEADERS></HEADERS>\n\t\t<BODY></BODY>\n\t</PUT>\n\t<DELETE>\n\t\t<CONTENTTYPE></CONTENTTYPE>\n\t\t<QUERY></QUERY>\n\t\t<HEADERS></HEADERS>\n\t\t<BODY></BODY>\n\t</DELETE>\n\t<SOAP>\n\t\t<CONTENTTYPE></CONTENTTYPE>\n\t\t<ACTION></ACTION>\n\t\t<HEADERS></HEADERS>\n\t\t<BODY></BODY>\n\t</SOAP>\n\t<!-- -->\n\t<AUTHENTICATION>\n\t\t<TYPE></TYPE>\n\t\t<USERNAME></USERNAME>\n\t\t<PASSWORD></PASSWORD>\n\t\t<DOMAIN></DOMAIN>\n\t</AUTHENTICATION>\n\t<TRANSFORM>\n\t\t<SOURCE></SOURCE>\n\t\t<TARGET></TARGET>\n\t\t<DECODE></DECODE>\n\t</TRANSFORM>\n\t<ROWS></ROWS>\n\t<NAMESPACES>\n\t\t<!--REPEAT FOR AS MANY NAMESPACES AS NEEDED (ONE IS REQUIRED HERE)-->\t\t\t\t\n\t\t<NAMESPACE>\n\t\t\t<PREFIX></PREFIX>\n\t\t\t<URI></URI>\n\t\t</NAMESPACE>\n\t</NAMESPACES>\n\t<COLUMNS>\n\t\t<!--REPEAT FOR AS MANY COLUMNS AS NEEDED-->\t\t\t\t\n\t\t<COLUMN>\n\t\t\t<NAME></NAME>\n\t\t\t<XPATH></XPATH>\n\t\t</COLUMN>\n\t</COLUMNS>\n</XML>"
							],
							"Items":[
								"DIRECTORY",
								"DOTNETNUKE.PORTALS",
								"JSON",
								"SYSTEM",
								"XML"
							]
						}
					]
			},			
			"ClipTags":
			{
				"Name":"Clip",
				"Abbr":"ClipTags",
				"onClick":"",
				"Tabs":
					[
						{
							"Style":"width: 170px;",
							"Columns":"4",
							"ColumnClass":"ScrollTab",
							"ItemEditor":[],
							"Items":[]
						}
					]
			},
			"Help":
			{
				"Name":"Help",
				"Abbr":"Help",
				"onClick":"",
				"Tabs":
					[
						{
							"Columns":"1",
							"ColumnClass":"HelpTab",
							"Help":"true",
							"Style":"border-right: 1px solid #cccccc;",
							"DefaultHelp":"Hover over an element to the left, and review the help."
						}
					]
			},
			"HtmlTags": 
			{
				"Name":"Html Tags",
				"Abbr":"HtmlTags",
				"onClick":"",
				"Tabs": 
					[
						{
							"Style":"width: 490px;",						
							"Columns":"4",
							"ColumnClass":"ScrollTab",
							"ItemEditor":[
							{"Text":"<a name=\"\" id=\"\" href=\"\"></a>","Cursor":"4"}, 
							{"Text":"<b></b>","Cursor":"4"}, 
							{"Text":"<big></big>","Cursor":"6"}, 
							{"Text":"<body></body>","Cursor":"7"},
							{"Text":"<br>","Cursor":"0"},
							{"Text":"<center></center>","Cursor":"9"},
							{"Text":"<input id=\"\" name=\"\" type=\"checkbox\">","Cursor":"0"},
							{"Text":"<!---->","Cursor":"3"}, 							
							{"Text":"<dd></dd>","Cursor":"5"},
							{"Text":"<div></div>","Cursor":"6"},
							{"Text":"<dl></dl>","Cursor":"5"},
							{"Text":"<dt></dt>","Cursor":"5"},
							{"Text":"<em></em>","Cursor":"5"},
							{"Text":"<embed></embed>",  "Cursor":"8"},
							{"Text":"<font style=\"\"></font>", "Cursor":"7"},
							{"Text":"<form></form>",  "Cursor":"7"},
							{"Text":"<h1></h1>", "Cursor":"5"},
							{"Text":"<h2></h2>",   "Cursor":"5"},
							{"Text":"<h3></h3>","Cursor":"5"},
							{"Text":"<h4></h4>","Cursor":"5"},
							{"Text":"<h5></h5>","Cursor":"5"},
							{"Text":"<h6></h6>","Cursor":"5"},
							{"Text":"<head></head>","Cursor":"7"},
							{"Text":"<input id=\"\" name=\"\" value=\"\" type=\"hidden\">","Cursor":"0"},
							{"Text":"<hr>","Cursor":"0"},
							{"Text":"<html></html>","Cursor":"7"},
							{"Text":"<i></i>","Cursor":"4"},
							{"Text":"<img src=\"\" alt=\"\">","Cursor":"0"},
							{"Text":"<input id=\"\" name=\"\">","Cursor":"0"},
							{"Text":"<li></li>","Cursor":"5"},
							{"Text":"<a href=\"\"></a>","Cursor":"4"},
							{"Text":"<marquee bgcolor=\"\" loop=\"\" scrollamount=\"\" width=\"\"></marquee>","Cursor":"10"},
							{"Text":"<meta name=\"\" content=\"\">","Cursor":"0"},
							{"Text":"<ol></ol>","Cursor":"5"},
							{"Text":"<option value=\"\"></option>","Cursor":"9"},
							{"Text":"<input id=\"\" name=\"\" value=\"\" type=\"password\">","Cursor":"0"},
							{"Text":"<input id=\"\" name=\"\" type=\"radio\">","Cursor":"0"},
							{"Text":"<select id=\"\" name=\"\"></select>","Cursor":"9"},
							{"Text":"<small></small>","Cursor":"8"},
							{"Text":"<strong></strong>","Cursor":"9"},
							{"Text":"<style></style>","Cursor":"8"},
							{"Text":"<input id=\"\" name=\"\" value=\"\" type=\"submit\">","Cursor":"0"},
							{"Text":"<table width=\"\" cellpadding=\"\" cellspacing=\"\"></table>","Cursor":"8"},
							{"Text":"<tbody></tbody>","Cursor":"8"},
							{"Text":"<td></td>","Cursor":"5"},
							{"Text":"<input id=\"\" name=\"\" value=\"\">","Cursor":"0"},
							{"Text":"<textarea style=\"\" id=\"\" name=\"\"></textarea>","Cursor":"11"},
							{"Text":"<th></th>","Cursor":"5"},
							{"Text":"<title></title>","Cursor":"8"},
							{"Text":"<tr></tr>","Cursor":"5"},
							{"Text":"<tt></tt>","Cursor":"5"},
							{"Text":"<u></u>","Cursor":"4"},
							{"Text":"<ul></ul>","Cursor":"5"}
							],
							"Items":[
							"A",
							"B",
							"BIG",
							"BODY",
							"BR",
							"CENTER",
							"CHECKBOX",
							"COMMENT",							
							"DD",
							"DIV",
							"DL",
							"DT",
							"EM",
							"EMBED",
							"FONT",
							"FORM",
							"H1",
							"H2",
							"H3",
							"H4",
							"H5",
							"H6",
							"HEAD",
							"HIDDEN",
							"HR",
							"HTML",
							"I",
							"IMG",
							"INPUT",
							"LI",
							"LINK",
							"MARQUEE",
							"META",
							"OL",
							"OPTION",
							"PASSWORD",
							"RADIO",
							"SELECT",
							"SMALL",
							"STRONG",
							"STYLE",
							"SUBMIT",
							"TABLE",
							"TBODY",
							"TD",
							"TEXTBOX",
							"TEXTAREA",
							"TH",
							"TITLE",
							"TR",
							"TT",
							"U",
							"UL"
								]
						}
					]
			},	
			"HtmlTemplates": 
			{
				"Name":"Html Templates",
				"Abbr":"HtmlTemplates",
				"onClick":"",
				"Tabs": 
					[
						{
							"Style":"width: 190px;",						
							"Columns":"1",
							"ColumnClass":"ScrollTab",
							"ItemEditor":[
							"",
							"{ACTIONS, columnName1, sessionVariableName1, variableType1, columnName2, sessionVariableName2, variableType2, destination}",
							"{ALTERNATE,Name,Value1,Value2,Value3...}", 
							"{CHECKLISTITEM, groupName, columnName, default}",
							"{CHECKLIST, groupName, sessionVariableName, default}",
							"{COLUMNS, colTemplate, separatorTemplate, separatePrePost, ignoreColumnList}",
							"{COUNT,myVariable, Value}",
							"[$Name:Source,FORMATTER]",
							"[FORMAT,\"Value\",FORMATTER]",
							"{IIF, \"Condition\", \"True Part\", \"False Part\"}",
							"[LOCALE,ResourceFilePath,Key]",
							"{MATH, \"Expression\"}",
							"{RADIO, sessionVariableName, columnName}",
							"{SET, myVariable, Value, Collection}",  
							"{SORT, columnName, standardText, ascendingText, descendingText, defaultOrder, sortIndex}", 
							"{SUBQUERY, Name=\"\", Query=\"\" , Header=\"\", Footer=\"\", NoResultFormat=\"\", NoQueryFormat=\"\", Format=\"\", SelectedFormat=\"\", Value=\"\", SelectedField=\"\", SelectedItems=\"\", UseCache=\"\"}",
							"{SUM,myVariable, Value}"
							],
							"Items":[
							"ACTION", 
							"ACTIONS", 
							"ALTERNATE", 
							"CHECKLISTITEM", 
							"CHECKLIST",  
							"COLUMNS", 
							"COUNT",
							"FORMAT (Pre)",
							"FORMAT (Post)",
							"IIF", 
							"LOCALE",
							"MATH",  
							"RADIO", 
							"SET",  
							"SORT", 
							"SUBQUERY",   
							"SUM"
								]
						}
					]
			},
			"XamlTags": 
			{
				"Name":"Silverlight Tags",
				"Abbr":"XamlTags",
				"onClick":"",
				"Tabs": 
					[
						{
							"Style":"width: 600px;",						
							"Columns":"3",
							"ColumnClass":"ScrollTab",
							"ItemEditor":[
							{"Text":"<ArcSegment IsLargeArc=\"\" Name=\"\" Point=\"\" Rotation=\"\" Size=\"\" SweepDirection=\"\"/>","Cursor":"0"},
							{"Text":"<BeginStoryboard></BeginStoryboard>","Cursor":"18"},
							{"Text":"<BezierSegment Name=\"\" Point1=\"\" Point2=\"\" Point3=\"\"/>","Cursor":"0"},
							{"Text":"<Canvas Background=\"\" Canvas.Left=\"\" Canvas.Top=\"\" Canvas.ZIndex=\"\" Cursor=\"\" Height=\"\" IsHitTestVisible=\"\" Name=\"\" Opacity=\"\" RenderTransformOrigin=\"\" Tag=\"\" Visibility=\"\" Width=\"\"></Canvas>","Cursor":"9"},
							{"Text":"<ColorAnimation AutoReverse=\"\" BeginTime=\"\" By=\"\" Duration=\"\" FillBehavior=\"\" From=\"\" Name=\"\" RepeatBehavior=\"\" SpeedRatio=\"\" Storyboard.TargetName=\"\" Storyboard.TargetProperty=\"\" To=\"\"/>","Cursor":"0"},
							{"Text":"<ColorAnimationUsingKeyFrames AutoReverse=\"\" BeginTime=\"\" Duration=\"\" FillBehavior=\"\" Name=\"\" RepeatBehavior=\"\" SpeedRatio=\"\" Storyboard.TargetName=\"\" Storyboard.TargetProperty=\"\"></ColorAnimationUsingKeyFrames>","Cursor":"31"},
							{"Text":"<ColorKeyFrameCollection Name=\"\"></ColorKeyFrameCollection>","Cursor":"26"},
							{"Text":"<DiscreteColorKeyFrame KeyTime=\"\" Name=\"\" Value=\"\"/>","Cursor":"0"},
							{"Text":"<DiscreteDoubleKeyFrame KeyTime=\"\" Name=\"\" Value=\"\"/>","Cursor":"0"},
							{"Text":"<DiscretePointKeyFrame KeyTime=\"\" Name=\"\" Value=\"\"/>","Cursor":"0"},
							{"Text":"<DoubleAnimation AutoReverse=\"\" BeginTime=\"\" By=\"\" Duration=\"\" FillBehavior=\"\" From=\"\" Name=\"\" RepeatBehavior=\"\" SpeedRatio=\"\" Storyboard.TargetName=\"\" Storyboard.TargetProperty=\"\" To=\"\"/>","Cursor":"0"},
							{"Text":"<DoubleAnimationUsingKeyFrames AutoReverse=\"\" BeginTime=\"\" Duration=\"\" FillBehavior=\"\" Name=\"\" RepeatBehavior=\"\" SpeedRatio=\"\" Storyboard.TargetName=\"\" Storyboard.TargetProperty=\"\"></DoubleAnimationUsingKeyFrames>","Cursor":"32"},
							{"Text":"<DoubleCollection Name=\"\"></DoubleCollection>","Cursor":"19"},
							{"Text":"<DoubleKeyFrameCollection Name=\"\"></DoubleKeyFrameCollection>","Cursor":"27"},
							{"Text":"<DrawingAttributes Color=\"\" Height=\"\" Name=\"\" OutlineColor=\"\" Width=\"\"></DrawingAttributes>","Cursor":"20"},
							{"Text":"<Ellipse Canvas.Left=\"\" Canvas.Top=\"\" Canvas.ZIndex=\"\" Cursor=\"\" Fill=\"\" Height=\"\" Name=\"\" Opacity=\"\" RenderTransformOrigin=\"\" Stretch=\"\" Stroke=\"\" StrokeDashArray=\"\" StrokeDashCap=\"\" StrokeDashOffset=\"\" StrokeEndLineCap=\"\" StrokeLineJoin=\"\" StrokeMiterLimit=\"\" StrokeStartCap=\"\" StrokeThickness=\"\" Tag=\"\" Width=\"\"/>","Cursor":"0"},
							{"Text":"<EllipseGeometry Center=\"\" FillRuleName=\"\" RadiusX=\"\" RadiusY=\"\" Transform=\"\"/>","Cursor":"0"},
							{"Text":"<GeometryCollection Name=\"\"></GeometryCollection>","Cursor":"21"},
							{"Text":"<GeometryGroup FillRule=\"\" Name=\"\" Transform=\"\"></GeometryGroup>","Cursor":"16"},
							{"Text":"<Glyphs Canvas.Left=\"\" Canvas.Top=\"\" Canvas.ZIndex=\"\" Cursor=\"\" Fill=\"\" FontRenderingEmSize=\"\" Height=\"\" Indices=\"\" Name=\"\" Opacity=\"\" OriginX=\"\" OriginY=\"\" RenderTransformOrigin=\"\" StyleSimulations=\"\" Tag=\"\" UnicodeString=\"\" Width=\"\"/>","Cursor":"0"},
							{"Text":"<GradientStop Color=\"\" Name=\"\" Offset=\"\"/>","Cursor":"0"},
							{"Text":"<GradientStopCollection Name=\"\"></GradientStopCollection>","Cursor":"25"},
							{"Text":"<Image Canvas.Left=\"\" Canvas.Top=\"\" Canvas.ZIndex=\"\" Cursor=\"\" Height=\"\" Name=\"\" Opacity=\"\" RenderTransformOrigin=\"\" Source=\"\" Stretch=\"\" Tag=\"\" Width=\"\"></Image>","Cursor":"8"},
							{"Text":"<ImageBrush AlignmentX=\"\" AlignmentY=\"\" ImageSource=\"\" Name=\"\" Opacity=\"\" RelativeTransform=\"\" Stretch=\"\" Transform=\"\"/>","Cursor":"0"},
							{"Text":"<InkPresenter Background=\"\" Canvas.Left=\"\" Canvas.Top=\"\" Canvas.ZIndex=\"\" Children=\"\" Clip=\"\" Cursor=\"\" Height=\"\" IsHitTestVisible=\"\" Name=\"\" Opacity=\"\" OpacityMask=\"\" RenderTransform=\"\" RenderTransformOrigin=\"\" Resources=\"\" Strokes=\"\" Tag=\"\" Visibility=\"\" Width=\"\"></InkPresenter>","Cursor":"15"},
							{"Text":"<Line Canvas.Left=\"\" Canvas.Top=\"\" Canvas.ZIndex=\"\" Cursor=\"\" Fill=\"\" Height=\"\" Name=\"\" Opacity=\"\" OpacityMask=\"\" RenderTransformOrigin=\"\" Stretch=\"\" Stroke=\"\" StrokeDashArray=\"\" StrokeDashCap=\"\" StrokeDashOffset=\"\" StrokeEndLineCap=\"\" StrokeLineJoin=\"\" StrokeMiterLimit=\"\" StrokeStartLineCap=\"\" StrokeThickness=\"\" Tag=\"\" Triggers=\"\" Width=\"\" X1=\"\" X2=\"\" Y1=\"\" Y2=\"\"/>","Cursor":"0"},
							{"Text":"<LinearColorKeyFrame KeyTime=\"\" Name=\"\" Value=\"\"/>","Cursor":"0"},
							{"Text":"<LinearDoubleKeyFrame KeyTime=\"\" Name=\"\" Value=\"\"/>","Cursor":"0"},
							{"Text":"<LinearGradientBrush ColorInterpolationMode=\"\" EndPoint=\"\" MappingMode=\"\" Name=\"\" Opacity=\"\" SpreadMethod=\"\" StartPoint=\"\"  Transform=\"\"></LinearGradientBrush>","Cursor":"22"},
							{"Text":"<LinearPointKeyFrame KeyTime=\"\" Name=\"\" Value=\"\"/>","Cursor":"0"},
							{"Text":"<LineBreak FontFamily=\"\" FontSize=\"\" FontStretch=\"\" FontStyle=\"\" FontWeight=\"\" Foreground=\"\" Name=\"\" TextDecorations=\"\"/>","Cursor":"0"},
							{"Text":"<LineGeometry EndPoint=\"\" FillRule=\"\" Name=\"\" StartPoint=\"\" Transform=\"\"/>","Cursor":"0"},
							{"Text":"<LineSegment Name=\"\" Point=\"\"/>","Cursor":"0"},
							{"Text":"<Matrix M11=\"\" M12=\"\" M21=\"\" M22=\"\" Name=\"\" OffsetX=\"\" OffsetY=\"\"/>","Cursor":"0"},
							{"Text":"<MatrixTransform Name=\"\"></MatrixTransform>","Cursor":"18"},
							{"Text":"<MediaElement AutoPlay=\"\" AudioStreamCount=\"\" AudioStreamIndex=\"\" Balance=\"\" BufferingTime=\"\" CanPause=\"\" CanSeek=\"\" Canvas.Left=\"\" Canvas.Top=\"\" Canvas.ZIndex=\"\" Cursor=\"\" Height=\"\" IsMuted=\"\" Name=\"\" NaturalDuration=\"\" NaturalVideoHeight=\"\" NaturalVideoWidth=\"\" Opacity=\"\" OpacityMask=\"\" Position=\"\" RenderTransformOrigin=\"\" Source=\"\" Stretch=\"\" Tag=\"\" Volume=\"\" Width=\"\"/>","Cursor":"0"},
							{"Text":"<Path Canvas.Left=\"\" Canvas.Top=\"\" Canvas.ZIndex=\"\" Cursor=\"\" Fill=\"\" Height=\"\" Name=\"\" Opacity=\"\" RenderTransformOrigin=\"\" Stretch=\"\" Stroke=\"\" StrokeDashArray=\"\" StrokeDashCap=\"\" StrokeDashOffset=\"\" StrokeEndLineCap=\"\" StrokeLineJoin=\"\" StrokeMiterLimit=\"\" StrokeStartLineCap=\"\" StrokeThickness=\"\" Tag=\"\" Width=\"\"/>","Cursor":"0"},
							{"Text":"<PathFigure IsClosed=\"\" Name=\"\" StartPoint=\"\"></PathFigure>","Cursor":"13"},
							{"Text":"<PathFigureCollection Name=\"\"></PathFigureCollection>","Cursor":"23"},
							{"Text":"<PathGeometry></PathGeometry>","Cursor":"15"},
							{"Text":"<PathSegmentCollection  Name=\"\"></PathSegmentCollection>","Cursor":"24"},
							{"Text":"<PointAnimation AutoReverse=\"\" BeginTime=\"\" By=\"\" Duration=\"\" FillBehavior=\"\" From=\"\" Name=\"\" RepeatBehavior=\"\" SpeedRatio=\"\" Storyboard.TargetName=\"\" Storyboard.TargetProperty=\"\" To=\"\"/>","Cursor":"0"},
							{"Text":"<PointAnimationUsingKeyFrames AutoReverse=\"\" BeginTime=\"\" Duration=\"\" FillBehavior=\"\" Name=\"\" RepeatBehavior=\"\" SpeedRatio=\"\" Storyboard.TargetName=\"\" Storyboard.TargetProperty=\"\"></PointAnimationUsingKeyFrames>","Cursor":"31"},
							{"Text":"<PointKeyFrame KeyTime=\"\" Name=\"\" Value=\"\"/>","Cursor":"0"},
							{"Text":"<PointKeyFrameCollection Name=\"\"></PointKeyFrameCollection>","Cursor":"26"},
							{"Text":"<PolyBezierSegment Name=\"\" Points=\"\"></PolyBezierSegment>","Cursor":"20"},
							{"Text":"<Polygon Canvas.Left=\"\" Canvas.Top=\"\" Canvas.ZIndex=\"\" Cursor=\"\" FillRule=\"\" Height=\"\" Name=\"\" Opacity=\"\" Points=\"\" RenderTransformOrigin=\"\" Stretch=\"\" Stroke=\"\" StrokeDashArray=\"\" StrokeDashCap=\"\" StrokeDashOffset=\"\" StrokeEndLineCap=\"\" StrokeLineJoin=\"\" StrokeMiterLimit=\"\" StrokeStartLineCap=\"\" StrokeThickness=\"\" Tag=\"\" Width=\"\"></Polygon>","Cursor":"10"},
							{"Text":"<Polyline Canvas.Left=\"\" Canvas.Top=\"\" Canvas.ZIndex=\"\" Clip=\"\" Cursor=\"\" Fill=\"\" FillRule=\"\" Height=\"\" Name=\"\" Opacity=\"\" OpacityMask=\"\" Points=\"\" RenderTransform=\"\" RenderTransformOrigin=\"\" Resources=\"\" Stretch=\"\" Stroke=\"\" StrokeDashArray=\"\" StrokeDashCap=\"\" StrokeDashOffset=\"\" StrokeEndLineCap=\"\" StrokeLineJoin=\"\" StrokeMiterLimit=\"\" StrokeStartLineCap=\"\" StrokeThickness=\"\" Tag=\"\" Triggers=\"\" Width=\"\"/>","Cursor":"0"},
							{"Text":"<PolyLineSegment Name=\"\" Points=\"\"/>","Cursor":"0"},
							{"Text":"<PolyQuadraticBezierSegment Name=\"\" Points=\"\"/>","Cursor":"0"},
							{"Text":"<QuadraticBezierSegment Name=\"\" Point1=\"\" Point2=\"\"/>","Cursor":"0"},
							{"Text":"<RadialGradientBrush Center=\"\" ColorInterpolationMode=\"\" GradientOrigin=\"\" GradientStops=\"\" MappingMode=\"\" Name=\"\" Opacity=\"\" RadiusX=\"\" RadiusY=\"\" RelativeTransform=\"\" SpreadMethod=\"\" Transform=\"\"></RadialGradientBrush>","Cursor":"22"},
							{"Text":"<Rectangle Canvas.Left=\"\" Canvas.Top=\"\" Canvas.ZIndex=\"\" Clip=\"\" Cursor=\"\" Fill=\"\" Height=\"\" Name=\"\" Opacity=\"\" OpacityMask=\"\" RadiusX=\"\" RadiusY=\"\" RenderTransform=\"\" RenderTransformOrigin=\"\" Resources=\"\" Stretch=\"\" Stroke=\"\" StrokeDashArray=\"\" StrokeDashCap=\"\" StrokeDashOffset=\"\" StrokeEndLineCap=\"\" StrokeLineJoin=\"\" StrokeMiterLimit=\"\" StrokeStartLineCap=\"\" StrokeThickness=\"\" Tag=\"\" Triggers=\"\" Width=\"\"/>","Cursor":"0"},
							{"Text":"<RectangleGeometry FillRule=\"\" Name=\"\" RadiusX=\"\" RadiusY=\"\" Rect=\"\" Transform=\"\"/>","Cursor":"0"},
							{"Text":"<ResourceDictionary Name=\"\"></ResourceDictionary>","Cursor":"21"},
							{"Text":"<RotateTransform Angle=\"\" CenterX=\"\" CenterY=\"\" Name=\"\"/>","Cursor":"0"},
							{"Text":"<Run FontFamily=\"\" FontSize=\"\" FontStretch=\"\" FontStyle=\"\" FontWeight=\"\" Foreground=\"\" Name=\"\" Text=\"\" TextDecorations=\"\"/>","Cursor":"0"},
							{"Text":"<ScaleTransform CenterX=\"\" CenterY=\"\" Name=\"\" ScaleX=\"\" ScaleY=\"\"/>","Cursor":"0"},
							{"Text":"<SkewTransform AngleX=\"\" AngleY=\"\" CenterX=\"\" CenterY=\"\" Name=\"\"/>","Cursor":"0"},
							{"Text":"<SolidColorBrush Color=\"\" Name=\"\" Opacity=\"\" RelativeTransform=\"\" Transform=\"\"/>","Cursor":"0"},
							{"Text":"<SplineColorKeyFrame KeySpline=\"\" KeyTime=\"\" Name=\"\" Value=\"\"/>","Cursor":"0"},
							{"Text":"<SplineDoubleKeyFrame KeySpline=\"\" KeyTime=\"\" Name=\"\" Value=\"\"/>","Cursor":"0"},
							{"Text":"<SplinePointKeyFrame KeySpline=\"\" KeyTime=\"\" Name=\"\" Value=\"\"/>","Cursor":"22"},
							{"Text":"<Storyboard AutoReverse=\"\" BeginTime=\"\" Children=\"\" Duration=\"\" FillBehavior=\"\" Name=\"\" RepeatBehavior=\"\" SpeedRatio=\"\" Storyboard.TargetName=\"\" Storyboard.TargetProperty=\"\"></Storyboard>","Cursor":"13"},
							{"Text":"<Stroke DrawingAttributes=\"\" Name=\"\" StylusPoints=\"\"/>","Cursor":"0"},
							{"Text":"<StylusPoint DrawingAttributes=\"\" Name=\"\" StylusPoints=\"\"/>","Cursor":"0"},
							{"Text":"<StylusPointCollecton Name=\"\"></StylusPointCollecton>","Cursor":"23"},
							{"Text":"<TextBlock ActualHeight=\"\" ActualWidth=\"\" Canvas.Left=\"\" Canvas.Top=\"\" Canvas.ZIndex=\"\" Clip=\"\" Cursor=\"\" FontFamily=\"\" FontSize=\"\" FontStretch=\"\" FontStyle=\"\" FontWeight=\"\" Foreground=\"\" Height=\"\" Inlines=\"\" Name=\"\" Opacity=\"\" OpacityMask=\"\" RenderTransform=\"\" RenderTransformOrigin=\"\" Resources=\"\" Text=\"\" TextDecorations=\"\" TextWrapping=\"\" Tag=\"\" Triggers=\"\" Visibility=\"\" Width=\"\"></TextBlock>","Cursor":"12"},
							{"Text":"<TransformCollection Name=\"\"></TransformCollection>","Cursor":"22"},
							{"Text":"<TransformGroup Name=\"\"></TransformGroup>","Cursor":"17"},
							{"Text":"<TranslateTransform Name=\"\" X=\"\" Y=\"\"/>","Cursor":"0"},
							{"Text":"<TriggerActionCollection Name=\"\"></TriggerActionCollection>","Cursor":"14"},
							{"Text":"<TriggerCollection Name=\"\"></TriggerCollection>","Cursor":"11"},
							{"Text":"<VideoBrush AlignmentX=\"\" AlignmentY=\"\" Name=\"\" Opacity=\"\" RelativeTransform=\"\" SourceName=\"\" Stretch=\"\" Transform=\"\"/>","Cursor":"0"}
							],
							"Items":[
							"ArcSegment",
							"BeginStoryboard",
							"BezierSegment",
							"Canvas",
							"ColorAnimation",
							"ColorAnimationUsingKeyFrames",
							"ColorKeyFrameCollection",
							"DiscreteColorKeyFrame",
							"DiscreteDoubleKeyFrame",
							"DiscretePointKeyFrame",
							"DoubleAnimation",
							"DoubleAnimationUsingKeyFrames",
							"DoubleCollection",
							"DoubleKeyFrameCollection",
							"DrawingAttributes",
							"Ellipse",
							"EllipseGeometry",
							"GeometryCollection",
							"GeometryGroup",
							"Glyphs",
							"GradientStop",
							"GradientStopCollection",
							"Image",
							"ImageBrush",
							"InkPresenter",
							"Line",
							"LinearColorKeyFrame",
							"LinearDoubleKeyFrame",
							"LinearGradientBrush",
							"LinearPointKeyFrame",
							"LineBreak",
							"LineGeometry",
							"LineSegment",
							"Matrix",
							"MatrixTransform",
							"MediaElement",
							"Path",
							"PathFigure",
							"PathFigureCollection",
							"PathGeometry",
							"PathSegmentCollection",
							"PointAnimation",
							"PointAnimationUsingKeyFrames",
							"PointKeyFrame",
							"PointKeyFrameCollection",
							"PolyBezierSegment",
							"Polygon",
							"Polyline",
							"PolyLineSegment",
							"PolyQuadraticBezierSegment",
							"QuadraticBezierSegment",
							"RadialGradientBrush",
							"Rectangle",
							"RectangleGeometry",
							"ResourceDictionary",
							"RotateTransform",
							"Run",
							"ScaleTransform",
							"SkewTransform",
							"SolidColorBrush",
							"SplineColorKeyFrame",
							"SplineDoubleKeyFrame",
							"SplinePointKeyFrame",
							"Storyboard",
							"Stroke",
							"StylusPoint",
							"StylusPointCollecton",
							"TextBlock",
							"TransformCollection",
							"TransformGroup",
							"TranslateTransform",
							"TriggerActionCollection",
							"TriggerCollection",
							"VideoBrush"
							
								]
						}
					]
			}
			
		}
});


CHANGELOG
=========

V2.0.0 (September 25, 2008)
--------------------------

	* Initial Release

V2.1.0 (October 10, 2008)
--------------------------

	* Expansion of Input and Query actions to support Digest Authentication
	* Modification of File action to allow for multiple consumption of form elements
	* Expansion of Action Query Variable to allow for multiple formatters to be applied to a variable value
	* Modification of Variable and Action Value handling to reduce complexity
	* Correct to Firewall Formatter
	* Correction to Replace Formatter
	* Expansion of JSON and XML Query to allow for Digest Authentication

V2.1.1 (October 15, 2008)
--------------------------

	* Correction to Alternate Token to correct issue with shared state value
	* Expansion of Sort Token to allow for additional Querystring and target objects, similar to the use of ows.Fetch
	* Corrections throughout a number of javascript libraries to eliminate references to beta and other.
	* Module Title assignment corrected
	* Action Input corrected to replace tokens for the ContentType
	* Action Input corrected to replace tokens for the Soap Action
	* Corrected Email Action to support multiple addresses for TO,CC and BCC

V2.1.2 (October 16, 2008)
--------------------------

	* Modified handling of Querystring, Session, Viewstate, Form and Cookie variable types to support empty values.
	* Correction to handling of Intramodule Communication Messages to resolve incoming message conflicts.
	* Changed handling of Sessions, Sessionless runtime is now fully supported.
	* Added Caching options for all queries.
	* Added STRIPHTML adjustment to support multiple parameters. For Example: [$Value,"{STRIPHTML:a,p,div,script,style}"]
	* Added CAST formatter to convert JSON to an Object and an Object to a JSON string. Will be used in upcoming OWS Python action.
	* Added Assignments: <System> Page.Response.Headers.Clear;  <System> Page.Response.Clear;  <System> Page.Response.End; 

V2.1.3 (October 23, 2008)
--------------------------

	* Fixed debugging for new configurations. (ejw)
	* Fixed Input action without a content type set explicitly. (ejw)

V2.1.4 (November 6, 2008)
--------------------------

	* Extended Functionality of Configuration Loader to allow for multiple configuration files which operate in incremental fashion.
	* Extended Configurations to include both admin and ui javascript libraries. All libraries are now plugin based
	* Extended UI javascript library functionality to allow for inclusions of Javascript libraries, marked as UI, from the General Settings


V2.1.5 (December 10, 2008)
--------------------------

	* Corrected Bug in GoTo which caused the operations to execute multiple times
	* Corrected Bug in Admin for settings the GoTo Configurations and Regions
	* Corrected loading of configuration file which caused unloading of configs from memory when file was left locked.
	* Modified Build file to generate Upgrade, Install and Source PA's

V2.1.7 (March 25, 2009)
-----------------------
	
	* Enhanced the runtime to provide a bit more cleanup during high Requests Per Second utilization
	* Modified the core runtime to override the module cache value. When Module Cache is enabled in OWS, Page References were left out of caching. No they are cached in the header of the module.
	* Modified Quickbuilder to create actions into a new region at the root of the config, rather than a child of the query action.
	* Modified Quickbuilder to generate the javascript include changes into the configuration javascriptIncludes
	* Corrected installation bug which left openwebstudio.217.config out fo the PA
	* Removed exception thrown when no configuration is specified within the callback querystring.
	* Modified Ajax functionality to include the ability for Progressive Enhancement. Non Ajax driven page loads, with SEO compatible AJAX integration.
	* Extended the DotNetNuke wrapper to support the ability to create a schedule that executes a configuration. Only requirement is setting the FullTypeName to "r2i.OWS.Wrapper.DNN.Interface.Scheduler, r2i.OWS.Wrapper.DotNetNuke.Interface" and the Dependencies to the GUID of the ConfigurationID.
	* Extended the Email Action to support SSL.
	* Corrected bug, Query Actions with child redirect actions failed to redirect.
	* Corrected bug, collision with DNN Help module
	* Corrected Paging and Status, an issue was caused in IE that added extra padding below the div paging and status elements.

V2.1.8 (April 20, 2009)
-----------------------
	
	* Added <SYSTEM> Query to get information about Cookie, Forms, Querystring, Context, User, Portal, et al.
	* Made publishing non-modal and created keybinds for save, open and new configurations.
	* Added OpenControl Token for inserting the output of one configuration into the output of another
	* Correcting Paging logic
	* Extended Growler notifications, included into each of the default interfaces.
	* Extended Growler warnings to include a warning whenever debugging is enabled.
	* Corrected IE7 Issue with paging

V2.1.9 (May 14, 2009)
---------------------

	* Correction to Paging logic for Postbacks (was off due to zero based index)
	* Correction to Admin.css for upgrades
	* Correction to Sort logic for PA's - Session variable for tracking sort now contains configurationID and moduleID

V2.1.10 (May 21, 2009)
----------------------

	* Expansion of General Tab to include ConfigurationID
	* Expansion of General Tab to include Help on ConfigurationID (How to use it for OpenControl,PA Module/Module Settings, Page)
	* Inclusion of Macros.vb, Macros.config and ChangeLog.txt in Source zip.
	* Changed version number to 02.01.10

V2.1.11 (May 22, 2009)
----------------------

	* Another correction to pager - FIRST was broken by the LAST update...
	* Changed version number to 02.01.11

V2.1.12 (June 8, 2009)
----------------------

	* Corrected ModuleCommunication messages for SkinObjects and ModuleSettings
	* Corrected Sort Tag issue which was created in 02.01.11 when sorting was modified for multiple module control interfaces
	* Corrected Paging issue that caused the pager to dissapear in IE7
	* Corrected Paging issue that cause the pager to operate incorrectly in Ajax mode.
	* Corrected spelling error in Quickbuilder
	* Corrected Quickbuilder generation for Prefix and Postfix of querystring parameters
	* Corrected Quickbuilder to leave the Primary Key field out of the INSERT list.
	* Changed version number to 02.01.12
	* Corrected ModuleCommunication messages for SkinObjects and ModuleSettings
	* Added additional logging to smtp exception in Utilities.SendEMail.

V2.1.13 (July 2, 2009)
----------------------

	* Corrected Google Chrome bug with Action tree keyboard detection
	* Corrected Firefox 3.5 issue with JSON namespace and saving configurations
	* Corrected IE8 issue with Validation library
	* Added Query Action ability to Execute Query with Custom Connectionstring
	* Added ability to generate text images with File action
	* Added ability to retrieve the current OWS Version via [OWSVersion,System]
	* Corrected Thickbox issues with latest version of jquery
	* Added current version check to About page
	* Added Querystring debugging to the Statistics within Debug
	* Corrected Debugging for Configurations executed by the Scheduler
	* Changed version number to 02.01.13

V2.1.14 (August 5, 2009)
------------------------

	* Moved Graphics library for Engine into Utilities
	* Added MeasureString formatter to return the width/height of a specific string with font and size identified
	* Added EncodeJavascript formatter to return the javascript encoded version of the value
	* Modified the Installation files for 5.x support issues
	* Added Pager token to generate a custom formatted pager: {PAGER, Current="", Pages="", Format="", CurrentFormat="", LeftFormat="", DisabledLeftFormat="", RightFormat="", DisabledRightFormat="", EmptyFormat=""}
	* Corrected History Page token for Progressive enhancement based modules.
	* Corrected Validation library which caused problems when used with Ventrian Articles module
	* Corrected issue causing PortalID to return as root portal in some instances
	* Corrected onload functionality - custom load function now executes properly.
	* Modified Upgrade from ListX to OWS to change the default order of regions to Variables,Actions followed by Template.
	* Changed version number to 02.01.14

V2.1.15 (August 6, 2009)
------------------------

	* Corrected Build 02.01.14 to contain correct ows.js file
	* Changed version number to 02.01.15

V2.1.16 (August 27, 2009)
------------------------

	* Extended SendEmail utility to support alternate SMTP port number, given format <host>:<port>
	* Extended the Debugging information within the Email Action
	* Modified the Copy/Paste logic to allow for Copy and Paste between configurations, sites, tabs and browsers
	* Corrected XML Query issue when using Transform/Source for items which have a complex tag structure.
	* Corrected iSearchable bug caused when CacheName and CacheTime were implemented (02.01.10)
	* Changed version number to 02.01.16

V2.1.17 (September 18, 2009)
----------------------------
	* Corrected Sliding Expiration problem within Cacheing of Query results. Now a negative cache time is a sliding expiration while a positive cache time is an absolute expiration
	* Modified Import/Export behaviour to copy module settings properly.
	* Corrected File Action - Compression failed for Form input elements.
	* Corrected File Action Admin - Transformation Type was not displaying properly within the list.
	* Changed version number to 02.01.17

V2.1.18 (December 15, 2009)
----------------------------

	* Added CheckExists function to control interface.Import of XML from iPortable will ONLY insert a configuration that doesnt exist already. No Updates will occur.
	* Added Samples to Framework Project for maintenance into /Framework/Samples, these samples include:
	* Sample: Captcha.Example.Using.Recaptcha
	* Sample: Country.Region.AJAX.Dropdown
	* Sample: Directory.List.With.Version.And.Image.Dimensions
	* Sample: DotNetNuke.Create.Portal (uses the new DotNetNuke Extension)
	* Sample: Example.Image.Text.Writer
	* Sample: Sample.Files.Upload.Download.Compress
	* Sample: Simple.List.Query.With.Grouping
	* Sample: Simple.List.Query
	* Sample: Simple.Notes.Add.Edit.List
	* Sample: Wrapper.Test.Routine
	* Added DotNetNuke.Wrapper.Extensions - Adds new Query plugin:
		<DOTNETNUKE.PORTALS>
			<METHOD>Create,Alias</METHOD>
		   <PARAMETERS> 
			<!--THE FOLLOWING ARE USED FOR CREATE:-->
			  <PORTALNAME></PORTALNAME> 
			  <PORTALALIAS></PORTALALIAS> 
			  <ISCHILD></ISCHILD> 
			  <HOMEDIRECTORY></HOMEDIRECTORY> 
			  <TITLE></TITLE> 
			  <KEYWORDS></KEYWORDS> 
			  <DESCRIPTION></DESCRIPTION> 
			  <FIRSTNAME></FIRSTNAME> 
			  <LASTNAME></LASTNAME> 
			  <PASSWORD></PASSWORD> 
			  <EMAIL></EMAIL> 
			  <TEMPLATE></TEMPLATE> 
			  <USERNAME></USERNAME> 
		<!--THE FOLLOWING ARE USED FOR ALIAS: (ALIAS CAN BE COMMA DELIMITED-->
			  <ALIAS></ALIAS>  
		   </PARAMETERS> 
		</DOTNETNUKE.PORTALS> 
	* Added Prepend/Append to Assignment action. 
	* Fixed assignment to header/footer/title to work the same way as all other assignments.
	* Fixed Basic Authentication Bug within INPUT action
	* Added HTTP Request Headers block to Input Action (colon delimiter between key/value, carriage return between Headers)
	* Added HTTP Request Headers block to JSON Query (colon delimiter between key/value, carriage return between Headers)
	* Added HTTP Request Headers block to XML Query (colon delimiter between key/value, carriage return between Headers)
	* Added Request Headers to Debug
	* Modified Debug to provide expand/collapse for Statistics Group
	* Added HTTP Request methods for Input Action: PUT,DELETE
	* Added HTTP Request methods for XML Query: PUT,DELETE
	* Added HTTP Request methods for JSON Query: PUT,DELETE
	* Added Filter Capability for Admin Configuration List
	* Added DIRECTORY,DOTNETNUKE.PORTAL,JSON,SYSTEM,XML Query formats to Editor ribbon
	* Corrected Administration Icon in iActionable Container elements
	* Changed version number to 02.01.18
	
V2.1.19 (February 10, 2010)
---------------------------

	* Modified Wrapper to support iPortable Actions
	* Modified Region to support Import and Export
	* Modified Export logic to support Action variables and Query Result Export
	* Modified Import logic to support Action variables and Query Result Import
	* Modified Query action to support Query with previously executed Name and no Query assigned, for reading existing Cached results.
	* Modified File Action to support UNC paths.
	* Modified Validation library to properly support Date datatype validtions.
	* Modified jQuery library to resolve support between 4.X and 5.X versions of DNN (if jquery is included, it checks before self registering).
	* Modified jQuery NoConflict check, if jQuery is included by default, noConflict is ignored, and $jq mirrors jQuery $.
	* Modfifed Encrypt/Decrypt to support both MD5 and SHA1.
	* Corrected Administration Icon in iActionable Container elements
	* Extended the Form source to store the Form Field value as the content of a stream when the Form Source field value is NOT a form File and IS present in the form.
	* Extended iSearchable to support Debugging
	* Modified Query Action to support reuse of result with Query Action of Same Name and No Query defined.
	* Extended iPortable Import to support Debugging
	* Extended iPortable Export to support Debugging
	* Corrected File Action Image Resizing (ghost border) problem.
	* Extended Render.Variable to include [ModuleTitle,System]
	* Extended ALTERNATE to include True/False as optional First parameter. When True, the Index is ignored and each request increments the Alternator.
	* Extended Module Settings to include View and Settings configurations.
	* Extended Module Settings to include OWS Visible flag, to hide OpenWebStudio functionality once the configurations have been chosen.
	* Added Samples - Release will coincide with tutorials to accompany each of the provided samples.
	* Changed version number to 02.01.19
	
V2.1.19.1 (February 14, 2010)
-----------------------------

	* Corrected XML Query handling, resolving [*INDEX].
	* Corrected XML Query handling HTML Decode properly
	* Changed version number to 02.01.19.01

V2.1.19.2 (February 22, 2010)
-----------------------------
	
	* Modified Email Action to support defining the Email Server parameters.
	* Modified File Action to correct Email Attachment assignment of Content Type
	* Modified File Action to support overriding File Name and Content Type
	* Modified File Action to allow ** wildcard for recursive searching of subdirectories.
	* Change Build Version to 02.01.19.02
	
V2.1.19.3 (April 20, 2010)
-------------------------
	
	* Modified File Action to remove foreslash from Filename during Compress transformation.
	* Added Encode Formatter with syntax [$Value,{ENCODE:type}] where type can be - UTF7,UTF8,UTF32,UNICODE,BIGENDIAN,ASCII,PIGLATIN,BODI,UBBIDUBBI,BACKSLANG,IZZLE
	* Added Sample.Three.Columns.With.Alternate
	* Added Sample.XML.Feed
	* Added Sample.jQuery.DatePicker
	* Added Sample.Encode.Formatter
	* Added Sample.Compress.Portal.Files.With.Extension.Filter
	* Added Sample.Custom.Pager.Using.lxPager
	* Added Sample.Custom.Paging
	* Enhanced OWS.js layer to better consume the Status Messages (2.2 will be released with custom status messages per module)
	* Extended File Action Image Transformation to include Filters and Text Writing on images
	* Added BreakWord formatter to conveniently end a string at or near the closet word break given a requested length
	* Added a Count Formatter to get a Count of the Remaining or Preceding columns from the current point which contain the same value.
	* Added a Replicate Formatter to repeat a value N times
	* Changed Build Version to 02.01.19.03
	
V2.1.20 (July 23, 2010)
-----------------------

	* Corrected Encoding issues, changing ASCII encoding in all places to UTF8 to support localization
	* Modified Action:Input and XML Query to support localization
	* Extended IIF, Math and conditional logic to support String delimiting (',") properly. The original version had issues when mathematical symbols were contained within string blocks.
	* Extended IIF, Math and conditional logic to support Date and Timespan mathematics. Any Date can be compared, or altered using standard US date syntax (MM/dd/yyyy hh:mm:ss.ff tt). Date mathematics requires placing the Date value between a pair of pound signs (#).
		a. Comparison: #01/01/2010 10:00 AM# > #01/01/2010 9:00:AM# (All comparison symbols are supported
		b. Manipulation: #01/02/2010 10:00 AM# - #01/01/2010 10:00 AM# yields a Timespan in date format: #00/01/00 00:00:00.00#
		c. Manipulation: #01/02/2010 10:00 AM# + #05/00/00#  yields a new date: #06/02/2010 10:00 AM#
		d. When the Timespan contains y,m and d - the timespan is not distinguishable from a date itself, so a timespan would need to have a trailing "ts" to identify it. For example: #01/01/2010# + #01/01/01 ts# yields #02/02/2011#
		e. When the manipulation is performed on date as both the left and right values, a timespan is the result
		f. When the manipulation is performed on date as the left, and timespan as the right, a date is the result
		g. When the manipulation is performed on timespan as the left and date as the right, a date is the result
	* Added GUID and UNIQUEIDENTIFIER as system values, used to generate a new random Guid on each call
	* Extended OWS.js to include OWS specific variables
	* Extended Action:Assignment to include assignment to runtime settings (General Properties) of the current configuration. The following settings are assigned by setting the assignment type to System, and Name to "configuration.???" where ??? is the name (non cased).
		a. AutoRefreshInterval - a number identifying how frequently to automatically refresh (in milliseconds)
		b. EnableAjax - identifying that ajax is enabled for the configuration
		c. EnableAjaxCustomPaging - Enables lxPager functionality for the configuration
		d. EnableAjaxCustomStatus - Enables customization of the status value
		e. EnableAjaxManual - identifies that the module will display no results and a physical ows.Fetch call will need to be made to display the configuration.
		f. EnableAjaxPageHistory - identifies that the URL will track the page hist within the url
		g. EnableAjaxPaging - identifies that Ajax will be set to progressively enhanced Ajax paging functionality
		h. EnabeAplhaFilter - displayes the alphabetic filter at the top of the module
		i. EnableCustomPaging - identifies that the query will support custom paging (the second table will contain the page stats)
		j. EnableForcedQuerySplit - identifies that GO will split the Queries to separate statements
		k. RecordsPerPage - the total records to display per page
		l. EnableHideOnNoQuery - the module will be automatically hidden when no query template is defined
		m. EnableHideOnNoResults - the module will be automatically hidden when no results are found after executing the template query
		n. EnableMultipleColumnSorting - when True, all columns are tracked for sort, when false, only one column is allowed at a time
		o. EnablePageSelection - displays the Page selection in non Ajax mode
		p. EnableQueryDebug - begins logging the debug of the current request from this action on for all users
		q. EnableQueryDebugAdmin - begins logging the debug of the current request from this action on for admin users
		r. EnableQueryDebugEdit - begins logging the debug of the current request from this action on for edit users
		s. EnableQueryDebugErrorLog - begins logging the debug of the current request from this action on when SQL throws an exception
		t. EnableQueryDebugSuper - begins logging the debug of the current request from this action on for super users
		u. EnableRecordsPerPage - places the records per page selector above the output
	* Added Custom Connection to File Action
	* Extended all Custom Connections to allow OleDB,ODBC and SQL Client (Native)
	* Added [ROWNUMBER,System] ([INDEX,SYSTEM]) to return the index number of the current row.
	* Added [ROWS,System] to return the total number of rows
	* Changed Build Version to 02.01.20.00
	
V2.1.21 (July 24, 2010)
-----------------------
	
	* Corrected the [INDEX,SYstem] handling to remove handling for *INDEX which conflicts with the Input Action.
	* Added Folder deletion with wildcards. TO delete all the files in a folder For example a folder named FolderTest inside the Portals/0 directory the syntax is: /Portals/0/**FolderTest/FolderTest
	* Changed Build Version to 02.01.21
	
V2.2.0 (April 1, 2011)
----------------------

	* Added the following encryption tokens:
		a. [CRYPTOGRAPHY.KEY.RIJNDAEL,System] - returns a unique key usable with the Rijndael Algorithm
		b. [CRYPTOGRAPHY.KEY.TRIPLEDES,System] - returns a unique key usable with the TripleDes Algorithm
		c. [CRYPTOGRAPHY.KEY.DES,System] - returns a unique key usable with the DES Algorithm
		d. [CRYPTOGRAPHY.KEY.RC2,System] - returns a unique key usable with the RC2 Algorithm
	* Added the following encryption formatters:
		a. [$.........,{ENCRYPT_RIJNDAEL:key}]
		b. [$.........,{ENCRYPT_TRIPLEDES:key}]
		c. [$.........,{ENCRYPT_DES:key}]
		d. [$.........,{ENCRYPT_RC2:key}]
	* Corrected Right Bracket Escape issue with jRibbon.
	* Extended Input Action to support identification (internal) of UTF8 String Encoding on local variables.
	* Extended File Action to support automatic String to Binary conversion of UTF8 String Encoded local variables.
	* Extended Assignment Action to support Response RedirectLocation
	* Extended Assignment to support Attributes JSON Block assignments of meta tags. 
		For example Type: <SYSTEM> Name:Page.Meta.{name:"METATAGNAME",property:"METATAGPROPERTY"} Value:"Testing" would add to the page header:
		<meta property="METATAGPROPERTY" name="METATAGNAME" content="Testing"/>
	* Corrected issue with Module Title not changing within PA modules
	* Resolved naming conflict of NewtonSoft Json.Net library. To continue backwards compatibility to .Net 1.X and 2.X, we built a custom build of NewtonSoft JSON.net
	* Changed Build Version to Production release 02.02.00.00
	
V2.2.2 (May 4, 2011)
--------------------

	* Corrected issue with Cryptography Key Generation
	* Changed Build Version to Production release 02.02.02.00
	
V2.2.3 (July 20, 2011)
----------------------

	* Corrected runtime to support DNN6. DotNetNuke 6 has an issue with Menu Breaks which use a tilda as the title. The issue throws an exception from DDRMenu with an object reference error.
	* Changed Build Version to Production release 02.02.03.00
	
V2.2.4 (November 17, 2011)
--------------------------

	* Correction to GetPassword call for unauthenticated users causing DotNetNuke to throw an exception
	* Changed Build Version to production release 02.02.04.00
	
V2.2.5 (August 27, 2012)
------------------------

	* Several corrections to support DNN 6.1/6.2/6.2.2 releases.
	* Changed Build Version to production release 02.02.05.00
	
V2.2.6 (September 2, 2012)
---------------------------

	* Added new logic for the UI as Editor.aspx
	* Changed Build Version to production release 02.02.06.00
	
V2.2.7 (September 19, 2012)
---------------------------

	* Added new logic for the UI as Editor.aspx
	* Added new service logic for UI as /_services/...
	* Extended System Query to include the Query token logic.
	* Extended System Query to include Form.Files
	* Changed Build Version to production release 02.02.07.00 (beta)
	
V2.2.8 (September 27, 2012)
---------------------------

	* Added configuration.showAll assignment to <System> Collection
	* Changed Build Version to production release 02.02.08.00 (beta)
	
V2.2.9 (January 23, 2013)
-------------------------
	
	* Changed Build Version to porudction release 02.02.09.00

V2.2.14 (February 14, 2013)
---------------------------

	* Extended FILE Formatter to include FILE.IMAGE.ORIENTATION which provides output of topleft,topright etc based on EXIF Orientation
	* Extended FILE Formatter to include FILE.IMAGE.ROTIATION which provides a rotation angle to rotate the image to the correct visual position based on EXIF Orientation
	* Extended FILE Action to include Image Rotation Transform. For all Angles divisible by 90, add X,Y or XY to the end of the angle to perform a rotate and flip. Setting the Angle to a an angle which is not divisible by 90 does not support the X/Y flip and uses the Graphics component to rotate which sometimes causes Memory exceptions due to the handling of the GDI+ library in IIS.
	* Corrected Administration Link in Menu (DNN7)
	* Adjusted build to better support DNN6/DNN7 installation routines
	* Changed Official Build Version to production release 02.02.14.00
	
V2.2.15 (March 1, 2013)
---------------------------

	* Adjusted File Action to properly close files open into memory
	* Adjusted File Action to allow loading into Context for direct stream manipulation
	* Adjusted Admin interface file action handling and features
	* Added Smart Crop feature for files. When Smart crop is used, the system will attempt to locate the beginning and end of the image contained within a white/black space region, and crop the image around that region, then perform the resize.
	* Added Crop feature for files cropping in image when given the Width/Height X/Y parameters.
	* Changed Official Build Version to production release 02.02.15.00
	
	
V2.2.16 (March 26, 2013)
------------------------

	* Changed the handling of the reflected fields within the UserInfo object to support reflection of returned objects. Previously any returned object from the profile would be converted to a string. For example:
		[UserInfo.Profile.PreferredTimeZone,System] would return (UTC-05:00) Eastern Time (US & Canada)
		   No child properties were supported. Now, however you can use any of the sub properties of the underlying object:
			Timezone: [UserInfo.Profile.PreferredTimeZone,System]
			Timezone Identifier: [UserInfo.Profile.PreferredTimeZone.Id,System]
			Timezone Daylight Name: [UserInfo.Profile.PreferredTimeZone.DaylightName,System]
			Timezone Display Name: [UserInfo.Profile.PreferredTimeZone.DisplayName,System]
			Timezone Standard Name: [UserInfo.Profile.PreferredTimeZone.StandardName,System]
			Timezone Supports Daylight Savings: [UserInfo.Profile.PreferredTimeZone.SupportsDaylightSavingTime,System]
			Timezone Utc: [UserInfo.Profile.PreferredTimeZone.Utc,System]
			Timezone Base Offset: [UserInfo.Profile.PreferredTimeZone.BaseUtcOffset,System]
			Timezone Base Offset (Minutes): [UserInfo.Profile.PreferredTimeZone.BaseUtcOffset.TotalMinutes,System]
	* Changed Official Build Version to production release 02.02.16.00

V2.2.17 (April 8, 2013)
-----------------------

	* Added "Action" parameter to assign the result of a JSON query directly to an Action variable
	* Added "Action.NAME.reflected.properties,System" logic for utilizing different sections of JSON objects stored as an action variable
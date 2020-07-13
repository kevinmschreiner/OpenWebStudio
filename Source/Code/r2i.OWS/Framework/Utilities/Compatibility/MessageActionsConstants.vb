'<LICENSE>
'   Open Web Studio - http://www.OpenWebStudio.com
'   Copyright (c) 2007-2008
'   by R2Integrated Inc. http://www.R2integrated.com
'      
'   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'    
'   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
'   the Software.
'   
'   This Software and associated documentation files are subject to the terms and conditions of the Open Web Studio 
'   End User License Agreement and version 2 of the GNU General Public License.
'    
'   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'   TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'   DEALINGS IN THE SOFTWARE.
'</LICENSE>
Namespace r2i.OWS.Framework.Utilities.Compatibility
    Public Class MessageActionsConstants
        Public Const MESSAGE_TYPE_KEY As String = "Type"
        Public Const MESSAGE_VALUE_KEY As String = "Value"

        Public Const ACTIONCOMMENT_VALUE_KEY As String = "Value"

        Public Const ACTIONLOG_NAME_KEY As String = "Name"
        Public Const ACTIONLOG_VALUE_KEY As String = "Value"

        Public Const ACTIONSEARCH_QUERY_KEY As String = "Query"
        Public Const ACTIONSEARCH_TITLE_KEY As String = "Title"
        Public Const ACTIONSEARCH_AUTHOR_KEY As String = "Author"
        Public Const ACTIONSEARCH_DATE_KEY As String = "Date"
        Public Const ACTIONSEARCH_QUERYSTRING_KEY As String = "Querystring"
        Public Const ACTIONSEARCH_KEY_KEY As String = "Key"
        Public Const ACTIONSEARCH_DESCRIPTION_KEY As String = "Description"
        Public Const ACTIONSEARCH_CONTENT_KEY As String = "Content"

        Public Const ACTIONEXECUTE_NAME_KEY As String = "Name"
        Public Const ACTIONEXECUTE_QUERY_KEY As String = "Query"
        Public Const ACTIONEXECUTE_ISPROCESS_KEY As String = "IsProcess"
        Public Const ACTIONEXECUTE_CONNECTION_KEY As String = "Connection"
        Public Const ACTIONEXECUTE_CACHENAME_KEY As String = "CacheName"
        Public Const ACTIONEXECUTE_CACHETIME_KEY As String = "CacheTime"
        Public Const ACTIONEXECUTE_CACHESHARE_KEY As String = "CacheShared"

        Public Const ACTIONASSIGNEMENT_TYPE_KEY As String = "Type"
        Public Const ACTIONASSIGNEMENT_NAME_KEY As String = "Name"
        Public Const ACTIONASSIGNEMENT_VALUE_KEY As String = "Value"
        Public Const ACTIONASSIGNEMENT_SKIPPROCESSING_KEY As String = "SkipProcessing"
        Public Const ACTIONASSIGNEMENT_ASSIGNEMENTTYPE_KEY As String = "AssignmentType"

        Public Const ACTIONREDIRECT_TYPE_KEY As String = "Type"
        Public Const ACTIONREDIRECT_LINK_KEY As String = "Link"
        Public Const ACTIONREDIRECT_PAGEID_KEY As String = "PageID"

        Public Const ACTIONEMAIL_FROM_KEY As String = "From"
        Public Const ACTIONEMAIL_TO_KEY As String = "To"
        Public Const ACTIONEMAIL_CC_KEY As String = "Cc"
        Public Const ACTIONEMAIL_BCC_KEY As String = "Bcc"
        Public Const ACTIONEMAIL_FORMAT_KEY As String = "Format"
        Public Const ACTIONEMAIL_SUBJECT_KEY As String = "Subject"
        Public Const ACTIONEMAIL_BODY_KEY As String = "Body"
        Public Const ACTIONEMAIL_RESULTVARIABLETYPE_KEY As String = "ResultVariableType"
        Public Const ACTIONEMAIL_RESULTVARIABLENAME_KEY As String = "ResultVariableName"
        Public Const ACTIONEMAIL_SMTPUSERNAME_KEY As String = "SMTPUsername"
        Public Const ACTIONEMAIL_SMTPPASSWORD_KEY As String = "SMTPPassword"
        Public Const ACTIONEMAIL_SMTPAUTHTYPE_KEY As String = "SMTPAuthType"
        Public Const ACTIONEMAIL_SMTPSERVER_KEY As String = "SMTPServer"
        Public Const ACTIONEMAIL_SMTPSSL_KEY As String = "SMTPSSL"

        Public Const ACTIONFILTER_OPTIONS_KEY As String = "Options"
        Public Const ACTIONFILTER_OPTION_TEXT_KEY As String = "Option"
        Public Const ACTIONFILTER_OPTION_FIELD_KEY As String = "Field"

        Public Const ACTIONFILE_SOURCETYPE_KEY As String = "SourceType"
        Public Const ACTIONFILE_SOURCEVARIABLETYPE_KEY As String = "SourceVariableType"
        Public Const ACTIONFILE_SOURCE_KEY As String = "Source"
        Public Const ACTIONFILE_SOURCEQUERY_KEY As String = "SourceQuery"
        Public Const ACTIONFILE_SOURCEQUERY_CONNECTION_KEY As String = "SourceConnection"
        Public Const ACTIONFILE_DESTINATIONTYPE_KEY As String = "DestinationType"
        Public Const ACTIONFILE_DESTINATIONVARIABLETYPE_KEY As String = "DestinationVariableType"
        Public Const ACTIONFILE_DESTINATIONRESPONSETYPE_KEY As String = "DestinationResponseType"
        Public Const ACTIONFILE_SQLFIRSTROW_KEY As String = "SQLFirstRow"
        Public Const ACTIONFILE_COLUMNMAPPING_KEY As String = "ColumnMappings"
        Public Const ACTIONFILE_COLUMNMAPPING_POSITION_KEY As String = "Position"
        Public Const ACTIONFILE_COLUMNMAPPING_NAME_KEY As String = "Name"
        Public Const ACTIONFILE_COLUMNMAPPING_TARGET_KEY As String = "Target"
        Public Const ACTIONFILE_COLUMNMAPPING_DEFAULTVALUE_KEY As String = "DefaultValue"
        Public Const ACTIONFILE_COLUMNMAPPING_NULLVALUE_KEY As String = "NullValue"
        Public Const ACTIONFILE_COLUMNMAPPING_FORMAT_KEY As String = "Format"
        Public Const ACTIONFILE_COLUMNMAPPING_INDEX_KEY As String = "Index"
        Public Const ACTIONFILE_COLUMNMAPPING_FILETYPE_KEY As String = "FileType"
        Public Const ACTIONFILE_COLUMNMAPPING_STARTCOLUMN_KEY As String = "StartColumn"
        Public Const ACTIONFILE_COLUMNMAPPING_ENDCOLUMN_KEY As String = "EndColumn"
        Public Const ACTIONFILE_DESTINATIONQUERY_KEY As String = "DestinationQuery"
        Public Const ACTIONFILE_RUNASPROCESS_KEY As String = "RunAsProcess"
        Public Const ACTIONFILE_PROCESSNAME_KEY As String = "ProcessName"
        Public Const ACTIONFILE_DESTINATION_KEY As String = "Destination"
        Public Const ACTIONFILE_TRANSFORMTYPE_KEY As String = "TransformType"
        Public Const ACTIONFILE_IMAGETRANSFORMTYPE_KEY As String = "ImageTransformType"
        Public Const ACTIONFILE_IMAGEFONT_KEY As String = "ImageFont"
        Public Const ACTIONFILE_IMAGESIZE_KEY As String = "ImageSize"
        Public Const ACTIONFILE_IMAGESIZETYPE_KEY As String = "ImageSizeType"
        Public Const ACTIONFILE_IMAGECOLOR_KEY As String = "ImageColor"
        Public Const ACTIONFILE_IMAGEBGCOLOR_KEY As String = "ImageBGColor"
        Public Const ACTIONFILE_IMAGEWARP_KEY As String = "ImageWarp"
        Public Const ACTIONFILE_IMAGETEXT_KEY As String = "ImageText"
        Public Const ACTIONFILE_IMAGEX_KEY As String = "ImageX"
        Public Const ACTIONFILE_IMAGEY_KEY As String = "ImageY"

        Public Const ACTIONFILE_IMAGEWIDTH_KEY As String = "ImageWidth"
        Public Const ACTIONFILE_IMAGEROTATION_KEY As String = "ImageRotation"
        Public Const ACTIONFILE_IMAGEWIDTHTYPE_KEY As String = "ImageWidthType"
        Public Const ACTIONFILE_IMAGEHEIGHT_KEY As String = "ImageHeight"
        Public Const ACTIONFILE_IMAGEHEIGHTYPE_KEY As String = "ImageHeightType"
        Public Const ACTIONFILE_IMAGEQUALITY_KEY As String = "ImageQuality"
        Public Const ACTIONFILE_XMLREADPATH_KEY As String = "XMLReadPath"
        Public Const ACTIONFILE_XMLWRITEPATH_KEY As String = "XMLWritePath"
        Public Const ACTIONFILE_FILETASK_KEY As String = "FileTask"


        Public Const ACTIONOUTPUT_OUTPUTTYPE_KEY As String = "OutputType"
        Public Const ACTIONOUTPUT_FILENAME_KEY As String = "FileName"
        Public Const ACTIONOUTPUT_DELIMITER_KEY As String = "Delimiter"

        Public Const ACTIONINPUT_URL_KEY As String = "URL"
        Public Const ACTIONINPUT_QUERYSTRING_KEY As String = "Querystring"
        Public Const ACTIONINPUT_DATA_KEY As String = "Data"
        Public Const ACTIONINPUT_HEADERS_KEY As String = "Headers"
        Public Const ACTIONINPUT_CONTENTTYPE_KEY As String = "ContentType"
        Public Const ACTIONINPUT_METHOD_KEY As String = "Method"
        Public Const ACTIONINPUT_VARIABLETYPE_KEY As String = "VariableType"
        Public Const ACTIONINPUT_VARIABLENAME_KEY As String = "VariableName"
        Public Const ACTIONINPUT_INPUTFORMAT_KEY As String = "InputFormat"
        Public Const ACTIONINPUT_XPATH_KEY As String = "XPath"
        Public Const ACTIONINPUT_AUTHENTICATIONTYPE_KEY As String = "AuthenticationType"
        Public Const ACTIONINPUT_DOMAIN_KEY As String = "Domain"
        Public Const ACTIONINPUT_USERNAME_KEY As String = "Username"
        Public Const ACTIONINPUT_PASSWORD_KEY As String = "Password"
        Public Const ACTIONINPUT_SOAPACTION_KEY As String = "SoapAction"
        Public Const ACTIONINPUT_SOAPRESULT_KEY As String = "SoapResult"

        Public Const ACTIONCONDITIONIF_LEFTCONDITION_KEY As String = "LeftCondition"
        Public Const ACTIONCONDITIONIF_RIGHTCONDITION_KEY As String = "RightCondition"
        Public Const ACTIONCONDITIONIF_OPERATOR_KEY As String = "Operator"
        Public Const ACTIONCONDITIONIF_ISADVANCED_KEY As String = "IsAdvanced"

        Public Const ACTIONCONDITIONELSEIF_LEFTCONDITION_KEY As String = "LeftCondition"
        Public Const ACTIONCONDITIONELSEIF_RIGHTCONDITION_KEY As String = "RightCondition"
        Public Const ACTIONCONDITIONELSEIF_OPERATOR_KEY As String = "Operator"
        Public Const ACTIONCONDITIONELSEIF_ISADVANCED_KEY As String = "IsAdvanced"

        Public Const ACTIONTEMPLATE_TYPE_KEY As String = "Type"
        Public Const ACTIONTEMPLATE_GROUPSTATEMENT_KEY As String = "GroupStatement"
        Public Const ACTIONTEMPLATE_GROUPINDEX_KEY As String = "GroupIndex"
        Public Const ACTIONTEMPLATE_VALUE_KEY As String = "Value"
        Public Const ACTIONTEMPLATE_QUERYCONNECTION_KEY As String = "Connection" '"QueryConnection"
        Public Const ACTIONTEMPLATE_QUERYFILTER_KEY As String = "Filter" '"QueryFilter"
        Public Const ACTIONTEMPLATE_CACHE_KEY As String = "CacheName"
        Public Const ACTIONTEMPLATE_CACHETIME_KEY As String = "CacheTime"
        Public Const ACTIONTEMPLATE_CACHESHARE_KEY As String = "CacheShared"

        Public Const ACTIONTEMPLATE_VARIABLE_TYPE_KEY As String = "VariableType"
        Public Const ACTIONTEMPLATE_VARIABLE_FORMATTERS_KEY As String = "Formatters"
        Public Const ACTIONTEMPLATE_VARIABLE_DATATYPE_KEY As String = "VariableDataType"
        Public Const ACTIONTEMPLATE_VARIABLE_QUERYSOURCE_KEY As String = "QuerySource"
        Public Const ACTIONTEMPLATE_VARIABLE_QUERYTARGET_KEY As String = "QueryTarget"
        Public Const ACTIONTEMPLATE_VARIABLE_QUERYTARGETLEFT_KEY As String = "QueryTargetLeft"
        Public Const ACTIONTEMPLATE_VARIABLE_QUERYTARGETRIGHT_KEY As String = "QueryTargetRight"
        Public Const ACTIONTEMPLATE_VARIABLE_QUERYTARGETEMPTY_KEY As String = "QueryTargetEmpty"
        Public Const ACTIONTEMPLATE_VARIABLE_ESCAPECODE_KEY As String = "EscapeListX"
        Public Const ACTIONTEMPLATE_VARIABLE_ESCAPESQL_KEY As String = "Protected"
        Public Const ACTIONTEMPLATE_VARIABLE_ESCAPEHTML_KEY As String = "EscapeHTML"

        Public Const ACTIONDELAY_TYPE_KEY As String = "Type"
        Public Const ACTIONDELAY_VALUE_KEY As String = "Value"

        Public Const ACTIONREGION_NAME_KEY As String = "Name"
        Public Const ACTIONREGION_RENDERTYPE_KEY As String = "RenderType"
        Public Const ACTIONREGION_DEBUG_KEY As String = "skipDebug"
        Public Const ACTIONREGION_SEARCH_KEY As String = "includeSearch"
        Public Const ACTIONREGION_IMPORT_KEY As String = "includeImport"
        Public Const ACTIONREGION_EXPORT_KEY As String = "includeExport"

        Public Const ACTIONGOTO_CONFIGURATIONNAME_KEY As String = "ConfigurationID"
        Public Const ACTIONGOTO_REGION_KEY As String = "Region"
        Public Const ACTIONGOTO_CONFIGURATIONDYN_KEY As String = "ConfigurationDyn"
        Public Const ACTIONGOTO_REGIONDYN_KEY As String = "RegionDyn"
    End Class
End Namespace
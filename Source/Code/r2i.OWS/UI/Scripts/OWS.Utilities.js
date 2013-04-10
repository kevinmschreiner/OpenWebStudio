var lxprintSource='';var lxprintCounter=0;var lxprintTimer;function lxprintContent(reportFile,reportPane,reportControl){lxwinPrint=window.open(reportFile,'mywin','left=20,top=20,width=700,height=700,toolbar=1,resizable=1,scrollbars=yes');lxprintSource=document.getElementById(reportPane).innerHTML;lxprintCounter=10;lxwaitForTarget(reportControl);}
function lxwaitForTarget(reportControl){lxprintCounter--;if(!lxwinPrint.document.getElementById(reportControl)){if(lxprintCounter>0){eval('lxprintTimer=setTimeout("lxwaitForTarget(\''+reportControl+'\')", 1000);	');}else{alert('Unable to load report. Please try again	later.');}}else{lxdoneTarget(reportControl);}}
function lxdoneTarget(reportControl){lxwinPrint.document.getElementById(reportControl).innerHTML=lxprintSource;lxwinPrint.print();}
function lxToggle(ObjectId,trackingType,trackingName,trackOn,trackOff,visualObjectId,visualAttribute,visualOn,visualOff){var state='none';var visual='';var tobj=null;var obj=false;obj=document.getElementById(ObjectId);if(obj){if(trackingType.toUpperCase()=='F'){try{var tobj=document.getElementById(trackingName);if(trackOn==tobj.value){state='none';tobj.value=trackOff;visual=visualOff;}else{state='block';tobj.value=trackOn;visual=visualOn;}}
catch(ex){}}else if(trackingType.toUpperCase()=='C'){try{var tobj=false;var cval='';tobj=lxGetCookie(trackingName);if(tobj){cval=tobj;}
if(trackOn==cval){state='none';lxSetCookie(trackingName,trackOff);visual=visualOff;}else{state='block';lxSetCookie(trackingName,trackOn);visual=visualOn;}}
catch(ex){}}else if(trackingType.toUpperCase()=='A'){try{var tobj=obj.getAttribute(trackingName);if(trackOn==tobj){state='none';obj.setAttribute(trackingName,trackOff);visual=visualOff;}else{state='block';obj.setAttribute(trackingName,trackOn);visual=visualOn;}}
catch(ex){}}else{if(obj.style.display.toUpperCase()=='BLOCK'){state='none';visual=visualOff;}else{state='block';visual=visualOn;}}
obj.style.display=state;var visObject=false;visObject=document.getElementById(visualObjectId);if(visObject){if(visualAttribute.toUpperCase()=='SCRIPT')
eval(visual);else{eval('visObject.'+visualAttribute+'=visual;');}}}}
function lxExpander(srcText,parentId,tagType,attributeName,autoHide,showAll,showChild){var parentObj=document.getElementById(parentId);if(parentObj){var childTags=parentObj.getElementsByTagName(tagType);var targetTag=false;for(var i=0;i<childTags.length&&targetTag==false;i++){if(childTags[i].getAttribute(attributeName)&&childTags[i].getAttribute(attributeName)==srcText){targetTag=childTags[i];}}
var defaultblock='none';if(!targetTag){defaultblock='none';}else{defaultblock=targetTag.style.display;}
for(var i=0;i<childTags.length;i++){if(childTags[i].getAttribute(attributeName)){var isExpander=(childTags[i].getAttribute(attributeName)==srcText);var isChildExpanded=false;if(showChild==true&&childTags[i].getAttribute(attributeName).indexOf(srcText)==0){isChildExpanded=true;}
if(isExpander==true||showAll==true){var blocktype=defaultblock;if(isExpander==false&&autoHide==true){blocktype='none';}
if(childTags[i].style.display==blocktype){if(blocktype=='none'){blocktype='block';}else{blocktype='none';}}
if(isExpander==true){if(defaultblock=='none'){blocktype='block';}else{blocktype='none';}}
childTags[i].style.display=blocktype;}else{if(autoHide==true){childTags[i].style.display='none';}}}}}}
function lxSetValue(objectName,Value,checkValue){var obj=document.getElementById(objectName);if(Value !=checkValue&&obj){obj.value=Value;}}
var lxSet_=new Array();var lxSet_i=0;function lxSet(src,ModuleID,Name,Value){lxSet_[lxSet_i]=new Image();lxSet_[lxSet_i].src=src+'/DesktopModules/ListX/xListing.IM.aspx?'+eval('S'+ModuleID)+'&IX='+lxSet_i+'&M='+ModuleID+'&V='+Value+'&N='+Name;lxSet_i++;}
function lxComboSelect(objectName,currentValue){var objDrop=document.getElementById(objectName);for(i=0;i<objDrop.options.length;i++){if(objDrop.options[i].value==currentValue)
objDrop.options[i].selected=true;else
objDrop.options[i].selected=false;}}
function lx_ie_getElementsByTagName(parentObj,str){if(str=="*")
return parentObj.all
else
return parentObj.all.tags(str)}
function lxSmartJoiner(parentObjID,childTagName,valueFunction){childTagName=childTagName.toUpperCase();if(valueFunction==null)
valueFunction=lxGetElementValue;var eltParent=document.getElementById(parentObjID);var eltsChildren;var sJoin='';var sHead='';var sValue;var eltChild;var arrItems=new Array();if(document.all)
eltsChildren=lx_ie_getElementsByTagName(eltParent,childTagName);else
eltsChildren=eltParent.getElementsByTagName(childTagName);if((eltsChildren !="undefined")&&(eltsChildren !=null)){for(var iChild=0;iChild<eltsChildren.length;iChild++){eltChild=eltsChildren[iChild];sValue=valueFunction(eltChild);if(sValue !=null){arrItems[arrItems.length]=sValue;}}}
return(lxSmartJoinArray(arrItems));}
function lxSmartJoinArray(values){var sValue;var sHead='';var sJoin='';if(values.length>0){for(var iValue=0;iValue<values.length;iValue++){sValue=values[iValue];if(sValue !=null){sHead+=sValue.length+';';sJoin+=sValue;}else{sHead+='0;';}}}else{sHead='0;';}
sHead=sHead.length+':'+sHead;return(sHead+sJoin);}
function lxSmartSplitter(sJoinedText){var iPos;var sItems=new Array();if((sJoinedText !=null)&&(sJoinedText.length>0)){iPos=sJoinedText.indexOf(':');if(iPos>0){var sHeadLen=sJoinedText.substring(0,iPos);var iHeadLen=parseInt(sHeadLen);if(iHeadLen==sHeadLen){if(sJoinedText.length>(iHeadLen+iPos)){var sHeader=sJoinedText.substring((iPos+1),(iPos+1+iHeadLen)).split(';');if((sHeader !=null)&&(sHeader.length>0)){iPos=(iPos+iHeadLen+1);for(var iChunk=0;iChunk<sHeader.length;iChunk++){var str=sHeader[iChunk];var iLen=parseInt(str);if(sJoinedText.length>=(iPos+iLen)){if(iLen>0){sItems[sItems.length]=sJoinedText.substring(iPos,(iPos+iLen));iPos+=iLen;}else{sItems[sItems.length]='';}}}}}}}}
return(sItems);}
function lxGetElementValue(elementObj){if((elementObj.getAttribute('type')!="undefined")&&(elementObj.getAttribute('type')!=null)){if(elementObj.getAttribute('type')=="checkbox")
return(elementObj.checked.toString());else
return(elementObj.value);}else if((elementObj.getAttribute('value')!="undefined")&&(elementObj.getAttribute('value')!=null))
return(elementObj.getAttribute('value'));else
return(elementObj.innerHTML);}
function lxEncodeURI(value){return encodeURIComponent(value);}
function lxDecodeURI(value){return decodeURIComponent(value);}
String.prototype.endsWith=function(svalue){if(this.length>svalue.length)
return(this.substr(this.length-svalue.length,svalue.length)==svalue);return false;}
function lxgetCookieVal(offset){var endstr=document.cookie.indexOf(";",offset);if(endstr==-1)
endstr=document.cookie.length;return unescape(document.cookie.substring(offset,endstr));}
function lxGetCookie(name){var arg=name+"=";var alen=arg.length;var clen=document.cookie.length;var cookiei=0;while(cookiei<clen){var j=cookiei+alen;if(document.cookie.substring(cookiei,j)==arg)
return lxgetCookieVal(j);cookiei=document.cookie.indexOf(" ",cookiei)+1;if(cookiei==0)break;}
return null;}
function lxSetCookie(name,value){var argv=lxSetCookie.arguments;var argc=lxSetCookie.arguments.length;var expires=(argc>2)?argv[2]:null;var path=(argc>3)?argv[3]:null;var domain=(argc>4)?argv[4]:null;var secure=(argc>5)?argv[5]:false;document.cookie=name+"="+escape(value)+((expires==null)?"":("; expires="+expires.toGMTString()))+((path==null)?"":("; path="+path))+((domain==null)?"":("; domain="+domain))+((secure==true)?"; secure":"");}
function lxDeleteCookie(){var exp=new Date();exp.setTime(exp.getTime()-1000000000);var cval=lxGetCookie('DemoName');document.cookie='DemoName'+"="+cval+"; expires="+exp.toGMTString();}
function lxsetSelectionRange(input,selectionStart,selectionEnd){if(input.setSelectionRange){input.focus();input.setSelectionRange(selectionStart,selectionEnd);}else if(input.createTextRange){var range=input.createTextRange();range.collapse(true);range.moveEnd('character',selectionEnd);range.moveStart('character',selectionStart);range.select();}}
function lxreplaceValue(replaceValue,input){var result=input;if(replaceValue==true){var re=new RegExp("\\\\+","g");var s='';var lindex=0;var m=re.exec(result);while(m!=null){if(m.index>lindex){s=s+result.substring(lindex,m.index);}
s=s+result.substring(m.index,m.index+m.length);lindex=m.index+m.length;m=re.exec(result);}
if(lindex<result.length){s=s+result.substring(lindex);}
result=s;re=new RegExp("[\"]","g");result=result.replace(re,"\\\"");re=new RegExp("[\[]","g");result=result.replace(re,"\\[");re=new RegExp("[\]]","g");result=result.replace(re,"\\]");re=new RegExp("[\{]","g");result=result.replace(re,'\\{');re=new RegExp("[\}]","g");result=result.replace(re,'\\}');}else{var re=new RegExp("\\\\+","g");var s='';var lindex=0;var m=re.exec(result);while(m!=null){if(m.index>lindex){s=s+result.substring(lindex,m.index);}
s=s+result.substring(m.index,m.index+m.length-1);lindex=m.index+m.length;m=re.exec(result);}
if(lindex<result.length){s=s+result.substring(lindex);}
result=s;}
return result;}
function lxreplaceSelection(input,replaceString,replaceValue){if(input.setSelectionRange){var selectionStart=input.selectionStart;var selectionEnd=input.selectionEnd;var scrollTop=input.scrollTop;if(replaceValue!=undefined&&selectionEnd>selectionStart){var result=input.value.substring(selectionStart,selectionEnd);replaceString=lxreplaceValue(replaceValue,result);}
input.value=input.value.substring(0,selectionStart)+replaceString+input.value.substring(selectionEnd);input.scrollTop=scrollTop;if(selectionStart !=selectionEnd){lxsetSelectionRange(input,selectionStart,selectionStart+replaceString.length);}else{lxsetSelectionRange(input,selectionStart+replaceString.length,selectionStart+replaceString.length);}}else if(document.selection){var range=document.selection.createRange();if(range.parentElement()==input){if(replaceValue!=undefined){var result=range.text;replaceString=lxreplaceValue(replaceValue,result);}
var isCollapsed=range.text=='';range.text=replaceString;if(!isCollapsed){range.moveStart('character',-replaceString.length);range.select();}}}}
function lxsetTab(item){var cookiei=0;if(lxGetCookie(item.name)!=null)
cookiei=parseInt(lxGetCookie(item.name).replace('px',''));var itemi=parseInt(item.style.height.replace('px',''));if(itemi<iMinHeight||cookiei<iMinHeight){itemi=iMinHeight;lxtxtUp(item);}else{item.style.height=lxGetCookie(item.name);}}
var iMinHeight=50;var iHitHeight=50;var iMinWidth=50;var iHitWidth=50;function lxtxtRt(item){var itemi=parseInt(item.style.width.replace('px',''));if(itemi>=iMinWidth){item.style.width=(itemi+iHitWidth)+'px';}else{item.style.width=iMinWidth+'px';}
lxSetCookie(item.name+'w',item.style.width);return false;}
function lxtxtLt(item){var itemi=parseInt(item.style.width.replace('px',''));if(itemi>iMinWidth){item.style.width=(itemi-iHitWidth)+'px';}else{item.style.width=iMinWidth+'px';}
lxSetCookie(item.name+'w',item.style.width);return false;}
function lxtxtUp(item){var itemi=parseInt(item.style.height.replace('px',''));if(itemi>=iMinHeight){item.style.height=(itemi+iHitHeight)+'px';}else{item.style.height=iMinHeight+'px';}
lxSetCookie(item.name,item.style.height);return false;}
function lxtxtDn(item){var itemi=parseInt(item.style.height.replace('px',''));if(itemi>iMinHeight){item.style.height=(itemi-iHitHeight)+'px';}else{item.style.height=iMinHeight+'px';}
lxSetCookie(item.name,item.style.height);return false;}
function lxtxtTb(item){lxreplaceSelection(item,String.fromCharCode(9));return false;}
function lxtxtEscape(item){lxreplaceSelection(item,'',true);return false;}
function lxtxtUnescape(item){lxreplaceSelection(item,'',false);return false;}
function lxcatchTab(item,e){if(!e)
e=window.event;c=e.which?e.which:e.keyCode;if(c==9){return lxtxtTb(item);}else if(c==40&&e.ctrlKey){return lxtxtUp(item);}else if(c==38&&e.ctrlKey){return lxtxtDn(item);}else if(c==39&&e.ctrlKey&&e.altKey){return lxtxtRt(item);}else if(c==37&&e.ctrlKey&&e.altKey){return lxtxtLt(item);}else if(c==69&&e.ctrlKey){return lxtxtEscape(item);}else if(c==82&&e.ctrlKey){return lxtxtUnescape(item);}}
var xUtilities=document.createElement('span');xUtilities.msgCtrlDown='Press	the	key	combination: [Control+DownArrow] within	the	text editor	to increase	the	height of the text content area. ';xUtilities.msgCtrlUp='Press the key combination: [Control+UpArrow] within	the	text editor	to decrease	the	height of the text content area. ';xUtilities.msgCtrlRight='Press the key combination: [Alt+Control+RightArrow] within the text editor to increase the width	of the text	content	area. ';xUtilities.msgCtrlLeft='Press	the	key	combination: [Alt+Control+LeftArrow] within	the	text editor	to decrease	the	width of the text content area.	';xUtilities.msgCtrlE='Press the key combination: [Control+E] to automatically escape (\\) the selected	value within the text area.	';xUtilities.msgCtrlR='Press the key combination: [Control+R] to automatically remove escapes (\\) from	the	selected value within the text area. ';function lxInit_RichText(requireRichtext){items=document.getElementsByTagName('TEXTAREA');var override=false;for(i=0;i<items.length;i++){rtext=items[i].getAttribute('richtext');if(requireRichtext!=null&&requireRichtext!=true){rtext=true;override=true;}
if(items[i].getAttribute('isLoaded'))
rtext=false;if(rtext){items[i].setAttribute('isLoaded','true');lxsetTab(items[i]);items[i].onkeydown=function(event){return lxcatchTab(this,event)};var dv=document.createElement('div');dv.style.border='1px solid #cccccc'
dv.style.background='#eeeeee';dv.style.width='100%';dv.style.fontFamily='arial';dv.style.fontSize='9px';dv.style.fontWeight='normal';dv.style.textAlign='center';dv.style.padding='2px';var btnstyle='onmouseover=this.style.background=\'#ffffff\' onmouseout=this.style.background=\'#cccccc\' style=\'margin-right:2px; border: 1px solid #bbbbbb;	background:	#cccccc;cursor:	pointer;\'';dv.innerHTML="";dv.innerHTML+="<a	"+btnstyle+" title='Increase the height	of this	editor.' onclick=\"alert(xUtilities.msgCtrlDown);\">Height [+]</a>";dv.innerHTML+="<a	"+btnstyle+" title='Decrease the height	of this	editor.'onclick=\"alert(xUtilities.msgCtrlUp);\">Height	[-]</a>";dv.innerHTML+="<a	"+btnstyle+" title='Increase the width of this editor.'	onclick=\"alert(xUtilities.msgCtrlRight);\">Width [+]</a>";dv.innerHTML+="<a	"+btnstyle+" title='Decrease the width of this editor.'onclick=\"alert(xUtilities.msgCtrlLeft);\">Width	[-]</a>";dv.innerHTML+="<a	"+btnstyle+" title='Escape (backslash) the selected	text.'onclick=\"alert(xUtilities.msgCtrlE);\">Escape</a>";dv.innerHTML+="<a	"+btnstyle+" title='Remove Escaping	(backslash)	from the selected text.'onclick=\"alert(xUtilities.msgCtrlR);\">Unescape</a>";dv.innerHTML+="";items[i].parentNode.appendChild(dv);}}}
function lxInit_Delete(TagType){var items=[];if(!TagType)
TagType='INPUT';items=document.getElementsByTagName(TagType);for(i=0;i<items.length;i++){if(items[i].src !=null&&items[i].src.length>0){if(items[i].src.endsWith('delete.gif')&&typeof(items[i].lxDL)=='undefined'){items[i].lxDL=true;items[i].onclick=function(){return confirm('Are you certain you want to delete	this item?');};}}}}
function lxContainerGroup(group,skipCookies){if(!document.forms[0].__VIEWSTATE){if(!skipCookies)
window.setTimeout('lxContainerGroup(\''+group+'\');',200);else
window.setTimeout('lxContainerGroup(\''+group+'\',true);',200);}else{var cki=false;var name=false;if(!skipCookies){cki=lxGetCookie('lxCon_'+group);}
if(cki){name=cki;}else{var containers=document.getElementsByTagName('lxContainer');for(var i=0;i<containers.length;i++){if(containers[i].getAttribute('Group')){if(containers[i].getAttribute('Group')==group){name=containers[i].getAttribute('Name');i=containers.length;}}}}
if(name){lxContainer(name);}}}
function lxSetClass(name,classname){if(name!=null&&name.length>0){var objs=document.getElementsByName(name);if(objs.length==0){var obj=false;obj=document.getElementById(name);if(obj){objs=new Array();objs[0]=obj;}}
for(var i=0;i<objs.length;i++){objs[i].className=classname;}}}
function lxContainer(name){
    var containers=document.getElementsByTagName('lxContainer');
    var srccontainer=false;
    for(var i=0;i<containers.length;i++){
        if(containers[i].getAttribute('Name')){
            if(containers[i].getAttribute('Name')==name){
                srccontainer=containers[i];i=containers.length;}
             }
         }
    if(srccontainer){
        var srcGroup='';
        if(srccontainer.getAttribute('Group')){srcGroup=srccontainer.getAttribute('Group');}
        for(var i=0;i<containers.length;i++){
            lxmoduleid=0;lxmoduleid=containers[i].getAttribute('ModuleId');
            if(lxmoduleid>0&&containers[i].getAttribute('Group')){
                if(containers[i].getAttribute('Group')==srcGroup){
                    var markername=containers[i].getAttribute('Marker');
                    var activemarker=containers[i].getAttribute('ActiveCss');
                    var inactivemarker=containers[i].getAttribute('InactiveCss');
                    var onInit=containers[i].getAttribute('onInit');
                    var onLoad=containers[i].getAttribute('onLoad');
                    var onUnload=containers[i].getAttribute('onUnload');
                    if(containers[i].getAttribute('Name')!=name){
                        if(containers[i].getAttribute('Loaded')&&onUnload)
                        eval(onUnload);
                        lxModule(lxmoduleid,false);
                        lxSetClass(markername,inactivemarker);}
                     else{
                        if(!containers[i].getAttribute('Loaded')){
                            containers[i].setAttribute('Loaded','true');
                            if(onInit){eval(onInit);}}
                        else if(onLoad){eval(onLoad);}
                        cki=lxSetCookie('lxCon_'+srcGroup,name);
                        lxModule(lxmoduleid,true);
                        lxSetClass(markername,activemarker);}}}}}}
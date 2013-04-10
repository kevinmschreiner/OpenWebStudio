var Browserize = false;
if (document.all)
	Browserize = true;

function getElementsByClass(searchClass,node,tag) {
	var classElements = new Array();
	if ( node == null )
		node = document;
	if ( tag == null )
		tag = '*';
	var els = node.getElementsByTagName(tag);
	var elsLen = els.length;
	var pattern = new RegExp('(^|\\s)'+searchClass+'(\\s|$)');
	for (i = 0, j = 0; i < elsLen; i++) {
		if ( pattern.test(els[i].className) ) {
			classElements[j] = els[i];
			j++;
		}
	}
	return classElements;
}


function debugTabs(target,tabs)
{
	var x = $(target);
	alert(x);
	x.innerHTML = '<TEXTAREA id=debugTabs style="width: 100%; height: 500px;" onblur="this.style.display=\'none\';"></TEXTAREA>'
	var y = $(tabs);
	$('debugTabs').value = y.innerHTML;
}


function hookEvent(element, eventName, callback)
{
  if(typeof(element) == "string")
    element = document.getElementById(element);
  if(element == null)
    return;
  if(element.addEventListener)
  {
    element.addEventListener(eventName, callback, false);
  }
  else if(element.attachEvent)
    element.attachEvent("on" + eventName, callback);
}

function unhookEvent(element, eventName, callback)
{
  if(typeof(element) == "string")
    element = document.getElementById(element);
  if(element == null)
    return;
  if(element.removeEventListener)
    element.removeEventListener(eventName, callback, false);
  else if(element.detachEvent)
    element.detachEvent("on" + eventName, callback);
}

function cancelEvent(e)
{
  e = e ? e : window.event;
  if(e.stopPropagation)
    e.stopPropagation();
  if(e.preventDefault)
    e.preventDefault();
  e.cancelBubble = true;
  e.cancel = true;
  e.returnValue = false;
  return false;
}

function Position(x, y)
{
  this.X = x;
  this.Y = y;
 
  this.Add = function(val)
  {
    var newPos = new Position(this.X, this.Y);
    if(val != null)
    {
      if(!isNaN(val.X))
        newPos.X += val.X;
      if(!isNaN(val.Y))
        newPos.Y += val.Y
    }
    return newPos;
  }
 
  this.Subtract = function(val)
  {
    var newPos = new Position(this.X, this.Y);
    if(val != null)
    {
      if(!isNaN(val.X))
        newPos.X -= val.X;
      if(!isNaN(val.Y))
        newPos.Y -= val.Y
    }
    return newPos;
  }
 
  this.Min = function(val)
  {
    var newPos = new Position(this.X, this.Y)
    if(val == null)
      return newPos;
   
    if(!isNaN(val.X) && this.X> val.X)
      newPos.X = val.X;
    if(!isNaN(val.Y) && this.Y> val.Y)
      newPos.Y = val.Y;
   
    return newPos; 
  }
 
  this.Max = function(val)
  {
    var newPos = new Position(this.X, this.Y)
    if(val == null)
      return newPos;
   
    if(!isNaN(val.X) && this.X <val.X)
      newPos.X = val.X;
    if(!isNaN(val.Y) && this.Y <val.Y)
      newPos.Y = val.Y;
   
    return newPos; 
  } 
 
  this.Bound = function(lower, upper)
  {
    var newPos = this.Max(lower);
    return newPos.Min(upper);
  }
 
  this.Check = function()
  {
    var newPos = new Position(this.X, this.Y);
    if(isNaN(newPos.X))
      newPos.X = 0;
    if(isNaN(newPos.Y))
      newPos.Y = 0;
    return newPos;
  }
 
  this.Apply = function(element)
  {
    if(typeof(element) == "string")
      element = document.getElementById(element);
    if(element == null)
      return;
    if(!isNaN(this.X))
      element.style.left = this.X + 'px';
    if(!isNaN(this.Y))
      element.style.top = this.Y + 'px'; 
  }
}

function absoluteCursorPostion(eventObj)
{
  eventObj = eventObj ? eventObj : window.event;
 
  if(isNaN(window.scrollX))
    return new Position(eventObj.clientX +
              document.documentElement.scrollLeft +
              document.body.scrollLeft,
              eventObj.clientY +
              document.documentElement.scrollTop +
              document.body.scrollTop);
  else
    return new Position(eventObj.clientX + window.scrollX,
              eventObj.clientY + window.scrollY);
}

function dragObject(element, attachElement,
    lowerBound, upperBound, startCallback,
    moveCallback, endCallback, attachLater)
{
  if(typeof(element) == "string")
    element = document.getElementById(element);
  if(element == null)
      return;

  if(lowerBound != null && upperBound != null)
  {
    var temp = lowerBound.Min(upperBound);
    upperBound = lowerBound.Max(upperBound);
    lowerBound = temp;
  }

  element.style.position = 'absolute';
  element.style.top = element.offsetTop;
  element.startPosition = element.offsetTop;

  var cursorStartPos = null;
  var elementStartPos = null;
  var dragging = false;
  var listening = false;
  var disposed = false;
 
  function dragStart(eventObj)
  {
    if(dragging || !listening || disposed) return;
    dragging = true;
   
    if(startCallback != null)
      startCallback(eventObj, element);
   
    cursorStartPos = absoluteCursorPostion(eventObj);
   
    elementStartPos = new Position(parseInt(element.style.left),
        parseInt(element.style.top));
   
    elementStartPos = elementStartPos.Check();
   
    hookEvent(document, "mousemove", dragGo);
    hookEvent(document, "mouseup", dragStopHook);
   
    return cancelEvent(eventObj);
  }
 
  function dragGo(eventObj)
  {
    if(!dragging || disposed) return;
   
    var newPos = absoluteCursorPostion(eventObj);
    newPos = newPos.Add(elementStartPos).Subtract(cursorStartPos);
    newPos = newPos.Bound(lowerBound, upperBound)
    newPos.Apply(element);
    if(moveCallback != null)
      moveCallback(newPos, element);
       
    return cancelEvent(eventObj);
  }
 
  function dragStopHook(eventObj)
  {
    dragStop();
    return cancelEvent(eventObj);
  }
 
  function dragStop()
  {
    if(!dragging || disposed) return;
    unhookEvent(document, "mousemove", dragGo);
    unhookEvent(document, "mouseup", dragStopHook);
    cursorStartPos = null;
    elementStartPos = null;
    if(endCallback != null)
      endCallback(element);
    dragging = false;
  }
 
  this.Dispose = function()
  {
    if(disposed) return;
    this.StopListening(true);
    element = null;
    attachElement = null
    lowerBound = null;
    upperBound = null;
    startCallback = null;
    moveCallback = null
    endCallback = null;
    disposed = true;
  }
 
  this.StartListening = function()
  {
    if(listening || disposed) return;
    listening = true;
    hookEvent(attachElement, "mousedown", dragStart);
  }
 
  this.StopListening = function(stopCurrentDragging)
  {
    if(!listening || disposed) return;
    unhookEvent(attachElement, "mousedown", dragStart);
    listening = false;
   
    if(stopCurrentDragging && dragging)
      dragStop();
  }
 
  this.IsDragging = function(){ return dragging; }
  this.IsListening = function() { return listening; }
  this.IsDisposed = function() { return disposed; }
  this.initialize = function() { if(moveCallback != null) moveCallback(null, element); if(endCallback != null) endCallback(element); }
 
  if(typeof(attachElement) == "string")
    attachElement = document.getElementById(attachElement);
  if(attachElement == null)
    attachElement = element;
   
  if(!attachLater)
    this.StartListening();
}
function splitterMove(eventObj,element)
{
var diff = (HSplitter.offsetTop - HList.offsetTop);
if (diff < 40)
{
	diff = 40;
	element.style.top = (HList.offsetTop + diff) + 'px';
}
element.style.left = '10px'; 
HList.style.height = diff + 'px';
}
function splitterEnd(eventObj,element)
{
HSplitter.startPosition = HSplitter.offsetTop;
}

var Bi4ce = {};

Bi4ce.GenerateGuid = function(value){
	 this._tabs = [];

	 if (value.match('^[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}$')){
		 this._tabs[0] = value.substring(0, 8).toUpperCase();
		 this._tabs[1] = value.substring(9, 13).toUpperCase();
		 this._tabs[2] = value.substring(14, 18).toUpperCase();
		 this._tabs[3] = value.substring(19, 23).toUpperCase();
		 this._tabs[4] = value.substring(24, 36).toUpperCase();
	 } 
	 else if (value.match('^{[A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}}$')){
		 this._tabs[0] = value.substring(1, 9).toUpperCase();
		 this._tabs[1] = value.substring(10, 14).toUpperCase();
		 this._tabs[2] = value.substring(15, 19).toUpperCase();
		 this._tabs[3] = value.substring(20, 24).toUpperCase();
		 this._tabs[4] = value.substring(25, 37).toUpperCase();
	 } 
	 else if (value.match('^\\([A-Za-z0-9]{8}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{4}-[A-Za-z0-9]{12}\\)$')){
		 this._tabs[0] = value.substring(1, 9).toUpperCase();
		 this._tabs[1] = value.substring(10, 14).toUpperCase();
		 this._tabs[2] = value.substring(15, 19).toUpperCase();
		 this._tabs[3] = value.substring(20, 24).toUpperCase();
		 this._tabs[4] = value.substring(25, 37).toUpperCase();
	 } 
	 else if (value.match('^[A-Za-z0-9]{32}$')){
		 this._tabs[0] = value.substring(0, 8).toUpperCase();
		 this._tabs[1] = value.substring(8, 12).toUpperCase();
		 this._tabs[2] = value.substring(12, 16).toUpperCase();
		 this._tabs[3] = value.substring(16, 20).toUpperCase();
		 this._tabs[4] = value.substring(20, 32).toUpperCase();
	 }
	 else {
		throw String.format(Sys.Res.formatInvalidString);
	 }
 }

Bi4ce.GenerateGuid.prototype = {
	 toString : function(format){
		 switch(format){
			 case 'N' :
				 return String.format('{0}{1}{2}{3}{4}', this._tabs[0], this._tabs[1], this._tabs[2], this._tabs[3], this._tabs[4]);
				 break;

			 case 'B' :
				 return String.format('{{{0}-{1}-{2}-{3}-{4}}}', this._tabs[0], this._tabs[1], this._tabs[2], this._tabs[3], this._tabs[4]);
				 break;

			 case 'P' :
				 return String.format('({0}-{1}-{2}-{3}-{4})', this._tabs[0], this._tabs[1], this._tabs[2], this._tabs[3], this._tabs[4]);
				 break;
			 case 'D' :
			 case '' :
			 case null :
			 case undefined :
				 return String.format('{0}-{1}-{2}-{3}-{4}', this._tabs[0], this._tabs[1], this._tabs[2], this._tabs[3], this._tabs[4]);
				 break;

			 default :
				 throw String.format(Sys.Res.formatInvalidString);
				 break;
		 }
	 }
 }

String.format = function()
{
    if( arguments.length == 0 )
        return null;

    var str = arguments[0];
    for(var i=1;i<arguments.length;i++)
    {
        var re = new RegExp('\\{' + (i-1) + '\\}','gm');
        str = str.replace(re, arguments[i]);
    }
    return str;
}



Bi4ce.GenerateGuid.empty = new Bi4ce.GenerateGuid('00000000-0000-0000-0000-000000000000');
Bi4ce.GenerateGuid.newGuid = function(){
 var d = new Date();
 var end = d.getTime().toString();
 for(var i = end.length; i < 32; i++){
 end += Math.floor(Math.random()*16).toString(16);
 }
 return new Bi4ce.GenerateGuid(end);
 } 
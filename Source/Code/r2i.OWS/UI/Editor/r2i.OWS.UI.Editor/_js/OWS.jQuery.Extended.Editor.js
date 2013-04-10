/*
(function($){
$.fn.texty = function(options) {

	var defaults = {
		replaceskip:new RegExp("\\\\+","g"),
		replacers=[
			{expression:new RegExp("[\"]","g"),replace:""},
			{expression:new RegExp("[\[]","g"),replace:""},
			{expression:new RegExp("[\\]]","g"),replace:""},
			{expression:new RegExp("[\{]","g"),replace:""},
			{expression:new RegExp("[\}]","g"),replace:""}
		],
		keys:[
			{control:false,shift:false,code:9,trigger:function(){this.EditorReplaceSelection(item,String.fromCharCode(9));}}
		]
	};
	var options = $.extend(defaults,options);

	return this.each(function() {
		
	});
};
})(jQuery);
*/

/*
 *	Tabby jQuery plugin version 0.12
 *
 *	Ted Devito - http://teddevito.com/demos/textarea.html
 *
 *	Copyright (c) 2009 Ted Devito
 *	 
 *	Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following 
 *	conditions are met:
 *	
 *		1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 *		2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer  
 *			in the documentation and/or other materials provided with the distribution.
 *		3. The name of the author may not be used to endorse or promote products derived from this software without specific prior written 
 *			permission. 
 *	 
 *	THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 *	IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR BE 
 *	LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
 *	PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY 
 *	THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
 *	OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *
 */
 
// create closure

(function($) {
 
	// plugin definition

	$.fn.texty = function(options) {
		//debug(this);
		// build main options before element iteration
		var opts = $.extend({}, $.fn.texty.defaults, options);
		var pressed = $.fn.texty.pressed; 
		
		// iterate and reformat each matched element
		return this.each(function() {
			$this = $(this);
			
			// build element specific options
			var options = $.meta ? $.extend({}, opts, $this.data()) : opts;
			
			$this.bind('keydown',function (e) {
				var kc = $.fn.texty.catch_kc(e);
				if (16 == kc) pressed.shft = true;
				/*
				because both CTRL+TAB and ALT+TAB default to an event (changing tab/window) that 
				will prevent js from capturing the keyup event, we'll set a timer on releasing them.
				*/
				if (17 == kc) {pressed.ctrl = true;	setTimeout("$.fn.texty.pressed.ctrl = false;",1000);}
				if (18 == kc) {pressed.alt = true; 	setTimeout("$.fn.texty.pressed.alt = false;",1000);}
					
				if (options.tabify && 9 == kc && !pressed.ctrl && !pressed.alt) {
					e.preventDefault; // does not work in O9.63 ??
					pressed.last = kc;	setTimeout("$.fn.texty.pressed.last = null;",0);
					process_keypress ($(e.target).get(0), pressed.shft, options);
					return false;
				}
				if (options.handlers!=null && options.handlers.length > 0) {
					for (var i = 0; i < options.handlers.length; i++) 
					{
						var h = options.handlers[i];
						if (typeof h.ctrl=='undefined' || h.ctrl==null) {h.ctrl=false;}
						if (typeof h.alt=='undefined' || h.alt==null) {h.alt=false;}
						if (typeof h.shft=='undefined' || h.shft==null) {h.shft=false;}
						if (h.keycode==kc && h.ctrl==pressed.ctrl && h.shft==pressed.shft && h.alt==pressed.alt  ) {
							pressed.last = kc; setTimeout("$.fn.texty.pressed.last = null;",0);
							var b = true;
							try {
								h.trigger($(e.target).get(0), pressed.shft, options);
							} catch(ex) {}
							if (!b) {
								e.preventDefault;
							}
							return b;
						}
					}
				}
				
			}).bind('keyup',function (e) {
				if (16 == $.fn.texty.catch_kc(e)) pressed.shft = false;
			}).bind('blur',function (e) { // workaround for Opera -- http://www.webdeveloper.com/forum/showthread.php?p=806588
				if (options.tabify && 9 == pressed.last) $(e.target).one('focus',function (e) {pressed.last = null;}).get(0).focus();
			});
		
		});
	};
	
	// define and expose any extra methods
	$.fn.texty.catch_kc = function(e) { return e.keyCode ? e.keyCode : e.charCode ? e.charCode : e.which; };
	$.fn.texty.pressed = {shft : false, ctrl : false, alt : false, last: null};
	
	// private function for debugging
	function debug($obj) {
		if (window.console && window.console.log)
		window.console.log('textarea count: ' + $obj.size());
	};

	function process_keypress (o,shft,options) {
		var scrollTo = o.scrollTop;
		//var tabString = String.fromCharCode(9);
		
		// gecko; o.setSelectionRange is only available when the text box has focus
		if (o.setSelectionRange) gecko_tab (o, shft, options);
		
		// ie; document.selection is always available
		else if (document.selection) ie_tab (o, shft, options);
		
		o.scrollTop = scrollTo;
	}
	
	// plugin defaults
	$.fn.texty.defaults = {tabString : String.fromCharCode(9),handlers:[],tabify:true};
	$.fn.texty.insert = function(o,t) {
		o.focus();
		process_keypress(o,false,{tabString:t});
	}
	function gecko_tab (o, shft, options) {
		var ss = o.selectionStart;
		var es = o.selectionEnd;	
		var tabString = options.tabString;
		
		// when there's no selection and we're just working with the caret, we'll add/remove the tabs at the caret, providing more control
		if(ss == es) {
			// SHIFT+TAB
			if (shft) {
				// check to the left of the caret first
				if ("\t" == o.value.substring(ss-tabString.length, ss)) {
					o.value = o.value.substring(0, ss-tabString.length) + o.value.substring(ss); // put it back together omitting one character to the left
					o.focus();
					o.setSelectionRange(ss - tabString.length, ss - tabString.length);
				} 
				// then check to the right of the caret
				else if ("\t" == o.value.substring(ss, ss + tabString.length)) {
					o.value = o.value.substring(0, ss) + o.value.substring(ss + tabString.length); // put it back together omitting one character to the right
					o.focus();
					o.setSelectionRange(ss,ss);
				}
			}
			// TAB
			else {			
				o.value = o.value.substring(0, ss) + tabString + o.value.substring(ss);
				o.focus();
	    		o.setSelectionRange(ss + tabString.length, ss + tabString.length);
			}
		} 
		// selections will always add/remove tabs from the start of the line
		else {
			// split the textarea up into lines and figure out which lines are included in the selection
			var lines = o.value.split("\n");
			var indices = new Array();
			var sl = 0; // start of the line
			var el = 0; // end of the line
			var sel = false;
			for (var i in lines) {
				el = sl + lines[i].length;
				indices.push({start: sl, end: el, selected: (sl <= ss && el > ss) || (el >= es && sl < es) || (sl > ss && el < es)});
				sl = el + 1;// for "\n"
			}
			
			// walk through the array of lines (indices) and add tabs where appropriate						
			var modifier = 0;
			for (var i in indices) {
				if (indices[i].selected) {
					var pos = indices[i].start + modifier; // adjust for tabs already inserted/removed
					// SHIFT+TAB
					if (shft && tabString == o.value.substring(pos,pos+tabString.length)) { // only SHIFT+TAB if there's a tab at the start of the line
						o.value = o.value.substring(0,pos) + o.value.substring(pos + tabString.length); // omit the tabstring to the right
						modifier -= tabString.length;
					}
					// TAB
					else if (!shft) {
						o.value = o.value.substring(0,pos) + tabString + o.value.substring(pos); // insert the tabstring
						modifier += tabString.length;
					}
				}
			}
			o.focus();
			var ns = ss + ((modifier > 0) ? tabString.length : (modifier < 0) ? -tabString.length : 0);
			var ne = es + modifier;
			o.setSelectionRange(ns,ne);
		}
	}
	
	function ie_tab (o, shft, options) {
		var range = document.selection.createRange();
		var tabString = options.tabString;		
		if (o == range.parentElement()) {
			// when there's no selection and we're just working with the caret, we'll add/remove the tabs at the caret, providing more control
			if ('' == range.text) {
				// SHIFT+TAB
				if (shft) {
					var bookmark = range.getBookmark();
					//first try to the left by moving opening up our empty range to the left
				    range.moveStart('character', -tabString.length);
				    if (tabString == range.text) {
				    	range.text = '';
				    } else {
				    	// if that didn't work then reset the range and try opening it to the right
				    	range.moveToBookmark(bookmark);
				    	range.moveEnd('character', tabString.length);
				    	if (tabString == range.text) 
				    		range.text = '';
				    }
				    // move the pointer to the start of them empty range and select it
				    range.collapse(true);
					range.select();
				}
				
				else {
					// very simple here. just insert the tab into the range and put the pointer at the end
					range.text = tabString; 
					range.collapse(false);
					range.select();
				}
			}
			// selections will always add/remove tabs from the start of the line
			else {
			
				var selection_text = range.text;
				var selection_len = selection_text.length;
				var selection_arr = selection_text.split("\r\n");
				
				var before_range = document.body.createTextRange();
				before_range.moveToElementText(o);
				before_range.setEndPoint("EndToStart", range);
				var before_text = before_range.text;
				var before_arr = before_text.split("\r\n");
				var before_len = before_text.length; // - before_arr.length + 1;
				
				var after_range = document.body.createTextRange();
				after_range.moveToElementText(o);
				after_range.setEndPoint("StartToEnd", range);
				var after_text = after_range.text; // we can accurately calculate distance to the end because we're not worried about MSIE trimming a \r\n
				
				var end_range = document.body.createTextRange();
				end_range.moveToElementText(o);
				end_range.setEndPoint("StartToEnd", before_range);
				var end_text = end_range.text; // we can accurately calculate distance to the end because we're not worried about MSIE trimming a \r\n
								
				var check_html = $(o).html();
				$("#r3").text(before_len + " + " + selection_len + " + " + after_text.length + " = " + check_html.length);				
				if((before_len + end_text.length) < check_html.length) {
					before_arr.push("");
					before_len += 2; // for the \r\n that was trimmed	
					if (shft && tabString == selection_arr[0].substring(0,tabString.length))
						selection_arr[0] = selection_arr[0].substring(tabString.length);
					else if (!shft) selection_arr[0] = tabString + selection_arr[0];	
				} else {
					if (shft && tabString == before_arr[before_arr.length-1].substring(0,tabString.length)) 
						before_arr[before_arr.length-1] = before_arr[before_arr.length-1].substring(tabString.length);
					else if (!shft) before_arr[before_arr.length-1] = tabString + before_arr[before_arr.length-1];
				}
				
				for (var i = 1; i < selection_arr.length; i++) {
					if (shft && tabString == selection_arr[i].substring(0,tabString.length))
						selection_arr[i] = selection_arr[i].substring(tabString.length);
					else if (!shft) selection_arr[i] = tabString + selection_arr[i];
				}
				
				if (1 == before_arr.length && 0 == before_len) {
					if (shft && tabString == selection_arr[0].substring(0,tabString.length))
						selection_arr[0] = selection_arr[0].substring(tabString.length);
					else if (!shft) selection_arr[0] = tabString + selection_arr[0];
				}

				if ((before_len + selection_len + after_text.length) < check_html.length) {
					selection_arr.push("");
					selection_len += 2; // for the \r\n that was trimmed
				}
				
				before_range.text = before_arr.join("\r\n");
				range.text = selection_arr.join("\r\n");
				
				var new_range = document.body.createTextRange();
				new_range.moveToElementText(o);
				
				if (0 < before_len)	new_range.setEndPoint("StartToEnd", before_range);
				else new_range.setEndPoint("StartToStart", before_range);
				new_range.setEndPoint("EndToEnd", range);
				
				new_range.select();
				
			} 
		}
	}

// end of closure
})(jQuery);


/*
this.EditorReplaceSelection = function (input,replaceString,replaceValue,itemCursor){
		if (itemCursor==null)
				itemCursor = 0;	
		if (!isNaN(input))
			input = this.Editors[input].Editor();
	input.focus();		
		if(input.setSelectionRange)
			{	
				var selectionStart=input.selectionStart;
				var selectionEnd=input.selectionEnd;
				var scrollTop=input.scrollTop;
				if(replaceValue!=undefined&&selectionEnd>selectionStart){
					var result=input.value.substring(selectionStart,selectionEnd);
					replaceString=this.EditorReplace(replaceValue,result);
				}
				input.value=input.value.substring(0,selectionStart)+replaceString+input.value.substring(selectionEnd);
				input.scrollTop=scrollTop;
				if(selectionStart !=selectionEnd){
					this.EditorSelectionRange(input,selectionStart,selectionStart+replaceString.length-itemCursor);
				}
				else
				{
					this.EditorSelectionRange(input,selectionStart+replaceString.length,selectionStart+replaceString.length-itemCursor);
				}
			}
		else if(document.selection)
				{ 
					var range=document.selection.createRange();
					if(range.parentElement()==input)
					{
						if(replaceValue!=undefined)
						{
							var result=range.text;
							replaceString=this.EditorReplace(replaceValue,result);
						}
						var isCollapsed=range.text=='';
						range.text=replaceString;
						if(!isCollapsed){
							range.moveStart('character',-replaceString.length-itemCursor);range.select();
						}
					}
				}
	}	
	this.EditorGetSelection = function (input){
	
		if (!isNaN(input))
			input = this.Editors[input].Editor();
	input.focus();		
		if(input.setSelectionRange)
			{	
				var selectionStart=input.selectionStart;
				var selectionEnd=input.selectionEnd;
				var scrollTop=input.scrollTop;
					var result=input.value.substring(selectionStart,selectionEnd);
					return result;
			}
		else if(document.selection)
				{ 
					var range=document.selection.createRange();
					if(range.parentElement()==input)
					{
							var result=range.text;
							return result;
					}
				}
	}	
	this.EditorReplace = function (replaceValue,input)
		{
			if (!isNaN(input))
				input = this.Editors[input].Editor();
				
			var result=input;
			if(replaceValue==true)
			{	var re=new RegExp("\\\\+","g");var s='';var lindex=0;var m=re.exec(result);
				while(m!=null){
					if(m.index>lindex)
					{s=s+result.substring(lindex,m.index);}
					s=s+result.substring(m.index,m.index+m.length);
					lindex=m.index+m.length;
					m=re.exec(result);
				}
				if(lindex<result.length)
					{s=s+result.substring(lindex);}
				result=s;
				re=new RegExp("[\"]","g");
				result=result.replace(re,"\\\"");
				re=new RegExp("[\[]","g");
				result=result.replace(re,"\\[");
				re=new RegExp("[\\]]","g");
				result=result.replace(re,"\\]");
				re=new RegExp("[\{]","g");
				result=result.replace(re,'\\{');
				re=new RegExp("[\}]","g");
				result=result.replace(re,'\\}');
			}
			else
			{
				var re=new RegExp("\\\\+","g");
				var s='';
				var lindex=0;
				var m=re.exec(result);
				while(m!=null){
					if(m.index>lindex)
						{s=s+result.substring(lindex,m.index);}
					s=s+result.substring(m.index,m.index+m.length-1);lindex=m.index+m.length;m=re.exec(result);
				}
				if(lindex<result.length){s=s+result.substring(lindex);}
				result=s;
			}
		return result;
	}
	this.EditorSelectionRange = function(input,selectionStart,selectionEnd)
	{
		if (!isNaN(input))
			input = this.Editors[input].Editor();
		if(input.setSelectionRange)
		{
			input.focus();
			input.setSelectionRange(selectionStart,selectionEnd);
		}
		else if(input.createTextRange)
		{
			var range=input.createTextRange();
			range.collapse(true);
			range.moveEnd('character',selectionEnd);
			range.moveStart('character',selectionStart);
			range.select();
		}
	}	
	this.EditorKeyDown = function (item,e)
	{
	
			if(!e)
				e=window.event;
			c=e.which?e.which:e.keyCode;
			if(c==9){
				return this.EditorKeyDownHandler(item,'TAB');}
			else if(c==40&&e.ctrlKey){
				return this.EditorKeyDownHandler(item,'UP');}
			else if(c==38&&e.ctrlKey){
				return this.EditorKeyDownHandler(item,'DOWN');}
			else if(c==39&&e.ctrlKey&&e.altKey){
				return this.EditorKeyDownHandler(item,'RIGHT');}
			else if(c==37&&e.ctrlKey&&e.altKey){
				return this.EditorKeyDownHandler(item,'LEFT');}
			else if(c==69&&e.ctrlKey){
				return this.EditorKeyDownHandler(item,'ESCAPE');}
			else if(c==82&&e.ctrlKey){
				return this.EditorKeyDownHandler(item,'UNESCAPE');}
			else if(c==192&&e.ctrlKey){
				return this.EditorKeyDownHandler(item,'AUTOPASTE');}
			else if(c==192&&e.altKey){
				return this.EditorKeyDownHandler(item,'AUTOCOPY');}
		}
*/	
var Page_IsSmartExecuted = false; 
var Page_ValidationActive = false; 
function Page_SmartValidate(groupname) {
   var i; 
   Page_SmartClientValidate_BuildValidators(groupname); 
   var result = Page_ClientValidate(groupname); 
   return result; 
   }
function Page_SmartClientValidate_BuildValidators(groupname) {
   if(typeof(window.Page_Validators) == "undefined")window.Page_Validators = new Array(); 
   if(typeof(window.Page_ValidationSummaries) == "undefined")window.Page_ValidationSummaries = new Array(); 
   var pgSValidatorI = 0; 
   var pgSSummaryI = 0; 
   var pgSItems = false; 
   var pgSAttribute = false; 
   Page_IsSmartExecuted = true; 
   Page_Validators = new Array(); 
   pgSItems = document.getElementsByTagName('SPAN'); 
   for(i = 0; i < pgSItems.length; i++) {
      pgSAttribute = false; 
      pgSAttribute = pgSItems[i].getAttribute('controltovalidate'); 
      if(pgSAttribute) {
         Page_Validators[pgSValidatorI] = pgSItems[i]; 
         pgSValidatorI++; 
         }
      else {
         pgSAttribute = false; 
         pgSAttribute = pgSItems[i].getAttribute('showsummary'); 
         if(pgSAttribute) {
            Page_ValidationSummaries[pgSSummaryI] = pgSItems[i]; 
            pgSSummaryI++; 
            }
         else {
            pgSAttribute = false; 
            pgSAttribute = pgSItems[i].getAttribute('showmessagebox'); 
            if(pgSAttribute) {
               Page_ValidationSummaries[pgSSummaryI] = pgSItems[i]; 
               pgSSummaryI++; 
               }
            }
         }
      }
   ValidatorOnLoad(); 
   if(typeof(ValidatorOnSubmit) == 'undefined') {
      window.ValidatorOnSubmit = new Function("e", "if (Page_ValidationActive) { return ValidatorCommonOnSubmit(e); } return true;"); 
      }
   }
var Page_ValidationVer = "125"; 
var Page_IsValid = true; 
var Page_BlockSubmit = false; 
function ValidatorUpdateDisplay(val) {
   if(typeof(val.display) == "string") {
      if(val.display == "None") {
         return; 
         }
      if(val.display == "Dynamic") {
         val.style.display = val.isvalid ? "none" : "inline"; 
         return; 
         }
      }
   val.style.visibility = val.isvalid ? "hidden" : "visible"; 
   }
function ValidatorUpdateIsValid() {
   var i; 
   for(i = 0; i < Page_Validators.length; i++) {
      if(!Page_Validators[i].isvalid) {
         Page_IsValid = false; 
         return; 
         }
      }
   Page_IsValid = true; 
   }
function ValidatorHookupControlID(controlID, val) {
   if(typeof(controlID) == 'undefined' || controlID==null || typeof(controlID) != "string") {
      return; 
      }
   var ctrl = document.getElementById(controlID); 
   if(typeof(ctrl) != "undefined") {
      ValidatorHookupControl(ctrl, val); 
      }
   else {
      val.isvalid = true; 
      val.enabled = false; 
      }
   }
function ValidatorHookupControl(control, val) {
   if (typeof(control)!="undefined"&&control!=null)
   {
       if(typeof(control.tagName) == "undefined" && typeof(control.length) == "number") {
          var i; 
          for(i = 0; i < control.length; i++) {
             var inner = control[i]; 
             if(typeof(inner.value) == "string") {
                ValidatorHookupControl(inner, val); 
                }
             }
          return; 
          }
       else if(control.tagName != "INPUT" && control.tagName != "TEXTAREA" && control.tagName != "SELECT") {
          var i; 
          for(i = 0; i < control.children.length; i++) {
             ValidatorHookupControl(control.children[i], val); 
             }
          return; 
          }
       else {
          if(typeof(control.Validators) == "undefined") {
             control.Validators = new Array; 
             var ev; 
             if(control.type == "radio") {
                ev = control.onclick; 
                }
             else {
                ev = control.onchange; 
                }
             if(typeof(ev) == "function") {
                ev = ev.toString(); 
                ev = ev.substring(ev.indexOf("{") + 1, ev.lastIndexOf("}")); 
                }
             else {
                ev = ""; 
                }
             var func = new Function("event", "ValidatorOnChange(event); " + ev); 
             if(control.type == "radio") {
                control.onclick = func; 
                }
             else {
                control.onchange = func; 
                }
             }
          control.Validators[control.Validators.length] = val; 
          }
        }
        else
            return;
   }
function ValidatorGetValue(id) {
   var control; 
   control = document.getElementById(id); 
   if(typeof(control.value) == "string") {
      return control.value; 
      }
   if(typeof(control.tagName) == "undefined" && typeof(control.length) == "number") {
      var j; 
      for(j = 0; j < control.length; j++) {
         var inner = control[j]; 
         if(typeof(inner.value) == "string" && (inner.type != "radio" || inner.status == true)) {
            return inner.value; 
            }
         }
      }
   else {
      return ValidatorGetValueRecursive(control); 
      }
   return ""; 
   }
function ValidatorGetValueRecursive(control) {
   if(typeof(control.value) == "string" && (control.type != "radio" || control.status == true)) {
      return control.value; 
      }
   var i, val; 
   for(i = 0; i < control.children.length; i++) {
      val = ValidatorGetValueRecursive(control.children[i]); 
      if(val != "")return val; 
      }
   return ""; 
   }
function Page_ClientValidate(groupname) {
   if(typeof(window.Page_Validators) == "undefined")window.Page_Validators = new Array(); 
   var i; 
   for(i = 0; i < Page_Validators.length; i++) {
      if(groupname != undefined && groupname != null) {
         var xval = Page_Validators[i]; 
         if(xval.group == groupname) {
            ValidatorValidate(Page_Validators[i]); 
            }
         else {
            xval.isvalid = true; 
            }
         }
      else {
         ValidatorValidate(Page_Validators[i]); 
         }
      }
   ValidatorUpdateIsValid(); 
   ValidationSummaryOnSubmit(); 
   Page_BlockSubmit =!Page_IsValid; 
   return Page_IsValid; 
   }
function ValidatorCommonOnSubmit(e) {
   var result =!Page_BlockSubmit; 
   Page_BlockSubmit = false; 
   if(!e) {
        if (typeof event != 'undefined' && event != null)
            event.returnValue = result; 
      }
   else {
      if(!result)e.preventDefault(); 
      }
   return result; 
   }
function ValidatorEnable(val, enable) {
   val.enabled = (enable != false); 
   ValidatorValidate(val); 
   ValidatorUpdateIsValid(); 
   }
function ValidatorOnChange(e) {
   if(!e) {
      var vals = event.srcElement.Validators; 
      }
   else {
      var vals = e.target.Validators; 
      }
   var i; 
   for(i = 0; i < vals.length; i++) {
      ValidatorValidate(vals[i]); 
      }
   ValidatorUpdateIsValid(); 
   }
function ValidatorValidate(val) {
   val.isvalid = true; 
   if(val.enabled != false) {
      if(typeof(val.evaluationfunction) == "function") {
         val.isvalid = val.evaluationfunction(val); 
         }
      }
   ValidatorUpdateDisplay(val); 
   }
function ValidatorOnLoad() {
   if(typeof(Page_Validators) == "undefined")return; 
   var i, val; 
   for(i = 0; i < Page_Validators.length; i++) {
      val = Page_Validators[i]; 
      val.evaluationfunction = val.getAttribute('evaluationfunction'); 
      val.errormessage = val.getAttribute('errormessage'); 
      val.validationexpression = val.getAttribute('validationexpression'); 
      val.controltovalidate = val.getAttribute('controltovalidate'); 
      val.controlhookup = val.getAttribute('controlhookup'); 
      val.initialvalue = val.getAttribute('initialvalue'); 
      val.display = val.getAttribute('display'); 
      val.minimumvalue = val.getAttribute('minimumvalue'); 
      val.maximumvalue = val.getAttribute('maximumvalue'); 
      val.enabled = val.getAttribute('enabled'); 
      val.century = val.getAttribute('century'); 
      val.cutoffyear = val.getAttribute('cutoffyear'); 
      val.decimalchar = val.getAttribute('decimalchar'); 
      val.groupchar = val.getAttribute('groupchar'); 
      val.digits = val.getAttribute('digits'); 
      val.group = val.getAttribute('group'); 
      val.dateorder = val.getAttribute('dateorder'); 
      val.controltocompare = val.getAttribute('controltocompare'); 
      val.valuetocompare = val.getAttribute('valuetocompare'); 
      val.operator = val.getAttribute('operator'); 
      val.clientvalidationfunction = val.getAttribute('clientvalidationfunction'); 
      if(typeof(val.type) == "undefined") {
         val.type = val.getAttribute('type'); 
         }
      if(typeof(val.evaluationfunction) == "string") {
         eval("val.evaluationfunction = " + val.evaluationfunction + ";"); 
         }
      if(typeof(val.isvalid) == "string") {
         if(val.isvalid == "False") {
            val.isvalid = false; 
            Page_IsValid = false; 
            }
         else {
            val.isvalid = true; 
            }
         }
      else {
         val.isvalid = true; 
         }
      if(typeof(val.enabled) == "string") {
         val.enabled = (val.enabled != "False"); 
         }
      ValidatorHookupControlID(val.controltovalidate, val); 
      ValidatorHookupControlID(val.controlhookup, val); 
      }
   Page_ValidationActive = true; 
   }
function ValidatorConvert(op, dataType, val) {
   function GetFullYear(year) {
      return(year + parseInt(val.century)) - ((year < val.cutoffyear) ? 0 : 100); 
      }
   var num, cleanInput, m, exp; 
   if(dataType == "Integer") {
      exp = /^\s*[-\+]?\d+\s*$/; 
      if(op.match(exp) == null)return null; 
      num = parseInt(op, 10); 
      return(isNaN(num) ? null : num); 
      }
   else if(dataType == "Double") {
      var decimalchar = val.decimalchar != undefined ? val.decimalchar : "."; 
      exp = new RegExp("^\\s*([-\\+])?(\\d+)?(\\" + decimalchar + "(\\d+))?\\s*$"); 
      m = op.match(exp); 
      // msft version
      //if (m == null)
      // return null;
      // new version
      if (m == null || op.length == 0) return null; 
      // msft version
      //cleanInput = m[1] + (m[2].length>0 ? m[2] : "0") + "." + m[4];
      // new version
      cleanInput = (m[1] != undefined ? m[1] : "") + (m[2] != undefined && m[2].length > 0 ? m[2] : "0") + "." + (m[4] != undefined ? m[4] : ""); 
      num = parseFloat(cleanInput); 
      return (isNaN(num) ? null : num); 
      }
   else if (dataType == "Currency") {
      exp = new RegExp("^\\s*([-\\+])?(((\\d+)\\" + val.groupchar + ")*)(\\d+)" + ((val.digits > 0) ? "(\\" + val.decimalchar + "(\\d{1," + val.digits + "}))?" : "") + "\\s*$"); 
      m = op.match(exp); 
      if (m == null) return null; 
      var intermed = m[2] + m[5] ; 
      cleanInput = m[1] + intermed.replace(new RegExp("(\\" + val.groupchar + ")", "g"), "") + ((val.digits > 0) ? "." + m[7] : 0); 
      num = parseFloat(cleanInput); 
      return(isNaN(num) ? null : num); 
      }
   else if(dataType == "Date") {
		try {
  		  var yearFirstExp = new RegExp("^\\s*((\\d{4})|(\\d{2}))([-/]|\\. ?)(\\d{1,2})\\4(\\d{1,2})\\s*$"); 
		  m = op.match(yearFirstExp); 
		  var day, month, year; 
		  
		  if(m != null && typeof m[2] != 'undefined' && typeof m[3] != 'undefined' &&  typeof m[5] != 'undefined' &&  typeof m[6] != 'undefined' &&   (m[2].length == 4 || val.dateorder == "ymd")) {
			 day = m[6]; 
			 month = m[5]; 
			 year = (m[2].length == 4) ? m[2] : GetFullYear(parseInt(m[3], 10))}
		  else {
			 if(val.dateorder == "ymd") {
				return null; 
				}
			 var yearLastExp = new RegExp("^\\s*(\\d{1,2})([-/]|\\. ?)(\\d{1,2})\\2((\\d{4})|(\\d{2}))\\s*$"); 
			 m = op.match(yearLastExp); 
			 if(m == null) {
				return null; 
				}
			 if (typeof m[1] == 'undefined' || typeof m[3] == 'undefined'  || (typeof m[5] == 'undefined' && typeof m[6] == 'undefined'))
				return null;
			 if(val.dateorder == "mdy") {
				day = m[3]; 
				month = m[1]; 
				}
			 else {
				day = m[1]; 
				month = m[3]; 
				}
				
			 year = (m[5].length == 4) ? m[5] : GetFullYear(parseInt(m[6], 10))}
		  month -= 1; 
		  var date = new Date(year, month, day); 
		  return(typeof(date) == "object" && year == date.getFullYear() && month == date.getMonth() && day == date.getDate()) ? date.valueOf() : null; 
		}
		catch(ex)
		{
			return null;
		}		  
      }
   else {
      return op.toString(); 
      }
   }
function ValidatorCompare(operand1, operand2, operator, val) {
   var dataType = val.type; 
   var op1, op2; 
   if(operand1 != null)op1 = ValidatorConvert(operand1, dataType, val); 
   if(operand2 != null)op2 = ValidatorConvert(operand2, dataType, val); 
   if(op1 == null)return false; 
   if(operator == "DataTypeCheck")return true; 
   if(op2 == null)return true; 
   switch(operator) {
      case "NotEqual" : return(op1 != op2); 
      case "GreaterThan" : return(op1 > op2); 
      case "GreaterThanEqual" : return(op1 >= op2); 
      case "LessThan" : return(op1 < op2); 
      case "LessThanEqual" : return(op1 <= op2); 
      default : return(op1 == op2); 
      }
   }
function CompareValidatorEvaluateIsValid(val) {
   var value = ValidatorGetValue(val.controltovalidate); 
   if(ValidatorTrim(value).length == 0)return true; 
   var compareTo = ""; 
   if(null == document.getElementById(val.controltocompare)) {
      if(typeof(val.valuetocompare) == "string") {
         compareTo = val.valuetocompare; 
         }
      }
   else {
      compareTo = ValidatorGetValue(val.controltocompare); 
      }
   return ValidatorCompare(value, compareTo, val.operator, val); 
   }
function CustomValidatorEvaluateIsValid(val) {
   var value = ""; 
   if(typeof(val.controltovalidate) == "string") {
      value = ValidatorGetValue(val.controltovalidate); 
      if(ValidatorTrim(value).length == 0)return true; 
      }
   var args = {
      Value : value, IsValid : true}; 
   if(typeof(val.clientvalidationfunction) == "string") {
      eval(val.clientvalidationfunction + "(val, args) ;"); 
      }
   return args.IsValid; 
   }
function RegularExpressionValidatorEvaluateIsValid(val) {
   var value = ValidatorGetValue(val.controltovalidate); 
   if(ValidatorTrim(value).length == 0)return true; 
   var rx = new RegExp(val.validationexpression); 
   var matches = rx.exec(value); 
   return(matches != null && value == matches[0]); 
   }
function ValidatorTrim(s) {
   var m = s.match(/^\s*(\S+(\s+\S+)*)\s*$/);
   return(m == null) ? "" : m[1]; 
   }
function RequiredFieldValidatorEvaluateIsValid(val) {
   return(ValidatorTrim(ValidatorGetValue(val.controltovalidate)) != ValidatorTrim(val.initialvalue))}
function RangeValidatorEvaluateIsValid(val) {
   var value = ValidatorGetValue(val.controltovalidate); 
   if(ValidatorTrim(value).length == 0) {
      return true; 
      }
   return(ValidatorCompare(value, val.minimumvalue, "GreaterThanEqual", val) && ValidatorCompare(value, val.maximumvalue, "LessThanEqual", val)); 
   }
function ValidationSummaryOnSubmit() {
   if(typeof(Page_ValidationSummaries) == "undefined")return; 
   var summary, sums, s; 
   for(sums = 0; sums < Page_ValidationSummaries.length; sums++) {
      summary = Page_ValidationSummaries[sums]; 
      summary.style.display = "none"; 
      if(!Page_IsValid) {
         if(typeof(summary.showsummary) == "undefined" && typeof(summary.getAttribute('showsummary'))) {
            summary.showsummary = summary.getAttribute('showsummary'); 
            summary.displaymode = summary.getAttribute('displaymode'); 
            summary.headertext = summary.getAttribute('headertext')}
         if(typeof(summary.showmessagebox) == "undefined" && typeof(summary.getAttribute('showmessagebox'))) {
            summary.showmessagebox = summary.getAttribute('showmessagebox'); 
            summary.displaymode = summary.getAttribute('displaymode'); 
            summary.headertext = summary.getAttribute('headertext')}
         if(summary.showsummary != "False") {
            summary.style.display = ""; 
            if(typeof(summary.displaymode) != "string") {
               summary.displaymode = "BulletList"; 
               }
            switch(summary.displaymode) {
               case "List" : headerSep = "<br>"; 
               first = ""; 
               pre = ""; 
               post = "<br>"; 
               finaltext = ""; 
               break; 
               case "BulletList" : default : headerSep = ""; 
               first = "<ul>"; 
               pre = "<li>"; 
               post = "</li>"; 
               finaltext = "</ul>"; 
               break; 
               case "SingleParagraph" : headerSep = " "; 
               first = ""; 
               pre = ""; 
               post = " "; 
               finaltext = "<br>"; 
               break; 
               }
            s = ""; 
            if(typeof(summary.headertext) == "string") {
               s += summary.headertext + headerSep; 
               }
            s += first; 
            for(i = 0; i < Page_Validators.length; i++) {
               if(!Page_Validators[i].isvalid && typeof(Page_Validators[i].errormessage) == "string") {
                  s += pre + Page_Validators[i].errormessage + post; 
                  }
               }
            s += finaltext; 
            summary.innerHTML = s; 
            window.scrollTo(0, 0); 
            }
         if(summary.showmessagebox == "True") {
            s = ""; 
            if(typeof(summary.headertext) == "string") {
               s += summary.headertext + "<BR>"; 
               }
            for(i = 0; i < Page_Validators.length; i++) {
               if(!Page_Validators[i].isvalid && typeof(Page_Validators[i].errormessage) == "string") {
                  switch(summary.displaymode) {
                     case "List" : s += Page_Validators[i].errormessage + "<BR>"; 
                     break; 
                     case "BulletList" : default : s += " - " + Page_Validators[i].errormessage + "<BR>"; 
                     break; 
                     case "SingleParagraph" : s += Page_Validators[i].errormessage + " "; 
                     break; 
                     }
                  }
               }
            span = document.createElement("SPAN"); 
            span.innerHTML = s; 
            s = span.innerText; 
            alert(s); 
            }
         }
      }
   }
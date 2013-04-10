/*General Object Manipulations*/
Array.prototype.filter = function (f, me) {
    if (typeof me == 'undefined')
    { me = false; }
    var that = [];
    if (this.length > 0) {
        var min = me ? this.length : 0;
        var max = me ? 0 : this.length;
        var inc = me ? -1 : 1;
        for (var i = min; i != max; i = i + inc) {
            var b = false;
            if (typeof f == 'function') {
                b = f(this[i]);
            }
            else {
                b = true;
                for (var name in f) {
                    if ((this[i])[name] != f[name])
                    { b = false; }
                }
            }
            if (b) {
                if (me) {
                    this.splice(i, 1);
                } else {
                    that.push(this[i]);
                }
            }
        }
    }
    return (me ? this : that);
};
Array.prototype.sortBy = function (type, name, direction) {
    if (typeof type != 'undefined' && typeof type.push != 'undefined' && type.length == 3) {
        name = type[1];
        direction = type[2];
        type = type[0];
    }
    if (typeof direction == 'undefined' || direction == null || direction.length == 0) { return this };
    var f = null;
    var lpa = 'a[';
    var rpa = ']';
    var lpb = 'b[';
    var rpb = ']';
    if (name.indexOf('.') > 0) {
        lpa = 'GetChainObjectValue(a,';
        lpb = 'GetChainObjectValue(b,';
        rpa = ')';
        rpb = ')';
    }
    switch (type.toLowerCase()) {
        case 'number':
        case 'date':
            f = new Function("a", "b", "return " + lpa + "'" + name + "'" + rpa + "-" + lpb + "'" + name + "'" + rpb + ";");
            break;
        case 'boolean':
            f = new Function("a", "b", "a=" + lpa + "'" + name + "'" + rpa + "; b=" + lpb + "'" + name + "'" + rpb + "; if (a==null) {a=false;} if (b==null) {b=false;} return (a?0:1)-(b?0:1);");
            break;
        default:
            f = new Function("a", "b", "return (" + lpa + "'" + name + "'" + rpa + ".toLowerCase()<=" + lpb + "'" + name + "'" + rpb + ".toLowerCase()?(" + lpa + "'" + name + "'" + rpa + ".toLowerCase()==" + lpb + "'" + name + "'" + rpb + ".toLowerCase()?0:-1):1)");
    }
    this.sort(f);
    var direction = direction.toLowerCase()[0];
    if (direction == 'd') { this.reverse() }
    f = null;
    return this;
}

var hoverintent_config = ($.hoverintent = {
    sensitivity: 7,
    interval: 300
});
$.event.special.hoverintent = {
    setup: function () {
        $(this).bind("mouseover", jQuery.event.special.hoverintent.handler);
    },
    teardown: function () {
        $(this).unbind("mouseover", jQuery.event.special.hoverintent.handler);
    },
    handler: function (event) {
        event.type = "hoverintent";
        var self = this,
					args = arguments,
					target = $(event.target),
					cX, cY, pX, pY;

        function track(event) {
            cX = event.pageX;
            cY = event.pageY;
        };
        pX = event.pageX;
        pY = event.pageY;
        function clear() {
            target
						.unbind("mousemove", track)
						.unbind("mouseout", arguments.callee);
            clearTimeout(timeout);
        }
        function handler() {
            if ((Math.abs(pX - cX) + Math.abs(pY - cY)) < hoverintent_config.sensitivity) {
                clear();
                jQuery.event.handle.apply(self, args);
            } else {
                pX = cX;
                pY = cY;
                timeout = setTimeout(handler, hoverintent_config.interval);
            }
        }
        var timeout = setTimeout(handler, hoverintent_config.interval);
        target.mousemove(track).mouseout(clear);
        return true;
    }
};
/*---*/
/* Global Functions */

function GetChainObjectValue(o, p) {
    try {
        var s = p.split('.');
        for (var i = 0; i < s.length; i++) {
            o = o[s[i]];
        }
    } catch (ex) {
    }
    return o;
}
function FormatDouble(v) {
    return ows.Utilities.Format.Number(v, 1, ',', '.', '', '', '-', '');
}
function FormatSingle(v) {
    return ows.Utilities.Format.Number(v * 100, 0, ',', '', '', '', '-', '');
}
/*---*/

if (typeof ows == 'undefined' || ows == null) { ows = {} };
if (typeof openwebstudio == 'undefined' || openwebstudio == null) { openwebstudio = ows };
/*
function OWSConfigurationClass(o) {
this._ = null;
this.load = function(o) {
this._ = o;
}
this.add = function(a) {

}
if (typeof o!='undefined') { this.load(o); }
}
function OWSAdminConfigurationsClass() {
this._ = [];
this._$= null;
this.active = function() {		
if (this.length>0) { 
if (this._$==null || this._$ >= this.length) { this._$=0 }
return this[this._$];
}
return new OWSConfigurationClass();
}
this.add = function(o) {
this.push(OWSConfigurationClass(o))
}
}
*/
//Create the OWS.Manager object, and the class loader
({
    newid: function () {
        var f = function () { return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1) };
        return (f() + f() + "-" + f() + "-" + f() + "-" + f() + "-" + f() + f() + f());
    },
    base: { "Name": "New Configuration", "recordsPerPage": 0, "enableAlphaFilter": false, "enablePageSelection": false, "enableRecordsPerPage": false, "enableCustomPaging": false, "enableExcelExport": false, "enableHide_OnNoQuery": false, "enableHide_OnNoResults": false, "enableAdvancedParsing": true, "enableCompoundIIFConditions": true, "enableQueryDebug": false, "enableQueryDebug_Edit": false, "enableQueryDebug_Admin": false, "enableQueryDebug_Super": false, "enableQueryDebug_Log": false, "enableQueryDebug_ErrorLog": false, "autoRefreshInterval": "", "skipRedirectActions": false, "skipSubqueryDebugging": false, "enableAdmin_Edit": true, "enableAdmin_Admin": false, "enableAdmin_Super": false, "enableAJAX": false, "enableAJAXCustomPaging": false, "enableAJAXCustomStatus": false, "enableAJAXManual": false, "includeJavascriptUtilities": false, "includeJavascriptValidation": false, "javascriptOnComplete": "", "enableMultipleColumnSorting": false, "ModuleCommunicationMessageType": "", "showAll": true, "useExplicitSystemVariables": false, "enabledForcedQuerySplit": false, "searchItems": [], "queryItems": [], "listItems": [], "messageItems": [
		{ "Index": 1,
		    "Level": 0,
		    "Parameters": { "Name": "OnLoad", "RenderType": "0", "skipDebug": "False", "includeSearch": "False", "includeExport": "False", "includeImport": "False" },
		    "ActionType": "Action-Region",
		    "ChildActions": [{ "Index": 2, "Level": 1, "Parameters": { "Value": "Place all general operations within this region as a starting point. This includes Template and Variable assignments." }, "ActionType": "Action-Comment", "ChildActions": [] }, { "Index": 3, "Level": 0, "Parameters": { "Name": "Query Variables", "RenderType": "0", "skipDebug": "False", "includeSearch": "False", "includeExport": "False", "includeImport": "False" }, "ActionType": "Action-Region", "ChildActions": [{ "Index": 4, "Level": 0, "Parameters": { "VariableType": "<Action>", "QuerySource": "Source Value", "QueryTarget": "New Query Variable", "QueryTargetLeft": "", "QueryTargetRight": "", "QueryTargetEmpty": "", "EscapeListX": "0", "Protected": "true", "EscapeHTML": "true" }, "ActionType": "Template-Variable", "ChildActions": [] }, { "Index": 5, "Level": 0, "Parameters": { "VariableType": "<Action>", "QuerySource": "Source Value", "QueryTarget": "New Query Variable", "QueryTargetLeft": "", "QueryTargetRight": "", "QueryTargetEmpty": "", "EscapeListX": "0", "Protected": "true", "EscapeHTML": "true" }, "ActionType": "Template-Variable", "ChildActions": [] }, { "Index": 6, "Level": 0, "Parameters": { "VariableType": "<Action>", "QuerySource": "Source Value", "QueryTarget": "New Query Variable", "QueryTargetLeft": "", "QueryTargetRight": "", "QueryTargetEmpty": "", "EscapeListX": "0", "Protected": "true", "EscapeHTML": "true" }, "ActionType": "Template-Variable", "ChildActions": []}]}]
		}, { "Index": 7, "Level": 0, "Parameters": { "Name": "OnRender", "RenderType": "0", "skipDebug": "False", "includeSearch": "False", "includeExport": "False", "includeImport": "False" }, "ActionType": "Action-Region", "ChildActions": [{ "Index": 8, "Level": 1, "Parameters": { "Value": "Default Region OnRender, used for the purpose of any runtime which will change the course of the outcome of the module, or handling of the general interaction." }, "ActionType": "Action-Comment", "ChildActions": [] }, { "Index": 9, "Level": 0, "Parameters": { "Type": "Query-Query", "GroupStatement": "", "GroupIndex": "", "Value": "New Template" }, "ActionType": "Template", "ChildActions": [{ "Index": 10, "Level": 0, "Parameters": { "Type": "Group-Header", "GroupStatement": "", "GroupIndex": "0", "Value": "Group Header" }, "ActionType": "Template", "ChildActions": [{ "Index": 11, "Level": 0, "Parameters": { "Type": "Detail-Detail", "GroupStatement": "", "GroupIndex": "", "Value": "General Detail" }, "ActionType": "Template", "ChildActions": [] }, { "Index": 12, "Level": 0, "Parameters": { "Type": "Detail-Alternate", "GroupStatement": "", "GroupIndex": "", "Value": "Alternate Detail" }, "ActionType": "Template", "ChildActions": []}] }, { "Index": 13, "Level": 0, "Parameters": { "Type": "Group-Footer", "GroupStatement": "", "GroupIndex": "0", "Value": "Group Footer" }, "ActionType": "Template", "ChildActions": []}] }, { "Index": 14, "Level": 0, "Parameters": { "Type": "Detail-NoResults", "GroupStatement": "", "GroupIndex": "", "Value": "Standard No Results" }, "ActionType": "Template", "ChildActions": [] }, { "Index": 15, "Level": 0, "Parameters": { "Type": "Detail-NoQuery", "GroupStatement": "", "GroupIndex": "", "Value": "Standard No Query" }, "ActionType": "Template", "ChildActions": []}]}], "query": "", "filter": "", "customConnection": "", "listItem": "", "listAItem": "", "defaultItem": "", "noqueryItem": "", "SearchQuery": "", "SearchTitle": "", "SearchLink": "", "SearchAuthor": "", "SearchDate": "", "SearchKey": "", "SearchContent": "", "SearchDescription": "", "Version": "20", "ConfigurationID": ""
    },
    _: [],
    _a: [],
    _activeElement: null,
    open: function (id) {
        if (typeof id != 'undefined') {
            //open an existing config
            ows.Services.request({
                url: id + '.js',
                callback: function (data) {
                    ows.Manager._open(data);
                }
            });
        } else {
            //create a new config
            var cfg = this.base;
            cfg.ConfigurationID = this.newid();
            ows.Manager._open(cfg);
        }
    },
    _open: function (response) {
        var targetId = null;
        if (typeof this.onOpen == 'function') {
            targetId = this.onOpen(response);
        }
        if (targetId.status == 1) {
            this._[response.ConfigurationID] = new OWSConfigurationClass(response, targetId.Id);
            this._.push(response.ConfigurationID);
        } else {
            //already opened!
        }
    },
    close: function (id) {
        if (typeof id == 'undefined') {
            id = null;
            var rslt = ows.Manager.active();
            if (rslt != null && rslt.ConfigurationID != null) {
                id = rslt.ConfigurationID;
                if (typeof this.onClose == 'function') { this.onClose(rslt); }
            }
        }
        if (id != null) { this._[id] = null; try { this._[id] = undefined } catch (ex) { }; var l = this._.length; for (var i = 0; this._.length == l && i < this._.length; i++) { this._.splice(i, 1); } }
        if (this._.length == 0) { this._ = [] };
    },
    closeAll: function () {
        while (this._.length > 0) {
            this.close();
        }
    },
    editAction: function (o, e, c) {
        var eid = this.newid();
        if (typeof o.Parameters == 'undefined') {
            o._getInfo = function () { return { id: eid, element: e.target, configuration: c} };
        } else {
            o._getInfo = function () { return { id: eid, element: e.target.parentElement, configuration: c} };
        }
        ows.Manager._editAction(o);
    },
    _editAction: function (o) {
        var targetId = null;
        if (typeof this.onEditAction == 'function') {
            targetId = this.onEditAction(o);
        }
        var v = o._getInfo();
        this._a[v.id] = new OWSActionClass(o, targetId);
        this._a.push(v.id);
    },
    addAction: function (o, e, c) {
        var eid = this.newid();
        /*
        if (typeof o.Parameters == 'undefined') {
        o._getInfo=function(){return {id:eid,element:e.target,configuration:c}};
        } else {
        o._getInfo=function(){return {id:eid,element:e.target.parentElement,configuration:c}};
        }*/
        ows.Manager.active().config().add(o[0]);
    },
    _addAction: function (o) {

    },
    activeElement: function (o) {
        if (typeof o == 'undefined' || o == null) {
            return (this._activeElement);
        } else {
            this._activeElement = o;
        }
    },
    saveAction: function (id) {
        ows.Manager._saveAction(ows.Editor.Save(id), id);
    },
    _saveAction: function (p, id) {
        var guid = null;
        if (typeof this.onGetAction == 'function') {
            guid = this.onGetAction(id);
        }
        var act = this._a[guid];
        for (var i = 0; i < p.length; i++) {
            act.action.Parameters[p[i].name] = p[i].value;
        }
        var v = act.action._getInfo();
        $(v.element).attr("data", JSON.stringify(act.action));
        act.update($(v.element));
        if (typeof this.onCloseAction == 'function') {
            this.onCloseAction(act);
        }
    },
    cancelAction: function (id) {
        ows.Manager._cancelAction(id);
    },
    _cancelAction: function (id) {
        var guid = null;
        if (typeof this.onGetAction == 'function') {
            guid = this.onGetAction(id);
        }
        var act = this._a[guid];
        if (typeof this.onCloseAction == 'function') {
            this.onCloseAction(act);
        }
    },
    save: function (id) {
        if (typeof id == 'undefined' || id == null) {
            if (typeof ows.Manager.active == 'function') {
                var at = ows.Manager.active();
                if (at != null && at.ConfigurationID != null) {
                    id = at.ConfigurationID;
                }
            }
        }
        if (typeof id != 'undefined' && id != null) {
            var obj = this._[id];
            if (typeof obj != 'undefined') {
                var config = obj.toConfig();
                if (config != null) {
                    //issue SAVE
                    var configJSON = JSON.stringify(config);
                    console.log(configJSON);
                }
            }
        }
    },
    _save: function (response) {
    },
    init: function () {
        if (typeof ows == 'undefined') {
            window.setTimeout(this.init, 13);
        } else {
            ows.Manager = this;
        }
    }
}).init();
ows.Manager.onClose = function (result) {
    $('#ideTopRight').tabs('remove', result.index);
    if ($('#ideTopRight').tabs('length') == 0) {
        $('#ideTopRight').tabs('destroy');
        $('#ideTopRight').html('');
    }
}
ows.Manager.onOpen = function (response) {
    var targetId = 'dv' + response.ConfigurationID.replace(/-/g, '');
    if ($('#' + targetId).length == 0) {
        var tgTabs = $('#ideTopRight');
        var addTab = false;

        if (tgTabs.children().length == 0) {
            tgTabs.append(document.createElement('ul'));
            tgTabs = $('#ideTopRight ul');
        } else {
            tgTabs = $('#ideTopRight ul');
            addTab = true;
        }
        var dv = $(document.createElement('div'));
        dv.attr('id', targetId);

        //add the tab
        if (!addTab) {
            var li = $(document.createElement('li'));
            var lia = $(document.createElement('a'));
            lia.attr('href', '#' + targetId);
            lia.html(response.Name);
            li.append(lia);

            tgTabs.append(li);
        }
        //tgTabs.parent().append(dv);
        //tgTabs.parent().tabs();
        if (addTab) {
            tgTabs.parent().tabs('add', '#' + targetId, response.Name);
            tgTabs.parent().tabs('select', '#' + targetId);
        } else {
            tgTabs.parent().append(dv);
            tgTabs.parent().tabs().find(".ui-tabs-nav").sortable({ axis: "x" });
            tgTabs.parent().tabs('select', '#' + targetId);
        }
        $('#' + targetId).attr('ConfigurationID', response.ConfigurationID);

        return { status: 1, Id: targetId };
    }
    else {
        return { status: 0, Id: targetId };
    }
}
ows.Manager.active = function () {
    var result = { ConfigurationID: null, id: null, tab: null, index: null, config: null };
    var tgTabs = $('#ideTopRight');
    result.index = $('#ideTopRight').tabs('option', 'selected');
    if (result.index >= 0) {
        try {
            result.tab = $('#ideTopRight').children('div')[result.index];
            result.ConfigurationID = $(result.tab).attr('ConfigurationID');
            result.id = $(result.tab).attr('id');
            result.config = function () { return ows.Manager._[this.ConfigurationID]; };
        } catch (ex) { }
    }
    return result;
}
ows.Manager.activeAction = function () {
    var result = { ActionID: null, id: null, tab: null, index: null, config: null };
    var tgTabs = $('#ideBottomRight');
    result.index = $('#ideBottomRight').tabs('option', 'selected');
    if (result.index >= 0) {
        try {
            result.tab = $('#ideBottomRight').children('div')[result.index];
            result.ActionID = $(result.tab).attr('ActionID');
            result.id = $(result.tab).attr('id');
            result.action = function () { return ows.Manager._a[this.ActionID]; };
        } catch (ex) { }
    }
    return result;
}
ows.Manager.onGetAction = function (targetId) {
    return $('#' + targetId).attr('ActionID');
}
ows.Manager.onCloseAction = function (act) {
    var result = ows.Manager.activeAction();
    var tbtabs = $('#ideBottomRight')
    tbtabs.tabs('remove', result.index);
    if (tbtabs.tabs('length') == 0) {
        ide.panels.body.layout.hide('south');
        tbtabs.tabs('destroy');
        tbtabs.html('');
    }

}
ows.Manager.onEditAction = function (action) {
    var o = action._getInfo();
    var targetId = 'dv' + o.id.replace(/-/g, '');
    if ($('#' + targetId).length == 0) {
        var tgTabs = $('#ideBottomRight');
        var addTab = false;

        if (tgTabs.children().length == 0) {
            tgTabs.append(document.createElement('ul'));
            tgTabs = $('#ideBottomRight ul');
        } else {
            tgTabs = $('#ideBottomRight ul');
            addTab = true;
        }
        var dv = $(document.createElement('div'));
        dv.attr('id', targetId);

        //add the tab
        if (!addTab) {
            var li = $(document.createElement('li'));
            var lia = $(document.createElement('a'));
            lia.attr('href', '#' + targetId);
            if (typeof action.ActionType == 'undefined') {
                lia.html("General: " + action.Name);
            } else {
                lia.html(action.ActionType);
            }
            li.append(lia);

            tgTabs.append(li);
        }
        //tgTabs.parent().append(dv);
        //tgTabs.parent().tabs();
        if (addTab) {
            tgTabs.parent().tabs('add', '#' + targetId, (typeof action.ActionType == 'undefined') ? "General: " + action.Name : action.ActionType);
            tgTabs.parent().tabs('select', '#' + targetId);
        } else {
            tgTabs.parent().append(dv);
            tgTabs.parent().tabs().find(".ui-tabs-nav").sortable({ axis: "x" });
            tgTabs.parent().tabs('select', '#' + targetId);
        }
        $('#' + targetId).attr('ActionID', o.id);
    }
    ide.panels.body.layout.open('south');
    return targetId;
}
ows.Editor = {};
ows.Editor.Nulls = function (v) {
    if (v == null)
    { return 'null'; }
    else
    { return v; }
}
ows.Editor.Save = function (id) {
    var p = [];
    var s = jQuery('#' + id + ' [parameter]');
    for (var i = 0; i < s.length; i++) {
        var pi = { 'name': null, 'value': null };
        var sf = 'return ' + s[i].getAttribute('onsave');
        s[i].onSave = new Function(sf);
        pi.name = s[i].getAttribute('parameter');
        pi.value = s[i].onSave();
        if (typeof s[i].getAttribute('method') != 'undefined' && s[i].getAttribute('method') != null) {
            pi.type = s[i].getAttribute('method');
        }
        s[i].onSave = null;

        if (pi.value != null) { // && pi.value.length > 0) {
            p.push(pi);
        }
    }
    return p;
}
//ows.run(function(){ 
//OWS.Manager.open('33114253-8499-3fba-d2ea-70c16eacdb21');
//OWS.Manager.open();
//});
function OWSActionClass(o, target) {
    this.action = o;
    this.target = target;
    this.load = function () {
    }
    this.update = function (t) {
        var act = ows.admin.plugins.items({ Code: this.action.ActionType });
        if (act.length > 0) {
            act = act[0];
            var p = "";
            if (typeof act != 'undefined' && act.onPrint != 'undefined')
                p = act.onPrint(this.action);
            else
                p = "Undefined for " + this.action.ActionType;
            t.html(p);
            var c = this.action._getInfo();
            c.configuration.sequence();
        }
    }
    this.render = function (id) {
        var act = null;
        var isSystem = false;
        if (typeof this.action.ActionType == 'undefined' || this.action.ActionType == null) {
            act = ows.admin.plugins.items({ Code: 'Configuration' });
            isSystem = true;
        }
        else {
            act = ows.admin.plugins.items({ Code: this.action.ActionType });
        }
        if (act.length > 0) {
            act = act[0];
            ows.admin.console.write('Action "' + act.Code + '" editing.');
            //$('#tabs-1').append('<p>'+act.Code+'</p>');
            var t = $('#' + id);

            if (typeof act.getTemplate == 'function') {
                $.tmpl(act.getTemplate(), (isSystem) ? this.action : this.action.Parameters).appendTo(t);
                if (typeof act.onbind != 'undefined') {
                    var tmp = {};
                    tmp.onbind = act.onbind;
                    if (typeof tmp.onbind != 'function') {
                        tmp.onbind = new Function("that", "data", tmp.onbind)
                    }
                    tmp.onbind(t, (isSystem) ? this.action : this.action.Parameters);
                }

                //add Save/Cancel
                var tbs = $(document.createElement('div'));
                tbs.attr("class", "ui-helper-reset ui-helper-clearfix ui-widget-header ui-corner-all");
                t.append(tbs);
                var bts = $(document.createElement('button'));
                bts.html('Save');
                bts.bind('click', function () { ide.actions.save(id); });
                tbs.append(bts);
                bts.button({ icons: { primary: 'ui-icon-disk'} });

                var btc = $(document.createElement('button'));
                btc.html('Cancel');
                btc.bind('click', function () { ide.actions.cancel(id); });
                tbs.append(btc);
                btc.button({ icons: { primary: 'ui-icon-cancel'} });
                ows.Manager.activeElement(t.find('[parameter]')[0]);
                t.find('[parameter]').bind('focus', function () { ows.Manager.activeElement(this) }).each(function () {
                    switch (this.type) {
                        case 'textarea':
                        case 'text':
                            $(this).texty();
                            $(this).texty({ handlers: [
								{ keycode: 69, ctrl: true, trigger: function (o, shft, options) {
								    var result = o.value;
								    var re = new RegExp("\\\\+", "g"); var s = ''; var lindex = 0; var m = re.exec(result);
								    while (m != null) {
								        if (m.index > lindex)
								        { s = s + result.substring(lindex, m.index); }
								        s = s + result.substring(m.index, m.index + m.length);
								        lindex = m.index + m.length;
								        m = re.exec(result);
								    }
								    if (lindex < result.length)
								    { s = s + result.substring(lindex); }
								    result = s;
								    re = new RegExp("[\"]", "g");
								    result = result.replace(re, "\\\"");
								    re = new RegExp("[\[]", "g");
								    result = result.replace(re, "\\[");
								    re = new RegExp("[\\]]", "g");
								    result = result.replace(re, "\\]");
								    re = new RegExp("[\{]", "g");
								    result = result.replace(re, '\\{');
								    re = new RegExp("[\}]", "g");
								    result = result.replace(re, '\\}');
								    o.value = result;
								    return false;
								} 
								},
								{ keycode: 69, shft: true, ctrl: true, trigger: function (o, shft, options) {
								    var result = o.value;

								    var re = new RegExp("\\\\+", "g");
								    var s = '';
								    var lindex = 0;
								    var m = re.exec(result);
								    while (m != null) {
								        if (m.index > lindex)
								        { s = s + result.substring(lindex, m.index); }
								        s = s + result.substring(m.index, m.index + m.length - 1); lindex = m.index + m.length; m = re.exec(result);
								    }
								    if (lindex < result.length) { s = s + result.substring(lindex); }
								    result = s;
								    o.value = result;
								    return false;
								} 
								},
							]
                            });
                        default:
                            break;
                    }
                });
            }
            else {

            }
        }
    }
    this.render(this.target);
}
function OWSConfigurationClass(c, target) {
    this.configuration = c;
    this.target = target;
    this.open = function () {
    }
    this.load = function () {

    }
    this.tree = null;
    this.active = function () {
        return this.tree.find('a.jstree-clicked:eq(0)')
    }
    this.expand = function (all) {
        if (!all) {
            jQuery(this.tree[0]).jstree('open_node', this.active());
        } else {
            jQuery(this.tree[0]).jstree('open_all');
        }
    }
    this.contract = function (all) {
        if (!all) {
            jQuery(this.tree[0]).jstree('close_node', this.active());
        } else {
            jQuery(this.tree[0]).jstree('close_all');
        }
    }
    this.add = function (action) {
        var o = [{ ActionType: action.Code, ChildActions: [], Index: 0, Level: 0, Parameters: action.base}];
        var t = this.toTree(o)[0];
        var parent = this.active();
        if (typeof parent != 'undefined' && parent != null) {
            debugger;
            var pdata = null;
            eval('pdata=' + jQuery(parent).attr('data'));
            var ta = openwebstudio.admin.plugins.actions().filter({ Code: pdata.ActionType })[0];
            if (typeof ta == 'undefined' || t == null || typeof ta.allow == 'undefined' || ta.allow == null) {
                parent = jQuery(parent).closest('ul')[0];
                parent = jQuery(parent).siblings('a')[0];
            }
        }
        jQuery(this.tree[0]).jstree('create_node', parent, 'inside', t);
    }
    this.render = function (id) {
        this.tree = $("#" + id);
        this.tree
		.bind("loaded.jstree", function (e, data) {
		    data.inst.open_all(-1); // -1 opens all nodes in the container
		})
		.bind("move_node.jstree", function (e, data) {
		    ows.Manager.active().config().sequence();
		})
		.bind("create_node.jstree", function (e, data) {
		    ows.Manager.active().config().sequence();
		})
		.bind("delete_node.jstree", function (e, data) {
		    ows.Manager.active().config().sequence();
		})
		.bind("dblclick.jstree", function (e, data) {
		    if (typeof $(e.target).attr('data') != 'undefined' && $(e.target).attr('data') != null) {
		        ows.Manager.active().config().editAction(e, $(e.target).attr('data'));
		    } else {
		        ows.Manager.active().config().editAction(e, $(e.target.parentElement).attr('data'));
		    }
		})
		.jstree({
		    "core": {
		        "html_titles": true
		    },
		    "themes": {
		        "theme": "default",
		        "dots": true,
		        "icons": false
		    },
		    "json_data": {
		        "data": this.toTree(this.configuration)
		    }
			, "ui": {
			    "select_limit": 1,
			    "select_multiple_modifier": "alt",
			    "select_parent_close": "select_parent"
			}
			, "crrm": {
			    "move":
					{
					    "check_move": function (m) {
					        console.log('move');
					        var act = $(m.np[0]).children('a').attr('type');
					        var t = openwebstudio.admin.plugins.actions().filter({ Code: act })[0];
					        //if(typeof $(m.np[0]).data().jstree.ActionType!='undefined' && $(m.np[0]).data().jstree.ActionType=='Template-Variable') {
					        //	return false;
					        //}
					        if (typeof t == 'undefined' || t == null || typeof t.allow == 'undefined' || t.allow == null) {
					            return false;
					        }
					        return true;
					    }
					}
			}
			, "dnd": {
			    "drop_finish": function () {
			        console.log('drop');
			    },
			    "drop_check": function (data) {
			        console.log('TEST');
			        /*if(data.r.data.ActionType == "Template-Variable") {
			        return false;
			        }*/
			        return {
			            after: false,
			            before: false,
			            inside: true
			        };
			    },
			    "drag_finish": function () {
			        alert("DRAG OK");
			    }
			}
			, "plugins": ["themes", "json_data", "ui", "dnd", "crrm", "metadata", "hotkeys"]
		})
    }
    this.create = function () {
    }
    this.save = function () {
    }
    this.saveAs = function () {
    }
    this.unload = function () {
    }
    this.editAction = function (e, data) {
        var o;
        eval('o=' + data);
        ide.actions.edit(o, e, this);
    }
    this.saveAction = function (o) {

    }
    this.clone = function (c, ignored, tos) {
        var cl = {};
        for (var k in c) {
            if (ignored.filter(k).length == 0) {
                cl[k] = c[k];
            }
        }
        if (typeof tos == 'undefined' || !tos) {
            return cl;
        } else {
            return JSON.stringify(cl);
        }
    }
    this.sequence = function () {
        var p = jQuery(this.root()).find('ins.linenumber');
        for (var i = 0; i < p.length; i++) {
            jQuery(p[i]).html((i + 1) + '.');
            eval('var a = ' + jQuery(p[i]).parent().attr('data'));
            if (a.Index != i + 1) {
                a.Index = i + 1;
                jQuery(p[i]).parent().attr('data', JSON.stringify(a));
            }

        }
    }
    this.root = function () {
        if (this.tree != null && this.tree.children('ul').length > 0 && this.tree.children('ul').children('li').length > 0) {
            var p = this.tree.children('ul').children('li')[0];
            return p;
        }
    }
    this.toConfig = function (branch, c) {
        var o = null;
        var l = null;
        var p = null;
        if (typeof branch == 'undefined') {
            //set p to the root li
            p = this.root();
            //set o to the li/a
            eval('o=' + jQuery(p).children('a').attr('data'));
            //add messageItems to o
            o.messageItems = [];
            //set l to the list of messageItems (our next target)
            l = o.messageItems;
        } else {
            p = branch;
            //set o to teh li/a
            eval('o=' + jQuery(p).children('a').attr('data'));
            //add ChildActions to o
            o.ChildActions = [];
            //set l to the list of ChildActions (our next target)
            l = o.ChildActions;
            c.push(o);
        }
        if (jQuery(p).children('ul').length > 0) { //looking for a child branch
            var s = jQuery(p).children('ul').children('li');
            for (var i = 0; i < s.length; i++) {
                //looping through children
                this.toConfig(jQuery(s[i]), l);
            }
        }
        if (typeof c == 'undefined') {
            return o;
        } else {
            return null;
        }
    }
    this.toTree = function (items, prnt) {
        if (typeof prnt == 'undefined') {
            prnt = [];
        }
        if (typeof items.messageItems != 'undefined') {
            var source = items;
            var target = { data: { title: '', icon: 'ui-icon-document', attr: { data: this.clone(source, ['messageItems'], true), type: 'Configuration'} }, state: "closed" };
            var t = openwebstudio.admin.plugins.actions().filter({ Code: 'Configuration' })[0];
            if (typeof t != 'undefined' && typeof t.allow != 'undefined' && t.allow != null) { target.children = [] };
            if (typeof t != 'undefined' && t.onPrint != 'undefined')
                target.data.title = t.onPrint(source);
            else
                target.data.title = source.Name;
            prnt.push(this.toTree(source.messageItems, target));
        } else {
            for (var i = 0; i < items.length; i++) {
                var source = items[i];
                var target = { data: { title: '', icon: '', attr: { data: this.clone(source, ['ChildActions'], true), type: source.ActionType} }, state: "closed" };
                var t = openwebstudio.admin.plugins.actions().filter({ Code: source.ActionType })[0];
                if (typeof t != 'undefined' && typeof t.allow != 'undefined' && t.allow != null) { target.children = [] };
                if (typeof t != 'undefined' && t.onPrint != 'undefined')
                    target.data.title = t.onPrint(source);
                else
                    target.data.title = "Undefined for " + source.ActionType;
                if (typeof target.children != 'undefined') {
                    if (typeof prnt.children == 'undefined') {
                        prnt.push(this.toTree(source.ChildActions, target));
                    } else {
                        prnt.children.push(this.toTree(source.ChildActions, target));
                    }
                }
                else {
                    if (typeof prnt.children == 'undefined') {
                        prnt.push(target);
                    } else {
                        prnt.children.push(target);
                    }
                }
            }
        }
        return prnt;
    }
    this.render(this.target);
}

function OWSAdminPluginsClass() {
    this._ = [];
    this.add = function (o) {
        if (o.length > 0) {
            for (var i = 0; i < o.length; i++) {
                if (typeof o[i].type == 'undefined' || o[i].type == null) { o[i].type = 'Action' }
                this._.push(o[i]);
                ows.admin.console.write(o[i].type + ' plugin "' + o[i].Code + '" enabled.');
            }
        } else {
            if (typeof o.type == 'undefined' || o.type == null) { o.type = 'Action' }
            this._.push(o);
        }
    }
    this.actions = function () {
        return this._.filter({ type: 'Action' });
    }
    this.items = function (f) {
        if (typeof f == 'undefined' || f == null) {
            return this._;
        }
        else {
            return this._.filter(f);
        }
    }
    this.load = function () {
        for (var a = 0; a < ows$plg._.length; a++) {
            var act = this._[a];
            if (typeof act.getTemplate != 'function') {
                if (act.Type == "action" && act.Template.length < 100) {
                    var code = act.Code;
                    //$.get(act.Template,function(template){ows$plg.bind(code,template);});
                    $.get(act.Template, new Function("data", "ows$plg.bind('" + code + "',data);"));
                } else {
                    this.bind(act.Code, act.Template);
                }
            }
        }
    }
    this.bind = function (code, template) {
        if (typeof $.template["Action-" + code] == 'undefined') {
            $.template("Action-" + code, template);
        }
        this.items({ Code: code })[0].getTemplate = function () { return "Action-" + code; };
    }
}
function OWSAdminConsole(c) {
    this.index = 1;
    this.length = 20;
    this.direction = 'down';
    this.target = { scroll: null, remove: null };
    $(c).append(document.createElement('div'));
    $(c).append(document.createElement('div'));
    if (this.direction == 'down') {
        this.target.scroll = $(c).children(':first');
        this.target.remove = $(c).children(':last');
    } else {
        this.target.scroll = $(c).children(':last');
        this.target.remove = $(c).children(':first');
    }
    this.write = function (s) {
        var el = $(document.createElement('div'));
        el.addClass('consoleline');
        el.hide();
        el.html(this.index + '. ' + s);
        if (this.direction == 'down') {
            this.target.scroll.prepend(el);
            el.slideDown('slow');
        } else {
            this.target.scroll.append(el);
            el.slideUp('slow');
        }
        this.cleanup();
        this.index++;
    }
    this.cleanup = function () {
        if (this.target.scroll.children().length > this.length) {
            var d = null;
            if (this.direction == 'down') {
                d = this.target.scroll.children(':last').detach();
                this.target.remove.prepend(d);
                d.slideUp('slow', function () { $(this).remove() });
            } else {
                d = this.target.children(':first').detach();
                this.target.remove.append(d);
                d.slideDown('slow', function () { $(this).remove() });
            }
        }
    }
}
function OWSAdminUtilityClass() {
    this.encodehtml = function (v) {
        v = v.replace(/</g, "&lt;");
        v = v.replace(/>/g, "&gt;");
        return v;
    }
    this.shorten = function (value, length) {
        //COMMENT
        if (value == undefined)
            value = '';
        if (length == undefined)
            length = 75;
        if (value.length > length && length > 0) {
            value = value.substring(0, length - 1);
        }
        //ENCODE HTML...
        value = this.encodehtml(value);

        return value;
    }
    this.describe = function (i, t, s, c) {
        var t = t.split('-');
        t = t[t.length - 1];
        if (typeof c == 'undefined' || c == null || c.length == 0) {
            return '<ins class="linenumber">' + i + '.</ins>' + '<strong>' + t + '</strong><i>' + this.shorten(s) + '</i>';
        } else {
            return '<ins class="linenumber">' + i + '.</ins>' + '<strong class="' + c + '">' + t + '</strong><i class="' + c + '">' + this.shorten(s) + '</i>';
        }
    }
}
function OWSAdminClass(d) {
    if (typeof d == 'undefined') { d = false };

    if (d) {
        this.console = new OWSAdminConsole('#ideFooter');
    } else {
        this.console = {};
        this.console.write = function (v) { };
    }

    //this.configurations = new OWSAdminConfigurationsClass();
    this.utility = new OWSAdminUtilityClass();
    this.plugins = null;

    this.bind = function (target, clear, prepend, template, data, append) {
        this.console.write('Template(' + template + ') bound to target "' + target + '".');
        var t = $(target);
        if (clear) { t.empty(); };
        if (prepend != null) { t.append(prepend); };
        $(template).tmpl(data).appendTo(t);
        if (typeof append != 'undefined' && append != null) { t.append(append); }
        if ($(template).attr('onbind') != null) {
            var tmp = $(template).get()[0];
            if (typeof tmp.onbind != 'function') {
                tmp.onbind = new Function("that", "data", $(template).attr('onbind'))
            }
            tmp.onbind(t, data);
        }
        t = null;
    }
    this.init = function () {
        this.plugins = new OWSAdminPluginsClass();
    }
    this.load = function () {
        //openwebstudio.admin.plugins.actions().filter({system:null})
        this.bind("#ideTopLeft", true, false, "#tmp_ideActions", this.plugins.actions().filter({ Type: "action", system: null }), true);
        this.bind("#ide_tokens", true, false, "#tmp_ideActions", this.plugins.actions().filter({ Type: "token", system: null }), true);
        $("#ide_plugins").accordion();
    }
}
function hideShowGroup(val, target, s, t, lst) {
    var c = $(val).attr(s);
    for (var i = 0; i < lst.length; i++) {
        target.find("[" + t + "=" + lst[i] + "]").hide();
    }
    if (c != null) {
        target.find("[" + t + "=" + c + "]").show();
    }
}
ows.admin = new OWSAdminClass(true);
openwebstudio = ows;
ows.admin.init();
ows$plg = openwebstudio.admin.plugins;
ows$plg.add([
  { //Configuration
      "Name": "Configuration",
      "Type": "action", "Code": "Configuration",
      "Description": "General Settings",
      "onPrint": function (a) { return a.Name },
      "allow": "all",
      "system": true,
      "Template": "_templates/ide.action.configuration.html",
      "base": { "Name": "" }
  },
  { //Comment
      "Name": "Comment",
      "Type": "action", "Code": "Action-Comment",
      "Description": "Provide the ability for a developer to place Comments within their Action script",
      "onPrint": function (a) { return ows.admin.utility.describe(a.Index, a.ActionType, ' - ' + a.Parameters.Value, 'Comment') },
      "allow": "all",
      "Template": "_templates/ide.action.comment.html",
      "base": { "Value": "" }
  },
  { //Goto
      "Name": "Goto", //NOT READY!
      "Type": "action", "Code": "Action-Goto",
      "Description": "Provide the ability to jump from one configuration to a point in another configuration",
      "onLoad": "onActionLoad_Goto",
      "onSave": "onActionSave_Goto",
      "onDelete": "onActionDelete",
      "onPrint": function (a) {
          var s = '';
          if (typeof a.Parameters.ConfigurationID != 'undefined' && a.Parameters.ConfigurationID.length > 0 && a.Parameters.ConfigurationID != '00000000-0000-0000-0000-000000000000')
              s += 'Configuration:' + a.Parameters.Name + ' ';
          if (a.Parameters.Region.length > 0)
              s += 'Region:' + a.Parameters.Region + ' ';
          return ows.admin.utility.describe(a.Index, a.ActionType, s)
      },
      "allow": null,
      "Template": "_templates/ide.action.goto.html",
      "base": { "ConfigurationID": null, "Name": null, "Region": "" }
  },
  { //Execute
      "Name": "Query",
      "Type": "action", "Code": "Action-Execute",
      "Description": "Execute a query to gain the results and user them as a loop, or as a reference point",
      "onPrint": function (a) { return ows.admin.utility.describe(a.Index, (a.Parameters.IsProcess.toUpperCase() == 'TRUE' ? '(Process)&nbsp;&nbsp;' : '') + a.ActionType + '[' + a.Parameters.Name + ']', ' - ' + a.Parameters.Query, 'Comment') },
      "allow": "all",
      "Template": "_templates/ide.action.execute.html",
      "base": { "IsProcess": "False", "Name": "New Query", "Query": "" }
  },
  { //Message
      "Name": "Message",
      "Type": "action", "Code": "Message",
      "Description": "Event driven logic, based on a message sender and receiver. Upon receipt of the defined message, this action will execute.",
      "onPrint": function (a) {
          var s = '';
          if (a.Parameters.Type.length > 0) {
              if (a.Parameters.Value.length > 0)
                  s = 'Awaiting Incoming Event with Type \'' + a.Parameters.Type + '\' and Value \'' + a.Parameters.Value + '\'';
              else
                  s = 'Awaiting Incoming Event with Type \'' + a.Parameters.Type + '\'';
          }
          else {
              if (a.Parameters.Value.length > 0)
                  s = 'Awaiting Incoming Event with Value \'' + a.Parameters.Value + '\'';
              else
                  s = 'Awaiting Incoming Event with any Type and value';
          }
          return ows.admin.utility.describe(a.Index, a.ActionType, s, 'Comment')
      },
      "allow": "all",
      "Template": "_templates/ide.action.message.html",
      "base": { "Value": "False", "Type": "" }
  },
  { //Search
      "Name": "Search",
      "Type": "action", "Code": "Action-Search",
      "Description": "Integration within the core systems search functionality.",
      "onPrint": function (a) { return ows.admin.utility.describe(a.Index, a.ActionType, 'Search Integration') },
      "allow": null,
      "Template": "_templates/ide.action.search.html",
      "base": {}
  },
  { //Delay
      "Name": "Delay",
      "Type": "action", "Code": "Action-Delay",
      "Description": "Provide the ability for a developer to force a specific length Delay",
      "onPrint": function (a) { return ows.admin.utility.describe(a.Index, a.ActionType, ' for ' + a.Parameters.Value + ' ' + a.Parameters.Type + (a.Parameters.Value != 1 ? 's' : '')) },
      "allow": "all",
      "Template": "_templates/ide.action.delay.html",
      "base": { "Value": "", "Type": "" }
  },
  { //Redirect
      "Name": "Redirect", //NOT READY!
      "Type": "action", "Code": "Action-Redirect",
      "Description": "Redirect the browser to another location",
      "onPrint": function (a) { return ows.admin.utility.describe(a.Index, a.ActionType, ' to ' + a.Parameters.Type + ' ' + a.Parameters.Link) },
      "allow": null,
      "Template": "_templates/ide.action.redirect.html",
      "base": { "Type": "", "Link": "" }
  },
  { //File
      "Name": "File", //NOT READY!
      "Type": "action", "Code": "Action-File",
      "Description": "Load Files or Import and Export data to and from your systems",
      "onPrint": function (a) {
          var s = '';
          s += a.Parameters.SourceType + ' source ';
          if (a.Parameters.Source != null && a.Parameters.Source.length > 0)
              s += '"' + a.Parameters.Source + '" ';
          if (a.Parameters.DestinationType != null && a.Parameters.DestinationType.length > 0)
              s += a.Parameters.DestinationType + ' destination ';
          if (a.Parameters.Destination != null && a.Parameters.Destination.length > 0)
              s += '"' + a.Parameters.Destination + '" ';
          if (a.Parameters.TransformType != null && a.Parameters.TransformType.length > 0)
              s += a.Parameters.TransformType + ' with ' + a.Parameters.TransformType + ' transform';
          return ows.admin.utility.describe(a.Index, a.ActionType, s)
      },
      "allow": "all",
      "Template": "_templates/ide.action.file.html",
      "base": { "SourceType": "", "Source": "", "DestinationType": "", "Destination": "", "TransformType": "" }
  },
  { //Filter
      "Name": "Filter", //NOT READY!
      "Type": "action", "Code": "Action-Filter",
      "Description": "Load Filter options for providing built in filtering for you query",
      "onPrint": function (a) { return ows.admin.utility.describe(a.Index, a.ActionType, ' Options ') },
      "allow": null,
      "Template":
   "<table class=Normal width=100% border=0 cellpadding=1 cellspacing=0>" +
    "<tr>" +
     "<td width=151>" +
     "<div class=SubHead id=fi14>Filter Options</div>" +
     "</td>" +
     "<td><div id=fi15></div></td>" +
    "</tr>" +
   "</table>",
      "base": {}
  },
  { //Assignment
      "Name": "Assignment",
      "Type": "action", "Code": "Action-Assignment",
      "Description": "Assign values to your runtime parameters",
      "onPrint": function (a) { return ows.admin.utility.describe(a.Index, a.ActionType, a.Parameters.Type + '.' + a.Parameters.Name + ' to ' + a.Parameters.Value) },
      "allow": null,
      "Template": "_templates/ide.action.assignment.html",
      "base": { "Type": "", "Name": "New Query", "Value": "" }
  },
  { //Email
      "Name": "Email", //NOT READY!
      "Type": "action", "Code": "Action-Email",
      "Description": "Send email from the actions",
      "onLoad": "onActionLoad_Email",
      "onSave": "onActionSave_Email",
      "onDelete": "onActionDelete",
      "onPrint": function (a) { return ows.admin.utility.describe(a.Index, a.ActionType, (a.Parameters.Format) + ' ' + a.Parameters.Subject + ' from ' + a.Parameters.From + ' to ' + a.Parameters.To) },
      "allow": "all",
      "Template": "_templates/ide.action.email.html"
  },
  { //Input
      "Name": "Input", //NOT READY!
      "Type": "action", "Code": "Action-Input",
      "Description": "Retrieve data from an external resource",
      "onLoad": "onActionLoad_Input",
      "onSave": "onActionSave_Input",
      "onDelete": "onActionDelete",
      "onPrint": function (a) { return ows.admin.utility.describe(a.Index, a.ActionType, ' From ' + a.Parameters.URL) },
      "allow": "all",
      "Template": "_templates/ide.action.input.html"
  },
  { //Output
      "Name": "Output", //NOT READY!!!
      "Type": "action", "Code": "Action-Output",
      "Description": "Change the desired output type from the standard rendering to an alternative format",
      "onLoad": "onActionLoad_Output",
      "onSave": "onActionSave_Output",
      "onDelete": "onActionDelete",
      "onPrint": function (a) { return ows.admin.utility.describe(a.Index, a.ActionType, a.Parameters.OutputType + (a.Parameters.Filename.length > 0 ? ' to file ' + a.Parameters.Filename : '')) },
      "allow": "all",
      "Template": "_templates/ide.action.output.html"
  },
  { //Log
      "Name": "Log", //NOT READY!!
      "Type": "action", "Code": "Action-Log",
      "Description": "Write an event into the Log",
      "onLoad": "onActionLoad_Log",
      "onSave": "onActionSave_Log",
      "onDelete": "onActionDelete",
      "onPrint": function (a) { return ows.admin.utility.describe(a.Index, a.ActionType, a.Parameters.Name + ' with a value of ' + a.Parameters.Value) },
      "allow": null,
      "Template": "_templates/ide.action.log.html"
  },
  { //Template
      "Name": "Template",
      "Type": "action", "Code": "Template",
      "Description": "Assign to the global Template property used for this configuration",
      "onPrint": function (a) {
          var s = '';
          var tname = '';
          switch (a.Parameters.Type) {
              case 'Query-Query':
                  tname = 'Query';
                  break;
              case 'Detail-Detail':
                  tname = 'Detail';
                  break;
              case 'Group-Header':
                  if (a.Parameters.GroupIndex == null)
                      a.Parameters.GroupIndex = '';
                  if (a.Parameters.GroupStatement == null)
                      a.Parameters.GroupStatement = '';
                  tname = 'Header [' + a.Parameters.GroupIndex + '] ' + (a.Parameters.GroupStatement.length > 0 ? ' "' + a.Parameters.GroupStatement + '"' : '');
                  break;
              case 'Group-Footer':
                  if (a.Parameters.GroupIndex == null)
                      a.Parameters.GroupIndex = '';
                  if (a.Parameters.GroupStatement == null)
                      a.Parameters.GroupStatement = '';
                  tname = 'Footer [' + a.Parameters.GroupIndex + '] ' + (a.Parameters.GroupStatement.length > 0 ? ' "' + a.Parameters.GroupStatement + '"' : '');
                  break;
              case 'Detail-NoQuery':
                  tname = 'No Query';
                  break;
              case 'Detail-NoResults':
                  tname = 'No Results';
                  break;
              case 'Detail-Alternate':
                  tname = 'Detail (Alternate)';
                  break;
          }
          s += ' - ' + a.Parameters.Value;
          return ows.admin.utility.describe(a.Index, tname + ' ' + a.ActionType, s)
      },
      "allow": "all",
      "Template": "_templates/ide.action.template.html"
  },
  { //Variable
      "Name": "Variable",
      "Type": "action", "Code": "Template-Variable",
      "Description": "Create a SQL Variable for use throughout the queries within the configuration",
      "onLoad": "onActionLoad_Variable",
      "onSave": "onActionSave_Variable",
      "onDelete": "onActionDelete",
      "onPrint": function (a) { return ows.admin.utility.describe(a.Index, a.ActionType, a.Parameters.QueryTarget + ' from ' + a.Parameters.VariableType + ' ' + a.Parameters.QuerySource) },
      "allow": null,
      "Template": "_templates/ide.action.variable.html"
  },
  { //Loop
      "Name": "Loop",
      "Type": "action", "Code": "Action-Loop",
      "Description": "Provide a simple conditional Looping operation",
      "onLoad": "onActionLoad_Loop",
      "onSave": "onActionSave_Loop",
      "onDelete": "onActionDelete",
      "onPrint": function (a) {
          if (a.Parameters.IsAdvanced.toUpperCase() == 'TRUE') {
              s = a.Parameters.LeftCondition;
          }
          else {
              s = a.Parameters.LeftCondition + '&nbsp;' + a.Parameters.Operator + '&nbsp;' + a.Parameters.RightCondition;
          }
          return ows.admin.utility.describe(a.Index, a.ActionType + ' Until ', s)
      },
      "allow": "all",
      "Template": "_templates/ide.action.loop.html"
  },
  { //If
      "Name": "If",
      "Type": "action", "Code": "Condition-If",
      "Description": "Provide a simple conditional If statement",
      "onLoad": "onActionLoad_Condition_If",
      "onSave": "onActionSave_Condition_If",
      "onDelete": "onActionDelete",
      "onPrint": function (a) {
          if (a.Parameters.IsAdvanced.toUpperCase() == 'TRUE') {
              s = a.Parameters.LeftCondition;
          }
          else {
              s = a.Parameters.LeftCondition + '&nbsp;' + a.Parameters.Operator + '&nbsp;' + a.Parameters.RightCondition;
          }
          return ows.admin.utility.describe(a.Index, a.ActionType, s)
      },
      "allow": "all",
      "Template": "_templates/ide.action.if.html"
  },
  { //Else-If
      "Name": "Else If",
      "Type": "action", "Code": "Condition-ElseIf",
      "Description": "Provide a simple conditional Else If statement",
      "onLoad": "onActionLoad_Condition_If",
      "onSave": "onActionSave_Condition_If",
      "onDelete": "onActionDelete",
      "onPrint": function (a) {
          if (a.Parameters.IsAdvanced.toUpperCase() == 'TRUE') {
              s = a.Parameters.LeftCondition;
          }
          else {
              s = a.Parameters.LeftCondition + '&nbsp;' + a.Parameters.Operator + '&nbsp;' + a.Parameters.RightCondition;
          }
          return ows.admin.utility.describe(a.Index, a.ActionType, s)
      },
      "allow": "all",
      "Template": "_templates/ide.action.elseif.html"
  },
  { //Else
      "Name": "Else",
      "Type": "action", "Code": "Condition-Else",
      "Description": "Provide a simple conditional Else statement",
      "onLoad": "",
      "onSave": "onActionSave_Condition_Else",
      "onDelete": "onActionDelete",
      "onPrint": function (a) { return ows.admin.utility.describe(a.Index, a.ActionType, "") },
      "allow": "all",
      "Template": "_templates/ide.action.else.html"
  },
  { //Select
      "Name": "Select",
      "Type": "action", "Code": "Condition-Select",
      "Description": "Provide a select conditional statement",
      "onLoad": "onActionLoad_Condition_Select",
      "onSave": "onActionSave_Condition_Select",
      "onDelete": "onActionDelete",
      "onPrint": function (a) { return ows.admin.utility.describe(a.Index, a.ActionType, ' "' + a.Parameters.Value + '"') },
      "allow": "all",
      "Template": "_templates/ide.action.select.html"
  },
  { //Case
      "Name": "Case",
      "Type": "action", "Code": "Condition-Case",
      "Description": "Provide a simple conditional Case statement",
      "onLoad": "onActionLoad_Condition_Case",
      "onSave": "onActionSave_Condition_Case",
      "onDelete": "onActionDelete",
      "onPrint": function (a) { return ows.admin.utility.describe(a.Index, a.ActionType, '[' + a.Parameters.Condition + ']') },
      "allow": "all",
      "Template": "_templates/ide.action.case.html"
  },
  { //Region
      "Name": "Region",
      "Type": "action", "Code": "Action-Region",
      "Description": "",
      "onLoad": "onActionLoad_Region",
      "onSave": "onActionSave_Region",
      "onPrint": function (a) {
          var cdbg = '';
          var csea = '';
          var csex = '';
          var csei = ''
          if (typeof a.Parameters.skipDebug == 'undefined' || a.Parameters.skipDebug == null)
              a.Parameters.skipDebug = 'False';
          if (a.Parameters.skipDebug.toLowerCase() == 'true')
              cdbg = '(Debug Disabled) ';
          if (typeof a.Parameters.includeSearch == 'undefined' || a.Parameters.includeSearch == null)
              a.Parameters.includeSearch = 'False';
          if (a.Parameters.includeSearch.toLowerCase() == 'true')
              csea = '(Include In Search) ';
          if (typeof a.Parameters.includeExport == 'undefined' || a.Parameters.includeExport == null)
              a.Parameters.includeExport = 'False';
          if (a.Parameters.includeExport.toLowerCase() == 'true')
              csex = '(Include In Export) ';
          if (typeof a.Parameters.includeImport == 'undefined' || a.Parameters.includeImport == null)
              a.Parameters.includeImport = 'False';
          if (a.Parameters.includeImport.toLowerCase() == 'true')
              csei = '(Include In Import) ';
          return ows.admin.utility.describe(a.Index, a.ActionType, ' ' + cdbg + csea + csex + csei + '- ' + a.Parameters.Name + (a.Parameters.RenderType == '1' ? '(Page Load Only)' : '') + (a.Parameters.RenderType == '2' ? '(AJAX Request Only)' : ''), 'Region')
      },
      "allow": "all",
      "Template": "_templates/ide.action.region.html"
  }]);

ows$plg.add([
  { "Name": "Action",
      "Type": "token", "Code": "Token-Action",
      "Description": "A dynamic link tag",
      "Template": "{ACTION, columnName, sessionVariableName, destination, variableType, renderHREF}"
  },
  { "Name": "Actions",
      "Type": "token", "Code": "Token-Actions",
      "Description": "A multi part dynamic link tag",
      "Template": "{ACTIONS, columnName1, sessionVariableName1, variableType1, columnName2, sessionVariableName2, variableType2, destination}"
  },
  { "Name": "Alternate",
      "Type": "token", "Code": "Token-Alternate",
      "Description": "Returns the current alternate value, based on the iteration of requests from the provided list. First request returns the first item, the next request the second and so on.",
      "Template": "{ALTERNATE,Name,Value1,Value2,Value3...}"
  },
  { "Name": "CheckListItem",
      "Type": "token", "Code": "Token-CheckListItem",
      "Description": "A single item in a check list",
      "Template": "{CHECKLISTITEM, groupName, value, default}"
  },
  { "Name": "CheckList",
      "Type": "token", "Code": "Token-CheckList",
      "Description": "A Check List Group",
      "Template": "{CHECKLIST, groupName, sessionVariableName}"
  },
  { "Name": "Columns",
      "Type": "token", "Code": "Token-Columns",
      "Description": "The Column Layout Tag provides a quick layout of query data",
      "Template": "{COLUMNS, colTemplate, separatorTemplate, separatePrePost, ignoreColumnList}"
  },
  { "Name": "Count",
      "Type": "token", "Code": "Token-Count",
      "Description": "Counts the numbers to a Bock Variable",
      "Template": "{COUNT,myVariable, Value}"
  },
  { "Name": "Format(pre)",
      "Type": "token", "Code": "Token-Format-Pre",
      "Description": "Formats the provided value using Pre formatting, meaning the value is formatted prior to rendering into the tag.\n\n[$Value:Source,Formatter]",
      "Template": "[$Name:Source,FORMATTER]"
  },
  { "Name": "Format(post)",
      "Type": "token", "Code": "Token-Format-Post",
      "Description": "Formats the provided value using Post formatting, meaning the value is formatted after rendering into the tag.\n\n[FORMAT,Value,Formatter]",
      "Template": "[FORMAT,\"Value\",FORMATTER]"
  },
  { "Name": "IIF",
      "Type": "token", "Code": "Token-IIF",
      "Description": "This is a conditional tag used to handle If and only If",
      "Template": "{IIF, \"Condition\", \"True Part\", \"False Part\"}"
  },
  { "Name": "Locale",
      "Type": "token", "Code": "Token-Locale",
      "Description": "Reads resource values directly from the provided file name for the provided key.",
      "Template": "[LOCALE,ResourceFilePath,Key]"
  },
  { "Name": "Math",
      "Type": "token", "Code": "Token-Math",
      "Description": "Evaluates a Mathematical Expression and replaces the output with the result",
      "Template": "{MATH, \"Expression\"}"
  },
  { "Name": "Radio",
      "Type": "token", "Code": "Token-Radio",
      "Description": "Convenient radio button method",
      "Template": "{RADIO, sessionVariableName, columnName}"
  },
  { "Name": "Set",
      "Type": "token", "Code": "Token-Set",
      "Description": "Set a variable to a temporary Action variable",
      "Template": "{SET, myVariable, Value, Collection}"
  },
  { "Name": "Sort",
      "Type": "token", "Code": "Token-Sort",
      "Description": "Provides a convenient way to provide sorting features",
      "Template": "{SORT, columnName, standardText, ascendingText, descendingText, defaultOrder, sortIndex, optional sortQuerystring, optional sortTargetId}"
  },
  { "Name": "Subquery",
      "Type": "token", "Code": "Token-Subquery",
      "Description": "Allows a query against the database with full support of Header/Footer/Detail template rendering within the context of the token",
      "Template": "{SUBQUERY, Name=\"\", Query=\"\" , Header=\"\", Footer=\"\", NoResultFormat=\"\", NoQueryFormat=\"\", Format=\"\", SelectedFormat=\"\", Value=\"\", SelectedField=\"\", SelectedItems=\"\", UseCache=\"\"}"
  },
  { "Name": "Sum",
      "Type": "token", "Code": "Token-Sum",
      "Description": "Adds numbers to a Bock Variable",
      "Template": "{SUM,myVariable, Value}"
  },
  { "Name": "TextEditor",
      "Type": "token", "Code": "Token-TextEditor",
      "Description": "Adds the Rich Text Editor configured as the default for your environment.",
      "Template": "{TEXTEDITOR,id,source name,source collection,width,height}"
  }
]);

ows$plg.load();
ows.admin.load();
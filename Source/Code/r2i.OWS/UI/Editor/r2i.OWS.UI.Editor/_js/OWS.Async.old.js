function OWSError(error, field) {
    this.Field = null;
    this.Error = null;
    if (typeof field != 'undefined') { this.Field = field; };
    if (typeof error != 'undefined') { this.Error = error; };
}
function OWSParameter(n, v) {
    this.name = null;
    this.value = null;
    if (typeof n != 'undefined') { this.name = n; }
    if (typeof v != 'undefined') { this.value = v; }
}
function OWSRequestParameter(n, v, t) {
    this.name = null;
    this.value = null;
    this.type = null;
    if (typeof n != 'undefined') {
        if (typeof n.name != 'undefined' && typeof n.value != 'undefined') {
            this.name = n.name;
            this.value = n.value;
            if (typeof v != 'undefined') { this.type = v; }
        }
        else {
            this.name = n;
            if (typeof v != 'undefined') { this.value = v; }
            if (typeof t != 'undefined') { this.type = t; } else { this.type = OWS.Constants.RequestType.get }
        }
    }
}
function OWSRequestParameters(v, u, c) {
    this.add = function (n, v, t) {
        var p = null;
        if (typeof v == 'undefined' && typeof t == 'undefined') {
            if (typeof n.type == 'undefined' || n.type == null) {
                if (typeof n.value == 'undefined') { n.value = null };
                p = new OWSRequestParameter(n.name, n.value, OWS.Constants.RequestType.get);
            }
            else
            { p = n; }
        }
        else
        { p = new OWSRequestParameter(n, v, t); }

        if (p.type == OWS.Constants.RequestType.get) {
            this.parameters.get.push(p);
        }
        else {
            this.parameters.post.push(p);
        }
    }
    this.get = function (t) {
        var sb = [];
        var c = '';
        switch (t) {
            case OWS.Constants.RequestType.get:
                c = '&';
                for (var i = 0; i < this.parameters.get.length; i++) {
                    sb.push(this.parameters.get[i].name + '=' + this.parameters.get[i].value);
                }
                break;
            case OWS.Constants.RequestType.post:
                c = '';
                for (var i = 0; i < this.parameters.post.length; i++) {
                    sb.push("<input type='hidden' name='" + this.parameters.post[i].name + "' value='" + (this.parameters.post[i].value + '').replace(/\'/g, "&#39;").replace(/\"/g, "&#34;") + "'/>");
                }
                break;
        }
        return sb.join(c);
    }
    this.parameters = { get: [{ name: 'type', value: 'json'}], post: [] };
    this.url = null;
    this.callback = null;
    if (typeof v != 'undefined' && v != null) {
        for (var i = 0; i < v.length; i++) {
            this.add(v[i]);
        }
    }
    if (typeof u != 'undefined') {
        this.url = u;
    }
    if (typeof c != 'undefined') {
        this.callback = c;
    }
}
function OWSCallback(pre, post) {
    this.prefix = null;
    this.postfix = null;
    this._register = function (f) {
        this.prefix = 'OWS.callback(' + OWS.Callbacks.register(f) + ',';
        this.postfix = ')';
    }
    if (typeof pre != 'undefined' && pre != null) {
        if (typeof post == 'undefined') {
            if (typeof pre == 'function') {
                this._register(pre);
            }
            else {
                var i = pre.indexOf('()', 0);
                var s = pre.split('()');
                if (s != null && s.length > 0) {
                    this.prefix = s[0] + (i >= 0 ? '(' : '');
                    if (s.length > 1) {
                        this.postfix = (i >= 0 ? ')' : '') + s[1];
                    }
                }
            }
        }
        else {
            this.prefix = pre;
            this.postfix = post;
        }
    }
    if (this.prefix == null) { this.prefix = ''; }
    if (this.postfix == null) { this.postfix = ''; }
}
function OWSCallbacks() {
    this._c = [];
    this._count = 0;
    this._max = 1000;
    this.register = function (f) {
        var id = this._count;
        this._c[id + 'c'] = f;
        if (this._count >= this._max) { this._count = 0; } else { this._count++ };
        return id;
    }
    this.run = function (id, data) {
        if (typeof this._c[id + 'c'] == 'function') { try { this._c[id + 'c'](data); } catch (ex) { } };
        this._c[id + 'c'] = null;
    }
}
function OWSRequest(u, p, c, n) {
    //Public Properties and Methods
    this.callback = null;
    this.parameters = null;
    this.url = null;
    this.name = null;
    this.service = null;
    if (typeof c != 'undefined') {
        if (typeof c == 'object' && c != null) {
            this.callback = c;
        }
        else {
            this.callback = new OWSCallback(c);
        }
    }
    if (typeof p != 'undefined') {
        if (typeof p.parameters == 'undefined') {
            this.parameters = new OWSRequestParameters(p);
        }
        else {
            this.parameters = p;
        }
    }
    if (typeof u != 'undefined') {
        this.url = u;
    }
    if (typeof n != 'undefined') {
        this.name = n;
    }

    //Private Properties and Methods
    this._ = function () //Primary Request function
    {
        this.service = OWS.Services.create(this.name);
        if (this._type() == OWS.Constants.RequestType.post) {
            this._post();
        }
        else {
            this._get();
        }
        this.dispose();
    }
    this._type = function () {
        if (this.parameters != null && typeof this.parameters.parameters != 'undefined' && this.parameters.parameters.post.length > 0) {
            return OWS.Constants.RequestType.post;
        }
        else {
            return OWS.Constants.RequestType.get;
        }
    }
    this._url = function (includeRedirect) {
        var p = [];
        if (this.parameters != null) { p = this.parameters.get(OWS.Constants.RequestType.get); }
        if (p.length > 0)
        { p += '&'; }
        var c = 'pre=' + this.callback.prefix + '&post=' + this.callback.postfix;
        var purl = OWS.Constants.ServiceUrl;
        if (this.parameters != null && this.parameters.url != null) {
            purl = this.parameters.url;
        }
        var r = '';
        if (typeof includeRedirect != 'undefined' && OWS.Constants.RedirectUrl != null) {
            r = '&redirectUrl=' + OWS.Constants.RedirectUrl
        }
        return purl + this.url + (this.url.indexOf('?') > 0 ? '&' : '?') + p + c + '&ref=' + this.service.id + ':' + this.service.requests + r;
    }
    this._get = function () {
        var elem = document.createElement('script');
        elem.setAttribute('type', 'text/javascript');
        elem.src = this._url();
        elem.id = this.service.clientId();

        if (typeof this.parameters.callback == 'function') {
            elem.callback = this.parameters.callback;
            elem.onload = elem.onreadystatechange = function () {
                if (this.callback != null && (!this.readyState ||
						this.readyState === "loaded" || this.readyState === "complete")) {

                    this.callback();

                    // Handle memory leak in IE
                    this.onload = this.onreadystatechange = null;
                    this.callback = null;
                }
            };
        }

        document.body.appendChild(elem);
    }
    this._post = function () {
        var sb = [];
        sb.push("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
        sb.push("<html><body>");
        sb.push("<form name='fOWS' method='post' enctype='multipart/form-data' id='fOWS' action='" + this._url(true) + "'>");
        sb.push(this.parameters.get(OWS.Constants.RequestType.post));
        sb.push("</form>");
        sb.push("</body></html>");

        var pelem = document.createElement('div');
        pelem.id = this.service.clientId();
        pelem.style.position = 'absolute'; pelem.style.top = '-10px'; pelem.style.left = '-10px'; pelem.style.width = '1px'; pelem.style.height = '1px'; pelem.style.overflow = 'hidden';

        var elem = document.createElement('iframe');
        elem.id = this.service.clientId() + 'iframe';
        elem.setAttribute('name', this.service.clientId() + 'iframe');

        pelem.appendChild(elem);
        document.body.appendChild(pelem);

        var doc = null;
        try { doc = window.frames[window.frames.length - 1].document; }
        catch (ex) { window.frames[elem.id] = elem; doc = window.frames[elem.id].document; }

        doc.open();
        doc.write(sb.join(''));
        doc.forms[0].submit();
        sb = null;
    }
    this.dispose = function () {
        if (this.service.disposeId != null) {
            document.body.removeChild(document.getElementById(this.service.disposeId));
        }
        this.service = null;
        this.callback = null;
        this.url = null;
        this.parameters = null;
        this.name = null;
    }
    this._();
}
function OWSRequestService(name, request, response, requests, id) {
    this.name = name;
    this.request = request;
    this.response = response;
    this.requests = requests;
    this.id = id;
    this.disposeId = null;
    this.clientId = function () {
        return this.id + '_' + this.requests;
    }
}
/*
function OWSResponse(d, e) {
    this.Data = null;
    this.Error = null;
    if (typeof d != 'undefined') {
        if (typeof e == 'undefined') {
            if (typeof d.Error != 'undefined') {
                this.Error = d.Error;
                this.Data = d.Data;
            }
            else {
                this.Data = d;
            }
        }
        else {
            this.Data = d;
            this.Error = e;
        }
    }
}
*/
function OWSRequests() {
    this._$ = []; //Array of Requests
    this.multiIndex = 0;
    this.maxIndex = 10;
    this.maxRequests = 32767;
    this.create = function (name) {
        if (typeof name == 'undefined' || name == null) {
            name = 'r2iOWSRequest' + this.multiIndex;
            this.multiIndex++;
            if (this.multiIndex > this.maxIndex)
            { this.multiIndex = 0; }
        }
        if (this.get(name) == null) {
            var uniqueId = Math.floor(Math.random() * 10000000 + 1);
            this.add(name, new OWSRequestService(name, false, false, 0, 'scall' + uniqueId));
        }
        else {
            var srs = this.get(name);
            srs.disposeId = srs.clientId();
            srs.requests++;
            if (srs.requests > this.maxRequests) {
                this.requests = 0;
            }
            this.set(srs);
        }
        return this.get(name);
    }
    this.set = function (srs) {
        this.add(srs.name, srs);
    }
    this.add = function (name, srs) {
        this._$[name] = srs;
    }
    this.get = function (name) {
        if (typeof this._$[name] == 'undefined' || this._$[name] === null) {
            return null;
        }
        else {
            return this._$[name];
        }
    }
}
function OWSServicesClass() {
    this.loaded = false;
    this.init = function () {
       // new OWSRequest('OWS.Services.' + r2i.version + '.js', new OWSRequestParameters(null, OWS.Constants.StaticUrl, OWS.Services.load), null)
    };
    this.load = function () { OWS.Services.loaded = true; OWS.run(); };
    this.create = function (name) {
        return this._requests.create(name);
    }
    this._requests = new OWSRequests();
    this._method = function (url, params, single, callback) {
        if (this.loaded) {
            //url: /services/service.ashx
            //p: OWSRequestParameters or [OWSRequestParameter]
            //s: true or false
            //callback: 'ui.loadProjects();'
            var name = url.replace(/\W/g, '');
            if (!single) { name = null; }
            if (params !== null && typeof params.parameters == 'undefined' && typeof params.push !== 'undefined') {
                params = new OWSRequestParameters(params);
            }
            if (callback !== null && typeof callback !== 'object') {
                callback = new OWSCallback(callback);
            }
            new OWSRequest(callback, params, url, name);
        }
    };

}
function OWSClass(version) {
	this.ready = false;
	window.OWS = this;
	this.version = version;
    this.Constants = {
        StaticUrl: 'http://tally.r2iOWS.com/scripts/',
        ServiceUrl: 'http://tally.r2iOWS.com/',
        RedirectUrl: null,
        CallbackKey: 'OWSCallback.',
        RequestType: { get: 'get', post: 'post' }
    };
    this.Services = new OWSServicesClass();

    //runtime queue
    this._q = [];
    this._nq = function (f) {
        this._q.push(f);
    }
    this._dq = function () {
        if (this._q.length > 0) {
            while (this._q.length > 0) {
                try { this._q.shift()(); } catch (ex) { }
            }
        }
    }
    this.run = function (f) {
        if (typeof f == 'string') {
            f = window[f];
        }
        if (this.Services.loaded) {
            this._dq();
            if (typeof f != 'undefined') { f(); }
        }
        else {
            if (typeof f != 'undefined') { this._nq(f); }
        }
    }

    //callback array
    this.Callbacks = new OWSCallbacks();
    this.callback = function (id, obj) {
        this.Callbacks.run(id, obj);
    }

    this.Formats = {};
    this.Formats.Number = function (num, dec, thou, pnt, curr1, curr2, n1, n2) {
        var x = Math.round(num * Math.pow(10, dec));
        if (x >= 0)
            n1 = n2 = '';
        var y = ('' + Math.abs(x)).split('');
        var z = y.length - dec;
        if (z < 0) z--;
        for (var i = z; i < 0; i++) y.unshift('0');
        if (z < 0) z = 1;
        y.splice(z, 0, pnt);
        if (y[0] == pnt) y.unshift('0');
        while (z > 3) { z -= 3; y.splice(z, 0, thou); }
        var r = curr1 + n1 + y.join('') + n2 + curr2;
        return r;
    }
    this.Formats.Pager = function (records, recordsperpage, display, current, base, callback) {
        //returns {previous:[1,2,3],current:4,next:[5,6,7],first:base,last:total+(base-1)}
        var half = Math.floor(display / 2.0);
        var total = Math.ceil(records / recordsperpage);
        var centered = 1;
        if (display % 2 != 0)
            centered = 0;

        var cLft = true;
        var cRgt = true;
        var distance = 1;
        var remaining = display - 1;
        if (current > total + (base - 1)) {
            current = total + (base - 1);
        }
        var pages = { previous: [], callback: callback, current: current, next: [], first: base, last: total + (base - 1) };
        while (cLft || cRgt) {
            if (remaining > 0 && current - distance >= base) {
                pages.previous.unshift(current - distance);
                remaining--;
            }
            else {
                cLft = false;
            }
            if (remaining > 0 && current + distance <= total + (base - 1)) {
                pages.next.push(current + distance);
                remaining--;
            }
            else {
                cRgt = false;
            }
            distance++;
        }


        return [pages];
    }

	this.load = function() {
		OWS.Services.init();
	}
	this.init = function() {
		if (!this.ready) {
			if (!document.body) {
				return setTimeout(function(){OWS.init()},13);
			}
			this.ready = true;
			this.load();
		}
	}
}
function OWS(version) {
    this.ready = false;
    window.r2i = this;
    this.version = version;
    this.OWS = new OWSClass();
    this.init = function () {
        if (!r2i.ready) {
            if (!document.body) {
                return setTimeout(r2i.init, 13);
            }
            r2i.ready = true;
            r2i.load();
        }
    }
    this.load = function () {
        OWS.Services.init();
    }
    window.$mash = this.OWS.Services;

    //Execute this when the document is ready.
    this.init();
}
new OWS('1.0'); 
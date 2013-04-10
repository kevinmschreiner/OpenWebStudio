var dateFormat = function () {
    var token = /d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g,
		timezone = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g,
		timezoneClip = /[^-+\dA-Z]/g,
		pad = function (val, len) {
		    val = String(val);
		    len = len || 2;
		    while (val.length < len) val = "0" + val;
		    return val;
		};

    // Regexes and supporting functions are cached through closure
    return function (date, mask, utc) {
        var dF = dateFormat;

        // You can't provide utc if you skip other args (use the "UTC:" mask prefix)
        if (arguments.length == 1 && Object.prototype.toString.call(date) == "[object String]" && !/\d/.test(date)) {
            mask = date;
            date = undefined;
        }

        // Passing date through Date applies Date.parse, if necessary
        date = date ? new Date(date) : new Date;
        if (isNaN(date)) throw SyntaxError("invalid date");

        mask = String(dF.masks[mask] || mask || dF.masks["default"]);

        // Allow setting the utc argument via the mask
        if (mask.slice(0, 4) == "UTC:") {
            mask = mask.slice(4);
            utc = true;
        }

        var _ = utc ? "getUTC" : "get",
			d = date[_ + "Date"](),
			D = date[_ + "Day"](),
			m = date[_ + "Month"](),
			y = date[_ + "FullYear"](),
			H = date[_ + "Hours"](),
			M = date[_ + "Minutes"](),
			s = date[_ + "Seconds"](),
			L = date[_ + "Milliseconds"](),
			o = utc ? 0 : date.getTimezoneOffset(),
			flags = {
			    d: d,
			    dd: pad(d),
			    ddd: dF.i18n.dayNames[D],
			    dddd: dF.i18n.dayNames[D + 7],
			    m: m + 1,
			    mm: pad(m + 1),
			    mmm: dF.i18n.monthNames[m],
			    mmmm: dF.i18n.monthNames[m + 12],
			    yy: String(y).slice(2),
			    yyyy: y,
			    h: H % 12 || 12,
			    hh: pad(H % 12 || 12),
			    H: H,
			    HH: pad(H),
			    M: M,
			    MM: pad(M),
			    s: s,
			    ss: pad(s),
			    l: pad(L, 3),
			    L: pad(L > 99 ? Math.round(L / 10) : L),
			    t: H < 12 ? "a" : "p",
			    tt: H < 12 ? "am" : "pm",
			    T: H < 12 ? "A" : "P",
			    TT: H < 12 ? "AM" : "PM",
			    Z: utc ? "UTC" : (String(date).match(timezone) || [""]).pop().replace(timezoneClip, ""),
			    o: (o > 0 ? "-" : "+") + pad(Math.floor(Math.abs(o) / 60) * 100 + Math.abs(o) % 60, 4),
			    S: ["th", "st", "nd", "rd"][d % 10 > 3 ? 0 : (d % 100 - d % 10 != 10) * d % 10]
			};

        return mask.replace(token, function ($0) {
            return $0 in flags ? flags[$0] : $0.slice(1, $0.length - 1);
        });
    };
} ();

// Some common format strings
dateFormat.masks = {
    "default": "ddd mmm dd yyyy HH:MM:ss",
    shortDate: "m/d/yy",
    mediumDate: "mmm d, yyyy",
    longDate: "mmmm d, yyyy",
    fullDate: "dddd, mmmm d, yyyy",
    shortTime: "h:MM TT",
    mediumTime: "h:MM:ss TT",
    longTime: "h:MM:ss TT Z",
    isoDate: "yyyy-mm-dd",
    isoTime: "HH:MM:ss",
    isoDateTime: "yyyy-mm-dd'T'HH:MM:ss",
    isoUtcDateTime: "UTC:yyyy-mm-dd'T'HH:MM:ss'Z'"
};

// Internationalization strings
dateFormat.i18n = {
    dayNames: [
		"Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat",
		"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
	],
    monthNames: [
		"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
		"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
	]
};

function OWSClass() {
	window['ows']=window['openwebstudio']=openwebstudio=ows=this;
	this.Utilities = {Format:{}};
    this.Utilities.Format.Number = function (num, dec, thou, pnt, curr1, curr2, n1, n2) {
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
    this.Utilities.Format.DateTime = function (val, mask, utc) {
        return dateFormat(val, mask, utc);
    }
    this.Utilities.Format.Pager = function (records, recordsperpage, display, current, base, callback) {
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
}
OWSClass();
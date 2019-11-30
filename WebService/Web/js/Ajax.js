window.Mysoft = window.Mysoft || {};
(function ($) {
    var requests = new Array();

    function create_http_request() {
        if (typeof (XMLHttpRequest) == 'undefined') {
            var request = null;
            try {
                request = new ActiveXObject('Msxml2.XMLHTTP');
            }
            catch (e) {
                try {
                    request = new ActiveXObject('Microsoft.XMLHTTP');
                }
                catch (ee)
                { }
            }
            return request;
        }
        else {
            return new XMLHttpRequest();
        }
    }

    function trim(str) {
        var begin = 0;
        while (begin < str.length && (str[begin] == ' ' || str[begin] == '\t' || str[begin] == '\r' || str[begin] == '\n')) {
            begin++;
        }

        var end = str.length - 1;
        while (end >= 0 && (str[end] == ' ' || str[end] == '\t' || str[end] == '\r' || str[end] == '\n')) {
            end--;
        }

        if (begin != 0 || end != str.length) {
            str = str.substr(begin, end - begin + 1);
        }

        return str;
    }

    function isHtml(str) {
        if (!str) {
            return false;
        }
        str = trim(str);

        if (str.length > 9 && str.substr(0, 9) == '<!DOCTYPE') {
            var idx = str.indexOf('>');
            if (idx > -1) {
                str = str.substr(idx + 1);
                str = trim(str);
            }
        }

        if (str.length > 12 && str.substr(0, 5).toLowerCase() == '<html' && str.substr(str.length - 7, 7).toLowerCase() == '</html>') {
            return true;
        }

        return false;
    }

    function ajax_stop() {
        for (var i = 0; i < requests.length; i++) {
            if (requests[i] != null)
                requests[i].abort();
        }
    }

    function ajax_create_request() {
        for (var i = 0; i < requests.length; i++) {
            if (requests[i].readyState == 4) {
                requests[i].abort();
                return requests[i];
            }
        }

        var pos = requests.length;

        requests[pos] = Object();
        requests[pos].obj = create_http_request();

        return requests[pos];
    }

    function ajax_request(url, data, callback, errorHandler, requestMethod) {
        url = url || '';
        $.error = null;
        var request = ajax_create_request();
        var async = typeof (callback) == 'function';

        if (async) request.obj.onreadystatechange = function () {
            if (request.obj.readyState == 4) {
                var response = new ajax_response(request);
                if (response.error) {
                    // 如果是登录重定向，直接跳转到登录页面
                    if (response.error instanceof $.UnauthorizedRequest) {
                        top.location.href = response.error.url;
                        return;
                    }

                    $.error = response.error;
                    if (typeof (errorHandler) == 'function') {
                        if (errorHandler(response.error) === true) {
                            if (typeof ($.errorHandler) == 'function') {
                                $.errorHandler(response.error);
                            }
                        }
                    }
                    else if (typeof ($.errorHandler) == 'function') {
                        $.errorHandler(response.error);
                    }
                }
                else {
                    callback(response.value);
                }
            }
        }

        if ($.getType(data) != 'string') {
            try {
                var arr = [];
                for (var p in data) {
                    if (arr.length > 0) {
                        arr.push('&');
                    }
                    arr.push(p);
                    arr.push('=');
                    var val = data[p];
                    if ($.getType(val) != 'string') {
                        val = $.toJSON(val);
                    }
                    arr.push(encodeURIComponent(val));
                }
                data = arr.join('');
            }
            catch (e) {
                $.error = new ajax_error('序列化错误: ' + e.message, '');
                if (typeof (errorHandler) == 'function') {
                    errorHandler($.error);
                }
                else if (typeof ($.errorHandler) == 'function') {
                    $.errorHandler($.error);
                }
                return;
            }
        }

        if (requestMethod == 'POST' || (data.length + url.length) > 2000) {
            request.obj.open('POST', url, async);
            request.obj.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            request.obj.send(data);
        }
        else {
            if (url.indexOf('?') > 0) {
                url += '&' + data;
            }
            else {
                url += '?' + data;
            }

            var rnd = Math.random();
            request.obj.open('GET', url + '&_r=' + rnd, async);
            request.obj.send();
        }

        if (!async) {
            var response = new ajax_response(request);
            if (response.error) {
                // 如果是登录重定向，直接跳转到登录页面
                if (response.error instanceof $.UnauthorizedRequest) {
                    top.location.href = response.error.url;
                    return;
                }

                $.error = response.error;
                if (typeof ($.errorHandler) == 'function') {
                    $.errorHandler(response.error);
                }
                else {
                    alert(response.error);
                }
            }
            else {
                return response.value;
            }
        }
    }

    function ajax_response(request) {
        this.request = request.obj;
        this.error = null;
        this.value = null;

        if (request.obj.status == 200) {
            try {
                if (request.obj.responseText) {
                    if (isHtml(request.obj.responseText)) {
                        this.error = new ajax_error_html(request.obj.responseText);
                        this.value = null;
                    }
                    else {
                        this.value = (new Function("return " + request.obj.responseText))();
                    }
                }

                if (this.value && this.value instanceof ajax_error) {
                    this.error = this.value;
                    this.value = null;
                }
            }
            catch (e) {
                this.error = new ajax_error(e.name, e.description);
            }
        }
        else {
            this.error = new ajax_error('HTTP 请求失败，返回: ' + request.obj.status, '');
            this.error.status = request.obj.status;
        }

        return this;
    }

    function ajax_error(name, description, method, extendData) {
        this.name = name;
        this.description = description;
        this.method = method;

        if (extendData) {
            for (var p in extendData) {
                this[p] = extendData[p];
            }
        }

        return this;
    }

    ajax_error.prototype.toString = function () {
        var msg = '';
        if (this.name) {
            msg = this.name;
        }

        if (this.description) {
            if (msg) {
                msg += ' ';
            }
            msg += this.description;
        }
        return msg;
    }

    function ajax_error_html(html) {
        this.html = html;
        ajax_error.call(this, html);
    }

    ajax_error_html.prototype = new ajax_error();
    ajax_error_html.prototype.constructor = ajax_error_html;

    $.AjaxRequest = ajax_request;
    $.ajaxRequest = $.get = ajax_request;
    $.AjaxError = ajax_error;
    $.AjaxErrorHtml = ajax_error_html;

    $.UnauthorizedRequest = function (url) {
        this.url = url;
    };

    $.UnauthorizedRequest.prototype = new ajax_error();
    $.UnauthorizedRequest.prototype.constructor = $.UnauthorizedRequest;

    $.call = function (className, methodName, data, callback, errorHandler) {
        return ajax_request('/ajax.axd?class=' + className + '&method=' + methodName, data, callback, errorHandler);
    };

    $.post = function (url, data, callback, errorHandler) {
        return ajax_request(url, data, callback, errorHandler, 'POST');
    };

    // the code of this function is from 
    // http://lucassmith.name/pub/typeof.html
    $.getType = function (o) {
        var _toS = Object.prototype.toString;
        var _types = {
            'undefined': 'undefined',
            'number': 'number',
            'boolean': 'boolean',
            'string': 'string',
            '[object Function]': 'function',
            '[object RegExp]': 'regexp',
            '[object Array]': 'array',
            '[object Date]': 'date',
            '[object Error]': 'error'
        };
        return _types[typeof o] || _types[_toS.call(o)] || (o ? 'object' : 'null');
    };

    // the code of these two functions is from mootools
    // http://mootools.net
    var $specialChars = { '\b': '\\b', '\t': '\\t', '\n': '\\n', '\f': '\\f', '\r': '\\r', '"': '\\"', '\\': '\\\\' };
    var $replaceChars = function (chr) {
        return $specialChars[chr] || '\\u00' + Math.floor(chr.charCodeAt() / 16).toString(16) + (chr.charCodeAt() % 16).toString(16);
    };

    $.isPlainObject = function (obj) {
        // Must be an Object.
        // Because of IE, we also have to check the presence of the constructor property.
        // Make sure that DOM nodes and window objects don't pass through, as well
        if (!obj || $.getType(obj) !== "object" || obj.nodeType || obj == obj.window) {
            return false;
        }

        try {
            // Not own constructor property must be Object
            if (obj.constructor &&
				!Object.prototype.hasOwnProperty.call(obj, "constructor") &&
				!Object.prototype.hasOwnProperty.call(obj.constructor.prototype, "isPrototypeOf")) {
                return false;
            }
        } catch (e) {
            // IE8,9 Will throw exceptions on certain host objects #9897
            return false;
        }

        // Own properties are enumerated firstly, so to speed up,
        // if last one is own, then all properties are own.

        var key;
        for (key in obj) { }

        return key === undefined || Object.prototype.hasOwnProperty.call(obj, key);
    };

    $.toJSON = function (o) {
        var s = [];
        switch ($.getType(o)) {
            case 'undefined':
            case 'null':
                return 'null';

            case 'number':
            case 'boolean':
                return o.toString();

            case 'function':
                return 'null';

            case 'date':
                return '"\\/Date(' + (o.getTime() - o.getTimezoneOffset() * 60 * 1000) + ')\\/"';

            case 'string':
                return '"' + o.replace(/[\x00-\x1f\\"]/g, $replaceChars) + '"';

            case 'array':
                for (var i = 0, l = o.length; i < l; i++) {
                    s.push($.toJSON(o[i]));
                }
                return '[' + s.join(',') + ']';

            case 'error':
                for (var p in o) {
                    if (typeof (o[p]) != 'function') {
                        s.push('"' + p + '":' + $.toJSON(o[p]));
                    }
                }
                return '{' + s.join(',') + '}';

            case 'object':
                if ($.isPlainObject(o)) {
                    for (var p in o) {
                        var val = o[p];
                        var t = $.getType(val);
                        if (t != 'function' && (t != 'object' || $.isPlainObject(val))) // 排除 HTML DOM 对象等非纯数据对象的对象
                        {
                            s.push('"' + p + '":' + $.toJSON(val));
                        }
                    }
                    return '{' + s.join(',') + '}';
                }
                else {
                    return 'null';
                }

            default:
                return '';

        }
    };

    $.parseJSON = function (str) {
        if (!str) {
            return null;
        }
        return (new Function("return " + str))();
    };

    /**
    * 时间对象的格式化;
    */
    $.formatDate = function (date, format) {
        /*
        * eg:format="yyyy-MM-dd HH:mm:ss";
        */
        if (date === null || date === undefined) {
            return '';
        }
        else if ($.getType(date) != 'date') {
            return date.toString();
        }

        if (date.getFullYear() == 1) {
            return '';
        }

        format = format || 'yyyy-MM-dd HH:mm';

        var o = {
            "M+": date.getMonth() + 1,  //month
            "d+": date.getDate(),     //day
            "h+": date.getHours(),    //hour
            "H+": date.getHours(),    //hour
            "m+": date.getMinutes(),  //minute
            "s+": date.getSeconds(), //second
            "q+": Math.floor((date.getMonth() + 3) / 3),  //quarter
            "S": date.getMilliseconds() //millisecond
        }

        if (/(y+)/.test(format)) {
            format = format.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
        }

        for (var k in o) {
            if (new RegExp("(" + k + ")").test(format)) {
                format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
            }
        }
        return format;
    };

    // 注册命名空间
    $.registerNameSpace = function (nameSpace) {
        if (!nameSpace) {
            return window;
        }

        var arr = nameSpace.split('.');
        var ns = window;
        for (var i = 0; i < arr.length; ++i) {
            ns[arr[i]] = ns[arr[i]] || {};
            ns = ns[arr[i]];
        }
        return ns;
    };
})

(Mysoft);




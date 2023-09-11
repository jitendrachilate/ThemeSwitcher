var XA = XA || (function ($, document) {
    var api = {}, onPreInitHandlers, onPostInitHandlers, modules = {};
    onPreInitHandlers = new Array();
    onPostInitHandlers = new Array();
    api.register = function (name, api, init) {
        modules[name] = {
            name: name,
            api: api,
            init: init || api.init || (function () { })
        };
    };
    api.hasPageModes = function () { return !!(window.Sitecore && window.Sitecore.PageModes); };
    api.registerOnPreInitHandler = function (handler) { onPreInitHandlers.push(handler); };
    api.registerOnPostInitHandler = function (handler) { onPostInitHandlers.push(handler); };
    var initScheduled = false;
    api.init = function () {
        if (!initScheduled) {
            initScheduled = true;
            XA.ready(function () {
                try {
                    for (var name in modules)
                        if (modules.hasOwnProperty(name)) {
                            $xa.each(onPreInitHandlers, function (i, h) { h.process(name, modules[name]); });
                            modules[name].init();
                            $xa.each(onPostInitHandlers, function (i, h) { h.process(name, modules[name]); });
                        }
                }
                finally {
                    initScheduled = false;
                }
            });
        }
    };
    api.ready = function (fn) {
        $(document).ready(fn);
    };
    api.component = {};
    api.connector = {};
    api.cookies = {
        shouldSendSameSiteNone: function (useragent) {
            return !this.isSameSiteNoneIncompatible(useragent);
        },
        isSameSiteNoneIncompatible: function (useragent) {
            return this.hasWebKitSameSiteBug(useragent) ||
                this.dropsUnrecognizedSameSiteCookies(useragent);
        },
        hasWebKitSameSiteBug: function (useragent) {
            return this.isIosVersion(12, useragent) ||
                (this.isMacosxVersion(10, 14, useragent) &&
                    (this.isSafari(useragent) || this.isMacEmbeddedBrowser(useragent)));
        },
        dropsUnrecognizedSameSiteCookies: function (useragent) {
            if (this.isUcBrowser(useragent)) {
                return !this.isUcBrowserVersionAtLeast(12, 13, 2, useragent);
            }
            return this.isChromiumBased(useragent) &&
                this.isChromiumVersionAtLeast(51, useragent) &&
                !this.isChromiumVersionAtLeast(67, useragent);
        },
        isIosVersion: function (major, useragent) {
            var regex = /\(iP.+; CPU .*OS (\d+)[_\d]*.*\) AppleWebKit\//;
            return useragent.match(regex) !== null && useragent.match(regex)[0] == major.toString();
        },
        isMacosxVersion: function (major, minor, useragent) {
            var regex = /\(Macintosh;.*Mac OS X (\d+)_(\d+)[_\d]*.*\) AppleWebKit\//;
            return useragent.match(regex) !== null &&
                ((useragent.match(regex)[0] == major.toString()) &&
                    useragent.match(regex)[1] == minor.toString());
        },
        isSafari: function (useragent) {
            var safari_regex = /Version\/.* Safari\//;
            return useragent.match(safari_regex) !== null &&
                !this.isChromiumBased(useragent);
        },
        isMacEmbeddedBrowser: function (useragent) {
            var regex = /^Mozilla\/[\.\d]+ \(Macintosh;.*Mac OS X [_\d]+\) AppleWebKit\/[\.\d]+ \(KHTML, like Gecko\)$/;
            return useragent.match(regex) !== null;
        },
        isChromiumBased: function (useragent) {
            var regex = /Chrom(e|ium)/;
            return useragent.match(regex) !== null;
        },
        isChromiumVersionAtLeast: function (major, useragent) {
            var regex = /Chrom[^ \/]+\/(\d+)[\.\d]* /;
            var version = parseInt(useragent.match(regex)[1]);
            return version >= major;
        },
        isUcBrowser: function (useragent) {
            var regex = /UCBrowser\//;
            return useragent.match(regex) !== null;
        },
        isUcBrowserVersionAtLeast: function (major, minor, build, useragent) {
            var regex = /UCBrowser\/(\d+)\.(\d+)\.(\d+)[\.\d]* /;
            var major_version = parseInt(useragent.match(regex)[0]);
            var minor_version = parseInt(useragent.match(regex)[1]);
            var build_version = parseInt(useragent.match(regex)[2]);
            if (major_version != major) {
                return major_version > major;
            }
            if (minor_version != minor) {
                return minor_version > minor;
            }
            return build_version >= build;
        },
        createCookie: function (name, value, days, sameSite, sameSiteForce) {
            var cookieValue = "", sameSiteValue = "", secureFlag = "";
            if (days) {
                var date = new Date();
                date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
                var expires = "; expires=" + date.toUTCString();
            }
            else {
                expires = "";
            }
            if (sameSite && sameSite.length > 0 && (sameSiteForce || this.shouldSendSameSiteNone(window.navigator.userAgent))) {
                sameSiteValue = "SameSite=" + sameSite + ";";
            }
            if (location.protocol === 'https:') {
                secureFlag = "; secure";
            }
            document.cookie = name + "=" + value + expires + ";" + sameSiteValue + " path=/" + secureFlag;
        },
        readCookie: function (name) {
            var nameEQ = name + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == ' ') {
                    c = c.substring(1, c.length);
                }
                if (c.indexOf(nameEQ) == 0) {
                    return c.substring(nameEQ.length, c.length);
                }
            }
            return null;
        },
        removeCookieWarning: function () {
            var cookieWarning = $xa(".privacy-warning");
            cookieWarning.remove();
        }
    };
    api.queryString = {
        getQueryParam: function (variable) {
            if (variable != null) {
                variable = variable.toLocaleLowerCase();
            }
            var query = window.location.search.substring(1);
            var vars = query.split("&");
            for (var i = 0; i < vars.length; i++) {
                var pair = vars[i].split("=");
                if (decodeURIComponent(pair[0].toLocaleLowerCase()) === variable) {
                    return decodeURIComponent(pair[1]);
                }
            }
            return null;
        }
    };
    var MutationObserver = window.MutationObserver || window.WebKitMutationObserver;
    var eventListenerSupported = window.addEventListener;
    api.dom = {
        observeDOM: function (obj, callback) {
            if (MutationObserver) {
                var obs = new MutationObserver(function (mutations) {
                    if (mutations[0].addedNodes.length || mutations[0].removedNodes.length)
                        callback();
                });
                var options = {
                    childList: true,
                    subtree: true
                };
                obs.observe(obj, options);
            }
            else if (eventListenerSupported) {
                obj.addEventListener('DOMNodeInserted', callback, false);
                obj.addEventListener('DOMNodeRemoved', callback, false);
            }
        }
    };
    return api;
})($, document);
XA.init();

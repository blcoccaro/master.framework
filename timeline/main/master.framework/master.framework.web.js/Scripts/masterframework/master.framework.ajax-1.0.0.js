(function (Page) {
    "use strict";
    var Queue = $.manageAjax.create('ajaxManagerUnique',
        {
            queue: true,
            maxRequests: 2,
            afterCompleteAll: function (request) {
                if (Page.AjaxService.AfterCompleteAll != null) Page.AjaxService.AfterCompleteAll(request);
            }
        });

    function ConvertToJSON(data, paramRequestName) {
        var seen = [];
        var jsonData = JSON.stringify(data, function (key, val) {
            if (typeof val == "object") {
                if ($.inArray(val, seen) >= 0)
                    return undefined;
                seen.push(val);
            }
            return val
        });
        if (paramRequestName == "" || paramRequestName === undefined) paramRequestName = "request";
        return "{ " + paramRequestName + ": " + jsonData + " }";
    };

    var ReturnType = { JSON: 0, HTML: 1 };
    var ContentType = { JSON: 'application/json', Form: 'application/x-www-form-urlencoded' };
    var Methods = { Post: 'POST', Get: 'GET' };

    Page.AjaxService = (function () {

        var AfterCompleteAll = null;
        var BeforeSend = null;

        var requestDefaults = {
            contentType: ContentType.JSON,
            returnType: ReturnType.JSON,
            methods: Methods.Post,
            requestName: '',
            response: null
        };

        function request(url, data, callback, callbackError, options) {
            if (options === undefined) options = {};
            var localOptions = $.extend({}, requestDefaults, options);

            var requestData = localOptions.contentType == ContentType.JSON ? ConvertToJSON(data, localOptions.requestName) : data;

            var ajaxOptions = {
                url: url,
                data: requestData,
                type: localOptions.methods,
                cache: false,
                processData: false,
                contentType: localOptions.contentType,
                timeout: 1800000,
                dataType: "text",
                beforeSend:
                    function (data) {
                        if (Page.AjaxService.BeforeSend != null) Page.AjaxService.BeforeSend();
                    },
                success:
                    function (data, textStatus, jqXHR) {
                        var dataParsed = (localOptions.returnType == ReturnType.JSON ? JSON.parse(data) : data);
                        if (callback != null) callback(dataParsed);
                    },
                error:
                    function (xhr, textStatus) {
                        if (callbackError != null) callbackError(xhr.responseText);
                    }
            };
            Queue.add(ajaxOptions);
        }

        return {
            Request: request
        };
    })();
})(Page);


; (function ($) {
    "use strict";

    $.fn.CallService = function (url, params, loadMsg, callbackLoadMsg, callback) {
        if (params == undefined || params == null)
            params = {};

        if (loadMsg != undefined && loadMsg != null) {
            $(loadMsg).val("Carregando...");
        }
        if (callbackLoadMsg != null && callbackLoadMsg != undefined) {
            callbackLoadMsg();
        }

        Page.AjaxService.Request(
            url,
            params,
            function (data) {
                if (data.Result == 'OK') {
                    if (callback != null && callback != undefined) {
                        callback(data);
                    }
                }
            },
            null
        );
    };

    $.fn.LoadDropDown = function (url, params, setLoadMsg) {
        if (params == undefined || params == null)
            params = {};
        var select = $(this);

        select.find('option').remove().end();

        if (setLoadMsg == undefined || !setLoadMsg) {
            select.append(
                $('<option>', { value: "-10" }).text("Carregando...")
            );
        }

        Page.AjaxService.Request(
            url,
            params,
            function (data) {
                if (data.Result == 'OK') {
                    select.find('option').remove().end();
                    Page.DropDownContainer.Add(select.prop("id"), data.Options);
                    $.each(data.Options, function (index, item) {
                        select.append(
                            $('<option>', { value: item.Value }).text(item.DisplayText)
                        );
                    });
                }
            },
            null
        );
    };

})(jQuery);
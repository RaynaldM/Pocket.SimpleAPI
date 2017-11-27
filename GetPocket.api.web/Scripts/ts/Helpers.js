/// <reference path="../typings/extends.d.ts" />
var Base;
(function (Base) {
    var Helpers = /** @class */ (function () {
        function Helpers() {
        }
        Helpers.prototype.AjaxService = function (type, url, data, successFunc, errorFunc, processData, async) {
            if (type == "POST")
                data = JSON.stringify(data);
            return $.ajax({
                type: type,
                url: url,
                data: data,
                //traditional: true,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                //processdata: processData != undefined && typeof processData == "boolean" ?<boolean>processData : true, 
                success: successFunc,
                error: function (jqXHR, textStatus, errorThrown) {
                    if (errorFunc) {
                        errorFunc(jqXHR);
                    }
                    else {
                        var errMessage = 'Uncaught Error.\n' + jqXHR.responseText;
                        if (jqXHR.status === 0) {
                            errMessage = 'Not connect.\n Verify Network.';
                        }
                        else if (jqXHR.status == 404) {
                            errMessage = 'Requested page not found. [404]';
                        }
                        else if (jqXHR.status == 500) {
                            errMessage = 'Internal Server Error [500].';
                        }
                        else if (errorThrown === 'parsererror') {
                            errMessage = 'Requested JSON parse failed.';
                        }
                        else if (errorThrown === 'timeout') {
                            errMessage = 'Time out error.';
                        }
                        else if (errorThrown === 'abort') {
                            errMessage = 'Ajax request aborted.';
                        }
                        alert(errMessage);
                    }
                },
                async: async != undefined && typeof async == "boolean" ? async : true
            });
        };
        return Helpers;
    }());
    Base.Helpers = Helpers;
})(Base || (Base = {}));
//# sourceMappingURL=Helpers.js.map
var __extends = (this && this.__extends) || (function () {
    var extendStatics = Object.setPrototypeOf ||
        ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
        function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
/// <reference path="base.ts" />
/// <reference path="../typings/extends.d.ts" />
var IndexPage = /** @class */ (function (_super) {
    __extends(IndexPage, _super);
    function IndexPage() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    IndexPage.prototype.Ready = function () {
        _super.prototype.Ready.call(this);
        var source = $("#list-template").html();
        var template = Handlebars.compile(source);
        $("#testSSD").click(function () {
            BaseHelpers.AjaxService("GET", "Home/RetrieveGetPocket", null, function (data) {
                if (data) {
                    $("#itemList").html(template(data));
                }
            });
        });
    };
    return IndexPage;
}(Base.Page));
//# sourceMappingURL=Index.js.map
/// <reference path="base.ts" />
/// <reference path="../typings/extends.d.ts" />
class IndexPage extends Base.Page {

    public Ready(): void {
        super.Ready();
        var source = $("#list-template").html();
        var template = Handlebars.compile(source);
        $("#testSSD").click(() => {
            BaseHelpers.AjaxService("GET", "Home/RetrieveGetPocket", null,
                (data) => {
                    if (data) {
                        $("#itemList").html(template(data));
                    }
                });
        });
    }
}
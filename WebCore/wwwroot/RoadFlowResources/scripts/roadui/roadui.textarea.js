//Text文本框
; RoadUI.Textarea = function () {
    var instance = this;
    this.init = function ($texts) {
        initElement($texts, "text");
        $texts.each(function () {
            var model = $(this).attr("model");
            var id = $(this).attr('id');
            var name = $(this).attr('name');
            var validate = $(this).attr('validate');
            var value = $(this).text();
            if ("html" === model) {
                
            }
        });

    };
};
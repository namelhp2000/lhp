CKEDITOR.plugins.add('rf_save', {
    init: function (editor) {
        var pluginName = 'rf_save';
        //给自定义插件注册一个调用命令
        editor.addCommand(pluginName, {
            exec: function () {
                var attJSON = formAttributeJSON;
                var eventJSON = formEventsJSON;
                var subtableJSON = formSubtabsJSON;
                var html = ckEditor.getData();
                $.ajax({
                    url: "../FormDesignerPlugin/SaveForm?" + query,
                    type: "post",
                    data: {
                        "attr": JSON.stringify(attJSON),
                        "event": JSON.stringify(eventJSON),
                        "subtable": JSON.stringify(subtableJSON),
                        "html": html
                    },
                    success: function (txt) {
                        alert(txt);
                    }
                });
            }
        });
        //注册一个按钮，来调用自定义插件
        editor.ui.addButton(pluginName, {
            label: "保存表单",
            command: pluginName,
            icon: this.path + "save.png",
            toolbar: 'rf_plugins,50'
        });
    }
});
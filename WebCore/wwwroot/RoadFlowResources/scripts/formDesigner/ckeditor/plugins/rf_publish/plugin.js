CKEDITOR.plugins.add('rf_publish', {
    init: function (editor) {
        var pluginName = 'rf_publish';
        //给自定义插件注册一个调用命令
        editor.addCommand(pluginName, {
            exec: function () {
                var attJSON = formAttributeJSON;
                var eventJSON = formEventsJSON;
                var subtableJSON = formSubtabsJSON;
                var html = ckEditor.getData();

                //这里是清理设置了又删除的子表，清理多余的JSON
                var subtableJSON1 = new Array();
                if (subtableJSON && subtableJSON.length > 0) {
                    var $html = $('<div>' + html + '</div>');
                    var $subtables = $("[data-type='subtable']", $html);
                    for (var i = 0; i < subtableJSON.length; i++) {
                        if ($("[data-type='subtable'][data-secondtable='" + subtableJSON[i].secondtable + "']", $html).size() > 0) {
                            subtableJSON1.push(subtableJSON[i]);
                        }
                    }
                }

                var formHtml = compileForm(attJSON, eventJSON, subtableJSON1, html);
                $.ajax({
                    url: "../FormDesignerPlugin/PublishForm?" + query,
                    type: "post",
                    data: {
                        "attr": JSON.stringify(attJSON),
                        "event": JSON.stringify(eventJSON),
                        "subtable": JSON.stringify(subtableJSON1),
                        "html": html,
                        "formHtml": formHtml
                    },
                    success: function (txt) {
                        alert(txt);
                    }
                });
            }
        });
        //注册一个按钮，来调用自定义插件
        editor.ui.addButton(pluginName, {
            label: "发布表单",
            command: pluginName,
            icon: this.path + "publish.png",
            toolbar: 'rf_plugins,51'
        });
    }
});
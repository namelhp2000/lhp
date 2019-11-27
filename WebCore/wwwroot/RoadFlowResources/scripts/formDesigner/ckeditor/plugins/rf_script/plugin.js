CKEDITOR.plugins.add('rf_script', {
    requires: ['dialog'],
    init: function (editor) {
        var pluginName = 'rf_script';
        //给自定义插件注册一个调用命令
        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));
        //editor.addCommand(pluginName + "_delete", {
        //    exec: function (editor) {
        //        CKEDITOR.rf_remove(editor);
        //    }
        //});

        
        //注册一个按钮，来调用自定义插件
        editor.ui.addButton(pluginName, {
            label: "页面脚本",
            command: pluginName,
            icon: this.path + "script.png",
            toolbar: 'rf_plugins,19'
        });
        
        CKEDITOR.dialog.add(pluginName, function (editor) {
            return {
                title: '页面脚本',
                minWidth: 600,
                minHeight: 330,
                contents: [{
                    id: pluginName + '_attr',
                    label: '页面脚本',
                    title: '页面脚本',
                    elements:
                        [
                            {
                                id: pluginName + '_attr',
                                type: 'html',
                                html: '<iframe style="width:100%;height:330px;border:0;" frameborder="0" src="../FormDesignerPlugin/Scripts?' + query + '"></iframe>'
                            }
                        ]
                }],
                onShow: function () {
                    var attrJSON = formAttributeJSON;
                    if (!attrJSON) {
                        return;
                    }
                    var $iframe = $("div[name='" + pluginName + "_attr'] iframe");
                    if ($iframe.size() == 0) {
                        return;
                    }
                    var id = "";
                    $iframe.show(function () {
                        initAttr();
                    });
                    $iframe.load(function () {
                        initAttr();
                    });

                    function initAttr() {
                        var doc = $iframe.get(0).contentWindow.document;
                        $("#scripts", doc).val(attrJSON.scripts);
                        try {
                            $iframe.get(0).contentWindow.initSelect2();
                        } catch (e) { }
                    }
                },
                onHide: function () {
                    currentSelectEditorElement = null;
                },
                onOk: function () {
                    var attrJSON = formAttributeJSON;
                    if (!attrJSON) {
                        return;
                    }
                    var $iframe = $("div[name='" + pluginName + "_attr'] iframe");
                    if ($iframe.size() == 0) {
                        return;
                    }
                    var doc = $iframe.get(0).contentWindow.document;
                    var scripts = $("#scripts", doc).val() || '';
                    attrJSON.scripts = scripts;
                }
            }
        });

    }
});
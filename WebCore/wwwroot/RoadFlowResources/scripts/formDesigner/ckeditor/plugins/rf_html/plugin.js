CKEDITOR.plugins.add('rf_html', {
    requires: ['dialog'],
    init: function (editor) {
        var pluginName = 'rf_html';
        //给自定义插件注册一个调用命令
        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));
        editor.addCommand(pluginName + "_delete", {exec: function (editor) {
            CKEDITOR.rf_remove(editor);
        }});

        //为文本框双击事件绑定一个事件，即显示弹出窗
        editor.on('doubleclick', function (evt) {
            var element = evt.data.element;
            if (element.is('textarea') && 'html' == element.getAttribute('data-type')) {
                currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                evt.data.dialog = pluginName;
            }
        });
        //注册一个按钮，来调用自定义插件
        editor.ui.addButton(pluginName, {
            label: "HTML编辑器",
            command: pluginName,
            icon: this.path + "html.png",
            toolbar: 'rf_plugins,9'
        });
        if (editor.contextMenu) {
            editor.addMenuGroup('rf_plugins');
            editor.addMenuItem(pluginName, {
                label: 'HTML编辑器属性',
                command: pluginName,
                group: 'rf_plugins'
            });
            editor.addMenuItem(pluginName + '_delete', {
                label: '删除HTML编辑器',
                command: pluginName + '_delete',
                group: 'rf_plugins'
            });

            //右键菜单的监听器，判断是否显示菜单
            editor.contextMenu.addListener(function (element) {
                if (element && !element.isReadOnly()) {
                    var name = element.getName();
                    if (name == 'textarea' && 'html' == element.getAttribute('data-type')) {
                        currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                        return {rf_html: CKEDITOR.TRISTATE_OFF, rf_html_delete: CKEDITOR.TRISTATE_OFF};
                    }
                }
            });
        }
        CKEDITOR.dialog.add(pluginName, function (editor) {
            return {
                title: 'HTML编辑器',
                minWidth: 600,
                minHeight: 330,
                contents: [{
                    id: pluginName + '_attr',
                    label: '属性',
                    title: '属性',
                    elements:
                        [
                            {
                                id: pluginName + '_attr',
                                type: 'html',
                                html: '<iframe style="width:100%;height:330px;border:0;" frameborder="0" src="../FormDesignerPlugin/Html?' + query + '"></iframe>'
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
                        var element = currentSelectEditorElement;
                        if (!element) {
                            var selection = editor.getSelection();
                            if (selection) {
                                element = selection.getStartElement();
                            }
                        }
                        var bindfiled = "";
                        if (element) {
                            id = element.getAttribute("id");
                            bindfiled = element.getAttribute("data-bindfiled");
                            $("#width", doc).val(element.getAttribute("data-width") || "");
                            $("#height", doc).val(element.getAttribute("data-height") || "");
                            $("input[name='toolbar'][value='" + (element.getAttribute("data-toolbar") || "") + "']", doc).prop("checked", true);
                        }
                        var fieldOptions = '<option value=""></option>' + getFields(attrJSON.dbConn, attrJSON.dbTable, bindfiled);
                        $("#bindfiled", doc).html(fieldOptions);
                        try {
                            $iframe.get(0).contentWindow.initSelect2();
                        } catch (e) {}
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
                    var width = $("#width", doc).val() || '';
                    var height = $("#height", doc).val() || '';
                    var table = attrJSON.dbTable;
                    var bindfiled = $("#bindfiled", doc).val() || '';

                    var editor1 = this.getParentEditor();
                    var input = null != currentSelectEditorElement ? currentSelectEditorElement : editor1.document.createElement("textarea");
                    input.setText("HTML编辑器(" + bindfiled + ")");
                    if(width) {
                        input.setStyle("width", width);
                    }
                    if(height) {
                        input.setStyle("height", height);
                    }
                    input.setStyle("color", "#999");
                    input.setAttribute("id", (table + "-" + bindfiled).toUpperCase());
                    //input.setAttribute("name", (table + "-" + bindfiled).toUpperCase())
                    input.setAttribute("data-bindfiled", bindfiled);
                    input.setAttribute("data-isflow", "1");
                    input.setAttribute("data-type", "html");
                    input.setAttribute("data-width", width);
                    input.setAttribute("data-height", height);
                    input.setAttribute("data-toolbar", $(":checked[name='toolbar']", doc).val());

                    if (null == currentSelectEditorElement) {
                        editor1.insertElement(input);
                    }
                    currentSelectEditorElement = null;
                }
            }
        });

    }
});
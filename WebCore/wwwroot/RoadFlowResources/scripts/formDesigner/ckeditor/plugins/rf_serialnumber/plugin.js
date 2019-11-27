CKEDITOR.plugins.add('rf_serialnumber', {
    requires: ['dialog'],
    init: function (editor) {
        var pluginName = 'rf_serialnumber';
        //给自定义插件注册一个调用命令
        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));
        editor.addCommand(pluginName + "_delete", {exec: function (editor) {
            CKEDITOR.rf_remove(editor);
        }});

        //为文本框双击事件绑定一个事件，即显示弹出窗
        editor.on('doubleclick', function (evt) {
            var element = evt.data.element;
            if (element.is('input')) {
                var type = element.getAttribute('type') || 'text';
                if (type == 'text' && 'serialnumber' == element.getAttribute('data-type')) {
                    currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                    evt.data.dialog = pluginName;
                }
            }
        });
        //注册一个按钮，来调用自定义插件
        editor.ui.addButton(pluginName, {
            label: "流水号",
            command: pluginName,
            icon: this.path + "serialnumber.png",
            toolbar: 'rf_plugins,16'
        });
        if (editor.contextMenu) {
            editor.addMenuGroup('rf_plugins');
            editor.addMenuItem('rf_serialnumber', {
                label: '流水号属性',
                command: 'rf_serialnumber',
                group: 'rf_plugins'
            });
            editor.addMenuItem(pluginName + '_delete', {
                label: '删除流水号',
                command: pluginName + '_delete',
                group: 'rf_plugins'
            });

            //右键菜单的监听器，判断是否显示菜单
            editor.contextMenu.addListener(function (element) {
                if (element && !element.isReadOnly()) {
                    var name = element.getName();
                    if (name == 'input') {
                        var type = element.getAttribute('type');
                        if (type == 'text' && 'serialnumber' == element.getAttribute('data-type')) {
                            currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                            return {rf_serialnumber: CKEDITOR.TRISTATE_OFF, rf_serialnumber_delete: CKEDITOR.TRISTATE_OFF};
                        }
                    }
                }
            });
        }
        CKEDITOR.dialog.add(pluginName, function (editor) {
            return {
                title: '流水号',
                minWidth: 600,
                minHeight: 350,
                contents: [{
                    id: pluginName + '_attr',
                    label: '属性',
                    title: '属性',
                    elements:
                        [
                            {
                                id: pluginName + '_attr',
                                type: 'html',
                                html: '<iframe style="width:100%;height:350px;border:0;" frameborder="0" src="../FormDesignerPlugin/SerialNumber?' + query + '"></iframe>'
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
                        var maxfiled = "";
                        if (element) {
                            id = element.getAttribute("id");
                            bindfiled = element.getAttribute("data-bindfiled");
                            maxfiled = element.getAttribute("data-maxfiled");
                            var placeholdertext = element.getAttribute("data-placeholdertext");
                            var length = element.getAttribute("data-length");
                            var formatstring = element.getAttribute("data-formatstring");
                            var width = element.getAttribute("data-width");
                            var sqlwhere = RoadUI.Core.unescape(element.getAttribute("data-sqlwhere"));
                            $("#width", doc).val(width);
                            $("#length", doc).val(length);
                            $("#formatstring", doc).val(formatstring);
                            $("#placeholdertext", doc).val(placeholdertext);
                            $("#sqlwhere", doc).val(sqlwhere);
                        }
                        var fieldOptions = '<option value=""></option>' + getFields(attrJSON.dbConn, attrJSON.dbTable, bindfiled, editor);
                        $("#bindfiled", doc).html(fieldOptions);
                        var maxfiledOptions = '<option value=""></option>' + getFields(attrJSON.dbConn, attrJSON.dbTable, maxfiled, editor);
                        $("#maxfiled", doc).html(maxfiledOptions);
                        try {
                            $iframe.get(0).contentWindow.initSelect2();
                        } catch (e) {
                        }
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
                    var bindfiled = $("#bindfiled", doc).val() || '';
                    var maxfiled = $("#maxfiled", doc).val() || '';
                    var length = $("#length", doc).val() || '';
                    var placeholdertext = $("#placeholdertext", doc).val() || '';
                    var formatstring = $("#formatstring", doc).val() || '';
                    var width = $("#width", doc).val() || '';
                    var sqlwhere = RoadUI.Core.escape($("#sqlwhere", doc).val() || '');
                    var table = attrJSON.dbTable;

                    var editor1 = this.getParentEditor();
                    var input = null != currentSelectEditorElement ? currentSelectEditorElement : editor1.document.createElement("input");
                    if (!input.getAttribute("type")) {
                        input.setAttribute("type", "text");
                    }
                    input.setAttribute("value", "流水号(" + bindfiled + ")");
                    input.setStyle("color", "#999");
                    input.setAttribute("id", (table + "-" + bindfiled).toUpperCase());
                    //input.setAttribute("name", (table + "-" + bindfiled).toUpperCase());
                    input.setAttribute("data-bindfiled", bindfiled);
                    input.setAttribute("data-maxfiled", maxfiled);
                    input.setAttribute("data-placeholdertext", placeholdertext);
                    input.setAttribute("data-length", length);
                    input.setAttribute("data-formatstring", formatstring);
                    input.setAttribute("data-width", width);
                    input.setAttribute("data-sqlwhere", sqlwhere);
                    input.setAttribute("data-isflow", "1");
                    input.setAttribute("data-type", "serialnumber");
                    if (width) {
                        input.setStyle("width", width);
                    }
                    if (null == currentSelectEditorElement) {
                        editor1.insertElement(input);
                    }
                    currentSelectEditorElement = null;
                }
            }
        });

    }
});
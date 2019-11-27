CKEDITOR.plugins.add('rf_text', {
    requires: ['dialog'],
    init: function (editor) {
        var pluginName = 'rf_text';
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
                if (type == 'text' && 'text' == element.getAttribute('data-type')) {
                    currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                    evt.data.dialog = pluginName;
                }
            }
        });
        //注册一个按钮，来调用自定义插件
        editor.ui.addButton(pluginName, {
            label: "文本框",
            command: pluginName,
            icon: this.path + "text.png",
            toolbar: 'rf_plugins,2'
        });
        if (editor.contextMenu) {
            editor.addMenuGroup('rf_plugins');
            editor.addMenuItem(pluginName, {
                label: '文本框属性',
                command: pluginName,
                group: 'rf_plugins'
            });
            editor.addMenuItem(pluginName + '_delete', {
                label: '删除文本框',
                command: pluginName + '_delete',
                group: 'rf_plugins'
            });

            //右键菜单的监听器，判断是否显示菜单
            editor.contextMenu.addListener(function (element) {
                if (element && !element.isReadOnly()) {
                    var name = element.getName();
                    if (name == 'input') {
                        var type = element.getAttribute('type');
                        if (type == 'text' && 'text' == element.getAttribute('data-type')) {
                            currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                            return {rf_text: CKEDITOR.TRISTATE_OFF, rf_text_delete: CKEDITOR.TRISTATE_OFF};
                        }
                    }
                }
            });
        }
        CKEDITOR.dialog.add(pluginName, function (editor) {
            return {
                title: '文本框',
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
                                html: '<iframe style="width:100%;height:350px;border:0;" frameborder="0" src="../FormDesignerPlugin/Text?' + query + '"></iframe>'
                            }
                        ]
                }, {
                    id: pluginName + '_events',
                    label: '事件',
                    title: '事件',
                    elements:
                        [
                            {
                                id: pluginName + '_events',
                                type: 'html',
                                html: '<iframe style="width:100%;height:350px;border:0;" frameborder="0" src="../FormDesignerPlugin/Events?' + query + '&pluginName=' + pluginName + '"></iframe>'
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
                        var defaultvalue = "";
                        if (element) {
                            id = element.getAttribute("id");
                            var align = element.getAttribute("data-align");
                            bindfiled = element.getAttribute("data-bindfiled");
                            var placeholdertext = element.getAttribute("data-placeholdertext");
                            var valuetype = element.getAttribute("data-valuetype");
                            var format = element.getAttribute("data-format");
                            var width = element.getAttribute("data-width");
                            var maxlength = element.getAttribute("data-maxlength");
                            defaultvalue = RoadUI.Core.unescape(element.getAttribute("data-defaultvalue"));
                            var inputtype = element.getAttribute("data-inputtype");
                            $("#width", doc).val(width);
                            $("#maxlength", doc).val(maxlength);
                            $("#align", doc).val(align);
                            $("#placeholdertext", doc).val(placeholdertext);
                            $("#format", doc).val(format);
                            $("#defaultvalue", doc).val(defaultvalue);
                            $("input[name='inputtype'][value='" + (inputtype || "text") + "']", doc).prop("checked", true);
                            $("#isreadonly", doc).prop("checked", "readonly" === element.getAttribute("readonly"));
                        }
                        var fieldOptions = '<option value=""></option>' + getFields(attrJSON.dbConn, attrJSON.dbTable, bindfiled, editor);
                        $("#bindfiled", doc).html(fieldOptions);
                        $("#defaultvalueselect", doc).html(getDefaultValueOptions());
                        $("#defaultvalueselect", doc).val(defaultvalue);
                        $("#valuetype", doc).html(getValueTypeOptions());
                        $("#valuetype", doc).val(valuetype || "0");
                        try {
                            $iframe.get(0).contentWindow.initSelect2();
                        } catch (e) {
                        }
                    }
                    var $iframe1 = $("div[name='" + pluginName + "_events'] iframe");
                    if ($iframe1.size() == 0) {
                        return;
                    }
                    $iframe1.show(function () {
                        initEvents();
                    });
                    $iframe1.load(function () {
                        initEvents();
                    });
                    function initEvents() {
                        var doc1 = $iframe1.get(0).contentWindow.document;
                        $("#event_table", doc1).val(attrJSON.dbTable);
                        $("#event_name", doc1).html(getEventsOptions());
                        var eventsJSON = formEventsJSON;
                        var isIn = false;
                        if ($.isArray(eventsJSON)) {
                            for (var i = 0; i < eventsJSON.length; i++) {
                                if (eventsJSON[i].id == id) {
                                    $("#event_name", doc1).val(eventsJSON[i].event);
                                    $("#event_script", doc1).val(eventsJSON[i].scripts);
                                    isIn = true;
                                    break;
                                }
                            }
                        }
                        if (!isIn) {
                            $("#event_script", doc1).val("");
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
                    var align = $("#align", doc).val() || '';
                    var placeholdertext = $("#placeholdertext", doc).val() || '';
                    var valuetype = $("#valuetype", doc).val() || '';
                    var format = $("#format", doc).val() || '';
                    var width = $("#width", doc).val() || '';
                    var maxlength = $("#maxlength", doc).val() || '';
                    var table = attrJSON.dbTable;
                    var defaultvalue = RoadUI.Core.escape($("#defaultvalue", doc).val() || '');
                    var inputtype = $(":checked[name='inputtype']", doc).val() || '';
                    var isreadonly = $("#isreadonly", doc).prop("checked");

                    var editor1 = this.getParentEditor();
                    var input = null != currentSelectEditorElement ? currentSelectEditorElement : editor1.document.createElement("input");
                    if (!input.getAttribute("type")) {
                        input.setAttribute("type", "text");
                    }
                    input.setAttribute("value", "文本框(" + bindfiled + ")");
                    input.setStyle("color", "#999");
                    input.setAttribute("id", (table + "-" + bindfiled).toUpperCase());
                    //input.setAttribute("name", (table + "-" + bindfiled).toUpperCase());
                    input.setAttribute("data-bindfiled", bindfiled);
                    input.setAttribute("data-align", align);
                    input.setAttribute("data-placeholdertext", placeholdertext);
                    input.setAttribute("data-valuetype", valuetype);
                    input.setAttribute("data-format", format);
                    input.setAttribute("data-width", width);
                    input.setAttribute("data-maxlength", maxlength);
                    input.setAttribute("data-isflow", "1");
                    input.setAttribute("data-type", "text");
                    input.setAttribute("data-defaultvalue", defaultvalue);
                    input.setAttribute("data-inputtype", inputtype)
                    if (width) {
                        input.setStyle("width", width);
                    }
                    if (isreadonly) {
                        input.setAttribute("readonly", "readonly");
                    } else {
                        input.removeAttribute("readonly");
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
CKEDITOR.plugins.add('rf_checkbox', {
    requires: ['dialog'],
    init: function (editor) {
        var pluginName = 'rf_checkbox';
        //给自定义插件注册一个调用命令
        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));
        editor.addCommand(pluginName + "_delete", {exec: function (editor) {
            CKEDITOR.rf_remove(editor);
        }});

        //为文本框双击事件绑定一个事件，即显示弹出窗
        editor.on('doubleclick', function (evt) {
            var element = evt.data.element;
            if (element.is('input')) {
                var type = element.getAttribute('type');
                if (type == 'checkbox') {
                    currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                    evt.data.dialog = pluginName;
                }
            }
        });
        //注册一个按钮，来调用自定义插件
        editor.ui.addButton(pluginName, {
            label: "复选按钮",
            command: pluginName,
            icon: this.path + "checkbox.png",
            toolbar: 'rf_plugins,6'
        });
        if (editor.contextMenu) {
            editor.addMenuGroup('rf_plugins');
            editor.addMenuItem(pluginName, {
                label: '复选按钮属性',
                command: pluginName,
                group: 'rf_plugins'
            });
            editor.addMenuItem(pluginName + '_delete', {
                label: '删除复选按钮',
                command: pluginName + '_delete',
                group: 'rf_plugins'
            });

            //右键菜单的监听器，判断是否显示菜单
            editor.contextMenu.addListener(function (element) {
                if (element && !element.isReadOnly()) {
                    var name = element.getName();
                    if (name == 'input') {
                        var type = element.getAttribute('type');
                        if (type == 'checkbox') {
                            currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                            return {rf_checkbox: CKEDITOR.TRISTATE_OFF, rf_checkbox_delete: CKEDITOR.TRISTATE_OFF};
                        }
                    }
                }
            });
        }
        CKEDITOR.dialog.add(pluginName, function (editor) {
            return {
                title: '复选按钮',
                minWidth: 600,
                minHeight: 380,
                contents: [{
                    id: pluginName + '_attr',
                    label: '属性',
                    title: '属性',
                    elements:
                        [
                            {
                                id: pluginName + '_attr',
                                type: 'html',
                                html: '<iframe style="width:100%;height:380px;border:0;" frameborder="0" src="../FormDesignerPlugin/Checkbox?' + query + '"></iframe>'
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
                                html: '<iframe style="width:100%;height:380px;border:0;" frameborder="0" src="../FormDesignerPlugin/Events?' + query + '&pluginName=' + pluginName + '"></iframe>'
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
                        var datasource = "";
                        var ds_sql_dbconn = "";
                        if (element) {
                            id = element.getAttribute("id");
                            bindfiled = element.getAttribute("data-bindfiled");
                            defaultvalue = RoadUI.Core.unescape(element.getAttribute("data-defaultvalue"));
                            datasource = element.getAttribute("data-datasource");
                            ds_sql_dbconn = element.getAttribute("data-ds_sql_dbconn");
                            $("#defaultvalue", doc).val(defaultvalue);
                            $("#ds_dict_value_checkbox", doc).val(element.getAttribute("data-ds_dict_value"));
                            $("#ds_dict_valuefield_checkbox", doc).val(element.getAttribute("data-ds_dict_valuefield"));
                            $("#ds_custom_string", doc).val(RoadUI.Core.unescape(element.getAttribute("data-ds_custom_string")));
                            $("#ds_sql_value", doc).val(RoadUI.Core.unescape(element.getAttribute("data-ds_sql_value")));
                            $("#ds_url_address", doc).val(RoadUI.Core.unescape(element.getAttribute("data-ds_url_address")));
                        }
                        var fieldOptions = '<option value=""></option>' + getFields(attrJSON.dbConn, attrJSON.dbTable, bindfiled, editor);
                        $("#bindfiled", doc).html(fieldOptions);
                        $("#defaultvalueselect", doc).html(getDefaultValueOptions());
                        $("#defaultvalueselect", doc).val(defaultvalue);
                        $("#ds_sql_dbconn", doc).html(dbconnOptions);
                        $("#ds_sql_dbconn", doc).val(ds_sql_dbconn);
                        $("#datasourcespan", doc).html(getDataSourceOptions("datasource", datasource, 'onclick="dsChange(this.value);"'));
                        try {
                            $iframe.get(0).contentWindow.dsChange(datasource);
                        } catch (e) { }
                        try {
                            $iframe.get(0).contentWindow.setValue();
                        } catch (e) { }
                        try {
                            $iframe.get(0).contentWindow.initSelect2();
                        } catch (e) { }
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
                    var defaultvalue = RoadUI.Core.escape($("#defaultvalue", doc).val() || '');
                    var table = attrJSON.dbTable;

                    var editor1 = this.getParentEditor();
                    var input = null != currentSelectEditorElement ? currentSelectEditorElement : editor1.document.createElement("input");
                    if (!input.getAttribute("type")) {
                        input.setAttribute("type", "checkbox");
                    }
                    //var label = editor1.document.createElement("label");
                    //label.setText("单选按钮(" + (table + "." + bindfiled).toUpperCase() + ")");
                    //input.insertBeforeMe(label);
                    //input.setAttribute("value", "");
                    //input.setStyle("color", "#999");
                    input.setAttribute("id", (table + "-" + bindfiled).toUpperCase());
                    //input.setAttribute("name", (table + "-" + bindfiled).toUpperCase());
                    input.setAttribute("data-bindfiled", bindfiled);
                    input.setAttribute("data-isflow", "1");
                    input.setAttribute("data-type", "checkbox");
                    input.setAttribute("data-defaultvalue", defaultvalue);
                    input.setAttribute("data-datasource", $(":checked[name='datasource']", doc).val() || "");
                    input.setAttribute("data-ds_dict_value", $("#ds_dict_value_checkbox", doc).val() || "");
                    input.setAttribute("data-ds_dict_valuefield", $("#ds_dict_valuefield_checkbox", doc).val() || "id");
                    input.setAttribute("data-ds_custom_string", RoadUI.Core.escape($("#ds_custom_string", doc).val() || ""));
                    input.setAttribute("data-ds_sql_dbconn", $("#ds_sql_dbconn", doc).val() || "");
                    input.setAttribute("data-ds_sql_value", RoadUI.Core.escape($("#ds_sql_value", doc).val() || ""));
                    input.setAttribute("data-ds_url_address", RoadUI.Core.escape($("#ds_url_address", doc).val() || ""));

                    if (null == currentSelectEditorElement) {
                        editor1.insertElement(input);
                    }
                    currentSelectEditorElement = null;
                }
            }
        });
    }
});
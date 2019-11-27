CKEDITOR.plugins.add('rf_select', {
    requires: ['dialog'],
    init: function (editor) {
        var pluginName = 'rf_select';
        //给自定义插件注册一个调用命令
        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));
        editor.addCommand(pluginName + "_delete", {exec: function (editor) {
            CKEDITOR.rf_remove(editor);
        }});

        //为文本框双击事件绑定一个事件，即显示弹出窗
        editor.on('doubleclick', function (evt) {
            var element = evt.data.element;
            if (element.is('select')) {
                currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                evt.data.dialog = pluginName;
            }
        });
        //注册一个按钮，来调用自定义插件
        editor.ui.addButton(pluginName, {
            label: "下拉选择",
            command: pluginName,
            icon: this.path + "select.png",
            toolbar: 'rf_plugins,4'
        });
        if (editor.contextMenu) {
            editor.addMenuGroup('rf_plugins');
            editor.addMenuItem('rf_select', {
                label: '下拉选择属性',
                command: pluginName,
                group: 'rf_plugins'
            });
            editor.addMenuItem(pluginName + '_delete', {
                label: '删除下拉选择',
                command: pluginName + '_delete',
                group: 'rf_plugins'
            });

            //右键菜单的监听器，判断是否显示菜单
            editor.contextMenu.addListener(function (element) {
                if (element && !element.isReadOnly()) {
                    var name = element.getName();
                    if (name == 'select') {
                        currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                        return {rf_select: CKEDITOR.TRISTATE_OFF, rf_select_delete: CKEDITOR.TRISTATE_OFF};
                    } else if (name == 'option') {
                        currentSelectEditorElement = element.getParent();//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                        return {rf_select: CKEDITOR.TRISTATE_OFF, rf_select_delete: CKEDITOR.TRISTATE_OFF};
                    }
                }
            });
        }
        CKEDITOR.dialog.add(pluginName, function (editor) {
            return {
                title: '下拉选择',
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
                                html: '<iframe style="width:100%;height:350px;border:0;" frameborder="0" src="../FormDesignerPlugin/Select?' + query + '"></iframe>'
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
                    var eidtor1 = this.getParentEditor();
                    var id = "";
                    $iframe.show(function () {
                        initAttr();
                    });
                    $iframe.load(function () {
                        initAttr();
                    });
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
                        var linkage_source = "";
                        var linkage_source_sql_conn = "";
                        var linkage_field = "";
                        if (element) {
                            id = element.getAttribute("id");
                            bindfiled = element.getAttribute("data-bindfiled");
                            datasource = element.getAttribute("data-datasource");
                            defaultvalue = RoadUI.Core.unescape(element.getAttribute("data-defaultvalue"));
                            ds_sql_dbconn = element.getAttribute("data-ds_sql_dbconn");
                            linkage_source = element.getAttribute("data-linkage_source");
                            linkage_source_sql_conn = element.getAttribute("data-linkage_source_sql_conn");
                            linkage_field = element.getAttribute("data-linkage_field");

                            $("#defaultvalue", doc).val(defaultvalue);
                            $("#width", doc).val(element.getAttribute("data-width")); 
                            $("#hasselect2", doc).prop("checked", "1" == element.getAttribute("data-hasselect2"));
                            $("#hasmultiple", doc).prop("checked", "1" == element.getAttribute("data-hasmultiple"));
                            //$("#hasempty", doc).prop("checked", "1" == element.getAttribute("data-hasempty"));
                            $("#emptytitle", doc).val(element.getAttribute("data-emptytitle") || '');
                            $("input[name='datasource'][value='" + element.getAttribute("data-datasource") + "']", doc).prop("checked", true);
                            $("#ds_dict_ischild", doc).prop("checked", "1" == element.getAttribute("data-ds_dict_ischild"));
                            $("#ds_dict_value_select", doc).val(element.getAttribute("data-ds_dict_value"));
                            $("#ds_dict_valuefield", doc).val(element.getAttribute("data-ds_dict_valuefield"));
                            $("#ds_custom_string", doc).val(RoadUI.Core.unescape(element.getAttribute("data-ds_custom_string")));
                            $("#ds_sql_value", doc).val(RoadUI.Core.unescape(element.getAttribute("data-ds_sql_value")));
                            $("#ds_url_address", doc).val(RoadUI.Core.unescape(element.getAttribute("data-ds_url_address")));
                            $("#linkage_field", doc).val(element.getAttribute("data-linkage_field"));
                            $("input[name='linkage_source'][value='" + linkage_source + "']", doc).prop("checked", true);
                            $("#linkage_field", doc).val(element.getAttribute("data-linkage_field"));
                            $("#linkage_source_text", doc).val(RoadUI.Core.unescape(element.getAttribute("data-linkage_source_text")));

                            //加载联动字段
                            var nodeList = eidtor1.document.find("[data-bindfiled]");
                            var linkage_field_options = '<option value=""></option>';
                            for(var i=0;i<nodeList.count();i++) {
                                var node = nodeList.getItem(i);
                                var nodeId = node.getAttribute("id");
                                if (nodeId && nodeId.length > 0) {
                                    linkage_field_options += '<option value="' + nodeId + '"' + (nodeId == linkage_field ? ' selected="selected"' : '') + '>'
                                        + nodeId + '</option>';
                                }
                            }
                            $("#linkage_field", doc).html(linkage_field_options);
                            try {
                                $iframe.get(0).contentWindow.dsChange(datasource);
                                $("input[name='linkage_source'][value='" + linkage_source + "']", doc).click();
                            } catch (e) {}
                            try {
                                $iframe.get(0).contentWindow.setValue();
                            } catch (e) {}
                        }
                        var fieldOptions = '<option value=""></option>' + getFields(attrJSON.dbConn, attrJSON.dbTable, bindfiled, editor);
                        $("#bindfiled", doc).html(fieldOptions);
                        $("#defaultvalueselect", doc).html(getDefaultValueOptions());
                        $("#defaultvalueselect", doc).val(defaultvalue);
                        $("#valuetype", doc).html(getValueTypeOptions());
                        $("#datasourcespan", doc).html(getDataSourceOptions("datasource", datasource, 'onclick="dsChange(this.value);"'));
                        $("#ds_sql_dbconn", doc).html(dbconnOptions);
                        $("#ds_sql_dbconn", doc).val(ds_sql_dbconn);
                        $("#linkage_source_sql_conn", doc).html(dbconnOptions);
                        $("#linkage_source_sql_conn", doc).val(linkage_source_sql_conn);
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
                    var bindfiled = $("#bindfiled", doc).val() || "";
                    var width = $("#width", doc).val() || "";
                    var table = attrJSON.dbTable;
                    var editor1 = this.getParentEditor();
                    var hasselect2 = $("#hasselect2", doc).prop("checked") ? "1" : "0";
                    var hasmultiple = $("#hasmultiple", doc).prop("checked") ? "1" : "0";
                    var input = null != currentSelectEditorElement ? currentSelectEditorElement : editor1.document.createElement("select");
                    input.setAttribute("data-type", hasselect2 ? "select2" : "select");
                    try {
                        input.setHtml("<option selected='selected'>下拉选择(" + bindfiled + ")</option>");
                    } catch (e){}
                    input.setStyle("color", "#999");
                    input.setStyle("height", "23px");
                    input.setAttribute("id", (table + "-" + bindfiled).toUpperCase());
                    //input.setAttribute("name", (table + "-" + bindfiled).toUpperCase());
                    input.setAttribute("data-bindfiled", bindfiled);
                    input.setAttribute("data-width", width); 
                    //input.setAttribute("data-hasempty", $("#hasempty", doc).prop("checked") ? "1" : "0");
                    input.setAttribute("data-emptytitle", $("#emptytitle", doc).val() || "");
                    input.setAttribute("data-hasselect2", hasselect2);
                    input.setAttribute("data-hasmultiple", hasmultiple);
                    input.setAttribute("data-datasource", $(":checked[name='datasource']", doc).val() || "");
                    input.setAttribute("data-ds_dict_value", $("#ds_dict_value_select", doc).val() || "");
                    input.setAttribute("data-ds_dict_ischild", $("#ds_dict_ischild", doc).prop("checked") ? "1" : "0");
                    input.setAttribute("data-ds_dict_valuefield", $("#ds_dict_valuefield", doc).val() || "id");
                    input.setAttribute("data-ds_custom_string", RoadUI.Core.escape($("#ds_custom_string", doc).val() || ""));
                    input.setAttribute("data-ds_sql_dbconn", $("#ds_sql_dbconn", doc).val() || "");
                    input.setAttribute("data-ds_sql_value", RoadUI.Core.escape($("#ds_sql_value", doc).val() || ""));
                    input.setAttribute("data-ds_url_address", RoadUI.Core.escape($("#ds_url_address", doc).val() || ""));
                    input.setAttribute("data-linkage_field", $("#linkage_field", doc).val() || "");
                    input.setAttribute("data-linkage_source", $(":checked[name='linkage_source']", doc).val() || "");
                    input.setAttribute("data-linkage_source_sql_conn", $("#linkage_source_sql_conn", doc).val() || "");
                    input.setAttribute("data-linkage_source_text", RoadUI.Core.escape($("#linkage_source_text", doc).val() || ""));
                    input.setAttribute("data-isflow", "1");
                    input.setAttribute("data-defaultvalue", RoadUI.Core.escape($("#defaultvalue", doc).val() || ""));
                    input.setAttribute("data-linkage_field", $("#linkage_field", doc).val() || "");
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
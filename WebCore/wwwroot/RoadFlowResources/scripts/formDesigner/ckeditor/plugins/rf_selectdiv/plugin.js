CKEDITOR.plugins.add('rf_selectdiv', {
    requires: ['dialog'],
    init: function (editor) {
        var pluginName = 'rf_selectdiv';
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
                if (type == 'text' && 'selectdiv' == element.getAttribute('data-type')) {
                    currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                    evt.data.dialog = pluginName;
                }
            }
        });
        //注册一个按钮，来调用自定义插件
        editor.ui.addButton(pluginName, {
            label: "弹出选择",
            command: pluginName,
            icon: this.path + "selectdiv.png",
            toolbar: 'rf_plugins,17'
        });
        if (editor.contextMenu) {
            editor.addMenuGroup('rf_plugins');
            editor.addMenuItem('rf_selectdiv', {
                label: '弹出选择属性',
                command: pluginName,
                group: 'rf_plugins'
            });
            editor.addMenuItem(pluginName + '_delete', {
                label: '删除弹出选择',
                command: pluginName + '_delete',
                group: 'rf_plugins'
            });

            //右键菜单的监听器，判断是否显示菜单
            editor.contextMenu.addListener(function (element) {
                if (element && !element.isReadOnly()) {
                    var name = element.getName();
                    if (name == 'input') {
                        var type = element.getAttribute('type');
                        if (type == 'text' && 'selectdiv' == element.getAttribute('data-type')) {
                            currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                            return { rf_selectdiv: CKEDITOR.TRISTATE_OFF, rf_selectdiv_delete: CKEDITOR.TRISTATE_OFF };
                        }
                    }
                }
            });
        }
        CKEDITOR.dialog.add(pluginName, function (editor) {
            return {
                title: '弹出选择',
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
                                html: '<iframe style="width:100%;height:400px;border:0;" frameborder="0" src="../FormDesignerPlugin/SelectDiv?' + query + '"></iframe>'
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
                                html: '<iframe style="width:100%;height:400px;border:0;" frameborder="0" src="../FormDesignerPlugin/Events?' + query + '&pluginName=' + pluginName + '"></iframe>'
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
                            defaultvalue = RoadUI.Core.unescape(element.getAttribute("data-defaultvalue"));
                           
                            $("#defaultvalue", doc).val(defaultvalue);
                            $("#form_types", doc).val(element.getAttribute("data-form_types")).change();
                            $("#form_forms", doc).val(element.getAttribute("data-form_forms")).change();
                            $("#width", doc).val(element.getAttribute("data-width"));
                            $("#windowwidth", doc).val(element.getAttribute("data-windowwidth"));
                            $("#windowheight", doc).val(element.getAttribute("data-windowheight"));
                            $("#windowtitle", doc).val(element.getAttribute("data-windowtitle"));
                            $("#pkfield", doc).val(element.getAttribute("data-pkfield") || '');
                            $("#titlefield", doc).val(element.getAttribute("data-titlefield") || '');
                            $("#paramsname", doc).val(element.getAttribute("data-paramsname"));
                            $("#paramsvalue", doc).val(element.getAttribute("data-paramsvalue"));
                            $("#opener", doc).val(element.getAttribute("data-opener"));
                        }
                        var fieldOptions = '<option value=""></option>' + getFields(attrJSON.dbConn, attrJSON.dbTable, bindfiled, editor);
                        $("#bindfiled", doc).html(fieldOptions);
                        $("#defaultvalueselect", doc).html(getDefaultValueOptions());
                        $("#defaultvalueselect", doc).val(defaultvalue);
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
                    var bindfiled = $("#bindfiled", doc).val() || '';
                    var table = attrJSON.dbTable;
                    var defaultvalue = RoadUI.Core.escape($("#defaultvalue", doc).val() || '');
                    var width = $("#width", doc).val() || "";

                    var editor1 = this.getParentEditor();
                    var input = null != currentSelectEditorElement ? currentSelectEditorElement : editor1.document.createElement("input");
                    if (!input.getAttribute("type")) {
                        input.setAttribute("type", "text");
                    }
                    input.setAttribute("value", "弹出选择(" + bindfiled + ")");
                    input.setStyle("color", "#999");
                    input.setAttribute("id", (table + "-" + bindfiled).toUpperCase());
                    input.setAttribute("data-bindfiled", bindfiled); 
                    input.setAttribute("data-width", width);
                    input.setAttribute("data-windowtitle", $("#windowtitle", doc).val() || "");
                    input.setAttribute("data-windowwidth", $("#windowwidth", doc).val() || "");
                    input.setAttribute("data-windowheight", $("#windowheight", doc).val() || "");
                    input.setAttribute("data-form_types", $("#form_types", doc).val() || "");
                    input.setAttribute("data-form_forms", $("#form_forms", doc).val() || "");
                    input.setAttribute("data-pkfield", $("#pkfield", doc).val() || "");
                    input.setAttribute("data-titlefield", $("#titlefield", doc).val() || "");
                    input.setAttribute("data-paramsname", $("#paramsname", doc).val() || "");
                    input.setAttribute("data-paramsvalue", $("#paramsvalue", doc).val() || "");
                    input.setAttribute("data-opener", $("#opener", doc).val() || "");
                    input.setAttribute("data-isflow", "1");
                    input.setAttribute("data-type", "selectdiv");
                    input.setAttribute("data-defaultvalue", defaultvalue);
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
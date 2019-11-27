CKEDITOR.plugins.add('rf_organize', {
    requires: ['dialog'],
    init: function (editor) {
        var pluginName = 'rf_organize';
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
                if (type == 'text' && 'organize' == element.getAttribute('data-type')) {
                    currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                    evt.data.dialog = pluginName;
                }
            }
        });
        //注册一个按钮，来调用自定义插件
        editor.ui.addButton(pluginName, {
            label: "组织架构选择",
            command: pluginName,
            icon: this.path + "organize.png",
            toolbar: 'rf_plugins,13'
        });
        if (editor.contextMenu) {
            editor.addMenuGroup('rf_plugins');
            editor.addMenuItem(pluginName, {
                label: '组织架构选择属性',
                command: pluginName,
                group: 'rf_plugins'
            });
            editor.addMenuItem(pluginName + '_delete', {
                label: '删除组织架构选择',
                command: pluginName + '_delete',
                group: 'rf_plugins'
            });

            //右键菜单的监听器，判断是否显示菜单
            editor.contextMenu.addListener(function (element) {
                if (element && !element.isReadOnly()) {
                    var name = element.getName();
                    if (name == 'input') {
                        var type = element.getAttribute('type');
                        if (type == 'text' && 'organize' == element.getAttribute('data-type')) {
                            currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                            return {rf_organize: CKEDITOR.TRISTATE_OFF, rf_organize_delete: CKEDITOR.TRISTATE_OFF};
                        }
                    }
                }
            });
        }
        CKEDITOR.dialog.add(pluginName, function (editor) {
            return {
                title: '组织架构选择',
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
                                html: '<iframe style="width:100%;height:330px;border:0;" frameborder="0" src="../FormDesignerPlugin/Organize?' + query + '"></iframe>'
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
                                html: '<iframe style="width:100%;height:330px;border:0;" frameborder="0" src="../FormDesignerPlugin/Events?' + query + '&pluginName=' + pluginName + '"></iframe>'
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
                        var bindfiled = "";
                        var defaultvalue = "";
                        if (!element) {
                            var selection = editor.getSelection();
                            if (selection) {
                                element = selection.getStartElement();
                            }
                        }
                        if (element) {
                            id = element.getAttribute("id");
                            bindfiled = element.getAttribute("data-bindfiled");
                            defaultvalue = RoadUI.Core.unescape(element.getAttribute("data-defaultvalue"));
                            $("#defaultvalue", doc).val(defaultvalue);
                            $("#width", doc).val(element.getAttribute("data-width") || '');
                            $("input[name='org_rang'][value='"+element.getAttribute("data-org_rang")+"']", doc).prop('checked',true);
                            $("#org_type_unit", doc).prop('checked',"1" == element.getAttribute("data-org_type_unit"));
                            $("#org_type_dept", doc).prop('checked',"1" == element.getAttribute("data-org_type_dept"));
                            $("#org_type_station", doc).prop('checked',"1" == element.getAttribute("data-org_type_station"));
                            $("#org_type_wrokgroup", doc).prop('checked',"1" == element.getAttribute("data-org_type_wrokgroup"));
                            $("#org_type_user", doc).prop('checked',"1" == element.getAttribute("data-org_type_user"));
                            $("#more", doc).prop('checked', "1" == element.getAttribute("data-more") || '');
                            $("#opener", doc).val(element.getAttribute("data-opener") || '');
                            if("2" == element.getAttribute("data-org_rang")){
                                $("#org_rang_2_value", doc).val(element.getAttribute("data-org_rang_2_value"));
                                new RoadUI.Member().setValue($("#org_rang_2_value", doc));
                            }
                        }
                        var fieldOptions = '<option value=""></option>' + getFields(attrJSON.dbConn, attrJSON.dbTable, bindfiled, editor);
                        $("#bindfiled", doc).html(fieldOptions);
                        $("#defaultvalueselect", doc).html(getDefaultValueOptions());
                        $("#defaultvalueselect", doc).val(defaultvalue);
                        try {
                            $iframe.get(0).contentWindow.initSelect2();
                        } catch (e) {}
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
                    var table = attrJSON.dbTable;
                    var bindfiled = $("#bindfiled", doc).val() || '';
                    var defaultvalue = RoadUI.Core.escape($("#defaultvalue", doc).val() || '');
                    var width = $("#width", doc).val() || '';
                    var editor1 = this.getParentEditor();
                    var input = null != currentSelectEditorElement ? currentSelectEditorElement : editor1.document.createElement("input");
                    if (!input.getAttribute("type")) {
                        input.setAttribute("type", "text");
                    }
                    input.setAttribute("value", "组织架构(" + bindfiled + ")");
                    input.setStyle("color", "#999");
                    if(width) {
                        input.setStyle("width", width);
                    }
                    input.setAttribute("id", (table + "-" + bindfiled).toUpperCase());
                    //input.setAttribute("name", (table + "-" + bindfiled).toUpperCase());
                    input.setAttribute("data-bindfiled", bindfiled);
                    input.setAttribute("data-isflow", "1");
                    input.setAttribute("data-type", "organize");
                    input.setAttribute("data-defaultvalue", defaultvalue);
                    input.setAttribute("data-width", width);
                    input.setAttribute("data-org_rang", $(":checked[name='org_rang']",doc).val());
                    input.setAttribute("data-org_rang_2_value", $("#org_rang_2_value",doc).val());
                    input.setAttribute("data-org_type_unit", $("#org_type_unit",doc).prop("checked")?"1":"0");
                    input.setAttribute("data-org_type_dept", $("#org_type_dept",doc).prop("checked")?"1":"0");
                    input.setAttribute("data-org_type_station", $("#org_type_station",doc).prop("checked")?"1":"0");
                    input.setAttribute("data-org_type_wrokgroup", $("#org_type_wrokgroup",doc).prop("checked")?"1":"0");
                    input.setAttribute("data-org_type_user", $("#org_type_user",doc).prop("checked")?"1":"0");
                    input.setAttribute("data-more", $("#more", doc).prop("checked") ? "1" : "0");
                    input.setAttribute("data-opener", $("#opener", doc).val() || '');

                    if (null == currentSelectEditorElement) {
                        editor1.insertElement(input);
                    }
                    currentSelectEditorElement = null;
                }
            }
        });

    }
});
CKEDITOR.plugins.add('rf_files', {
    requires: ['dialog'],
    init: function (editor) {
        var pluginName = 'rf_files';
        //给自定义插件注册一个调用命令
        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));
        editor.addCommand(pluginName + "_delete", {exec: function (editor) {
            CKEDITOR.rf_remove(editor);
        }});

        //为文本框双击事件绑定一个事件，即显示弹出窗
        editor.on('doubleclick', function (evt) {
            var element = evt.data.element;
            if (element.is('input')) {
                var type = element.getAttribute('data-type');
                if (type == 'files') {
                    currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                    evt.data.dialog = pluginName;
                }
            }
        });
        //注册一个按钮，来调用自定义插件
        editor.ui.addButton(pluginName, {
            label: "附件",
            command: pluginName,
            icon: this.path + "files.png",
            toolbar: 'rf_plugins,11'
        });
        if (editor.contextMenu) {
            editor.addMenuGroup('rf_plugins');
            editor.addMenuItem(pluginName, {
                label: '附件属性',
                command: pluginName,
                group: 'rf_plugins'
            });
            editor.addMenuItem(pluginName + '_delete', {
                label: '删除附件',
                command: pluginName + '_delete',
                group: 'rf_plugins'
            });

            //右键菜单的监听器，判断是否显示菜单
            editor.contextMenu.addListener(function (element) {
                if (element && !element.isReadOnly()) {
                    var name = element.getName();
                    if (name == 'input') {
                        var type = element.getAttribute('data-type');
                        if (type == 'files') {
                            currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                            return {rf_files: CKEDITOR.TRISTATE_OFF, rf_files_delete: CKEDITOR.TRISTATE_OFF};
                        }
                    }
                }
            });
        }
        CKEDITOR.dialog.add(pluginName, function (editor) {
            return {
                title: '附件',
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
                                html: '<iframe style="width:100%;height:330px;border:0;" frameborder="0" src="../FormDesignerPlugin/Files?' + query + '"></iframe>'
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
                            bindfiled = element.getAttribute("data-bindfiled");
                            $("#width", doc).val(element.getAttribute("data-width") || '');
                            $("#fileType", doc).val(element.getAttribute("data-filetype") || '');
                            $("#opener", doc).val(element.getAttribute("data-opener") || '');
                            $("#fileshow", doc).prop("checked", "1" == element.getAttribute("data-fileshow"));
                            $("#fileshowwidth", doc).val(element.getAttribute("data-fileshowwidth") || '');
                            $("#fileshowheight", doc).val(element.getAttribute("data-fileshowheight") || '');
                        }
                        var fieldOptions = '<option value=""></option>' + getFields(attrJSON.dbConn, attrJSON.dbTable, bindfiled, editor);
                        $("#bindfiled", doc).html(fieldOptions);
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
                    var width = $("#width", doc).val() || '';
                    var fileType = $("#fileType", doc).val() || '';
                    var table = attrJSON.dbTable;
                    var editor1 = this.getParentEditor();
                    var id = (table + "-" + bindfiled).toUpperCase();
                    var input = null != currentSelectEditorElement ? currentSelectEditorElement : editor1.document.createElement("input");
                    if (!input.getAttribute("type")) {
                        input.setAttribute("type", "text");
                    }
                    input.setAttribute("value", "附件(" + bindfiled + ")");
                    if (width) {
                        input.setStyle("width", width);
                    }

                    input.setAttribute("id", id);
                    //input.setAttribute("name", id);
                    input.setStyle("color", "#999");
                    input.setAttribute("data-bindfiled", bindfiled);
                    input.setAttribute("data-isflow", "1");
                    input.setAttribute("data-width", width);
                    input.setAttribute("data-filetype", fileType);
                    input.setAttribute("data-type", "files");
                    input.setAttribute("data-opener", $("#opener", doc).val() || '');
                    input.setAttribute("data-fileshow", $("#fileshow", doc).prop("checked") ? "1" : "0");
                    input.setAttribute("data-fileshowwidth", $("#fileshowwidth", doc).val() || '');
                    input.setAttribute("data-fileshowheight", $("#fileshowheight", doc).val() || '');
                    if (null == currentSelectEditorElement) {
                        editor1.insertElement(input);
                    }
                    currentSelectEditorElement = null;
                }
            }
        });

    }
});
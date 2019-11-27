CKEDITOR.plugins.add('rf_subtable', {
    requires: ['dialog'],
    init: function (editor) {
        var pluginName = 'rf_subtable';
        //给自定义插件注册一个调用命令
        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));
        editor.addCommand(pluginName + "_delete", {exec: function (editor) {
            CKEDITOR.rf_remove(editor);
        }});

        //为文本框双击事件绑定一个事件，即显示弹出窗
        editor.on('doubleclick', function (evt) {
            var element = evt.data.element;
            if (element.is('textarea')) {
                if ('subtable' == element.getAttribute('data-type')) {
                    currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                    evt.data.dialog = pluginName;
                }
            }
        });
        //注册一个按钮，来调用自定义插件
        editor.ui.addButton(pluginName, {
            label: "子表",
            command: pluginName,
            icon: this.path + "subtable.png",
            toolbar: 'rf_plugins,15'
        });
        if (editor.contextMenu) {
            editor.addMenuGroup('rf_plugins');
            editor.addMenuItem(pluginName, {
                label: '子表属性',
                command: pluginName,
                group: 'rf_plugins'
            });
            editor.addMenuItem(pluginName + '_delete', {
                label: '删除子表',
                command: pluginName + '_delete',
                group: 'rf_plugins'
            });

            //右键菜单的监听器，判断是否显示菜单
            editor.contextMenu.addListener(function (element) {
                if (element && !element.isReadOnly()) {
                    var name = element.getName();
                    if (name == 'textarea') {
                        if ('subtable' == element.getAttribute('data-type')) {
                            currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                            return {rf_subtable: CKEDITOR.TRISTATE_OFF, rf_subtable_delete: CKEDITOR.TRISTATE_OFF};
                        }
                    }
                }
            });
        }
        CKEDITOR.dialog.add(pluginName, function (editor) {
            return {
                title: '子表',
                minWidth: 900,
                minHeight: 440,
                contents: [{
                    id: pluginName + '_attr',
                    label: '属性',
                    title: '属性',
                    elements:
                        [
                            {
                                id: pluginName + '_attr',
                                type: 'html',
                                html: '<iframe style="width:100%;height:440px;border:0;" frameborder="0" src="../FormDesignerPlugin/SubTable?' + query + '"></iframe>'
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
                        var secondtable = "";
                        var primarytablefiled = "";
                        var secondtableprimarykey = "";
                        var secondtablerelationfield = "";
                        if (element) {
                            id = element.getAttribute("id");
                            secondtable = element.getAttribute("data-secondtable");
                            primarytablefiled = element.getAttribute("data-primarytablefiled");
                            secondtableprimarykey = element.getAttribute("data-secondtableprimarykey");
                            secondtablerelationfield = element.getAttribute("data-secondtablerelationfield");
                            $("#tablewidth", doc).val(element.getAttribute("data-width"));
                            $("#showindex", doc).prop("checked", "1"==element.getAttribute("data-showindex"));
                        }
                        var attrJSON = formAttributeJSON;
                        if(attrJSON) {
                            $("#secondtabledbconn", doc).val(attrJSON.dbConn);
                            $("#secondtable", doc).html('<option></option>' + getTables(attrJSON.dbConn, secondtable));
                            $("#primarytablefiled", doc).html('<option></option>'+ getFields(attrJSON.dbConn, attrJSON.dbTable, primarytablefiled || attrJSON.dbTablePrimaryKey));
                            $("#secondtableprimarykey", doc).val(secondtableprimarykey);
                            $("#secondtablerelationfield", doc).val(secondtablerelationfield);
                            var opts = getFields(attrJSON.dbConn, secondtable, "");
                            $("#secondtableprimarykey", doc).html('<option></option>' + opts);
                            $("#secondtablerelationfield", doc).html('<option></option>' + opts);
                            $("#secondtableprimarykey", doc).val(secondtableprimarykey);
                            $("#secondtablerelationfield", doc).val(secondtablerelationfield);

                            var initJSON = {};
                            for(var i=0;i<formSubtabsJSON.length;i++) {
                                if(formSubtabsJSON[i].id == id){
                                    initJSON = formSubtabsJSON[i];
                                    $("#sortstring", doc).val(initJSON.sortstring);
                                    break;
                                }
                            }
                            try {
                                if($.isFunction($iframe.get(0).contentWindow.addTableFields)) {
                                    $iframe.get(0).contentWindow.addTableFields(opts, secondtable, initJSON);
                                }
                                if ($.isFunction($iframe.get(0).contentWindow.init)) {
                                    $iframe.get(0).contentWindow.init(initJSON);
                                }
                            }catch (e){}
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
                    var secondtable = $("#secondtable", doc).val() || '';
                    var primarytablefiled = $("#primarytablefiled", doc).val() || '';
                    var secondtableprimarykey = $("#secondtableprimarykey", doc).val() || '';
                    var secondtablerelationfield = $("#secondtablerelationfield", doc).val() || '';
                    var width = $("#tablewidth", doc).val() || '';
                    var sortstring = $("#sortstring", doc).val() || '';
                    var table = attrJSON.dbTable;

                    var editor1 = this.getParentEditor();
                    var input = null != currentSelectEditorElement ? currentSelectEditorElement : editor1.document.createElement("textarea");
                    input.setText("子表");
                    input.setStyle("color", "#999");
                    if(width){
                        input.setStyle("width", width);
                    }
                    input.setStyle("height", "40px");
                    input.setAttribute("data-isflow", "1");
                    input.setAttribute("data-type", "subtable");
                    input.setAttribute("data-secondtable", secondtable);
                    input.setAttribute("data-primarytablefiled", primarytablefiled);
                    input.setAttribute("data-secondtableprimarykey", secondtableprimarykey);
                    input.setAttribute("data-secondtablerelationfield", secondtablerelationfield);
                    input.setAttribute("data-width", width);
                    input.setAttribute("data-showindex", $("#showindex", doc).prop("checked") ? "1" : "0")
                    var id = (secondtable + "-" + primarytablefiled + "-" + secondtableprimarykey + "-" + secondtablerelationfield).toUpperCase();
                    input.setAttribute("id", id);

                    var json = {};
                    json.id = id;
                    json.secondtable = secondtable;
                    json.primarytablefiled = primarytablefiled;
                    json.secondtableprimarykey = secondtableprimarykey;
                    json.secondtablerelationfield = secondtablerelationfield;
                    json.width = width;
                    json.editmodel = $(":checked[name='editmodel']", doc).val();
                    json.editformtype = $("#form_types", doc).val();
                    json.editform = $("#editform", doc).val();
                    json.displaymodewidth = $("#editmodel_width", doc).val();
                    json.displaymodeheight = $("#editmodel_height", doc).val();
                    json.showindex = $("#showindex", doc).prop("checked") ? "1" : "0";
                    json.sortstring = $("#sortstring", doc).val();
                    json.colnums = [];
                    $("#listtable tbody tr", doc).each(function (index) {
                        var field = $("input[type='checkbox'][id^='field_']", $(this)).val();
                        var jstr = $("input[id='set_" + field + "_hidden']", $(this)).val();
                        var colnum = {};
                        colnum.name = (secondtable + "-" + field).toUpperCase();
                        colnum.fieldname = field;
                        colnum.isshow = $("input[type='checkbox'][id^='field_']", $(this)).prop("checked") ? "1" : "0";
                        colnum.showname = $("input[name='name_" + field + "']", $(this)).val();
                        colnum.editmode = jstr.length > 0 ? JSON.parse(jstr) : {};
                        colnum.displaymode = $("[id='field_display_" + field + "']", $(this)).val();
                        colnum.displaymodeformat = $("[id='field_display_" + field + "_format']", $(this)).val();
                        colnum.displaymodesql = $("[id='field_display_" + field + "_sql']", $(this)).val();
                        colnum.issum = $("input[type='checkbox'][name='field_count']", $(this)).prop("checked") ? "1" : "0";
                        colnum.index = $("input[id='field_index_" + field + "']", $(this)).val();
                        colnum.align = $("[id='field_align_" + field + "']", $(this)).val();
                        colnum.width = $("[id='field_width_" + field + "']", $(this)).val();
                        json.colnums.push(colnum);
                    });

                    if(formSubtabsJSON){
                        var isIN = false;
                        for(var i=0;i<formSubtabsJSON.length;i++){
                            if(formSubtabsJSON[i].id == id){
                                formSubtabsJSON[i] = json;
                                isIN = true;
                                break;
                            }
                        }
                        if(!isIN){
                            formSubtabsJSON.push(json);
                        }
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
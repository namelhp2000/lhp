CKEDITOR.plugins.add('rf_datatable', {
    requires: ['dialog'],
    init: function (editor) {
        var pluginName = 'rf_datatable';
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
                if (type == 'text' && 'datatable' == element.getAttribute('data-type')) {
                    currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                    evt.data.dialog = pluginName;
                }
            }
        });
        //注册一个按钮，来调用自定义插件
        editor.ui.addButton(pluginName, {
            label: "数据表格",
            command: pluginName,
            icon: this.path + "datatable.png",
            toolbar: 'rf_plugins,19'
        });
        if (editor.contextMenu) {
            editor.addMenuGroup('rf_plugins');
            editor.addMenuItem(pluginName, {
                label: '数据表格属性',
                command: pluginName,
                group: 'rf_plugins'
            });
            editor.addMenuItem(pluginName + '_delete', {
                label: '删除数据表格',
                command: pluginName + '_delete',
                group: 'rf_plugins'
            });

            //右键菜单的监听器，判断是否显示菜单
            editor.contextMenu.addListener(function (element) {
                if (element && !element.isReadOnly()) {
                    var name = element.getName();
                    if (name == 'input') {
                        var type = element.getAttribute('type');
                        if (type == 'text' && 'datatable' == element.getAttribute('data-type')) {
                            currentSelectEditorElement = element;//保存当前焦点对象 currentSelectEditorElement在index1.jsp中定义
                            return { rf_datatable: CKEDITOR.TRISTATE_OFF, rf_datatable_delete: CKEDITOR.TRISTATE_OFF};
                        }
                    }
                }
            });
        }
        CKEDITOR.dialog.add(pluginName, function (editor) {
            return {
                title: '数据表格',
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
                                html: '<iframe style="width:100%;height:380px;border:0;" frameborder="0" src="../FormDesignerPlugin/DataTable?' + query + '"></iframe>'
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
                        
                        if (element) {
                            id = element.getAttribute("id");
                            var width = element.getAttribute("data-width");
                            var height = element.getAttribute("data-height");
                            var datasource = element.getAttribute("data-datasource");
                            var datasourcetext = element.getAttribute("data-datasourcetext");
                            //var params = element.getAttribute("data-params");
                           
                            $("#width1", doc).val(width);
                            $("#height1", doc).val(height);
                            $("input[name='datasource'][value='" + datasource + "']", doc).prop("checked", true);
                            $("#datasourcetext", doc).val(datasourcetext);
                            //$("#params").val(params);
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
                    var table = attrJSON.dbTable;
                    var doc = $iframe.get(0).contentWindow.document;
                    var bindfiled = $("#bindfiled", doc).val() || '';
                    var width = $("#width1", doc).val() || '';
                    var height = $("#height1", doc).val() || '';
                    var datasource = $(":checked[name='datasource']", doc).val() || '';
                    var datasourcetext = $("#datasourcetext", doc).val() || '';
                    //var params = $("#params", doc).val() || '';

                    var editor1 = this.getParentEditor();
                    var input = null != currentSelectEditorElement ? currentSelectEditorElement : editor1.document.createElement("input");
                    if (!input.getAttribute("type")) {
                        input.setAttribute("type", "text");
                    }
                    input.setAttribute("data-type", "datatable");
                    input.setAttribute("data-isflow", "1");
                    input.setAttribute("value", "数据表格");
                    input.setStyle("color", "#999");
                    input.setAttribute("id", table + "_" + RoadUI.Core.newid(false));
                    //input.setAttribute("name", (table + "-" + bindfiled).toUpperCase());
                    input.setAttribute("data-width", width);
                    input.setAttribute("data-height", height);
                    input.setAttribute("data-datasource", datasource);
                    input.setAttribute("data-datasourcetext", datasourcetext);
                    //input.setAttribute("data-params", params);
 
                    if (null == currentSelectEditorElement) {
                        editor1.insertElement(input);
                    }
                    currentSelectEditorElement = null;
                }
            }
        });

    }
});
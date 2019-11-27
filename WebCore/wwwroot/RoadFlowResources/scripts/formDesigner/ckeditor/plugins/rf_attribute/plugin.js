CKEDITOR.plugins.add('rf_attribute', {
    requires: ['dialog'],
    init: function (editor) {
        var pluginName = 'rf_attribute';
        //给自定义插件注册一个调用命令
        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));
        //注册一个按钮，来调用自定义插件
        editor.ui.addButton(pluginName, {
            label: "表单属性",
            command: pluginName,
            icon: this.path + "attribute.png",
            toolbar: 'rf_plugins,1'
        });
        CKEDITOR.dialog.add(pluginName, function (editor) {
            return {
                title: '表单属性',
                minWidth: 500,
                minHeight: 390,
                resizable: CKEDITOR.DIALOG_RESIZE_WIDTH,
                contents: [{
                    id: pluginName,
                    label: '表单属性',
                    title: '表单属性',
                    elements:
                        [
                            {
                                id: pluginName,
                                type: 'html',
                                html: '<iframe style="width:100%;height:390px;border:0;" frameborder="0" src="../FormDesignerPlugin/Attribute?' + query + '"></iframe>'
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
                    $iframe.show(function () {
                        initAttr();
                    });
                    $iframe.load(function () {
                        initAttr();
                    });
                    function initAttr() {
                        var doc = $iframe.get(0).contentWindow.document;
                        try {
                            $iframe.get(0).contentWindow.initSelect2();
                        } catch (e) { }
                        try {
                            $iframe.get(0).contentWindow.initMember();
                        } catch (e) { }
                    }
                },
                onOk: function () {
                    var $iframe = $("div[name='rf_attribute'] iframe");
                    if ($iframe.size() == 0) {
                        return;
                    }
                    var doc = $iframe.get(0).contentWindow.document;
                    var id = $("#id", doc).val();
                    var name = $("#name", doc).val();
                    var dbconn = $("#dbconn", doc).val();
                    var dbtable = $("#dbtable", doc).val();
                    var dbpk = $("#dbpk", doc).val();
                    var dbtitle = $("#dbtitle", doc).val();
                    var dbtitle1 = $("#dbtitle1", doc).val();
                    var type = $("#type", doc).val();
                    var manageUser = $("#manageuser", doc).val();
                    var validatePromptType = $(":checked[name='validatePromptType']", doc).val();
                    formAttributeJSON.id = id;
                    formAttributeJSON.name = $.trim(name);
                    formAttributeJSON.dbConn = dbconn;
                    formAttributeJSON.dbTable = dbtable;
                    formAttributeJSON.dbTablePrimaryKey = dbpk;
                    formAttributeJSON.dbTableTitle = dbtitle;
                    formAttributeJSON.dbTableTitleExpression = dbtitle1;
                    formAttributeJSON.formType = type;
                    formAttributeJSON.manageUser = manageUser;
                    formAttributeJSON.validatePromptType = validatePromptType;
                }
            }
        });

    }
});
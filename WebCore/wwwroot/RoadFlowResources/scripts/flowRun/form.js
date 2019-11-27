var formLoad = {
    //加载表单数据
    //formDataJSON 数据JSON
    //fieldStatusJSON 字段状态JSON
    //tableName 表名
    //displayModel 显示方式 1只读 其它按字段状态显示
    //defaultValuesJSON 默认值JSON
    load: function (formDataJSON, fieldStatusJSON, tableName, displayModel, defaultValuesJSON) {
        if (!formDataJSON || !fieldStatusJSON) {
            return;
        }
        if (!displayModel) {
            displayModel = RoadUI.Core.queryString("display");
        }
        //计算子表合计
        $("[data-isflowsubtable='1']").each(function () {
            var $tds = $("tfoot tr td", $(this));
            for (var i = 0; i < $tds.size() ; i++) {
                if ($tds.eq(i).attr("data-table") && $tds.eq(i).attr("data-field")) {
                    formLoad.subTableSum($tds.eq(i).attr("data-table"), $tds.eq(i).attr("data-field"));
                }
            }
            //子表如果有一个字段为只读，则要屏蔽增加删除按钮
            var isReadOnly = false;
            var $tds1 = $("tbody tr td", $(this));
            for (var i = 0; i < $tds1.size() ; i++) {
                var $ele = $("[data-table][data-field]", $tds1.eq(i));
                if ($ele.size() > 0) {
                    var fieldStatus = formLoad.getFieldStatus(fieldStatusJSON, ($ele.attr("data-table") + "-" + $ele.attr("data-field")).toUpperCase());
                    var fieldStatus = "1" == displayModel ? 1 : fieldStatus ? fieldStatus.status : 0;//字段状态 0编辑 1只读 2隐藏
                    if ("1" == fieldStatus || 2 == fieldStatus) {
                        isReadOnly = true;
                        break;
                    }
                }
            }
            if (isReadOnly) {
                $("thead tr", $(this)).each(function () {
                    $("th:last", $(this)).hide();
                });
                $("tbody tr", $(this)).each(function () {
                    $("td:last", $(this)).hide();
                });
                $("tfoot tr", $(this)).each(function () {
                    $("td:last", $(this)).hide();
                });
            }
        });
        $("[data-isflow='1']").each(function () {
            var $element = $(this);
            var ctlType = $element.attr("data-type");//控件类型
            var name = $element.attr("name");
            var id = $element.attr("id");
            var fieldValue = "";
            if ("organize" == ctlType || "lrselect" == ctlType || "files" == ctlType || "signature" == ctlType
                || "selectdiv" == ctlType || "files" == ctlType) {
                id = name.substr(0, name.lastIndexOf("_text"));
            } else if ("radio" == ctlType || "checkbox" == ctlType) {//如果是单选或复选则ID要取name,否则无法从JSON中取到值
                id = name;
            }
            var fieldStatusId = id;
            if ("1" == $element.attr("data-issubtable")) {
                fieldStatusId = ($element.attr("data-table") + "-" + $element.attr("data-field")).toUpperCase();
            }
            var fieldStatusj = formLoad.getFieldStatus(fieldStatusJSON, fieldStatusId);
            var fieldStatus = "1" == displayModel ? 1 : fieldStatusj ? fieldStatusj.status : 0;//字段状态 0编辑 1只读 2隐藏
            var fieldCheck = fieldStatusj ? fieldStatusj.check : 0;//字段检查 0不检查 1允许为空,非空时检查 2不允许为空,并检查

            if ("1" == $element.attr("data-issubtable")) {
                if ("checkbox" == ctlType || "radio" == ctlType || "select" == ctlType || "organize" == ctlType
                    || "lrselect" == ctlType || "files" == ctlType) {
                    fieldValue = $element.attr("data-value");
                } else {
                    fieldValue = $element.val();
                }
            } else {
                fieldValue = formLoad.getFieldValue(formDataJSON, id);//字段值
            }
            //if (formDataJSON && formDataJSON.length == 0 && (!fieldValue || $.trim(fieldValue).length == 0)) { //如果数据表中没有值，则查找是否有默认值
            //if (formDataJSON && formDataJSON.length == 0 && (!fieldValue || $.trim(fieldValue).length == 0)) { //如果数据表中没有值，则查找是否有默认值
            //if ((!fieldValue || $.trim(fieldValue).length == 0) && fieldStatus == 0) { //如果数据表中没有值，则查找是否有默认值
            if ((!fieldValue || $.trim(fieldValue).length == 0)) { //如果数据表中没有值，则查找是否有默认值 去掉了 && fieldStatus == 0 条件只读也可以取默认值
                var defaultValueId = id;
                if (typeof ($element.attr("data-defaultvalueid")) != 'undefined') {
                    defaultValueId = $element.attr("data-defaultvalueid");
                }
                fieldValue = formLoad.getFieldDefaultValue(defaultValuesJSON, defaultValueId);
            }
            var validateType = $element.attr("validate") || "empty";
            //如果设置的数据类型，如（int）又不检查，则要加上可空
            if (0 == fieldCheck && $element.attr("validate") && $.trim($element.attr("validate")).length > 0) {
                $element.attr("validate", "cannull," + validateType);
            }
            switch (ctlType) {
                case "text":
                    if (0 == fieldStatus) {
                        $element.val(fieldValue);
                    } else if (1 == fieldStatus) {
                        $element.after('<span>' + fieldValue + '</span>').remove();
                    } else if (2 == fieldStatus) {
                        $element.remove();
                    }
                    if (1 == fieldCheck) {
                        $element.attr("validate", "cannull," + validateType);
                    } else if (2 == fieldCheck) {
                        $element.attr("validate", validateType);
                    }
                    break;
                case "textarea":
                    if (0 == fieldStatus) {
                        $element.text(fieldValue);
                    } else if (1 == fieldStatus) {
                        $element.after('<span style="word-break:break-all;word-wrap:break-word;">' + fieldValue + '</span>').remove();
                    } else if (2 == fieldStatus) {
                        $element.remove();
                    }
                    if (1 == fieldCheck) {
                        $element.attr("validate", "cannull," + validateType);
                    } else if (2 == fieldCheck) {
                        $element.attr("validate", validateType);
                    }
                    break;
                case "select":
                case "select2":
                    var linkage_field = $element.attr("data-linkage_field");
                    if (linkage_field && $.trim(linkage_field).length > 0) {
                        $element.val(fieldValue).attr("data-value", fieldValue).change();
                    }
                    if (0 == fieldStatus) {
                        if ($element.prop("multiple")) {
                            if (fieldValue && $.trim(fieldValue).length > 0) {
                                $element.children().each(function () {
                                    if ((',' + fieldValue + ",").indexOf(',' + $(this).val() + ',') >= 0) {
                                        $(this).prop("selected", true);
                                    }
                                });
                                new RoadUI.Select().init2($element);
                            }
                        } else {
                            $element.val(fieldValue);
                        }
                        $element.attr("data-value", fieldValue);
                    } else if (1 == fieldStatus) {
                        if (fieldValue && $.trim(fieldValue).length > 0) {
                            var defaultText = new Array();
                            $element.children().each(function () {
                                if ((',' + fieldValue + ",").indexOf(',' + $(this).val() + ',') >= 0) {
                                    defaultText.push($(this).text());
                                    //return true;
                                }
                            });
                            if (defaultText.length > 0) {
                                $element.after(defaultText.join("，"));
                            }
                        }
                        if ('select2' == ctlType) {
                            $element.next(".select2").remove();
                        }
                        $element.remove();
                    } else if (2 == fieldStatus) {
                        if ('select2' == ctlType) {
                            $element.next(".select2").remove();
                        }
                        $element.remove();
                    }
                    if (1 == fieldCheck) {
                        $element.attr("validate", "cannull," + validateType);
                    } else if (2 == fieldCheck) {
                        $element.attr("validate", validateType);
                    }
                    break;
                case "selectdiv":
                    if (0 == fieldStatus) {
                        $element.prev().val(fieldValue);
                        if (fieldValue && $.trim(fieldValue).length > 0) {
                            new RoadUI.SelectDiv().setValue($element.prev());
                        }
                    } else if (1 == fieldStatus) {
                        $element.prev().val(fieldValue);
                        if (fieldValue && $.trim(fieldValue).length > 0) {
                            new RoadUI.SelectDiv().setValue($element.prev());
                        }
                        $element.prev().remove();
                        $element.next().remove();
                        $element.after($element.val());
                        $element.remove();
                    } else if (2 == fieldStatus) {
                        $element.prev().remove();
                        $element.next().remove();
                        $element.remove();
                    }
                    if (1 == fieldCheck) {
                        $element.attr("validate", "cannull," + validateType);
                    } else if (2 == fieldCheck) {
                        $element.attr("validate", validateType);
                    }
                    break;
                case "radio":
                    if (0 == fieldStatus) {
                        $element.prop("checked", fieldValue == $element.val());
                    } else if (1 == fieldStatus) {
                        if (fieldValue == $element.val()) {
                            $element.remove();
                        } else {
                            $element.next("label").remove();
                            $element.remove();
                        }
                    } else if (2 == fieldStatus) {
                        $element.next("label").remove();
                        $element.remove();
                    }
                    if (1 == fieldCheck) {
                        $element.attr("validate", "cannull,radio");
                    } else if (2 == fieldCheck) {
                        $element.attr("validate", "radio");
                    }
                    break;
                case "checkbox":
                    if (0 == fieldStatus) {
                        $element.prop("checked", ("," + fieldValue + ",").indexOf("," + ($element.val() || "") + ",") >= 0);
                    } else if (1 == fieldStatus) {
                        if (("," + fieldValue + ",").indexOf("," + ($element.val() || "") + ",") >= 0) {
                            $element.next("label").after("&nbsp;&nbsp;");
                            $element.remove();
                        } else {
                            $element.next("label").remove();
                            $element.remove();
                        }
                    } else if (2 == fieldStatus) {
                        $element.next("label").remove();
                        $element.remove();
                    }
                    if (1 == fieldCheck) {
                        $element.attr("validate", "cannull,checkbox");
                    } else if (2 == fieldCheck) {
                        $element.attr("validate", "checkbox");
                    }
                    break;
                case "hidden":
                    if (0 == fieldStatus) {
                        $element.val(fieldValue);
                    } else if (1 == fieldStatus) {
                        $element.remove();
                    } else if (2 == fieldStatus) {
                        $element.remove();
                    }
                    if (1 == fieldCheck) {
                        $element.attr("validate", "cannull," + validateType);
                    } else if (2 == fieldCheck) {
                        $element.attr("validate", validateType);
                    }
                    break;
                case "html":
                    if (0 == fieldStatus) {
                        $element.val(fieldValue);
                    } else if (1 == fieldStatus) {
                        $element.after(fieldValue);
                        var id = $element.attr("id");
                        for (var editor in CKEDITOR.instances) {
                            if (editor == id) {
                                //CKEDITOR.remove(CKEDITOR.instances[editor]);
                                CKEDITOR.instances[editor].destroy();
                                $element.remove();
                            }
                        }
                    } else if (2 == fieldStatus) {
                        var id = $element.attr("id");
                        for (var editor in CKEDITOR.instances) {
                            if (editor == id) {
                                CKEDITOR.instances[editor].destroy();
                                $element.remove();
                            }
                        }
                    }
                    break;
                case "label":
                    var bindfiled = $element.attr("data-bindfiled");
                    if (0 == fieldStatus) {
                        $element.html(fieldValue);
                        if (bindfiled && bindfiled.length > 0) {
                            $("#" + $element.attr("id") + "_hidden").val(fieldValue);
                        }
                    } else if (1 == fieldStatus) {
                        $element.html(fieldValue);
                        if (bindfiled && bindfiled.length > 0) {
                            $("#" + $element.attr("id") + "_hidden").remove();
                        }
                    } else if (2 == fieldStatus) {
                        $element.remove();
                        if (bindfiled && bindfiled.length > 0) {
                            $("#" + $element.attr("id") + "_hidden").remove();
                        }
                    }
                    break;
                case "datetime":
                    if (0 == fieldStatus) {
                        $element.val(fieldValue);
                    } else if (1 == fieldStatus) {
                        $element.after(fieldValue).remove();
                    } else if (2 == fieldStatus) {
                        $element.remove();
                    }
                    if (1 == fieldCheck) {
                        $element.attr("validate", "cannull," + validateType);
                    } else if (2 == fieldCheck) {
                        $element.attr("validate", validateType);
                    }
                    break;
                case "organize":
                    if (0 == fieldStatus) {
                        $element.prev().val(fieldValue);
                        if (fieldValue && $.trim(fieldValue).length > 0) {
                            new RoadUI.Member().setValue($element.prev());
                        }
                    } else if (1 == fieldStatus) {
                        $element.prev().val(fieldValue);
                        if (fieldValue && $.trim(fieldValue).length > 0) {
                            new RoadUI.Member().setValue($element.prev());
                        }
                        $element.prev().remove();
                        $element.next().remove();
                        $element.after($element.val());
                        $element.remove();
                    } else if (2 == fieldStatus) {
                        $element.prev().remove();
                        $element.next().remove();
                        $element.remove();
                    }
                    if (1 == fieldCheck) {
                        $element.attr("validate", "cannull," + validateType);
                    } else if (2 == fieldCheck) {
                        $element.attr("validate", validateType);
                    }
                    break;
                case "lrselect":
                    if (0 == fieldStatus) {
                        $element.prev().val(fieldValue);
                        if (fieldValue && $.trim(fieldValue).length > 0) {
                            new RoadUI.Dict().setValue($element.prev());
                        }
                    } else if (1 == fieldStatus) {
                        $element.prev().val(fieldValue);
                        if (fieldValue && $.trim(fieldValue).length > 0) {
                            new RoadUI.Dict().setValue($element.prev());
                        }
                        $element.prev().remove();
                        $element.next().remove();
                        $element.after($element.val());
                        $element.remove();
                    } else if (2 == fieldStatus) {
                        $element.prev().remove();
                        $element.next().remove();
                        $element.remove();
                    }
                    if (1 == fieldCheck) {
                        $element.attr("validate", "cannull," + validateType);
                    } else if (2 == fieldCheck) {
                        $element.attr("validate", validateType);
                    }
                    break;
                case "files":
                    if (0 == fieldStatus) {
                        if ($.trim(fieldValue).length > 0) {
                            $element.val('共' + fieldValue.split('|').length + '个文件');
                        }
                        $element.prev().val(fieldValue);
                    }
                    else if (1 == fieldStatus) {
                        var links = '';
                        if ($.trim(fieldValue).length > 0) {
                            data_fileshow = $element.attr("data-fileshow");//显示类型 为1时显示为图片
                            data_fileshowwidth = $element.attr("data-fileshowwidth");//图片宽度
                            data_fileshowheight = $element.attr("data-fileshowheight");//图片高度
                            $.ajax({
                                url: "/RoadFlowCore/Controls/UploadFiles_GetShowString",
                                data: { "files": fieldValue, "showtype": data_fileshow || "", "width": data_fileshowwidth || "", "height": data_fileshowheight || "" },
                                type: "post",
                                success: function (links) {
                                    $element.next().remove();
                                    $element.prev().remove();
                                    $element.after(links);
                                    $element.remove();
                                }
                            });
                        }
                        else {
                            links = "无";
                            $element.next().remove();
                            $element.prev().remove();
                            $element.after(links);
                            $element.remove();
                        }
                    }
                    else if (2 == fieldStatus) {
                        $element.prev().remove();
                        $element.next().remove();
                        $element.remove();
                    }
                    if (1 == fieldCheck) {
                        $element.attr("validate", "cannull," + validateType);
                    } else if (2 == fieldCheck) {
                        $element.attr("validate", validateType);
                    }
                    break;
                case "serialnumber":
                    if (0 == fieldStatus) {
                        $element.val(fieldValue);
                    } else if (1 == fieldStatus) {
                        $element.after('<span>' + fieldValue + '</span>').remove();
                    } else if (2 == fieldStatus) {
                        $element.remove();
                    }
                    if (1 == fieldCheck) {
                        $element.attr("validate", "cannull," + validateType);
                    } else if (2 == fieldCheck) {
                        $element.attr("validate", validateType);
                    }
                    break;
                case "button":
                    if (1 == displayModel) {
                        fieldStatus = 2;//如果是查看模式，不显示按钮
                    }
                    if (1 == fieldStatus) {
                        $element.prop('disabled', true);
                    } else if (2 == fieldStatus) {
                        $element.remove();
                    }
                    break;
                case "signature":
                    id1 = $element.attr("data-id");
                    if (0 == fieldStatus || 1 == fieldStatus) {
                        if (fieldValue && fieldValue.length > 0) {
                            $element.attr("data-src", fieldValue);
                            $("#" + id1).val(fieldValue);
                            $element.after('<span><img style="vertical-align:middle;margin-left:' + (1 == fieldStatus ? '0' : '10') + 'px;" src="' + fieldValue + '" border="0" /></span>');
                        }
                        if (1 == fieldStatus) {
                            $("#" + id1).remove();
                            $element.remove();
                        }
                    } else if (2 == fieldStatus) {
                        $("#" + id1).remove();
                        $element.remove();
                    }
                    if (1 == fieldCheck) {
                        //$("#" + id1).attr("validate", "cannull," + validateType);
                    } else if (2 == fieldCheck) {
                        $("#" + id1).attr("validate", validateType).attr("errmsg", "请签章!");
                    }
                    break;
            }
        });
    },
    //得到字段状态和检查
    getFieldStatus: function (fieldStatusJSON, field) {
        if (!field || $.trim(field).length == 0)
        {
            return null;
        }
        for (var i = 0; i < fieldStatusJSON.length; i++) {
            if (field.toUpperCase() == fieldStatusJSON[i].name) {
                return fieldStatusJSON[i];
            }
        }
        return null;
    },
    //得到字段值
    getFieldValue: function (formDataJSON, field) {
        if (!field || $.trim(field).length == 0) {
            return null;
        }
        for (var i = 0; i < formDataJSON.length; i++) {
            if (field.toUpperCase() == formDataJSON[i].name) {
                return formDataJSON[i].value;
            }
        }
        return "";
    },
    //得到字段默认值
    getFieldDefaultValue: function (defaultValuesJSON, field) {
        if (!field || $.trim(field).length == 0) {
            return "";
        }
        if (defaultValuesJSON && $.isArray(defaultValuesJSON)) {
            for (var j = 0; j < defaultValuesJSON.length; j++) {
                if (field.toUpperCase() == defaultValuesJSON[j].id) {
                    return defaultValuesJSON[j].value;
                }
            }
        }
        return "";
    },
    //子表添加一行
    subtableAddRow: function (tableId, $tr, isFirst) {
        var $newtr = null;
        var $table = $("#" + tableId);
        if ($tr.size() == 0) {
            $newtr = $table.data("data-firstrow").clone();
            $("tbody", $table).append($newtr);
        } else {
            var $radio = $("input[type='radio']:checked", $tr);//记录下当前行选中的radio，以便后面重新选中，radio clone后以前先中的会变为没有选中
            $newtr = $tr.clone(true);
            (!isFirst ? $tr : $("tbody tr:last", $table)).after($newtr);
            if ($radio.size() > 0) {
                $("input[type='radio'][id='" + $radio.attr("id") + "']", $tr).prop("checked", true);//重新选中clone之前的选中项
            }
        }
        var rowIndex = RoadUI.Core.newid(false).toUpperCase();
        $("[name='" + tableId + "_rowindex']", $newtr).val(rowIndex);
        formLoad.setTableIndex($table);
        $("[data-isflow]", $newtr).each(function (index) {
            var $this = $(this);
            var field = $this.attr("data-field");
            var idName = tableId + "_" + field + "_" + rowIndex;
            var dataType = $this.attr("data-type");
            var defaultValue = formLoad.getFieldDefaultValue(defaultValuesJSON, $this.attr("data-defaultvalueid"));
            switch (dataType) {
                case "text":
                case "textarea":
                    $this.attr("id", idName).attr("name", idName);
                    $this.val(defaultValue);
                    break;
                case "select":
                    $this.attr("id", idName).attr("name", idName);
                    $this.val(defaultValue);
                    if ($this.attr("onchange")) {
                        $this.change();
                    }
                    break;
                case "checkbox":
                case "radio":
                    $this.attr("id", idName).attr("name", idName);
                    $this.attr("id", idName + index.toString()).next("label").attr("for", idName + index.toString());
                    $this.prop("checked", $this.val() == defaultValue);
                    break;
                case "datetime":
                    $this.attr("id", idName).attr("name", idName);
                    $this.val(defaultValue);
                    new RoadUI.Calendar().init($this);
                    break;
                case "organize":
                    $this.prev().attr("id", idName);
                    $this.prev().attr("name", idName);
                    $this.prev().val(defaultValue);
                    $this.attr("id", idName + "_text").attr("name", idName + "_text");
                    if (defaultValue && $.trim(defaultValue).length > 0) {
                        new RoadUI.Member().setValue($this.prev());
                    } else {
                        $this.val("");
                    }
                    break;
                case "lrselect":
                    $this.prev().attr("id", idName);
                    $this.prev().attr("name", idName);
                    $this.prev().val(defaultValue);
                    $this.attr("id", idName + "_text").attr("name", idName + "_text");
                    if (defaultValue && $.trim(defaultValue).length > 0) {
                        new RoadUI.Dict().setValue($this.prev());
                    } else {
                        $this.val("");
                    }
                    break;
                case "files":
                    $this.prev().attr("id", idName);
                    $this.prev().attr("name", idName);
                    $this.prev().val(defaultValue);
                    $this.attr("id", idName + "_text").attr("name", idName + "_text");
                    $this.val("");
                    break;
            }
            var initDataType = dataType
            if ("textarea" == initDataType || "datetime" == initDataType || "organize" == initDataType || "lrselect" == initDataType) {
                initDataType = "text";
            }
            try {
                if (currentFocusObj) {
                    $this.mouseout();
                }
            } catch (e) { }
            initElement($this, initDataType);
        });
    },
    //子表删除一行
    subtableDelRow: function ($tr) {
        var $table = $tr.parent().parent();
        $table.data("data-firstrow", $tr);
        $tr.remove();
        //重新设置序号
        formLoad.setTableIndex($table);
        //重新计算合计
        var $tds = $("tfoot tr td", $table);
        for (var i = 0; i < $tds.size() ; i++) {
            if ($tds.eq(i).attr("data-table") && $tds.eq(i).attr("data-field")) {
                formLoad.subTableSum($tds.eq(i).attr("data-table"), $tds.eq(i).attr("data-field"));
            }
        }
    },
    //重置一个表的序号列的数据
    setTableIndex: function ($table) {
        $("[data-tdindex='1']", $table).each(function (index) {
            $(this).text(index + 1);
        });
    },
    //计算合计
    subTableSum: function (tableId, field) {
        var $table = $("#" + tableId);
        var $eles = $("[id^='" + tableId + "_" + field + "']", $("tbody", $table));
        var sum = 0;
        $eles.each(function () {
            var val = $(this).val();
            if (parseFloat(val)) {
                sum = RoadUI.Core.accAdd(sum, parseFloat(val));
            }
        });
        $("#SUM_" + tableId + "_" + field, $("tfoot", $table)).text(sum);
    },
    //载入联动项 source:sql,url,dict dictValueField:当来源为数据字典时的值字段, dictId联动字段选定的DICT根ID hasempty:为1添加空选项 isSubTable:是否是子表 dictIschild数据字典是否加载下级
    loadChildOptions: function (srcElement, lindField, source, connId, text, dictValueField, dictId, hasempty, isSubTable, dictIschild) {
        var value = srcElement.value;
        var $lindField = isSubTable ? $("[name^='" + lindField + "']", $(srcElement).parent().parent()) : $("#" + lindField);
        var defaultValue = $lindField.val();
        if (!defaultValue || $.trim(defaultValue).length == 0) {
            defaultValue = $lindField.attr("data-value");
        }
        var emptyTitle = $lindField.attr("data-emptyTitle");
        $.ajax({
            url: "../FormDesigner/GetChildOptions",
            type: "post",
            async: false,
            data: {
                "source": source,
                "value": value,
                "connid": connId,
                "text": RoadUI.Core.unescape(text),
                "dictvaluefield": dictValueField,
                "dictid": dictId,
                "dictIschild": dictIschild ? 1 : 0,
                "defaultvalue": defaultValue || ""
            },
            success: function (opts) {
                $lindField.html(("1" == hasempty ? '<option value="">' + (emptyTitle || '') + '</option>' : '') + opts);
            }
        });
    },
    //子表添加行（弹出模式） appId 表单应用程序库ID, width 弹出窗口宽度 height 弹出窗口高度 flowId 流程ID StepId 步骤ID 
    //instanceId 主表实例ID secondtable 从表表名 secondtablerelationfield 子表与主表的关系字段 iframeId 主表窗口ID
    subtableAddData: function (appId, width, height, flowId, StepId, instanceId, secondtable, secondtablerelationfield, iframeId, editId) {
        if (!instanceId || $.trim(instanceId).length == 0)
        {
            alert('请先保存,再添加!');
            return;
        }
        var url = '/RoadFlowCore/FlowRun/FormEdit?applibraryid=' + appId + '&rf_appopenmodel=2&primarytableid=' + instanceId
            + '&flowid=' + flowId + "&stepid=" + StepId + '&secondtable=' + secondtable + '&secondtablerelationfield=' + secondtablerelationfield + "&instanceid=" + (editId || "");
        new RoadUI.Window().open({ url: url, title:'添加', width: width, height: height, openerid: iframeId });
    },
    //子表删除行（弹出模式）connId 数据连接ID secondtable 从表表名 secondtablePrimaryKey 从表主键 secondtableId 从表主键值 but 按钮对象
    subtableDelData: function (connId, secondtable, secondtablePrimaryKey, secondtableId, but) {
        if (!confirm('您真的要删除吗?'))
        {
            return;
        }
        $(but).prop("disabled", true);
        $.ajax({
            url: "/RoadFlowCore/FlowRun/FormDelete?connId=" + connId + "&secondtable=" + secondtable + "&secondtablePrimaryKey="
                + secondtablePrimaryKey + "&secondtableId=" + secondtableId,
            data: { "__RequestVerificationToken": $("input[name='__RequestVerificationToken']").val() },
            type: "post",
            success: function (txt) {
                alert(txt);
                if (txt.indexOf("成功") >= 0) {
                    window.location = window.location;
                }
            }
        });
    },

    //ckeditor编辑器完整工具栏
    ckeditor_toolbarFullGroups: [
        { name: 'document', groups: ['mode', 'document', 'doctools'] },
        { name: 'clipboard', groups: ['clipboard', 'undo'] },
        { name: 'editing', groups: ['find', 'selection', 'spellchecker', 'editing'] },
        { name: 'basicstyles', groups: ['basicstyles', 'cleanup'] },
        { name: 'paragraph', groups: ['blocks'] },
        { name: 'links', groups: ['links'] },
        { name: 'insert', groups: ['insert'] },
        '/',
        { name: 'styles', groups: ['styles'] },
        { name: 'colors', groups: ['colors', 'align', 'list', 'indent'] },
        { name: 'tools', groups: ['tools'] },
        { name: 'others', groups: ['others'] },
        { name: 'about', groups: ['about'] }
    ],
    //ckeditor编辑器简洁工具栏
    ckeditor_toolbarMessageGroups: [
        { name: 'styles', groups: ['styles'] },
        { name: 'basicstyles', groups: ['basicstyles'] },
        { name: 'colors', groups: ['colors', 'align', 'links', 'insert'] },
        { name: 'about', groups: ['about'] }
    ],
    //ckeditor编辑器极简工具栏
    ckeditor_toolbarMobileGroups: [
        { name: 'basicstyles', groups: ['colors', 'align', 'basicstyles', 'insert'] },
        { name: 'about', groups: ['about'] }
    ]
};
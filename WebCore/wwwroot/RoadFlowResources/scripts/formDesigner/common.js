var links_tables_fields_formdesinger = new Array();
var links_tables_fields_formdesinger_dbs = new Array();

//得到一连接的所有表
function getTables(connid, table) {
    var html = "";
    $.ajax({
        url: "../Dbconnection/GetTableOptions?connid=" + connid + "&table=" + (table || "") + "&" + query,
        async: false,
        success: function (options) {
            html = options;
        }
    });
    return html;
}

//得到一个表所有字段
function getFields(connid, table, field, editor) {
    var isDbsIn = false;
    for (var i = 0; i < links_tables_fields_formdesinger_dbs.length; i++) {
        if (links_tables_fields_formdesinger_dbs[i].link == connid && links_tables_fields_formdesinger_dbs[i].table == table && !isDbsIn) {
            isDbsIn = true;
            break;
        }
    }
    if (!isDbsIn) {
        var dbs = {"link": connid, "table": table};
        links_tables_fields_formdesinger_dbs.push(dbs);
        init_links_tables_fields_formdesinger(links_tables_fields_formdesinger_dbs);
    }

    if (links_tables_fields_formdesinger.length > 0) {
        var filedArray = [];
        for (var i = 0; i < links_tables_fields_formdesinger.length; i++) {
            if (links_tables_fields_formdesinger[i].table == table && links_tables_fields_formdesinger[i].connId == connid) {
                for (var j = 0; j < links_tables_fields_formdesinger[i].fields.length; j++) {
                    var fname = links_tables_fields_formdesinger[i].fields[j].name;
                    if (fname && fname.length > 0) {
                        filedArray.push(links_tables_fields_formdesinger[i].fields[j]);
                    }
                }
            }
        }

        if (filedArray.length > 0) {
            var options = "";
            for (var i = 0; i < filedArray.length; i++)
            {
                options += '<option value="' + filedArray[i].name + '"';
                if (field && field.length>0 && filedArray[i].name.toUpperCase() == field.toUpperCase()) {
                    options += ' selected="selected"';
                }
                options += '>';
                options += filedArray[i].name;

                //if (editor && editor.document.getById((table + "-" + filedArray[i].name).toUpperCase()))
                //{
                //    options += " - 已绑定";
                //}
                
                if (filedArray[i].comment && filedArray[i].comment.length > 0) {
                    options += '(' + filedArray[i].comment + ')';
                }
                options += '</option>';
            }
            return options;
        }
    }
    return "";
}

//加载连接的表和字段
function init_links_tables_fields_formdesinger(dbs) {
    $.ajax({
        url: "../Dbconnection/GetTableJSON?" + query,
        dataType: "json",
        data: { "dbs": JSON.stringify(dbs)},
        type: "post",
        async: false,
        success: function (json) {
            links_tables_fields_formdesinger = json;
        }
    });
}

//得到默认值下拉选项
function getDefaultValueOptions() {
    var opts = '<option></option>'
        + '<optgroup label="组织机构相关选项"></optgroup>'
        + '<option value="{<UserId>}">当前用户ID</option>'
        + '<option value="{<UserName>}">当前用户姓名</option>'
        + '<option value="{<UserDeptId>}">当前用户部门ID</option>'
        + '<option value="{<UserDeptName>}">当前用户部门名称</option>'
        + '<option value="{<UserStationId>}">当前用户岗位ID</option>'
        + '<option value="{<UserStationName>}">当前用户岗位名称</option>'
        + '<option value="{<UserWorkGroupId>}">当前用户工作组ID</option>'
        + '<option value="{<UserWorkGroupName>}">当前用户工作组名称</option>'
        + '<option value="{<UserDeptLeaderId>}">当前用户部门领导ID</option>'
        + '<option value="{<UserDeptLeaderName>}">当前用户部门领导姓名</option>'
        + '<option value="{<UserCharegLeaderId>}">当前用户分管领导ID</option>'
        + '<option value="{<UserCharegLeaderName>}">当前用户分管领导姓名</option>'
        + '<option value="{<UserUnitId>}">当前用户单位ID</option>'
        + '<option value="{<UserUnitName>}">当前用户单位名称</option>'
        + '<option value="{<InitiatorId>}">发起者ID</option>'
        + '<option value="{<InitiatorName>}">发起者姓名</option>'
        + '<option value="{<InitiatorDeptId>}">发起者部门ID</option>'
        + '<option value="{<InitiatorDeptName>}">发起者部门名称</option>'
        + '<option value="{<InitiatorStationId>}">发起者岗位ID</option>'
        + '<option value="{<InitiatorStationName>}">发起者岗位名称</option>'
        + '<option value="{<InitiatorWorkGroupId>}">发起者工作组ID</option>'
        + '<option value="{<InitiatorWorkGroupName>}">发起者工作组名称</option>'
        + '<option value="{<InitiatorUnitId>}">发起者单位ID</option>'
        + '<option value="{<InitiatorUnitName>}">发起者单位名称</option>'
        + '<option value="{<InitiatorLeaderId>}">发起者部门领导ID</option>'
        + '<option value="{<InitiatorLeaderName>}">发起者部门领导姓名</option>'
        + '<option value="{<InitiatorCharegId>}">发起者分管领导ID</option>'
        + '<option value="{<initiatorCharegName>}">发起者分管领导姓名</option>'
        + '<optgroup label="日期时间相关选项"></optgroup>'
        + '<option value="{<ShortDate>}">短日期格式(yyyy-MM-dd)</option>'
        + '<option value="{<LongDate>}">长日期格式(yyyy年MM月dd日)</option>'
        + '<option value="{<ShortDateTime>}">短日期时间(yyyy-MM-dd HH:mm)</option>'
        + '<option value="{<LongDateTime>}">长日期时间格式(yyyy年MM月dd日 HH时mm分)</option>'
        + '<optgroup label="流程实例相关选项"></optgroup>'
        + '<option value="{<FlowId>}">当前流程ID</option>'
        + '<option value="{<FlowName>}">当前流程名称</option>'
        + '<option value="{<StepId>}">当前步骤ID</option>'
        + '<option value="{<StepName>}">当前步骤名称</option>'
        + '<option value="{<TaskId>}">当前任务ID</option>'
        + '<option value="{<InstanceId>}">当前实例ID</option>'
        + '<option value="{<GroupId>}">当前实例组ID</option>'
        + '<option value="{<PrevInstanceId>}">前一步实例ID</option>'
        + '<option value="{<PrevFlowTitle>}">前一步流程任务标题</option>';
    return opts;
}

//得到输入类型下拉选项
function getValueTypeOptions() {
    var opts = ''
        + '<option value="empty">字符串</option>'
        + '<option value="decimal">数字</option>'
        + '<option value="int">整数</option>'
        + '<option value="positivefloat">正数</option>'
        + '<option value="negativefloat">负数</option>'
        + '<option value="positiveint">正整数</option>'
        + '<option value="negativeint">负整数</option>'
        + '<option value="mobile">手机号码</option>';
    return opts;
}

//得到事件下拉选项
function getEventsOptions() {
    var opts = '<option value=""></option>'
        + '<option value="onchange">onChange</option>'
        + '<option value="onclick">onClick</option>'
        + '<option value="ondblclick">onDblclick</option>'
        + '<option value="onblur">onBlur</option>'
        + '<option value="onfocus">onFocus</option>'
        + '<option value="onkeydown">onKeydown</option>'
        + '<option value="onkeypress">onKeypress</option>'
        + '<option value="onkeyup">onKeyup</option>'
        + '<option value="onmousedown">onMousedown</option>'
        + '<option value="onmouseup">onMouseup</option>'
        + '<option value="onmouseover">onMouseover</option>'
        + '<option value="onmouseout">onMouseout</option>';
    return opts;
}

//得到数据来源下拉选项
function getDataSourceOptions(name, value, events) {
    var opts = '<input style="vertical-align:middle;" ' + events + ' type="radio" value="0" name="' + name + '" id="' + name + '_ds0"' + ('0' == value ? ' checked="checked"' : '') + '/>'
        + '<label style="vertical-align:middle;" for="' + name + '_ds0">数据字典</label>'
        + '<input style="vertical-align:middle;" ' + events + ' type="radio" value="1" name="' + name + '" id="' + name + '_ds1"' + ('1' == value ? ' checked="checked"' : '') + '/>'
        + '<label style="vertical-align:middle;" for="' + name + '_ds1">字符串</label>'
        + '<input style="vertical-align:middle;" ' + events + ' type="radio" value="2" name="' + name + '" id="' + name + '_ds2"' + ('2' == value ? ' checked="checked"' : '') + '/>'
        + '<label style="vertical-align:middle;" for="' + name + '_ds2">SQL</label>'
        + '<input style="vertical-align:middle;" ' + events + ' type="radio" value="3" name="' + name + '" id="' + name + '_ds3"' + ('3' == value ? ' checked="checked"' : '') + '/>'
        + '<label style="vertical-align:middle;" for="' + name + '_ds3">URL</label>'
    return opts;
}

//在文本框光标入插入文本
function insertElementText(obj, str) {
    if (typeof obj.selectionStart === 'number' && typeof obj.selectionEnd === 'number') {
        var startPos = obj.selectionStart,
            endPos = obj.selectionEnd,
            cursorPos = startPos,
            tmpStr = obj.value;
        obj.value = tmpStr.substring(0, startPos) + str + tmpStr.substring(endPos, tmpStr.length);
        cursorPos += str.length;
        obj.selectionStart = obj.selectionEnd = cursorPos;
    } else {
        obj.value += str;
    }
}

//转义html脚本
function escapeHTML(a) {
    a = "" + a;
    return a.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&apos;");
}

//还原html脚本
function unescapeHTML (a) {
    a = "" + a;
    return a.replace(/&lt;/g, "<").replace(/&gt;/g, ">").replace(/&amp;/g, "&").replace(/&quot;/g, '"').replace(/&apos;/g, "'");
}

//测试SQL是否正确
function testSql(sql, connId)
{
    if (!sql || $.trim(sql).length == 0)
    {
        alert("sql不能为空!");
        return;
    }
    if (!connId || $.trim(connId).length == 0)
    {
        alert("数据连接不能为空!");
        return;
    }
    $.ajax({
        url: "../Dbconnection/TestSql", data: {"sql": sql, "connid": connId}, type: "post", success: function (txt) {
            alert("1" == txt ? "SQL测试正确!" : txt);
        }
    })
}

var compieform_defaultvalues = new Array();//默认值JSON
var compieform_formats= new Array();//格式JSON
//编译表单
function compileForm(attrJson, eventsJSON, subtableJSON, html) {
    compieform_defaultvalues = new Array();
    compieform_formats = new Array();
    var $html = $('<div>' + html + '</div>');
    var $elements = $("[data-isflow='1']", $html);
    if (!$elements || $elements.size() == 0) {
        //return html;
    }
    
    //编译表单中控件
    for (var i = 0; i < $elements.size(); i++) {
        var $element = $elements.eq(i);
        var type = $element.attr("data-type");
        switch (type) {
            case "text":
                setTextHtml($element, eventsJSON);
                break;
            case "textarea":
                setTextareaHtml($element, eventsJSON);
                break;
            case "select":
            case "select2":
                setSelectHtml($element, eventsJSON, $html);
                break;
            case "radio":
                setRadioHtml($element, eventsJSON);
                break;
            case "checkbox":
                setCheckboxHtml($element, eventsJSON);
                break;
            case "hidden":
                setHiddenHtml($element, eventsJSON);
                break;
            case "button":
                setButtonHtml($element, eventsJSON);
                break;
            case "html":
                setHtmlHtml($element, eventsJSON);
                break;
            case "label":
                setLabelHtml($element, eventsJSON);
                break;
            case "datetime":
                setDatetimeHtml($element, eventsJSON);
                break;
            case "organize":
                setOrganizeHtml($element, eventsJSON);
                break;
            case "lrselect":
                setLRSelectHtml($element, eventsJSON);
                break;
            case "files":
                setFilesHtml($element, eventsJSON);
                break;
            case "subtable":
                setSubtableHtml($element, eventsJSON, subtableJSON, attrJson.dbConn);
                break;
            case "serialnumber":
                setSerialNumberHtml($element, eventsJSON);
                break;
            case "selectdiv":
                setSelectDiv($element, eventsJSON)
                break;
            case "signature":
                setSignature($element, eventsJSON)
                break;
            case "datatable":
                setDataTable($element, eventsJSON, attrJson.dbConn)
                break;
        }
    }

    var formHtml = '';
    formHtml += '@using RoadFlow.Utility;\r\n';
    formHtml += '@using RoadFlow.Business;\r\n';
    formHtml += '@using Microsoft.AspNetCore.Http;\r\n';
    formHtml += '@{\r\n';
    formHtml += '\tvar Request = (HttpRequest)ViewData["request"];\r\n';
    formHtml += '\tstring instanceId = Request.Querys("instanceid");\r\n';
    formHtml += '\tstring taskId = Request.Querys("taskid");\r\n';
    formHtml += '\tstring stepId = Request.Querys("stepid");\r\n';
    formHtml += '\tstring flowId = Request.Querys("flowid");\r\n';
    formHtml += '\tstring display = Request.Querys("display");\r\n';
    formHtml += '\tbool showArchive = "1".Equals(Request.Querys("showarchive"));\r\n';
    formHtml += '\tstring fieldStatusJSON = string.Empty;\r\n';
    formHtml += '\tstring formData = showArchive ? new RoadFlow.Business.FlowArchive().GetArchiveData(Request.Querys("archiveid").ToGuid()) : new RoadFlow.Business.Form().GetFormData("' + attrJson.dbConn + '", "' + attrJson.dbTable + '", "' + attrJson.dbTablePrimaryKey + '", instanceId, stepId, flowId, "[' + compieform_formats.join(',') + ']", out fieldStatusJSON);\r\n';
    formHtml += '}\r\n';
    formHtml += '<script type="text/javascript" src="~/RoadFlowResources/scripts/flowRun/form.js"></script>\r\n';
    formHtml += '<script type="text/javascript">\r\n';
    formHtml += '\tvar formDataJSON = @Html.Raw(formData.IsNullOrWhiteSpace() ? "[]" : formData);\r\n';
    formHtml += '\tvar fieldStatusJSON = @Html.Raw(fieldStatusJSON.IsNullOrWhiteSpace() ? "[]" : fieldStatusJSON);\r\n';
    formHtml += '\tvar defaultValuesJSON = @Html.Raw(new RoadFlow.Business.Form().GetDefaultValuesJSON("[' + compieform_defaultvalues.join(",") + ']", fieldStatusJSON));\r\n';
    formHtml += '\t$(window).load(function () {\r\n';
    formHtml += '\t\tformLoad.load(formDataJSON, fieldStatusJSON, "' + attrJson.dbTable + '", "@(display.IsNullOrWhiteSpace() ? ViewData["display"].ToString() : display)", defaultValuesJSON);\r\n';
    formHtml += '\t});\r\n';

    if ($.isArray(eventsJSON)) {
        for (i = 0; i < eventsJSON.length; i++) {
            if ($.trim(eventsJSON[i].event).length > 0 && $.trim(eventsJSON[i].scripts).length > 0 && $.trim(eventsJSON[i].functionName).length > 0) {
                formHtml += '\tfunction ' + eventsJSON[i].functionName + '(srcElement){\r\n';
                formHtml += '\t\t' + eventsJSON[i].scripts + '\r\n';
                formHtml += '\t}\r\n';
            }
        }
    }
    formHtml += '</script>\r\n';
    formHtml += '<link href="~/RoadFlowResources/scripts/flowRun/common.css" rel="stylesheet" />\r\n';
    formHtml += '<input type="hidden" name="form_dbconnid" id="form_dbconnid" value="' + attrJson.dbConn + '"/>\r\n';
    formHtml += '<input type="hidden" name="form_dbtable" id="form_dbtable" value="' + attrJson.dbTable + '"/>\r\n';
    formHtml += '<input type="hidden" name="form_dbtableprimarykey" id="form_dbtableprimarykey" value="' + attrJson.dbTablePrimaryKey + '"/>\r\n';
    formHtml += '<input type="hidden" name="form_dbtabletitle" id="form_dbtabletitle" value="' + attrJson.dbTableTitle + '"/>\r\n';
    formHtml += '<input type="hidden" name="form_instanceid" id="form_instanceid" value="@instanceId"/>\r\n';
    formHtml += '<textarea style="display:none;" name="form_fieldstatus" id="form_fieldstatus">@Html.Raw(fieldStatusJSON.IsNullOrWhiteSpace() ? "[]" : fieldStatusJSON)</textarea>\r\n';
    formHtml += '<textarea style="display:none;" name="form_dataformatjson" id="form_dataformatjson">[' + compieform_formats.join(',') +']</textarea>\r\n';
    //标题表达式
    if(attrJson.dbTableTitleExpression && $.trim(attrJson.dbTableTitleExpression).length>0) {
        formHtml += '<textarea style="display:none;" name="form_dbtabletitleexpression" id="form_dbtabletitleexpression">' + attrJson.dbTableTitleExpression +'</textarea>\r\n';
    }
    formHtml += unescapeHTML($html.html());

    return formHtml;
}
//编译文本框
function setTextHtml($element, eventsJSON) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var defaultValue = RoadUI.Core.unescape($element.attr("data-defaultvalue"));
    var align = $element.attr("data-align");
    var placeholdertext = $element.attr("data-placeholdertext");
    var maxlength = $element.attr("data-maxlength");
    var inputtype = $element.attr("data-inputtype");
    var valuetype = $element.attr("data-valuetype");
    if ("password" == inputtype) {
        if (document.all) {
            var passwordhtml = '<input data-isflow="1" data-type="text" data-valuetype="" id="' + id + '" name="' + id
                + '" style="' + ($element.attr("data-width") ? 'width: ' + $element.attr("data-width") + ';' : '')
                + '"' + ($element.attr("data-placeholdertext") ? ' placeholder="' + $element.attr("data-placeholdertext") + '"' : '')
                + ($element.attr("data-maxlength") ? ' maxlength="' + $element.attr("data-maxlength") + '"' : '')
                + ' type="password" value="" class="mytext">';
            $element.after(passwordhtml);
            $element.remove();
            return;
        }
        $element.attr("type", inputtype);
    }
    if (align && $.trim(align).length > 0) {
        $element.css("text-align", align);
    }
    if (placeholdertext && $.trim(placeholdertext).length > 0) {
        $element.attr("placeholder", placeholdertext);
    }
    if (maxlength && !isNaN(maxlength)) {
        $element.attr("maxlength", maxlength);
    }
    $element.attr("value","").attr("class","mytext");
    $element.css("color", "");
    if (valuetype) {
        $element.attr("validate", valuetype);
    }
    $element.removeAttr("data-inputtype")
        .removeAttr("data-valuetype")
        .removeAttr("data-placeholdertext")
        .removeAttr("data-defaultvalue")
        .removeAttr("data-bindfiled")
        .removeAttr("data-width")
        .removeAttr("data-format")
        .removeAttr("data-maxlength")
        .removeAttr("data-align");
    var id = $element.attr("id");
    $element.attr("name", id);
    if($.isArray(eventsJSON)){
        for(var i=0; i<eventsJSON.length; i++){
            if(id == eventsJSON[i].id && $.trim(eventsJSON[i].event).length > 0 && $.trim(eventsJSON[i].scripts).length > 0 && $.trim(eventsJSON[i].functionName).length > 0){
                $element.attr($.trim(eventsJSON[i].event), eventsJSON[i].functionName+"(this);")
            }
        }
    }
    if (defaultValue && $.trim(defaultValue).length > 0) {
        compieform_defaultvalues.push("{'id':'" + id + "','value':'" + defaultValue + "'}");
    }
}
//编译文本域
function setTextareaHtml($element, eventsJSON) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var defaultValue = RoadUI.Core.unescape($element.attr("data-defaultvalue"));
    var placeholdertext = $element.attr("data-placeholdertext");
    var maxlength = $element.attr("data-maxlength");
    var valuetype = $element.attr("data-valuetype");
    if (placeholdertext && $.trim(placeholdertext).length > 0) {
        $element.attr("placeholder", placeholdertext);
    }
    if (maxlength && !isNaN(maxlength)) {
        $element.attr("maxlength", maxlength);
    }
    $element.text("").attr("class","mytextarea");
    $element.css("color", "");
    if (valuetype) {
        $element.attr("validate", valuetype);
    }
    $element.removeAttr("data-valuetype")
        .removeAttr("data-placeholdertext")
        .removeAttr("data-defaultvalue")
        .removeAttr("data-bindfiled")
        .removeAttr("data-width")
        .removeAttr("data-maxlength")
        .removeAttr("data-height");
    var id = $element.attr("id");
    $element.attr("name", id);
    if($.isArray(eventsJSON)){
        for(var i=0; i<eventsJSON.length; i++){
            if(id == eventsJSON[i].id && $.trim(eventsJSON[i].event).length > 0 && $.trim(eventsJSON[i].scripts).length > 0 && $.trim(eventsJSON[i].functionName).length > 0){
                $element.attr($.trim(eventsJSON[i].event), eventsJSON[i].functionName+"(this);")
            }
        }
    }
    if (defaultValue && $.trim(defaultValue).length > 0) {
        compieform_defaultvalues.push("{'id':'" + id + "','value':'" + defaultValue + "'}");
    }
}
//编译下拉选择
function setSelectHtml($element, eventsJSON, $html) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var id = $element.attr("id");
    var defaultValue = RoadUI.Core.unescape($element.attr("data-defaultvalue"));
    if (defaultValue && $.trim(defaultValue).length > 0) {
        compieform_defaultvalues.push("{'id':'" + id + "','value':'" + defaultValue + "'}");
    }
    isSelect2 = "1" == $element.attr("data-hasselect2");
    width = $element.attr("data-width");
    if (isSelect2 && !width)
    {
        width = "120px";
    }
    $element.html("").attr("class", isSelect2 ? "myselect2" : "myselect");
    if (width)
    {
        $element.css("width", width);
    }
    $element.css("color","");
    $element.css("height", "");
    $element.attr("name", id);

    var datasource = $element.attr("data-datasource");
    var hasempty = "0";
    var emptytitle = $element.attr("data-emptytitle") || "";
    var hasmultiple = "1" == $element.attr("data-hasmultiple");//是否多选
    var ds_dict_valuefield = "";
    var ds_dict_value = "";
    if (emptytitle.length > 0) {
        hasempty = "1";
    }
    if (hasmultiple) {
        $element.attr("multiple", "multiple");
    }
    if ("0" == datasource) {//数据字典
        ds_dict_value = $element.attr("data-ds_dict_value") || "";
        ds_dict_valuefield = $element.attr("data-ds_dict_valuefield") || "id";
        var ds_dict_ischild = "1" == $element.attr("data-ds_dict_ischild");
        var valueField = "RoadFlow.Utility.ValueField.Id";
        switch (ds_dict_valuefield) {
            case "code":
                valueField = "RoadFlow.Utility.ValueField.Code";
                break;
            case "value":
                valueField = "RoadFlow.Utility.ValueField.Value";
                break;
            case "title":
                valueField = "RoadFlow.Utility.ValueField.Title";
                break;
            case "note":
                valueField = "RoadFlow.Utility.ValueField.Note";
                break;
            case "other":
                valueField = "RoadFlow.Utility.ValueField.Other";
                break;
        }
        $element.text(("1" == hasempty ? '<option value="">' + emptytitle + '</option>' : '')
            + '@Html.Raw(new RoadFlow.Business.Dictionary().GetOptionsByID' +
            '("' + ds_dict_value + '".ToGuid(), ' + valueField + ', "", ' + (ds_dict_ischild ? 'true' : 'false') + '))');

    } else if ("1" == datasource) {//字符串
        var ds_custom_string = (RoadUI.Core.unescape($element.attr("data-ds_custom_string") || "")).split(";");
        var options = "";
        if ("1" == hasempty) {
            options += '<option value="">' + emptytitle + '</option>';
        }
        for (var i = 0; i < ds_custom_string.length; i++) {
            var ds_custom_string1 = ds_custom_string[i].split(",");
            var title = ds_custom_string1.length > 0 ? ds_custom_string1[0] : "";
            var value = ds_custom_string1.length > 1 ? ds_custom_string1[1] : title;
            options += '<option value="' + value + '">' + title + '</option>';
        }
        $element.html(options);
    } else if ("2" == datasource) {//SQL
        var ds_sql_dbconn = $element.attr("data-ds_sql_dbconn");
        var ds_sql_value = RoadUI.Core.unescape($element.attr("data-ds_sql_value"));
        $element.text(("1" == hasempty ? '<option value="">' + emptytitle + '</option>' : '')
            + '@Html.Raw(new RoadFlow.Business.Form().GetOptionsBySQL("' + ds_sql_dbconn + '", "' + ds_sql_value + '", ""))');
    } else if ("3" == datasource) {//URL
        var ds_url_address = RoadUI.Core.unescape($element.attr("data-ds_url_address"));
        $element.text(("1" == hasempty ? '<option value="">' + emptytitle + '</option>' : '')
            + '@Html.Raw(new RoadFlow.Business.Form().GetOptionsByUrl("' + ds_url_address + '"))');
    }

    if ($.isArray(eventsJSON)) {
        for (var i = 0; i < eventsJSON.length; i++) {
            if (id == eventsJSON[i].id && $.trim(eventsJSON[i].event).length > 0 && $.trim(eventsJSON[i].scripts).length > 0 && $.trim(eventsJSON[i].functionName).length > 0) {
                $element.attr($.trim(eventsJSON[i].event), eventsJSON[i].functionName + "(this);")
            }
        }
    }
    //联动
    var linkage_field= $element.attr("data-linkage_field");
    if(linkage_field && $.trim(linkage_field).length>0) {
        var linkage_source = $element.attr("data-linkage_source");
        var linkage_source_sql_conn = $element.attr("data-linkage_source_sql_conn");
        var linkage_source_text = $element.attr("data-linkage_source_text");
        var childHasEmmpty = $("#" + linkage_field, $html).attr("data-hasempty");
        var change = $element.attr("onchange");
        $element.attr("onchange", (change && $.trim(change).length > 0 ? change + ";" : "")
            + "formLoad.loadChildOptions(this, '" + linkage_field + "', '"
            + linkage_source + "', '" + linkage_source_sql_conn + "', '" + linkage_source_text + "', '"
            + ds_dict_valuefield + "', '" + ds_dict_value + "', '" + childHasEmmpty + "', false, " + ds_dict_ischild + ");");
    }

    $element.removeAttr("data-width")
        .removeAttr("data-bindfiled")
        .removeAttr("data-emptytitle")
        .removeAttr("data-hasmultiple")
        .removeAttr("data-datasource")
        .removeAttr("data-ds_dict_value")
        .removeAttr("data-ds_dict_ischild")
        .removeAttr("data-ds_dict_valuefield")
        .removeAttr("data-ds_custom_string")
        .removeAttr("data-ds_sql_dbconn")
        .removeAttr("data-ds_sql_value")
        .removeAttr("data-ds_url_address")
        .removeAttr("data-linkage_source")
        .removeAttr("data-linkage_source_sql_conn")
        .removeAttr("data-linkage_source_text")
        .removeAttr("data-hasselect2")
        .removeAttr("data-defaultvalue");
}
//编译单选框
function setRadioHtml($element, eventsJSON) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var id = $element.attr("id");
    var defaultValue = RoadUI.Core.unescape($element.attr("data-defaultvalue"));
    if (defaultValue && $.trim(defaultValue).length > 0) {
        compieform_defaultvalues.push("{'id':'" + id + "','value':'" + defaultValue + "'}");
    }
    var datasource = $element.attr("data-datasource") || "-1";
    var ds_dict_value = $element.attr("data-ds_dict_value") || "";
    var ds_dict_valuefield = $element.attr("data-ds_dict_valuefield") || "id";
    var ds_sql_dbconn = $element.attr("data-ds_sql_dbconn") || "";
    var ds_sql_value = RoadUI.Core.unescape($element.attr("data-ds_sql_value") || "");
    var ds_url_address = RoadUI.Core.unescape($element.attr("data-ds_url_address") || "");
    var ds_custom_string = RoadUI.Core.unescape($element.attr("data-ds_custom_string") || "");
    var text = "";
    switch (datasource) {
        case "1":
            text = ds_custom_string;
            break;
        case "2":
            text = ds_sql_value;
            break;
        case "3":
            text = ds_url_address;
            break;
    }
    $element.after('@Html.Raw(new RoadFlow.Business.Form().GetRadioOrCheckboxHtml(' + datasource + ', "' + ds_sql_dbconn + '", "' + ds_dict_value + '", "'
        + ds_dict_valuefield.toLowerCase() + '", "' + text + '", "", "radio", "' + id + '", "data-isflow=\\"1\\" data-type=\\"radio\\""))');
    $element.remove();
}
//编译复选框
function setCheckboxHtml($element, eventsJSON) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var id = $element.attr("id");
    var defaultValue = RoadUI.Core.unescape($element.attr("data-defaultvalue"));
    if (defaultValue && $.trim(defaultValue).length > 0) {
        compieform_defaultvalues.push("{'id':'" + id + "','value':'" + defaultValue + "'}");
    }
    var datasource = $element.attr("data-datasource") || "-1";
    var ds_dict_value = $element.attr("data-ds_dict_value") || "";
    var ds_dict_valuefield = $element.attr("data-ds_dict_valuefield") || "id";
    var ds_sql_dbconn = $element.attr("data-ds_sql_dbconn") || "";
    var ds_sql_value = RoadUI.Core.unescape($element.attr("data-ds_sql_value") || "");
    var ds_url_address = RoadUI.Core.unescape($element.attr("data-ds_url_address") || "");
    var ds_custom_string = RoadUI.Core.unescape($element.attr("data-ds_custom_string") || "");
    var text = "";
    switch (datasource) {
        case "1":
            text = ds_custom_string;
            break;
        case "2":
            text = ds_sql_value;
            break;
        case "3":
            text = ds_url_address;
            break;
    }
    $element.after('@Html.Raw(new RoadFlow.Business.Form().GetRadioOrCheckboxHtml(' + datasource + ', "' + ds_sql_dbconn + '", "' + ds_dict_value + '", "' + ds_dict_valuefield.toLowerCase()
        + '", "' + text + '", "", "checkbox", "' + id + '", "data-isflow=\\"1\\" data-type=\\"checkbox\\""))');
    $element.remove();
}
//编译隐藏域
function setHiddenHtml($element, eventsJSON) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var id = $element.attr("id");
    var defaultValue = RoadUI.Core.unescape($element.attr("data-defaultvalue"));
    if (defaultValue && $.trim(defaultValue).length > 0) {
        compieform_defaultvalues.push("{'id':'" + id + "','value':'" + defaultValue + "'}");
    }
    if ($.isArray(eventsJSON)) {
        for (var i = 0; i < eventsJSON.length; i++) {
            if (id == eventsJSON[i].id && $.trim(eventsJSON[i].event).length > 0 && $.trim(eventsJSON[i].scripts).length > 0 && $.trim(eventsJSON[i].functionName).length > 0) {
                $element.attr($.trim(eventsJSON[i].event), eventsJSON[i].functionName + "(this);")
            }
        }
    }
    var hidden = '<input data-isflow="1" data-type="hidden" id="' + id + '" name="' + id + '" type="hidden" value="">';
    $element.after(hidden);
    $element.remove();
}
//编译按钮
function setButtonHtml($element, eventsJSON) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var id = $element.attr("id");
    var value = $element.attr("data-value");
    if ($.isArray(eventsJSON)) {
        for (var i = 0; i < eventsJSON.length; i++) {
            if (id == eventsJSON[i].id && $.trim(eventsJSON[i].event).length > 0 && $.trim(eventsJSON[i].scripts).length > 0 && $.trim(eventsJSON[i].functionName).length > 0) {
                $element.attr($.trim(eventsJSON[i].event), eventsJSON[i].functionName + "(this);")
            }
        }
    }
    $element.val(value || "按钮");
    $element.attr("name", id);
    $element.addClass("mybutton").removeAttr("data-style").removeAttr("data-bindfiled").removeAttr("data-value");
}
//编译HTML
function setHtmlHtml($element, eventsJSON) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var id = $element.attr("id");
    $element.text("");
    $element.attr("rows", "1");
    $element.attr("cols", "1");
    $element.attr("name", id);
    $element.after('<script src="~/RoadFlowResources/scripts/ckeditor/ckeditor.js"></script>'
        + '<script>CKEDITOR.replace("' + id + '",{'
        + 'height:' + $element.height()
        + ',toolbarGroups:' + ($element.attr("data-toolbar") || "formLoad.ckeditor_toolbarFullGroups")
        + ',filebrowserImageUploadUrl:"/RoadFlowCore/Controls/SaveCKEditorFiles"'
        + '});</script>');
    $element.removeAttr("data-width")
        .removeAttr("data-bindfiled")
        .removeAttr("data-toolbar")
        .removeAttr("data-height");
}
//编译LABEL
function setLabelHtml($element, eventsJSON) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var id = $element.attr("id");
    var style = $element.attr("data-style");
    var bindfiled = $element.attr("data-bindfiled");
    var defaultValue = RoadUI.Core.unescape($element.attr("data-defaultvalue"));
    var isBindFiled = bindfiled && $.trim(bindfiled).length > 0;
    if (!isBindFiled) {
        id = ("label_" + RoadUI.Core.newid(false)).toUpperCase();
    }
    if (defaultValue && $.trim(defaultValue).length > 0) {
        compieform_defaultvalues.push("{'id':'" + id + "','value':'" + defaultValue + "'}");
    }
    var label = '';
    if (isBindFiled) {//如果绑定了字段，要加HIDDEN，将值保存到对应字段
        label += '<input type="hidden" id="' + id + '_hidden" name="' + id + '" value=""/>';
    }
    label += '<label data-isflow="1" data-type="label"' + (isBindFiled ? ' data-bindfiled="' + id + '"' : '') + ' id="' + id + '" style="' + (style || '') + '"></label>';
    $element.after(escapeHTML(label));
    $element.remove();
}
//编译时期时间
function setDatetimeHtml($element, eventsJSON) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var id = $element.attr("id");
    var defaultValue = RoadUI.Core.unescape($element.attr("data-defaultvalue"));
    var width = $element.attr("data-width");
    var min = $element.attr("data-min");
    var max = $element.attr("data-max");
    var daybefor = $element.attr("data-daybefor");
    var dayafter = $element.attr("data-dayafter");
    var currentmonth = $element.attr("data-currentmonth");
    var istime = $element.attr("data-istime");
    var format = $element.attr("data-format");

    $element.attr("value", "").attr("style", "").attr("class", "mycalendar").attr("name", id);
    if (width) {
        $element.css('width', width);
    }
    if (min) {
        $element.attr('mindate', min);
    }
    if (max) {
        $element.attr('maxdate', max);
    }
    if ('1' == daybefor) {
        $element.attr('daybefor', "1");
    }
    if ('1' == dayafter) {
        $element.attr('dayafter', "1");
    }
    if ('1' == currentmonth) {
        $element.attr('currentmonth', "1");
    }
    if ('1' == istime) {
        $element.attr('istime', "1");
    }
    if (format) {
        $element.attr('format', format);
    }
    var format1 = format;
    if (!format1 || $.trim(format1).length == 0) {
        format1 = '1' == istime ? "yyyy-MM-dd HH:mm" : "yyyy-MM-dd";
    }
    compieform_formats.push("{'id':'" + id + "','type':'datetime','format':'" + format1 + "'}");

    if (defaultValue && $.trim(defaultValue).length > 0) {
        compieform_defaultvalues.push("{'id':'" + id + "','value':'" + defaultValue + "'}");
    }

    if ($.isArray(eventsJSON)) {
        for (var i = 0; i < eventsJSON.length; i++) {
            if (id == eventsJSON[i].id && $.trim(eventsJSON[i].event).length > 0 && $.trim(eventsJSON[i].scripts).length > 0 && $.trim(eventsJSON[i].functionName).length > 0) {
                $element.attr($.trim(eventsJSON[i].event), eventsJSON[i].functionName + "(this);")
            }
        }
    }

    $element.removeAttr("data-width")
        .removeAttr("data-bindfiled")
        .removeAttr("data-min")
        .removeAttr("data-max")
        .removeAttr("data-daybefor")
        .removeAttr("data-dayafter")
        .removeAttr("data-currentmonth")
        .removeAttr("data-istime")
        .removeAttr("data-defaultvalue")
        .removeAttr("data-format");
}
//编译组织架构选择
function setOrganizeHtml($element, eventsJSON) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var id = $element.attr("id");
    var defaultValue = RoadUI.Core.unescape($element.attr("data-defaultvalue"));
    var width = $element.attr("data-width");
    var opener = $element.attr("data-opener");
    $element.attr("value", "").removeAttr("style", "").attr("class", "mymember").attr("name", id);
    if(width){
        $element.css("width", width);
    }

    var rootId = "";
    var org_rang = $element.attr("data-org_rang");
    if ("0" == org_rang) {
        rootId = "@(RoadFlow.Business.Wildcard.Filter(\"{<initiatordeptid>}\"))";
    } else if ("1" == org_rang) {
        rootId = "@(RoadFlow.Business.Wildcard.Filter(\"{<userdeptid>}\"))";
    } else if ("2" == org_rang) {
        rootId = $element.attr("data-org_rang_2_value");
    }
    if (rootId && $.trim(rootId).length > 0) {
        $element.attr("rootid", rootId);
        $element.attr("isChangeType", "0");
    }

    $element.attr("unit", $element.attr("data-org_type_unit"));
    $element.attr("dept", $element.attr("data-org_type_dept"));
    $element.attr("station", $element.attr("data-org_type_station"));
    $element.attr("group", $element.attr("data-org_type_wrokgroup"));
    $element.attr("user", $element.attr("data-org_type_user"));
    $element.attr("more", $element.attr("data-more"));

    if (opener && $.trim(opener).length > 0) {
        $element.attr("opener", $.trim(opener));
    }

    if (defaultValue && $.trim(defaultValue).length > 0) {
        compieform_defaultvalues.push("{'id':'" + id + "','value':'" + defaultValue + "'}");
    }

    if ($.isArray(eventsJSON)) {
        for (var i = 0; i < eventsJSON.length; i++) {
            if (id == eventsJSON[i].id && $.trim(eventsJSON[i].event).length > 0 && $.trim(eventsJSON[i].scripts).length > 0 && $.trim(eventsJSON[i].functionName).length > 0) {
                $element.attr($.trim(eventsJSON[i].event), eventsJSON[i].functionName + "(this);")
            }
        }
    }

    $element.removeAttr("data-width")
        .removeAttr("data-bindfiled")
        .removeAttr("data-defaultvalue")
        .removeAttr("data-org_rang")
        .removeAttr("data-org_rang_2_value")
        .removeAttr("data-org_type_unit")
        .removeAttr("data-org_type_dept")
        .removeAttr("data-org_type_station")
        .removeAttr("data-org_type_wrokgroup")
        .removeAttr("data-org_type_user")
        .removeAttr("data-opener")
        .removeAttr("data-more");
}
//编译数据字典选择
function setLRSelectHtml($element, eventsJSON) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var id = $element.attr("id");
    var defaultValue = RoadUI.Core.unescape($element.attr("data-defaultvalue"));
    var width = $element.attr("data-width");
    var dialogtitle = $element.attr("data-dialogtitle");
    var ismore = $element.attr("data-ismore");
    var isroot = $element.attr("data-isroot");
    var isparent = $element.attr("data-isparent");
    var ds_dict_value = $element.attr("data-ds_dict_value");
    var ds_dict_allchild = $element.attr("data-ds_dict_allchild");
    var opener = $element.attr("data-opener");
    $element.attr("value", "").removeAttr("style", "").attr("class", "mydict").attr("name", id);
    if(width){
        $element.css("width", width);
    }
    if(dialogtitle && $.trim(dialogtitle).length>0){
        $element.attr("title", dialogtitle);
    }
    if(ismore && $.trim(ismore).length>0){
        $element.attr("ismore", ismore);
    }
    if(isroot && $.trim(isroot).length>0){
        $element.attr("isroot", isroot);
    }
    if(isparent && $.trim(isparent).length>0){
        $element.attr("isparent", isparent);
    }
    if(ds_dict_value && $.trim(ds_dict_value).length>0){
        $element.attr("rootid", ds_dict_value);
    }
    if(ds_dict_allchild && $.trim(ds_dict_allchild).length>0){
        $element.attr("ischild", ds_dict_allchild);
    }
    if (defaultValue && $.trim(defaultValue).length > 0) {
        compieform_defaultvalues.push("{'id':'" + id + "','value':'" + defaultValue + "'}");
    }
    if (opener && $.trim(opener).length > 0) {
        $element.attr("opener", $.trim(opener));
    }
    if ($.isArray(eventsJSON)) {
        for (var i = 0; i < eventsJSON.length; i++) {
            if (id == eventsJSON[i].id && $.trim(eventsJSON[i].event).length > 0 && $.trim(eventsJSON[i].scripts).length > 0 && $.trim(eventsJSON[i].functionName).length > 0) {
                $element.attr($.trim(eventsJSON[i].event), eventsJSON[i].functionName + "(this);")
            }
        }
    }

    $element.removeAttr("data-width")
        .removeAttr("data-bindfiled")
        .removeAttr("data-defaultvalue")
        .removeAttr("data-dialogtitle")
        .removeAttr("data-ismore")
        .removeAttr("data-isroot")
        .removeAttr("data-isparent")
        .removeAttr("data-ds_dict_value")
        .removeAttr("data-opener")
        .removeAttr("data-ds_dict_allchild");
}
//编译附件
function setFilesHtml($element, eventsJSON) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var id = $element.attr("id");
    var width = $element.attr("data-width");
    var fileType = $element.attr("data-filetype");
    var opener = $element.attr("data-opener");
    $element.attr("value", "").removeAttr("style", "").attr("class", "myfile").attr("name", id);
    if (width && $.trim(width).length > 0) {
        $element.css("width", width);
    }
    if (fileType && $.trim(fileType).length > 0) {
        $element.attr("filetype", fileType);
    }
    if (opener && $.trim(opener).length > 0) {
        $element.attr("opener", $.trim(opener));
    }
    $element.removeAttr("data-width")
        .removeAttr("data-bindfiled")
        .removeAttr("data-opener")
        .removeAttr("data-filetype");
}
//编译流水号
function setSerialNumberHtml($element, eventsJSON) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var id = $element.attr("id");
    var width = $element.attr("data-width");
    var maxfiled = $element.attr("data-maxfiled");
    var placeholdertext = $element.attr("data-placeholdertext") || "自动生成流水号";
    var length = $element.attr("data-length");
    var formatstring = $element.attr("data-formatstring");
    var sqlwhere = $element.attr("data-sqlwhere");
    var config = {};
    config.maxfiled = maxfiled;
    config.length = length;
    config.formatstring = formatstring;
    config.sqlwhere = sqlwhere;
    $element.attr("placeholder", placeholdertext).attr("style", "").attr("value", "").attr("class", "mytext").prop("readonly", true);
    if (width) {
        $element.css('width', width);
    }
    $element.attr("name", id);
    $element.after('<input type="hidden" value="' + id + '" name="rf_serialnumber"/>');
    $element.after('<textarea style="width:0;height:0;display:none;" name="rf_serialnumber_config_' + id + '">' + JSON.stringify(config) + '</textarea>');
    $element.removeAttr("data-width")
        .removeAttr("data-placeholdertext")
        .removeAttr("data-sqlwhere")
        .removeAttr("data-maxfiled")
        .removeAttr("data-length")
        .removeAttr("data-formatstring")
        .removeAttr("data-length")
        .removeAttr("data-bindfiled");
}
//编译弹出选择
function setSelectDiv($element, eventsJSON) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var id = $element.attr("id");
    var defaultValue = RoadUI.Core.unescape($element.attr("data-defaultvalue"));
    var width = $element.attr("data-width");
    var forms = $element.attr("data-form_forms");
    var windowwidth = $element.attr("data-windowwidth");
    var windowheight = $element.attr("data-windowheight");
    var pkfield = $element.attr("data-pkfield");
    var titlefield = $element.attr("data-titlefield");
    var paramsname = $element.attr("data-paramsname"); 
    var paramsvalue = $element.attr("data-paramsvalue");
    var windowtitle = $element.attr("data-windowtitle");
    var opener = $element.attr("data-opener");
    $element.attr("value", "").removeAttr("style", "").attr("class", "myselectdiv").attr("name", id);
    if (width) {
        $element.css("width", width);
    }
    $element.attr("showtitle", windowtitle).attr("appid", forms).attr("titlefield", titlefield).attr("pkfield", pkfield).attr("width", windowwidth).attr("height", windowheight).attr("paramsname", $.trim(paramsname)).attr("paramsvalue", encodeURI($.trim(paramsvalue))).attr("opener", opener);

    if (defaultValue && $.trim(defaultValue).length > 0) {
        compieform_defaultvalues.push("{'id':'" + id + "','value':'" + defaultValue + "'}");
    }

    if ($.isArray(eventsJSON)) {
        for (var i = 0; i < eventsJSON.length; i++) {
            if (id == eventsJSON[i].id && $.trim(eventsJSON[i].event).length > 0 && $.trim(eventsJSON[i].scripts).length > 0 && $.trim(eventsJSON[i].functionName).length > 0) {
                $element.attr($.trim(eventsJSON[i].event), eventsJSON[i].functionName + "(this);")
            }
        }
    }

    $element.removeAttr("data-width")
        .removeAttr("data-form_types")
        .removeAttr("data-defaultvalue")
        .removeAttr("data-form_forms")
        .removeAttr("data-windowwidth")
        .removeAttr("data-windowheight")
        .removeAttr("data-pkfield")
        .removeAttr("data-titlefield")
        .removeAttr("data-paramsname")
        .removeAttr("data-windowtitle")
        .removeAttr("data-paramsvalue")
        .removeAttr("data-bindfiled");
}
//编译签章
function setSignature($element, eventsJSON) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var id = $element.attr("id");
    var ispassword = $element.attr("data-ispassword");
    $element.attr("value", "签章").removeAttr("style", "").attr("class", "mybutton").attr("data-id", id)
        .attr("id", id + "_text").attr("name", id + "_text").attr("data-src", "@Url.Content(\"~\" + new RoadFlow.Business.User().GetSignSrc())");
    $element.attr("onclick", "signature('" + id + "_text', false);");
    $element.after('<input type="hidden" value="" id="' + id + '" name="' + id + '"/>');
    $element.removeAttr("data-bindfiled");
}
//编译数据表格
function setDataTable($element, eventsJSON, connId) {
    if (!$element || $element.size() == 0) {
        return;
    }
    var width = $element.attr("data-width");
    var height = $element.attr("data-height");
    var datasource = $element.attr("data-datasource");
    var datasourcetext = escapeHTML($element.attr("data-datasourcetext"));
    //var params = $element.attr("data-params");
    $element.after('@Html.Raw(new RoadFlow.Business.Form().GetDataTableHtml("' + width + '", "' + height + '", "' + datasource + '", "' + datasourcetext + '", "' + connId + '", formData))');
    $element.remove();
}

//编译子表
function setSubtableHtml($element, eventsJSON, subtableJSON, connId) {
    if (!$element || $element.size() == 0) {
        return;
    }
    if (!subtableJSON || !$.isArray(subtableJSON)) {
        return;
    }
    var id = $element.attr("id");
    var width = $element.attr("data-width");
    var subJSON = null;
    for (i = 0; i < subtableJSON.length; i++) {
        if (subtableJSON[i].id == id) {
            subJSON = subtableJSON[i];
            break;
        }
    }
    var subHTML = '';
    if (subJSON) {
        var secondtable = subJSON.secondtable;
        var primarytablefiled = subJSON.primarytablefiled;
        var secondtableprimarykey = subJSON.secondtableprimarykey;
        var secondtablerelationfield = subJSON.secondtablerelationfield;
        var editmodel = subJSON.editmodel || "0";
        var editformtype = subJSON.editformtype;
        var editform = subJSON.editform;
        var displaymodewidth = subJSON.displaymodewidth;
        var displaymodeheight = subJSON.displaymodeheight;
        var showindex = subJSON.showindex;
        var sortstring = subJSON.sortstring;
        var width = subJSON.width;
        var eventScripts = new Array();//控件事件
        subJSON.colnums.sort(arrayCompare("index"));//按列序号排序
        //常规编辑方式
        if ("0" == editmodel) {
            subHTML = '<table data-isflowsubtable="1"' + (width && $.trim(width).length > 0 ? ' width="' + width + '"' : '') + ' id="SUBTABLE_' + id + '">\r\n';
            subHTML += '\t<thead>\r\n';
            subHTML += '\t\t<tr>\r\n';
            if ("1" == showindex) {//显示序号列
                subHTML += '\t\t\t<th>序号</th>\r\n';
            }
            isSum = false;//是否有合计列
            for (var i = 0; i < subJSON.colnums.length; i++) {
                var colJSON = subJSON.colnums[i];
                if ("1" != colJSON.isshow) {
                    continue;
                }
                if ("1" == colJSON.issum && !isSum) {
                    isSum = true;
                }
                var field = colJSON.fieldname
                var colTitle = colJSON.showname || field;
                var colWidth = colJSON.width || "";
                subHTML += '\t\t\t<th' + (colWidth ? ' width="' + colWidth + '"' : '') + '>';
                subHTML += colTitle;
                subHTML += '</th>\r\n';
            }
            subHTML += '\t\t\t<th width="120px" style="text-align:left;">\r\n';
            subHTML += '\t\t\t\t<input type="hidden" name="SUBTABLE_id" value="' + id + '"/>\r\n';
            subHTML += '\t\t\t\t<input type="hidden" name="SUBTABLE_' + id + '_secondtable" value="' + secondtable + '"/>\r\n';
            subHTML += '\t\t\t\t<input type="hidden" name="SUBTABLE_' + id + '_primarytablefiled" value="' + primarytablefiled + '"/>\r\n';
            subHTML += '\t\t\t\t<input type="hidden" name="SUBTABLE_' + id + '_secondtableprimarykey" value="' + secondtableprimarykey + '"/>\r\n';
            subHTML += '\t\t\t\t<input type="hidden" name="SUBTABLE_' + id + '_secondtablerelationfield" value="' + secondtablerelationfield + '"/>\r\n';
            subHTML += '\t\t\t\t<input type="button" value="添加" class="mybutton" onclick="formLoad.subtableAddRow(\'SUBTABLE_' + id + '\', $(\'#SUBTABLE_' + id + ' tbody tr:first\'), true);"/>\r\n\t\t\t</th>\r\n';
            subHTML += '\t\t</tr>\r\n';
            subHTML += '\t</thead>\r\n';
            subHTML += '\t<tbody>\r\n';
            subHTML += '\t\t@{\r\n';
            if ("1" == showindex) {//定义序号变量
                subHTML += '\t\tint subtableIndex_' + secondtable + "_" + primarytablefiled + "_" + secondtableprimarykey + "_" + secondtablerelationfield + ' = 1;\r\n';
            }
            subtableMapListName = 'subtableMapList_' + secondtable + "_" + primarytablefiled + "_" + secondtableprimarykey + "_" + secondtablerelationfield;
            subHTML += '\t\tSystem.Data.DataTable ' + subtableMapListName + ' = new RoadFlow.Business.DbConnection().GetDataTable(new RoadFlow.Business.DbConnection().Get("' + connId + '".ToGuid()), "' + secondtable + '", "' + secondtablerelationfield + '", instanceId, "' + sortstring + '");\r\n';
            subHTML += '\t\tif(null == ' + subtableMapListName + ' || ' + subtableMapListName + '.Rows.Count == 0)\r\n';
            subHTML += '\t\t{\r\n';
            subHTML += '\t\t\t' + subtableMapListName + ' = new System.Data.DataTable();\r\n';
            subHTML += '\t\t\t' + subtableMapListName + '.Rows.Add(' + subtableMapListName + '.NewRow());\r\n';
            subHTML += '\t\t}\r\n';
            subHTML += '\t\tforeach(System.Data.DataRow dr in ' + subtableMapListName + '.Rows)\r\n';
            subHTML += '\t\t{\r\n';
            subHTML += '\t\t\tObject rowIndexObj = ' + subtableMapListName + '.Columns.Contains("' + secondtableprimarykey + '") ? dr["' + secondtableprimarykey + '"] : null;\r\n';
            subHTML += '\t\t\tstring rowIndexStr = null == rowIndexObj ? Guid.NewGuid().ToString() : rowIndexObj.ToString();\r\n';
            for (i = 0; i < subJSON.colnums.length; i++) {
                var colJSON = subJSON.colnums[i];
                if ("1" != colJSON.isshow) {
                    continue;
                }
                field = colJSON.fieldname;
                subHTML += '\t\t\tstring value_' + field + ' = ' + subtableMapListName + '.Columns.Contains("' + field + '") ? dr[\"' + field + '\"].ToString() : string.Empty;\r\n';
            }

            subHTML += '\t\t<tr>\r\n';
            if ("1" == showindex) {//显示序号
                subHTML += '\t\t\t<td data-tdindex="1" style="text-align:center;">@(' + 'subtableIndex_' + secondtable + "_" + primarytablefiled + "_" + secondtableprimarykey + "_" + secondtablerelationfield + '++)</td>\r\n';
            }
            for (i = 0; i < subJSON.colnums.length; i++) {
                var colJSON = subJSON.colnums[i];
                if ("1" != colJSON.isshow) {
                    continue;
                }
                var coleditmode = colJSON.editmode;
                if (!coleditmode) {
                    continue;
                }
                var editmode = coleditmode.editmode || "text";
                var align = colJSON.align || "0";
                align = "0" == align ? "left" : "1" == align ? "center" : "right";
                var field = colJSON.fieldname;
                var scripts = new Array();
                if (coleditmode.scripts && $.isArray(coleditmode.scripts)) {
                    for (j = 0; j < coleditmode.scripts.length; j++) {
                        scripts.push(coleditmode.scripts[j]);
                    }
                }
                var events = '';
                //如果列要计算合计，则在事件中添加计算合计方法formLoad.subTableSum
                if ("1" == colJSON.issum) {
                    if (!scripts || !$.isArray(scripts) || scripts.length == 0) {
                        scripts = [];
                    }
                    var blurIsIn = false;
                    for (var j = 0; j < scripts.length; j++) {
                        if (scripts[j].name.toUpperCase() == "ONCHANGE") {
                            scripts[j].script = RoadUI.Core.escape(RoadUI.Core.unescape(scripts[j].script) + ';' + 'formLoad.subTableSum("SUBTABLE_' + id + '","' + field + '");');
                            blurIsIn = true;
                        }
                    }
                    if (!blurIsIn) {
                        scripts.push({
                            "id": RoadUI.Core.newid(false).toUpperCase(),
                            "name": "onchange",
                            "elementName": "Set_" + field,
                            "script": RoadUI.Core.escape('formLoad.subTableSum("SUBTABLE_' + id + '", "' + field + '");')
                        });
                    }
                }
                if (scripts && $.isArray(scripts) && scripts.length > 0) {
                    for (j = 0; j < scripts.length; j++) {
                        events += ' ' + scripts[j].name + '="fun_' + scripts[j].id + '(this);"';
                        eventScripts.push({ "funname": "fun_" + scripts[j].id, "script": scripts[j].script });
                    }
                }
                tdHTML = '\t\t\t<td style="text-align:' + align + ';">';
                switch (editmode) {
                    case "text":
                        var text_defaultvalue = RoadUI.Core.unescape(coleditmode.text_defaultvalue);
                        var text_valuetype = coleditmode.text_valuetype;
                        var text_width = coleditmode.text_width;
                        var text_maxlength = coleditmode.text_maxlength;
                        var text_readonly = coleditmode.text_readonly;
                        var text_id = ('SUBTABLE_' + id + '_' + field).toUpperCase();
                        if (text_defaultvalue && $.trim(text_defaultvalue).length > 0) {
                            for (j = 0; j < compieform_defaultvalues.length; j++) {
                                if (compieform_defaultvalues[j].id == text_id) {
                                    compieform_defaultvalues.remove(j);
                                }
                            }
                            compieform_defaultvalues.push("{'id':'" + text_id + "','value':'" + text_defaultvalue + "'}");
                        }
                        tdHTML += '<input type="text" value="@value_' + field + '"' + events
                            + (text_defaultvalue ? ' data-defaultvalueid="' + text_id + '"' : '')
                            + (text_valuetype ? ' validate="' + text_valuetype + '"' : '')
                            + ' data-isflow="1" data-issubtable="1" data-type="text" data-table="' + secondtable + '" data-field="' + field + '" name="SUBTABLE_' + id + '_' + field + '_@(rowIndexStr)" id="SUBTABLE_' + id + '_' + field + '_@(rowIndexStr)" class="mytext"'
                            + (text_maxlength ? ' maxlength="' + text_maxlength + '"' : '')
                            + (text_width ? ' style="width:' + text_width + ';"' : '')
                            + ("1" == text_readonly ? ' readonly="readonly"' : '') + '/>';
                        break;
                    case "textarea":
                        var textarea_defaultvalue = RoadUI.Core.unescape(coleditmode.textarea_defaultvalue);
                        var textarea_valuetype = coleditmode.textarea_valuetype;
                        var textarea_maxlength = coleditmode.textarea_maxlength;
                        var textarea_width = coleditmode.textarea_width;
                        var textarea_height = coleditmode.textarea_height;
                        var textarea_id = ('SUBTABLE_' + id + '_' + field).toUpperCase();
                        if (textarea_defaultvalue && $.trim(textarea_defaultvalue).length > 0) {
                            for (j = 0; j < compieform_defaultvalues.length; j++) {
                                if (compieform_defaultvalues[j].id == textarea_id) {
                                    compieform_defaultvalues.remove(j);
                                }
                            }
                            compieform_defaultvalues.push("{'id':'" + textarea_id + "','value':'" + textarea_defaultvalue + "'}");
                        }
                        var textarea_style = '';
                        if (textarea_width) {
                            textarea_style += 'width:' + textarea_width + ';';
                        }
                        if (textarea_height) {
                            textarea_style += 'height:' + textarea_height + ';';
                        }
                        tdHTML += '<textarea' + events
                            + (textarea_defaultvalue ? ' data-defaultvalueid="' + textarea_id + '"' : '')
                            + (textarea_valuetype ? ' validate="' + textarea_valuetype + '"' : '')
                            + ' data-isflow="1" data-issubtable="1" data-type="textarea" data-table="' + secondtable + '" data-field="' + field + '" name="SUBTABLE_' + id + '_' + field + '_@(rowIndexStr)" id="SUBTABLE_' + id + '_' + field + '_@(rowIndexStr)" class="mytextarea"'
                            + (textarea_maxlength ? ' maxlength="' + textarea_maxlength + '"' : '')
                            + (textarea_style ? ' style="' + textarea_style + '"' : '')
                            + '>@value_' + field + ''
                            + '</textarea>';
                        break;
                    case "select":
                        var select_defaultvalue = RoadUI.Core.unescape(coleditmode.select_defaultvalue);
                        var select_width = coleditmode.select_width;
                        var select_ds = coleditmode.select_ds;
                        var select_ds_dict = coleditmode.select_ds_dict;
                        var select_ds_sql = RoadUI.Core.unescape(coleditmode.select_ds_sql);
                        var select_ds_string = RoadUI.Core.unescape(coleditmode.select_ds_string);
                        var select_hasempty = coleditmode.select_hasempty;
                        var select_ds_dict_ischild = coleditmode.select_ds_dict_ischild;
                        var select_ds_dict_valuefield = coleditmode.select_ds_dict_valuefield;
                        var select_Linkage_Field = coleditmode.select_Linkage_Field;
                        var select_Linkage_Source = coleditmode.select_Linkage_Source;
                        var select_Linkage_Source_sql_conn = coleditmode.select_Linkage_Source_sql_conn;
                        var select_Linkage_Source_text = coleditmode.select_Linkage_Source_text;
                        var select_id = ('SUBTABLE_' + id + '_' + field).toUpperCase();
                        if (select_defaultvalue && $.trim(select_defaultvalue).length > 0) {
                            for (j = 0; j < compieform_defaultvalues.length; j++) {
                                if (compieform_defaultvalues[j].id == select_id) {
                                    compieform_defaultvalues.remove(j);
                                }
                            }
                            compieform_defaultvalues.push("{'id':'" + select_id + "','value':'" + select_defaultvalue + "'}");
                        }
                        var select_options = '';
                        switch (select_ds) {
                            case "select_dsdict":
                                select_ds_ValueField = 'RoadFlow.Utility.ValueField.Id';
                                switch (select_ds_dict_valuefield) {
                                    case "id":
                                        select_ds_ValueField = 'RoadFlow.Utility.ValueField.Id';
                                        break;
                                    case "code":
                                        select_ds_ValueField = 'RoadFlow.Utility.ValueField.Code';
                                        break;
                                    case "value":
                                        select_ds_ValueField = 'RoadFlow.Utility.ValueField.Value';
                                        break;
                                    case "title":
                                        select_ds_ValueField = 'RoadFlow.Utility.ValueField.Title';
                                        break;
                                    case "note":
                                        select_ds_ValueField = 'RoadFlow.Utility.ValueField.Note';
                                        break;
                                    case "other":
                                        select_ds_ValueField = 'RoadFlow.Utility.ValueField.Other';
                                        break;
                                }
                                select_options = ("1" == select_hasempty ? '<option value=""></option>' : '')
                                    + '@Html.Raw(new RoadFlow.Business.Dictionary().GetOptionsByID'
                                    + '("' + select_ds_dict + '".ToGuid(), ' + select_ds_ValueField + ', value_' + field + '))';
                                break;
                            case "select_dssql":
                                select_options = ("1" == select_hasempty ? '<option value=""></option>' : '')
                                    + '@Html.Raw(new RoadFlow.Business.Form().GetOptionsBySQL("' + connId + '", "' + select_ds_sql + '", value_' + field + '))';
                                break;
                            case "select_dsstring":
                                var select_ds_string_array = (select_ds_string || "").split(";");
                                select_options = ("1" == select_hasempty ? '<option value=""></option>' : '');
                                for (var x = 0; x < select_ds_string_array.length; x++) {
                                    var ds_custom_string1 = select_ds_string_array[x].split(",");
                                    var title = ds_custom_string1.length > 0 ? ds_custom_string1[0] : "";
                                    var value = ds_custom_string1.length > 1 ? ds_custom_string1[1] : value;
                                    select_options += '<option value="' + value + '">' + title + '</option>';
                                }
                                break;
                        }
                        //联动
                        var select_change = '';
                        if (select_Linkage_Field && $.trim(select_Linkage_Field).length > 0) {
                            var select_change1 = '';
                            select_change = ' data-linkage_field="SUBTABLE_' + id + '_' + select_Linkage_Field + '" onchange="'
                                + (select_change1 && $.trim(select_change1).length > 0 ? select_change1 + ";" : "")
                                + "formLoad.loadChildOptions(this, 'SUBTABLE_" + id + '_' + select_Linkage_Field + "', '"
                                + select_Linkage_Source + "', '" + select_Linkage_Source_sql_conn + "', '" + select_Linkage_Source_text + "', '"
                                + 'ID' + "', '" + select_ds_dict + "', '" + select_hasempty + "', true);\"";
                        }
                        tdHTML += '<select data-value="@value_' + field + '"' + events
                            + (select_defaultvalue ? ' data-defaultvalueid="' + select_id + '"' : '')
                            + ' data-isflow="1" data-issubtable="1" data-type="select" data-table="' + secondtable + '" data-field="' + field + '" name="SUBTABLE_' + id + '_' + field + '_@(rowIndexStr)" id="SUBTABLE_' + id + '_' + field + '_@(rowIndexStr)" class="myselect"'
                            + (select_width ? ' style="width:' + select_width + ';"' : '')
                            + select_change + '>' + select_options
                            + '</select>';
                        break;
                    case "checkbox":
                        var checkbox_defaultvalue = RoadUI.Core.unescape(coleditmode.checkbox_defaultvalue);
                        var checkbox_ds = coleditmode.checkbox_ds;
                        var checkbox_ds_dict = coleditmode.checkbox_ds_dict;
                        var checkbox_ds_dict_valuefield = coleditmode.checkbox_ds_dict_valuefield || "id";
                        var checkbox_ds_sql = RoadUI.Core.unescape(coleditmode.checkbox_ds_sql);
                        var checkbox_ds_string = RoadUI.Core.unescape(coleditmode.checkbox_ds_string);
                        var checkbox_id = ('SUBTABLE_' + id + '_' + field).toUpperCase();
                        if (checkbox_defaultvalue && $.trim(checkbox_defaultvalue).length > 0) {
                            for (j = 0; j < compieform_defaultvalues.length; j++) {
                                if (compieform_defaultvalues[j].id == checkbox_id) {
                                    compieform_defaultvalues.remove(j);
                                }
                            }
                            compieform_defaultvalues.push("{'id':'" + checkbox_id + "','value':'" + checkbox_defaultvalue + "'}");
                        }
                        var checkbox_text = '';
                        var checkbox_source = '';
                        switch (checkbox_ds) {
                            case "checkbox_dsdict":
                                checkbox_source = '0';
                                break;
                            case "checkbox_dssql":
                                checkbox_source = '2';
                                checkbox_text = checkbox_ds_sql;
                                break;
                            case "checkbox_dsstring":
                                checkbox_source = '1';
                                checkbox_text = checkbox_ds_string;
                                break;
                        }
                        var checkbox_name = 'SUBTABLE_' + id + '_' + field;
                        tdHTML += '@Html.Raw(new RoadFlow.Business.Form().GetRadioOrCheckboxHtml(' + checkbox_source + ', "' + connId + '", "' + checkbox_ds_dict + '", "' + checkbox_ds_dict_valuefield
                            + '", "' + checkbox_text + '", value_' + field + ', "checkbox", "'
                            + checkbox_name + '_" + rowIndexStr, "data-isflow=\\"1\\" data-type=\\"checkbox\\" data-table=\\"'
                            + secondtable + '\\" data-field=\\"' + field + '\\" data-issubtable=\\"1\\" data-value=\\""+value_' + field + '+"\\" '
                            + events.replaceAll('"', '\\"') + '"))';
                        break;
                    case "radio":
                        var radio_defaultvalue = RoadUI.Core.unescape(coleditmode.radio_defaultvalue);
                        var radio_ds = coleditmode.radio_ds;
                        var radio_ds_dict = coleditmode.radio_ds_dict;
                        var radio_ds_dict_valuefield = coleditmode.radio_ds_dict_valuefield || "id";
                        var radio_ds_sql = RoadUI.Core.unescape(coleditmode.radio_ds_sql);
                        var radio_ds_string = RoadUI.Core.unescape(coleditmode.radio_ds_string);
                        var radio_id = ('SUBTABLE_' + id + '_' + field).toUpperCase();
                        if (radio_defaultvalue && $.trim(radio_defaultvalue).length > 0) {
                            for (j = 0; j < compieform_defaultvalues.length; j++) {
                                if (compieform_defaultvalues[j].id == radio_id) {
                                    compieform_defaultvalues.remove(j);
                                }
                            }
                            compieform_defaultvalues.push("{'id':'" + radio_id + "','value':'" + radio_defaultvalue + "'}");
                        }
                        var radio_text = '';
                        var radio_source = '';
                        switch (radio_ds) {
                            case "radio_dsdict":
                                radio_source = '0';
                                break;
                            case "radio_dssql":
                                radio_source = '2';
                                radio_text = radio_ds_sql;
                                break;
                            case "radio_dsstring":
                                radio_source = '1';
                                radio_text = radio_ds_string;
                                break;
                        }
                        var radio_name = 'SUBTABLE_' + id + '_' + field;
                        tdHTML += '@Html.Raw(new RoadFlow.Business.Form().GetRadioOrCheckboxHtml(' + radio_source + ', "' + connId + '", "' + radio_ds_dict + '", "' + radio_ds_dict_valuefield
                            + '", "' + radio_text + '", value_' + field + ', "radio", "' + radio_name + '_" + rowIndexStr, "data-isflow=\\"1\\" data-type=\\"radio\\" data-table=\\"' + secondtable + '\\" data-field=\\"' + field + '\\" data-issubtable=\\"1\\" data-value=\\""+value_' + field + '+"\\"' + events.replaceAll('"', '\\"') + '"))';
                        break;
                    case "datetime":
                        var datetime_defaultvalue = RoadUI.Core.unescape(coleditmode.datetime_defaultvalue);
                        var datetime_width = coleditmode.datetime_width;
                        var datetime_min = coleditmode.datetime_min;
                        var datetime_max = coleditmode.datetime_max;
                        var datetime_istime = coleditmode.datetime_istime;
                        var datetime_format = coleditmode.datetime_format;
                        var datetime_id = ('SUBTABLE_' + id + '_' + field).toUpperCase();
                        if (datetime_defaultvalue && $.trim(datetime_defaultvalue).length > 0) {
                            for (j = 0; j < compieform_defaultvalues.length; j++) {
                                if (compieform_defaultvalues[j].id == datetime_id) {
                                    compieform_defaultvalues.remove(j);
                                }
                            }
                            compieform_defaultvalues.push("{'id':'" + datetime_id + "','value':'" + datetime_defaultvalue + "'}");
                        }
                        tdHTML += '<input type="text" value="@value_' + field + '"' + events
                            + (text_defaultvalue ? ' data-defaultvalueid="' + datetime_id + '"' : '')
                            + ' data-isflow="1" data-issubtable="1" data-type="datetime" data-table="' + secondtable + '" data-field="' + field + '" name="SUBTABLE_' + id + '_' + field + '_@(rowIndexStr)" id="SUBTABLE_' + id + '_' + field + '_@(rowIndexStr)" class="mycalendar"'
                            + (datetime_width ? ' style="width:' + datetime_width + ';"' : '')
                            + (datetime_format ? ' format="' + datetime_format + '"' : '')
                            + (datetime_min ? ' mindate="' + datetime_min + '"' : '')
                            + (datetime_max ? ' maxdate="' + datetime_max + '"' : '')
                            + ("1" == datetime_istime ? ' istime="1"' : '') + '/>';
                        break;
                    case "org":
                        var org_defaultvalue = RoadUI.Core.unescape(coleditmode.org_defaultvalue);
                        var org_width = coleditmode.org_width;
                        var org_rang = coleditmode.org_rang;
                        var org_rang1 = coleditmode.org_rang1;
                        var org_type = coleditmode.org_type;
                        var org_more = coleditmode.org_more;
                        var org_id = ('SUBTABLE_' + id + '_' + field).toUpperCase();
                        if (org_defaultvalue && $.trim(org_defaultvalue).length > 0) {
                            for (j = 0; j < compieform_defaultvalues.length; j++) {
                                if (compieform_defaultvalues[j].id == org_id) {
                                    compieform_defaultvalues.remove(j);
                                }
                            }
                            compieform_defaultvalues.push("{'id':'" + org_id + "','value':'" + org_defaultvalue + "'}");
                        }
                        var org_rootid = '';
                        switch (org_rang) {
                            case "1":
                                break;
                            case "2":
                                break;
                            case "3":
                                org_rootid = org_rang1;
                                break;
                        }
                        var org_selecttype = ' unit="' + (org_type.indexOf(',unit,') >= 0 ? '1' : '0') + '"';
                        org_selecttype += ' dept="' + (org_type.indexOf(',dept,') >= 0 ? '1' : '0') + '"';
                        org_selecttype += ' station="' + (org_type.indexOf(',station,') >= 0 ? '1' : '0') + '"';
                        org_selecttype += ' wrokgroup="' + (org_type.indexOf(',wrokgroup,') >= 0 ? '1' : '0') + '"';
                        org_selecttype += ' user="' + (org_type.indexOf(',user,') >= 0 ? '1' : '0') + '"';
                        tdHTML += '<input type="text" value="@value_' + field + '" data-value="@value_' + field + '"' + events
                            + (org_defaultvalue ? ' data-defaultvalueid="' + org_id + '"' : '')
                            + ' data-isflow="1" data-issubtable="1" data-type="organize" data-table="' + secondtable + '" data-field="' + field + '" name="SUBTABLE_' + id + '_' + field + '_@(rowIndexStr)" id="SUBTABLE_' + id + '_' + field + '_@(rowIndexStr)" class="mymember"'
                            + (org_width ? ' style="width:' + org_width + ';"' : '')
                            + (org_rootid ? ' rootid="' + org_rootid + ';"' : '')
                            + (org_more ? ' more="1"' : '')
                            + (org_selecttype ? ' ' + org_selecttype : '')
                            + '/>';
                        break;
                    case "dict":
                        var dict_defaultvalue = RoadUI.Core.unescape(coleditmode.dict_defaultvalue);
                        var dict_width = coleditmode.dict_width;
                        var dict_rang = coleditmode.dict_rang;
                        var dict_more = coleditmode.dict_more;
                        var dict_id = ('SUBTABLE_' + id + '_' + field).toUpperCase();
                        if (dict_defaultvalue && $.trim(dict_defaultvalue).length > 0) {
                            for (j = 0; j < compieform_defaultvalues.length; j++) {
                                if (compieform_defaultvalues[j].id == dict_id) {
                                    compieform_defaultvalues.remove(j);
                                }
                            }
                            compieform_defaultvalues.push("{'id':'" + dict_id + "','value':'" + dict_defaultvalue + "'}");
                        }
                        tdHTML += '<input type="text" value="@value_' + field + '" data-value="@value_' + field + '"' + events
                            + (dict_defaultvalue ? ' data-defaultvalueid="' + dict_id + '"' : '')
                            + ' datasource="0" data-isflow="1" data-issubtable="1" data-type="lrselect" data-table="' + secondtable + '" data-field="' + field + '" name="SUBTABLE_' + id + '_' + field + '_@(rowIndexStr)" id="SUBTABLE_' + id + '_' + field + '_@(rowIndexStr)" class="mylrselect"'
                            + (dict_width ? ' style="width:' + dict_width + ';"' : '')
                            + (dict_rang ? ' rootid="' + dict_rang + '"' : '')
                            + (dict_more ? ' more="1"' : '')
                            + '/>';
                        break;
                    case "files":
                        var files_width = coleditmode.files_width;
                        var files_filetype = coleditmode.files_filetype;
                        tdHTML += '<input type="text" value="@value_' + field + '" data-value="@value_' + field + '"'
                            + ' data-isflow="1" data-issubtable="1" data-type="files" data-table="' + secondtable + '" data-field="' + field + '" name="SUBTABLE_' + id + '_' + field + '_@(rowIndexStr)" id="SUBTABLE_' + id + '_' + field + '_@(rowIndexStr)" class="myfile"'
                            + (files_width ? ' style="width:' + files_width + ';"' : '')
                            + (files_filetype ? ' filetype="' + files_filetype + ';"' : '')
                            + '/>';
                        break;
                }
                tdHTML += '</td>\r\n';
                subHTML += tdHTML;
            }
            subHTML += '\t\t\t<td>';
            subHTML += '<input type="hidden" name="SUBTABLE_' + id + '_rowindex" value="@rowIndexStr"/>';
            subHTML += '<input type="button" value="插入" class="mybutton" onclick="formLoad.subtableAddRow(\'SUBTABLE_' + id + '\',$(this).parent().parent(), false);" style="margin-right:3px;"/><input type="button" class="mybutton" value="删除" onclick="formLoad.subtableDelRow($(this).parent().parent());"/></td>\r\n';
            subHTML += '\t\t</tr>\r\n';
            subHTML += '\t\t}\r\n';
            subHTML += '\t\t}\r\n';
            subHTML += '\t</tbody>\r\n';
            if (isSum) { //添加合计行
                subHTML += '\t<tfoot>\r\n'
                subHTML += '\t\t<tr>\r\n';
                if ("1" == showindex) {//显示序号
                    subHTML += '\t\t\t<td></td>\r\n';
                }
                for (var i = 0; i < subJSON.colnums.length; i++) {
                    colJSON = subJSON.colnums[i];
                    if ("1" != colJSON.isshow) {
                        continue;
                    }
                    if ("1" == colJSON.issum) {
                        align = colJSON.align || "0";
                        align = "0" == align ? "left" : "1" == align ? "center" : "right";
                        subHTML += '\t\t\t<td data-table="SUBTABLE_' + id + '" data-field="' + colJSON.fieldname + '" style="text-align:' + align + ';"><label id="SUM_SUBTABLE_' + id + '_' + colJSON.fieldname + '"></label></td>\r\n';
                    } else {
                        subHTML += '\t\t\t<td></td>\r\n';
                    }
                }
                subHTML += '\t\t\t<td></td>\r\n';
                subHTML += '\t\t</tr>\r\n';
                subHTML += '\t</tfoot>\r\n';
            }
            subHTML += '</table>\r\n';
            //生成事件脚本
            if (eventScripts.length > 0) {
                subHTML += '<script type="text/javascript">\r\n';
                for (i = 0; i < eventScripts.length; i++) {
                    subHTML += '\tfunction ' + eventScripts[i].funname + '(srcElement)\r\n';
                    subHTML += '\t{\r\n';
                    subHTML += '\t\t' + RoadUI.Core.unescape(eventScripts[i].script) + '\r\n';
                    subHTML += '\t}\r\n';
                }
                subHTML += '</script>\r\n';
            }
        }
        else {
            //弹出模式
            subHTML = '<table data-isflowsubtable="1"' + (width && $.trim(width).length > 0 ? ' width="' + width + '"' : '') + ' id="SUBTABLE_' + id + '">\r\n';
            subHTML += '\t<thead>\r\n';
            subHTML += '\t\t<tr>\r\n';
            if ("1" == showindex) {//显示序号列
                subHTML += '\t\t\t<th>序号</th>\r\n';
            }
            sumField = new Array();//是否有合计列
            for (var i = 0; i < subJSON.colnums.length; i++) {
                var colJSON = subJSON.colnums[i];
                if ("1" != colJSON.isshow) {
                    continue;
                }
                var field = colJSON.fieldname
                if ("1" == colJSON.issum) {
                    sumField.push(field);
                }
                var colTitle = colJSON.showname || field;
                var colWidth = colJSON.width || "";
                subHTML += '\t\t\t<th' + (colWidth ? ' width="' + colWidth + '"' : '') + '>';
                subHTML += colTitle;
                subHTML += '</th>\r\n';
            }
            subHTML += '\t\t\t<th width="120px" style="text-align:left;">\r\n';
            subHTML += '\t\t\t\t<input type="button" value="添加" class="mybutton" onclick="formLoad.subtableAddData(\'' + editform + '\', ' + displaymodewidth + ', ' + displaymodeheight + ', \'@flowId\', \'@stepId\', \'@instanceId\', \'' + secondtable + '\', \'' + secondtablerelationfield + '\', \'@Request.Querys("tabid")\');"/>\r\n\t\t\t</th>\r\n';
            subHTML += '\t\t</tr>\r\n';
            subHTML += '\t</thead>\r\n';
            subHTML += '\t<tbody>\r\n';
            subHTML += '\t\t@{\r\n';
            //定义合计列
            for (j = 0; j < sumField.length; j++) {
                subHTML += '\t\t\tdecimal sum_' + sumField[j] + "_" + secondtable + "_" + primarytablefiled + "_" + secondtableprimarykey + "_" + secondtablerelationfield + " = 0;\r\n";
            }
            subHTML += '\t\t}\r\n';
            subHTML += '\t\t@{\r\n';
            if ("1" == showindex) {//定义序号变量
                subHTML += '\t\tint subtableIndex_' + secondtable + "_" + primarytablefiled + "_" + secondtableprimarykey + "_" + secondtablerelationfield + ' = 1;\r\n';
            }
            subtableMapListName = 'subtableMapList_' + secondtable + "_" + primarytablefiled + "_" + secondtableprimarykey + "_" + secondtablerelationfield;
            subHTML += '\t\tSystem.Data.DataTable ' + subtableMapListName + ' = new RoadFlow.Business.DbConnection().GetDataTable(new RoadFlow.Business.DbConnection().Get("' + connId + '".ToGuid()), "' + secondtable + '", "' + secondtablerelationfield + '", instanceId, "' + sortstring + '");\r\n';
            
            subHTML += '\t\tRoadFlow.Business.Form bform_' + secondtable + "_" + primarytablefiled + "_" + secondtableprimarykey + "_" + secondtablerelationfield + ' = new RoadFlow.Business.Form();\r\n';
            
            subHTML += '\t\tforeach(System.Data.DataRow dr in ' + subtableMapListName + '.Rows)\r\n';
            subHTML += '\t\t{\r\n';
            subHTML += '\t\t\tObject rowIndexObj = ' + subtableMapListName + '.Columns.Contains("' + secondtableprimarykey + '") ? dr["' + secondtableprimarykey + '"] : null;\r\n';
            subHTML += '\t\t\tstring rowIndexStr = null == rowIndexObj ? Guid.NewGuid().ToString() : rowIndexObj.ToString();\r\n';
            for (i = 0; i < subJSON.colnums.length; i++) {
                var colJSON = subJSON.colnums[i];
                if ("1" != colJSON.isshow) {
                    continue;
                }
                field = colJSON.fieldname;
                subHTML += '\t\t\tstring value_' + field + ' = ' + subtableMapListName + '.Columns.Contains("' + field + '") ? dr[\"' + field + '\"].ToString() : string.Empty;\r\n';
            }
            //计算合计
            for (j = 0; j < sumField.length; j++) {
                subHTML += '\t\t\tsum_' + sumField[j] + "_" + secondtable + "_" + primarytablefiled + "_" + secondtableprimarykey + "_" + secondtablerelationfield + " += value_" + sumField[j] + ".ToDecimal(0);\r\n";
            }
            subHTML += '\t\t\t<tr>\r\n';
            if ("1" == showindex) {//显示序号
                subHTML += '\t\t\t\t<td data-tdindex="1" style="text-align:center;">@(' + 'subtableIndex_' + secondtable + "_" + primarytablefiled + "_" + secondtableprimarykey + "_" + secondtablerelationfield + '++)</td>\r\n';
            }
            for (i = 0; i < subJSON.colnums.length; i++) {
                var colJSON = subJSON.colnums[i];
                if ("1" != colJSON.isshow) {
                    continue;
                }
                align = colJSON.align || "0";
                align = "0" == align ? "left" : "1" == align ? "center" : "right";
                field = colJSON.fieldname;
                displaymode = colJSON.displaymode;
                displaymodeformat = colJSON.displaymodeformat;
                displaymodesql = colJSON.displaymodesql;
                tdHTML = '\t\t\t\t<td style="text-align:' + align + ';">';
                tdHTML += '@Html.Raw(bform_' + secondtable + "_" + primarytablefiled + "_" + secondtableprimarykey + "_" + secondtablerelationfield + '.GetShowString(value_' + field + ', "' + (displaymode || '') + '", "' + (displaymodeformat || '') + '", "' + (displaymodesql || '') + '"))';
                tdHTML += '</td>\r\n';
                subHTML += tdHTML;
            }
            subHTML += '\t\t\t\t<td>';
            subHTML += '<input type="hidden" name="SUBTABLE_' + id + '_rowindex" value="@rowIndexStr"/>';
            subHTML += '<input type="button" value="编辑" class="mybutton" onclick="formLoad.subtableAddData(\'' + editform + '\', ' + displaymodewidth + ', ' + displaymodeheight + ', \'@flowId\', \'@stepId\', \'@instanceId\', \'' + secondtable + '\', \'' + secondtablerelationfield + '\', \'@Request.Querys("tabid")\', \'@dr["' + secondtableprimarykey + '"]\');" style="margin-right:3px;"/><input type="button" class="mybutton" value="删除" onclick="formLoad.subtableDelData(\'' + connId + '\',\'' + secondtable + '\',\'' + secondtableprimarykey + '\',\'@dr["' + secondtableprimarykey + '"]\', this);"/></td>\r\n';
            subHTML += '\t\t\t</tr>\r\n';
            subHTML += '\t\t}\r\n';
            subHTML += '\t\t}\r\n';
            subHTML += '\t</tbody>\r\n';
            if (sumField.length > 0) { //添加合计行
                subHTML += '\t<tfoot>\r\n'
                subHTML += '\t\t<tr>\r\n';
                if ("1" == showindex) {//显示序号
                    subHTML += '\t\t\t<td></td>\r\n';
                }
                for (var i = 0; i < subJSON.colnums.length; i++) {
                    colJSON = subJSON.colnums[i];
                    if ("1" != colJSON.isshow) {
                        continue;
                    }
                    if ("1" == colJSON.issum) {
                        align = colJSON.align || "0";
                        align = "0" == align ? "left" : "1" == align ? "center" : "right";
                        subHTML += '\t\t\t<td data-table="SUBTABLE_' + id + '" data-field="' + colJSON.fieldname + '" style="text-align:' + align + ';"><label>@sum_' + colJSON.fieldname + "_" + secondtable + "_" + primarytablefiled + "_" + secondtableprimarykey + "_" + secondtablerelationfield + '</label></td>\r\n';
                    } else {
                        subHTML += '\t\t\t<td></td>\r\n';
                    }
                }
                subHTML += '\t\t\t<td></td>\r\n';
                subHTML += '\t\t</tr>\r\n';
                subHTML += '\t</tfoot>\r\n';
            }
            subHTML += '</table>\r\n';

        }
    }
    $element.after(escapeHTML(subHTML));
    $element.remove();
}
//排序
function arrayCompare(property) {
    return function (a, b) {
        var value1 = a[property];
        var value2 = b[property];
        return value1 - value2;
    }
}
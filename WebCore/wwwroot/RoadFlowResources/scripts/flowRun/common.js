function execute(script) {
    if (!script || $.trim(script).length == 0) {
        return false;
    }
    eval(script);
}

function getMainDialog() {
    try {
        return top && top.mainDialog ? top.mainDialog : new RoadUI.Window();
    } catch (e) {
        return new RoadUI.Window();
    }
}

function checkSign() {
    if (isSign) {
        if ($.trim($("#comment").val()).length == 0) {
            alert("请填写处理意见!");
            try {
                $("#comment").focus();
            }
            catch (e) { }
            return false;
        }
        if (signType == "2") {
            if ("1" != $("#issign").val()) {
                alert("请签章!");
                try {
                    $("#signbutton").click();
                }
                catch (e) { }
                return false;
            }
        }
    }
    return true;
}

function setSign() {
    $("#issign").val("1");
    //$("#signbutton").hide();
    //$("#signbutton").prop("disabled", true);
    //$("#signimg").show();
}

function flowSend(isSubmit) {
    if (!validateForm() || !checkSign()) {
        return false;
    }
    if (!isSubmit && isSystemDetermine) {
        saveData('flowSend');
    } else {
        var mainDialog = getMainDialog();
        mainDialog.open({
            url: "/RoadFlowCore/FlowRun/FlowSend?" + query + "&instanceid1=" + ($("#form_instanceid").val() || ""),
            openerid: iframeid,
            width: isMobile ? 350 : 550,
            height: 370,
            title: "发送"
        });
    }
}

function flowFreedomSend(isSubmit) {
    if (!validateForm() || !checkSign()) {
        return false;
    }
    if (!isSubmit && isSystemDetermine) {
        saveData('flowFreedomSend');
    } else {
        var mainDialog = getMainDialog();
        mainDialog.open({
            url: "/RoadFlowCore/FlowRun/FlowSend?" + query + "&instanceid1=" + $("#form_instanceid").val() + "&freedomsend=1",
            openerid: iframeid,
            width: isMobile ? 350 : 550,
            height: 300,
            title: "自由发送"
        });
    }
}

function flowBack(isSubmit) {
    if (!checkSign()) {
        return false;
    }
    var mainDialog = getMainDialog();
    mainDialog.open({
        url: "/RoadFlowCore/FlowRun/FlowBack?" + query,
        openerid: iframeid,
        width: isMobile ? 350 : 550,
        height: 300,
        title: "退回"
    });
}

function showComment() {
    var mainDialog = getMainDialog();
    mainDialog.open({
        url: "/RoadFlowCore/FlowRun/ShowComment?" + query,
        openerid: iframeid,
        width: isMobile ? 350 : 800,
        height: 420,
        title: "查看流程处理意见"
    });
}

function flowSave(isSubmit) {
    if (!validateForm()) {
        return false;
    }
    var options = {};
    options.type = "save";
    options.steps = [];
    formSubmit(options);
}

function flowSaveIframe(flag, isSend) {
    if (flag) {
        flowSave();
    }
    else {
        contentWin = $("#customeformiframe").get(0).contentWindow;
        f = contentWin.document.forms[0];
        action = $(f).attr("action");
        if (contentWin.CKEDITOR) {
            CKEDITOR = contentWin.CKEDITOR;
        }
        if (new RoadUI.Validate().validateForm(f)) {
            var o = RoadUI.Core.serializeForm($(f));
            $("[model='html']", contentWin.document).each(function () {
                htmlId = $(this).attr("id");
                htmlName = $(this).attr("name");
                eval("o." + htmlName + "=CKEDITOR.instances." + htmlId + ".getData()");
            });
            $.ajax({
                url: action + (action.indexOf("?") >= 0 ? query : "?" + query), data: o, type: "POST", async: false, cache: false, dataType: "json", success: function (json) {
                    if (1 == json.success) {
                        $('#instanceid').val(json.instanceid);
                        $("#form_instanceid").val(json.instanceid);
                        $('#customformtitle').val(json.title);
                        if (isSend) {
                            flowSaveAndSendIframe(true)
                        }
                        else {
                            flowSaveIframe(true);
                        }
                    }
                    else {
                        alert(json.message);
                    }
                }
            });
        }
    }
}

function flowSaveAndSendIframe(flag) {
    if (flag) {
        flowSend(true);
    }
    else {
        flowSaveIframe(flag, true);
    }
}

function flowCompleted() {
    if (!validateForm() || !checkSign()) {
        return false;
    }
    var options = {};
    options.type = "completed";
    options.steps = [];
    formSubmit(options);
}

function flowCopyforCompleted() {
    var options = {};
    options.type = "copyforcompleted";
    options.steps = [];
    formSubmit(options);
}

function flowRedirect() {
    var mainDialog = getMainDialog();
    mainDialog.open({
        url: "/RoadFlowCore/FlowRun/FlowRedirect?" + query,
        openerid: iframeid,
        width: isMobile ? 350 : 450,
        height: 200,
        title: "选择接收人员"
    });
}

function formSubmit(options) {
    if (!options || !options.type || !options.steps) {
        alert("参数不足!");
        return false;
    }
    if (options.type.toLowerCase() != "save" && options.type.toLowerCase() != "completed"
        && options.type.toLowerCase() != "copyforcompleted" && options.type.toLowerCase() != "addwrite" && options.steps.length == 0) {
        alert("没有要处理的步骤!");
        return false;
    }
    showProcessing(options.type);
    window.setTimeout('', 100);
    $("#params").val(JSON.stringify(options));
    var f = document.forms[0];
    f.action = "Execute?" + query;
    f.submit();
}

function saveData(opation) {
    showProcessing("savedata");
    window.setTimeout('', 100);
    var f = document.forms[0];
    f.action = "SaveData?" + query + "&opation=" + opation;
    f.submit();
}

function validateForm() {
    //验证提示类型 0-弹出 1-图标加提示信息 2-图标
    var validateAlertType = $("#Form_ValidateAlertType").size() > 0 && !isNaN($("#Form_ValidateAlertType").val()) ? parseInt($("#Form_ValidateAlertType").val()) : 1;
    return new RoadUI.Validate().validateForm(document.forms[0], validateAlertType);
}

function showProcessing(type) {
    var title = "正在处理...";
    switch (type) {
        case "save":
            title = "正在保存...";
            break;
        case "savedata":
            title = "正在保存...";
            break;
        case "submit":
            title = "正在发送...";
            break;
        case "back":
            title = "正在退回...";
            break;
        case "redirect":
            title = "正在转交...";
            break;
        case "taskend":
            title = "正在终止...";
            break;
    }
    RoadUI.Core.showWait(title, true);
}

function sign(id) {
    var mainDialog = getMainDialog();
    mainDialog.open({
        title: "请输入签章密码",
        width: 300,
        height: 130,
        url: "/RoadFlowCore/FlowRun/Sign?" + query + (id && id.length > 0 ? "&buttonid=" + id : ""),
        openerid: iframeid,
        resize: false
    });
}

function signature(id, issign) {
    var $but = $("#" + id);
    var ispassword = $but.attr("data-ispassword") || "0";
    var id1 = $but.attr("data-id");
    if (!issign && "1" == ispassword) {
        sign(id);
    } else {
        var src = $but.attr("data-src");
        if (src) {
            $but.next('span').remove();
            $but.after('<span><img style="vertical-align:middle;margin-left:10px;" src="' + src + '" border="0" /></span>');
            $("#" + id1).val(src).next("span[class^='validate']").remove();
        }
    }
}

function showProcess() {
    var mainDialog = getMainDialog();
    mainDialog.open({
        id: 'showprocess',
        title: '查看处理过程',
        url: "/RoadFlowCore/FlowTask/Detail?" + query,
        width: isMobile ? 350 : 1024,
        height: isMobile ? 450 : 510,
        openerid: iframeid
    });
}

function showFlowDesign() {
    var mainDialog = getMainDialog();
    mainDialog.open({
        id: 'showflowdesign',
        title: '查看流程图',
        url: "/RoadFlowCore/FlowRun/ShowDesign?" + query,
        width: isMobile ? 350 : 1024,
        height: isMobile ? 450 : 510,
        openerid: iframeid
    });
}

//打印
function formPrint() {
    var w = 800;
    var h = 600;
    try {
        if (top) {
            w = $(top).width() - 200;
            h = $(top).height() - 80;
        }
    } catch (e) {
        w = $(window).width() - 200;
        h = $(window).height() - 80;
    }
    RoadUI.Core.open("/RoadFlowCore/FlowRun/Print?" + query + "&instanceid1=" + $("#form_instanceid").val() + "&isreadonly=1&display=1", w, h , "打印表单");
}

function showSubFlow() {
    var mainDialog = getMainDialog();
    mainDialog.open({
        id: 'showsubflow',
        title: '查看子流程处理过程',
        url: "/RoadFlowCore/FlowTask/DetailSubFlow?" + query,
        width: isMobile ? 350 : 1024,
        height: isMobile ? 450 : 510
    });
}

//终止
function taskEnd() {
    if (!confirm('您真的要终止当前流程吗?')) {
        return;
    }
    if (!checkSign()) {
        return;
    }
    showProcessing("taskend");
    var f = document.forms[0];
    window.setTimeout('', 100);
    $("#params").val("{\"type\":\"taskend\",\"steps\":[]}");
    //f.action = "TaskEnd?" + query;
    f.action = "Execute?" + query;
    f.submit();
}

//抄送
function flowCopyFor() {
    var mainDialog = getMainDialog();
    mainDialog.open({
        url: "/RoadFlowCore/FlowRun/FlowCopyFor?" + query,
        openerid: iframeid,
        width: isMobile ? 350 : 480,
        height: 200,
        title: "选择接收人员"
    });
}

//查看主流程表单
function showMainFlowForm() {
    var mainDialog = getMainDialog();
    mainDialog.open({
        id: 'showmainflowform',
        title: '查看主流程表单',
        url: "/RoadFlowCore/FlowRun/ShowMainForm?" + query,
        width: isMobile ? 350 : 1024,
        height: isMobile ? 450 : 510
    });
}

//加签
function addWrite() {
    if (!checkSign()) {
        return;
    }
    var mainDialog = getMainDialog();
    mainDialog.open({
        id: 'addWrite',
        title: '加签',
        url: "/RoadFlowCore/FlowRun/AddWrite?" + query,
        width: isMobile ? 350 : 550,
        height: 250,
        openerid: iframeid
    });
}

//指定后续步骤处理人
function setNextStepHandler() {
    if (RoadUI.Core.queryString("taskid").length == 0) {
        alert('请先保存当前任务再指定！');
        return;
    }
    var mainDialog = getMainDialog();
    mainDialog.open({
        id: 'setNextStepHandler',
        title: '指定后续步骤处理人',
        url: "/RoadFlowCore/FlowRun/SetNextStepHandler?" + query,
        width: isMobile ? 350 : 550,
        height: 450,
        openerid: iframeid
    });
}

//保存不通过流程设计的表单
function saveEditFormData(a) {
    if (!validateForm()) {
        return false;
    }
    if (a) {
        RoadUI.Core.disableda(a);
    }
    showProcessing("savedata");
    window.setTimeout('', 100);
    var f = document.forms[0];
    f.submit();
}

//关闭窗口
function closeWindw(openModel) {
    if (!openModel) {
        openModel = 0;
    }
    if (0 == openModel) {
        top.mainTab.closeTab();
    }
    else if (1 == openModel || 2 == openModel) {
        new RoadUI.Window().close();
    }
    else if (3 == openModel || 4 == openModel || 5 == openModel) {
        window.close();
    }
}

var wf_r = null;
var wf_steps = [];
var wf_texts = [];
var wf_conns = [];
var wf_imgs = [];
var wf_option = "";
var wf_focusObj = null;
var wf_width = 108;
var wf_height = 50;
var wf_rect = 15;
var wf_designer = false;
var wf_connColor = "#898a89";
var wf_connColor1 = "green"; //已经过步骤连线颜色
var wf_nodeBorderColor = "#587aa9";
var wf_nodeBorderColor1 = "#187713"; //已经过步骤边框颜色
var wf_noteColor = "#e6e6e8";
var wf_noteColor1 = "#37b72f"; //已经过步骤颜色
var wf_currentColor = "#fc7803";//当前步骤颜色
var wf_currentColor1 = "#c45d02";//当前步骤边框颜色
var wf_stepDefaultName = "";

var tempArrPath = [];
var mouseX = 0;
var mouseY = 0;

var wf_json = {};
var wf_id = "";
var links_tables_fields = [];
var wf_r_width = 0;
var wf_r_height = 0;

$(function () {
    wf_r_width = 4000;
    wf_r_height = 2000;
    wf_r = Raphael("flowdiv", wf_r_width, wf_r_height);
    wf_r.customAttributes.type1 = function () {
    };
    wf_r.customAttributes.fromid = function () {
    };
    wf_r.customAttributes.toid = function () {
    };
});

function addStep(x, y, text, id, isIn, stepColor, stepShape, title, imgSrc) {
    var guid = getGuid();
    var xy = getNewXY();
    x = x || xy.x;
    y = y || xy.y;
    text = text || wf_stepDefaultName;
    id = id || guid;
    if (isIE8()) {
        rect = wf_rect;//ie8显示不了椭圆
    }
    if (x > wf_r_width) {
        wf_r_width = wf_r_width + wf_width + 50;
        wf_r.setSize(wf_r_width, wf_r_height);
        //wf_r.setViewBox(0, 10, wf_r_width, wf_r_height);
    }
    if (y > wf_r_height) {
        wf_r_height = wf_r_height + wf_height + 100;
        wf_r.setSize(wf_r_width, wf_r_height);
        //wf_r.setViewBox(0, 10, wf_r_width, wf_r_height);
    }
    var rect = wf_r.rect(x, y, wf_width, wf_height, "1" == stepShape && !RoadUI.Core.isIE8() ? 50 : wf_rect);
    var fillcolor = stepColor || wf_noteColor;
    var strokecolor = wf_nodeBorderColor;
    if (isIn) {
        fillcolor = wf_noteColor1;
        strokecolor = wf_nodeBorderColor1;
    }
    rect.attr({
        "fill": fillcolor, "stroke": strokecolor, "stroke-width": 1.3, "cursor": "default", "font-family": "微软雅黑", "title": text
    });
    if (title && $.trim(title).length > 0) {
        var divId = 'stepinfodiv_' + id;
        rect.showinfo = title;
        var dialogHeight = 200;
        var dialogWidth = 480;
        var left = x, top1 = y + 112;
        rect.hover(function () {
            if (top1 + dialogHeight > $(window).height()) {
                top1 = y - 145;
            }
            if (left + dialogWidth > $(window).width()) {
                left = x - dialogWidth + wf_width;
            }
            $tooltip = $('<div id="' + divId + '" style="height:200px;overflow:auto;">' + this.showinfo + '</div>');
            $("body").append($tooltip);
            if ($('#stepinfo_' + id).size() == 0) {
                var mainDialog = null;
                try {
                    if (top && top.mainDialog) {
                        mainDialog = top.mainDialog;
                    }
                } catch (e) {
                    mainDialog = new RoadUI.Window();
                }
                mainDialog.open({
                    elementid: divId, id: 'stepinfo_' + id, left: left, top: top1, opener: window,
                    ismodal: false, showtitle: false, settopwindow: false,
                    width: dialogWidth, height: dialogHeight,
                    mouseout: function () { closeTooltip('stepinfo_' + id, left, top1); },
                    mouseover: function () { }
                });
            }
        }, function () {
            closeTooltip('stepinfo_' + id, left, top1)
        });
    }
    rect.id = id;
    rect.type1 = "step";
    wf_steps.push(rect);

    var hasImg = imgSrc && $.trim(imgSrc).length > 0;//是否有图标
    var text2 = text.length > 8 ? text.substr(0, 7) + "…" : text;
    var text1 = wf_r.text(x + 52, y + (hasImg ? 32 : 25), text2);
    text1.attr({ "font-size": "12px", "title": text });
    text1.id = "text_" + id;
    text1.type1 = "text";
    wf_texts.push(text1);
    if (hasImg) {
        var img = wf_r.image(imgSrc, x + 42, y + 7, 16, 16);
        img.id = "img_" + id;
        img.type1 = "image";
        wf_imgs.push(img);
    }
}

function closeTooltip(id, x, y) {
    var e = arguments.callee.caller.arguments[0] || window.event;
    var wx = e.clientX;
    var wy = e.clientY;
    var d_left = x;
    var d_top = y;
    var d_width = 480;
    var d_height = 204;
    if (wx < d_left || wy < d_top || wx > (d_left + d_width) || wy > (d_top + d_height)) {
        var mainDialog = null;
        try {
            if (top && top.mainDialog) {
                mainDialog = top.mainDialog;
            }
        } catch (e) {
            mainDialog = new RoadUI.Window();
        }
        mainDialog.close(id);
    }
}

function setStepText(id, txt) {
    var stepText = wf_r.getById("text_" + id);
    if (stepText != null) {
        if (txt.length > 8) {
            stepText.attr({"title": txt});
            txt = txt.substr(0, 7) + "...";
        }
        stepText.attr({"text": txt});
    }
}

function setLineText(id, txt) {
    var line;
    for (var i = 0; i < wf_conns.length; i++) {
        if (wf_conns[i].id == id) {
            line = wf_conns[i];
            break;
        }
    }
    if (!line) {
        return;
    }
    var bbox = line.arrPath.getBBox();
    var txt_x = (bbox.x + bbox.x2) / 2;
    var txt_y = (bbox.y + bbox.y2) / 2;

    var lineText = wf_r.getById("line_" + id);
    if (lineText != null) {
        if (!txt) {
            lineText.remove();
        }
        else {
            lineText.attr("x", txt_x);
            lineText.attr("y", txt_y);
            lineText.attr("text", txt || "");
            lineText.attr({"font-size": "12px"});
        }
        return;
    }
    if (txt) {
        var textObj = wf_r.text(txt_x, txt_y, txt);
        textObj.type1 = "line";
        textObj.id = "line_" + id;
        textObj.attr({"font-size": "12px"});
    }
}

function connObj(obj, isIn, text, lineType) {
    if (isLine(obj)) {
        wf_conns.push(wf_r.drawArr(obj, isIn, lineType));
        setLineText(obj.id, text);
    }
}

function isLine(obj) {
    if (!obj || !obj.obj1 || !obj.obj2) {
        return false;
    }
    if (obj.obj1 === obj.obj2) {
        return false;
    }
    if (!isStepObj(obj.obj1) || !isStepObj(obj.obj2)) {
        return false;
    }
    for (var i = 0; i < wf_conns.length; i++) {
        if (obj.obj1 === obj.obj2 || (wf_conns[i].obj1 === obj.obj1 && wf_conns[i].obj2 === obj.obj2)) {
            return false;
        }
    }
    return true;
}

function isStepObj(obj) {
    return obj && obj.type1 && obj.type1.toString() == "step";
}

function dragger() {
    this.ox = this.attr("x");
    this.oy = this.attr("y");
    changeStyle(this);
}

Raphael.fn.drawArr = function (obj, isIn, lineType) {
    if (!obj || !obj.obj1) {
        return;
    }
    lineType = lineType || 0;
    if (!obj.obj2) {
        var point1 = getStartEnd(obj.obj1, obj.obj2);
        var path2 = getArr(point1.start.x, point1.start.y, mouseX, mouseY, 7, lineType);
        for (var i = 0; i < tempArrPath.length; i++) {
            tempArrPath[i].arrPath.remove();
        }
        tempArrPath = [];
        obj.arrPath = this.path(path2);
        obj.arrPath.attr({"stroke-width": 1.7, "stroke": isIn ? wf_connColor1 : wf_connColor, "fill": wf_connColor});
        tempArrPath.push(obj);
        return;
    }

    var point = getStartEnd(obj.obj1, obj.obj2);
    var path1 = getArr(point.start.x, point.start.y, point.end.x, point.end.y, 7, lineType);
    try {
        if (obj.arrPath) {
            obj.arrPath.attr({path: path1});
        }
        else {
            obj.arrPath = this.path(path1);
            obj.arrPath.attr({
                "stroke-width": 1.7,
                "stroke": isIn ? wf_connColor1 : wf_connColor,
                "fill": wf_connColor
            });
            if (wf_designer) {
                obj.arrPath.click(connClick);
                obj.arrPath.dblclick(connSetting);
                obj.arrPath.id = obj.id;
                obj.arrPath.fromid = obj.obj1.id;
                obj.arrPath.toid = obj.obj2.id;
            }
        }
    } catch (e) {
    }
    return obj;
}

function getStartEnd(obj1, obj2) {
    var bb1 = obj1 ? obj1.getBBox() : null;
    var bb2 = obj2 ? obj2.getBBox() : null;
    var p = [
        {x: bb1.x + bb1.width / 2, y: bb1.y - 1},
        {x: bb1.x + bb1.width / 2, y: bb1.y + bb1.height + 1},
        {x: bb1.x - 1, y: bb1.y + bb1.height / 2},
        {x: bb1.x + bb1.width + 1, y: bb1.y + bb1.height / 2},
        bb2 ? {x: bb2.x + bb2.width / 2, y: bb2.y - 1} : {},
        bb2 ? {x: bb2.x + bb2.width / 2, y: bb2.y + bb2.height + 1} : {},
        bb2 ? {x: bb2.x - 1, y: bb2.y + bb2.height / 2} : {},
        bb2 ? {x: bb2.x + bb2.width + 1, y: bb2.y + bb2.height / 2} : {}
    ];
    var d = {}, dis = [];
    for (var i = 0; i < 4; i++) {
        for (var j = 4; j < 8; j++) {
            var dx = Math.abs(p[i].x - p[j].x),
                dy = Math.abs(p[i].y - p[j].y);
            if (
                (i == j - 4) ||
                (((i != 3 && j != 6) || p[i].x < p[j].x) &&
                    ((i != 2 && j != 7) || p[i].x > p[j].x) &&
                    ((i != 0 && j != 5) || p[i].y > p[j].y) &&
                    ((i != 1 && j != 4) || p[i].y < p[j].y))
            ) {
                dis.push(dx + dy);
                d[dis[dis.length - 1]] = [i, j];
            }
        }
    }
    if (dis.length == 0) {
        var res = [0, 4];
    } else {
        res = d[Math.min.apply(Math, dis)];
    }
    var result = {};
    result.start = {};
    result.end = {};
    result.start.x = p[res[0]].x;
    result.start.y = p[res[0]].y;
    result.end.x = p[res[1]].x;
    result.end.y = p[res[1]].y;
    return result;
}

function getArr(x1, y1, x2, y2, size, lineType) {
    if (!lineType) {
        lineType = 0;//线类型 0直线，1曲线
    }
    if (0 == lineType) {
        angle = Raphael.angle(x1, y1, x2, y2);
        a45 = Raphael.rad(angle - 28);
        a45m = Raphael.rad(angle + 28);
        x2a = x2 + Math.cos(a45) * size;
        y2a = y2 + Math.sin(a45) * size;
        x2b = x2 + Math.cos(a45m) * size;
        y2b = y2 + Math.sin(a45m) * size;
        return ["M", x1, y1, "L", x2, y2, "M", x2, y2, "L", x2b, y2b, "L", x2a, y2a, "z"].join(",");
    } else if (1 == lineType) {
        x11 = x1;
        y11 = y1;
        angle = y2 > y1 ? 270 : 90;
        if (x1 < x2) {
            x11 = (x2 - x1) / 2 + x1;
            angle = 180;
        } else if (x1 > x2) {
            x11 = (x1 - x2) / 2 + x2;
            angle = 0;
        }
        if (y1 < y2) {
            y11 = (y2 - y1) / 2 + y2;
        } else if (y1 > y2) {
            y11 = (y1 - y2) / 2 + y2;
        }
        a45 = Raphael.rad(angle - 28);
        a45m = Raphael.rad(angle + 28);
        x2a = x2 + Math.cos(a45) * size;
        y2a = y2 + Math.sin(a45) * size;
        x2b = x2 + Math.cos(a45m) * size;
        y2b = y2 + Math.sin(a45m) * size;
        return ["M", x1, y1, "L", x11, y1, "M", x11, y1, "L", x11, y2, "M", x11, y2, "L", x2, y2, "M", x2, y2, "L", x2b, y2b, "L", x2a, y2a, "z"].join(",");
    }
}

function getNewXY() {
    var x = 10, y = 50;
    if (wf_steps.length > 0) {
        var step = wf_steps[wf_steps.length - 1];
        x = parseInt(step.attr("x")) + 170;
        y = parseInt(step.attr("y"));
        if (x > wf_r.width - wf_width) {
            x = 10;
            y = y + 100;
        }

        if (y > wf_r.height - wf_height) {
            y = wf_r.height - wf_height;
        }
    }
    return {x: x, y: y};
}

function getGuid() {
    return Raphael.createUUID().toLowerCase();
}

function initwf() {
    wf_json = {};
    wf_steps = new Array();
    wf_texts = new Array();
    wf_conns = new Array();
    wf_r.clear();
}

Array.prototype.remove = function (n) {
    if (isNaN(n) || n > this.length) {
        return false;
    }
    this.splice(n, 1);
}

function isIE8() {
    return navigator.appName == "Microsoft Internet Explorer" && navigator.appVersion.split(";")[1].replace(/[ ]/g, "") == "MSIE8.0";
}

function removeArray(array, n) {
    if (isNaN(n) || n > array.length) {
        return false;
    }
    array.splice(n, 1);
}

//得到已完成步骤的显示title
function getStepTitle(stepId, tasks) {
    var html = '<table class="listtable"><thead><tr><th>处理人</th><th>处理类型</th><th>接收时间</th><th>完成时间</th><th>处理意见</th></tr></thead><tbody>';
    for (var j = 0; j < tasks.length; j++) {
        if (tasks[j].stepid.toLowerCase() == stepId.toLowerCase()) {
            html += '<tr><td>' + tasks[j].receiver + '</td><td>' + tasks[j].statustitle + (tasks[j].tasktype == 5 ? "(抄送)" : "") + '</td><td>' + tasks[j].sendtime + '</td><td>' + tasks[j].completedtime1 + '</td><td>' + $.trim(tasks[j].comment) + '</td></tr>';
        }
    }
    html += '</tbody></table>';
    return html;
}

function reloadFlow(json, tasks) {
    if ((typeof json) === "string") {
        json = JSON.parse(json);
    }
    if (!json || !json.id || $.trim(json.id) == "") return false;
    wf_json = json;
    wf_id = wf_json.id;
    wf_r.clear();
    wf_steps = [];
    wf_conns = [];
    wf_texts = [];
    var steps = wf_json.steps;
    tasks = tasks || taskJSON;
    if ((typeof tasks) === "string") {
        tasks = JSON.parse(tasks);
    }
    if (steps && steps.length > 0) {
        for (var i = 0; i < steps.length; i++) {
            var isIn = false;//该步骤是否走过
            var nodeTitle = "";
            for (var j = 0; j < tasks.length; j++) {
                if (tasks[j].stepid.toLowerCase() == steps[i].id.toLowerCase() && tasks[j].tasktype != 5 && !isIn) {
                    isIn = true;
                    break;
                }
            }
            if (isIn) {
                nodeTitle = getStepTitle(steps[i].id, tasks);
            }
            addStep(steps[i].position.x, steps[i].position.y, steps[i].name, steps[i].id.toLowerCase(), isIn, steps[i].stepColor, steps[i].stepShape, nodeTitle, steps[i].ico);
        }
        for (var x = 0; x < tasks.length; x++) {
            if ((tasks[x].status == "0" || tasks[x].status == "1") && tasks[x].tasktype != 5) {
                var id = tasks[x].stepid.toLowerCase();
                var step = wf_r.getById(id);
                if (step) {
                    step.attr({ "fill": wf_currentColor, "stroke": wf_currentColor1 });
                }
            }
        }
    }
    var lines = wf_json.lines;
    if (lines && lines.length > 0) {
        for (var i = 0; i < lines.length; i++) {
            var isIn = false; //该线是否走过
            for (var j = 0; j < tasks.length; j++) {
                if (tasks[j].prevstepid.toLowerCase() == lines[i].from.toLowerCase()
                    && tasks[j].stepid.toLowerCase() == lines[i].to.toLowerCase() && tasks[j].tasktype != 5 && !isIn) {
                    isIn = true;
                    break;
                }
            }
            connObj({
                id: lines[i].id,
                obj1: wf_r.getById(lines[i].from.toLowerCase()),
                obj2: wf_r.getById(lines[i].to.toLowerCase())
            }, isIn, lines[i].text, lines[i].lineType);
        }
    }
}


//id 流程ID
//dynamicStepId 动态步骤Id，包含动态步骤json要从rf_flowdynamic表中取
function openFlow1(id, dynamicStepId, groupId) {
    var json = $.ajax({
        url: "../FlowDesigner/GetJSON?type=0&appid=&flowid=" + id + "&dynamicstepid=" + (dynamicStepId || "") + "&groupid=" + (groupId || ""),
        async: false,
        cache: false,
        type: "post",
        dataType: "json",
        success: function (json) {
            reloadFlow(json);
        }
    });
}
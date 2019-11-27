var wf_r = null; //画板对象
var wf_steps = []; //步骤数组
var wf_texts = []; //文本数组
var wf_conns = []; //连线数组
var wf_imgs = []; //图标数组
var wf_option = ""; //当前操作
var wf_focusObj = null; //当前焦点对象
var wf_width = 108; //步骤宽度
var wf_height = 50; //步骤高度
var wf_rect = 15; //圆角大小
var wf_designer = true; //是否是设计模式(查看流程图时不绑定双击事件）
var wf_connColor = "#959a9d"; //连线的常规颜色
var wf_nodeBorderColor = "#587aa9"; //节点边框颜色
var wf_noteColor = "#e6e6e8"; //节点填充颜色
var wf_hoverColor = "#ed0404"; //鼠标经过颜色
var wf_focusColor = "#ed0404"; //当前焦点颜色
var wf_stepDefaultName = "步骤";//默认步骤名称
var wf_lineType = 0; //当前连线类型

var tempArrPath = []; //临时连线
var mouseX = 0;
var mouseY = 0;

var wf_json = {}; //设计json
var wf_id = "";//当前流程ID
var links_tables_fields = [];//当前流程的所有连接所有表和字段

$(function () {
    wf_r = Raphael("flowdiv", $(window).width(), $(window).height() - 28);
    wf_r.customAttributes.type1 = function () { };
    wf_r.customAttributes.fromid = function () { };
    wf_r.customAttributes.toid = function () { };
});

//添加步骤
function addStep(x, y, text, id, addToJSON, type1, bordercolor, bgcolor, rect, imgSrc) {
    var guid = getGuid();
    var xy = getNewXY();
    x = x || xy.x;
    y = y || xy.y;
    text = text || wf_stepDefaultName;
    id = id || guid;
    if(RoadUI.Core.isIE8()){
        rect = wf_rect;//ie8显示不了椭圆
    }
    rect = wf_r.rect(x, y, wf_width, wf_height, rect || wf_rect);
    var step_color = bgcolor || wf_noteColor;
    rect.attr({
        "fill": step_color,
        "stroke": bordercolor || wf_nodeBorderColor,
        "stroke-width": 1.3,
        "cursor": "default", "title": text
    });
    rect.id = id;
    rect.type1 = type1 ? type1 : "normal";
    rect.drag(move, dragger, up);
    rect.data("stepcolor", step_color);
    if (wf_designer) {
        rect.click(click);
        if ("normal" == rect.type1) {
            rect.dblclick(stepSetting);
        }
        else if ("subflow" == rect.type1) {
            rect.dblclick(subflowSetting);
        }
    }
    wf_steps.push(rect);
    var hasImg = imgSrc && $.trim(imgSrc).length > 0;//是否有图标
    var text2 = text.length > 8 ? text.substr(0, 7) + "…" : text;
    var text1 = wf_r.text(x + 52, y + (hasImg ? 32 : 25), text2);
    text1.attr({ "font-size": "12px", "title": text, "font-family": "微软雅黑" });
    text1.id = "text_" + id;
    text1.type1 = "text";
    wf_texts.push(text1);
    if (hasImg) {
        var img = wf_r.image(imgSrc, x + 42, y + 7, 16, 16);
        img.id = "img_" + id;
        img.type1 = "image";
        wf_imgs.push(img);
    }
    if (addToJSON == undefined || addToJSON == null) addToJSON = true;
    if (addToJSON) {
        var step = {};
        step.id = guid;
        step.type = type1 ? type1 : "normal";
        step.name = text;
        step.position = {x: x, y: y, width: wf_width, height: wf_height};
        step.opinionDisplay = "";
        step.expiredPrompt = "";
        step.signatureType = "";
        step.workTime = "";
        step.limitTime = "";
        step.otherTime = "";
        step.archives = "";
        step.archivesParams = "";
        step.note = "";
        step.behavior = {};
        step.forms = [];
        step.buttons = [];
        step.fieldStatus = [];
        step.event = {};
        step.subflow = {};
        addStep1(step);
    }
}

//添加子流程节点
function addSubFlow() {
    addStep(null, null, "子流程步骤", null, null, "subflow", null, null)
}

function clone(obj) {
    var o;
    switch (typeof obj) {
        case 'undefined':
            break;
        case 'string':
            o = obj + '';
            break;
        case 'number':
            o = obj - 0;
            break;
        case 'boolean':
            o = obj;
            break;
        case 'object':
            if (obj === null) {
                o = null;
            } else {
                if (obj instanceof Array) {
                    o = [];
                    for (var i = 0, len = obj.length; i < len; i++) {
                        o.push(clone(obj[i]));
                    }
                } else {
                    o = {};
                    for (var k in obj) {
                        o[k] = clone(obj[k]);
                    }
                }
            }
            break;
        default:
            o = obj;
            break;
    }
    return o;
}

//复制当前选中步骤
function copyStep() {
    if (wf_focusObj == null || !isStepObj(wf_focusObj)) {
        alert("请选择要复制的步骤");
        return;
    }
    var json = {};
    var text = "";
    var id = getGuid();
    if (wf_json && wf_json.steps) {
        for (var i = 0; i < wf_json.steps.length; i++) {
            if (wf_json.steps[i].id == wf_focusObj.id) {
                json = clone(wf_json.steps[i]);
                json.forms = wf_json.steps[i].forms;
                json.buttons = wf_json.steps[i].buttons;
                json.fieldStatus = wf_json.steps[i].fieldStatus;
                json.id = id;
                text = json.name;
                addStep1(json);
                addStep(undefined, undefined, text, id, false);
                break;
            }
        }
    }
}

//设置步骤文本
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

//设置步骤图标
function setStepIco(id, x, y, imgSrc) {
    var stepIco = wf_r.getById("img_" + id);
    if (stepIco != null) {
        if (!imgSrc || $.trim(imgSrc).length == 0) {
            for (var i = 0; i < wf_imgs.length; i++) {
                if (wf_imgs[i].id == "img_" + id) {
                    wf_imgs.remove(i);
                }
            }
            for (var j = 0; j < wf_texts.length; j++) {
                if (wf_texts[j].id == "text_" + id) {
                    wf_texts[j].attr("y", parseFloat(wf_texts[j].attr("y")) - 7);
                }
            }
        }
        stepIco.attr({ "src": imgSrc });
    } else {
        if (!imgSrc || $.trim(imgSrc).length == 0) {
            return;
        }
        var img = wf_r.image(imgSrc, x + 42, y + 7, 16, 16);
        img.id = "img_" + id;
        img.type1 = "image";
        wf_imgs.push(img);
        var stepText = wf_r.getById("text_" + id);
        if (stepText) {
            stepText.attr({ "y": y + 32 });
        }
    }
}

//设置步骤样式 color 颜色 shape 形状 0炬型 1椭圆 2圆
function setStepStyle(id, color, shape) {
    var step = wf_r.getById(id);
    if (step) {
        var stepcolor = "";
        if (color) {
            stepcolor = color;
        } else {
            stepcolor = wf_noteColor;
        }
        step.attr("fill", stepcolor);
        step.data("stepcolor", stepcolor);
        var isIE8 = RoadUI.Core.isIE8();
        if(isIE8){
            shape = 0;
        }
        if (shape) {
            if ("0" == shape) {
                step.attr("r", wf_rect);
            }
            else if ("1" == shape) {
                step.attr({ "r": wf_height })
            }
            else if ("2" == shape) {
                step.attr({"width": wf_height+25, "height": wf_height+25, "r": wf_height+25});
                var text = wf_r.getById("text_" + id);
                if (text) {
                    var text_x = text.attr("x");
                    text.attr({"x": parseFloat(text_x)+15 - ((wf_height+50) / 2)});
                }
            }
        }
    }
}

//得到新步骤的XY
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

//添加连线
function addConn(lineType) {
    if (!wf_focusObj || !isStepObj(wf_focusObj)) {
        alert("请选择要连接的步骤!");
        return false;
    }
    wf_option = "line";
    wf_lineType = lineType;
    document.body.onmousemove = mouseMove;
    document.body.onmousedown = function () {
        for (var i = 0; i < tempArrPath.length; i++) {
            tempArrPath[i].arrPath.remove();
        }
        tempArrPath = [];
        document.body.onmousemove = null;
    };
}

function mouseCoords(ev) {
    if (ev.pageX || ev.pageY) {
        return {x: ev.pageX, y: ev.pageY};
    }
    return {
        x: ev.clientX + document.body.scrollLeft - document.body.clientLeft,
        y: ev.clientY + document.body.scrollTop - document.body.clientTop
    };
}

//连接对象
function connObj(obj, addToJSON, title, lineType) {
    if (addToJSON == undefined || addToJSON == null) addToJSON = true;
    if (isLine(obj)) {
        lineType = !isNaN(lineType) ? lineType : 0;
        var newline = wf_r.drawArr(obj, lineType);
        wf_conns.push(newline);
        if (addToJSON) {
            var line = {};
            line.id = obj.id;
            line.from = obj.obj1.id;
            line.to = obj.obj2.id;
            line.customMethod = "";
            line.sql = "";
            line.noaccordMsg = "";
            line.text = title || "";
            line.lineType = lineType || (wf_lineType || 0);
            addLine(line);
        } else {
            if (title) {
                setLineText(obj.id, title);
            }
        }
    }
}

//单击事件执行相关操作
function click() {
    var o = this;
    switch (wf_option) {
        case "line":
            var obj = {id: getGuid(), obj1: wf_focusObj, obj2: o};
            connObj(obj);
            break;
        default:
            changeStyle(o);
            break;
    }
    wf_option = "";
    wf_lineType = undefined;
    wf_focusObj = this;
}

//连线单击事件
function connClick() {
    for (var i = 0; i < wf_conns.length; i++) {
        if (wf_conns[i].arrPath === this) {

            wf_conns[i].arrPath.attr({"stroke": "#db0f14"});
        }
        else {
            wf_conns[i].arrPath.attr({"stroke": wf_connColor});
        }
    }
    //for (var i = 0; i < wf_steps.length; i++) {
    //    wf_steps[i].attr("fill", "#efeff0");
    //    wf_steps[i].attr("stroke", "#23508e");
    //}
    wf_focusObj = this;
}

//判断一个节点与另一个节点之间是否可以连线
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

//判断一个对象是否是步骤对象
function isStepObj(obj) {
    return obj && obj.type1 && (obj.type1.toString() == "normal" || obj.type1.toString() == "subflow");
}

//得到GUID
function getGuid() {
    return Raphael.createUUID();
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
        wf_texts.push(textObj);
    }
    //line.arrPath.attr("title", txt);
}

//删除当前焦点及其附属对象
function removeObj() {
    if (!wf_focusObj) {
        alert("请选择要删除的对象!");
        return false;
    }
    else if (!confirm('您真的要删除选定对象吗?')) {
        return false;
    }
    if (isStepObj(wf_focusObj))//如果选中的是步骤
    {
        if (wf_focusObj.id) {
            for (var i = 0; i < wf_texts.length; i++) {
                if (wf_texts[i].id == "text_" + wf_focusObj.id) {
                    wf_texts.remove(i);
                    var text = wf_r.getById("text_" + wf_focusObj.id);
                    if (text) text.remove();
                }
            }
            for (var i = 0; i < wf_imgs.length; i++) {
                if (wf_imgs[i].id == "img_" + wf_focusObj.id) {
                    wf_imgs.remove(i);
                    var img = wf_r.getById("img_" + wf_focusObj.id);
                    if (img) img.remove();
                }
            }
        }
        var deleteConnIndex = new Array();
        for (var j = 0; j < wf_conns.length; j++) {
            if (wf_conns[j].arrPath && (wf_conns[j].obj1.id == wf_focusObj.id || wf_conns[j].obj2.id == wf_focusObj.id)) {
                deleteLine(wf_conns[j].id, wf_conns[j].arrPath.id);
                deleteConnIndex.push(j);
                wf_conns[j].arrPath.remove();
            }
        }
        for (var m = deleteConnIndex.length; m--;) {
            wf_conns.remove(deleteConnIndex[m]);
        }
        deleteConnIndex = new Array();

        for (var k = 0; k < wf_steps.length; k++) {
            if (wf_steps[k].id == wf_focusObj.id) {
                wf_steps.remove(k);
                deleteStep(wf_focusObj.id);
            }
        }
        wf_focusObj.remove();
    }
    else//如果选中的是线
    {
        for (var j = 0; j < wf_conns.length; j++) {
            if (wf_conns[j].arrPath && wf_conns[j].arrPath.id == wf_focusObj.id) {
                deleteLine(wf_conns[j].id, wf_conns[j].arrPath.id);
                wf_conns.remove(j);
            }
        }
        wf_focusObj.remove();
    }
    wf_focusObj = undefined;
}

function mouseMove(ev) {
    ev = ev || window.event;
    var mousePos = mouseCoords(ev);
    mouseX = mousePos.x;
    mouseY = mousePos.y;
    var obj = {obj1: wf_focusObj, obj2: null};
    wf_r.drawArr(obj);
}

//改变节点样式
function changeStyle(obj) {
    if (!obj) {
        return;
    }
    for (var i = 0; i < wf_steps.length; i++) {
        var noteColor = wf_noteColor;
        if (wf_steps[i].data("stepcolor")) {
            noteColor = wf_steps[i].data("stepcolor");
        }
        if (wf_steps[i].id == obj.id) {
            wf_steps[i].attr("fill", noteColor);
            wf_steps[i].attr("stroke", wf_focusColor);
        }
        else {
            wf_steps[i].attr("fill", noteColor);
            wf_steps[i].attr("stroke", wf_nodeBorderColor);
        }
    }

    //for (var i = 0; i < wf_conns.length; i++) {
    //    if (wf_conns[i].arrPath) {
    //        wf_conns[i].arrPath.attr({"stroke": wf_connColor, "fill": wf_connColor});
    //    }
    //}
    //obj.animate({ }, 500);
}

//拖动节点开始时的事件
function dragger() {
    this.ox = this.attr("x");
    this.oy = this.attr("y");
    changeStyle(this);
}

//拖动事件
function move(dx, dy) {
    var x = this.ox + dx;
    var y = this.oy + dy;

    if (x < 0) {
        x = 0;
    }
    else if (x > wf_r.width - wf_width) {
        x = wf_r.width - wf_width;
    }

    if (y < 0) {
        y = 0;
    }
    else if (y > wf_r.height - wf_height) {
        y = wf_r.height - wf_height;
    }
    this.attr("x", x);
    this.attr("y", y);
    if (this.id) {
        var img = wf_r.getById("img_" + this.id);
        if (img) {
            img.attr("x", x + 42);
            img.attr("y", y + 7);
        }
        var text = wf_r.getById('text_' + this.id);
        if (text) {
            text.attr("x", x + 52);
            text.attr("y", y + (img ? 32 : 25));
        }
    }
    for (var j = wf_conns.length; j--;) {
        if (wf_conns[j].obj1.id == this.id || wf_conns[j].obj2.id == this.id) {
            var lineType = 0;
            for (var n = 0; n < wf_json.lines.length; n++) {
                if (wf_json.lines[n].id == wf_conns[j].arrPath.id) {
                    setLineText(wf_json.lines[n].id, wf_json.lines[n].text);
                    lineType = wf_json.lines[n].lineType;
                    break;
                }
            }
            wf_r.drawArr(wf_conns[j], lineType);
        }
    }
    wf_r.safari();
}

//拖动结束后的事件
function up() {
    changeStyle(this);
    //记录移动后的位置
    if (isStepObj(this)) {
        var bbox = this.getBBox();
        if (bbox) {
            var steps = wf_json.steps;
            if (steps && steps.length > 0) {
                for (var i = 0; i < steps.length; i++) {
                    if (steps[i].id == this.id) {
                        steps[i].position = {"x": bbox.x, "y": bbox.y, "width": bbox.width, "height": bbox.height};
                        break;
                    }
                }
            }
        }
    }
}

//随着节点位置的改变动态改变箭头
Raphael.fn.drawArr = function (obj, lineType) {
    if (!obj || !obj.obj1) {
        return;
    }
    lineType = lineType || (wf_lineType || 0);
    if (!obj.obj2) {
        var point1 = getStartEnd(obj.obj1, obj.obj2);
        var path2 = getArr(point1.start.x, point1.start.y, mouseX, mouseY, 7, lineType);
        for (var i = 0; i < tempArrPath.length; i++) {
            tempArrPath[i].arrPath.remove();
        }
        tempArrPath = [];
        obj.arrPath = this.path(path2);
        obj.arrPath.attr({"stroke-width": 1.7, "stroke": wf_connColor, "fill": wf_connColor});
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
                "stroke": wf_connColor,
                "fill": wf_connColor,
                "x": point.start.x,
                "y": point.start.y,
                "x1": point.end.x,
                "y1": point.end.y
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

//初始化画板
function initwf() {
    wf_json = {};
    wf_steps = new Array();
    wf_texts = new Array();
    wf_conns = new Array();
    wf_r.clear();
}

function removeArray(array, n) {
    if (isNaN(n) || n > array.length) {
        return false;
    }
    array.splice(n, 1);
}

//加载一个流程的所有连接的表和字段
function init_links_tables_fields(dbs) {
    $.ajax({
        url: "../Dbconnection/GetTableJSON?appid=" + appid,
        dataType: "json",
        data: {"dbs": JSON.stringify(dbs)},
        type: "post",
        async: false,
        success: function (json) {
            links_tables_fields = json;
        }
    });
}

//得到一连接的所有表
function getTables(connid, table) {
    var html = "";
    $.ajax({
        url: "../Dbconnection/GetTableOptions?connid=" + connid + "&table=" + (table || ""),
        async: false,
        success: function (options) {
            html = options;
        }
    });
    return html;
}

//得到一个表所有字段
function getFields(connid, table, field) {
    var fields = "";
    $.ajax({
        url: "../Dbconnection/GetFieldsOptions?connid=" + connid + "&table=" + table + "&field=" + (field || ""),
        async: false,
        success: function (options) {
            fields = options;
        }
    });
    return fields;
}

//添加步骤
function addStep1(step) {
    if (!step) return;
    if (!wf_json.steps) wf_json.steps = [];
    var isup = false;
    for (var i = 0; i < wf_json.steps.length; i++) {
        if (wf_json.steps[i].id == step.id) {
            wf_json.steps[i] = step;
            setStepStyle(step.id, step.stepColor, step.stepShape);
            isup = true;
        }
    }
    if (!isup) {
        wf_json.steps.push(step);
    }
}

//添加线
function addLine(line) {
    if (!line || !line.from || !line.to) return;
    if (!wf_json.lines) wf_json.lines = [];
    var isup = false;
    for (var i = 0; i < wf_json.lines.length; i++) {
        if (wf_json.lines[i].id == line.id) {
            line.lineType = wf_json.lines[i].lineType;
            wf_json.lines[i] = line;
            isup = true;
        }
    }
    if (!isup) {
        wf_json.lines.push(line);
    }
    setLineText(line.id, line.text);
}

//根据当前JSON重载入流程
function reloadFlow(json) {
    if (!json || !json.id || $.trim(json.id) == "") return false;
    wf_json = json;
    wf_id = wf_json.id;
    wf_r.clear();
    wf_steps = [];
    wf_conns = [];
    wf_texts = [];
    var steps = wf_json.steps;
    if (steps && steps.length > 0) {
        for (var i = 0; i < steps.length; i++) {
            addStep(steps[i].position.x, steps[i].position.y, steps[i].name, steps[i].id, false, steps[i].type, "", steps[i].stepColor, steps[i].stepShape == 1 ? wf_height : wf_rect, steps[i].ico);
        }
    }
    var lines = wf_json.lines;
    if (lines && lines.length > 0) {
        for (var i = 0; i < lines.length; i++) {
            connObj({
                id: lines[i].id,
                obj1: wf_r.getById(lines[i].from),
                obj2: wf_r.getById(lines[i].to)
            }, false, lines[i].text, lines[i].lineType);
        }
    }
    $("#flowatt").attr("title", "名称：" + wf_json.name);
    //初始化数据连接
    init_links_tables_fields(wf_json.databases, wf_json.titleField);
}

//从json中删除步骤
function deleteStep(stepid) {
    var steps = wf_json.steps;
    if (steps && steps.length > 0) {
        for (var i = 0; i < steps.length; i++) {
            if (steps[i].id == stepid) {
                removeArray(steps, i);
            }
        }
    }
}

//从json中删除线
function deleteLine(lineid, textid) {
    var lines = wf_json.lines;
    if (lines && lines.length > 0) {
        for (var i = 0; i < lines.length; i++) {
            if (lines[i].id == lineid) {
                removeArray(lines, i);
            }
        }
    }
    if (textid) {
        if (wf_texts && wf_texts.length > 0) {
            for (var i = 0; i < wf_texts.length; i++) {

                if (wf_texts[i].id == "line_" + textid) {
                    wf_texts[i].remove();
                }
            }
        }
    }
}

//步骤属性设置(步骤双击)
function stepSetting() {
    var bbox = this.getBBox();
    var url = "Set_Step?issubflow=0&appid=" + appid + "&id=" + this.id + "&x=" + bbox.x + "&y=" + bbox.y + "&width=" + bbox.width + "&height=" + bbox.height;
    dialog.open({
        title: "步骤设置",
        width: 700,
        height: 455,
        url: url,
        opener: window,
        openerid: iframeid,
        resize: false,
        showmaskdiv: false
    });
}

//子流程设置
function subflowSetting() {
    var bbox = this.getBBox();
    var url = "Set_Step?issubflow=1&appid=" + appid + "&id=" + this.id + "&x=" + bbox.x + "&y=" + bbox.y + "&width=" + bbox.width + "&height=" + bbox.height;
    dialog.open({
        title: "子流程步骤参数设置",
        width: 700,
        height: 455,
        url: url,
        opener: window,
        openerid: iframeid,
        resize: false,
        showmaskdiv: false
    });
}

//流转条件设置(连线双击)
function connSetting() {
    var url = "Set_Line?appid=" + appid + "&id=" + this.id + "&from=" + this.fromid + "&to=" + this.toid;
    dialog.open({
        title: "流转条件设置",
        width: 700,
        height: 420,
        url: url,
        opener: window,
        openerid: iframeid,
        resize: false,
        showmaskdiv: false
    });
}

//流程属性设置
function flowAttrSetting(isAdd) {
    var url = "Set_Flow?appid=" + appid + "&isadd=" + (isAdd || 0).toString() + "&flowid=" + (wf_json.id || "");
    dialog.open({
        title: "流程设置",
        width: 550,
        height: 410,
        url: url,
        opener: window,
        openerid: iframeid,
        resize: false,
        showmaskdiv: false
    });
}

//打开流程
function openFlow() {
    var url = "Open?appid=" + appid;
    dialog.open({
        title: "打开流程",
        width: 1000,
        height: 500,
        url: url,
        opener: top,
        openerid: iframeid,
        resize: false,
        showmaskdiv: false
    });
}

function openFlow1(id) {
    var json = $.ajax({
        url: "GetJSON?appid=" + appid + "&flowid=" + id,
        type: "post",
        async: false,
        cache: false,
        dataType: "json",
        success: function (json) {
            reloadFlow(json);
        }
    });
}

//新建流程
function addFlow() {
    flowAttrSetting(1);
}

//保存流程
function saveFlow(op)
{
    if (!wf_json)
    {
        alert("未设置流程!");
        return false;
    }
    else if (!wf_json.id || $.trim(wf_json.id) == "")
    {
        alert("请先新建或打开流程!");
        return false;
    }
    else if (!wf_json.name || $.trim(wf_json.name) == "")
    {
        alert("流程名称不能为空!");
        return false;
    }
    if (op == "delete" && !confirm("您真的要删除该流程吗?"))
    {
        return;
    }
    var title = "";
    if (op == "save") title = "保存流程";
    else if (op == "install") title = "安装流程";
    else if (op == "uninstall") title = "卸载流程";
    else if (op == "delete") title = "删除流程";
    var url = "Opation?appid=" + appid + "&flowid=" + wf_json.id + "&op=" + (op || "save");
    dialog.open({
        title: title,
        width: 260,
        height: 120,
        url: url,
        opener: window,
        openerid: iframeid,
        resize: false,
        showclose: false
    });
}

//另存为
function saveAs() {
    if (!wf_json || !wf_json.id || $.trim(wf_json.id) == "") {
        alert("请先新建或打开一个流程!");
        return false;
    }
    var url = "SaveAs?appid=" + appid + "&flowid=" + wf_json.id;
    dialog.open({
        title: "流程另存为",
        width: 600,
        height: 230,
        url: url,
        opener: window,
        openerid: iframeid,
        resize: false,
        showmaskdiv: false
    });
}

/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/
/*
Clara is a server-side DOM rendering library for C#
This file is the client runtime for Clara.
https://claraui.com
*/
var ClaraUI;
(function (ClaraUI) {
    var documentId;
    function initialize(id) {
        documentId = id;
        window.addEventListener("unload", terminate, false);
    }
    ClaraUI.initialize = initialize;
    function terminate() {
        var url = "/_discard?doc=" + documentId;
        navigator.sendBeacon(url);
    }
    function plug(el, eventName) {
        var url = getEventUrl(el, eventName);
        sendAjax(url);
    }
    ClaraUI.plug = plug;
    function getEventUrl(el, eventName) {
        return "/_event?doc=" + documentId
            + "&el=" + el.id
            + "&ev=" + eventName;
    }
    function sendAjax(url) {
        var ajax = new XMLHttpRequest();
        ajax.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    processAjaxResult(this);
                }
                else {
                    console.log("Error in Ajax call: " + this.statusText);
                }
            }
        };
        var message = ClaraUI.collectValues();
        ajax.open("POST", url, true);
        if (message.isEmpty()) {
            ajax.send();
        }
        else {
            ajax.send(JSON.stringify(message));
        }
    }
    function processAjaxResult(ajax) {
        var result = JSON.parse(ajax.responseText);
        if (result.List) {
            ClaraUI.processResult(result.List);
        }
    }
})(ClaraUI || (ClaraUI = {}));
/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/
var ClaraUI;
(function (ClaraUI) {
    var ContentNodeType;
    (function (ContentNodeType) {
        ContentNodeType[ContentNodeType["Element"] = 1] = "Element";
        ContentNodeType[ContentNodeType["Text"] = 2] = "Text";
    })(ContentNodeType = ClaraUI.ContentNodeType || (ClaraUI.ContentNodeType = {}));
})(ClaraUI || (ClaraUI = {}));
/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/
var ClaraUI;
(function (ClaraUI) {
    var DeltaType;
    (function (DeltaType) {
        DeltaType[DeltaType["Append"] = 1] = "Append";
        DeltaType[DeltaType["Insert"] = 2] = "Insert";
        DeltaType[DeltaType["TextModified"] = 3] = "TextModified";
        DeltaType[DeltaType["Remove"] = 4] = "Remove";
        DeltaType[DeltaType["EditAttribute"] = 5] = "EditAttribute";
        DeltaType[DeltaType["RemoveAttribute"] = 6] = "RemoveAttribute";
        DeltaType[DeltaType["Focus"] = 7] = "Focus";
        DeltaType[DeltaType["SetId"] = 8] = "SetId";
        DeltaType[DeltaType["SetValue"] = 9] = "SetValue";
    })(DeltaType = ClaraUI.DeltaType || (ClaraUI.DeltaType = {}));
})(ClaraUI || (ClaraUI = {}));
/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/
var ClaraUI;
(function (ClaraUI) {
    var ClientEventValue = /** @class */ (function () {
        function ClientEventValue() {
        }
        return ClientEventValue;
    }());
    ClaraUI.ClientEventValue = ClientEventValue;
    var ClientEventMessage = /** @class */ (function () {
        function ClientEventMessage() {
        }
        ClientEventMessage.prototype.isEmpty = function () {
            return this.Values.length == 0;
        };
        return ClientEventMessage;
    }());
    ClaraUI.ClientEventMessage = ClientEventMessage;
    function collectValues() {
        var message = new ClientEventMessage();
        message.Values = [];
        collectElementType("input", message);
        collectElementType("textarea", message);
        collectElementType("select", message);
        collectElementType("button", message);
        return message;
    }
    ClaraUI.collectValues = collectValues;
    function collectElementType(tagName, message) {
        var list = document.getElementsByTagName(tagName);
        for (var index = 0; index < list.length; index++) {
            var input = list[index];
            if (input.id) {
                var value = new ClientEventValue();
                value.ElementId = input.id;
                value.Value = input.value;
                message.Values.push(value);
            }
        }
    }
})(ClaraUI || (ClaraUI = {}));
/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/
var ClaraUI;
(function (ClaraUI) {
    function processResult(steps) {
        for (var _i = 0, steps_1 = steps; _i < steps_1.length; _i++) {
            var step = steps_1[_i];
            processStep(step);
        }
    }
    ClaraUI.processResult = processResult;
    function processStep(step) {
        switch (step.Type) {
            case ClaraUI.DeltaType.Append:
                append(step);
                break;
            case ClaraUI.DeltaType.Insert:
                insert(step);
                break;
            case ClaraUI.DeltaType.TextModified:
                textModified(step);
                break;
            case ClaraUI.DeltaType.Remove:
                remove(step);
                break;
            case ClaraUI.DeltaType.EditAttribute:
                editAttribute(step);
                break;
            case ClaraUI.DeltaType.RemoveAttribute:
                removeAttribute(step);
                break;
            case ClaraUI.DeltaType.Focus:
                focus(step);
                break;
            case ClaraUI.DeltaType.SetId:
                setId(step);
                break;
            case ClaraUI.DeltaType.SetValue:
                setValue(step);
                break;
            default:
                console.log("Error processing event response. Unknown step type: " + step.Type);
        }
    }
    function append(delta) {
        var el = document.getElementById(delta.ParentId);
        var child = createNode(delta.NodeAdded);
        el.appendChild(child);
    }
    function insert(delta) {
        var el = document.getElementById(delta.ParentElementId);
        var child = createNode(delta.ContentNode);
        if (delta.Index < el.childNodes.length) {
            var before = el.childNodes[delta.Index];
            el.insertBefore(child, before);
        }
        else {
            el.appendChild(child);
        }
    }
    function createNode(node) {
        if (node.Type == ClaraUI.ContentNodeType.Text) {
            return createTextNode(node);
        }
        else if (node.Type == ClaraUI.ContentNodeType.Element) {
            return createElementNode(node);
        }
        else {
            console.log("Error processing event response. Unknown content type: " + node.Type);
            document.createTextNode("");
        }
    }
    function createTextNode(node) {
        return document.createTextNode(node.Data);
    }
    function createElementNode(node) {
        var child = document.createElement(node.TagName);
        for (var _i = 0, _a = node.Attributes; _i < _a.length; _i++) {
            var attribute = _a[_i];
            child.setAttribute(attribute.Attribute, attribute.Value);
        }
        for (var _b = 0, _c = node.Children; _b < _c.length; _b++) {
            var branch = _c[_b];
            child.appendChild(createNode(branch));
        }
        return child;
    }
    function textModified(delta) {
        var el = document.getElementById(delta.ParentElementId);
        var child = el.childNodes[delta.ChildNodeIndex];
        child.textContent = delta.Text;
    }
    function remove(delta) {
        var parent = document.getElementById(delta.ParentId);
        var child = parent.childNodes[delta.ChildIndex];
        child.remove();
    }
    function editAttribute(delta) {
        var el = document.getElementById(delta.ElementId);
        el.setAttribute(delta.Attribute, delta.Value);
    }
    function removeAttribute(delta) {
        var el = document.getElementById(delta.ElementId);
        el.removeAttribute(delta.Attribute);
    }
    function focus(delta) {
        var el = document.getElementById(delta.ElementId);
        el.focus();
    }
    function setId(delta) {
        var el = resolveElement(delta.Locator);
        el.id = delta.NewId;
    }
    function resolveElement(locator) {
        var el = document.getElementById(locator.StartingId);
        for (var index = locator.Steps.length; index >= 0; index--) {
            var step = locator.Steps[index];
            el = el.childNodes[step];
        }
        return el;
    }
    function setValue(delta) {
        var input = document.getElementById(delta.ElementId);
        input.value = delta.Value;
    }
})(ClaraUI || (ClaraUI = {}));

/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

import {
    ContentArrayNode, ContentElementNode, ContentNode,
    ContentNodeType, ContentTextNode
} from "./ContentInterfaces";
import {
    AttributeEditedDelta, AttributeRemovedDelta, BaseDelta, ClearChildrenDelta, DeltaType,
    ElementLocator, FocusDelta, NodeAddedDelta, NodeInsertedDelta, NodeRemovedDelta,
    ReplaceDelta, SetCheckedDelta, SetIdDelta, SetValueDelta, SubmitJsDelta, SubscribeDelta,
    SwapChildrenDelta, TextModifiedDelta, UnsubscribeDelta
} from "./DeltaInterfaces";
import { listenServerEvents, plug, plugEvent } from "./index";
import { addElementEvent, removeElementEvent } from "./RegisteredEvents";
import { debounce } from "debounce";

export function processResult(steps: BaseDelta[]): void {
    for (var step of steps) {
        if (!processStepCatch(step)) {
            return;
        }
    }
}

function processStepCatch(step: BaseDelta): boolean {
    try {
        processStep(step);
        return true;
    } catch (Error) {
        console.log("Error processing step:");
        console.log(Error);
        console.log(step);
        return false;
    }
}

function processStep(step: BaseDelta): void {
    switch (step.Type) {
        case DeltaType.Append:
            append(step as NodeAddedDelta);
            break;
        case DeltaType.Insert:
            insert(step as NodeInsertedDelta);
            break;
        case DeltaType.TextModified:
            textModified(step as TextModifiedDelta);
            break;
        case DeltaType.Remove:
            remove(step as NodeRemovedDelta);
            break;
        case DeltaType.EditAttribute:
            editAttribute(step as AttributeEditedDelta);
            break;
        case DeltaType.RemoveAttribute:
            removeAttribute(step as AttributeRemovedDelta);
            break;
        case DeltaType.Focus:
            focus(step as FocusDelta);
            break;
        case DeltaType.SetId:
            setId(step as SetIdDelta);
            break;
        case DeltaType.SetValue:
            setValue(step as SetValueDelta);
            break;
        case DeltaType.SubmitJS:
            submitJS(step as SubmitJsDelta);
            break;
        case DeltaType.SetChecked:
            setChecked(step as SetCheckedDelta);
            break;
        case DeltaType.ClearChildren:
            clearChildren(step as ClearChildrenDelta);
            break;
        case DeltaType.Replace:
            replaceLocation(step as ReplaceDelta);
            break;
        case DeltaType.ServerEvents:
            listenServerEvents();
            break;
        case DeltaType.SwapChildren:
            swapChildren(step as SwapChildrenDelta);
            break;
        case DeltaType.Subscribe:
            subscribe(step as SubscribeDelta);
            break;
        case DeltaType.Unsubscribe:
            unsubscribe(step as UnsubscribeDelta);
            break;

        default:
            console.log("Error processing event response. Unknown step type: " + step.Type);
    }
}

function append(delta: NodeAddedDelta): void {
    let el = document.getElementById(delta.ParentId);
    let children = createNodes(delta.Node);
    appendChildren(el, children);
}

function appendChildren(el: Element, children: Node[]): void {
    for (let child of children) {
        el.appendChild(child);
    }
}

function insert(delta: NodeInsertedDelta): void {
    let el = document.getElementById(delta.ParentElementId);
    let children = createNodes(delta.ContentNode);
    if (delta.Index < el.childNodes.length) {
        let before = el.childNodes[delta.Index];
        insertBeforeChildren(el, before, children);
    } else {
        appendChildren(el, children);
    }
}

function insertBeforeChildren(el: Element, before: ChildNode, children: Node[]): void {
    for (let child of children) {
        el.insertBefore(child, before);
        before = child.nextSibling;
    }
}

function createNodes(node: ContentNode): Node[] {
    let list: Node[] = [];
    pushNodes(node, list);
    return list;
}

function pushNodes(node: ContentNode, list: Node[]): void {
    if (node.Type == ContentNodeType.Text) {
        list.push(createTextNode(node as ContentTextNode));
    } else if (node.Type == ContentNodeType.Element) {
        list.push(createElementNode(node as ContentElementNode));
    } else if (node.Type == ContentNodeType.Array) {
        pushArrayNodes(node as ContentArrayNode, list);
    } else {
        console.log("Error processing event response. Unknown content type: " + node.Type);
        document.createTextNode("");
    }
}

function pushArrayNodes(node: ContentArrayNode, list: Node[]): void {
    for (let index = 0; index < node.Nodes.length; index++) {
        let item = node.Nodes[index];
        pushNodes(item, list);
    }
}

function createTextNode(node: ContentTextNode): Node {
    let div = document.createElement("div");
    div.innerHTML = node.Data;
    return document.createTextNode(div.innerText);
}

function createElementNode(node: ContentElementNode): Element {
    let child = createRootNode(node);
    for (var attribute of node.Attributes) {
        setAttribute(child, attribute.Attribute, attribute.Value);
    }
    for (var branch of node.Children) {
        let nodes = createNodes(branch);
        for (let node of nodes) {
            child.appendChild(node);
        }
    }
    return child;
}

function setAttribute(child: Element, attribute: string, value: string): void {
    if (!value) {
        value = "";
    }
    if (attribute == "value" && (child instanceof HTMLInputElement
        || child instanceof HTMLSelectElement
        || child instanceof HTMLTextAreaElement)) {
        child.value = value;
        return;
    }
    if (attribute == "checked" && child instanceof HTMLInputElement) {
        child.checked = true;
        return;
    }
    child.setAttribute(attribute, value);
}

function createRootNode(node: ContentElementNode): Element {
    if (node.NS) {
        return document.createElementNS(node.NS, node.TagName);
    } else {
        return document.createElement(node.TagName);
    }
}

function textModified(delta: TextModifiedDelta): void {
    let el = document.getElementById(delta.ParentElementId);
    let child = el.childNodes[delta.ChildNodeIndex];
    child.textContent = delta.Text;
}

function remove(delta: NodeRemovedDelta): void {
    let parent = document.getElementById(delta.ParentId);
    let child = parent.childNodes[delta.ChildIndex];
    child.remove();
}

function editAttribute(delta: AttributeEditedDelta): void {
    let el = document.getElementById(delta.ElementId);
    if (el.tagName == "OPTION" && delta.Attribute == "selected") {
        let option = el as HTMLOptionElement;
        option.selected = true;
    } else {
        el.setAttribute(delta.Attribute, delta.Value);
    }
}

function removeAttribute(delta: AttributeRemovedDelta): void {
    let el = document.getElementById(delta.ElementId);
    if (el.tagName == "OPTION" && delta.Attribute == "selected") {
        let option = el as HTMLOptionElement;
        option.selected = false;
    } else {
        el.removeAttribute(delta.Attribute);
    }
}

function focus(delta: FocusDelta): void {
    let el = document.getElementById(delta.ElementId);
    el.focus();
}

function setId(delta: SetIdDelta): void {
    var el = resolveElement(delta.Locator);
    el.id = delta.NewId;
}

function resolveElement(locator: ElementLocator): HTMLElement {
    let el = document.getElementById(locator.StartingId);
    for (let index = locator.Steps.length - 1; index >= 0; index--) {
        let step = locator.Steps[index];
        el = el.children[step] as HTMLElement;
    }
    return el;
}

function setValue(delta: SetValueDelta): void {
    let input = document.getElementById(delta.ElementId) as HTMLInputElement;
    input.value = delta.Value;
}

function submitJS(delta: SubmitJsDelta): void {
    try {
        eval(delta.Code);
    } catch (e) {
        console.log((<Error>e).message);
    }
}

function setChecked(delta: SetCheckedDelta): void {
    let input = document.getElementById(delta.ElementId) as HTMLInputElement;
    input.checked = delta.Checked;
}

function clearChildren(delta: ClearChildrenDelta): void {
    let parent = document.getElementById(delta.ElementId);
    while (parent.lastChild) {
        parent.removeChild(parent.lastChild);
    }
}

function replaceLocation(delta: ReplaceDelta): void {
    location.replace(delta.Location);
}

function swapChildren(step: SwapChildrenDelta): void {
    let el = document.getElementById(step.ParentId);
    let node1 = el.childNodes[step.Index1];
    let node2 = el.childNodes[step.Index2];
    swapDom(node1, node2);
}

function swapDom(obj1: Node, obj2: Node): void {
    let temp = document.createElement("div");
    obj1.parentNode.insertBefore(temp, obj1);
    obj2.parentNode.insertBefore(obj1, obj2);
    temp.parentNode.insertBefore(obj2, temp);
    temp.parentNode.removeChild(temp);
}

function subscribe(step: SubscribeDelta): void {
    let element = document.getElementById(step.ElementId);
    let handler = buildHandler(element, step);
    addElementEvent(element, step.Settings.EventName, handler);
}

function buildHandler(element: Element, step: SubscribeDelta): EventListener {
    if (step.DebounceInterval) {
        return buildDebouncedHandler(element, step);
    } else {
        return buildRegularHandler(element, step);
    }
}

function buildDebouncedHandler(element: Element, step: SubscribeDelta): EventListener {
    let handler = function (_ev: Event): void {
        plug(element, step.Settings);
    };
    return debounce(handler, step.DebounceInterval);
}

function buildRegularHandler(element: Element, step: SubscribeDelta): EventListener {
    return function (ev: Event): void {
        plugEvent(element, ev, step.Settings);
    };
}

function unsubscribe(step: UnsubscribeDelta): void {
    let element = document.getElementById(step.ElementId);
    removeElementEvent(element, step.EventName);
}

/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace LaraUI {

    export function processResult(steps: BaseDelta[]): void {
        for (var step of steps) {
            processStep(step);
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
            default:
                console.log("Error processing event response. Unknown step type: " + step.Type);
        }
    }

    function append(delta: NodeAddedDelta): void {
        let el = document.getElementById(delta.ParentId);
        let child = createNode(delta.NodeAdded);
        el.appendChild(child);
    }

    function insert(delta: NodeInsertedDelta): void {
        let el = document.getElementById(delta.ParentElementId);
        let child = createNode(delta.ContentNode);
        if (delta.Index < el.childNodes.length) {
            let before = el.childNodes[delta.Index];
            el.insertBefore(child, before);
        } else {
            el.appendChild(child);
        }
    }

    function createNode(node: ContentNode): Node {
        if (node.Type == ContentNodeType.Text) {
            return createTextNode(node as ContentTextNode);
        } else if (node.Type == ContentNodeType.Element) {
            return createElementNode(node as ContentElementNode);
        } else {
            console.log("Error processing event response. Unknown content type: " + node.Type);
            document.createTextNode("");
        }
    }

    function createTextNode(node: ContentTextNode): Node {
        return document.createTextNode(node.Data);
    }

    function createElementNode(node: ContentElementNode): Node {
        let child = document.createElement(node.TagName);
        for (var attribute of node.Attributes) {
            child.setAttribute(attribute.Attribute, attribute.Value);
        }
        for (var branch of node.Children) {
            child.appendChild(createNode(branch));
        }
        return child;
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
        el.setAttribute(delta.Attribute, delta.Value);
    }

    function removeAttribute(delta: AttributeRemovedDelta): void {
        let el = document.getElementById(delta.ElementId);
        el.removeAttribute(delta.Attribute);
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
        for (let index = locator.Steps.length; index >= 0; index--) {
            let step = locator.Steps[index];
            el = el.childNodes[step] as HTMLElement;
        }
        return el;
    }

    function setValue(delta: SetValueDelta): void {
        let input = document.getElementById(delta.ElementId) as HTMLInputElement;
        input.value = delta.Value;
    }
}
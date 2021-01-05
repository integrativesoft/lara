/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

import {
  ContentArrayNode,
  ContentElementNode,
  ContentNode,
  ContentNodeType,
  ContentPlaceholder,
  ContentTextNode
} from "./ContentInterfaces"
import * as Delta from "./DeltaInterfaces"
import { listenServerEvents } from "./index"
import { subscribe, unsubscribe } from "./RegisteredEvents"

export function processResult(steps: Delta.BaseDelta[]): void {
  for (const step of steps) {
    if (!processStepCatch(step)) {
      return
    }
  }
}

function processStepCatch(step: Delta.BaseDelta): boolean {
  try {
    processStep(step)
    return true
  } catch (Error) {
    // eslint-disable-next-line no-console
    console.log("Error processing step:")
    // eslint-disable-next-line no-console
    console.log(Error)
    // eslint-disable-next-line no-console
    console.log(step)
    return false
  }
}

function processStep(step: Delta.BaseDelta): void {
  switch (step.Type) {
    case Delta.DeltaType.Append:
      append(step as Delta.NodeAddedDelta)
      break
    case Delta.DeltaType.Insert:
      insert(step as Delta.NodeInsertedDelta)
      break
    case Delta.DeltaType.TextModified:
      textModified(step as Delta.TextModifiedDelta)
      break
    case Delta.DeltaType.Remove:
      remove(step as Delta.NodeRemovedDelta)
      break
    case Delta.DeltaType.EditAttribute:
      editAttribute(step as Delta.AttributeEditedDelta)
      break
    case Delta.DeltaType.RemoveAttribute:
      removeAttribute(step as Delta.AttributeRemovedDelta)
      break
    case Delta.DeltaType.Focus:
      focus(step as Delta.FocusDelta)
      break
    case Delta.DeltaType.SetId:
      setId(step as Delta.SetIdDelta)
      break
    case Delta.DeltaType.SetValue:
      setValue(step as Delta.SetValueDelta)
      break
    case Delta.DeltaType.SubmitJS:
      submitJS(step as Delta.SubmitJsDelta)
      break
    case Delta.DeltaType.SetChecked:
      setChecked(step as Delta.SetCheckedDelta)
      break
    case Delta.DeltaType.ClearChildren:
      clearChildren(step as Delta.ClearChildrenDelta)
      break
    case Delta.DeltaType.Replace:
      replaceLocation(step as Delta.ReplaceDelta)
      break
    case Delta.DeltaType.ServerEvents:
      listenServerEvents()
      break
    case Delta.DeltaType.SwapChildren:
      swapChildren(step as Delta.SwapChildrenDelta)
      break
    case Delta.DeltaType.Subscribe:
      subscribe(step as Delta.SubscribeDelta)
      break
    case Delta.DeltaType.Unsubscribe:
      unsubscribe(step as Delta.UnsubscribeDelta)
      break
    case Delta.DeltaType.RemoveElementId:
      removeElementId(step as Delta.RemoveElement)
      break
    case Delta.DeltaType.Render:
      render(step as Delta.RenderDelta)
      break
    case Delta.DeltaType.UnRender:
      unRender(step as Delta.UnRenderDelta)
      break
    default:
      // eslint-disable-next-line no-console
      console.log(
        "Error processing event response. Unknown step type: " + step.Type
      )
  }
}

function append(delta: Delta.NodeAddedDelta): void {
  const el = document.getElementById(delta.ParentId)
  const children = createNodes(delta.Node)
  appendChildren(el, children)
}

function appendChildren(el: Element, children: Node[]): void {
  for (const child of children) {
    el.appendChild(child)
  }
}

function insert(delta: Delta.NodeInsertedDelta): void {
  const el = document.getElementById(delta.ParentElementId)
  const children = createNodes(delta.ContentNode)
  if (delta.Index < el.childNodes.length) {
    const before = el.childNodes[delta.Index]
    insertBeforeChildren(el, before, children)
  } else {
    appendChildren(el, children)
  }
}

function render(delta: Delta.RenderDelta): void {
  const stub = locateNode(delta.Locator)
  const elements = createNodes(delta.Node)
  if (elements.length == 0) {
    stub.remove()
    return
  }
  let last = elements.pop()
  const parent = stub.parentElement
  parent.replaceChild(last, stub)
  while (elements.length) {
    const pop = elements.pop()
    parent.insertBefore(pop, last)
    last = pop
  }
}

function insertBeforeChildren(
  el: Element,
  before: ChildNode,
  children: Node[]
): void {
  for (const child of children) {
    el.insertBefore(child, before)
    before = child.nextSibling
  }
}

function createNodes(node: ContentNode): Node[] {
  const list: Node[] = []
  pushNodes(node, list)
  return list
}

function pushNodes(node: ContentNode, list: Node[]): void {
  if (node.Type == ContentNodeType.Text) {
    list.push(createTextNode(node as ContentTextNode))
  } else if (node.Type == ContentNodeType.Element) {
    list.push(createElementNode(node as ContentElementNode))
  } else if (node.Type == ContentNodeType.Array) {
    pushArrayNodes(node as ContentArrayNode, list)
  } else if (node.Type == ContentNodeType.Placeholder) {
    list.push(createPlaceholder(node as ContentPlaceholder))
  } else {
    // eslint-disable-next-line no-console
    console.log(
      "Error processing event response. Unknown content type: " + node.Type
    )
    document.createTextNode("")
  }
}

function pushArrayNodes(node: ContentArrayNode, list: Node[]): void {
  for (let index = 0; index < node.Nodes.length; index++) {
    const item = node.Nodes[index]
    pushNodes(item, list)
  }
}

function createTextNode(node: ContentTextNode): Node {
  const div = document.createElement("div")
  div.innerHTML = node.Data
  return document.createTextNode(div.innerText)
}

function createElementNode(node: ContentElementNode): Element {
  const child = createRootNode(node)
  for (const attribute of node.Attributes) {
    setAttribute(child, attribute.Attribute, attribute.Value)
  }
  for (const branch of node.Children) {
    const nodes = createNodes(branch)
    for (const node of nodes) {
      child.appendChild(node)
    }
  }
  return child
}

function createPlaceholder(node: ContentPlaceholder): Element {
  const stub = document.createElement("script")
  stub.id = node.ElementId
  stub.type = "placeholder/lara"
  return stub
}

function setAttribute(child: Element, attribute: string, value: string): void {
  if (!value) {
    value = ""
  }
  if (
    attribute == "value" &&
    (child instanceof HTMLInputElement ||
      child instanceof HTMLSelectElement ||
      child instanceof HTMLTextAreaElement)
  ) {
    child.value = value
    return
  }
  if (attribute == "checked" && child instanceof HTMLInputElement) {
    child.checked = true
    return
  }
  child.setAttribute(attribute, value)
}

function createRootNode(node: ContentElementNode): Element {
  if (node.NS) {
    return document.createElementNS(node.NS, node.TagName)
  } else {
    return document.createElement(node.TagName)
  }
}

function textModified(delta: Delta.TextModifiedDelta): void {
  const el = document.getElementById(delta.ParentElementId)
  const child = el.childNodes[delta.ChildNodeIndex]
  child.textContent = delta.Text
}

function remove(delta: Delta.NodeRemovedDelta): void {
  const parent = document.getElementById(delta.ParentId)
  const child = parent.childNodes[delta.ChildIndex]
  child.remove()
}

function editAttribute(delta: Delta.AttributeEditedDelta): void {
  const el = document.getElementById(delta.ElementId)
  if (el.tagName == "OPTION" && delta.Attribute == "selected") {
    const option = el as HTMLOptionElement
    option.selected = true
  } else {
    el.setAttribute(delta.Attribute, delta.Value)
  }
}

function removeAttribute(delta: Delta.AttributeRemovedDelta): void {
  const el = document.getElementById(delta.ElementId)
  if (el.tagName == "OPTION" && delta.Attribute == "selected") {
    const option = el as HTMLOptionElement
    option.selected = false
  } else {
    el.removeAttribute(delta.Attribute)
  }
}

function focus(delta: Delta.FocusDelta): void {
  const el = document.getElementById(delta.ElementId)
  el.focus()
}

function setId(delta: Delta.SetIdDelta): void {
  const el = document.getElementById(delta.OldId)
  el.id = delta.NewId
}

function setValue(delta: Delta.SetValueDelta): void {
  const input = document.getElementById(delta.ElementId) as HTMLInputElement
  input.value = delta.Value
}

function submitJS(context: Delta.SubmitJsDelta): void {
  try {
    eval(context.Code)
  } catch (e) {
    // eslint-disable-next-line no-console
    console.log((<Error>e).message)
  }
}

function setChecked(delta: Delta.SetCheckedDelta): void {
  const input = document.getElementById(delta.ElementId) as HTMLInputElement
  input.checked = delta.Checked
}

function clearChildren(delta: Delta.ClearChildrenDelta): void {
  const parent = document.getElementById(delta.ElementId)
  while (parent.lastChild) {
    parent.removeChild(parent.lastChild)
  }
}

function replaceLocation(delta: Delta.ReplaceDelta): void {
  location.replace(delta.Location)
}

function swapChildren(step: Delta.SwapChildrenDelta): void {
  const el = document.getElementById(step.ParentId)
  const node1 = el.childNodes[step.Index1]
  const node2 = el.childNodes[step.Index2]
  swapDom(node1, node2)
}

function swapDom(obj1: Node, obj2: Node): void {
  const temp = document.createElement("div")
  obj1.parentNode.insertBefore(temp, obj1)
  obj2.parentNode.insertBefore(obj1, obj2)
  temp.parentNode.insertBefore(obj2, temp)
  temp.parentNode.removeChild(temp)
}

function removeElementId(delta: Delta.RemoveElement): void {
  const element = document.getElementById(delta.ElementId)
  element.remove()
}

function unRender(delta: Delta.UnRenderDelta): void {
  const node = locateNode(delta.Locator)
  const stub = document.createElement("script")
  if (node instanceof Element) {
    stub.id = node.id
  }
  stub.type = "placeholder/lara"
  node.parentElement.replaceChild(stub, node)
}

function locateNode(locator: Delta.NodeLocator): ChildNode {
  const element = document.getElementById(locator.StartingId)
  const index = locator.ChildIndex
  if (index == 0 || index > 0) {
    return element.childNodes[index]
  }
  return element
}

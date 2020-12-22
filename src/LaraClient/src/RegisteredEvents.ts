/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

import { SubscribeDelta, UnsubscribeDelta } from "./DeltaInterfaces"
import { plug, plugEvent, getTargetId } from "./index"
import { debounce } from "debounce"
const registered = new Map<string, EventListener>()

function addElementEvent(
  element: EventTarget,
  eventName: string,
  handler: EventListener
): void {
  const id = getTargetId(element)
  const key = getKey(id, eventName)
  registered.set(key, handler)
  element.addEventListener(eventName, handler)
}

function removeElementEvent(element: EventTarget, eventName: string): void {
  const id = getTargetId(element)
  const key = getKey(id, eventName)
  const handler = registered.get(key)
  registered.delete(key)
  element.removeEventListener(eventName, handler)
}

function getKey(id: string, eventName: string) {
  return id + " " + eventName
}

export function subscribe(step: SubscribeDelta): void {
  const element = getEventTarget(step.ElementId)
  let handler = buildHandler(element, step)
  if (step.EvalFilter) {
    handler = addEvalFilter(handler, step.EvalFilter)
  }
  addElementEvent(element, step.Settings.EventName, handler)
}

function getEventTarget(id: string): EventTarget {
  if (id) {
    return document.getElementById(id)
  } else {
    return document
  }
}

function buildHandler(
  element: EventTarget,
  step: SubscribeDelta
): EventListener {
  if (step.DebounceInterval) {
    return buildDebouncedHandler(element, step)
  } else {
    return buildRegularHandler(element, step)
  }
}

function buildDebouncedHandler(
  element: EventTarget,
  step: SubscribeDelta
): EventListener {
  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  const handler = function (_ev: Event): void {
    plug(element, step.Settings)
  }
  return debounce(handler, step.DebounceInterval)
}

function buildRegularHandler(
  element: EventTarget,
  step: SubscribeDelta
): EventListener {
  return function (ev: Event): void {
    plugEvent(element, ev, step.Settings)
  }
}

function addEvalFilter(handler: EventListener, filter: string): EventListener {
  return function (event: Event): void {
    let run = false
    try {
      const result = eval(filter)
      if (result) {
        run = true
      }
      // eslint-disable-next-line no-empty
    } catch {}
    if (run) {
      handler(event)
    }
  }
}

export function unsubscribe(step: UnsubscribeDelta): void {
  const element = getEventTarget(step.ElementId)
  removeElementEvent(element, step.EventName)
}

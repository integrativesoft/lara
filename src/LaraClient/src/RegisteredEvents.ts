/*
Copyright (c) 2019 Integrative Software LLC
Created: 10/2019
Author: Pablo Carbonell
*/

let registered = new Map<string, EventListener>();

export function addElementEvent(element: Element, eventName: string, handler: EventListener): void {
    let key = getKey(element.id, eventName);
    registered.set(key, handler);
    element.addEventListener(eventName, handler);
}

export function removeElementEvent(element: Element, eventName: string): void {
    let key = getKey(element.id, eventName);
    let handler = registered.get(key);
    registered.delete(key);
    element.removeEventListener(eventName, handler);
}

function getKey(id: string, eventName: string) {
    return id + ' ' + eventName;
}

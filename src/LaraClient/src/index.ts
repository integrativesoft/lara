/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

/*
Lara is a server-side DOM rendering library for C#.
This file is the client runtime for Lara.
https://laraui.com
*/

import { clean } from "./Initializer";
import { ClientEventMessage, collectValues } from "./InputCollector";
import { block, unblock } from "./Blocker";
import { EventResult, EventResultType } from "./DeltaInterfaces";
import { processResult } from "./Worker";

let documentId: string;

export function initialize(id: string): void {
    documentId = id;
    window.addEventListener("unload", terminate, false);
    clean(document);
    let json = document.head.getAttribute('data-lara-initialdelta');
    if (json) {
        let result = JSON.parse(json);
        processEventResult(result);
    }
}
    
function terminate(): void {
    let url = "/_discard?doc=" + documentId;
    navigator.sendBeacon(url);
}

export interface PlugOptions {
    EventName: string;
    Block?: boolean;
    BlockElementId?: string;
    BlockHTML?: string;
    BlockShownId?: string;
    ExtraData?: string;
    LongRunning?: boolean;
}

export class EventParameters {
    DocumentId: string;
    ElementId: string;
    EventName: string;
    Message: ClientEventMessage;
}

export function plug(el: Element, event: Event): void {
    event.stopPropagation();
    let eventName = event.type;
    let attribute = "data-lara-event-" + eventName;
    let json = el.getAttribute(attribute);
    if (json) {
        let options = JSON.parse(json) as PlugOptions;
        options.EventName = eventName;
        plugOptions(el, options);
    }
}

function plugOptions(el: Element, plug: PlugOptions): void {
    if (plug.LongRunning) {
        plugWebSocket(el, plug);
    } else {
        plugAjax(el, plug);
    }
}

function plugWebSocket(el: Element, plug: PlugOptions): void {
    block(plug);
    let url = getSocketUrl('/_event');
    let socket = new WebSocket(url);
    socket.onopen = function (_event) {
        socket.onmessage = function (e1) {
            onSocketMessage(e1.data);
        };
        socket.onclose = function (_e2) {
            unblock(plug);
        }
        let json = buildEventParameters(el, plug);
        socket.send(json);
    };
    socket.onerror = function (_event) {
        console.log('Error on websocket communication. Reloading.');
        location.reload();
    }
}

function getSocketUrl(name: string): string {
    var url: string;
    if (location.protocol == "https:") {
        url = "wss://";
    } else {
        url = "ws://";
    }
    return url + window.location.host + name;
}

function buildEventParameters(el: Element, plug: PlugOptions): string {
    let params = new EventParameters();
    params.DocumentId = documentId;
    params.ElementId = el.id;
    params.EventName = plug.EventName;
    params.Message = collectValues();
    params.Message.ExtraData = plug.ExtraData;
    return JSON.stringify(params);
}

function onSocketMessage(json: string): void {
    let result = JSON.parse(json) as EventResult;
    processEventResult(result);
}

function plugAjax(el: Element, plug: PlugOptions): void {
    block(plug);
    let url = getEventUrl(el, plug.EventName);
    let ajax = new XMLHttpRequest();
    ajax.onreadystatechange = function () {
        if (this.readyState == 4) {
            processAjax(this);
            unblock(plug);
        }
    };
    let message = collectValues();
    message.ExtraData = plug.ExtraData;
    ajax.open("POST", url, true);
    if (message.isEmpty()) {
        ajax.send();
    } else {
        ajax.send(JSON.stringify(message));
    }
}

export interface MessageOptions {
    key: string;
    data?: string;
    block?: boolean;
    blockElementId?: string;
    blockHtml?: string;
    blockShowElementId?: string;
    longRunning?: boolean;
}

export function sendMessage(options: MessageOptions): void {
    let params: PlugOptions = {
        EventName: "_" + options.key,
        Block: options.block,
        BlockElementId: options.blockHtml,
        BlockHTML: options.blockHtml,
        BlockShownId: options.blockShowElementId,
        ExtraData: options.data,
        LongRunning: options.longRunning
    };
    plugOptions(document.head, params);
}

function processAjax(ajax: XMLHttpRequest): void {
    if (ajax.status == 200) {
        processAjaxResult(ajax);
    } else {
        processAjaxError(ajax);
    }
}

function getEventUrl(el: Element, eventName: string): string {
    return "/_event?doc=" + documentId
        + "&el=" + el.id
        + "&ev=" + eventName;
}

function processAjaxResult(ajax: XMLHttpRequest): void {
    let result = JSON.parse(ajax.responseText) as EventResult;
    processEventResult(result);
}

function processEventResult(result: EventResult): void {
    if (result.ResultType == EventResultType.Success) {
        if (result.List) {
            processResult(result.List);
        }
    } else if (result.ResultType == EventResultType.NoSession) {
        location.reload();
    }
}

function processAjaxError(ajax: XMLHttpRequest): void {
    if (ajax.responseText) {
        document.write(ajax.responseText);
    } else {
        console.log("Internal Server Error on AJAX call. Detailed exception information on the client is turned off.")
    }
}

export function listenServerEvents(): void {
    plugWebSocket(document.head, {
        EventName: '_server_event',
        Block: false,
        ExtraData: '',
        LongRunning: true
    });
}

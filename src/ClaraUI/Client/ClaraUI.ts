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

namespace ClaraUI {

    let documentId: string;

    export function initialize(id: string): void {
        documentId = id;
        window.addEventListener("unload", terminate, false);
    }
    
    function terminate(): void {
        let url = "/_discard?doc=" + documentId;
        navigator.sendBeacon(url);
    }

    export function plug(el: Element, eventName: string): void {
        let url = getEventUrl(el, eventName);
        sendAjax(url);
    }

    function getEventUrl(el: Element, eventName: string): string {
        return "/_event?doc=" + documentId
            + "&el=" + el.id
            + "&ev=" + eventName;
    }

    function sendAjax(url: string): void {
        let ajax = new XMLHttpRequest();
        ajax.onreadystatechange = function () {
            if (this.readyState == 4) {
                if (this.status == 200) {
                    processAjaxResult(this);
                } else {
                    console.log("Error in Ajax call: " + this.statusText);
                }
            }
        };
        let message = collectValues();
        ajax.open("POST", url, true);
        if (message.isEmpty()) {
            ajax.send();
        } else {
            ajax.send(JSON.stringify(message));
        }
    }

    function processAjaxResult(ajax: XMLHttpRequest): void {
        let result = JSON.parse(ajax.responseText) as EventResult;
        if (result.List) {
            processResult(result.List);
        }
    }
}

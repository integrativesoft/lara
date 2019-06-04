/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

/*
Lara is a server-side DOM rendering library for C#
This file is the client runtime for Lara.
https://laraui.com
*/

namespace LaraUI {

    let documentId: string;

    export function initialize(id: string): void {
        documentId = id;
        window.addEventListener("unload", terminate, false);
        clean(document);
    }
    
    function terminate(): void {
        let url = "/_discard?doc=" + documentId;
        navigator.sendBeacon(url);
    }

    export interface PlugOptions {
        EventName: string;
        Block: boolean;
        BlockElementId: string;
        BlockHTML: string;
    }

    export function plug(el: Element, plug: PlugOptions): void {
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
        ajax.open("POST", url, true);
        if (message.isEmpty()) {
            ajax.send();
        } else {
            ajax.send(JSON.stringify(message));
        }
    }

    function processAjax(ajax: XMLHttpRequest): void {
        try {
            if (ajax.status == 200) {
                processAjaxResult(ajax);
            } else {
                processAjaxError(ajax);
            }
        } catch (error) {
            console.log(error.message);
        }
    }

    function getEventUrl(el: Element, eventName: string): string {
        return "/_event?doc=" + documentId
            + "&el=" + el.id
            + "&ev=" + eventName;
    }

    function processAjaxResult(ajax: XMLHttpRequest): void {
        let result = JSON.parse(ajax.responseText) as EventResult;
        if (result.List) {
            processResult(result.List);
        }
    }

    function processAjaxError(ajax: XMLHttpRequest): void {
        if (ajax.responseText) {
            document.write(ajax.responseText);
        } else {
            console.log("Internal Server Error on AJAX call. Detailed exception information on the client is turned off.")
        }
    }

    function clean(node: Node): void {
        for (let n = 0; n < node.childNodes.length; n++) {
            let child = node.childNodes[n];
            if
                (
                child.nodeType === 8
                ||
                (child.nodeType === 3 && !/\S/.test(child.nodeValue))
            ) {
                node.removeChild(child);
                n--;
            }
            else if (child.nodeType === 1) {
                clean(child);
            }
        }
    }
}

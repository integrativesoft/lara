/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

import { PlugOptions } from "./index";

export class ElementEventValue {
    ElementId: string;
    Value: string;
    Checked: boolean;
}

export class ClientEventMessage {
    Values: ElementEventValue[];
    ExtraData: string;
    isEmpty(): boolean {
        return this.Values.length == 0
            && !this.ExtraData;
    }
}

export function collectValues(plug: PlugOptions): FormData {
    let data = new FormData();
    var message = collectMessage(plug);
    let fileCount = collectFiles(plug, data);
    if (message.isEmpty() && fileCount == 0) {
        return undefined;
    } else {
        data.append('_message', JSON.stringify(message));
        return data;
    }
}

export function collectMessage(plug: PlugOptions): ClientEventMessage {
    var message = new ClientEventMessage();
    message.Values = [];
    message.ExtraData = plug.ExtraData;
    collectType("input", message, collectInput);
    collectType("textarea", message, collectSimpleValue);
    collectType("button", message, collectSimpleValue);
    collectType("select", message, collectSimpleValue);
    collectType("option", message, collectOption);
    return message;
}

function collectType(tagName: string,
    message: ClientEventMessage,
    processor: (el: Element, m: ElementEventValue) => void) {
    let list = document.getElementsByTagName(tagName);
    for (let index = 0; index < list.length; index++) {
        let el = list[index];
        if (el.id) {
            let entry = new ElementEventValue();
            entry.ElementId = el.id;
            processor(el, entry);
            message.Values.push(entry);
        }
    }
}

function collectInput(el: Element, entry: ElementEventValue): void {
    let input = el as HTMLInputElement;
    entry.Value = getValue(input);
    entry.Checked = input.checked;
}

function collectSimpleValue(el: Element, entry: ElementEventValue): void {
    entry.Value = getValue(el);
}

function collectOption(el: Element, entry: ElementEventValue): void {
    let option = el as HTMLOptionElement;
    entry.Checked = option.selected;
}

function getValue(el: Element): string {
    if (el.hasAttribute('data-lara-value')) {
        return el.getAttribute('data-lara-value');
    } else if ('value' in el) {
        return el['value'];
    } else {
        return '';
    }
}

function collectFiles(plug: PlugOptions, data: FormData): number {
    if (!plug.UploadFiles) {
        return 0;
    }
    let count = 0;
    let list = document.getElementsByTagName("input");
    for (let index = 0; index < list.length; index++) {
        let input = list[index] as HTMLInputElement;
        count += collectFilesInput(input, data);
    }
    return count;
}

function collectFilesInput(input: HTMLInputElement, data: FormData): number {
    if (!input.id || input.type != 'file') {
        return 0;
    }
    let files = input.files;
    let key = 'file/' + input.id;
    for (let index = 0; index < files.length; index++) {
        let file = files[index];
        data.append(key, file, file.name);
    }
    return files.length;
}
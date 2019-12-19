/*
Copyright (c) 2019 Integrative Software LLC
Created: 12/2019
Author: Pablo Carbonell
*/

import { PlugOptions } from "./index";
import { ClientEventMessage } from "./InputCollector";

export class EventParameters {
    DocumentId: string;
    ElementId: string;
    EventName: string;
    EventNumber: number;
    Message: ClientEventMessage;
}

export class SocketEventParameters extends EventParameters {
    SocketFiles: FormFileCollection;
}

class FormFileCollection {
    InnerList: FormFile[];

    constructor() {
        this.InnerList = [];
    }
}

class FormFile {
    ContentType: string;
    ContentDisposition: string;
    Name: string;
    FileName: string;
    Content: string;
}

export async function loadFiles(plug: PlugOptions): Promise<FormFileCollection> {
    let result = new FormFileCollection();
    if (!plug.UploadFiles) {
        return result;
    }
    let list = document.getElementsByTagName("input");
    for (let index = 0; index < list.length; index++) {
        let input = list[index] as HTMLInputElement;
        await collectFilesInput(input, result);
    }
    return result;
}

async function collectFilesInput(input: HTMLInputElement, data: FormFileCollection): Promise<void> {
    if (!input.id || input.type != 'file') {
        return;
    }
    let files = input.files;
    let key = 'file/' + input.id;
    for (let index = 0; index < files.length; index++) {
        let file = files[index];
        let copy = await copyFile(file, key);
        copy.Name = key;
        data.InnerList.push(copy);
    }
}

async function copyFile(file: File, name: string): Promise<FormFile> {
    let bytes = await readFile(file);
    let copy = new FormFile();
    copy.ContentType = file.type;
    copy.Content = bufferToBase64(bytes);
    copy.Name = name;
    copy.FileName = file.name;
    return copy;
}

function bufferToBase64(buffer: ArrayBuffer): string {
    let binary = '';
    let bytes = new Uint8Array(buffer);
    let len = bytes.byteLength;
    for (let index = 0; index < len; index++) {
        binary += String.fromCharCode(bytes[index]);
    }
    return window.btoa(binary);
}

async function readFile(file: File): Promise<ArrayBuffer> {
    let reader = new FileReader();
    reader.readAsArrayBuffer(file);
    return new Promise<ArrayBuffer>((resolve, reject) => {
        reader.onerror = () => {
            reject('Error reading input file.');
        };
        reader.onload = function () {
            resolve(reader.result as ArrayBuffer);
        }
    });
}
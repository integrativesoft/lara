/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace ClaraUI {

    export class ClientEventValue {
        ElementId: string;
        Value: string;
    }

    export class ClientEventMessage {
        Values: ClientEventValue[];

        isEmpty(): boolean {
            return this.Values.length == 0;
        }
    }

    export function collectValues(): ClientEventMessage {
        var message = new ClientEventMessage();
        message.Values = [];
        collectElementType("input", message);
        collectElementType("textarea", message);
        collectElementType("select", message);
        collectElementType("button", message);
        return message;
    }

    function collectElementType(tagName: string, message: ClientEventMessage) {
        let list = document.getElementsByTagName(tagName);
        for (let index = 0; index < list.length; index++) {
            let input = list[index] as HTMLInputElement;
            if (input.id) {
                var value = new ClientEventValue();
                value.ElementId = input.id;
                value.Value = input.value;
                message.Values.push(value);
            }
        }
    }

}
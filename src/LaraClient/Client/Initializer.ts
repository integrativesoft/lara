/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

namespace LaraUI {

    export function clean(node: Node): void {
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
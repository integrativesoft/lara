/*
Copyright (c) 2019 Integrative Software LLC
Created: 6/2019
Author: Pablo Carbonell
*/

namespace LaraUI {

    export function block(plug: PlugOptions): void {
        if (plug.Block) {
            let target = resolveTarget(plug);
            let params = buildParameters(plug);
            if (target) {
                ($(target) as any).block(params);
            } else {
                $.blockUI(params);
            }
        }
    }

    export function unblock(plug: PlugOptions): void {
        if (plug.Block) {
            let target = resolveTarget(plug);
            if (target) {
                ($(target) as any).unblock();
            } else {
                $.unblockUI();
            }
        }
    }

    function buildParameters(plug: PlugOptions): blockUISettings {
        let result: blockUISettings = {};
        if (plug.BlockHTML == "") {
            result.message = null;
        } else if (plug.BlockHTML) {
            result.message = plug.BlockHTML;
        }
        result.baseZ = 2000;
        return result;
    }

    function resolveTarget(plug: PlugOptions): Element {
        if (plug.BlockElementId) {
            let el = document.getElementById(plug.BlockElementId);
            if (el) {
                return el;
            }
        }
        return null;
    }

}
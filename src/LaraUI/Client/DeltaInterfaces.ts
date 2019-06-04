/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace LaraUI {

    export enum DeltaType {
        Append = 1,
        Insert = 2,
        TextModified = 3,
        Remove = 4,
        EditAttribute = 5,
        RemoveAttribute = 6,
        Focus = 7,
        SetId = 8,
        SetValue = 9,
        SubmitJS = 10,
        SetChecked = 11,
        ClearChildren = 12
    }

    export interface BaseDelta {
        Type: DeltaType;
    }

    export interface EventResult {
        List: BaseDelta[];
    }

    export interface NodeAddedDelta extends BaseDelta {
        ParentId: string;
        Node: ContentNode;
    }

    export interface NodeInsertedDelta extends BaseDelta {
        ParentElementId: string;
        Index: number;
        ContentNode: ContentNode;
    }

    export interface TextModifiedDelta extends BaseDelta {
        ParentElementId: string;
        ChildNodeIndex: number;
        Text: string;
    }

    export interface NodeRemovedDelta extends BaseDelta {
        ParentId: string;
        ChildIndex: number;
    }

    export interface AttributeEditedDelta extends BaseDelta {
        ElementId: string;
        Attribute: string;
        Value: string;
    }

    export interface AttributeRemovedDelta extends BaseDelta {
        ElementId: string;
        Attribute: string;
    }

    export interface ElementLocator {
        StartingId: string;
        Steps: number[];
    }

    export interface FocusDelta extends BaseDelta {
        ElementId: string;
    }

    export interface SetIdDelta extends BaseDelta {
        Locator: ElementLocator;
        NewId: string;
    }

    export interface SetValueDelta extends BaseDelta {
        ElementId: string;
        Value: string;
    }

    export interface SubmitJsDelta extends BaseDelta {
        Code: string;
    }

    export interface SetCheckedDelta extends BaseDelta {
        ElementId: string;
        Checked: boolean;
    }

    export interface ClearChildrenDelta extends BaseDelta {
        ElementId: string;
    }

}
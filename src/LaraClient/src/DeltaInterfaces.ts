/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

import { ContentNode } from "./ContentInterfaces";
import { PlugOptions } from "./index";

export enum EventResultType {
    Success = 0,
    NoSession = 1,
    NoElement = 2
}

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
    ClearChildren = 12,
    Replace = 13,
    ServerEvents = 14,
    SwapChildren = 15,
    Subscribe = 16,
    Unsubscribe = 17
}

export interface BaseDelta {
    Type: DeltaType;
}

export interface EventResult {
    ResultType: EventResultType;
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
    Payload?: string;
}

export interface SetCheckedDelta extends BaseDelta {
    ElementId: string;
    Checked: boolean;
}

export interface ClearChildrenDelta extends BaseDelta {
    ElementId: string;
}

export interface ReplaceDelta extends BaseDelta {
    Location: string;
}

export interface SwapChildrenDelta extends BaseDelta {
    ParentId: string;
    Index1: number;
    Index2: number;
}

export interface SubscribeDelta extends BaseDelta {
    ElementId: string;
    Settings: PlugOptions;
    DebounceInterval: number;
}

export interface UnsubscribeDelta extends BaseDelta {
    ElementId: string;
    EventName: string;
}

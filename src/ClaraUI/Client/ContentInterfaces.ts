/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace ClaraUI {
    export enum ContentNodeType {
        Element = 1,
        Text = 2
    }

    export interface ContentNode {
        Type: ContentNodeType;
    }

    export interface ContentTextNode extends ContentNode {
        Data: string;
    }

    export interface ContentAttribute {
        Attribute: string;
        Value: string;
    }

    export interface ContentElementNode extends ContentNode {
        TagName: string;
        Attributes: ContentAttribute[];
        Children: ContentNode[];
    }
}
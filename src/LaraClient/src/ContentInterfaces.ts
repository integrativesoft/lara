/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

export enum ContentNodeType {
  // eslint-disable-next-line no-unused-vars
  Element = 1,
  // eslint-disable-next-line no-unused-vars
  Text = 2,
  // eslint-disable-next-line no-unused-vars
  Array = 3,
  // eslint-disable-next-line no-unused-vars
  Placeholder = 4
}

export interface ContentNode {
  Type: ContentNodeType
}

export interface ContentTextNode extends ContentNode {
  Data: string
}

export interface ContentAttribute {
  Attribute: string
  Value: string
}

export interface ContentElementNode extends ContentNode {
  TagName: string
  NS: string
  Attributes: ContentAttribute[]
  Children: ContentNode[]
}

export interface ContentArrayNode extends ContentNode {
  Nodes: ContentNode[]
}

export interface ContentPlaceholder extends ContentNode {
  ElementId: string
}

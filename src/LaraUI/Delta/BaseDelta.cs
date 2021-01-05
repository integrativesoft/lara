/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    internal enum DeltaType
    {
        Append = 1,
        Insert = 2,
        TextModified = 3,
        Remove = 4,
        EditAttribute = 5,
        RemoveAttribute = 6,
        Focus = 7,
        SetId = 8,
        SetValue = 9,
        SubmitJs = 10,
        SetChecked = 11,
        ClearChildren = 12,
        Replace = 13,
        ServerEvents = 14,
        SwapChildren = 15,
        Subscribe = 16,
        Unsubscribe = 17,
        RemoveElement = 18,
        Render = 19,
        UnRender = 20
    }

    [DataContract]
    [KnownType(typeof(NodeAddedDelta))]
    [KnownType(typeof(NodeInsertedDelta))]
    [KnownType(typeof(TextModifiedDelta))]
    [KnownType(typeof(NodeRemovedDelta))]
    [KnownType(typeof(AttributeEditedDelta))]
    [KnownType(typeof(AttributeRemovedDelta))]
    [KnownType(typeof(FocusDelta))]
    [KnownType(typeof(SetIdDelta))]
    [KnownType(typeof(SetValueDelta))]
    [KnownType(typeof(SubmitJsDelta))]
    [KnownType(typeof(SetCheckedDelta))]
    [KnownType(typeof(ClearChildrenDelta))]
    [KnownType(typeof(ReplaceDelta))]
    [KnownType(typeof(ServerEventsDelta))]
    [KnownType(typeof(SwapChildrenDelta))]
    [KnownType(typeof(SubscribeDelta))]
    [KnownType(typeof(UnsubscribeDelta))]
    [KnownType(typeof(RemoveElementDelta))]
    [KnownType(typeof(RenderDelta))]
    [KnownType(typeof(UnRenderDelta))]
    internal abstract class BaseDelta
    {
        [DataMember]
        public DeltaType Type { get; set; }

        protected BaseDelta(DeltaType type)
        {
            Type = type;
        }
    }
}

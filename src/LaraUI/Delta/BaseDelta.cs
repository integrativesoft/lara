/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    enum DeltaType
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
        SubmitJS = 10,
        SetChecked = 11,
        ClearChildren = 12
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
    abstract class BaseDelta
    {
        [DataMember]
        public DeltaType Type { get; set; }

        public BaseDelta(DeltaType type)
        {
            Type = type;
        }
    }
}

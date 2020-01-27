/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal class ServerEventsDelta : BaseDelta
    {
        public ServerEventsDelta() : base(DeltaType.ServerEvents)
        {
        }
    }
}

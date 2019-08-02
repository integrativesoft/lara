/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara.Delta
{
    [DataContract]
    class ServerEventsDelta : BaseDelta
    {
        public ServerEventsDelta() : base(DeltaType.ServerEvents)
        {
        }
    }
}

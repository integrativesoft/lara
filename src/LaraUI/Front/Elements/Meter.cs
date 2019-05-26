/*
Copyright (c) 2019 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

namespace Integrative.Lara
{
    public sealed class Meter : Element
    {
        public Meter() : base("meter")
        {
        }

        public int? High
        {
            get => GetIntAttribute("high");
            set { SetIntAttribute("high", value); }
        }

        public int? Low
        {
            get => GetIntAttribute("low");
            set { SetIntAttribute("low", value); }
        }

        public int? Max
        {
            get => GetIntAttribute("max");
            set { SetIntAttribute("max", value); }
        }

        public int? Min
        {
            get => GetIntAttribute("min");
            set { SetIntAttribute("min", value); }
        }

        public int? Optimum
        {
            get => GetIntAttribute("optimum");
            set { SetIntAttribute("optimum", value); }
        }

        public int? Value
        {
            get => GetIntAttribute("value");
            set { SetIntAttribute("value", value); }
        }
    }
}

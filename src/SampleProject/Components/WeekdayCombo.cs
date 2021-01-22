/*
Copyright (c) 2020-2021 Integrative Software LLC
Created: 12/2020
Author: Pablo Carbonell
*/

using Integrative.Lara;

namespace SampleProject.Components
{
    public class WeekdayCombo : WebComponent
    {
        private static readonly string[] _WeekDays
            = { "Monday", "Tuesday", "Wednesday", "Thursday",
            "Friday", "Saturday", "Sunday" };

        private int _weekday;
        private int Weekday { get => _weekday; set => SetProperty(ref _weekday, value); }

        public WeekdayCombo()
        {
            var combo = new HtmlSelectElement { Class = "form-control", Id = "MyWeek" };
            for (var index = 0; index < _WeekDays.Length; index++)
            {
                combo.AddOption(index.ToString(), _WeekDays[index]);
            }
            combo.Bind(this, _ => combo.Value = Weekday.ToString());
            combo.BindBack(_ => Weekday = int.Parse(combo.Value ?? ""));
            ShadowRoot.Children = new[] { combo };
        }

        public void NextDay()
            => Weekday = (Weekday + 1) % _WeekDays.Length;
    }
}

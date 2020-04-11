using System;

namespace mgWinChart.Helpers
{
    public struct RealTimeDataEntry
    {
        public DateTime Time;
        public string Name;
        public decimal? Value;

        public RealTimeDataEntry(DateTime Time, string Name, decimal? Value)
        {
            this.Time = Time;
            this.Name = Name;
            this.Value = Value;
        }
    }
}
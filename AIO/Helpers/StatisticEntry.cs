using System.Linq;
using robotManager.Helpful;

namespace AIO.Helpers {
    public class StatisticEntry {
        private readonly RingBuffer<int> HistoricalData;
        public double Average { get; private set; }
        public int Count => HistoricalData.Count;

        public StatisticEntry(int startValue) {
            HistoricalData = new RingBuffer<int>(10, true);
            Add(startValue);
        }

        public void Add(int value) {
            if(value <= 0) return;
            HistoricalData.Add(value);
            Average = HistoricalData.Average();
        }
    }
}
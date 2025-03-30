using System.Globalization;

namespace Demo2.Core
{
    internal class ExchangeInfo
    {
        public Symbol[] symbols { get; set; }
    }

    internal class Symbol
    {
        public string symbol { get; set; }
        public string pair { get; set; }
        public string contractType { get; set; }
    }

    internal class Snapshot
    {
        public string lastPrice { get; set; }
        public decimal price => decimal.Parse(lastPrice, CultureInfo.InvariantCulture);
    }
}

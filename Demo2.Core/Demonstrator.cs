using Kw.Common;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Demo2.Core
{
    public class Demonstrator(Context context, ILogger<Demonstrator> logger, IHostApplicationLifetime lifetime)
    {
        static bool ready;

        static string symbolQuarterly;
        static string symbolBiquarterly;

        static decimal keptQuaterly;
        static decimal keptBiquaterly;

        //
        // Получить символические имена для квартального и биквартального фьючерса.
        // Завершение программы при ошибке.
        //
        async Task QuerySymbols()
        {
            try
            {
                logger.Log(LogLevel.Information, "Запрос символов...");

                ExchangeInfo info = await Download<ExchangeInfo>($"https://fapi.binance.com/fapi/v1/exchangeInfo");

                symbolQuarterly = info.symbols.Single(x => x is { contractType: "CURRENT_QUARTER", pair: "BTCUSDT" }).symbol;
                symbolBiquarterly = info.symbols.Single(x => x is { contractType: "NEXT_QUARTER", pair: "BTCUSDT" }).symbol;

                logger.Log(LogLevel.Information, $"Квартальный фьючерс '{symbolQuarterly}'");
                logger.Log(LogLevel.Information, $"Биквартальный фьючерс '{symbolBiquarterly}'");

                ready = true;
            }
            catch (Exception x)
            {
                logger.Log(LogLevel.Error, "Ошибка запроса символов, останов программы", x);

                lifetime.StopApplication();
            }

        }

        async Task<T> Download<T>(string url) =>
            JsonSerializer.Deserialize<T>(
                await new HttpClient().GetStringAsync(url)
                )!;

        async Task<decimal> DownloadPrice(string symbol)
        {
            Snapshot snapshot = await Download<Snapshot>($"https://fapi.binance.com/fapi/v1/ticker/24hr?symbol={symbol}");

            if (snapshot.price == 0)
                throw new Exception();

            return snapshot.price;
        }

        decimal FallbackPrice(string symbol)
        {
            if (symbolQuarterly == symbol)
                return keptQuaterly;

            if (symbolBiquarterly == symbol)
                return keptBiquaterly;

            throw new NotReachableException();
        }

        void KeepPrice(string symbol, decimal value)
        {
            if (symbolQuarterly == symbol)
                keptQuaterly = value;

            if (symbolBiquarterly == symbol)
                keptBiquaterly = value;
        }

        //
        // Получить и сохранить цену инструмента
        // При ошибке вернуть ранее сохранённую цену
        //
        async Task<decimal> QueryPrice(string symbol)
        {
            decimal price = 0;
            try
            {
                price = await DownloadPrice(symbol);

                logger.Log(LogLevel.Information, $"Загружен '{symbol}': {price}");

                KeepPrice(symbol, price);
            }
            catch
            {
                price = FallbackPrice(symbol);
            }

            return price;
        }

        /// <remarks>
        /// Синтетический арбитраж
        /// </remarks>
        public async Task<Entry> Invoke()
        {
            if (!ready) await QuerySymbols();

            decimal quaterlyPrice = 0, biquaterlyPrice = 0;

            quaterlyPrice = await QueryPrice(symbolQuarterly);
            biquaterlyPrice = await QueryPrice(symbolBiquarterly);

            Entry entry = new();

            if (0 != quaterlyPrice && 0 != biquaterlyPrice)
            {
                decimal difference = biquaterlyPrice - quaterlyPrice;

                entry.Difference = difference;
                entry.Timestamp = DateTime.UtcNow;
                
                await Write(entry);
            }

            return entry;
        }

        public async Task Write(Entry entry)
        {
            context.Entries.Add(entry);
            await context.SaveChangesAsync();

            logger.Log(LogLevel.Information, $"Добавлено значение '{entry.Difference}");
        }

        public async Task<Entry> Read()
        {
            Entry entry = context.Entries.OrderByDescending(x => x.Timestamp).Take(1).SingleOrDefault() ?? new();

            return entry;
        }
    }
}

using System;
using System.Globalization;
using Comfort.Common;
using EFT;
using EFT.HandBook;
using EFT.InventoryLogic;

namespace SptCurrencyConverter
{
    internal static class CurrencyConverter
    {
        public static bool TryGetRubBracket(int amount, MongoID? currencyId, out string bracket)
        {
            bracket = null;

            if (Plugin.Enabled == null || !Plugin.Enabled.Value)
                return false;

            if (currencyId == null || amount <= 0)
                return false;

            if (!CurrencyUtil.TryGetCurrencyType(currencyId, out var type))
                return false;

            if (type != ECurrencyType.USD && type != ECurrencyType.EUR)
                return false;

            var rate = GetRate(type);
            if (rate <= 0)
                return false;

            var rub = Math.Round(amount * (decimal)rate, 0, MidpointRounding.AwayFromZero);
            bracket = "\u20BD" + rub.ToString("N0", CultureInfo.InvariantCulture);
            return true;
        }

        private static float GetRate(ECurrencyType type)
        {
            if (Plugin.ConversionSource != null &&
                Plugin.ConversionSource.Value == RateSource.LiveTraderRates &&
                TryGetLiveRate(type, out var liveRate))
            {
                return liveRate;
            }

            var usd = Plugin.ManualFallbackRate?.Value ?? 95f;
            return type == ECurrencyType.EUR ? usd * 1.08f : usd;
        }

        // uses for trader math and ragfair taxes.
        private static bool TryGetLiveRate(ECurrencyType type, out float rate)
        {
            rate = 0f;

            try
            {
                var handbook = Singleton<Handbook>.Instance;
                if (handbook == null)
                    return false;

                var id = type == ECurrencyType.USD ? CurrencyUtil.DOLLAR_ID : CurrencyUtil.EURO_ID;
                var price = handbook.GetBasePrice(id);
                if (price <= 0)
                    return false;

                rate = (float)price;
                return true;
            }
            catch (Exception e)
            {
                Plugin.Log?.LogWarning($"Rate lookup failed, using fallback: {e.Message}");
                return false;
            }
        }
    }
}

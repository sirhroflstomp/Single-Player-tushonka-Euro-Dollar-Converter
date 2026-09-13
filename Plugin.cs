using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using SptCurrencyConverter.Patches;

namespace SptCurrencyConverter
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        internal static ConfigEntry<bool> Enabled;
        internal static ConfigEntry<RateSource> ConversionSource;
        internal static ConfigEntry<float> ManualFallbackRate;

        private void Awake()
        {
            Log = Logger;

            Enabled = Config.Bind("General", "Enabled", true, "Show RUB equivalents next to USD/EUR prices.");

            ConversionSource = Config.Bind("Rates", "Source", RateSource.LiveTraderRates,
                "LiveTraderRates uses the game's handbook price for USD/EUR. ManualFallback uses the fixed rate below.");

            ManualFallbackRate = Config.Bind("Rates", "ManualFallbackRateUSD", 95.0f,
                "Used if live rates can't be read. EUR is 1.08x this value.");

            new TradingPricePatch().Enable();
            new OfferItemPricePatch().Enable();
            new MoneyStackValuePatch().Enable();

            Log.LogInfo($"{PluginInfo.NAME} v{PluginInfo.VERSION} loaded.");
        }
    }

    internal enum RateSource
    {
        LiveTraderRates,
        ManualFallback
    }

    internal static class PluginInfo
    {
        public const string GUID = "com.sirhroflstomp.eurodollarcurrencyconverter";
        public const string NAME = "sirhroflstomp-EuroDollarCurrencyConverter";
        public const string VERSION = "0.3.0";
    }
}

using System.Reflection;
using EFT.Trading;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;
using TMPro;
using UnityEngine;

namespace SptCurrencyConverter.Patches
{
    public class TradingPricePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(TradingItemView), nameof(TradingItemView.SetPrice));
        }

        [PatchPostfix]
        public static void Postfix(TradingItemView __instance, Trader.ItemPrice? price)
        {
            var label = __instance._price;
            if (label == null)
                return;

            Reset(label);

            if (price == null)
                return;

            if (!CurrencyConverter.TryGetRubBracket(price.Value.Amount, price.Value.CurrencyId, out var bracket))
                return;

            label.text = $"{label.text}\n<size=80%><color=#FFFFFFFF>({bracket})</color></size>";
            label.overflowMode = TextOverflowModes.Overflow;

            var rect = label.rectTransform;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, label.fontSize * 2.1f + 8f);
        }

        private static void Reset(TextMeshProUGUI label)
        {
            label.overflowMode = TextOverflowModes.Truncate;
            var rect = label.rectTransform;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, label.fontSize + 6f);
        }
    }
}

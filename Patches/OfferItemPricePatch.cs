using System.Reflection;
using EFT.InventoryLogic;
using EFT.UI.Ragfair;
using HarmonyLib;
using SPT.Reflection.Patching;
using UnityEngine;

namespace SptCurrencyConverter.Patches
{
    // both go through OfferItemPrice.Show.
    public class OfferItemPricePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(OfferItemPrice), nameof(OfferItemPrice.Show));
        }

        [PatchPostfix]
        public static void Postfix(OfferItemPrice __instance, Offer offer)
        {
            if (offer == null || offer.NotAvailable || !offer.OnlyMoney)
                return;

            if (offer.Requirements == null || offer.Requirements.Length == 0)
                return;

            var req = offer.Requirements[0];
            if (!CurrencyConverter.TryGetRubBracket(req.IntCount, req.TemplateId, out var bracket))
                return;

            var label = __instance._priceLabel;
            if (label == null || !label.gameObject.activeSelf)
                return;

            if (!CurrencyUtil.TryGetCurrencyType(req.TemplateId, out var type))
                return;

            var symbol = CurrencyUtil.GetCurrencyChar(type) ?? "";
            var hex = CurrencyUtil.CurrencyColors.TryGetValue(type, out var color)
                ? ColorUtility.ToHtmlStringRGBA(color)
                : "FFFFFFFF";

            label.text = $"{label.text}<color=#{hex}>{symbol}</color> ({bracket})";

            var icon = __instance._priceIcon;
            if (icon != null)
                icon.gameObject.SetActive(false);
        }
    }
}

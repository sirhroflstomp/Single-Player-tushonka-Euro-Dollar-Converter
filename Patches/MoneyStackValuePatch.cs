using System.Reflection;
using EFT.InventoryLogic;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;
using TMPro;
using UnityEngine;

namespace SptCurrencyConverter.Patches
{
    public class MoneyStackValuePatch : ModulePatch
    {
        private const string LineSize = "70%";

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GridItemView), nameof(GridItemView.SetCountValue));
        }

        [PatchPostfix]
        public static void Postfix(GridItemView __instance)
        {
            if (!(__instance.Item is Money money))
                return;

            var label = __instance.ItemValue;
            if (label == null)
                return;

            if (!CurrencyConverter.TryGetRubBracket(money.StackObjectsCount, money.TemplateId, out var bracket))
                return;

            __instance.UpdateItemValue($"{__instance.CurrentItemValue}\n<size={LineSize}><color=#FFFFFFFF>({bracket})</color></size>");

            label.overflowMode = TextOverflowModes.Overflow;
            var rect = label.rectTransform;
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, label.fontSize * 2.1f + 6f);
        }
    }
}

using MelonLoader;
using HarmonyLib;
using System.Linq;
using System;

namespace RandomBattlegroundsSkin
{
    public class RandomBattlegroundsSkinMod : MelonMod
    {
        public static MelonLogger.Instance SharedLogger;

        public override void OnInitializeMelon()
        {
            RandomBattlegroundsSkinMod.SharedLogger = LoggerInstance;
            var harmony = this.HarmonyInstance;
            harmony.PatchAll(typeof(EntityPatcher));
        }
    }

    // Implementation insired by https://github.com/Pik-4/HsMod/blob/master/HsMod/Patcher.cs#L1955
    public static class EntityPatcher
    {
        [HarmonyPatch(typeof(Entity), "LoadCard", new Type[] { typeof(string), typeof(Entity.LoadCardData) })]
        [HarmonyPrefix]
        public static bool Prefix(Entity __instance, ref string cardId)
        {
            if (cardId != null && Utils.IsBattlegroundsSkin(cardId, out Utils.BattlegroundsSkin skin))
            {
                if (__instance.GetCard().GetControllerSide() == Player.Side.FRIENDLY)
                {
                    var randomIndex = UnityEngine.Random.Range(0, skin.Variations.Count);
                    var variation = skin.Variations[randomIndex];
                    RandomBattlegroundsSkinMod.SharedLogger.Msg($"Loaded new skin for {cardId}. All options were {string.Join(", ", skin.Variations)}");
                    cardId = variation;
                }
            }
            return true;
        }
    }
}

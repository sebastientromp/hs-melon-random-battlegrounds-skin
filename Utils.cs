using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomBattlegroundsSkin
{
    public class Utils
    {
        public class BattlegroundsSkin
        {
            public string BaseCardId;
            public List<string> Variations;

            public override string ToString()
            {
                return $"baseCardId={BaseCardId}, variations={string.Join(",", Variations)}";
            }
        }

        public static List<BattlegroundsSkin> CacheBattlegroundsSkin = new List<BattlegroundsSkin>();

        public static class CacheInfo
        {
            public static void UpdateBattlegroundsSkin()
            {
                CacheBattlegroundsSkin.Clear();
                HashSet<Hearthstone.BattlegroundsHeroSkinId> ownedBgSkins = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsHeroSkins>().OwnedBattlegroundsSkins;
                var heroBaseAndSkinCardIds = CollectionManager.Get().GetAllBattlegroundsHeroCardIds();
                foreach (var cardId in heroBaseAndSkinCardIds)
                {
                    var baseCardId = CollectionManager.Get().GetBattlegroundsBaseHeroCardId(cardId);
                    var isSkin = baseCardId != cardId;
                    var dbId = GameUtils.TranslateCardIdToDbId(cardId);
                    var skinId = new Hearthstone.BattlegroundsHeroSkinId();
                    CollectionManager.Get().GetBattlegroundsHeroSkinIdForSkinCardId(dbId, out skinId);
                    // Only include skins that we own
                    if (isSkin && !ownedBgSkins.Contains(skinId))
                    {
                        continue;
                    }

                    var existingSkin = CacheBattlegroundsSkin.FirstOrDefault(s => s.BaseCardId == baseCardId);
                    if (existingSkin == null)
                    {
                        existingSkin = new BattlegroundsSkin
                        {
                            BaseCardId = baseCardId,
                            Variations = new List<string>(),
                        };
                        CacheBattlegroundsSkin.Add(existingSkin);
                    }
                    existingSkin.Variations.Add(cardId);
                }

            }
        }

        public static bool IsBattlegroundsSkin(string cardID, out BattlegroundsSkin skin)
        {
            if (CacheBattlegroundsSkin.Count == 0)
            {
                CacheInfo.UpdateBattlegroundsSkin();
            }

            foreach (var bgSkin in CacheBattlegroundsSkin)
            {
                if (bgSkin.Variations.Contains(cardID))
                {
                    skin = bgSkin;
                    return true;
                }
            }
            skin = new BattlegroundsSkin();
            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace TerrariaChaosEditionUnleashed
{
    internal class ChaosSceneEffects : ModSceneEffect
    {
        public override SceneEffectPriority Priority => (SceneEffectPriority)999999;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override bool IsSceneEffectActive(Player player)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_MUSIC)) return true;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.APRIL_WEATHER)) return true;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.CHAOS_MUSIC)) return true;
            //if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_MONOLITH_FX)) return true;
            return base.IsSceneEffectActive(player);
        }

        public override int Music => ModContent.GetInstance<ChaosSystem>().currMusic;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;

            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.APRIL_WEATHER))
            {
                if (chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.APRIL_WEATHER, 5) == 1)
                {
                    if (SkyManager.Instance["Blizzard"] != null)
                    {
                        SkyManager.Instance.Activate("Blizzard", default(Vector2), new object[0]);
                    }
                    Filters.Scene.Activate("Blizzard", default(Vector2));
                    
                    if (Overlays.Scene["Blizzard"] != null)
                    {
                        Overlays.Scene.Activate("Blizzard", default(Vector2), new object[0]);  
                    }
                }
            }
            base.SpecialVisuals(player, isActive);
        }
    }
}

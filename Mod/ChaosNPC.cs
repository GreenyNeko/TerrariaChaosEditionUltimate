using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace TerrariaChaosEditionUnleashed
{
    internal class ChaosNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        float healthBarScale;
        Vector2 healthBarPositionOffset;
        byte healthBarPosition;
        bool healthBarFxInit = false;
        public override bool? DrawHealthBar(NPC npc, byte hbPosition, ref float scale, ref Vector2 position)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_HEALTH_BARS))
            {
                if(!healthBarFxInit)
                {
                    healthBarScale = Main.rand.NextFloat(1f, 25f);
                    healthBarPosition = (byte)Main.rand.Next(255);
                    healthBarPositionOffset = Main.rand.NextVector2Circular(500f, 500f);
                    healthBarFxInit = true;
                }
                scale = healthBarScale;
                hbPosition = healthBarPosition;
                position += healthBarPositionOffset;
                base.DrawHealthBar(npc, hbPosition, ref scale, ref position);
                return true;
            }
            
            return base.DrawHealthBar(npc, hbPosition, ref scale, ref position);
        }
    }
}

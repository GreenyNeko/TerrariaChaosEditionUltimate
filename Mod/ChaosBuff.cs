using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaChaosEditionUnleashed.Utility;

namespace TerrariaChaosEditionUnleashed
{
    internal class ChaosBuff : GlobalBuff
    {
        public override void Load()
        {
            base.Load();
            On_Player.AddBuff += On_Player_AddBuff;
            On_NPC.AddBuff += On_NPC_AddBuff;
        }

        private void On_NPC_AddBuff(On_NPC.orig_AddBuff orig, NPC self, int type, int time, bool quiet)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.INVERSE_DE_BUFF))
            {
                if (ChaosUtilities.reverseBuff.TryGetValue(type, out int outValue))
                {
                    type = outValue;
                }
            }
            orig.Invoke(self, type, time, quiet);
        }

        private void On_Player_AddBuff(On_Player.orig_AddBuff orig, Player self, int type, int timeToAdd, bool quiet, bool foodHack)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.INVERSE_DE_BUFF))
            {
                if(ChaosUtilities.reverseBuff.TryGetValue(type, out int outValue))
                {
                    type = outValue;
                }
            }
            orig.Invoke(self, type, timeToAdd, quiet, foodHack);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, int type, int buffIndex, ref BuffDrawParams drawParams)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RAND_BUFF_UI_FX))
            {
                if(!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RAND_BUFF_UI_FX))
                {
                    chaosManager.WriteMetaDataByte(0, (byte)Main.rand.Next(), 0);
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RAND_BUFF_UI_FX);
                }
                byte offset = chaosManager.ReadMetaDataByte(0, 0);
                drawParams.Texture = Terraria.GameContent.TextureAssets.Buff[((buffIndex + 1) * type * offset) % Terraria.GameContent.TextureAssets.Buff.Length].Value;
            }
                //spriteBatch.Draw(drawParams.Texture,new Microsoft.Xna.Framework.Rectangle((int)drawParams.Position.X, (int)(drawParams.Position.Y), 64, 1024), drawParams.SourceRectangle, drawParams.DrawColor);
                //drawParams.Position += 3*new Microsoft.Xna.Framework.Vector2(MathF.Cos((float)Main.time), MathF.Sin((float)Main.time));
                
            return base.PreDraw(spriteBatch, type, buffIndex, ref drawParams);
        }
    }
}

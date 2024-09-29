using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static TerrariaChaosEditionUnleashed.ChaosManager;

namespace TerrariaChaosEditionUnleashed
{
    internal class ChaosPlayer : ModPlayer
    {
        public override void PreUpdate()
        {
            base.PreUpdate();
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.CRAZY_GRAVITY))
            {
                Player.gravity = Main.rand.NextFloat()*4f-1.8f;
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.TIME_TRAVEL))
            {
                Main.time += Player.velocity.X * 8f;
            }
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY))
            {
                byte value = (byte)Main.rand.Next(8);
                chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, value, 0);
            }
            base.OnHurt(info);
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ABOMINATION))
            {
                List<Microsoft.Xna.Framework.Vector2> offsets = [
                    drawInfo.backShoulderOffset,
                    drawInfo.frontShoulderOffset,
                    drawInfo.hairOffset,
                    drawInfo.headVect,
                    drawInfo.bodyVect,
                    drawInfo.legVect,
                    drawInfo.helmetOffset,
                    drawInfo.legsOffset,
                ];
                if(!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.ABOMINATION))
                {
                    List<int> indices = [0, 1, 2, 3, 4, 5, 6, 7];
                    List<int> randIndices = new List<int>();
                    for (int i = 0; i < 8; i++)
                    {
                        int randIdx = Main.rand.Next(indices);
                        randIndices.Add(randIdx);
                        indices.Remove(randIdx);
                    }
                    for(int i = 0; i < 8; i++)
                    {
                        chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, (byte)randIndices[i], (uint)i);
                    }
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.ABOMINATION);
                }
                drawInfo.backShoulderOffset = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 0)];
                drawInfo.frontShoulderOffset = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 1)];
                drawInfo.hairOffset = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 2)];
                drawInfo.headVect = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 3)];
                drawInfo.bodyVect = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 4)];
                drawInfo.legVect = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 5)];
                drawInfo.helmetOffset = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 6)];
                drawInfo.legsOffset = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 7)];
            }

            base.ModifyDrawInfo(ref drawInfo);
        }
    }
}

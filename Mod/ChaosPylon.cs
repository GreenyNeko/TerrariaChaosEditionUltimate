using Microsoft.Xna.Framework;
using rail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria.DataStructures;

namespace TerrariaChaosEditionUnleashed
{
    internal class ChaosPylon : GlobalPylon
    {
        public override bool PreDrawMapIcon(ref MapOverlayDrawContext context, ref string mouseOverText, ref TeleportPylonInfo pylonInfo, ref bool isNearPylon, ref Color drawColor, ref float deselectedScale, ref float selectedScale)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_PYLON_FX))
            {
                if(!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_PYLON_FX))
                {
                    chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.RANDOM_PYLON_FX, (byte)Main.rand.Next(256), 0);
                    //chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.RANDOM_PYLON_FX, (byte)Main.rand.Next(256), 1);
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_PYLON_FX);
                }
                byte rand = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.RANDOM_PYLON_FX, 0);
                //byte rand = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.RANDOM_PYLON_FX, 1);
                int fx = rand * pylonInfo.PositionInTiles.X * pylonInfo.PositionInTiles.Y * (int)pylonInfo.TypeOfPylon % 5;
                switch(fx)
                {
                    case 5:
                        drawColor.R = (byte)Main.rand.Next(255);
                        drawColor.G = (byte)Main.rand.Next(255);
                        drawColor.B = (byte)Main.rand.Next(255);
                        break;
                    case 4:
                        pylonInfo.PositionInTiles += new Point16(Main.rand.Next(10) - 5, Main.rand.Next(10) - 5);
                        break;
                    case 3:
                        deselectedScale = Main.rand.NextFloat() * 20f;
                        selectedScale = Main.rand.NextFloat() * 20f;
                        break;
                    case 2:
                        pylonInfo.PositionInTiles += new Point16((int)Math.Sin(Main.time), (int)Math.Cos(Main.time));
                        break;
                    case 1:
                        int rgb = fx % (256 * 256 * 256);
                        drawColor.R = (byte)(rgb % 256);
                        rgb /= 256;
                        drawColor.G = (byte)(rgb % 256);
                        rgb /= 256;
                        drawColor.B = (byte)(rgb % 256);
                        break;
                    default:
                        selectedScale = fx / 255f * 20f;
                        deselectedScale = fx / 255f * 20f;
                        break;
                }

            }
            return base.PreDrawMapIcon(ref context, ref mouseOverText, ref pylonInfo, ref isNearPylon, ref drawColor, ref deselectedScale, ref selectedScale);
        }
    }
}

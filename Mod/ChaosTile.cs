using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace TerrariaChaosEditionUnleashed
{
    internal class ChaosTile : GlobalTile
    {
        public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY))
            {
                byte counter = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, 0);
                int blue = counter % 2;
                int green = (counter >> 1) % 2;
                int red = (counter >> 2) % 2;

                if(blue == 0 && green == 0 && red == 0)
                {
                    blue = green = red = 1;
                }
                drawData.tileLight = new Color(drawData.tileLight.R/255f * (float)red, drawData.tileLight.G/255f * (float)green, drawData.tileLight.B/255f *  (float)blue);
            }
            base.DrawEffects(i, j, type, spriteBatch, ref drawData);
        }

        public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);

            Tile tile = Main.tile[i, j];
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.HUGE_WORLD))
            {
                spriteBatch.Draw(TextureAssets.Tile[type].Value,
                    new Vector2(i * 16 - (int)Main.screenPosition.X - 32, j * 16 - (int)Main.screenPosition.Y - 32) + zero,
                    new Rectangle(0, 0, 16, 16),
                    Lighting.GetColor(i, j), 0f, default, 4f, SpriteEffects.None, 0f
                );
                return false;
            }
            return base.PreDraw(i, j, type, spriteBatch);
        }

        public override void FloorVisuals(int type, Player player)
        {
            base.FloorVisuals(type, player);
        }
    }
}

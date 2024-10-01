using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace TerrariaChaosEditionUnleashed
{
    internal class ChaosItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        float timeInWorld;
        double lastUpdate = 0;

        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            double deltaTime = Main.gameTimeCache.TotalGameTime.TotalSeconds - lastUpdate;
            timeInWorld += (float)deltaTime;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RAND_ITEM_DROP_FX))
            {
                if(!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RAND_ITEM_DROP_FX))
                {
                    chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.RAND_ITEM_DROP_FX, (byte)Main.rand.Next(256), 0);
                    chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.RAND_ITEM_DROP_FX, (byte)Main.rand.Next(256), 1);
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RAND_ITEM_DROP_FX);
                }
                int fx = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.RAND_ITEM_DROP_FX, 0);
                fx = (fx * item.netID) % 65;
                if ((fx & 1) == 1)
                { 
                    gravity = 0;
                    maxFallSpeed = 0;
                }
                if((fx & 2) == 2)
                {
                    gravity = -gravity;
                }
            }
            base.Update(item, ref gravity, ref maxFallSpeed);
            lastUpdate = Main.gameTimeCache.TotalGameTime.TotalSeconds;
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            double deltaTime = Main.gameTimeCache.TotalGameTime.TotalSeconds - lastUpdate;
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RAND_ITEM_DROP_FX))
            {
                int whoAmIVariation = whoAmI + 1;
                int fx = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.RAND_ITEM_DROP_FX, 0);
                fx = (fx * whoAmIVariation) % 65;
                if ((fx & 4) == 4)
                {
                    byte colorByte = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.RAND_ITEM_DROP_FX, 1);
                    int red = colorByte % 8;
                    int green = (colorByte >> 3) % 8;
                    int blue = colorByte >> 6;
                    lightColor = new Color(((red * whoAmIVariation) % 8) / 7f, ((green * whoAmIVariation) % 8) / 7f, (blue * whoAmIVariation) / 3f);
                }
                if ((fx & 8) == 8)
                {
                    byte strength = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.RAND_ITEM_DROP_FX, 1);
                    strength = (byte)((strength * whoAmIVariation) % 256);
                    rotation += timeInWorld * ((float)(strength) - 127) / 32f;
                }
                if((fx & 16) == 16)
                {
                    scale = Main.rand.NextFloat(0.01f, 7.5f);
                }
                if((fx & 32) == 32)
                {
                    byte size = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.RAND_ITEM_DROP_FX, 1);
                    size = (byte)((size * whoAmIVariation) % 256);
                    scale += (size+1f)/8f;
                }
                if ((fx & 64) == 64)
                {
                    scale *= (timeInWorld + 1);
                }
            }
            
            return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_ITEM_ICONS))
            {
                if(!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_ITEM_ICONS))
                {
                    chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.RANDOM_ITEM_ICONS, (byte)Main.rand.Next(256), 0);
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_ITEM_ICONS);
                }
                int itemTextures = Terraria.GameContent.TextureAssets.Item.Length; //+1 % itemTextures
                int offset = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.RANDOM_ITEM_ICONS, 0);
                Main.instance.LoadItem((item.type + offset) % itemTextures);
                spriteBatch.Draw(Terraria.GameContent.TextureAssets.Item[(item.type + offset) % itemTextures].Value, position, frame, drawColor, 0, origin, scale, SpriteEffects.None, 0f);
                return false;
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.INVISIBLE_INVENTORY_ITEMS))
            {
                return false;
            }
            return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }
    }
}

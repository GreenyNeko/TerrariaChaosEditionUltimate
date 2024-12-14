using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;
using TerrariaChaosEditionUnleashed.Utility;

namespace TerrariaChaosEditionUnleashed
{
    internal class ChaosItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        float timeInWorld;
        double lastUpdate = 0;
        int overridenType = 0;

        public override void OnSpawn(Item item, IEntitySource source)
        {
           
            //overridenType = item.type;
            //Main.NewText(item.Name + " " + item.damage + " " + item.DamageType);
            base.OnSpawn(item, source);
            // works but items change back which we don't want
            //item.type = Main.rand.Next(Terraria.ID.ItemID.Count);
            //item.netID = Main.rand.Next(Terraria.ID.ItemID.Count);
        }

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
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.PLAYER_TORNADO))
            {
                Player closestPlayer = ChaosUtilities.ClosestPlayer(item.position);
                Vector2 vec = closestPlayer.position - item.position;
                vec.Normalize();
                item.velocity = vec;
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

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_TOOLTIP_EFFECTS))
            {
                if(!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_TOOLTIP_EFFECTS))
                {
                    chaosManager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.RANDOM_TOOLTIP_EFFECTS, BitConverter.GetBytes(Main.rand.Next()), 0);
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_TOOLTIP_EFFECTS);
                }

                for (int i = 0; i < tooltips.Count(); i++)
                {
                    
                    byte[] randData = chaosManager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.RANDOM_TOOLTIP_EFFECTS, 0, 4);
                    // generate pseudo random number dependent on tooltip line count, item id, and random generated value from effect
                    float randValueTemp = MathF.Abs(MathF.Sin(Vector2.Dot(new Vector2((float)randData[0],(float)(i + item.netID)), new Vector2(12.9898f, 78.233f))) * 43758.5453f);
                    float randValue = randValueTemp - (int)randValueTemp;
                    tooltips[i].IsModifier = (randValue < 0.6666f);
                    tooltips[i].IsModifierBad = (randValue < 0.3333f);

                    // new random value for these effects
                    randValueTemp = MathF.Abs(MathF.Sin(Vector2.Dot(new Vector2((float)randData[1], (float)(i + item.netID)), new Vector2(12.9898f, 78.233f))) * 43758.5453f);
                    randValue = randValueTemp - (int)randValueTemp;
                    // use digits to roll each effect
                    if((int)(randValue * 10) < 2)
                    {
                        // backwards
                        string newText = "";
                        for (int j = 0; j < tooltips[i].Text.Length; j++)
                        {
                            newText += tooltips[i].Text[tooltips[i].Text.Length - j - 1];
                        }
                        tooltips[i].Text = newText;
                    }
                    if((int)(randValue * 100) % 10 < 2)
                    {
                        // swap + and -
                        tooltips[i].Text = tooltips[i].Text.Replace("+", "<temp>").Replace("-", "+").Replace("<temp>", "-");
                        if (tooltips[i].Name == "Damage" || tooltips[i].Name == "CritChance" 
                            || tooltips[i].Name == "PickPower" || tooltips[i].Name == "AxePower"
                            || tooltips[i].Name == "HammerPower")
                        {
                            tooltips[i].Text = "-" + tooltips[i].Text;
                        }
                    }
                    if((int)(randValue * 1000) % 10 < 2)
                    {
                        // no vowels
                        tooltips[i].Text = tooltips[i].Text.Replace("a", "").Replace("e", "").Replace("i", "").Replace("o", "").Replace("u", "");
                    }
                    if((int)(randValue * 10000) % 10 < 2)
                    {
                        // replace random words with other words

                        //tooltips[i].Text = tooltips[i].Text.Replace("damage", Lang.GetItemNameValue());
                    }
                }
            }
            
            base.ModifyTooltips(item, tooltips);
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

        public override bool? UseItem(Item item, Player player)
        {
            
            return base.UseItem(item, player);
        }
    }
}

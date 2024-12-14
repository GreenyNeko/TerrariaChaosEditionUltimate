using Humanizer.Bytes;
using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using TerrariaChaosEditionUnleashed.Utility;
using static TerrariaChaosEditionUnleashed.ChaosManager;

namespace TerrariaChaosEditionUnleashed
{
    internal class ChaosPlayer : ModPlayer
    {
        bool stunTooltipSeen = false;
        float stunThreshold = 0;
        float stunTime = 0;
        float initialStunTime = 0;
        int lastInput = -1;
        double lastUpdate = 0;
        bool stunned = false;
        Vector2 prevPos = Vector2.Zero;
        Vector2 subPixel = Vector2.Zero;
        Vector2 vanillaVel = Vector2.Zero;
        Vector2 effectVel = Vector2.Zero;
        /// <summary> Effect data per player </summary>
        Dictionary<int, byte[]> PlayerFxData = new Dictionary<int, byte[]>();

        public override void Load()
        {
            On_Player.WaterCollision += OnPlayerWaterCollision;
            On_Player.OverheadMessage.NewMessage += OverheadMessage_NewMessage;
        }

        private void OverheadMessage_NewMessage(On_Player.OverheadMessage.orig_NewMessage orig, ref Player.OverheadMessage self, string message, int displayTime)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.VERIFY_HUMAN))
            {
                byte success = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.VERIFY_HUMAN, 0);
                byte[] wordData = chaosManager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.VERIFY_HUMAN, 1, 6);
                string word = Encoding.Default.GetString(wordData);
                if (word == message)
                {
                    chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.VERIFY_HUMAN, 1, 0);
                }
            }
            orig.Invoke(ref self, message, displayTime);
        }

        private void OnPlayerWaterCollision(On_Player.orig_WaterCollision orig, Player self, bool fallThrough, bool ignorePlats)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            // copy code from lava
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.HOT_WAWA))
            {
                if (!self.lavaImmune)
                {
                    self.Hurt(PlayerDeathReason.ByOther(2), 80, 1, false, true, 2, false, 0, 0, 0);
                    self.AddBuff(Terraria.ID.BuffID.OnFire, Main.expertMode ? 14 * 30 : 7 * 30);
                }
            }
            orig.Invoke(self, fallThrough, ignorePlats);
        }

        public override void ModifyScreenPosition()
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.OLD_SCHOOL_CAM))
            {
                float width = (Main.screenWidth - (Main.GameViewMatrix.Zoom.X - 1) * Main.screenWidth * 0.5f);
                float height = (Main.screenHeight - (Main.GameViewMatrix.Zoom.Y - 1) * Main.screenHeight * 0.5f);
                float offscreenWidth = Main.screenWidth - width;
                float offscreenHeight = Main.screenHeight - height;
                int x = (int)(Player.Center.X / width);
                int y = (int)(Player.Center.Y / height);
                Main.screenPosition = new Vector2(x * width - offscreenWidth / 2f, y * height - offscreenHeight / 2f);
            }
            base.ModifyScreenPosition();
        }

        public override void PreUpdateMovement()
        {
        }

        public override void PreUpdate()
        {
            double deltaTime = Main.gameTimeCache.TotalGameTime.TotalSeconds - lastUpdate;
            base.PreUpdate();
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.SHIMMERING))
            {
                Player.ShimmerCollision(true, true, true);
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.CRAZY_GRAVITY))
            {
                Player.gravity = Main.rand.NextFloat() * 4f - 1.8f;
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.NO_GRAVITY))
            {
                Player.gravity = 0;
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.NO_CREATIVITY))
            {
                Player.noBuilding = true;
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_TELEPORT))
            {
                if (!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_TELEPORT))
                {
                    Player.TeleportationPotion();
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_TELEPORT);
                }
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.LIFE_MANA_SWAP))
            {
                if (!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.LIFE_MANA_SWAP))
                {
                    int temp = Player.statLifeMax;
                    Player.statLifeMax = Player.statManaMax;
                    Player.statManaMax = temp;
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.LIFE_MANA_SWAP);
                }
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.MAGIC_MIRROR))
            {
                if (!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.MAGIC_MIRROR))
                {
                    Player.Spawn(PlayerSpawnContext.RecallFromItem);
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.MAGIC_MIRROR);
                }
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_BUFF))
            {
                if (!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_BUFF))
                {
                    Player.AddBuff(Main.rand.Next(ChaosUtilities.goodBuffs), (int)(60 * chaosManager.GetEffectDuration((int)ChaosManager.ChaosEffects.RANDOM_BUFF)));
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_BUFF);
                }
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_DEBUFF))
            {
                if (!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_DEBUFF))
                {
                    Player.AddBuff(Main.rand.Next(ChaosUtilities.badBuffs), (int)(60 * chaosManager.GetEffectDuration((int)ChaosManager.ChaosEffects.RANDOM_DEBUFF)));
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_DEBUFF);
                }
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_PET))
            {
                if (!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_DEBUFF))
                {
                    Player.AddBuff(Main.rand.Next(ChaosUtilities.petsBuffs), (int)(60 * chaosManager.GetEffectDuration((int)ChaosManager.ChaosEffects.RANDOM_PET)));
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_PET);
                }
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.DANGEROUS_XRAY))
            {
                if (!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.DANGEROUS_XRAY))
                {
                    Player.AddBuff(BuffID.Dangersense, (int)(60 * chaosManager.GetEffectDuration((int)ChaosManager.ChaosEffects.RANDOM_PET)));
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.DANGEROUS_XRAY);
                }
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.MAX_LIFE_MANA))
            {
                if (!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.MAX_LIFE_MANA))
                {
                    int roll = Main.rand.Next(4);
                    (int, int)[] rolls = new (int, int)[] { (-20, 0), (20, 0), (0, -20), (0, 20) };
                    Player.statLifeMax += rolls[roll].Item1;
                    Player.statManaMax += rolls[roll].Item2;
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.MAX_LIFE_MANA);
                }
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RAND_LIFE_MANA))
            {
                Player.statLife = Main.rand.Next(Player.statLifeMax2) + 1;
                Player.statMana = Main.rand.Next(Player.statManaMax2);
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.BUTTER_FINGERS))
            {
                if (!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.BUTTER_FINGERS))
                {
                    Player.DropSelectedItem();
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.BUTTER_FINGERS);
                }
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.TIME_TRAVEL))
            {
                Main.time += Player.velocity.X * 8f;
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ENEMIES_STUN))
            {
                if (stunThreshold > 0)
                {
                    stunThreshold -= (float)deltaTime;
                }
                if (stunTime > 0)
                {
                    stunTime -= (float)deltaTime;
                }
                else
                {
                    if (stunned)
                    {
                        CombatText.NewText(Player.getRect(), Color.LimeGreen, "Stun Recovery!", false);
                        stunned = false;
                    }
                }
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.UP_OR_DIE))
            {
                if (!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.UP_OR_DIE))
                {
                    Main.NewText("Press UP to not die!");
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.UP_OR_DIE);
                    chaosManager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.UP_OR_DIE, BitConverter.GetBytes(0f), 0);
                }
                byte done = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.UP_OR_DIE, 4);
                if (done == 0)
                {
                    byte[] timeData = chaosManager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.UP_OR_DIE, 0, 4);
                    float time = BitConverter.ToSingle(timeData, 0);
                    time += (float)deltaTime;
                    if (time > 3)
                    {
                        Player.Hurt(PlayerDeathReason.ByOther(0), Player.statLifeMax2, 0, false, true, -1, false, 9999f, 9999f, 0f);
                        chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.UP_OR_DIE, 1, 4);
                    }
                    chaosManager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.UP_OR_DIE, BitConverter.GetBytes(time), 0);
                }
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RAND_NUMBER_FX))
            {
                Color[] colors = new Color[]{CombatText.HealLife, CombatText.HealMana, CombatText.DamagedFriendly, CombatText.DamagedFriendlyCrit, CombatText.DamagedHostile,
                    CombatText.DamagedHostileCrit, CombatText.OthersDamagedHostile, CombatText.OthersDamagedHostileCrit, CombatText.LifeRegen, CombatText.LifeRegenNegative,
                };
                CombatText.NewText(Player.getRect(), Main.rand.Next(colors), Main.rand.Next(-9999999, 9999999), Main.rand.NextBool(), Main.rand.NextBool());
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ALWAYS_WET))
            {
                Player.wet = true;
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ROD_OF_DISCORD))
            {
                if(!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.ROD_OF_DISCORD))
                {
                    Player.Teleport(Main.MouseWorld, Terraria.ID.TeleportationStyleID.RodOfDiscord);
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.ROD_OF_DISCORD);
                }
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.FAKE_PICKUP))
            {
                if (!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.FAKE_PICKUP))
                {
                    Item item = Player.QuickSpawnItemDirect(new EntitySource_Misc("temp spawn"), Main.rand.Next(ItemID.Count));
                    PopupText.NewText(PopupTextContext.RegularItemPickup, item, 0);
                    item.TurnToAir();
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.FAKE_PICKUP);
                }
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.SPOOKY_GHOST))
            {
                Player.ghost = true;
            }
            else
            {
                if(chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.SPOOKY_GHOST))
                {
                    Player.ghost = false;
                    chaosManager.ResetEffectAsInitalDoneFlag((int)ChaosManager.ChaosEffects.SPOOKY_GHOST);
                }
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_CRAFT))
            {
                if(!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_CRAFT))
                {
                    int idx = Main.availableRecipe[Main.rand.Next(Main.numAvailableRecipes)];
                    Recipe recipe = Main.recipe.Single(recipe => recipe.RecipeIndex == idx);
                    foreach (Item ingredient in recipe.requiredItem)
                    {
                        int removalAmount = ingredient.stack;
                        for (int i = 0; i < Player.inventory.Length && removalAmount > 0; i++)
                        {
                            if (Player.inventory[i].type != ingredient.type)
                            {
                                continue;
                            }
                            int amount = Player.inventory[i].stack;
                            if (amount - removalAmount < 0)
                            {
                                Player.inventory[i].TurnToAir();
                            }
                            else
                            {
                                Player.inventory[i].stack -= removalAmount;
                            }
                            removalAmount -= amount;
                        }
                        
                    }
                    Player.QuickSpawnItem(null, recipe.createItem, recipe.createItem.stack);
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_CRAFT);
                }
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.FALL_SENSITIVITY))
            {
                if (!Player.dead)
                {
                    Player.extraFall -= 25;
                }
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.NON_BINARY_GENDER))
            {
                Player.Male = Main.rand.NextBool();
            }
            lastUpdate = Main.gameTimeCache.TotalGameTime.TotalSeconds;
        }

        public override void PostUpdate()
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ACCUMULATING_VELOCITY))
            {
                Vector2 velDiff = Player.velocity - Player.oldVelocity;
                // 2x velocity change
                Player.velocity += velDiff * 0.05f;
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.DISCRETIZED_MOVEMENT))
            {
                // TODO: which of these vars do we want to store in the effect?
                // get vanilla vel
                vanillaVel = Player.velocity - effectVel;
                // remove effect vel from previous subpixel boost
                Player.velocity = vanillaVel;
                Vector2 playerPos = Player.position;
                Vector2 roundedPos = new Vector2(MathF.Round(playerPos.X / 16f) * 16, MathF.Round(playerPos.Y / 16f) * 16);
                // fix to ground
                roundedPos.Y += 6;
                subPixel += Player.position - roundedPos;
                Player.position = roundedPos;
                Vector2 newVel = Vector2.Zero;
                if (MathF.Abs(subPixel.X) >= 16)
                {
                    newVel.X = MathF.Sign(subPixel.X) * 16;
                    subPixel.X = 0;
                }
                if (MathF.Abs(subPixel.Y) >= 16)
                {
                    newVel.Y = MathF.Sign(subPixel.Y) * 16;
                    subPixel.Y = 0;
                }
                effectVel = newVel;
                Player.velocity = vanillaVel + effectVel;
                prevPos = roundedPos;
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.UPSIDE_DOWN))
            {
                Player.gravDir = -1f;
                if (Player.direction == 1)
                {
                    Player.itemLocation -= new Vector2(16, -40);
                }
                else
                {
                    Player.itemLocation -= new Vector2(-48, -40);
                }
            }
            base.PostUpdate();
        }

        public override void PreSavePlayer()
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.LIFE_MANA_SWAP))
            {
                if (chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.LIFE_MANA_SWAP))
                {
                    int temp = Player.statLifeMax;
                    Player.statLifeMax = Player.statManaMax;
                    Player.statManaMax = temp;
                }
            }
            base.PreSavePlayer();
        }

        public override void SetControls()
        {
            base.SetControls();
            if(stunTime > 0)
            {
                Player.controlUp = false;
                Player.controlDown = false;
                Player.controlLeft = false;
                Player.controlRight = false;
                Player.controlJump = false;
                Player.controlUseItem = false;
                Player.controlMount = false;
                Player.controlUseTile = false;
                Player.controlThrow = false;
                Player.controlQuickMana = false;
                Player.controlHook = false;
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ENEMIES_STUN))
            {
                stunThreshold += MathF.Pow(hurtInfo.Knockback + 1f, 2);
                if (stunTime > 0)
                {
                    stunTime += hurtInfo.Knockback;
                    if (stunTime > initialStunTime)
                    {
                        initialStunTime = stunTime;
                    }
                }
                else if (stunThreshold > 100)
                {
                    CombatText.NewText(Player.getRect(), Color.OrangeRed, "STUNNED!", true);
                    if (!stunTooltipSeen)
                    {
                        stunTooltipSeen = true;
                        Main.NewText("Hint: Tap left and right repeatedly to free yourself from the stun!");
                    }
                    stunned = true;
                    hurtInfo.Knockback *= 5;
                    stunTime = 7.5f + stunThreshold - 100;
                    initialStunTime = stunTime;
                    stunThreshold -= 100;
                }
            }
            base.OnHitByProjectile(proj, hurtInfo);
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ENEMIES_STUN))
            {
                stunThreshold += MathF.Pow(hurtInfo.Knockback + 1f, 2);
                if (stunTime > 0)
                {
                    stunTime += hurtInfo.Knockback;
                    if (stunTime > initialStunTime)
                    {
                        initialStunTime = stunTime;
                    }
                }
                else if (stunThreshold > 100)
                {
                    CombatText.NewText(Player.getRect(), Color.OrangeRed, "STUNNED!", true);
                    if (!stunTooltipSeen)
                    {
                        stunTooltipSeen = true;
                        Main.NewText("Hint: Tap left and right repeatedly to free yourself from the stun!");
                    }
                    stunned = true;
                    hurtInfo.Knockback *= 5;
                    stunTime = 7.5f + stunThreshold - 100;
                    initialStunTime = stunTime;
                    stunThreshold -= 100;
                }
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.REINFORCEMENTS))
            {
                if(Main.rand.NextFloat() < 0.1)
                {
                    NPC.SpawnOnPlayer(Player.whoAmI, npc.type);
                }
            }
            base.OnHitByNPC(npc, hurtInfo);
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY))
            {
                byte value = (byte)Main.rand.Next(8);
                chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.PAIN_SHIFTS_REALITY, value, 0);
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.SMASH_BROS))
            {
                if (!PlayerFxData.ContainsKey((int)ChaosManager.ChaosEffects.SMASH_BROS))
                {
                    PlayerFxData.Add((int)ChaosManager.ChaosEffects.SMASH_BROS, new byte[] { 0, 0 });
                }
                byte[] currDamageData = PlayerFxData[(int)ChaosManager.ChaosEffects.SMASH_BROS];
                short currDamage = BitConverter.ToInt16(currDamageData);
                currDamage = Math.Max((short)currDamage, (short)(currDamage + info.Damage / 10));
                PlayerFxData[(int)ChaosManager.ChaosEffects.SMASH_BROS] = BitConverter.GetBytes(currDamage);
                info.Knockback *= (1 + currDamage / 100f);
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.SONIC_HEALTH))
            {
                int accumulatedValue = 0;
                for (int i = 0; i < Player.inventory.Count(); i++)
                {
                    if (Player.inventory[i].type != ItemID.CopperCoin && Player.inventory[i].type != ItemID.SilverCoin
                        && Player.inventory[i].type != ItemID.GoldCoin && Player.inventory[i].type != ItemID.PlatinumCoin)
                    {
                        continue;
                    }
                    switch (Player.inventory[i].type)
                    {
                        case ItemID.PlatinumCoin:
                            accumulatedValue += (int)MathF.Ceiling(Player.inventory[i].stack / 2f) * 1000000;
                            break;
                        case ItemID.GoldCoin:
                            accumulatedValue += (int)MathF.Ceiling(Player.inventory[i].stack / 2f) * 10000;
                            break;
                        case ItemID.SilverCoin:
                            accumulatedValue += (int)MathF.Ceiling(Player.inventory[i].stack / 2f) * 100;
                            break;
                        case ItemID.CopperCoin:
                            accumulatedValue += (int)MathF.Ceiling(Player.inventory[i].stack / 2f);
                            break;
                    }
                    int coinLoss = (int)MathF.Ceiling(Player.inventory[i].stack / 2f);
                    Player.inventory[i].stack -= coinLoss;
                    int coinItem = Item.NewItem(new EntitySource_DropAsItem(Player), (int)Player.position.X, (int)Player.position.Y, Player.width, Player.height, Player.inventory[i].type, 1, false, 0, false, false);
                    Main.item[coinItem].stack = coinLoss;
                    Main.item[coinItem].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.4f;
                    Main.item[coinItem].velocity.X = (float)Main.rand.Next(-20, 21) * 0.8f;
                    Main.item[coinItem].noGrabDelay = 300;
                }
                if (accumulatedValue == 0)
                {
                    Player.KillMe(info.DamageSource, info.Damage, info.HitDirection);
                    //info.Damage = 999999;
                }
            }
            base.OnHurt(info);
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            //drawInfo.Position -= new Vector2(0, 15);
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ABOMINATION))
            {
                List<Microsoft.Xna.Framework.Vector2> offsets = [
                    drawInfo.hairOffset,
                    Player.headPosition,
                    Player.bodyPosition,
                    Player.legPosition,
                ];
                if(!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.ABOMINATION))
                {
                    List<int> indices = [0, 1, 2, 3];
                    List<int> randIndices = new List<int>();
                    for (int i = 0; i < 4; i++)
                    {
                        int randIdx = Main.rand.Next(indices);
                        randIndices.Add(randIdx);
                        indices.Remove(randIdx);
                    }
                    for(int i = 0; i < 4; i++)
                    {
                        chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, (byte)randIndices[i], (uint)i);
                    }
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.ABOMINATION);
                }
                drawInfo.hairOffset = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 1)];
                Player.headPosition = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 2)];
                Player.bodyPosition = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 3)];
                Player.legPosition = offsets[chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.ABOMINATION, 4)];
            }
            if(stunTime > 0)
            {
                Main.instance.DrawHealthBar(Player.position.X, Player.position.Y + 20f, (int)stunTime, (int)initialStunTime, 1f);
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.PLAYER_SOLAR_SYSTEM))
            {
                if(!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.PLAYER_SOLAR_SYSTEM))
                {
                    // rotDirFlag body, posDirFlag head, rotDirFlag head, rotDirFlag leg, posDirFlag hair
                    // 5bit
                    chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.PLAYER_SOLAR_SYSTEM, (byte)Main.rand.Next(256), 0);
                }
                byte rotData = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.PLAYER_SOLAR_SYSTEM, 0);
                float dir = (rotData & 1) == 1 ? -1 : 1;
                float angle = (float)Main.time / 200f * dir;
                Player.bodyRotation = (angle) % MathF.PI * 2;

                Player.headPosition = new Vector2(0, -15);
                dir = (rotData & 2) == 2 ? -1 : 1;
                angle = (float)Main.time / 100f * dir;
                float helperX = Player.headPosition.X * MathF.Cos(angle) - Player.headPosition.Y * MathF.Sin(angle);
                float helperY = Player.headPosition.X * MathF.Sin(angle) + Player.headPosition.Y * MathF.Cos(angle);
                Player.headPosition = new Vector2(helperX, helperY) + new Vector2(0, 15);
                dir = (rotData & 4) == 4 ? -1 : 1;
                angle = (float)Main.time / 200f * dir;
                Player.headRotation = (angle) % MathF.PI * 2;

                dir = (rotData & 8) == 8 ? -1 : 1;
                angle = (float)Main.time / 150f * dir;
                Player.legRotation = (angle) % MathF.PI * 2;

                drawInfo.hairOffset = -Player.headPosition;
                dir = (rotData & 16) == 16 ? -1 : 1;
                angle = (float)Main.time / 200f * dir;
                helperX = drawInfo.hairOffset.X * MathF.Cos(angle) - drawInfo.hairOffset.Y * MathF.Sin(angle);
                helperY = drawInfo.hairOffset.X * MathF.Sin(angle) + drawInfo.hairOffset.Y * MathF.Cos(angle);
                drawInfo.hairOffset = new Vector2(helperX, helperY) + Player.headPosition;
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.UPSIDE_DOWN))
            {
                Player.direction *= -1;
                drawInfo.rotation = MathF.PI;
                drawInfo.Position -= new Vector2(-16, -44);
            }
            base.ModifyDrawInfo(ref drawInfo);
        }

        public override void UpdateDead()
        {
            // TODO move these to effect data
            stunTime = 0;
            stunThreshold = 0;
            stunned = false;
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (PlayerFxData.ContainsKey((int)ChaosManager.ChaosEffects.SMASH_BROS))
            {
                PlayerFxData[(int)ChaosManager.ChaosEffects.SMASH_BROS] = new byte[] { 0, 0 };
            }
            base.UpdateDead();
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.EXPLOSIVE_DEATH))
            {
                int[] explosiveType = { ProjectileID.Bomb, ProjectileID.Grenade, ProjectileID.Dynamite, ProjectileID.DynamiteKitten, ProjectileID.BouncyDynamite,
                    ProjectileID.BouncyBomb, ProjectileID.BouncyGrenade, ProjectileID.ExplosiveBunny, ProjectileID.BouncyGrenade, ProjectileID.Grenade, ProjectileID.ExplosiveBunny,
                    ProjectileID.DynamiteKitten, ProjectileID.HappyBomb, ProjectileID.DirtBomb, ProjectileID.HoneyBomb, ProjectileID.DryBomb, ProjectileID.StickyBomb, ProjectileID.WetBomb,
                    ProjectileID.SmokeBomb, ProjectileID.DryGrenade, ProjectileID.WetGrenade, ProjectileID.LavaGrenade, ProjectileID.PartyGirlGrenade, ProjectileID.Landmine, 
                    ProjectileID.Beenade,
                };
                Entity source;
                damageSource.TryGetCausingEntity(out source);
                Projectile.NewProjectileDirect(new EntitySource_Death(source), Player.position, Vector2.Zero, Main.rand.Next(explosiveType), 0, 0);
            }
            base.Kill(damage, hitDirection, pvp, damageSource);
        }

        public override void ModifyLuck(ref float luck)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.SEVEN_YEARS_BAD_LUCK))
            {
                luck -= 999999f;
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.CURSED_BY_LUCK))
            {
                luck += 999999f;
            }
            base.ModifyLuck(ref luck);
        }

        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.HEALING_HURTS))
            {
                healValue = -healValue;
            }
            healValue = -healValue;
            base.GetHealLife(item, quickHeal, ref healValue);
        }

        public override void ModifyItemScale(Item item, ref float scale)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.BIG_WEAPONS))
            {
                scale = 5f;
            }
            base.ModifyItemScale(item, ref scale);
        }

        public override float UseSpeedMultiplier(Item item)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ITEMS_GO_HAM))
            {
                return 999f;
            }
            return base.UseSpeedMultiplier(item);
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            //triggersSet.
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ENEMIES_STUN))
            {
                if(stunTime > 0)
                {
                    if (triggersSet.Left && lastInput != 0)
                    {
                        lastInput = 0;
                        stunTime -= 1;
                    }
                    else if (triggersSet.Right && lastInput != 1)
                    {
                        lastInput = 1;
                        stunTime -= 1;
                    }
                }
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.UP_OR_DIE))
            {
                byte done = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.UP_OR_DIE, 4);
                if (done == 0)
                {
                    if (triggersSet.Up)
                    {
                        chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.UP_OR_DIE, 1, 4);
                    }
                }
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.BREAKOUT))
            {
                if (chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.BREAKOUT, 28) == 0)
                {
                    byte[] dataPaddleX = chaosManager.ReadMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, 16, 4);
                    int paddleX = BitConverter.ToInt32(dataPaddleX);
                    if (triggersSet.Left)
                    {
                        if (paddleX - 1 > 0)
                        {
                            paddleX -= 8;
                        }
                    }
                    else if (triggersSet.Right)
                    {
                        if (paddleX + 1 < Main.ScreenSize.X - 280 - 140)
                        {
                            paddleX += 8;
                        }
                    }
                    chaosManager.WriteMetaDataBytes((int)ChaosManager.ChaosEffects.BREAKOUT, BitConverter.GetBytes(paddleX), 16);
                }
            }
            base.ProcessTriggers(triggersSet);
        }
    }
}

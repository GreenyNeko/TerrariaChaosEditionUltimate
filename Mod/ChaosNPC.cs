using Humanizer.Bytes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.WorldBuilding;

namespace TerrariaChaosEditionUnleashed
{
    internal class ChaosNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        float healthBarScale;
        Vector2 healthBarPositionOffset;
        byte healthBarPosition;
        bool healthBarFxInit = false;
        // TODO: put them into NpcfxData
        Vector2 prevPos = Vector2.Zero;
        Vector2 subPixel = Vector2.Zero;
        Vector2 vanillaVel = Vector2.Zero;
        Vector2 effectVel = Vector2.Zero;
        /// <summary> Effect data per npc </summary>
        Dictionary<int, byte[]> NpcFxData;

        public override void SetDefaults(NPC entity)
        {
            NpcFxData = new Dictionary<int, byte[]>();
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.FAKE_ENEMIES))
            {
                if(Main.rand.NextFloat() < 0.05)
                {
                    entity.friendly = true;
                    entity.immortal = true;
                }
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RECOLORED_NPCS))
            {
                entity.color = new Color(Main.rand.NextFloat(), Main.rand.NextFloat(), Main.rand.NextFloat());
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RAND_NPC_FX))
            {
                int flag = Main.rand.Next(128);
                entity.rotation = (flag & 1) == 1 ? Main.rand.NextFloat() : entity.rotation;
                entity.aiStyle = (flag & 2) == 2 ? Main.rand.Next(126) : entity.aiStyle; 
                entity.alpha = (flag & 4) == 4 ? Main.rand.Next(256) : entity.alpha;
                entity.dontTakeDamage = (flag & 8) == 8 ? !entity.dontTakeDamage : entity.dontTakeDamage;
                entity.ForcePartyHatOn =(flag & 16) == 16 ? true : false;
                entity.friendly = (flag & 32) == 32 ? !entity.friendly : entity.friendly;
                entity.direction = (flag & 64) == 64 ? -entity.direction : entity.direction;
                flag = Main.rand.Next(128);
                entity.knockBackResist = (flag & 1) == 1 ? 0f : entity.knockBackResist;
                entity.shimmering = (flag & 2) == 2 ? !entity.shimmering : entity.shimmering;
                entity.scale = (flag & 4) == 4 ? Main.rand.NextFloat() * 5 : entity.scale;
                entity.noGravity = (flag & 8) == 8 ? !entity.noGravity : entity.noGravity;
                entity.noTileCollide = (flag & 16) == 16 ? !entity.noTileCollide : entity.noTileCollide;
                entity.reflectsProjectiles = (flag & 32) == 32 ? !entity.reflectsProjectiles : entity.reflectsProjectiles;
                entity.soundDelay = (flag & 64) == 64 ? 5 : entity.soundDelay;
                entity.spriteDirection = (flag & 1) == 1 ? Main.rand.Next(2) : entity.spriteDirection;
                entity.teleporting = (flag & 2) == 2 ? true : entity.teleporting;
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.NO_FLYING_AND_WORMING))
            {
                entity.noTileCollide = false;
                entity.noGravity = false;
            }
            base.SetDefaults(entity);
        }

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.IMMENSE_SPAWN_RATE))
            {
                spawnRate = 9001;
                maxSpawns = 9001;
            }
            base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
        }

        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            base.ModifyGlobalLoot(globalLoot);
        }

        public override bool PreAI(NPC npc)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.NO_GRAVITY))
            {
                npc.noGravity = true;
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.PLAYER_TORNADO))
            {
                int playerId = npc.FindClosestPlayer();
                Vector2 vec = (Main.player[playerId].position - npc.position);
                vec.Normalize();
                npc.velocity = vec;
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.NPC_GROW_SHRINK))
            {
                if (NpcFxData.ContainsKey((int)ChaosManager.ChaosEffects.NPC_GROW_SHRINK))
                {
                    NpcFxData.Add((int)ChaosManager.ChaosEffects.NPC_GROW_SHRINK, new byte[] { (byte)Main.rand.Next(2) });
                }
                // map 0,1 -> 1,2 -> 4,8 -> -2,2 -> -1,1
                npc.scale += 0.01f * ((NpcFxData[(int)ChaosManager.ChaosEffects.NPC_GROW_SHRINK][0] + 1) * 4 - 6) / 2;
            }
            return base.PreAI(npc);
        }



        public override void PostAI(NPC npc)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.ACCUMULATING_VELOCITY))
            {
                Vector2 velDiff = npc.velocity - npc.oldVelocity;
                npc.velocity += velDiff * 0.05f;
            }
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.DISCRETIZED_MOVEMENT))
            {
                // TODO: which of these vars do we want to store in the effect?
                // get vanilla vel
                vanillaVel = npc.velocity - effectVel;
                // remove effect vel from previous subpixel boost
                npc.velocity = vanillaVel;
                Vector2 playerPos = npc.position;
                Vector2 roundedPos = new Vector2(MathF.Round(playerPos.X / 16f) * 16, MathF.Round(playerPos.Y / 16f) * 16);
                // fix to ground
                roundedPos.Y += 6;
                subPixel += npc.position - roundedPos;
                npc.position = roundedPos;
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
                npc.velocity = vanillaVel + effectVel;
                prevPos = roundedPos;
            }
            base.PostAI(npc);
        }

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

        public override bool? CanChat(NPC npc)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.TALKING_CREATURES))
            {
                return true;
            }
            return base.CanChat(npc);
        }

        public override void GetChat(NPC npc, ref string chat)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.TALKING_CREATURES))
            {
                if (!npc.townNPC)
                {
                    chat = new string[]{ "You should stay indoors at night. It is very dangerous to be wandering around in the dark.",
                        "I wonder why my predecessor spontaneously burst into flames. Hopefully it doesn't happen to me...",
                        "It's cold and ominous around here. I feel something very evil calling out to me.",
                        "Check out my dirt blocks; they are extra dirty.", "Did you say gold?  I'll take that off of ya.",
                        "Would you like a lollipop?", "Eww... What happened to your face?", "I don't give happy endings.",
                        "It's a good day to die!", "I have noooo idea how I got here, but it's mega rad.", 
                        "Like wow, I feel like I belong with the creepy killer things running around right now. So glad I'm not that mindless.",
                        "The world is in balance.", "You are so close!", "The sands of time are flowing. And well, you are not aging very gracefully.",
                        "Why do you have to be so confrontational during a time like this?", "I know the difference between turquoise and blue-green. But I won't tell you.",
                        "If sand is causing you nightmares, your wedge is your dreamcatcher", "In this game, only losers go for that high score.",
                        "If you see any suspicious activity out in that fog, I wasn't involved.", "They say you're strong, well, I know strong. Let's see if you measure up.",
                        "What am I doing here...", "Check in with me, and do your job.", "My hands are sticky from all that... wax.",
                        "Oh you poor, poor thing. Just... just sit down here. It'll be okay. Shhhh.", "I've been cursed! Take me to the hospital!",
                        "Just-could you just... Please? Ok? Ok. Ugh.", "I don't appreciate the way you're looking at me. I am WORKING right now.",
                        "Do you want some magic candy? No? Ok.", "Are you here for a peek at my crystal ball?", "Now that we know each other, I can move in with you, right?",
                        "As if living underground wasn't bad enough, jerks like you come in while I'm sleeping and steal my children", "Ahh... nice and damp. I feel so good",
                        "This place makes me feel good. I'm not sure why...", "Stay off me booty, ya scallywag!"
                    }[Main.rand.Next(32)];
                }
            }
                
            base.GetChat(npc, ref chat);
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.SHADOW_ENEMIES))
            {
                drawColor = Color.Black;
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.INVERTED_ENEMIES))
            {
                npc.rotation = MathF.PI;
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.NPC_SPIN_2_WIN))
            {
                if(!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.NPC_SPIN_2_WIN))
                {
                    chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.NPC_SPIN_2_WIN, (byte)Main.rand.Next(255), 0);
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.NPC_SPIN_2_WIN);
                }
                byte roll = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.NPC_SPIN_2_WIN, 0);
                int value = ((npc.type + 1) * (npc.netID + 1) * roll);
                int direction = value % 2 == 0 ? 1 : -1;
                npc.rotation += direction * value / 100f;
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_NPC_SPRITES))
            {
                if(!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RANDOM_NPC_SPRITES))
                {
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RANDOM_NPC_SPRITES);
                    chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.RANDOM_NPC_SPRITES, (byte)Main.rand.Next(256), 0);
                }
                byte offset = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.RANDOM_NPC_SPRITES, 0);
                int npcTextures = TextureAssets.Npc.Length;
                Main.instance.LoadNPC((npc.type + offset) % npcTextures);
                spriteBatch.Draw(TextureAssets.Npc[(npc.type + offset) % npcTextures].Value, npc.Center - screenPos, npc.frame, drawColor, npc.rotation, npc.frame.Size() / 2, npc.scale, npc.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                return false;
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            base.ModifyIncomingHit(npc, ref modifiers);
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.SMASH_BROS))
            {
                if (!NpcFxData.ContainsKey((int)ChaosManager.ChaosEffects.SMASH_BROS))
                {
                    NpcFxData.Add((int)ChaosManager.ChaosEffects.SMASH_BROS, new byte[] { 0, 0 });
                }
                byte[] currDamageData = NpcFxData[(int)ChaosManager.ChaosEffects.SMASH_BROS];
                short currDamage = BitConverter.ToInt16(currDamageData);
                currDamage = Math.Max((short)currDamage, (short)(currDamage + damageDone / 10));
                NpcFxData[(int)ChaosManager.ChaosEffects.SMASH_BROS] = BitConverter.GetBytes(currDamage);
                hit.Knockback *= (1 + currDamage / 100f);
            }
            base.OnHitByItem(npc, player, item, hit, damageDone);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.SMASH_BROS))
            {
                if (!NpcFxData.ContainsKey((int)ChaosManager.ChaosEffects.SMASH_BROS))
                {
                    NpcFxData.Add((int)ChaosManager.ChaosEffects.SMASH_BROS, new byte[] { 0, 0 });
                }
                byte[] currDamageData = NpcFxData[(int)ChaosManager.ChaosEffects.SMASH_BROS];
                short currDamage = BitConverter.ToInt16(currDamageData);
                currDamage = Math.Max((short)currDamage, (short)(currDamage + damageDone / 10));
                NpcFxData[(int)ChaosManager.ChaosEffects.SMASH_BROS] = BitConverter.GetBytes(currDamage);
                hit.Knockback *= (1 + currDamage / 100f);
            }
            base.OnHitByProjectile(npc, projectile, hit, damageDone);
        }

        public override void OnKill(NPC npc)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RAND_ENEMY_FX_II))
            {
                int fx = Main.rand.Next(3);
                if(fx == 0) // drop explosive
                {
                    int[] explosiveType = { 
                        ProjectileID.BouncyGrenade, ProjectileID.Grenade, ProjectileID.ExplosiveBunny, ProjectileID.DynamiteKitten, ProjectileID.HappyBomb,
                        ProjectileID.DirtBomb, ProjectileID.HoneyBomb, ProjectileID.DryBomb, ProjectileID.StickyBomb, ProjectileID.WetBomb, ProjectileID.SmokeBomb,
                        ProjectileID.DryGrenade, ProjectileID.WetGrenade, ProjectileID.LavaGrenade, ProjectileID.PartyGirlGrenade, ProjectileID.Landmine, ProjectileID.Beenade,
                    };
                    Projectile.NewProjectile(new EntitySource_Death(Main.player[npc.lastInteraction]), npc.position, Vector2.Zero, Main.rand.Next(explosiveType), 30, 1f);
                }
                else if(fx == 1) // act like mother slime
                {
                    int count = Main.rand.Next(3) + 1;
                    for(int i = 0; i < count; i++)
                    {
                        NPC.NewNPC(new EntitySource_Death(npc), (int)npc.Center.X, (int)npc.Center.Y, NPCID.BabySlime);
                    }
                }
                else if (fx == 2) // like mother slime but don't drop slimes
                {
                    int count = Main.rand.Next(3) + 1;
                    for (int i = 0; i < count; i++)
                    {
                        NPC babyNPC = NPC.NewNPCDirect(new EntitySource_Death(npc), (int)npc.Center.X, (int)npc.Center.Y, NPCID.BabySlime);
                        babyNPC.scale = 0.5f;
                    }
                }
                else if(fx == 3) // like lava slime
                {
                    WorldGen.PlaceLiquid((int)npc.Center.X, (int)npc.Center.Y, (byte)LiquidID.Lava, 96);
                }
            }
            base.OnKill(npc);
        }
    }
}

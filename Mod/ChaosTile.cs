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
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using TerrariaChaosEditionUnleashed.Utility;

namespace TerrariaChaosEditionUnleashed
{
    internal class ChaosTile : GlobalTile
    {
        public List<TileOverride> tileOverrides = new List<TileOverride>();
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

        

        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.DARKNESS_ENSUES))
            {
                r = g = b = 0;
            }
            base.ModifyLight(i, j, type, ref r, ref g, ref b);
        }

        public override bool? IsTileDangerous(int i, int j, int type, Player player)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.DANGEROUS_XRAY))
            {
                return true;
            }
            return base.IsTileDangerous(i, j, type, player);
        }

        public override void RandomUpdate(int i, int j, int type)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RAND_TILE_CONVERT))
            {
                if(!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RAND_TILE_CONVERT))
                {
                    int conversionType = Main.rand.Next(3);
                    var lookupTable = new Dictionary<int, ushort[]>();
                    lookupTable.Add(TileID.Grass, new ushort[] { TileID.CorruptGrass, TileID.CrimsonGrass, TileID.HallowedGrass });
                    lookupTable.Add(TileID.Stone, new ushort[] { TileID.Ebonstone, TileID.Crimstone, TileID.Pearlstone });
                    lookupTable.Add(TileID.Sand, new ushort[] { TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand });
                    lookupTable.Add(TileID.Sandstone, new ushort[] { TileID.CorruptSandstone, TileID.CrimsonSandstone, TileID.HallowSandstone });
                    lookupTable.Add(TileID.HardenedSand, new ushort[] {TileID.CorruptHardenedSand, TileID.CrimsonHardenedSand, TileID.HallowHardenedSand });
                    lookupTable.Add(TileID.IceBlock, new ushort[] { TileID.CorruptIce, TileID.FleshIce, TileID.HallowedIce });
                    lookupTable.Add(TileID.WoodBlock, new ushort[] { TileID.Ebonwood, TileID.Shadewood, TileID.Pearlwood });
                    if (lookupTable.ContainsKey(type))
                    {
                        Main.tile[i, j].TileType = lookupTable[type][conversionType];
                    }
                }
            }
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.JUNGLE_GROWS))
            {
                if(type == TileID.JungleGrass || type == TileID.JungleThorns || type == TileID.JungleVines)
                {
                    bool retry = true;
                    while (retry)
                    {
                        retry = false;
                        int x = i + WorldGen.genRand.Next(-3, 4);
                        int y = j + WorldGen.genRand.Next(-3, 4);
                        if (Main.tile[x, y].TileType == TileID.Dirt)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                retry = true;
                            }
                            Main.tile[x, y].TileType = TileID.Mud;
                            WorldGen.SquareTileFrame(x, y, true);
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                        }
                        else if (Main.tile[x, y].TileType == TileID.Grass)
                        {
                            if (WorldGen.genRand.Next(2) == 0)
                            {
                                retry = true;
                            }
                            Main.tile[x, y].TileType = TileID.JungleGrass;
                            WorldGen.SquareTileFrame(x, y, true);
                            NetMessage.SendTileSquare(-1, x, y, 1, TileChangeType.None);
                        }
                    }
                }
            }
            base.RandomUpdate(i, j, type);
        }

        public override void DropCritterChance(int i, int j, int type, ref int wormChance, ref int grassHopperChance, ref int jungleGrubChance)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.INFINITE_BUGS))
            {
                wormChance = 3;
                grassHopperChance = 5;
                jungleGrubChance = 7;
            }
            base.DropCritterChance(i, j, type, ref wormChance, ref grassHopperChance, ref jungleGrubChance);
        }

        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if(chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RANDOM_TILE_FX))
            {
                switch(Main.rand.Next(6))
                {
                    case 5: // gravestone fx
                        int count = Main.npc.Sum(npc => npc.type == NPCID.Ghost ? 1 : 0);
                        if(count < 5)
                        {
                            NPC.SpawnOnPlayer(Main.CurrentPlayer.whoAmI, NPCID.Ghost);
                        }
                        break;
                    case 4: // lava fx
                        WorldGen.PlaceLiquid(i, j, (byte)LiquidID.Lava, 96);
                        break;
                    case 3:
                        WorldGen.PlaceLiquid(i, j, (byte)LiquidID.Honey, 96);
                        break;
                    case 2:
                        NPC.NewNPC(new EntitySource_TileBreak(i, j), i, j, NPCID.BeeSmall);
                        break;
                    case 1:
                        if(!fail)
                        {
                            noItem = true;
                            WorldGen.PlaceTile(i, j, type);
                        }
                        break;
                    default:
                        break;
                }
            }
            // do we need to undo?
            /*if(tileOverrides.ContainsKey((i,j)))
            {
                foreach(TileOverride tileOverride in tileOverrides[(i,j)])
                {
                    tileOverride.UndoOverride();
                }
            }*/
            base.KillTile(i, j, type, ref fail, ref effectOnly, ref noItem);
        }

        public override void NearbyEffects(int i, int j, int type, bool closer)
        {
            ChaosManager chaosManager = ModContent.GetInstance<ChaosSystem>().manager;
            if (chaosManager.IsEffectActive((int)ChaosManager.ChaosEffects.RECOLORED_TILES))
            {
                if(!chaosManager.IsEffectInitialDone((int)ChaosManager.ChaosEffects.RECOLORED_TILES))
                {
                    chaosManager.WriteMetaDataByte((int)ChaosManager.ChaosEffects.RECOLORED_TILES, (byte)Main.rand.Next(256), 0);
                    chaosManager.FlagEffectAsInitalDone((int)ChaosManager.ChaosEffects.RECOLORED_TILES);
                }
                // already has overrides, but not this one
                if(!tileOverrides.Any(to => to.overrideType == (int)OverrideType.COLOR && to.X == i && to.Y == j))
                {
                    TileOverride tileOverride = new TileOverride();
                    byte rand = chaosManager.ReadMetaDataByte((int)ChaosManager.ChaosEffects.RECOLORED_TILES, 0);
                    byte randColor = (byte)((rand * i * j * type) % 256);
                    tileOverride.ApplyOverrideTile((int)OverrideType.COLOR, i, j, Main.tile[i, j].TileColor, randColor);
                    tileOverrides.Add(tileOverride);
                }
            }
            /*if(tileOverrides.ContainsKey((i,j)))
            {
                foreach(TileOverride tileOverride in tileOverrides[(i, j)])
                {
                    tileOverride.UndoOverride();
                }
            }*/
            base.NearbyEffects(i, j, type, closer);
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

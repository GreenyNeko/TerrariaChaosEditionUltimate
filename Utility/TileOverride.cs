using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TerrariaChaosEditionUnleashed.Utility
{
    public enum OverrideType
    {
        COLOR,
        BOUNCY,
        REQUIRE_AXE,
        OPAQUE,
        CUTTABLE,
        REQUIRE_HAMMER,
        LAVA_DEATH,
        WATER_DEATH,
        CLIMBABLE,
        FALLS,
        SOLID,
        LIGHTSOURCE,
        PLATFORM,
        MAX,
    };

    internal class TileOverride
    {
        public int overrideType;
        public int X;
        public int Y;
        public int tileType;
        public byte oldValue;
        public byte newValue;

        public void ApplyOverrideType(int type, int tileType, byte newValue)
        {
            overrideType = type;
            this.tileType = tileType;
            this.newValue = newValue;
            switch (overrideType)
            {
                case (int)OverrideType.BOUNCY:
                    this.oldValue = Main.tileBouncy[tileType] ? (byte)1 : (byte)0;
                    Main.tileBouncy[tileType] = newValue != 0;
                    break;
                case (int)OverrideType.REQUIRE_AXE:
                    this.oldValue = Main.tileAxe[tileType] ? (byte)1 : (byte)0;
                    Main.tileAxe[tileType] = newValue != 0;
                    break;
                case (int)OverrideType.OPAQUE:
                    this.oldValue = Main.tileBlockLight[tileType] ? (byte)1 : (byte)0;
                    Main.tileBlockLight[tileType] = newValue != 0;
                    break;
                case (int)OverrideType.CUTTABLE:
                    this.oldValue = Main.tileCut[tileType] ? (byte)1 : (byte)0;
                    Main.tileCut[tileType] = newValue != 0;
                    break;
                case (int)OverrideType.REQUIRE_HAMMER:
                    this.oldValue = Main.tileHammer[tileType] ? (byte)1 : (byte)0;
                    Main.tileHammer[tileType] = newValue != 0;
                    break;
                case (int)OverrideType.LAVA_DEATH:
                    this.oldValue = Main.tileLavaDeath[tileType] ? (byte)1 : (byte)0;
                    Main.tileLavaDeath[tileType] = newValue != 0;
                    break;
                case (int)OverrideType.WATER_DEATH:
                    this.oldValue = Main.tileWaterDeath[tileType] ? (byte)1 : (byte)0;
                    Main.tileWaterDeath[tileType] = newValue != 0;
                    break;
                case (int)OverrideType.CLIMBABLE:
                    this.oldValue = Main.tileRope[tileType] ? (byte)1 : (byte)0;
                    Main.tileRope[tileType] = newValue != 0;
                    break;
                case (int)OverrideType.FALLS:
                    this.oldValue = Main.tileSand[tileType] ? (byte)1 : (byte)0;
                    Main.tileSand[tileType] = newValue != 0;
                    break;
                case (int)OverrideType.SOLID:
                    this.oldValue = Main.tileSolid[tileType] ? (byte)1 : (byte)0;
                    Main.tileSolid[tileType] = newValue != 0;
                    break;
                case (int)OverrideType.LIGHTSOURCE:
                    this.oldValue = Main.tileLighted[tileType] ? (byte)1 : (byte)0;
                    Main.tileLighted[tileType] = newValue != 0;
                    break;
                case (int)OverrideType.PLATFORM:
                    this.oldValue = Main.tileSolidTop[tileType] ? (byte)1 : (byte)0;
                    Main.tileSolidTop[tileType] = newValue != 0;
                    break;
            }
        }

        public void ApplyOverrideTile(int type, int x, int y, byte oldValue, byte newValue)
        {
            overrideType = type;
            X = x;
            Y = y;
            this.oldValue = oldValue;
            this.newValue = newValue;
            switch(overrideType)
            {
                case (int)OverrideType.COLOR:
                    Tile tile = Terraria.Main.tile[X, Y];
                    oldValue = tile.TileColor;
                    tile.TileColor = newValue;
                    break;
            }
        }

        public void UndoOverrideTile()
        {
            switch(overrideType)
            {
                case (int)OverrideType.COLOR:
                    Tile tile = Terraria.Main.tile[X, Y];
                    tile.TileColor = oldValue;
                    break;
            }
        }

        public void UndoOverrideType()
        {
            switch (overrideType)
            {
                case (int)OverrideType.BOUNCY:
                    Main.tileBouncy[tileType] = oldValue != 0;
                    break;
                case (int)OverrideType.REQUIRE_AXE:
                    Main.tileAxe[tileType] = oldValue != 0;
                    break;
                case (int)OverrideType.OPAQUE:
                    Main.tileBlockLight[tileType] = oldValue != 0;
                    break;
                case (int)OverrideType.CUTTABLE:
                    Main.tileCut[tileType] = oldValue != 0;
                    break;
                case (int)OverrideType.REQUIRE_HAMMER:
                    Main.tileHammer[tileType] = oldValue != 0;
                    break;
                case (int)OverrideType.LAVA_DEATH:
                    Main.tileLavaDeath[tileType] = oldValue != 0;
                    break;
                case (int)OverrideType.WATER_DEATH:
                    Main.tileWaterDeath[tileType] = oldValue != 0;
                    break;
                case (int)OverrideType.CLIMBABLE:
                    Main.tileRope[tileType] = oldValue != 0;
                    break;
                case (int)OverrideType.FALLS:
                    Main.tileSand[tileType] = oldValue != 0;
                    break;
                case (int)OverrideType.SOLID:
                    Main.tileSolid[tileType] = oldValue != 0;
                    break;
                case (int)OverrideType.LIGHTSOURCE:
                    Main.tileLighted[tileType] = oldValue != 0;
                    break;
                case (int)OverrideType.PLATFORM:
                    Main.tileSolidTop[tileType] = oldValue != 0;
                    break;
            }
        }
    }
}

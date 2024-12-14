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
            tileType = type;
            this.newValue = newValue;
            switch (overrideType)
            {
                case (int)OverrideType.BOUNCY:
                    this.oldValue = Main.tileBouncy[type] ? (byte)1 : (byte)0;
                    Main.tileBouncy[type] = newValue != 0;
                    break;
                case (int)OverrideType.REQUIRE_AXE:
                    this.oldValue = Main.tileAxe[type] ? (byte)1 : (byte)0;
                    Main.tileAxe[type] = newValue != 0;
                    break;
                case (int)OverrideType.OPAQUE:
                    this.oldValue = Main.tileBlockLight[type] ? (byte)1 : (byte)0;
                    Main.tileBlockLight[type] = newValue != 0;
                    break;
                case (int)OverrideType.CUTTABLE:
                    this.oldValue = Main.tileCut[type] ? (byte)1 : (byte)0;
                    Main.tileCut[type] = newValue != 0;
                    break;
                case (int)OverrideType.REQUIRE_HAMMER:
                    this.oldValue = Main.tileHammer[type] ? (byte)1 : (byte)0;
                    Main.tileHammer[type] = newValue != 0;
                    break;
                case (int)OverrideType.LAVA_DEATH:
                    this.oldValue = Main.tileLavaDeath[type] ? (byte)1 : (byte)0;
                    Main.tileLavaDeath[type] = newValue != 0;
                    break;
                case (int)OverrideType.WATER_DEATH:
                    this.oldValue = Main.tileWaterDeath[type] ? (byte)1 : (byte)0;
                    Main.tileWaterDeath[type] = newValue != 0;
                    break;
                case (int)OverrideType.CLIMBABLE:
                    this.oldValue = Main.tileRope[type] ? (byte)1 : (byte)0;
                    Main.tileRope[type] = newValue != 0;
                    break;
                case (int)OverrideType.FALLS:
                    this.oldValue = Main.tileSand[type] ? (byte)1 : (byte)0;
                    Main.tileSand[type] = newValue != 0;
                    break;
                case (int)OverrideType.SOLID:
                    this.oldValue = Main.tileSolid[type] ? (byte)1 : (byte)0;
                    Main.tileSolid[type] = newValue != 0;
                    break;
                case (int)OverrideType.LIGHTSOURCE:
                    this.oldValue = Main.tileLighted[type] ? (byte)1 : (byte)0;
                    Main.tileLighted[type] = newValue != 0;
                    break;
                case (int)OverrideType.PLATFORM:
                    this.oldValue = Main.tileSolidTop[type] ? (byte)1 : (byte)0;
                    Main.tileSolidTop[type] = newValue != 0;
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

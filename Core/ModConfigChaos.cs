using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace TerrariaChaosEditionUnleashed
{
    internal class ModConfigChaos : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public enum GameMode
        {
            CLASSIC,
            SURVIVAL,
            CHALLENGE,
            CUSTOM,
        }

        public GameMode gameMode;
        public bool ShowEffectOverHead;
        public bool ShowNextEffectInChat;
        public ConfigClassic GameModeClassic = new ConfigClassic();
        public ConfigSurvival GameModeSurvival = new ConfigSurvival();
        public ConfigChallenge GameModeChallenge = new ConfigChallenge();
        public ConfigCustom GameModeCustom = new ConfigCustom();
        [SeparatePage]
        public class ConfigClassic
        {
            /*public enum Difficulty
            {
                SAFE,       // only positive effects
                EASY,       // no "unfair" effects
                NORMAL,     // unfair effects happen rarely 
                HARD,       // unfair effects happen sometimes
                CLASSIC,    // every effect has same chance of happening
                UNFAIR,     // unfair effects happen more frequently than others
                CHAOTIC,    // focus on effects causing pure chaos
            }

            public Difficulty difficulty;*/
        }

        [SeparatePage]
        public class ConfigSurvival
        {

        }

        [SeparatePage]
        public class ConfigChallenge
        {

        }

        [SeparatePage]
        public class ConfigCustom
        {
            [Range(5f, 60f)]
            public float effectCooldown = 20f;
            [Range(0f, 30f)]
            public float effectCooldownRandomOffset = 7.5f;
            [Range(5f, 60f)]
            public float effectLength = 30f;
            [Range(0f, 30f)]
            public float effectLengthRandomOffset = 7.5f;
            public int CrazyGravityFxWeight = 100;
            public int AbominationFxWeight = 100;
            public int TimeTravelFxWeight = 100;
            public int PainShiftsRealityFxWeight = 100;
            public int HugeWorldFxWeight = 100;
            public int RandomHealthBarsFxWeight = 100;
            public int UnmissableCursorFxWeight = 100;
            public int NearsightedFxWeight = 100;
            public int EnemiesStunFxWeight = 100;
            public int UpOrDieFxWeight = 100;
            public int RandomItemDropFxWeight = 100;
        }
    }
}

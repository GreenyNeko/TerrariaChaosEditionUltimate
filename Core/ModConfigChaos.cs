using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;
using static TerrariaChaosEditionUnleashed.ModConfigChaos.ConfigClassic;

namespace TerrariaChaosEditionUnleashed
{
    internal class ModConfigChaos : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public enum GameMode
        {
            CLASSIC,
            //SURVIVAL,
            CHALLENGE,
            CUSTOM,
        }

        public GameMode gameMode;
        public bool ShowEffectOverHead;
        public bool ShowNextEffectInChat;
        public ConfigClassic GameModeClassic = new ConfigClassic();
        //public ConfigSurvival GameModeSurvival = new ConfigSurvival();
        public ConfigChallenge GameModeChallenge = new ConfigChallenge();
        public ConfigCustom GameModeCustom = new ConfigCustom();
        [SeparatePage]
        public class ConfigClassic
        {
            public enum ClassicEffectSelection
            {
                ALL,
                FEATURED
            }
            public ClassicEffectSelection classicEffectSelection;
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
            public enum ChallengeSelection
            {
                ORIGINAL,
                TELEPORTATION,
                MINIGAMES
            }
            public ChallengeSelection challengeSelection;
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

            // v1
            public int ChangeHardmodeFxWeight = 100;
            public int SkipTimeFxWeight = 100;
            public int NoGravityFxWeight = 100;
            public int RandomMusicFxWeight = 100;
            public int RandomSoundFxWeight = 100;
            public int RandomNPCFxWeight = 100;
            public int NoCreativityFxWeight = 100;
            public int RandomTeleportFxWeight = 100;
            public int LifeManaSwapFxWeight = 100;
            public int MagicMirrorFxWeight = 100;
            public int RecoloredNPCsFxWeight = 100;
            public int RandomBuffFxWeight = 100;
            public int RandomDebuffFxWeight = 100;
            public int RandomPetFxWeight = 100;
            public int HealingHurtsFxWeight = 100;
            public int RandomChatMsg = 100;
            public int RandomMonolithFxFxWeight = 100;
            public int DangerousXRayFxWeight = 100;
            public int RandTileConvert = 100;
            public int RandNPCFxFxWeight = 100;
            public int MerryXMaxFxWeight = 100;
            public int RandomEventFxWeight = 100;
            public int NoFlyingAndWormingFxWeight = 100;
            public int SmashBrosFxWeight = 100;
            //v2
            public int ImmenseSpawnRateFxWeight = 100;
            public int MaxLifeManaFxWeight = 100;
            public int ButterFingersFxWeight = 100;
            public int InfiniteBugsFxWeight = 100;
            public int ShadowEnemiesFxWeight = 100;
            public int NPCSpin2WinFxWeight = 100;
            public int UltimateBossFxWeight = 100;
            public int WorldBlessingsFxWeight = 100;
            public int TalkingCreaturesFxWeight = 100;
            public int RodOfDiscordFxWeight = 100;
            public int RandProjFxFxWeight = 100;
            public int PlayerTornadoFxWeight = 100;
            public int NPCGrowShrinkFxWeight = 100;
            public int JungleGrowsFxWeight = 100;
            public int SwapPlacesFxWeight = 100;
            public int ExplosiveDeathFxWeight = 100;
            public int RandEnemeyFxIIFxWeight = 100;
            public int SonicHealthFxWeight = 100;
            public int UpsideDownFxWeight = 100;
            public int RandomCraftFxWeight = 100;
            public int RandomTileFxfxWeight = 100;
            public int RecoloredTilesFxWeight = 100;
            public int AccumulatingVelocityFxWeight = 100;
            public int VerifyHumanFxWeight = 100;
            public int NonBinaryGenderFxWeight = 100;
            //v3
            public int RandLifeManaFxWeight = 100;
            public int CursedByLuckFxWeight = 100;
            public int BigWeaponsFxWeight = 100;
            public int ItemsGoHamFxWeight = 100;
            public int HalloHalloweenFxWeight = 100;
            public int RandNumberFxFxWeight = 100;
            public int ReinforcementsFxWeight = 100;
            public int InvertedEnemiesFxWeight = 100;
            public int AlwaysWetFxWeight = 100;
            public int DarknessEnsuesFxWeight = 100;
            public int InverseDeBuffFxWeight = 100;
            public int RandomCreditzFxWeight = 100;
            public int FakePickupFxWeight = 100;
            public int RandomAudioPitchesFxWeight = 100;
            public int AprilWeatherFxWeight = 100;
            public int SpookyGhostFxWeight = 100;
            public int OldSchoolCamFxWeight = 100;
            public int PlayerSolarSystemFxWeight = 100;
            public int ReadableUIFxWeight = 100;
            public int WeirdLightingFxWeight = 100;
            public int GraveyardShiftFxWeight = 100;
            public int RandomPylonFxFxWeight = 100;
            public int RandProjFxIIFxWeight = 100;
            public int FallSensitivityFxWeight = 100;
            public int RandomTileFxIIFxWeight = 100;
            // unleashed v1
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
            public int InvisibleInventoryItemsFxWeight = 100;
            public int RandomItemIconsFxWeight = 100;
            public int RandomNPCSpritesFxWeight = 100;
            public int ShimmeringFxWeight = 100;
            public int HotWaWaFxWeight = 100;
            public int TooltipMixUpFxWeight = 100;
            public int BreakoutFxWeight = 100;
            public int RandomTooltipFxWeight = 100;
            public int FlatrarriaFxWeight = 100;
            public int RandomBuffUIFxWeight = 100;
            //public int KowalskiAnalysisFxWeight = 100;
            public int BrightNightFxWeight = 100;
            //public int MinirarriaFxWeight = 100;
            public int SevenYearsBadLuckFxWeight = 100;
            public int DiscretizedMovementFxWeight = 100;
            public int FakeEnemiesFxWeight = 100;
            public int ChaosMusicFxWeight = 100;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TerrariaChaosEditionUnleashed
{
    /***
     * Handles active chaos effects.
     */
    class ChaosManager
    {
        int totalWeight;
        int nextEffectId;
        public enum ChaosEffects
        {
            CRAZY_GRAVITY,
            ABOMINATION,
            TIME_TRAVEL,
            PAIN_SHIFTS_REALITY,
            HUGE_WORLD,
            RANDOM_HEALTH_BARS,
            UNMISSABLE_CURSOR,
            NEARSIGHTED,
            ENEMIES_STUN,
            UP_OR_DIE,
        }

        List<ChaosEffect> allEffects;
        // reduce update
        List<ChaosEffect> activeEffects;

        public ChaosManager()
        {
            allEffects = new List<ChaosEffect>();
            activeEffects = new List<ChaosEffect>();
            allEffects.Add(new ChaosEffect("Crazy Gravity", 100));
            allEffects.Add(new ChaosEffect("Abomination", 100));
            allEffects.Add(new ChaosEffect("Time Travel", 100));
            allEffects.Add(new ChaosEffect("Pain Shifts Reality", 100));
            allEffects.Add(new ChaosEffect("Huge World", 100));
            allEffects.Add(new ChaosEffect("Random Health Bars", 100));
            allEffects.Add(new ChaosEffect("Unmissable Cursor", 100));
            allEffects.Add(new ChaosEffect("Nearsighted", 100));
            allEffects.Add(new ChaosEffect("Enemies Stun!", 100));
            allEffects.Add(new ChaosEffect("Up or die!", 100));
            totalWeight = allEffects.Count * 100;
        }

        public void ApplyWeights(ModConfigChaos.ConfigCustom configCustom)
        {
            totalWeight = 0;
            allEffects[0].weight = configCustom.CrazyGravityFxWeight;
            allEffects[1].weight = configCustom.AbominationFxWeight;
            allEffects[2].weight = configCustom.TimeTravelFxWeight;
            allEffects[3].weight = configCustom.PainShiftsRealityFxWeight;
            allEffects[4].weight = configCustom.HugeWorldFxWeight;
            allEffects[5].weight = configCustom.RandomHealthBarsFxWeight;
            allEffects[6].weight = configCustom.UnmissableCursorFxWeight;
            allEffects[7].weight = configCustom.NearsightedFxWeight;
            allEffects[8].weight = configCustom.EnemiesStunFxWeight;
            allEffects[9].weight = configCustom.UpOrDieFxWeight;
            totalWeight = allEffects.Sum(fx => fx.weight);
        }

        public void UpdateActiveEffects(double deltaTime)
        {
            // update and remove
            for(int i = 0; i < activeEffects.Count; i++)
            {
                ChaosEffect effect = activeEffects[i];
                effect.Update((float)deltaTime);
                if(!effect.IsEnabled())
                {
                    activeEffects.RemoveAt(i);
                    i--;
                }
            }
        }

        public void TriggerChaosEffect(float duration)
        {
            ChaosEffect effect = allEffects[nextEffectId];
            effect.Enable(duration);
            if(!activeEffects.Contains(effect))
            {
                activeEffects.Add(effect);
            }
        }

        public void GenerateNextChaosEffect(int gameMode)
        {
            switch(gameMode)
            {
                case (int)ModConfigChaos.GameMode.CUSTOM:
                    int roll = Main.rand.Next(totalWeight);
                    for(int i = 0; i < allEffects.Count; i++)
                    {
                        roll -= allEffects[i].weight;
                        if(roll <= 0)
                        {
                            nextEffectId = i;
                            break;
                        }
                    }
                    break;
                case (int)ModConfigChaos.GameMode.CHALLENGE:
                case (int)ModConfigChaos.GameMode.SURVIVAL:
                case (int)ModConfigChaos.GameMode.CLASSIC:
                    nextEffectId = Main.rand.Next(0, allEffects.Count);
                    break;
            }
        }

        public string GetNextChaosEffectName()
        {
            return allEffects[nextEffectId].effectName;
        }

        public bool IsEffectActive(int index)
        {
            return allEffects[index].IsEnabled();
        }

        public void FlagEffectAsInitalDone(int index)
        {
            allEffects[index].FlagInitialDone();
        }

        public bool IsEffectInitialDone(int index)
        {
            return allEffects[index].IsInitialDone();
        }

        public void WriteMetaDataByte(int fxIdx, byte data, uint offset)
        {
            allEffects[fxIdx].StoreMetaDataByte(data, offset);
        }

        public byte ReadMetaDataByte(int fxIdx, uint offset)
        {
            return allEffects[fxIdx].RetrieveMetaDataByte(offset);
        }

        public byte[] ReadMetaDataBytes(int fxIdx, uint offset, uint bytes)
        {
            return allEffects[fxIdx].RetrieveMetaDataBytes(offset, bytes);
        }

        public void WriteMetaDataBytes(int fxIdx, byte[] data, uint offset)
        {
            allEffects[fxIdx].StoreMetaDataBytes(data, offset);
        }
    }
}

using Microsoft.Build.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaChaosEditionUnleashed
{
    internal class ChaosEffect
    {
        public string effectName;
        public int weight;
        public byte[] data;

        float duration;
        bool active;
        bool initialDone;

        public ChaosEffect(string name, int weight)
        {
            effectName = name;
            this.weight = weight;
            data = new byte[128];
        }

        public bool IsEnabled()
        {
            return active || duration > 0;
        }

        public bool IsInitialDone()
        {
            return initialDone;
        }

        public void Enable(float duration=-1)
        {
            active = true;
            this.duration = duration;
            initialDone = false;
        }

        public void Disable()
        {
            active = false;
            this.duration = -1;
            initialDone = false;
        }

        public void Update(float deltaTime)
        {
            this.duration -= deltaTime;
            if(this.duration < 0)
            {
                active = false;
            }
        }

        public void FlagInitialDone()
        {
            initialDone = true;
        }

        public void StoreMetaDataByte(byte data, uint offset)
        {
            byte byteData = data;
            // overflow
            if (offset + 1 >= this.data.Length)
            {
                throw new IndexOutOfRangeException();
            }
            // write data in
            this.data[offset] = byteData;
        }

        public byte RetrieveMetaDataByte(uint offset)
        {
            // overflow
            if (offset + 1 >= data.Length)
            {
                throw new IndexOutOfRangeException();
            }
            return data[offset];
        }
    }
}

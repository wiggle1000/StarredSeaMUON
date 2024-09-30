using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.World.Objects
{
    internal class WorldObject
    {
        public DateTime creationTime;
        public long roomID = -1;
        public float weight;
        public float size;
        public float age = 0;
        public float lightEmission;
        public string name = "";
        public string description = "";
    }
}

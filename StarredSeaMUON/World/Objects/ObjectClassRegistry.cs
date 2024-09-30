using StarredSeaMUON.World.Languages;
using StarredSeaMUON.World.Objects.Animals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.World.Objects
{
    internal class ObjectClassRegistry
    {
        public static Dictionary<string, Type> OBJECT_CLASSES = new Dictionary<string, Type>();

        public static WorldObject? GetNewObj(string typeName)
        {
            if(!OBJECT_CLASSES.ContainsKey(typeName))
            {
                Logger.LogError("Attempted to load object of nonexistant type! " + typeName);
                return null;
            }
            return (WorldObject?)Activator.CreateInstance(OBJECT_CLASSES[typeName]);
        }

        private static void AddType(Type t)
        {
            if (!t.IsAssignableFrom(typeof(WorldObject)))
            {
                Logger.LogError("Attempted to register object class that doesn't inherit from WorldObject! " + t.Name);
                return;
            }
            OBJECT_CLASSES.Add(t.Name, t);
            Console.WriteLine("REGISTERED OBJECT CLASS: " + t.Name);
        }
        static ObjectClassRegistry()
        {
            AddType(typeof(WorldObject));

            AddType(typeof(Animal));
            AddType(typeof(Cat));

            AddType(typeof(Humanoid));
            AddType(typeof(Human));
        }
    }
}

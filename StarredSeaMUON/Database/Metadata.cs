using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace StarredSeaMUON.Database
{
    public class Metadata
    {
        public Dictionary<string, string> MetaList = new();

        public bool HasMeta(string name)
        {
            return MetaList.ContainsKey(name);
        }
        public string GetMeta(string name)
        {
            if (HasMeta(name))
            {
                return MetaList[name];
            }
            return "";
        }
        public void SetMeta(string name, string value)
        {
            if (HasMeta(name))
            {
                MetaList.Remove(name);
            }
            MetaList.Add(name, value);
        }
        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return MetaList.GetHashCode();
        }
    }
}

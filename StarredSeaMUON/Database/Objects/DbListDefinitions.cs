using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace StarredSeaMUON.Database.Objects
{
    public class ValueConverterListString : ValueConverter<List<String>, string>
    {
        public ValueConverterListString() : base(
            v => DBTableConversionHelper.Serialize(v),
            v => DBTableConversionHelper.Deserialize<List<string>>(v))
        { }
    }
    public class ValueConverterMetadata : ValueConverter<Metadata, string>
    {
        public ValueConverterMetadata() : base(
            v => DBTableConversionHelper.Serialize(v.MetaList),
            v => new Metadata() { MetaList = DBTableConversionHelper.Deserialize<Dictionary<string, string>>(v) })
        { }
    }
    public class ValueConverterDictStringString : ValueConverter<Dictionary<string, string>, string>
    {
        public ValueConverterDictStringString() : base(
            v => DBTableConversionHelper.Serialize(v),
            v => DBTableConversionHelper.Deserialize<Dictionary<string, string>>(v))
        { }
    }
    
}

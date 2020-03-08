using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epok.Presentation.WebApi.Models
{
    public class EnumModel
    {
        public static EnumModel New(int key, string value) => 
            new EnumModel(key, value);

        private EnumModel(int key, string value)
        {
            Key = key;
            Value = value;
        }

        public int Key { get; }
        public string Value { get; }
    }
}

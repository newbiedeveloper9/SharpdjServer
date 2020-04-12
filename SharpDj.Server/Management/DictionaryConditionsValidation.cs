using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpDj.Server.Management
{
    public class DictionaryConditionsValidation<TEnum> where TEnum:Enum
    {
        public Dictionary<TEnum, bool> Conditions { get; set; }

        public DictionaryConditionsValidation()
        {
            Conditions = new Dictionary<TEnum, bool>();
        }

        public Enum Validate()
        {
            foreach (var result in Conditions.Where(result => result.Value))
            {
                return result.Key;
            }

            return null;
        }
    }
}

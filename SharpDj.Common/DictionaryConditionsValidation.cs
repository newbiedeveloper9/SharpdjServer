using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpDj.Server.Application.Management
{
    public class DictionaryConditionsValidation<TEnum> where TEnum : Enum
    {
        public Dictionary<TEnum, bool> Conditions { get; set; }

        public DictionaryConditionsValidation()
        {
            Conditions = new Dictionary<TEnum, bool>();
        }

        public Enum AnyError()
        {
            var error = Conditions.FirstOrDefault(x => x.Value);
            if (error.Equals(default(KeyValuePair<TEnum, bool>)))
                return null;
            return error.Key;
        }
    }
}

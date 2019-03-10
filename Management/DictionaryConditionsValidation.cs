using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Management
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
            foreach (var result in Conditions)
            {
                if (!result.Value) continue;

                return result.Key;
            }
            return null;
        }
    }
}

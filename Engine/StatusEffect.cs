using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class StatusEffect
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public StatusEffect(int id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}

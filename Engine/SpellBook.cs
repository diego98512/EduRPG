using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class SpellBook
    {
        public Spell Details { get; set; }

        public int Quantity { get; set; }

        public SpellBook(Spell details, int quantity)
        {
            Details = details;
            Quantity = quantity;
        }
    }
}

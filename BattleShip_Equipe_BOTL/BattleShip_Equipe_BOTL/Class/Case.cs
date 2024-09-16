using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleShip_Equipe_BOTL.Class
{
    public class Case
    {
        public int id;
        public bool isHit { get; set; }
        public bool isBoat { get; set; }

        public Case() { }

        public Case(int id)
        {
            this.id = id;
            isBoat = false; 
            isHit = false;  
            
        }
        public Case getCase()
        {
            return this;
        }
        
        
    }
}

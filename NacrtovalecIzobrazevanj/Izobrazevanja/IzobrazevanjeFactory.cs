using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NacrtovalecIzobrazevanj
{
    internal class IzobrazevanjeFactory : IIzobrazevanjeFactory
    {
        public INovoIzobrazevanje Kreiraj()
        {
            return new Izobrazevanje();
        }
    }
}

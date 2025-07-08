using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NacrtovalecIzobrazevanj
{
    internal class TecajFactory : IIzobrazevanjeFactory
    {
        public INovoIzobrazevanje Kreiraj()
        {
            return new Tecaj();
        }
    }
}

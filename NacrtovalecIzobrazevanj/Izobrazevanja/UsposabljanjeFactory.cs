using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NacrtovalecIzobrazevanj
{
    internal class UsposabljanjeFactory : IIzobrazevanjeFactory
    {
        public INovoIzobrazevanje Kreiraj()
        {
            return new Usposabljanje();
        }
    }
}

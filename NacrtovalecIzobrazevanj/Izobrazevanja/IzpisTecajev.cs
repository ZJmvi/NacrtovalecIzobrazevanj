using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NacrtovalecIzobrazevanj
{
    internal class IzpisTecajev : IIterator<string>
    {
        private readonly List<string> SeznamTecajev;
        private int index;

        public IzpisTecajev(List<string> seznamTecajev) => SeznamTecajev = seznamTecajev;

        public string Current() => index < SeznamTecajev.Count ? SeznamTecajev[index] : null;

        public bool HasNext() => index < SeznamTecajev.Count;

        public void Next()
        {
            if (HasNext())
                index++;
        }
    }
}

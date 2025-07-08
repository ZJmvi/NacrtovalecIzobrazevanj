using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NacrtovalecIzobrazevanj
{
    internal class IzpisIzvajalcev : IIterator<Izvajalec>
    {
        private readonly List<Izvajalec> _seznamIzvajalcev;
        private int index = 0;
        public IzpisIzvajalcev(List<Izvajalec> seznamIzvajalcev) => _seznamIzvajalcev = seznamIzvajalcev;

        public Izvajalec Current() => _seznamIzvajalcev.ElementAtOrDefault(index);

        public bool HasNext() => index < _seznamIzvajalcev.Count;

        public void Next()
        {
            if (HasNext())
            {
                index++;
            }
        }
    }
}

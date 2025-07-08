using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NacrtovalecIzobrazevanj
{
    internal interface IIterator<T>
    {
        void Next();
        bool HasNext();
        T Current();
    }
}

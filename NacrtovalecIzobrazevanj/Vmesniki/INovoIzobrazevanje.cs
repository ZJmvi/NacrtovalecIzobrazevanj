using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NacrtovalecIzobrazevanj
{
    public enum PodrocjeIzobrazevanja
    {
        Strezniki,
        Omrezja,
        PodatkovneBaze,
    }
    internal interface INovoIzobrazevanje
    {
        public int KodaIzobrazevanja { get; set; }
        public string Naziv { get; set; }
        public DateOnly DatumZacetka {  get; set; }
        public int Trajanje { get; set; }
        public int KodaPodjetja { get; set; }
        public int KodaIzvajalca {  get; set; }
        public void NovoIzobrazevanje(int kodaPodjetja, int kodaIzvajalca, List<string> seznamTecajev);
        public string PopraviDatum(string datum);
        public void IzbiraPodrocja();
        public void UrediTecaj(int kodaPodjetja, int kodaIzvajalca, string kodaTecaja, List<string> seznamTecajev);
    }
}

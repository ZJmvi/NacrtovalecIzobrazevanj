using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NacrtovalecIzobrazevanj
{
    internal class Tecaji
    {
        public List<string> SeznamTecajev = new List<string>();
        private static UpravljanjeDatotek datoteka = new UpravljanjeDatotek();
        public NacrtIzobrazevanj NacrtIzobrazevanjInstanca { get; set; }
        protected Tecaji(NacrtIzobrazevanj nacrtIzobrazevanj)
        {
            this.NacrtIzobrazevanjInstanca = nacrtIzobrazevanj;
        }
        public Tecaji() { }
        public void NovoIzobrazevanje(IIzobrazevanjeFactory novoIzobrazevanje, int kodaPodjetja, int kodaIzvajalca, List<string> seznamTecajev)
        {
            novoIzobrazevanje.Kreiraj().NovoIzobrazevanje(kodaPodjetja, kodaIzvajalca, seznamTecajev);
        }

        public IIterator<string> KreirajIteratorTecajev(List<string> seznamTecajev)
        {
            return new IzpisTecajev(seznamTecajev);
        }

        public void DodajanjeTecaja(int kodaPodjetja, int kodaIzvajalca)
        {
            PopolniSeznamIzobrazevanj(kodaPodjetja, kodaIzvajalca);
            Console.Clear();
            Console.WriteLine("Dodajanje izobraževanja:");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("1 - Nov tečaj");
            Console.WriteLine("2 - Novo usposabljanje");
            Console.WriteLine("3 - Novo izobraževanje");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("N - Nazaj na osnovni meni");
            Console.WriteLine("X - Izhod\n");
            Console.Write("Vnesite vašo izbiro: ");
            string izbira = Console.ReadLine().ToLower();

            switch (izbira)
            {
                case "n":
                    if (NacrtIzobrazevanjInstanca != null)
                        NacrtIzobrazevanjInstanca.OsnovniMeni();
                    break;
                case "x":
                    Environment.Exit(0);
                    break;
                case "1":
                    NovoIzobrazevanje(new TecajFactory(), kodaPodjetja, kodaIzvajalca, SeznamTecajev);
                    DodajanjeTecaja(kodaPodjetja, kodaIzvajalca);
                    break;
                case "2":
                    NovoIzobrazevanje(new UsposabljanjeFactory(), kodaPodjetja, kodaIzvajalca, SeznamTecajev);
                    DodajanjeTecaja(kodaPodjetja, kodaIzvajalca);
                    break;
                case "3":
                    NovoIzobrazevanje(new IzobrazevanjeFactory(), kodaPodjetja, kodaIzvajalca, SeznamTecajev);
                    DodajanjeTecaja(kodaPodjetja, kodaIzvajalca);
                    break;
                default:
                    Console.WriteLine("\nNapačen vnos! Poskusite znova. Pritisni katero koli tipko za nadaljevanje.");
                    Console.ReadKey();
                    DodajanjeTecaja(kodaPodjetja, kodaIzvajalca);
                    break;
            }
        }

        public void IzpisTecajev(IIterator<string> it)
        {
            Console.WriteLine("Seznam tečajev:");
            while (true)
            {
                string trenutni = it.Current();
                if (trenutni == null) break;

                Console.WriteLine(trenutni);
                if (!it.HasNext()) break;

                it.Next();
            }
        }

        public void UrejanjeTecaja(int kodaPodjetja, int kodaIzvajalca)
        {
            PopolniSeznamIzobrazevanj(kodaPodjetja, kodaIzvajalca);

            if (SeznamTecajev.Count == 0)
            {
                Console.WriteLine("Ta izvajalec v bazi še nima vnešenih izobraževanj! Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"\nUREJANJE IZOBRAŽEVANJA:");
            Console.WriteLine("----------------------------------------");
            string koda = kodaIzvajalca.ToString();

            Console.WriteLine("Koda izobraževanja; Koda podjetja; Koda izvajalca; Tip izobraževanja; Naziv; Datum začetka; Trajanje; Področje; Vrsta izobraževanja");
            var poIzvajalcih = SeznamTecajev
                .Where(vrstica =>
                {
                    var razdeli = vrstica.Split(";");
                    return razdeli.Length >= 3 && razdeli[2].Trim()  == koda;
                }).ToList();

            // Implementacija Behavioral Design Patterna Iterator
            var iterator = KreirajIteratorTecajev(poIzvajalcih);
            IzpisTecajev(iterator);

            Console.WriteLine("----------------------------------------");
            Console.Write("Vpišite kodo izobraževanja, ki ga želite urediti: ");
            string izbira = Console.ReadLine();

            var izbraniTecaj = SeznamTecajev
                .Where(vrsta =>
                {
                    var razdeli = vrsta.Split(";");
                    return razdeli.Length >= 3 && razdeli[0].Trim() == izbira;
                }).ToList();

            foreach (var vrstica in izbraniTecaj)
            {
                var deli = vrstica.Split(";");
                string vrstaIzobrazevanja = deli[3].Trim();

                switch(vrstaIzobrazevanja)
                {
                    case "Tečaj":
                        Tecaj tecaj = new Tecaj();
                        tecaj.UrediTecaj(kodaPodjetja, kodaIzvajalca, izbira, SeznamTecajev);
                        if (NacrtIzobrazevanjInstanca != null)
                            NacrtIzobrazevanjInstanca.OsnovniMeni();
                        break;
                    case "Usposabljanje":
                        Usposabljanje usposabljanje = new Usposabljanje();
                        usposabljanje.UrediTecaj(kodaPodjetja, kodaIzvajalca, izbira, SeznamTecajev);
                        if (NacrtIzobrazevanjInstanca != null)
                            NacrtIzobrazevanjInstanca.OsnovniMeni();
                        break;
                    case "Izobraževanje":
                        Izobrazevanje izobrazevanje = new Izobrazevanje();
                        izobrazevanje.UrediTecaj(kodaPodjetja, kodaIzvajalca, izbira, SeznamTecajev);
                        if (NacrtIzobrazevanjInstanca != null)
                            NacrtIzobrazevanjInstanca.OsnovniMeni();
                        break;
                    default:
                        Console.WriteLine("Napačna izbira! Poskusite ponovno. Za nadaljevanje pritisnite katero koli tipko.");
                        Console.ReadKey();
                        if (NacrtIzobrazevanjInstanca != null)
                            NacrtIzobrazevanjInstanca.OsnovniMeni();
                        break;
                }
            }
        }
        public void PopolniSeznamIzobrazevanj(int kodaPodjetja, int kodaIzvajalca)
        {
            string pot = LokalnaPot(kodaPodjetja);
            if (SeznamTecajev.Count == 0)
            {
                if (File.Exists(pot))
                {
                    PreberiIzobrazevanjaIzBaze(kodaPodjetja, kodaIzvajalca);
                }
            }
        }

        private void PreberiIzobrazevanjaIzBaze(int kodaPodjetja, int kodaIzvajalca)
        {
            string pot = LokalnaPot(kodaPodjetja);
            var preberiIzDatoteke = datoteka.IzpisIzDatoteke(pot);
            
            string str = preberiIzDatoteke.Data;

            foreach (string vrstica in str.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                string[] razdeli = vrstica.Split(";");
                if (razdeli.Length == 9 && int.TryParse(razdeli[0], out int kodaIzobrazevanja))
                {
                    int podjetje = int.Parse(razdeli[1]);
                    int izvajalec = int.Parse(razdeli[2]);
                    string izobrazevanje = razdeli[3];
                    string naziv = razdeli[4];
                    string datum = razdeli[5];
                    string popravljenDatum = PopraviDatum(datum);
                    int trajanje = int.Parse(razdeli[6]);
                    string podrocje = razdeli[7];
                    string vrstaTecaja = razdeli[8];

                    string tecaj =$"{kodaIzobrazevanja};{podjetje};{izvajalec};{izobrazevanje};{naziv};{popravljenDatum};{trajanje};{podrocje};{vrstaTecaja}";

                    SeznamTecajev.Add(tecaj);
                }
            }
        }
        public string PopraviDatum(string datum)
        {
            string[] razdeli = datum.Split(".");

            string dan = int.Parse(razdeli[0].Trim()).ToString("D2");
            string mesec = int.Parse(razdeli[1].Trim()).ToString("D2");
            string leto = razdeli[2].Trim();
            return $"{dan}. {mesec}. {leto}";
        }
        public void ShraniIzobrazevanja(int kodaPodjetja, int kodaIzvajalca, List<string> seznamTecajev)
        {
            string pot = LokalnaPot(kodaPodjetja);
            File.Delete(pot);

            if (seznamTecajev == null || seznamTecajev.Count == 0)
            {
                Console.WriteLine("Seznam je prazen! Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
            }

            foreach (var vrstica in seznamTecajev)
            {
                var zapis = new PodatkovnaDatoteka<string>(pot, vrstica);
                datoteka.ShraniVDatoteko(zapis);
            }
        }
        private string LokalnaPot(int kodaPodjetja)
        {
            return datoteka.PotDoDatotek($"Izobrazevanja", $"{kodaPodjetja}_SeznamIzobrazevanj.pdb");
        }
    }
}

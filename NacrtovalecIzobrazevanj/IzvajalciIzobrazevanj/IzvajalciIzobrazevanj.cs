using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NacrtovalecIzobrazevanj
{
    internal abstract class IzvajalciIzobrazevanj
    {
        public int KodaIzvajalca {  get; set; }
        public string NazivIzvajalca {  set; get; }
        public Dictionary<int, Tecaji> SeznamTecajev { get; set; }
        public List<Izvajalec> SeznamIzvajalcev { get; set; } = new List<Izvajalec>();
        private static UpravljanjeDatotek datoteka = new UpravljanjeDatotek();
        public NacrtIzobrazevanj NacrtIzobrazevanjInstanca { get; set; }
        public Tecaji tecaji;
        
        protected IzvajalciIzobrazevanj(NacrtIzobrazevanj nacrtIzobrazevanj)
        {
            this.NacrtIzobrazevanjInstanca = nacrtIzobrazevanj;
        }
        public IzvajalciIzobrazevanj() { }
        public abstract IIterator<Izvajalec> KreirajIteratorIzvajalcev(List<Izvajalec> seznam);

        public void MeniIzvajalcev(int kodaPodjetja)
        {
            PopolniSeznamIzvajalcev(kodaPodjetja);
            Console.Clear();
            Console.WriteLine($"IZVAJALCI IZOBRAŽEVANJ:");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("1 - Dodajanje novega izvajalca");
            Console.WriteLine("2 - Urejanje izvajalcev");
            Console.WriteLine("3 - Dodajanje izobraževanja");
            Console.WriteLine("4 - Urejanje izobraževanj");
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
                    Izvajalec izvajalec = new Izvajalec(NacrtIzobrazevanjInstanca);
                    izvajalec.DodajIzvajalca(kodaPodjetja, SeznamIzvajalcev);
                    MeniIzvajalcev(kodaPodjetja);
                    break;
                case "2":
                    Izvajalec novIzvajalec = new Izvajalec(NacrtIzobrazevanjInstanca);
                    novIzvajalec.UrejanjeIzvajalcev(kodaPodjetja, SeznamIzvajalcev);
                    MeniIzvajalcev(kodaPodjetja);
                    break;
                case "3":
                    tecaji = new Tecaji();
                    KodaIzvajalca = IzbiraIzvajalca(kodaPodjetja, SeznamIzvajalcev);
                    tecaji.DodajanjeTecaja(kodaPodjetja, KodaIzvajalca);
                    MeniIzvajalcev(kodaPodjetja);
                    break;
                case "4":
                    tecaji = new Tecaji();
                    KodaIzvajalca = IzbiraIzvajalca(kodaPodjetja, SeznamIzvajalcev);
                    tecaji.UrejanjeTecaja(kodaPodjetja, KodaIzvajalca);
                    MeniIzvajalcev(kodaPodjetja);
                    break;
                default:
                    Console.WriteLine("\nNapačen vnos! Poskusite znova. Pritisni katero koli tipko za nadaljevanje.");
                    Console.ReadKey();
                    MeniIzvajalcev(kodaPodjetja);
                    break;
            }
        }

        public void PopolniSeznamIzvajalcev(int kodaPodjetja)
        {
            string pot = LokalnaPot(kodaPodjetja);
            if (SeznamIzvajalcev.Count == 0)
            {
                if (File.Exists(pot))
                    PreberiIzvajalceIzBaze(kodaPodjetja);
            }
        }

        private void PreberiIzvajalceIzBaze(int kodaPodjetja)
        {
            string pot = LokalnaPot(kodaPodjetja);
            var preberiIzDatoteke = datoteka.IzpisIzDatoteke(pot);
            string str = preberiIzDatoteke.Data;

            Console.WriteLine("Prebrano iz datoteke:");
            foreach (string vrstica in str.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                string[] razdeli = vrstica.Split(";");
                if (razdeli.Length == 4 && int.TryParse(razdeli[0], out int kodaIzvajalca))
                {
                    string naziv = razdeli[1];
                    string oddelek = razdeli[2];
                    string status = razdeli[3];

                    Izvajalec izvajalec = new Izvajalec(NacrtIzobrazevanjInstanca);
                    izvajalec.KodaIzvajalca = kodaIzvajalca;
                    izvajalec.NazivIzvajalca = naziv;
                    izvajalec.Oddelek = oddelek;
                    izvajalec.Status = status == "Zunanji" ? StatusIzvajalca.Zunanji : StatusIzvajalca.Notranji;

                    SeznamIzvajalcev.Add(izvajalec);
                }
            }
        }

        public int IzbiraIzvajalca(int kodaPodjetja, List<Izvajalec> seznamIzvajalcev)
        {
            Console.Clear();
            Console.WriteLine("IZBIRA IZVAJALCA IZOBRAŽEVANJ:");
            Console.WriteLine("----------------------------------------");

            if (seznamIzvajalcev.Count == 0)
            {
                Console.WriteLine("Za izbrano podjetje še ni izdelan seznam izvajalcev izobraževanj. Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                MeniIzvajalcev(kodaPodjetja);
            }

            Console.WriteLine("Koda\t Naziv\t Oddelek/Naslov\t Status");

            // Implementacija Behavioral Design Patterna Iterator
            var iterator = KreirajIteratorIzvajalcev(seznamIzvajalcev);
            while (iterator.HasNext())
            {
                Console.WriteLine($"{iterator.Current().KodaIzvajalca}, {iterator.Current().NazivIzvajalca}, {iterator.Current().Oddelek}, {iterator.Current().Status}");
                iterator.Next();
            }

            Console.WriteLine("----------------------------------------");
            Console.Write("\nVpišite kodo izvajalca: ");
            string koda = Console.ReadLine();

            bool isNum = int.TryParse(koda, out int kodaIzvajalca);
            if (!isNum)
            {
                Console.WriteLine("\nNapačen vnos. Poskusite znova.");
                Console.ReadKey();
                IzbiraIzvajalca(kodaPodjetja, seznamIzvajalcev);
            }

            return kodaIzvajalca;
        }
        private string LokalnaPot(int kodaPodjetja)
        {
            return datoteka.PotDoDatotek($"Izvajalci", $"{kodaPodjetja}_SeznamIzvajalcev.pdb");
        }
    }
}


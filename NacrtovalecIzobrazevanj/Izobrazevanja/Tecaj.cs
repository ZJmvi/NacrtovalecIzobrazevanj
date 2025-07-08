using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace NacrtovalecIzobrazevanj
{
    public enum VrsteTecajev
    {
        Funkcionalni,
        Strokovni,
        Dopolnilni
    }
    internal class Tecaj : INovoIzobrazevanje
    {
        public int KodaIzobrazevanja { get; set; }
        public int KodaPodjetja {  get; set; }
        public int KodaIzvajalca {  get; set; }
        public string Naziv {  get; set; }
        public DateOnly DatumZacetka { get; set; }
        public int Trajanje { get; set; }
        public PodrocjeIzobrazevanja Podrocje { get; set; }
        public VrsteTecajev VrstaTecaja { get; set; }

        public void NovoIzobrazevanje(int kodaPodjetja, int kodaIzvajalca, List<string> seznamTecajev)
        {
            KodaIzobrazevanja = IdGenerator.GetId.GenerirajKodoIzobrazevanja(kodaPodjetja, seznamTecajev);
            KodaPodjetja = kodaPodjetja;
            KodaIzvajalca = kodaIzvajalca;

            Console.WriteLine("\nDodajanje novega tečaja:");
            Console.WriteLine("----------------------------------------");
            Console.Write("Vpišite naziv tečaja: ");
            Naziv = Console.ReadLine();

            Console.Write("Vpišite datum začetka tečaja (primer: 01/04/2025): ");
            string datum = Console.ReadLine();
            datum = PopraviDatum(datum);
            if (!DateOnly.TryParseExact(datum, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly datumZacetka))
            {
                Console.WriteLine("Napačen format datuma! Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                return;
            }
            DatumZacetka = datumZacetka;

            Console.Write("Vpišite čas trajanja tečaja v dnevih: ");
            if (!int.TryParse(Console.ReadLine(), out int trajanje))
            {
                Console.WriteLine("Napačen vnos trajanja! Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                return;
            }
            Trajanje = trajanje;

            IzbiraPodrocja();

            IzbiraVrsteTecaja();

            string tecaj = KodaIzobrazevanja + ";" + KodaPodjetja + ";" + KodaIzvajalca + ";Tečaj;" + Naziv + ";" + DatumZacetka + ";" + Trajanje + ";" + Podrocje + ";" + VrstaTecaja;

            seznamTecajev.Add(tecaj);

            Tecaji tecaji = new Tecaji();
            tecaji.ShraniIzobrazevanja(kodaPodjetja, kodaIzvajalca, seznamTecajev);
        }

        public string PopraviDatum(string datum)
        {
            string[] razdeli = datum.Split(new[] { "/", "." }, StringSplitOptions.None);

            string dan = int.Parse(razdeli[0].Trim()).ToString("D2");
            string mesec = int.Parse(razdeli[1].Trim()).ToString("D2");
            string leto = razdeli[2].Trim();
            return $"{dan}/{mesec}/{leto}";
        }

        public void IzbiraPodrocja()
        {
            Console.WriteLine("\nPodročje izobraževanja:\n1 - Strežniki\n2 - Omrežja\n3 - Podatkovne baze");
            Console.WriteLine("----------------------------------------");
            Console.Write("Vpišite vašo izbiro: ");
            string izbira = Console.ReadLine();

            switch (izbira)
            {
                case "1":
                    Podrocje = PodrocjeIzobrazevanja.Strezniki;
                    break;
                case "2":
                    Podrocje = PodrocjeIzobrazevanja.Omrezja;
                    break;
                case "3":
                    Podrocje = PodrocjeIzobrazevanja.PodatkovneBaze;
                    break;
                default:
                    Console.WriteLine("Napačen izbor področja! Za nadaljevanje pritisnite katerokoli tipko");
                    Console.WriteLine();
                    IzbiraPodrocja();
                    break;
            }
        }

        public void IzbiraVrsteTecaja()
        {
            Console.WriteLine("\nVrsta tečaja:\n1 - Funkcionalni tečaj\n2 - Strokovni tečaj\n3 - Dopolnilni tečaj");
            Console.WriteLine("----------------------------------------");
            Console.Write("Vpišite vašo izbiro: ");
            string vrsta = Console.ReadLine();
            
            switch (vrsta)
            {
                case "1":
                    VrstaTecaja = VrsteTecajev.Funkcionalni;
                    break;
                case "2":
                    VrstaTecaja = VrsteTecajev.Strokovni;
                    break;
                case "3":
                    VrstaTecaja = VrsteTecajev.Dopolnilni;
                    break;
                default:
                    Console.WriteLine("Napačen izbor področja! Za nadaljevanje pritisnite katerokoli tipko");
                    Console.WriteLine();
                    IzbiraVrsteTecaja();
                    break;
            }
        }

        public void UrediTecaj(int kodaPodjetja, int kodaIzvajalca, string kodaTecaja, List<string> seznamTecajev)
        {
            string izbira = PoisciTecaj(kodaTecaja, seznamTecajev);

            Console.WriteLine($"\nIzbrani tečaj:\n{izbira}");
            Console.Write("\nŽelte spremeniti (n)aziv, (d)atum začetka, (t)rajanje, (p)odročje, (v)rsto tečaja ali zelte (b)risati tečaj? ");
            string str = Console.ReadLine().ToLower();
            
            switch (str)
            {
                case "n":
                    Console.Write("Vnesite nov naziv tečaja: ");
                    string novNaziv = Console.ReadLine();
                    SpremeniTecaj(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, novNaziv, "n");
                    break;
                case "d":
                    Console.Write("Vpišite datum začetka tečaja (primer: 01/04/2025): ");
                    string datum = Console.ReadLine();
                    SpremeniTecaj(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, datum, "d");
                    break;
                case "t":
                    Console.Write("Vpišite čas trajanja tečaja v dnevih: ");
                    string trajanje = Console.ReadLine();
                    SpremeniTecaj(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, trajanje, "t");
                    break;
                case "p":
                    Console.WriteLine("\nPodročje izobraževanja:\n1 - Strežniki\n2 - Omrežja\n3 - Podatkovne baze");
                    Console.Write("Vnesite vašo izbiro: ");
                    string podrocje = Console.ReadLine();
                    if (podrocje == "1")
                        podrocje = "Strežniki";
                    else if (podrocje == "2")
                        podrocje = "Omrežja";
                    else if (podrocje == "3")
                        podrocje = "Podatkovne baze";
                    else
                    {
                        Console.WriteLine("Napačen vnos! Za nadaljevanje pritisnite katerokoli tipko.");
                        Console.ReadKey();
                        UrediTecaj(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev);
                    }
                    SpremeniTecaj(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, podrocje, "p");
                    break;
                case "v":
                    Console.WriteLine("\nVrsta tečaja:\n1 - Funkcionalni tečaj\n2 - Strokovni tečaj\n3 - Dopolnilni tečaj");
                    Console.Write("Vnesite vašo izbiro: ");
                    string vrsta = Console.ReadLine();
                    if (vrsta == "1")
                        vrsta = "Funkcionalni";
                    else if (vrsta == "2")
                        vrsta = "Strokovni";
                    else if (vrsta == "3")
                        vrsta = "Dopolnilni";
                    else
                    {
                        Console.WriteLine("Napačen vnos! Za nadaljevanje pritisnite katerokoli tipko.");
                        Console.ReadKey();
                        UrediTecaj(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev);
                    }
                    SpremeniTecaj(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, vrsta, "v");
                    break;
                case "b":
                    seznamTecajev.Remove(izbira);
                    Tecaji tecaji = new Tecaji();
                    tecaji.ShraniIzobrazevanja(kodaPodjetja, kodaIzvajalca, seznamTecajev);
                    break;
                default:
                    Console.WriteLine("Napačna izbira! Za nadaljevanje pritisnite katero koli tipko.");
                    Console.ReadKey();
                    UrediTecaj(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev);
                    break;
            }
        }

        public string PoisciTecaj(string kodaTecaja, List<string> seznamTecajev)
        {
            string izbira = "";
            var izbraniTecaj = seznamTecajev
                .Where(vrsta =>
                {
                    var razdeli = vrsta.Split(";");
                    return razdeli.Length >= 3 && razdeli[0].Trim() == kodaTecaja;
                }).ToList();

            foreach (var vrstica in izbraniTecaj)
            {
                izbira = vrstica.Trim();
            }

            return izbira;
        }

        public void SpremeniTecaj(int kodaPodjetja, int kodaIzvajalca, string kodaTecaja, List<string> seznamTecajev, string sprememba, string spr)
        {
            string vrstica = PoisciTecaj(kodaTecaja, seznamTecajev);
            if (string.IsNullOrWhiteSpace(vrstica))
            {
                Console.WriteLine("Tečaj s podano kodo ni bil najden! Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                SpremeniTecaj(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, sprememba, spr);
            }

            var izbira = vrstica.Split(";");
            if (izbira.Length < 9)
            {
                Console.WriteLine("Neveljaven zapis tečaja! Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                SpremeniTecaj(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, sprememba, spr);
            }

            KodaIzobrazevanja = int.Parse(izbira[0]);
            KodaPodjetja = int.Parse(izbira[1]);
            KodaIzvajalca = int.Parse(izbira[2]);

            Naziv = (spr == "n") ? sprememba : izbira[4];

            string datum = (spr == "d") ? sprememba : izbira[5].ToString();
            datum = PopraviDatum(datum);
            if (!DateOnly.TryParseExact(datum, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly datumZacetka))
            {
                Console.WriteLine("Napačen format datuma! Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                return;
            }
            DatumZacetka = datumZacetka;
            
            Trajanje = (spr == "t") ? int.Parse(sprememba) :  int.Parse(izbira[6]);

            string podrocje = (spr == "p") ? sprememba : izbira[07]; 

            if (podrocje == "Strežniki")
                Podrocje = PodrocjeIzobrazevanja.Strezniki;
            else if (podrocje == "Omrežja")
                Podrocje = PodrocjeIzobrazevanja.Omrezja;
            else if (podrocje == "Podatkovne baze")
                Podrocje = PodrocjeIzobrazevanja.PodatkovneBaze;
            

            string vrsta = (spr == "v") ? sprememba : izbira[8];

            if (vrsta == "Funkcionalni")
                VrstaTecaja = VrsteTecajev.Funkcionalni;
            else if (vrsta == "Strokovni")
                VrstaTecaja = VrsteTecajev.Strokovni;
            else if (vrsta == "Dopolnilni")
                VrstaTecaja = VrsteTecajev.Dopolnilni;

            string tecaj = KodaIzobrazevanja + ";" + KodaPodjetja + ";" + KodaIzvajalca + ";Tečaj;" + Naziv + ";" + DatumZacetka + ";" + Trajanje + ";" + Podrocje + ";" + VrstaTecaja;

            int indeks = seznamTecajev.FindIndex(i => i.StartsWith(kodaTecaja + ";"));
            if (indeks != -1)
            {
                seznamTecajev[indeks] = tecaj;
            }
            else
            {
                Console.WriteLine("Tečaj ni bil najden v seznamu. Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                SpremeniTecaj(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, sprememba, spr); ;
            }

            Tecaji tecaji = new Tecaji();
            tecaji.ShraniIzobrazevanja(kodaPodjetja, kodaIzvajalca, seznamTecajev);

            Console.WriteLine($"Spremenjen tečaj:\n{tecaj}");
            Console.Write("\nZa nadaljevanje pritisnite katerokoli tipko.");
            Console.ReadKey();
        }
    }
}

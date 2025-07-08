using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NacrtovalecIzobrazevanj
{
    public enum VrsteIzobrazevanj
    {
        Karierno,
        Formalno,
        Neformalno
    }
    internal class Izobrazevanje : INovoIzobrazevanje
    {
        public int KodaIzobrazevanja { get; set; }
        public int KodaPodjetja { get; set; }
        public int KodaIzvajalca {  get; set; }
        public string Naziv { get; set; }
        public DateOnly DatumZacetka { get; set; }
        public int Trajanje { get; set; }
        public PodrocjeIzobrazevanja Podrocje { get; set; }
        public VrsteIzobrazevanj VrsteIzobrazevanja { get; set; }

        public void NovoIzobrazevanje(int kodaPodjetja, int kodaIzvajalca, List<string> seznamTecajev)
        {
            KodaIzobrazevanja = IdGenerator.GetId.GenerirajKodoIzobrazevanja(kodaPodjetja, seznamTecajev);
            KodaPodjetja = kodaPodjetja;
            KodaIzvajalca = kodaIzvajalca;

            Console.WriteLine("\nDodajanje novega izobraževanja:");
            Console.WriteLine("----------------------------------------");
            Console.Write("Vpišite naziv izobraževanja: ");
            Naziv = Console.ReadLine();

            Console.Write("Vpišite datum začetka izobraževanja (primer: 01/04/2025): ");
            string datum = Console.ReadLine();
            datum = PopraviDatum(datum);
            if (!DateOnly.TryParseExact(datum, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly datumZacetka))
            {
                Console.WriteLine("Napačen format datuma! Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                return;
            }
            DatumZacetka = datumZacetka;
            string dat = DatumZacetka.ToString();

            Console.Write("Vpišite čas trajanja tečaja v dnevih: ");
            if (!int.TryParse(Console.ReadLine(), out int trajanje))
            {
                Console.WriteLine("Napačen vnos trajanja! Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                return;
            }
            Trajanje = trajanje;
            

            IzbiraPodrocja();

            IzbiraVrsteIzobrazevanja();

            string tecaj = KodaIzobrazevanja + ";" + KodaPodjetja + ";" + KodaIzvajalca + ";Izobraževanje;" + Naziv + ";" + DatumZacetka + ";" + Trajanje + ";" + Podrocje + ";" + VrsteIzobrazevanja;

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

        public void IzbiraVrsteIzobrazevanja()
        {
            Console.WriteLine("\nVrsta izobraževanja:\n1 - Karierno\n2 - Formalno\n3 - Neformalno");
            Console.WriteLine("----------------------------------------");
            Console.Write("Vpišite vašo izbiro: ");
            string vrsta = Console.ReadLine();
            switch (vrsta)
            {
                case "1":
                    VrsteIzobrazevanja = VrsteIzobrazevanj.Karierno;
                    break;
                case "2":
                    VrsteIzobrazevanja = VrsteIzobrazevanj.Formalno;
                    break;
                case "3":
                    VrsteIzobrazevanja = VrsteIzobrazevanj.Neformalno;
                    break;
                default:
                    Console.WriteLine("Napačen izbor področja! Za nadaljevanje pritisnite katerokoli tipko");
                    Console.WriteLine();
                    IzbiraVrsteIzobrazevanja();
                    break;
            }
        }

        public void UrediTecaj(int kodaPodjetja, int kodaIzvajalca, string kodaTecaja, List<string> seznamTecajev)
        {
            string izbira = PoisciTecaj(kodaTecaja, seznamTecajev);

            Console.WriteLine($"\nIzbrano izobraževanje:\n{izbira}");
            Console.Write("\nŽelte spremeniti (n)aziv, (d)atum začetka, (t)rajanje, (p)odročje, (v)rsto izobraževanja ali zelte (b)risati izobraževanje? ");
            string str = Console.ReadLine().ToLower();
            
            switch (str)
            {
                case "n":
                    Console.Write("Vnesite nov naziv tečaja: ");
                    string novNaziv = Console.ReadLine();
                    SpremeniIzobraževanje(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, novNaziv, "n");
                    break;
                case "d":
                    Console.Write("Vpišite datum začetka tečaja (primer: 01/04/2025): ");
                    string datum = Console.ReadLine();
                    SpremeniIzobraževanje(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, datum, "d");
                    break;
                case "t":
                    Console.Write("Vpišite čas trajanja tečaja v dnevih: ");
                    string trajanje = Console.ReadLine();
                    SpremeniIzobraževanje(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, trajanje, "t");
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
                    SpremeniIzobraževanje(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, podrocje, "p");
                    break;
                case "v":
                    Console.WriteLine("\nVrsta izobraževanja:\n1 - Karierno\n2 - Formalno\n3 - Neformalno");
                    Console.Write("Vnesite vašo izbiro: ");
                    string vrsta = Console.ReadLine();
                    if (vrsta == "1")
                        vrsta = "Karierno";
                    else if (vrsta == "2")
                        vrsta = "Formalno";
                    else if (vrsta == "3")
                        vrsta = "Neformalno";
                    else
                    {
                        Console.WriteLine("Napačen vnos! Za nadaljevanje pritisnite katerokoli tipko.");
                        Console.ReadKey();
                        UrediTecaj(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev);
                    }
                    SpremeniIzobraževanje(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, vrsta, "v");
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

        public void SpremeniIzobraževanje(int kodaPodjetja, int kodaIzvajalca, string kodaTecaja, List<string> seznamTecajev, string sprememba, string spr)
        {
            string vrstica = PoisciTecaj(kodaTecaja, seznamTecajev);
            if (string.IsNullOrWhiteSpace(vrstica))
            {
                Console.WriteLine("Tečaj s podano kodo ni bil najden! Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                SpremeniIzobraževanje(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, sprememba, spr);
            }

            var izbira = vrstica.Split(";");
            if (izbira.Length < 9)
            {
                Console.WriteLine("Neveljaven zapis izobraževanja! Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                SpremeniIzobraževanje(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, sprememba, spr);
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

            Trajanje = (spr == "t") ? int.Parse(sprememba) : int.Parse(izbira[6]);

            string podrocje = (spr == "p") ? sprememba : izbira[07];

            if (podrocje == "Strežniki")
                Podrocje = PodrocjeIzobrazevanja.Strezniki;
            else if (podrocje == "Omrežja")
                Podrocje = PodrocjeIzobrazevanja.Omrezja;
            else if (podrocje == "Podatkovne baze")
                Podrocje = PodrocjeIzobrazevanja.PodatkovneBaze;


            string vrsta = (spr == "v") ? sprememba : izbira[8];

            if (vrsta == "Karierno")
                VrsteIzobrazevanja = VrsteIzobrazevanj.Karierno;
            else if (vrsta == "Formalno")
                VrsteIzobrazevanja = VrsteIzobrazevanj.Formalno;
            else if (vrsta == "Neformalno")
                VrsteIzobrazevanja = VrsteIzobrazevanj.Neformalno;

            string tecaj = KodaIzobrazevanja + ";" + KodaPodjetja + ";" + KodaIzvajalca + ";Izobraževanje;" + Naziv + ";" + DatumZacetka + ";" + Trajanje + ";" + Podrocje + ";" + VrsteIzobrazevanja;

            int indeks = seznamTecajev.FindIndex(i => i.StartsWith(kodaTecaja + ";"));
            if (indeks != -1)
            {
                seznamTecajev[indeks] = tecaj;
            }
            else
            {
                Console.WriteLine("Izobraževanje ni bil najdeno v seznamu. Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                SpremeniIzobraževanje(kodaPodjetja, kodaIzvajalca, kodaTecaja, seznamTecajev, sprememba, spr); ;
            }

            Tecaji tecaji = new Tecaji();
            tecaji.ShraniIzobrazevanja(kodaPodjetja, kodaIzvajalca, seznamTecajev);

            Console.WriteLine($"Spremenjeno izobraževanje:\n{tecaj}");
            Console.Write("\nZa nadaljevanje pritisnite katerokoli tipko.");
            Console.ReadKey();
        }
    }
}

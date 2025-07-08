using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace NacrtovalecIzobrazevanj
{
    internal class Zaposleni
    {
        public int KodaZaposlenega { get; set; }
        public string Ime { get; set; }
        public string Oddelek { get; set; }
        public List<Zaposleni> SeznamZaposlenih { get; set; } = new List<Zaposleni>();
        private static UpravljanjeDatotek datoteka = new UpravljanjeDatotek();
        private NacrtIzobrazevanj NacrtIzobrazevanj;

        public Zaposleni() { }

        public Zaposleni(NacrtIzobrazevanj nacrtIzobrazevanj)
        {
            this.NacrtIzobrazevanj = nacrtIzobrazevanj;
        }

        public void MeniZaposleni(int kodaPodjetja)
        {
            PopolniSeznam(kodaPodjetja);
            Console.Clear();
            Console.WriteLine($"Urejanje zaposlenih:");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("1 - Dodajanje novega zaposlenega");
            Console.WriteLine("2 - Ogled in urejanje zaposlenih");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("N - Nazaj na glavni meni");
            Console.WriteLine("X - Izhod\n");
            Console.Write("Vnesite vašo izbiro: ");
            string izbira = Console.ReadLine().ToLower();

            switch (izbira)
            {
                case "n":
                    NacrtIzobrazevanj.OsnovniMeni();
                    break;
                case "x":
                    Environment.Exit(0);
                    break;
                case "1":
                    DodajZaposlenega(kodaPodjetja);
                    MeniZaposleni(kodaPodjetja);
                    break;
                case "2":
                    OgledZaposlenega(kodaPodjetja);
                    MeniZaposleni(kodaPodjetja);
                    break;
                default:
                    Console.WriteLine("\nNapačen vnos! Poskusite znova. Pritisni katero koli tipko za nadaljevanje.");
                    Console.ReadKey();
                    MeniZaposleni(kodaPodjetja);
                    break;
            }
        }

        public void DodajZaposlenega(int kodaPodjetja)
        {
            string pot = LokalnaPot(kodaPodjetja);
            Console.Clear();
            Console.WriteLine("DODAJANJE ZAPOSLENEGA:");
            Console.WriteLine("----------------------------------------");
            Console.Write("Vpišite ime zaposlenega: ");
            string ime = Console.ReadLine();
            Console.Write("Vpišite oddelek zaposlenega: ");
            string oddelek = Console.ReadLine();
            int kodaZaposlenega = IdGenerator.GetId.GenerirajKodoZaposlenega(kodaPodjetja, SeznamZaposlenih);

            Zaposleni zaposleni = new Zaposleni();
            zaposleni.KodaZaposlenega = kodaZaposlenega;
            zaposleni.Ime = ime;
            zaposleni.Oddelek = oddelek;
            
            SeznamZaposlenih.Add(zaposleni);

            var zapis = new PodatkovnaDatoteka<string>(pot, $"{kodaZaposlenega};{ime};{oddelek}");
            datoteka.ShraniVDatoteko(zapis);

            Console.Write("Želite dodati naslednjega zaposlenega (d/n)? ");
            string naslednji = Console.ReadLine().ToLower();
            if (naslednji == "d")
            {
                DodajZaposlenega(kodaPodjetja);
            }
            else
            {
                MeniZaposleni(kodaPodjetja);
            }
        }

        public void OgledZaposlenega(int kodaPodjetja)
        {
            Console.Clear();
            Console.WriteLine("OGLED IN UREJANJE ZAPOSLENIH:");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Koda\t Ime\t Oddelek");
            if (SeznamZaposlenih.Count != 0)
            {
                foreach (var zaposleni in SeznamZaposlenih)
                {
                    Console.WriteLine($"{zaposleni.KodaZaposlenega}\t{zaposleni.Ime}\t{zaposleni.Oddelek}");
                }

                Console.Write("\nŽelite urediti seznam zaposlenih (d/n) ");
                string urejanje = Console.ReadLine().ToLower();

                if (urejanje == "d")
                    UrejanjeZaposlenih(kodaPodjetja);
                else
                    MeniZaposleni(kodaPodjetja);
            }
            else
            {
                Console.WriteLine("Za izbrano podjetje še ni izdelan seznam zaposlenih. Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                MeniZaposleni(kodaPodjetja);
            }
        }

        public void UrejanjeZaposlenih(int kodaPodjetja)
        {
            Console.Write("\nVpišite kodo zaposlenega: ");
            string koda = Console.ReadLine();

            bool isNum = int.TryParse(koda, out int kodaZaposlenega);
            if (!isNum)
            {
                Console.WriteLine("\nNapačen vnos. Poskusite znova.");
                Console.ReadKey();
            }

            var zaposleni = SeznamZaposlenih.FirstOrDefault(k => k.KodaZaposlenega == kodaZaposlenega);
            if (zaposleni == null)
            {
                Console.WriteLine("\nZaposleni s podano kodo ne obstaja. Poskusite znova.");
                Console.ReadKey();
            }

            Console.WriteLine($"\nUrejanje zaposlenega: {zaposleni.Ime}, Oddelek: {zaposleni.Oddelek}");
            Console.Write("\nŽelte spremeniti (i)me, (o)ddelek ali zelite (b)risati zaposlenega? ");
            string sprememba = Console.ReadLine().ToLower();

            switch (sprememba)
            {
                case "i":
                    ZamenjajIme(kodaPodjetja, zaposleni.KodaZaposlenega);
                    break;
                case "o":
                    ZamenjajOddelek(kodaPodjetja, zaposleni.KodaZaposlenega);
                    break;
                case "b":
                    SeznamZaposlenih.Remove(zaposleni);
                    ShraniZaposlene(kodaPodjetja);
                    Console.WriteLine("Zaposleni odstranjen iz baze. Za nadaljevanje pritisnite katerokoli tipko.");
                    Console.ReadKey();
                    MeniZaposleni(kodaPodjetja);
                    break;
                default:
                    Console.WriteLine("Napačen vnos. Za nadaljevanje pritisnite katerokoli tipko.");
                    Console.ReadKey();
                    MeniZaposleni(kodaPodjetja);
                    break;
            }
        }

        public void ZamenjajIme(int kodaPodjetja, int kodaZaposlenega)
        {
            Console.Write("Vpišite novo ime zaposlenega: ");
            string novoIme = Console.ReadLine();

            SeznamZaposlenih.Where(seznam => seznam.KodaZaposlenega == kodaZaposlenega)
                .ToList()
                .ForEach(seznam => seznam.Ime = novoIme);

            ShraniZaposlene(kodaPodjetja);
        }

        public void ZamenjajOddelek(int kodaPodjetja, int kodaZaposlenega)
        {
            Console.Write("Vpišite nov oddelek zaposlenega: ");
            string novOddelek = Console.ReadLine();

            SeznamZaposlenih.Where(seznam => seznam.KodaZaposlenega == kodaZaposlenega)
                .ToList()
                .ForEach(seznam => seznam.Oddelek = novOddelek);
            ShraniZaposlene(kodaPodjetja);
        }


        // Dodajanje zaposlenih iz baze v seznam
        public void PreberiZaposleneIzBaze(int kodaPodjetja)
        {
            string pot = LokalnaPot(kodaPodjetja);
            var preberiIzDatoteke = datoteka.IzpisIzDatoteke(pot);
            string str = preberiIzDatoteke.Data;

            Console.WriteLine("Prebrano iz datoteke:");
            foreach (string vrstica in str.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                string[] razdeli = vrstica.Split(";");
                if (razdeli.Length == 3 && int.TryParse(razdeli[0], out int kodaZaposlenega))
                {
                    string ime = razdeli[1];
                    string oddelek = razdeli[2];

                    Zaposleni zaposleni = new Zaposleni();
                    zaposleni.KodaZaposlenega = kodaZaposlenega;
                    zaposleni.Ime = ime;
                    zaposleni.Oddelek = oddelek;

                    SeznamZaposlenih.Add(zaposleni);
                }
            }
        }

        public void PopolniSeznam(int kodaPodjetja)
        {
            string pot = LokalnaPot(kodaPodjetja);
            if (SeznamZaposlenih.Count == 0)
            {
                if (File.Exists(pot))
                    PreberiZaposleneIzBaze(kodaPodjetja);
            }
        }

        public void ShraniZaposlene(int kodaPodjetja)
        {
            string pot = LokalnaPot(kodaPodjetja);
            File.Delete(pot);
            if (SeznamZaposlenih != null)
            {
                foreach(var vrstica in SeznamZaposlenih)
                {
                    int kodaZaposlenega = vrstica.KodaZaposlenega;
                    string ime = vrstica.Ime;
                    string oddelek = vrstica.Oddelek;
                    
                    var zapis = new PodatkovnaDatoteka<string>(pot, $"{kodaZaposlenega};{ime};{oddelek}");
                    datoteka.ShraniVDatoteko(zapis);
                }
            }
            else
            {
                Console.WriteLine("Seznam je prazen! Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
            }
        }

        private string LokalnaPot(int kodaPodjetja)
        {
            return datoteka.PotDoDatotek($"Zaposleni", $"{kodaPodjetja}_SeznamZaposlenih.pdb");
        }
    }
}

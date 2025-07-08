using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NacrtovalecIzobrazevanj
{
    public enum StatusIzvajalca
    {
        Zunanji,
        Notranji
    }
    internal class Izvajalec : IzvajalciIzobrazevanj
    {
        public StatusIzvajalca Status {  get; set; } = StatusIzvajalca.Notranji;
        public string Oddelek {  get; set; }
        private static UpravljanjeDatotek datoteka = new UpravljanjeDatotek();
        public Izvajalec(NacrtIzobrazevanj nacrtIzobrazevanj) : base(nacrtIzobrazevanj) { }
        
        public override IIterator<Izvajalec> KreirajIteratorIzvajalcev(List<Izvajalec> seznam)
        {
            return new IzpisIzvajalcev(seznam);
        }

        public void DodajIzvajalca(int kodaPodjetja, List<Izvajalec> seznamIzvajalcev)
        {
            string pot = LokalnaPot(kodaPodjetja);
            Console.Clear();
            Izvajalec izvajalec = new Izvajalec(this.NacrtIzobrazevanjInstanca);
            Console.WriteLine("DODAJANJE IZVAJALCA IZOBRAŽEVANJ:");
            Console.WriteLine("----------------------------------------");

            izvajalec.KodaIzvajalca = IdGenerator.GetId.GenerirajKodoIzvajalca(kodaPodjetja, seznamIzvajalcev);
            

            Console.Write("Vpišite naziv izvajalca: ");
            izvajalec.NazivIzvajalca = Console.ReadLine();
            
            StatusIzvajalcaIzobrazevanj();
            
            if(Status.Equals(StatusIzvajalca.Notranji))
            {
                Console.Write("Vpišite oddelek izvajalca: ");
                izvajalec.Status = StatusIzvajalca.Notranji;
                izvajalec.Oddelek = Console.ReadLine();
            }
            else
            {
                Console.Write("Vpišite naslov izvajalca: ");
                izvajalec.Status = StatusIzvajalca.Zunanji;
                izvajalec.Oddelek = Console.ReadLine();
            }

            seznamIzvajalcev.Add(izvajalec);

            var zapis = new PodatkovnaDatoteka<string>(pot, $"{izvajalec.KodaIzvajalca};{izvajalec.NazivIzvajalca};{izvajalec.Oddelek};{izvajalec.Status}");
            datoteka.ShraniVDatoteko(zapis);

            Console.Write("\nŽelite dodati izobraževanje (d/n)? ");
            string tecaj = Console.ReadLine().ToLower();
            if (tecaj == "d")
            {
                DodajIzvajalca(kodaPodjetja, seznamIzvajalcev);
            }
            else
            {
                MeniIzvajalcev(kodaPodjetja);
            }
        }

        private void StatusIzvajalcaIzobrazevanj()
        {
            Console.Write("Izberite status izvajalca (1 - Zunanji, 2 - Notranji): ");
            string st = Console.ReadLine().ToLower();
            if (st == "1")
            {
                this.Status = StatusIzvajalca.Zunanji;
            }
            else if (st == "2")
            {
                this.Status = StatusIzvajalca.Notranji;
            }
            else
            {
                Console.WriteLine("Napačen vnos. Poskusite znova.");
                StatusIzvajalcaIzobrazevanj();
            }
        }

        public void UrejanjeIzvajalcev(int kodaPodjetja, List<Izvajalec> seznamIzvajalcev)
        {
            KodaIzvajalca = IzbiraIzvajalca(kodaPodjetja, seznamIzvajalcev);

            var izvajalec = seznamIzvajalcev.FirstOrDefault(k => k.KodaIzvajalca == KodaIzvajalca);
            if (izvajalec == null)
            {
                Console.WriteLine("\nIzvajalec s podano kodo ne obstaja. Poskusite znova.");
                Console.ReadKey();
            }

            Console.WriteLine($"\nUrejanje Izvajalca: {izvajalec.NazivIzvajalca}, Oddelek: {izvajalec.Oddelek}");
            Console.Write("\nŽelte spremeniti (n)aziv, (o)ddelek/naslov ali zelte (b)risati izvajalca? ");
            string sprememba = Console.ReadLine().ToLower();

            switch (sprememba)
            {
                case "n":
                    ZamenjajNaziv(kodaPodjetja, izvajalec.KodaIzvajalca, seznamIzvajalcev);
                    MeniIzvajalcev(kodaPodjetja);
                    break;
                case "o":
                    ZamenjajOddelekIzvajalca(kodaPodjetja, izvajalec.KodaIzvajalca, seznamIzvajalcev);
                    break;
                case "b":
                    seznamIzvajalcev.Remove(izvajalec);
                    ShraniIzvajalce(kodaPodjetja, seznamIzvajalcev);
                    Console.WriteLine("Zaposleni odstranjen iz baze. Za nadaljevanje pritisnite katerokoli tipko.");
                    Console.ReadKey();
                    MeniIzvajalcev(kodaPodjetja);
                    break;
                default:
                    Console.WriteLine("Napačen vnos. Za nadaljevanje pritisnite katerokoli tipko.");
                    Console.ReadKey();
                    MeniIzvajalcev(kodaPodjetja);
                    break;
            }
        }

        public void ZamenjajNaziv(int kodaPodjetja, int kodaIzvajalca, List<Izvajalec> seznamIzvajalcev)
        {
            Console.Write("Vpišite nov naziv izvajalca: ");
            string novNaziv = Console.ReadLine();

            seznamIzvajalcev.Where(seznam => seznam.KodaIzvajalca == kodaIzvajalca)
                .ToList()
                .ForEach(seznam => seznam.NazivIzvajalca = novNaziv);

            ShraniIzvajalce(kodaPodjetja, seznamIzvajalcev);
        }

        public void ZamenjajOddelekIzvajalca(int kodaPodjetja, int kodaIzvajalca, List<Izvajalec> seznamIzvajalcev)
        {
            Console.Write("Vpišite nov oddelek/naslov izvajalca: ");
            string novOddelek = Console.ReadLine();

            seznamIzvajalcev.Where(seznam => seznam.KodaIzvajalca == kodaIzvajalca)
                .ToList()
                .ForEach(seznam => seznam.Oddelek = novOddelek);
            ShraniIzvajalce(kodaPodjetja, seznamIzvajalcev);
        }


        public void ShraniIzvajalce(int kodaPodjetja, List<Izvajalec> seznamIzvajalcev)
        {
            string pot = LokalnaPot(kodaPodjetja);
            File.Delete(pot);
            if (seznamIzvajalcev != null)
            {
                foreach (var vrstica in seznamIzvajalcev)
                {
                    int kodaIzvajalca = vrstica.KodaIzvajalca;
                    string naziv = vrstica.NazivIzvajalca;
                    string oddelek = vrstica.Oddelek;
                    string status = vrstica.Status.ToString();

                    var zapis = new PodatkovnaDatoteka<string>($"Baza\\Izvajalci\\{kodaPodjetja}_SeznamIzvajalcev.pdb", $"{kodaIzvajalca};{naziv};{oddelek};{status}");
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
            return datoteka.PotDoDatotek($"Izvajalci", $"{kodaPodjetja}_SeznamIzvajalcev.pdb");
        }
    }
}

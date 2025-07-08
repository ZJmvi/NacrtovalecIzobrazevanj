using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NacrtovalecIzobrazevanj
{
    internal class Nacrtovalec
    {
        private Dictionary<int, NacrtIzobrazevanj> SeznamPodjetij {  get; set; }

        private UpravljanjeDatotek datoteka = new UpravljanjeDatotek();
        public Nacrtovalec()
        {
            SeznamPodjetij = new Dictionary<int, NacrtIzobrazevanj>();
        }

        public void GlavniMeni()
        {
            string pot = LokalnaPot();
            if (SeznamPodjetij.Count == 0)
            {
                if (File.Exists(pot))
                    SeznamPodjetij = PreberiPodjetjaIzBaze();
            }

            Console.Clear();
            Console.WriteLine("GLAVNI MENI:");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Možnosti:");
            Console.WriteLine("\t1 - Dodaj podjetje");
            Console.WriteLine("\t2 - Izberi podjetje");
            Console.WriteLine("\tX - Izhod");
            Console.Write("Vaša izbira: ");
            string izbira = Console.ReadLine().ToLower();

            switch(izbira)
            {
                case "1":
                    DodajPodjetje();
                    break;
                case "2":
                    IzberiPodjetje();
                    break;
                case "x":
                    Environment.Exit(0);
                    break;
                default:
                    GlavniMeni();
                    break;
            }
        }

        public void DodajPodjetje()
        {
            string pot = LokalnaPot();
            Console.Clear();
            Console.WriteLine("DODAJANJE NOVEGA PODJETJA:");
            Console.WriteLine("----------------------------------------");
            Console.Write("Vpišite naziv podjetja: ");
            string naziv = Console.ReadLine();
            Console.Write("Vpišite kodo podjetja: ");
            int koda = Int32.Parse(Console.ReadLine());

            // Kreiranje novega podjetja
            NacrtIzobrazevanj podjetje = new NacrtIzobrazevanj(koda, naziv, this);

            // Dodajanje podjetja na seznam
            SeznamPodjetij.Add(koda, podjetje);

            // Shranjevanje podjetja v datoteko
            var zapis = new PodatkovnaDatoteka<string>(pot, $"{koda};{naziv}");
            datoteka.ShraniVDatoteko(zapis);

            GlavniMeni();
        }

        public void IzberiPodjetje()
        {
            string pot = LokalnaPot();
            Console.Clear();
            Console.WriteLine("IZBIRA PODJETJA:");
            Console.WriteLine("----------------------------------------");

            // Izpis podjetij za izbiro
            if (SeznamPodjetij.Count == 0)
            {
                if (!File.Exists(pot))
                {
                    Console.Write("V bazi ni še nobenega podjetja! Pritisni katero koli tipko za nadaljevanje.");
                    Console.ReadKey();
                    GlavniMeni();
                }
                else
                {
                    SeznamPodjetij = PreberiPodjetjaIzBaze();
                    IzberiPodjetje();
                }
            }
            else
            {
                int koda = 0;

                Console.WriteLine("N - Nazaj");
                Console.WriteLine("X - Izhod\n");
                Console.WriteLine("Koda - Naziv podjetja");

                // Izpis podjetji v slovarju
                SeznamPodjetij.ToList().ForEach(x => Console.WriteLine($"{x.Key} - {x.Value.NazivPodjetja}"));
                

                Console.Write($"\nVpišite kodo izbranega podjetja: ");
                string izbira = Console.ReadLine().ToLower();

                if (string.IsNullOrWhiteSpace(izbira))
                {
                    Console.WriteLine("Vpisati morate kodo podjetja iz seznama! Za nadaljevanje pritisnite katero koli tipko.");
                    Console.ReadKey();
                    IzberiPodjetje();
                }
                else if (izbira == "n")
                {
                    GlavniMeni();
                }
                else if (izbira == "x")
                {
                    Environment.Exit(0);
                }

                // Izbira podjetja s strani uporabnika
                if(int.TryParse(izbira, out koda))
                {
                    var izbranoPodjetje = SeznamPodjetij.FirstOrDefault(p => p.Key == koda);

                    if (!izbranoPodjetje.Equals(default(KeyValuePair<int, NacrtIzobrazevanj>)))
                    {
                        Console.WriteLine($"Izbrano podjetje je: {izbranoPodjetje.Value.KodaPodjetja} - {izbranoPodjetje.Value.NazivPodjetja}\n\n");
                        
                        izbranoPodjetje.Value.OsnovniMeni();
                        IzberiPodjetje();
                    }
                    else
                    {
                        Console.WriteLine("Podjetje s to kodo ne obstaja! Poskusite ponovno.");
                        IzberiPodjetje();
                    }
                }
                else
                {
                    Console.WriteLine("Neveljaven vnos. Poskusite ponovno.");
                    IzberiPodjetje();
                }
            }
        }

        // Dodajanje podjetij iz baze v seznam
        public Dictionary<int, NacrtIzobrazevanj> PreberiPodjetjaIzBaze()
        {
            string pot = LokalnaPot();
            var preberiIzDatoteke = datoteka.IzpisIzDatoteke(pot);
            string str = preberiIzDatoteke.Data;
            string naziv;
            

            Console.WriteLine("Prebrano iz datoteke:");
            foreach (string vrstica in str.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                string[] razdeli = vrstica.Split(";");
                if (razdeli.Length == 2 && int.TryParse(razdeli[0], out int koda))
                {
                    naziv = razdeli[1];
                    NacrtIzobrazevanj podjetje = new NacrtIzobrazevanj(koda, naziv, this);
                    SeznamPodjetij.Add(koda, podjetje);
                }
            }

            return SeznamPodjetij;
        }
        private string LokalnaPot()
        {
            return datoteka.PotDoDatotek($"Podjetja", "SeznamPodjetij.pdb");
        }
    }
}

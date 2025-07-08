using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NacrtovalecIzobrazevanj
{
    internal class NacrtIzobrazevanj
    {
        private Nacrtovalec Nacrtovalec;
        public Nacrt NovNacrt = new Nacrt();
        public int KodaPodjetja { get; set; }
        public string NazivPodjetja { get; set; }
        private Zaposleni NovZaposleni;
        private Izvajalec NovIzvajalec;


        public NacrtIzobrazevanj(int kodaPodjetja, string nazivPodjetja, Nacrtovalec nacrtovalec)
        {
            this.KodaPodjetja = kodaPodjetja;
            this.NazivPodjetja = nazivPodjetja;
            this.Nacrtovalec = nacrtovalec;

            NovZaposleni = new Zaposleni(this);
            NovIzvajalec = new Izvajalec(this);
        }

        public void OsnovniMeni()
        {
            Console.Clear();
            Console.WriteLine($"NAČRTOVALEC IZOBRAŽEVANJ PODJETJA {NazivPodjetja}:");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("1 - Načrti izobraževanj");
            Console.WriteLine("2 - Izvajalci izobraževanj");
            Console.WriteLine("3 - Zaposleni");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("N - Nazaj na izbiro podjetja");
            Console.WriteLine("G - Nazaj na glavni meni");
            Console.WriteLine("X - Izhod\n");
            Console.Write("Vnesite vašo izbiro: ");
            string izbira = Console.ReadLine().ToLower();

            switch(izbira)
            {
                case "n":
                    Nacrtovalec.IzberiPodjetje();
                    break;
                case "g":
                    Nacrtovalec.GlavniMeni();
                    break;
                case "x":
                    Environment.Exit(0);
                    break;
                case "1":
                    NovNacrt.MeniNacrtAsync(KodaPodjetja).Wait();
                    OsnovniMeni();
                    break;
                case "2":
                    NovIzvajalec.MeniIzvajalcev(KodaPodjetja);
                    OsnovniMeni();
                    break;
                case "3":
                    NovZaposleni.MeniZaposleni(KodaPodjetja);
                    OsnovniMeni();
                    break;
                default:
                    Console.WriteLine("\nNapačen vnos! Poskusite znova. Pritisni katero koli tipko za nadaljevanje.");
                    Console.ReadKey();
                    OsnovniMeni();
                    break;
            }
        }
    }
}

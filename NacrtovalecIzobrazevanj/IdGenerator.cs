using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NacrtovalecIzobrazevanj
{
    internal sealed class IdGenerator
    {
        // Uporaba Singelton designe pattern za generiranje id števil
        private static int id = 1;
        private static IdGenerator instanca = null;

        public static IdGenerator GetId
        {
            get
            {
                if (instanca == null)
                    instanca = new IdGenerator();
                return instanca;
            }
        }

        private IdGenerator() { }

        public int GenerirajKodoZaposlenega(int kodaPodjetja, List<Zaposleni> seznamZaposlenih)
        {
            if (seznamZaposlenih.Count != 0)
            {
                int koda = seznamZaposlenih.Last().KodaZaposlenega;
                return ++koda;
            }
            else
            {
                return (kodaPodjetja * 10) + 256 + id;
            }
        }

        public int GenerirajKodoIzobrazevanja(int kodaPodjetja, List<string> seznamTecajev)
        {
            if (seznamTecajev?.Count > 0)
            {
                var zadnjaVrstica = seznamTecajev.Last();
                var razdeli = zadnjaVrstica.Split(';');

                if (int.TryParse(razdeli[0], out int koda))
                {
                    return koda + 1;
                }
                else
                {
                    Console.WriteLine("Napaka pri pretvorbi kode! Za nadaljevanje pritisnite katerokoli tipko.");
                    Console.ReadKey();
                    return -1;
                }
            }
            else
            {
                return (kodaPodjetja / 3) + 1024 + id;
            }
        }

        public int GenerirajKodoIzvajalca(int kodaPodjetja, List<Izvajalec> seznamIzvajalcev)
        {
            if (seznamIzvajalcev.Count != 0)
            {
                int koda = seznamIzvajalcev.Last().KodaIzvajalca;
                return ++koda;
            }
            else
            {
                return (kodaPodjetja * 2) + 512 + id;
            }
        }

        public int GenerirajKodoNacrta(int kodaPodjetja, ConcurrentDictionary<(int, int), List<Zaposleni>> nacrt)
        {
            if (nacrt.Count > 0)
            {
                int koda = nacrt.Keys.Max(k => k.Item1);
                return koda + 1;
            }
            else
            {
                return (kodaPodjetja * 5) + 128 + id;
            }
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Reflection.Metadata.Ecma335;

namespace NacrtovalecIzobrazevanj
{
    internal class Nacrt
    {
        private int KodaNacrta {  get; set; }
        private List<string> SeznamTecajev = new List<string>();
        private List<Zaposleni> SeznamZaposlenih = new List<Zaposleni>();
        private ConcurrentDictionary<(int, int), List<Zaposleni>> NacrtDictionary = new ConcurrentDictionary<(int, int), List<Zaposleni>>();
        private static readonly UpravljanjeDatotek datoteka = new UpravljanjeDatotek();
        private static readonly object ConsoleLock = new object();
        public Nacrt() { }

        public async Task MeniNacrtAsync(int kodaPodjetja)
        {
            while (true)
            {
                Console.Clear();

                await PopolniSeznameAsync(kodaPodjetja);
                PopolniSeznamNacrtov(kodaPodjetja);
                

                Console.WriteLine("NAČRTI IZOBRAŽEVANJ:");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("1 - Ustvari nov načrt");
                Console.WriteLine("2 - Ogled načrta");
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("N - Nazaj na osnovni meni");
                Console.WriteLine("X - Izhod\n");
                Console.Write("Vnesite vašo izbiro: ");
                string izbira = Console.ReadLine().ToLower();

                switch (izbira)
                {
                    case "n":
                        
                        return;
                    case "x":
                        Environment.Exit(0);
                        break;
                    case "1":
                        KodaNacrta = IdGenerator.GetId.GenerirajKodoNacrta(kodaPodjetja, NacrtDictionary);
                        var nacrt = await PripraviNacrtAsync(SeznamTecajev, SeznamZaposlenih);
                        ShraniNacrt(kodaPodjetja, nacrt);
                        IzpisNacrtov(nacrt);
                        break;
                    case "2":
                        IzpisNacrtov(NacrtDictionary);
                        break;
                    default:
                        Console.WriteLine("Za nadaljevanje pritisni katerokoli tipko...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        public void PopolniSeznamNacrtov(int kodaPodjetja)
        {
            string pot = LokalnaPot(kodaPodjetja);

            if (NacrtDictionary.IsEmpty)
            {
                if (File.Exists(pot))
                {
                    PreberiNacrteIzBaze(kodaPodjetja);
                }
            }
        }
        public void IzpisNacrtov(ConcurrentDictionary<(int kodaN, int kodaT), List<Zaposleni>> nacrt)
        {
            Console.Clear();
            string tip = string.Empty;
            string nazivTecaja = string.Empty;
            string datum = string.Empty;
            string trajanje = string.Empty;

            if (!NacrtDictionary.IsEmpty)
            {
                Console.WriteLine("Izpis načrtov:");
                foreach (var par in nacrt)
                {
                    foreach (var vrstica in SeznamTecajev)
                    {
                        var razdeli = vrstica.Split(";");
                        if (razdeli.Length == 9 && int.TryParse(razdeli[0], out int kodaTecaja))
                        {
                            if (par.Key.kodaT == kodaTecaja)
                            {
                                tip = razdeli[3];
                                nazivTecaja = razdeli[4];
                                datum = razdeli[5];
                                trajanje = razdeli[6];
                            }
                        }
                    }

                    Console.WriteLine($"\nNačrt #{par.Key.kodaN}, Izobraževanje #{par.Key.kodaT} -> {tip}: {nazivTecaja}, datum začetka: {datum}, trajanje {trajanje} dni");

                    foreach (var z in par.Value)
                    {
                        Console.WriteLine($"   - {z.Ime}, {z.Oddelek}");
                    }
                }
                Console.WriteLine("Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("V bazi ni še nobenega načrta izobraževanj! Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
            }
        }
        private async Task PopolniSeznameAsync(int kodaPodjetja)
        {
            (string pot1, string pot2) = PotSeznami($"{kodaPodjetja}_SeznamZaposlenih.pdb", $"{kodaPodjetja}_SeznamIzobrazevanj.pdb");
            SeznamZaposlenih.Clear();
            SeznamTecajev.Clear();


            var taskZaposleni = Task.Run(() => datoteka.IzpisIzDatoteke(pot1));

            var taskTecaji = Task.Run(() => datoteka.IzpisIzDatoteke(pot2));
            
            await Task.WhenAll(taskZaposleni, taskTecaji);

            // Popolnjevanje seznama zaposlenih
            string strZaposleni = taskZaposleni.Result.Data ?? "";
            foreach (var vrstica in strZaposleni.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
                
            {
                var razdeli = vrstica.Split(";");
                if (razdeli.Length == 3 && int.TryParse(razdeli[0], out int kodaZaposlenega))
                {
                    SeznamZaposlenih.Add(new Zaposleni
                    {
                        KodaZaposlenega = kodaZaposlenega,
                        Ime = razdeli[1].Trim(),
                        Oddelek = razdeli[2].Trim()
                    });
                }
            }

            // Popolnjevanje seznama tečajev
            string strTecaji = taskTecaji.Result.Data ?? "";
            foreach (var vrstica in strTecaji.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            {
                var razdeli = vrstica.Split(";");
                if (razdeli.Length == 9 && int.TryParse(razdeli[0], out int kodaTecaja))
                {
                    var popDate = new Tecaji().PopraviDatum(razdeli[5]);
                    SeznamTecajev.Add($"{kodaTecaja};{razdeli[1]};{razdeli[2]};{razdeli[3]};{razdeli[4]};{popDate};{razdeli[6]};{razdeli[7]};{razdeli[8]}");
                }
            }
        }
        public async Task<ConcurrentDictionary<(int, int), List<Zaposleni>>> PripraviNacrtAsync(List<string> seznamTecajev, List<Zaposleni> seznamZaposlenih)
        {
            var tasks = seznamTecajev.Select(vrstica =>
                Task.Run(() =>
                {
                    var deli = vrstica.Split(';');
                    if (!int.TryParse(deli[0], out int kodaTecaja)) return;

                    List<int> izbraniIDji;
                    lock (ConsoleLock)
                    {
                        Console.WriteLine($"\nTečaj: {kodaTecaja}\t{deli[3]}\t{deli[4]}\t{deli[5]}");
                        Console.WriteLine("Izberite udeležence (loči ID-je z vejico):");

                        foreach (var zaposleni in seznamZaposlenih)
                        {
                            Console.WriteLine($"{zaposleni.KodaZaposlenega}\t{zaposleni.Ime}\t{zaposleni.Oddelek}");
                        }
                        Console.Write("> ");
                        var izbor = Console.ReadLine() ?? "";
                        izbraniIDji = izbor.Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(z => int.TryParse(z.Trim(), out var x) ? x : -1)
                        .Where(x => x >= 0)
                        .Distinct()
                        .ToList();
                    }

                    var izbraniZaposleni = seznamZaposlenih.Where(z => izbraniIDji.Contains(z.KodaZaposlenega)).ToList();

                    // Tečaj je dodan v slovar smo, kadar je uporabnik tečaju dodal zaposlene 
                    if (izbraniZaposleni.Count > 0)
                    {
                        NacrtDictionary[(KodaNacrta, kodaTecaja)] = izbraniZaposleni;
                    }
                })
            ).ToArray();

            await Task.WhenAll(tasks);

            return NacrtDictionary;
        }

        public void ShraniNacrt(int kodaPodjetja, ConcurrentDictionary<(int, int), List<Zaposleni>> nacrt)
        {
            string pot = LokalnaPot(kodaPodjetja);

            if (nacrt == null || nacrt.Count == 0)
            {
                Console.WriteLine("Seznam je prazen! Za nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                return;
            }

            if (File.Exists(pot))
            {
                File.Delete(pot);
            }

            foreach (var vrstica in nacrt)
            {
                int kodaNacrta = vrstica.Key.Item1;
                int kodaTecaja = vrstica.Key.Item2;

                foreach (var z in vrstica.Value)
                {
                    string razdeli = string.Join(";", kodaNacrta, kodaTecaja, z.KodaZaposlenega, z.Ime, z.Oddelek);
                    datoteka.ShraniVDatoteko(new PodatkovnaDatoteka<string>(pot, razdeli));
                }
            }
        }

        // Dodajanje zaposlenih iz baze v seznam
        public void PreberiNacrteIzBaze(int kodaPodjetja)
        {
            NacrtDictionary.Clear();

            string pot = LokalnaPot(kodaPodjetja);

            var preberiIzDatoteke = datoteka.IzpisIzDatoteke(pot);
            string str = preberiIzDatoteke.Data;

            if (string.IsNullOrWhiteSpace(str)) return;

            var vrstice = str.Split(new[] {Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var vrstica in vrstice)
            {
                var razdeli = vrstica.Split(";");

                if (razdeli.Length != 5) continue;

                if (!int.TryParse(razdeli[0], out int kodaNacrta) || !int.TryParse(razdeli[1], out int kodatecaja) || !int.TryParse(razdeli[2], out int kodaZaposlenega)) continue;

                var z = new Zaposleni
                {
                    KodaZaposlenega = kodaZaposlenega,
                    Ime = razdeli[3],
                    Oddelek = razdeli[4],

                };

                NacrtDictionary.AddOrUpdate(
                    (kodaNacrta, kodatecaja),
                    _ => new List<Zaposleni> { z },
                    (_, list) =>
                    {
                        list.Add(z);
                        return list;
                    });
            }
        }
        private string LokalnaPot(int kodaPodjetja)
        {
            return datoteka.PotDoDatotek($"NacrtiIzobrazevanj", $"{kodaPodjetja}_SeznamNacrtov.pdb");
        }

        private (string, string) PotSeznami(string datoteka1, string datoteka2)
        {
            string exeMapa = AppContext.BaseDirectory;
            string glavnaMapa = Path.Combine(exeMapa, "Baza");
            string mapaZaposleni = Path.Combine(glavnaMapa, "Zaposleni");
            string mapaTecaji = Path.Combine(glavnaMapa, "Izobrazevanja");
            string pot1 = Path.Combine(mapaZaposleni, datoteka1);
            string pot2 = Path.Combine(mapaTecaji, datoteka2);
            return (pot1, pot2);
        }
    }
}

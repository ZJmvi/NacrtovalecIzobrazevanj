using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NacrtovalecIzobrazevanj
{
    public struct PodatkovnaDatoteka<T>
    {
        public string LokacijaDatoteke { get; }
        public T Data { get; }
        

        public PodatkovnaDatoteka(string lokacijaDatoteke, T data)
        {
            this.LokacijaDatoteke = lokacijaDatoteke;
            this.Data = data;
        }
    }

    
    internal class UpravljanjeDatotek
    {
        public string PotDoDatotek(string mapa, string datoteka)
        {
            string exeMapa = AppContext.BaseDirectory;
            string glavnaMapa = Path.Combine(exeMapa, "Baza");
            string podMapa = Path.Combine(glavnaMapa, mapa);
            return Path.Combine(podMapa, datoteka);
        }
        // Metoda za shranjevanje podatkov v datoteko
        public  void ShraniVDatoteko<T>(PodatkovnaDatoteka<T> podatkovnaDatoteka)
        {
            try
            {
                // Kreiranje mape Baza, če še ne obstaja
                if (!Directory.Exists("Baza"))
                {
                    Directory.CreateDirectory("Baza");
                    Directory.CreateDirectory("Baza\\Podjetja");
                    Directory.CreateDirectory("Baza\\Zaposleni");
                    Directory.CreateDirectory("Baza\\Izvajalci");
                    Directory.CreateDirectory("Baza\\Izobrazevanja");
                    Directory.CreateDirectory("Baza\\NacrtiIzobrazevanj");
                }
                    

                // Zapis podatkov v datoteko
                StreamWriter sw = new StreamWriter(podatkovnaDatoteka.LokacijaDatoteke, true);
                sw.WriteLine(podatkovnaDatoteka.Data?.ToString());
                sw.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Napaka pri shranjevanju podatkov: {ex.Message}\nZa nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
            }
        }

        public PodatkovnaDatoteka<string> IzpisIzDatoteke(string lokacijaDatoteke)
        {
            try
            {
                // Branje podatkov iz datoteke in vračanje 
                string data = File.ReadAllText(lokacijaDatoteke);
                return new PodatkovnaDatoteka<string>(lokacijaDatoteke, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Napaka pri branju podatkov: {ex.Message}\nZa nadaljevanje pritisnite katerokoli tipko.");
                Console.ReadKey();
                return new PodatkovnaDatoteka<string>(lokacijaDatoteke, string.Empty);
            }
        }
    }
}

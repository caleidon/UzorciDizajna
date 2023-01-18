#region

using avranic_zadaca_1.Sucelja;

#endregion

namespace avranic_zadaca_1;

public class Servis : IServis
{
    private readonly BrodskaLuka _brodskaLuka = BrodskaLuka.Instance;

    public void IspisiStatuseVezova()
    {
        Console.WriteLine($"|{"Id ",4}|{"Oznaka ",8}|{"Cijena ",8}|{"Max Duljina ",12}|{"Max Sirina ",12}|" +
                          $"{"Max Dubina ",12}|{"Status ",10}|");

        DateTime vrijeme = VirtualniSat.Instance.VirtualnoVrijeme;

        foreach (Vez vez in _brodskaLuka.SviVezovi) {
            Console.WriteLine($"|{vez.Id,4}|{vez.Oznaka,8}|{vez.CijenaPoSatu,8}|{vez.MaksimalnaDuljina,12}|" +
                              $"{vez.MaksimalnaSirina,12}|{vez.MaksimalnaDubina,12}|" +
                              $"{(vez.RezerviranUDatumskomRasponu(vrijeme, vrijeme) ? "Zauzet" : "Slobodan"),10}|");
        }
    }

    public void PromjeniVirtualnoVrijeme(DateTime novoVrijeme)
    {
        VirtualniSat.Instance.VirtualnoVrijeme = novoVrijeme;

        foreach (Vez vez in _brodskaLuka.SviVezovi) {
            if (vez.PrivezaniBrod != null && vez.BrodPrivezanDo != null && novoVrijeme > vez.BrodPrivezanDo) {
                Console.WriteLine($"Brodska luka: Brod {vez.PrivezaniBrod.Id} je otpušten sa veza {vez.Id}.");
                vez.PrivezaniBrod = null;
                vez.BrodPrivezanDo = null;
            }

            // vez.SveZauzetosti.RemoveAll(z => z is RezervacijskaZauzetost rez && novoVrijeme > rez.RezerviranDo);

            foreach (IZauzetost zauzetost in vez.SveZauzetosti.ToList()) {
                if (zauzetost is not RezervacijskaZauzetost rez) { continue; }

                if (novoVrijeme <= rez.RezerviranDo) { continue; }

                vez.SveZauzetosti.Remove(rez);
                Console.WriteLine($"Rezervacija veza {vez.Id} od {rez.RezerviranOd} do {rez.RezerviranDo} je istekla");
            }
        }
    }

    public void IspisiZauzetostiVezova(Vez.VrstaVeza vrsta, bool zauzeti, DateTime datumOd, DateTime datumDo)
    {
        List<Vez> vezoviSaIspravnomVrstom = _brodskaLuka.SviVezovi.Where(vez => vez.Vrsta == vrsta).ToList();

        var sviRezerviraniRasponi = new List<Tuple<int, DateTime, DateTime>>();

        List<Vez> vezoviPoZauzetosti;

        if (zauzeti) {
            vezoviPoZauzetosti = vezoviSaIspravnomVrstom.Where(v => {
                                                            bool zauzet = v.RezerviranUDatumskomRasponu(datumOd,
                                                                datumDo,
                                                                out List<Tuple<int, DateTime, DateTime>>
                                                                    rezerviraniRasponi);
                                                            sviRezerviraniRasponi.AddRange(rezerviraniRasponi);
                                                            return zauzet;
                                                        })
                                                        .ToList();
        } else {
            vezoviPoZauzetosti = vezoviSaIspravnomVrstom.Where(v => !v.RezerviranUDatumskomRasponu(datumOd, datumDo,
                                                            out List<Tuple<int, DateTime, DateTime>> _))
                                                        .ToList();
        }

        if (!vezoviPoZauzetosti.Any()) { return; }

        string status = zauzeti ? "Zauzet" : "Slobodan";

        Console.WriteLine($"|{"Id ",4}|{"Oznaka ",8}|{"Cijena ",8}|{"Max Duljina ",12}|{"Max Sirina ",12}|" +
                          $"{"Max Dubina ",12}|{$"{status} od ",20}|{$"{status} do ",20}|");

        if (zauzeti) {
            foreach (Tuple<int, DateTime, DateTime> podaciRezervacije in sviRezerviraniRasponi) {
                Vez vez = vezoviSaIspravnomVrstom.First(v => v.Id == podaciRezervacije.Item1);

                Console.WriteLine($"|{vez.Id,4}|{vez.Oznaka,8}|{vez.CijenaPoSatu,8}|{vez.MaksimalnaDuljina,12}|" +
                                  $"{vez.MaksimalnaSirina,12}|{vez.MaksimalnaDubina,12}|" +
                                  $"{podaciRezervacije.Item2,10}|{podaciRezervacije.Item3,10}|");
            }
        } else {
            foreach (Vez vez in vezoviSaIspravnomVrstom) {
                Console.WriteLine($"|{vez.Id,4}|{vez.Oznaka,8}|{vez.CijenaPoSatu,8}|{vez.MaksimalnaDuljina,12}|" +
                                  $"{vez.MaksimalnaSirina,12}|{vez.MaksimalnaDubina,12}|" +
                                  $"{datumOd,10}|{datumDo,10}|");
            }
        }
    }

    public void UcitajZahtjeveRezervacije(string nazivDatoteke)
    {
        try {
            _brodskaLuka.SviZahtjeviRezervacije = new ZahtjevRezervacijeCsvCitac().Citaj(nazivDatoteke);
        } catch (Exception) {
            Program.IspisiGresku($"Unesen je krivi naziv datoteke rezervacije: {nazivDatoteke}");
            return;
        }

        foreach (ZahtjevRezervacije zahtjevRezervacije in _brodskaLuka.SviZahtjeviRezervacije) {
            DateTime datumOd = zahtjevRezervacije.DatumVrijemeOd;
            DateTime datumDo = datumOd.AddHours(zahtjevRezervacije.TrajanjePriveza);

            IEnumerable<Vez> sviNerezerviraniVezovi =
                _brodskaLuka.SviVezovi.Where(v => !v.RezerviranUDatumskomRasponu(datumOd, datumDo));

            Vez? podrzaniVez = sviNerezerviraniVezovi.FirstOrDefault(v => v.PodrzavaBrod(zahtjevRezervacije.Brod));

            if (podrzaniVez == null) {
                Console.WriteLine(
                    $"Zahtjev za rezervaciju broda sa ID {zahtjevRezervacije.Brod.Id} nije moguće izvršiti.");
                continue;
            }

            Console.WriteLine(
                $"Zahtjev za rezervaciju broda sa ID {zahtjevRezervacije.Brod.Id} je uspješno izvršen na vezu sa ID " +
                $"{podrzaniVez.Id} od {datumOd} do {datumDo}.");
            ZauzetostKreator zauzetostKreator = new RezervacijskaZauzetostKreator();

            podrzaniVez.SveZauzetosti.Add(zauzetostKreator.KreirajZauzetost(
                new ParametriRezervacijskeZauzetosti(zahtjevRezervacije.Brod, podrzaniVez, datumOd, datumDo)));
        }
    }

    public void PriveziZaRezerviraniVez(Brod brod)
    {
        DateTime vrijeme = VirtualniSat.Instance.VirtualnoVrijeme;

        foreach (Vez vez in _brodskaLuka.SviVezovi.Where(v => v.PodrzavaBrod(brod))) {
            foreach (IZauzetost zauzetost in vez.SveZauzetosti) {
                if (!zauzetost.ZauzetURasponu(vrijeme, vrijeme, out List<Tuple<int, DateTime, DateTime>> raspon) ||
                    zauzetost.Brod.Id != brod.Id) { continue; }

                vez.PrivezaniBrod = brod;
                vez.BrodPrivezanDo = raspon.First().Item3;
                Console.WriteLine($"Brod sa ID {brod.Id} je uspješno zauzeo vez {vez.Id}.");
                return;
            }
        }

        Console.WriteLine($"Nije moguće pronaći rezervirani vez za brod sa ID {brod.Id}.");
    }

    public void PriveziZaSlobodniVez(Brod brod, int trajanjeUSatima)
    {
        DateTime datumPocetkaZahtjeva = VirtualniSat.Instance.VirtualnoVrijeme;
        DateTime datumKrajaZahtjeva = VirtualniSat.Instance.VirtualnoVrijeme.AddHours(trajanjeUSatima);

        List<Vez> sviOdgovarajuciVezovi = _brodskaLuka.SviVezovi.Where(v =>
                                                          !v.RezerviranUDatumskomRasponu(datumPocetkaZahtjeva,
                                                              datumKrajaZahtjeva) &&
                                                          v.PodrzavaBrod(brod))
                                                      .ToList();

        if (sviOdgovarajuciVezovi.Count == 0) {
            Console.WriteLine("Nije moguće pronaći slobodan vez koji podržava brod.");
            return;
        }

        Vez vez = sviOdgovarajuciVezovi.First();
        vez.PrivezaniBrod = brod;
        vez.BrodPrivezanDo = datumKrajaZahtjeva;

        ZauzetostKreator zauzetostKreator = new RezervacijskaZauzetostKreator();

        vez.SveZauzetosti.Add(zauzetostKreator.KreirajZauzetost(new ParametriRezervacijskeZauzetosti(brod, vez,
            datumPocetkaZahtjeva, datumKrajaZahtjeva)));
        Console.WriteLine($"Brod sa ID {brod.Id} je uspješno zauzeo vez {vez.Id}.");
    }
}
#region

using System.Globalization;
using avranic_zadaca_1.ChainOfResponsibility;
using avranic_zadaca_1.Observer;
using avranic_zadaca_1.Sucelja;

#endregion

namespace avranic_zadaca_1;

public class Servis : IServis
{
    private readonly BrodskaLuka _brodskaLuka = BrodskaLuka.Instance;

    public void IspisiStatuseVezova()
    {
        var zaglavlje = new List<string> {
            "Id",
            "Oznaka",
            "Cijena",
            "Max Duljina",
            "Max Sirina",
            "Max Dubina",
            "Status"
        };

        var podaci = new List<List<object>>();

        DateTime vrijeme = VirtualniSat.Instance.VirtualnoVrijeme;

        foreach (Vez vez in _brodskaLuka.SviVezovi) {
            var redak = new List<object> {
                vez.Id,
                vez.Oznaka,
                vez.CijenaPoSatu,
                vez.MaksimalnaDuljina,
                vez.MaksimalnaSirina,
                vez.MaksimalnaDubina,
                vez.RezerviranUDatumskomRasponu(vrijeme, vrijeme) ? "Zauzet" : "Slobodan"
            };

            podaci.Add(redak);
        }

        IspisivacTablice.Instance.IspisiTablicu(zaglavlje, podaci);
    }

    public void PromjeniVirtualnoVrijeme(DateTime novoVrijeme)
    {
        VirtualniSat.Instance.VirtualnoVrijeme = novoVrijeme;

        foreach (Vez vez in _brodskaLuka.SviVezovi) {
            if (novoVrijeme > vez.BrodPrivezanDo) { vez.OslobodiPrivezaniBrod(); }

            vez.UkloniZastarjeleRezervacije();
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

        var zaglavlje = new List<string> {
            "Id",
            "Oznaka",
            "Cijena",
            "Max Duljina",
            "Max Sirina",
            "Max Dubina",
            $"{status} od",
            $"{status} do"
        };

        var podaci = new List<List<object>>();

        if (zauzeti) {
            foreach (Tuple<int, DateTime, DateTime> podaciRezervacije in sviRezerviraniRasponi) {
                Vez vez = vezoviSaIspravnomVrstom.First(v => v.Id == podaciRezervacije.Item1);

                var redak = new List<object> {
                    vez.Id,
                    vez.Oznaka,
                    vez.CijenaPoSatu,
                    vez.MaksimalnaDuljina,
                    vez.MaksimalnaSirina,
                    vez.MaksimalnaDubina,
                    podaciRezervacije.Item2.ToString(CultureInfo.CreateSpecificCulture("hr-HR")),
                    podaciRezervacije.Item3.ToString(CultureInfo.CreateSpecificCulture("hr-HR"))
                };

                podaci.Add(redak);
            }
        } else {
            foreach (Vez vez in vezoviSaIspravnomVrstom) {
                var redak = new List<object> {
                    vez.Id,
                    vez.Oznaka,
                    vez.CijenaPoSatu,
                    vez.MaksimalnaDuljina,
                    vez.MaksimalnaSirina,
                    vez.MaksimalnaDubina,
                    datumOd.ToString(CultureInfo.CreateSpecificCulture("hr-HR")),
                    datumDo.ToString(CultureInfo.CreateSpecificCulture("hr-HR"))
                };

                podaci.Add(redak);
            }
        }

        IspisivacTablice.Instance.IspisiTablicu(zaglavlje, podaci);
    }

    public void UcitajZahtjeveRezervacije(string nazivDatoteke)
    {
        try {
            _brodskaLuka.SviZahtjeviRezervacije = new ZahtjevRezervacijeCsvCitac().Citaj(nazivDatoteke);
        } catch (Exception) { return; }

        foreach (ZahtjevRezervacije zr in _brodskaLuka.SviZahtjeviRezervacije) {
            DateTime datumOd = zr.DatumVrijemeOd;
            DateTime datumDo = datumOd.AddHours(zr.TrajanjePriveza);

            IEnumerable<Vez> sviOdgovarajuciVezovi = _brodskaLuka.SviVezovi.Where(v =>
                !v.RezerviranUDatumskomRasponu(datumOd, datumDo) && v.PodrzavaBrod(zr.Brod));

            Vez? podrzaniVez = sviOdgovarajuciVezovi.SortirajPoEkonomicnosti().FirstOrDefault();

            if (podrzaniVez == null) {
                Alati.IspisiGresku($"Zahtjev za rezervaciju broda {zr.Brod.Id} nije moguće izvršiti.");
                continue;
            }

            Alati.IspisiPotvrdu($"Zahtjev za rezervaciju broda {zr.Brod.Id} je uspješno izvršen na vezu " +
                                $"{podrzaniVez.Id} od {datumOd} do {datumDo}.");
            ZauzetostKreator zauzetostKreator = new RezervacijskaZauzetostKreator();

            podrzaniVez.SveZauzetosti.Add(zauzetostKreator.KreirajZauzetost(
                new ParametriRezervacijskeZauzetosti(zr.Brod, podrzaniVez, datumOd, datumDo)));
        }
    }

    public void PosaljiPorukuZahjevaPrivezaZaRezerviraniVez(Brod brod)
    {
        if (brod.Mediator == null) {
            Alati.IspisiGresku($"Brod {brod.Id} nije spojen na kanal, pa ne može slati zahtjeve.");
            return;
        }

        brod.Mediator.PosaljiPoruku(new ZahtjevZaPrivezNaRezerviraniVez(brod));
    }

    public bool ProbajPrivezatiZaRezerviraniVez(Brod brod)
    {
        DateTime vrijeme = VirtualniSat.Instance.VirtualnoVrijeme;

        foreach (Vez vez in _brodskaLuka.SviVezovi.Where(v => v.PodrzavaBrod(brod))) {
            foreach (IZauzetost zauzetost in vez.SveZauzetosti) {
                if (!zauzetost.ZauzetURasponu(vrijeme, vrijeme, out List<Tuple<int, DateTime, DateTime>> raspon) ||
                    zauzetost.Brod.Id != brod.Id) { continue; }

                vez.PrivezaniBrod = brod;
                vez.BrodPrivezanDo = raspon.First().Item3;
                Alati.IspisiPotvrdu($"Brod {brod.Id} je uspješno zauzeo vez {vez.Id}.");
                return true;
            }
        }

        Alati.IspisiGresku($"Nije moguće pronaći rezervirani vez za brod {brod.Id}.");
        return false;
    }

    public void PosaljiPorukuZahjevaPrivezaZaSlobodniVez(Brod brod, int trajanjeUSatima)
    {
        if (brod.Mediator == null) {
            Alati.IspisiGresku($"Brod {brod.Id} nije spojen na kanal, pa ne može slati zahtjeve.");
            return;
        }

        brod.Mediator.PosaljiPoruku(new ZahtjevZaPrivezNaSlobodanVez(brod, trajanjeUSatima));
    }

    public bool ProbajPrivezatiZaSlobodniVez(Brod brod, int trajanjeUSatima)
    {
        DateTime datumPocetkaZahtjeva = VirtualniSat.Instance.VirtualnoVrijeme;
        DateTime datumKrajaZahtjeva = VirtualniSat.Instance.VirtualnoVrijeme.AddHours(trajanjeUSatima);

        List<Vez> sviOdgovarajuciVezovi = _brodskaLuka.SviVezovi.Where(v =>
                                                          !v.RezerviranUDatumskomRasponu(datumPocetkaZahtjeva,
                                                              datumKrajaZahtjeva) &&
                                                          v.PodrzavaBrod(brod))
                                                      .ToList();

        if (sviOdgovarajuciVezovi.Count == 0) {
            Alati.IspisiGresku("Nije moguće pronaći slobodan vez koji podržava brod.");
            return false;
        }

        Vez vez = sviOdgovarajuciVezovi.SortirajPoEkonomicnosti().First();
        vez.PrivezaniBrod = brod;
        vez.BrodPrivezanDo = datumKrajaZahtjeva;

        ZauzetostKreator zauzetostKreator = new RezervacijskaZauzetostKreator();

        vez.SveZauzetosti.Add(zauzetostKreator.KreirajZauzetost(new ParametriRezervacijskeZauzetosti(brod, vez,
            datumPocetkaZahtjeva, datumKrajaZahtjeva)));

        Alati.IspisiPotvrdu($"Brod {brod.Id} je uspješno zauzeo vez {vez.Id}.");
        return true;
    }

    public void PromjeniPostavkeTablicnogIspisa(bool prikaziZaglavlje, bool prikaziPodnozje, bool prikaziRedneBrojeve)
    {
        IspisivacTablice.Instance.IspisZaglavlja = prikaziZaglavlje;
        IspisivacTablice.Instance.IspisPodnozja = prikaziPodnozje;
        IspisivacTablice.Instance.IspisRednihBrojeva = prikaziRedneBrojeve;

        Alati.IspisiPotvrdu("Postavke tabličnog ispisa izmijenjene.");

        if (prikaziZaglavlje || prikaziPodnozje || prikaziRedneBrojeve) {
            Alati.IspisiPotvrdu("Tablica će sada ispisivati: " +
                                $"{(IspisivacTablice.Instance.IspisZaglavlja ? "ZAGLAVLJE " : "")} " +
                                $"{(IspisivacTablice.Instance.IspisPodnozja ? "PODNOŽJE " : "")} " +
                                $"{(IspisivacTablice.Instance.IspisRednihBrojeva ? "REDNE BROJEVE" : "")}");
        }
    }

    public void IspisiZauzetostVezovaPremaVrstiNaDatum(DateTime vrijemeOd, DateTime vrijemeDo)
    {
        var vezVisitor = new VezZauzetostPremaVrstamaVisitor(vrijemeOd, vrijemeDo);

        foreach (Vez vez in _brodskaLuka.SviVezovi) { vez.Accept(vezVisitor); }

        vezVisitor.IspisiZauzetostiPoVrstama();
    }

    public void SpojiSeNaKanal(Brod brod, int frekvencija, bool odjava)
    {
        if (odjava) {
            if (brod.Mediator == null) {
                Alati.IspisiGresku($"Brod {brod.Id} nije prijavljen na kanal, stoga se ne može odjaviti.");
                return;
            }

            brod.Mediator.PosaljiPoruku(new ZahtjevZaOdjavu(brod));
            return;
        }

        if (brod.Mediator != null) {
            Alati.IspisiGresku(
                $"Brod {brod.Id} je već prijavljen na kanal {((Kanal) brod.Mediator).Frekvencija}, stoga se ne može prijavljivati na ostale.");
            return;
        }

        Kanal? prikladniKanal =
            _brodskaLuka.SviKanali.FirstOrDefault(k =>
                k.Frekvencija == frekvencija && k.IspunjeniZahtjeviZaKomunikaciju());

        if (prikladniKanal == null) {
            Alati.IspisiGresku($"Kanal sa frekvencijom {frekvencija} nije ne postoji ili je zauzet.");
            return;
        }

        prikladniKanal.Subscribe(brod);
        brod.Mediator = prikladniKanal;
        Alati.IspisiPotvrdu($"Brod {brod.Id} je uspješno prijavljen na kanal {prikladniKanal.Id}.");
    }

    public void IspisiDnevnik()
    {
        var zaglavlje = new List<string> {
            "ID broda",
            "Tip zahtjeva",
            "Odobreno",
            "Datum od",
            "Datum do"
        };

        var podaci = new List<List<object>>();

        foreach (ZapisZahtjevaBroda zapisZahtjevaZaPrivez in _brodskaLuka.Dnevnik) {
            string tip = zapisZahtjevaZaPrivez.Tip switch {
                ZapisZahtjevaBroda.TipZahtjeva.RezerviraniVez => "Privez na rezervirani vez",
                ZapisZahtjevaBroda.TipZahtjeva.SlobodanVez    => "Privez na slobodni vez",
                ZapisZahtjevaBroda.TipZahtjeva.Odjava         => "Odjava sa veza",
                _                                             => throw new ArgumentOutOfRangeException()
            };

            var redak = new List<object> {
                zapisZahtjevaZaPrivez.Brod.Id,
                tip,
                zapisZahtjevaZaPrivez.Odobreno ? "DA" : "NE",
                zapisZahtjevaZaPrivez.OdobrenoOd.ToString(CultureInfo.CreateSpecificCulture("hr-HR")),
                zapisZahtjevaZaPrivez.OdobrenoDo.ToString(CultureInfo.CreateSpecificCulture("hr-HR"))
            };

            podaci.Add(redak);
        }

        IspisivacTablice.Instance.IspisiTablicu(zaglavlje, podaci);
    }

    public void OrganizirajHitniPrijevoz(int brojSluzbenika, int brojImigranta)
    {
        var hitniPrijevoz = new HitniPrijevoz(brojSluzbenika, brojImigranta);

        List<Brod> listaSvihPrivezanihBrodova = _brodskaLuka.SviBrodovi.Where(b => b.JePrivezan).ToList();

        if (listaSvihPrivezanihBrodova.Count == 0) {
            Alati.IspisiGresku("Niti jedan brod nije privezan, stoga se ne može organizirati hitni prijevoz.");
            return;
        }

        for (int index = 0; index < listaSvihPrivezanihBrodova.Count; index++) {
            Brod brod = listaSvihPrivezanihBrodova[index];

            if (index + 1 >= listaSvihPrivezanihBrodova.Count) { continue; }

            Brod sljedeciBrod = listaSvihPrivezanihBrodova[index + 1];
            ((IHitniPrijevozHandler) brod).SetNextHitniPrijevozHandler(sljedeciBrod);
        }

        listaSvihPrivezanihBrodova[0].HandleHitniPrijevoz(hitniPrijevoz);

        foreach (Brod brod in listaSvihPrivezanihBrodova) { brod.SetNextHitniPrijevozHandler(null); }

        if (hitniPrijevoz.PreostaloImigranta > 0 || hitniPrijevoz.PreostaloSluzbenika > 0) {
            Alati.IspisiGresku("Hitni prijevoz nije se uspio organizirati u potpunosti. " +
                               $"Preostalo je {hitniPrijevoz.PreostaloImigranta} imigranata i " +
                               $"{hitniPrijevoz.PreostaloSluzbenika} službenika.");
        } else {
            Alati.IspisiPotvrdu(
                "Hitni prijevoz je uspješno organiziran. Svi imigranti i službenici su dobili mjesto na nekom brodu.");
        }
    }
}
#region

using System.Diagnostics.CodeAnalysis;
using avranic_zadaca_1.Sucelja;

#endregion

namespace avranic_zadaca_1;

public class Vez
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum VrstaVeza
    {
        PU,
        PO,
        OS
    }

    public Brod? PrivezaniBrod { get; set; }
    public DateTime? BrodPrivezanDo { get; set; }

    public int CijenaPoSatu { get; }
    public int Id { get; }
    public int MaksimalnaDubina { get; }

    public int MaksimalnaDuljina { get; }
    public int MaksimalnaSirina { get; }
    public string Oznaka { get; }
    public VrstaVeza Vrsta { get; }
    public List<IZauzetost> SveZauzetosti { get; } = new();

    public Vez(int id,
               string oznaka,
               VrstaVeza vrsta,
               int cijenaPoSatu,
               int maksimalnaDuljina,
               int maksimalnaSirina,
               int maksimalnaDubina)
    {
        Id = id;
        Oznaka = oznaka;
        CijenaPoSatu = cijenaPoSatu;
        Vrsta = vrsta;
        MaksimalnaDuljina = maksimalnaDuljina;
        MaksimalnaSirina = maksimalnaSirina;
        MaksimalnaDubina = maksimalnaDubina;
    }

    public bool PodrzavaBrod(Brod brod)
    {
        bool zadovoljavaDimenzije = brod.Duljina <= MaksimalnaDuljina &&
                                    brod.Sirina <= MaksimalnaSirina &&
                                    brod.Gaz <= MaksimalnaDubina;

        bool zadovoljavaVrstu = Vrsta switch {
            VrstaVeza.PU => brod.Vrsta is Brod.VrstaBroda.TR or Brod.VrstaBroda.KA or Brod.VrstaBroda.KL
                or Brod.VrstaBroda.KR,
            VrstaVeza.PO => brod.Vrsta is Brod.VrstaBroda.RI or Brod.VrstaBroda.TE,
            VrstaVeza.OS => brod.Vrsta is Brod.VrstaBroda.JA or Brod.VrstaBroda.BR or Brod.VrstaBroda.RO,
            _            => false
        };

        return zadovoljavaDimenzije && zadovoljavaVrstu;
    }

    public bool RezerviranUDatumskomRasponu(DateTime datumOd, DateTime datumDo)
    {
        return SveZauzetosti.Any(z => z.ZauzetURasponu(datumOd, datumDo));
    }

    public bool RezerviranUDatumskomRasponu(DateTime datumOd,
                                            DateTime datumDo,
                                            out List<Tuple<int, DateTime, DateTime>> periodiRezerviranosti)
    {
        periodiRezerviranosti = new List<Tuple<int, DateTime, DateTime>>();

        foreach (IZauzetost zauzetost in SveZauzetosti) {
            if (zauzetost.ZauzetURasponu(datumOd, datumDo, out List<Tuple<int, DateTime, DateTime>> period)) {
                periodiRezerviranosti.AddRange(period);
            }
        }

        return periodiRezerviranosti.Count > 0;
    }
}
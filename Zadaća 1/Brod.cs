// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace avranic_zadaca_1;

public class Brod
{
    public enum VrstaBroda
    {
        TR,
        KA,
        KL,
        KR,
        RI,
        TE,
        JA,
        BR,
        RO
    }

    public float Duljina { get; }
    public float Gaz { get; }
    private float MaksimalnaBrzina { get; }
    public float Sirina { get; }

    public int Id { get; }
    private int KapacitetOsobnihVozila { get; }
    private int KapacitetPutnika { get; }
    private int KapacitetTereta { get; }
    private string Naziv { get; }
    private string OznakaBroda { get; }
    public VrstaBroda Vrsta { get; }

    public Brod(int id,
                string oznakaBroda,
                string naziv,
                VrstaBroda vrsta,
                float duljina,
                float sirina,
                float gaz,
                float maksimalnaBrzina,
                int kapacitetPutnika,
                int kapacitetOsobnihVozila,
                int kapacitetTereta)
    {
        Id = id;
        OznakaBroda = oznakaBroda;
        Naziv = naziv;
        Vrsta = vrsta;
        Duljina = duljina;
        Gaz = gaz;
        KapacitetOsobnihVozila = kapacitetOsobnihVozila;
        KapacitetPutnika = kapacitetPutnika;
        KapacitetTereta = kapacitetTereta;
        MaksimalnaBrzina = maksimalnaBrzina;
        Sirina = sirina;
    }
}
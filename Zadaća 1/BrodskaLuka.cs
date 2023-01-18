// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace avranic_zadaca_1;

public class BrodskaLuka
{
    private static BrodskaLuka? _instance;

    public static BrodskaLuka Instance => _instance ??= new BrodskaLuka();

    public int DubinaLuke { get; set; }
    public int UkupniBrojOstalihVezova { get; set; }
    public int UkupniBrojPoslovnihVezova { get; set; }
    public int UkupniBrojPutnickihVezova { get; set; }

    public List<Brod> SviBrodovi { get; set; } = new();
    public List<Raspored> SviRasporedi { get; set; } = new();
    public List<Vez> SviVezovi { get; set; } = new();
    public List<ZahtjevRezervacije> SviZahtjeviRezervacije { get; set; } = new();
    public string GpsSirina { get; set; } = null!;
    public string GpsVisina { get; set; } = null!;
    public string Naziv { get; set; } = null!;
    public DateTime InicijalnoVirtualnoVrijeme { get; set; }

    private BrodskaLuka() { }
}
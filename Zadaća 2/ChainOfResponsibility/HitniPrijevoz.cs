namespace avranic_zadaca_1.ChainOfResponsibility;

public class HitniPrijevoz
{
    public int PreostaloSluzbenika { get; set; }
    public int PreostaloImigranta { get; set; }

    public HitniPrijevoz(int preostaloSluzbenika, int preostaloImigranta)
    {
        PreostaloSluzbenika = preostaloSluzbenika;
        PreostaloImigranta = preostaloImigranta;
    }
}
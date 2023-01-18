namespace avranic_zadaca_1.Sucelja;

public class ParametriRasporedneZauzetosti : IParametriZauzetosti
{
    public Vez Vez { get; }
    public Brod Brod { get; }
    public List<int> Dani { get; }
    public TimeSpan VrijemeDo { get; }
    public TimeSpan VrijemeOd { get; }

    public ParametriRasporedneZauzetosti(Vez vez, Brod brod, List<int> dani, TimeSpan vrijemeOd, TimeSpan vrijemeDo)
    {
        Vez = vez;
        Brod = brod;
        Dani = dani;
        VrijemeDo = vrijemeDo;
        VrijemeOd = vrijemeOd;
    }
}
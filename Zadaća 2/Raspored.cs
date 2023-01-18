namespace avranic_zadaca_1;

public class Raspored
{
    public Brod Brod { get; }
    public List<int> Dani { get; }
    public TimeSpan VrijemeDo { get; }
    public TimeSpan VrijemeOd { get; }
    public Vez Vez { get; }

    public Raspored(Vez vez, Brod brod, List<int> dani, TimeSpan vrijemeOd, TimeSpan vrijemeDo)
    {
        Vez = vez;
        Brod = brod;
        Dani = dani;
        VrijemeOd = vrijemeOd;
        VrijemeDo = vrijemeDo;
    }
}
namespace avranic_zadaca_1.Sucelja;

public class RasporednaZauzetost : IZauzetost
{
    private Vez Vez { get; }
    public Brod Brod { get; }
    private List<int> Dani { get; }
    private TimeSpan VrijemeDo { get; }
    private TimeSpan VrijemeOd { get; }

    public RasporednaZauzetost(IParametriZauzetosti parametriZauzetosti)
    {
        var prz = (ParametriRasporedneZauzetosti) parametriZauzetosti;

        Vez = prz.Vez;
        Brod = prz.Brod;
        Dani = prz.Dani;
        VrijemeOd = prz.VrijemeOd;
        VrijemeDo = prz.VrijemeDo;
    }

    public bool ZauzetURasponu(DateTime datumOd, DateTime datumDo)
    {
        return ZauzetURasponu(datumOd, datumDo, out List<Tuple<int, DateTime, DateTime>> _);
    }

    public bool ZauzetURasponu(DateTime datumOd,
                               DateTime datumDo,
                               out List<Tuple<int, DateTime, DateTime>> rezerviraniRasponi)
    {
        int razlikaUDanima = (datumDo - datumOd).Days;
        var datumiKojiSeKose = new List<Tuple<int, DateTime, DateTime>>();

        for (int i = 0; i <= razlikaUDanima; i++) {
            DateTime datum = datumOd.AddDays(i);

            var datumOdPeriod = new DateTime(datum.Year, datum.Month, datum.Day, VrijemeOd.Hours, VrijemeOd.Minutes,
                VrijemeOd.Seconds);

            if (datum.TimeOfDay < VrijemeOd && datum.TimeOfDay < VrijemeDo && VrijemeOd > VrijemeDo) {
                DateTime prijasnjiDan = datumOdPeriod.Subtract(new TimeSpan(1, 0, 0, 0));

                if (Dani.Contains((int) prijasnjiDan.DayOfWeek)) { datumOdPeriod = prijasnjiDan; }
            }

            var datumDoPeriod = new DateTime(datumOdPeriod.Year, datumOdPeriod.Month, datumOdPeriod.Day,
                VrijemeDo.Hours, VrijemeDo.Minutes, VrijemeDo.Seconds);

            if (VrijemeOd >= VrijemeDo) { datumDoPeriod = datumDoPeriod.AddDays(1); }

            if (Dani.Contains((int) datumOdPeriod.DayOfWeek) &&
                ((IZauzetost) this).DatumiSePreklapaju(datumOd, datumDo, datumOdPeriod, datumDoPeriod)) {
                datumiKojiSeKose.Add(new Tuple<int, DateTime, DateTime>(Vez.Id, datumOdPeriod, datumDoPeriod));
            }
        }

        rezerviraniRasponi = datumiKojiSeKose.GroupBy(x => new { x.Item1, x.Item2 }).Select(x => x.First()).ToList();
        return rezerviraniRasponi.Count > 0;
    }
}
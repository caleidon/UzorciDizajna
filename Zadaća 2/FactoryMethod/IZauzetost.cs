namespace avranic_zadaca_1.Sucelja;

public interface IZauzetost
{
    public Brod Brod { get; }
    public bool ZauzetURasponu(DateTime datumOd, DateTime datumDo);

    public bool ZauzetURasponu(DateTime datumOd,
                               DateTime datumDo,
                               out List<Tuple<int, DateTime, DateTime>> rezerviraniRasponi);

    public bool DatumiSePreklapaju(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
    {
        return startA <= endB && endA >= startB;
    }
}
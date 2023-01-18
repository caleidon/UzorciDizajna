namespace avranic_zadaca_1.Sucelja;

public class RezervacijskaZauzetostKreator : ZauzetostKreator
{
    public override IZauzetost KreirajZauzetost(IParametriZauzetosti parametriZauzetosti)
    {
        return new RezervacijskaZauzetost(parametriZauzetosti);
    }
}
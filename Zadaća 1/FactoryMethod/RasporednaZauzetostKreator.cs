namespace avranic_zadaca_1.Sucelja;

public class RasporednaZauzetostKreator : ZauzetostKreator
{
    public override IZauzetost KreirajZauzetost(IParametriZauzetosti parametriZauzetosti)
    {
        return new RasporednaZauzetost(parametriZauzetosti);
    }
}
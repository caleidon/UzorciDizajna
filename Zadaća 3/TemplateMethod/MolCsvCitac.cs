#region

using avranic_zadaca_1.Utilities;

#endregion

namespace avranic_zadaca_1;

public class MolCsvCitac : CsvCitacAbstract<Mol>
{
    public MolCsvCitac(Model model) : base(model) { }

    protected override Mol ParsirajPodatke(string[] values, IEnumerable<Mol> postojeciPodaci, out int indexStupca)
    {
        indexStupca = -1;

        int id = int.Parse(values[++indexStupca]);
        string naziv = values[++indexStupca];

        if (postojeciPodaci.Any(mol => mol.Id == id)) { throw new PogresniPodaciException($"Mol {id} već postoji."); }

        return new Mol(id, naziv);
    }
}
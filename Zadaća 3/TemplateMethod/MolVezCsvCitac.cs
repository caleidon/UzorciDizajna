#region

using avranic_zadaca_1.Utilities;

#endregion

namespace avranic_zadaca_1;

public class MolVezCsvCitac : CsvCitacAbstract<MolVez>
{
    private readonly List<Vez> _vezovi;
    public MolVezCsvCitac(List<Vez> vezovi, Model model) : base(model) { _vezovi = vezovi; }

    protected override MolVez ParsirajPodatke(string[] values, IEnumerable<MolVez> postojeciPodaci, out int indexStupca)
    {
        indexStupca = -1;

        int idMola = int.Parse(values[++indexStupca]);

        Mol mol = BrodskaLuka.Instance.DajSveMolove().FirstOrDefault(m => m.Id == idMola) ??
                  throw new StavkaNijePronadenaException($"Mol {idMola} ne postoji.");

        string[] idVezoviStrings = values[++indexStupca].Split(",");

        int[] idVezovi = Array.ConvertAll(idVezoviStrings, int.Parse);
        IEnumerable<MolVez> postojeciMolVezovi = postojeciPodaci.ToList();

        foreach (int idVeza in idVezovi) {
            Vez? vez = _vezovi.FirstOrDefault(v => v.Id == idVeza);

            if (vez == null) {
                Model.IspisiGresku(
                    $"Greška u datoteci DZ_2_mol_vez.csv za mol {idMola}. Razlog: Vez {idVeza} ne postoji.", false);
                continue;
            }

            if (mol.DajSveVezove().Contains(vez)) {
                Model.IspisiGresku(
                    $"Greška u datoteci DZ_2_mol_vez.csv za mol {idMola}. Razlog: mol već sadrži vez {vez.Id}", false);
                continue;
            }

            if (postojeciMolVezovi.Any(m => m.Vezovi.Contains(vez))) {
                Model.IspisiGresku(
                    $"Greška u datoteci DZ_2_mol_vez.csv za mol {idMola}. Razlog: vez {vez.Id} već je dodijeljen drugome molu.",
                    false);
                continue;
            }

            mol.AddChild(vez);
        }

        MolVez? postojeciMolVez = postojeciMolVezovi.FirstOrDefault(m => m.Mol == mol);
        return postojeciMolVez ?? new MolVez(mol, mol.DajSveVezove());
    }
}
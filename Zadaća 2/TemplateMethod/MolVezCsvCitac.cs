#region

using avranic_zadaca_1.Utilities;

#endregion

namespace avranic_zadaca_1;

public class MolVezCsvCitac : CsvCitacAbstract<MolVez>
{
    protected override MolVez ParsirajPodatke(string[] values, IEnumerable<MolVez> postojeciPodaci, out int indexStupca)
    {
        indexStupca = -1;

        int idMola = int.Parse(values[++indexStupca]);

        Mol mol = BrodskaLuka.Instance.SviMolovi.FirstOrDefault(m => m.Id == idMola) ??
                  throw new StavkaNijePronadenaException($"Mol {idMola} ne postoji.");

        string[] idVezoviStrings = values[++indexStupca].Split(",");

        int[] idVezovi = Array.ConvertAll(idVezoviStrings, int.Parse);
        IEnumerable<MolVez> postojeciMolVezovi = postojeciPodaci.ToList();

        foreach (int idVeza in idVezovi) {
            Vez? vez = BrodskaLuka.Instance.SviVezovi.FirstOrDefault(v => v.Id == idVeza);

            if (vez == null) {
                Alati.IspisiGresku(
                    $"Greška u datoteci DZ_2_mol_vez.csv za mol {idMola}. Razlog: Vez {idVeza} ne postoji.");
                continue;
            }

            if (mol.Vezovi.Contains(vez)) {
                Alati.IspisiGresku(
                    $"Greška u datoteci DZ_2_mol_vez.csv za mol {idMola}. Razlog: mol već sadrži vez {vez.Id}");
                continue;
            }

            if (postojeciMolVezovi.Any(m => m.Vezovi.Contains(vez))) {
                Alati.IspisiGresku(
                    $"Greška u datoteci DZ_2_mol_vez.csv za mol {idMola}. Razlog: vez {vez.Id} već je dodijeljen drugome molu.");
                continue;
            }

            mol.Vezovi.Add(vez);
        }

        MolVez? postojeciMolVez = postojeciMolVezovi.FirstOrDefault(m => m.Mol == mol);
        return postojeciMolVez ?? new MolVez(mol, mol.Vezovi);
    }
}
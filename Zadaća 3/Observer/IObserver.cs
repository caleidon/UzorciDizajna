#region

using avranic_zadaca_1;

#endregion

public interface IObserver
{
    void Update(Model.IzvrsenaNaredba izvrsenaNaredba, bool odmahOsvjeziEkran);
}
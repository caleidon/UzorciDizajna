#region

using System.Timers;
using Timer = System.Timers.Timer;

#endregion

namespace avranic_zadaca_1;

public class VirtualniSat
{
    private static VirtualniSat? _instance;

    public static VirtualniSat Instance => _instance ??= new VirtualniSat();
    private readonly Timer _timer = new(1000);

    public static Action NaPromjenuVremena { get; set; } = delegate { };

    public DateTime VirtualnoVrijeme { get; set; }

    public void ZapocniVirtualniSat(DateTime vrijeme)
    {
        VirtualnoVrijeme = vrijeme;
        _timer.Elapsed += OnSecondPassed;
        _timer.Interval = 1000;
        _timer.Enabled = true;
    }

    private void OnSecondPassed(object? source, ElapsedEventArgs e)
    {
        VirtualnoVrijeme = VirtualnoVrijeme.AddSeconds(1);
        NaPromjenuVremena.Invoke();
    }
}
namespace avranic_zadaca_1.ChainOfResponsibility;

public interface IHitniPrijevozHandler
{
    public void SetNextHitniPrijevozHandler(IHitniPrijevozHandler handler);
    public void HandleHitniPrijevoz(HitniPrijevoz hitniPrijevoz);
}
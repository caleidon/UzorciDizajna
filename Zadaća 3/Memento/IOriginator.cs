public interface IOriginator
{
    public IMemento Save(string naziv);
    public void Restore(IMemento memento);
}
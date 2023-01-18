public interface IIterator<T>
{
    public T GetNext();
    public bool IsDone();
}
#region

#endregion

public abstract class Component
{
    public virtual void AddChild(Component component)
    {
        throw new ArgumentException($"Ne može se dodati dijete komponenti {GetType().Name}");
    }

    public virtual void RemoveChild(Component component)
    {
        throw new ArgumentException($"Ne može se oduzeti dijete komponenti {GetType().Name}");
    }

    public virtual bool IsComposite() { return true; }
}
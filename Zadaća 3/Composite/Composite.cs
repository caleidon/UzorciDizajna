public class Composite : Component
{
    protected List<Component> Djeca { get; } = new();

    public override void AddChild(Component component) { Djeca.Add(component); }

    public override void RemoveChild(Component component) { Djeca.Remove(component); }
}
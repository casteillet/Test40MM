public class ChangeFactionCommand : ICommand
{
    private Entity entity;
    private EntityFaction faction;

    public bool IsFinished => true;

    public ChangeFactionCommand(EntityFaction faction)
    {
        this.faction = faction;
    }

    public void Initialize(Entity entity)
    {
        this.entity = entity;
        entity.SetFaction(faction);
    }

    public void Update() { }
}
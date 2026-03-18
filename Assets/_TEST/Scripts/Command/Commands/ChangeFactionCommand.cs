public class ChangeFactionCommand : ICommand
{
    private Entity entity;
    private EntityThreat threat;

    public bool IsFinished => true;

    public ChangeFactionCommand(EntityThreat threat)
    {
        this.threat = threat;
    }

    public void Initialize(Entity entity)
    {
        this.entity = entity;
        entity.SetFaction(threat);
    }

    public void Update() { }
}
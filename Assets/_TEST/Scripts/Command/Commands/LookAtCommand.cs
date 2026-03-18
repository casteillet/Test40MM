using UnityEngine;

public class LookAtCommand : ICommand
{
    private Entity entity;
    private Transform target;

    public bool IsFinished => false;

    public LookAtCommand(Transform target)
    {
        this.target = target;
    }

    public void Initialize(Entity entity)
    {
        this.entity = entity;
    }

    public void Update()
    {
        var dir = target.position - entity.transform.position;

        // Move aim of this entity if it can
        //entity.transform.rotation = Quaternion.LookRotation(dir);
    }
}
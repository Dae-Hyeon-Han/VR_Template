using UnityEngine;

public class BaseController : MonoBehaviour, IController
{
    public virtual Vector3 Position { get; }
    public virtual Vector3 Forward { get; }

    public virtual bool TriggerPress()
    {
        return false;
    }

    public virtual bool TriggerDown()
    {
        return false;
    }

    public virtual bool TriggerUp()
    {
        return false;
    }

    public virtual bool GripPress()
    {
        return false;
    }

    public virtual bool GripDown()
    {
        return false;
    }

    public virtual bool GripUp()
    {
        return false;
    }

    public virtual bool LeftAxis()
    {
        return false;
    }

    public virtual bool RightAxis()
    {
        return false;
    }

    public virtual bool UpAxis()
    {
        return false;
    }

    public virtual bool DownAxis()
    {
        return false;
    }
}


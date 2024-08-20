using UnityEngine;

public enum eInteractiveType
{
    None = 0,
    UI,
    GrabObject
}

public class Interactive : MonoBehaviour
{
    [SerializeField]
    private eInteractiveType _type = eInteractiveType.None;

    public bool IsOn = false;


    public eInteractiveType Type
    {
        get { return _type; }
    }

    public virtual void On()
    {
        Debug.Log($"On {name}");

        gameObject.SetActive(true);
        IsOn = true;
    }

    public virtual void Off()
    {
        Debug.Log($"Off {name}");

        gameObject.SetActive(false);
        IsOn = false;
    }

    public virtual void Execute()
    {

    }

    public virtual void OnHover()
    {

    }

    public virtual void OnExit()
    {

    }
}

using UnityEngine;

public class WaitConditionAndActive : MonoBehaviour, IWaitCondition
{
    public string WaitConditionID;
    public bool ActiveValue = true;
    public string Name { get => gameObject.name; }
    public string ConditionID { get => this.WaitConditionID; }

    public void OnConditionChanged(string conditionID, bool value)
    {
        if (conditionID == WaitConditionID && value)
        {
            gameObject.SetActive(ActiveValue);

            //PlayerManager.Instance.SendHaptic();
        }
    }
}

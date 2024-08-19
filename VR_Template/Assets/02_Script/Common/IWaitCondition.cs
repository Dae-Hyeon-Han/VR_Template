public interface IWaitCondition
{
    void OnConditionChanged(string conditionID, bool value);
}
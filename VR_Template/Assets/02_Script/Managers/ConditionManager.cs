using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionsType
{
    None,
    OR,
    AND
}

[Serializable]
public class Condition
{
    public string ID { get; set; }
    public bool State { get; set; }
}

public class GroupCondition
{ 
    public string ID { get; set; }
    public string ActivityID { get; set; }
    public string Message { get; set; }
    public ConditionsType ConditionsType { get; private set; }
    public List<GroupCondition> Conditions { get; set; } = new List<GroupCondition>();

    public bool State
    {
        get
        {
            if (Conditions.Count > 0)
            {
                if (ConditionsType == ConditionsType.AND)
                {
                    foreach (var condition in Conditions)
                    {
                        if (condition.State == false)
                            return false;
                    }
                    return true;
                }
                else if (ConditionsType == ConditionsType.OR)
                {
                    foreach (var condition in Conditions)
                    {
                        if (condition.State == true)
                            return true;
                    }
                    return false;
                }
            }
            return ConditionManager.Instance.GetCondition(ID);
        }
    }

    public GroupCondition(string conditionIDs)
    {
        if (conditionIDs.Contains("/"))
        {
            foreach (var conditionID in conditionIDs.Split('/'))
            {
                var condition = new GroupCondition(conditionID.Trim());
                Conditions.Add(condition);
            }

            ConditionsType = ConditionsType.OR;
        }
        else if (conditionIDs.Contains("&"))
        {
            foreach (var conditionID in conditionIDs.Split('&'))
            {
                var condition = new GroupCondition(conditionID.Trim());
                Conditions.Add(condition);
            }

            ConditionsType = ConditionsType.AND;
        }
        else
        {
            if(conditionIDs.Contains("("))
            {
                var items = conditionIDs.Split(new char[] { '(', ')' });
                conditionIDs = items[0];
                Message = items[1];
            }

            if (conditionIDs.Contains("="))
            {
                var keyvalue = conditionIDs.Split('=');

                if (keyvalue.Length > 1)
                {
                    var conditionID = keyvalue[0];
                    var activityID = keyvalue[1];

                    ConditionsType = ConditionsType.None;
                    ID = conditionID.Trim();
                    ActivityID = activityID.Trim();
                }
            }
            else
            {
                ConditionsType = ConditionsType.None;
                ID = conditionIDs.Trim();
            }
        }
    }
    
    public (bool state, string conditionID, string activityID, string toastMessage) GetActiveConditions()
    {
        if (Conditions.Count > 0)
        {
            if (ConditionsType == ConditionsType.AND)
            {
                foreach (var condition in Conditions)
                {
                    var (state, id, actvitiyID, message) = condition.GetActiveConditions();
                    if (state == false)
                        return (false, null, null, null);
                }
                return (true, null, null, null);
            }
            else if (ConditionsType == ConditionsType.OR)
            {
                foreach (var condition in Conditions)
                {
                    var (state, conditionID, actvitiyID, message) = condition.GetActiveConditions();
                    if (state == true)
                        return (true, conditionID, actvitiyID, message);
                }
                return (false, null, null, null);
            }
        }

        return (ConditionManager.Instance.GetCondition(ID), ID, ActivityID, Message);
    }
}

public class ConditionManager
{
    public static ConditionManager Instance 
    {
        get
        {
            if (_instance == null)
                _instance = new ConditionManager();

            return _instance;
        }
    }
    private static ConditionManager _instance;
    
    public List<Condition> Conditions = new List<Condition>();

    private List<IWaitCondition> _waitConditions { get; set; } = new List<IWaitCondition>();
    private Dictionary<string, Condition> _conditionDictionary = new Dictionary<string, Condition>();

    public void Clear()
    {
        _conditionDictionary.Clear();
    }

    public bool GetCondition(string conditionID)
    {
        if (_conditionDictionary.ContainsKey(conditionID))
            return _conditionDictionary[conditionID].State;

        return false;
    }

    public void SetCondition(string conditionID, bool state = true)
    {
        if (string.IsNullOrEmpty(conditionID))
            return;

        Debug.Log($"SetCondition({conditionID}) = {state}");

        if (_conditionDictionary.ContainsKey(conditionID))
            _conditionDictionary[conditionID].State = state;
        else
        {
            Condition _condition = new Condition { ID = conditionID, State = true };
            _conditionDictionary[conditionID] = _condition;
        }

        lock (_waitConditions)
        {
            foreach (var waiter in _waitConditions)
            {
                waiter.OnConditionChanged(conditionID, state);
            }
        }
    }

    public IEnumerator WaitForConditions(GroupCondition groupCondition)
    {
        while (true)
        {
            if (groupCondition.State)
            {
                break;
            }

            yield return null;
        }        
    }

    internal void AddWaitConditions(IWaitCondition waitCondition)
    {
        lock (_waitConditions)
        {
            if (_waitConditions.Contains(waitCondition) == false)
            {
                _waitConditions.Add(waitCondition);
            }
        }
    }

    internal void RemoveWaitConditions(IWaitCondition waitCondition)
    {
        lock (_waitConditions)
        {
            _waitConditions.Remove(waitCondition);
        }
    }
}

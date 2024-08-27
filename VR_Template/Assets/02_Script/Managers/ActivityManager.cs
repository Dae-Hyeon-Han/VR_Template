using System.Collections.Generic;
using UnityEngine;

public class ActivityManager : MonoBehaviour
{
    public static ActivityManager Instance;
    private Dictionary<string, GameObject> _activity = new Dictionary<string, GameObject>();
    private GameObject _currentSelected = null;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }

        Instance.Load();
    }

    public void Load()
    {
        _activity.Clear();

        foreach (Transform child in transform.GetChildren(1))
        {
            _activity[child.name] = child.gameObject;
        }
        
        //Debug.Log("StepManager Initialized");
    }

    public void Select(string activityID)
    {
        if(_currentSelected != null)
        {
            foreach(var interactive in _currentSelected.GetComponentsInChildren<Interactive>())
            {
                interactive.Off();
            }
            _currentSelected = null;
        }

        if (!string.IsNullOrEmpty(activityID) && _activity.ContainsKey(activityID))
        {
            GameObject child = _activity[activityID];
            _currentSelected = child;

            foreach (var interactive in _currentSelected.GetComponentsInChildren<Interactive>())
            {
                interactive.On();
            }
        }
    }
}

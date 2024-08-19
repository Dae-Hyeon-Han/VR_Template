using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Stage
{
    public string ID;
    public GameObject StaticPlace;
    public GameObject DynamicObjectRoot;
    public AudioClip BgmAudioClip;

    private List<IWaitCondition> _addWaitConditions = new List<IWaitCondition>();

    public void Show()
    {
        if (StaticPlace != null)
        {
            StaticPlace.SetActive(true);
        }
        else
        {
            Debug.LogError("StaticPlace == null");
        }

        if (DynamicObjectRoot != null)
        {
            DynamicObjectRoot.SetActive(true);
            
            var components = DynamicObjectRoot.transform.GetAllComponentsInChildren<IWaitCondition>();

            foreach(var child in components)
            {
                _addWaitConditions.Add(child);
                ConditionManager.Instance.AddWaitConditions(child);
            }
        }
        else
        {
            Debug.LogError("DynamicObjectRoot == null");
        }
        
        SoundManager.Instance.PlayBGM(BgmAudioClip);
    }

    public void Hide()
    {
        foreach(var waitCondition in _addWaitConditions)
        {
            ConditionManager.Instance.RemoveWaitConditions(waitCondition);
        }

        _addWaitConditions.Clear();

        if (StaticPlace != null)
            StaticPlace.SetActive(false);

        if(DynamicObjectRoot != null)
            DynamicObjectRoot.SetActive(false);

        SoundManager.Instance?.StopBGM();
    }
}


public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; set; }
    public List<Stage> Stages;

    private Stage _currentStage;

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

        foreach(var stage in Stages)
        {
            stage.Hide();
        }
    }

    public void MoveToStage(string stageID)
    {
        if (_currentStage?.ID != stageID)
        {
            if (_currentStage != null)
            {
                _currentStage.Hide();
                _currentStage = null;
            }

            var stage = Stages.Where(o => o.ID == stageID).FirstOrDefault();

            if (stage != null)
            {
                stage.Show();
                _currentStage = stage;
            }
        }
    }

    public GameObject FindDynamicObject(string name)
    {
        if(_currentStage != null)
        {
            return _currentStage.DynamicObjectRoot.transform.FindDeepChild(name).gameObject;
        }

        return null;
    }

    public IEnumerable<TargetObject> FindTargetObjects(string targetObjects)
    {
        if (_currentStage?.DynamicObjectRoot != null)
        {
            foreach (var obj in _currentStage.DynamicObjectRoot.transform.FindTargetObjects(targetObjects))
            {
                yield return obj;
            }
        }
    }
}

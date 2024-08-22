using Scenario;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;
    public Dictionary<string, Transform> animDictionary;


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

        foreach (Transform childTransform in transform)
        {
            animDictionary[childTransform.name] = childTransform;
        }
    }

    public void PlayAnimation(string AnimationID)
    {
        if (animDictionary.ContainsKey(AnimationID))
        {
            animDictionary[AnimationID].GetComponents<AnimItem>();
        }
        else
        {
            Debug.Log($"{AnimationID}라는 애니메이션이 존재하지 않음");
        }
    }
}
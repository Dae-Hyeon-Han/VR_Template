using System.Collections.Generic;
using UnityEngine;


public class LocationManager : MonoBehaviour
{
    public static LocationManager Instance { get; set; }

    private Dictionary<string, Transform> locationDictionary = new Dictionary<string, Transform>();


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

        foreach(Transform childTransform in transform)
        {
            locationDictionary[childTransform.name] = childTransform;
        }
    }

    public Transform GetLocationTransform(string locationID)
    {
        if(locationDictionary.ContainsKey(locationID))
        {
            return locationDictionary[locationID];
        }

        return null;
    }


}

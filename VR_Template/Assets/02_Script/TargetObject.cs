using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HighlightPlus;
using Scenario;

[Serializable]
public class Touch
{
    public string Collider = "Ray";
    //public string ActivityID;
    public string SetConditionID;
    //public CustomDOTweenAnimation[] Dotweens;
}


public class TargetObject : MonoBehaviour
{
    public static TargetObject LastTargetObject 
    {
        get => _lastTargetObject;
        set
        {
            if (_lastTargetObject == value)
                return;
            
            _lastTargetObject?.Hover(false);
            _lastTargetObject = value;
            _lastTargetObject?.Hover(true);
        }
    }

    [Header("Indicator")]
    public bool UseIndicator = true;
    public Transform IndicatorCenter;
    public Vector3 OffsetIndicatorCenter;

    [Header("Highlight")]
    public bool UseHighlight = true;

    [Header("Condition")]
    public Touch[] Touches;

    [Header("Collider")]
    public bool AutoMeshCollider = true;

    private static TargetObject _lastTargetObject;
    private static HighlightProfile _highlightSetting = null;

    private bool _isHover;
    private string _setConditionID;
    private Activity _currentActivity;
    private HighlightEffect _highlightEffect = null;
    private List<MeshCollider> _meshColliders = new List<MeshCollider>();

    private void Awake()
    {
        if (_highlightSetting == null)
        {
            _highlightSetting = Resources.Load<HighlightProfile>("HighlightSettings");
        }

        if(Touches == null || Touches.Length == 0)
        {
            //Touches = new Touch[] { new Touch { Collider = "Ray", SetConditionID = name} };         // ConditionsIDs를 오브젝트 네임으로 지정하고, 컴포넌트 상에서 별다른 조치를 취해보지 말 것.
            Touches = new Touch[] { new Touch { Collider = "DefaultHand", SetConditionID = name + "_Touch" } };         // ConditionsIDs를 오브젝트 네임으로 지정하고, 컴포넌트 상에서 별다른 조치를 취해보지 말 것.
        }

        if (UseHighlight && _highlightEffect == null)
        {
            _highlightEffect = gameObject.AddComponent<HighlightEffect>();
            _highlightEffect.ProfileLoad(_highlightSetting);
            _highlightEffect.ignoreObjectVisibility = true;
            _highlightEffect.outlineColor = new Color32(255,160,160,255);
            _highlightEffect.highlighted = false;
        }

        if (IndicatorCenter == null && UseIndicator)
        {
            if (OffsetIndicatorCenter == Vector3.zero)
            {
                IndicatorCenter = transform;
            }
            else
            {
                IndicatorCenter = new GameObject("IndicatorCenter").transform;
                IndicatorCenter.parent = transform;
                IndicatorCenter.position = transform.position + OffsetIndicatorCenter;
            }
        }     
    }
    
    public IEnumerator SetEnable(bool enable)
    {
        if (enable)
        {
            if (gameObject.activeSelf == false)
            {
                gameObject.SetActive(true);
            }

            CreateMeshCollider();

            if (UseHighlight)
            {
                yield return new WaitUntil(() => _highlightEffect != null);
                _highlightEffect.highlighted = true;
            }

            if (IndicatorCenter == null && UseIndicator)
            {
                if(OffsetIndicatorCenter == Vector3.zero)
                {
                    IndicatorCenter = transform;
                }
                else
                {
                    IndicatorCenter = new GameObject("IndicatorCenter").transform;
                    IndicatorCenter.position = transform.position + OffsetIndicatorCenter;
                }
            }

            //if (IndicatorCenter != null)
            //    OffScreenIndicator.instance.AddIndicator(IndicatorCenter, 0);

            UpdateHover();
        }
        else
        {
            // RemoveMeshCollider();

            if (_highlightEffect != null)
                _highlightEffect.highlighted = false;


            //if (IndicatorCenter != null)
            //    OffScreenIndicator.instance.RemoveIndicator(IndicatorCenter);

            Hover(false);
        }
    }

    //private void CreateMeshCollider()
    //{
    //    if (AutoMeshCollider)
    //    {
    //        foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
    //        {
    //            if (meshFilter.gameObject.GetComponent<MeshCollider>() == null)
    //            {
    //                var meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
    //                meshCollider.sharedMesh = meshFilter.mesh;
    //                meshCollider.enabled = true;
    //                _meshColliders.Add(meshCollider);
    //            }
    //        }
    //    }
    //}

    private void CreateMeshCollider()
    {
        if (AutoMeshCollider)
        {
            foreach (var meshFilter in GetComponentsInChildren<MeshFilter>())
            {
                if (meshFilter.gameObject.GetComponent<MeshCollider>() == null)
                {
                    var meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = meshFilter.mesh;
                    meshCollider.enabled = true;
                    _meshColliders.Add(meshCollider);
                }
                else
                {
                    var meshCollider = meshFilter.GetComponent<MeshCollider>();
                    if(meshCollider.enabled == false)
                    {
                        meshCollider.enabled = true;
                    }
                    _meshColliders.Add(meshCollider);
                }
            }
        }
    }



    private void RemoveMeshCollider()
    {
        foreach(var collider in _meshColliders)
        {
            Destroy(collider);
        }
    }

    public void DisEnableMeshCollider()
    {
        foreach (var collider in _meshColliders)
        {
            collider.enabled = false;
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        _currentActivity = ChapterManager.Instance.CurrentActivity;

        if (_currentActivity.TargetObject != this.gameObject.name)
        {
            return; 
        }

        Debug.Log($"Touch {name}:{other.name}");
        Touch(other.name, true);
    }

    public void Touch(string colliderName, bool isHover)
    {
        _currentActivity = ChapterManager.Instance.CurrentActivity;

        //var touch = Touches.FirstOrDefault(o => o.ActivityID == _currentActivity.ID);
         var touch = Touches.FirstOrDefault(o => o.Collider == colliderName);
        
        if (touch == null)
            return;
        
        if (!touch.Collider.Equals(colliderName))
            return;


        Hover(isHover);
        if (!isHover) return;

        _setConditionID = touch.SetConditionID;


        //Debug.Log($"ㅇㅇㅇ{_currentActivity.ID}");
        AnimationManager.Instance.PlayAnimation(_currentActivity.ID);
        ConditionManager.Instance.SetCondition(_setConditionID);

        //SoundManager.Instance.CorrectPlayer.Play();

        //if (touch.Dotweens.Length == 0)
        //{

        //    FinishActivity(true);
        //    //if(Touches.Length > 1)
        //    //{
        //    //    Touches = Touches.Skip(1).ToArray();
        //    //}
        //    return;
        //}

        //foreach (var dotween in touch.Dotweens)
        //{
        //    if (dotween != null)
        //    {
        //        dotween.RewindThenRecreateTweenAndPlay();
        //    }
        //}
        DisEnableMeshCollider();

        

        //if (Touches.Length > 1)
        //{
        //    Touches = Touches.Skip(1).ToArray();
        //}
        Debug.Log($"{GetType()} - Last  _currentActivity.ID : {_currentActivity.ID}");

    }

    public void FinishActivity(bool isFinish)
    {
        if (isFinish && !String.IsNullOrEmpty(_setConditionID))
        {
            Debug.Log($"!!!!! Activity Finish -> {_setConditionID} SET !!!!!");
            ConditionManager.Instance.SetCondition(_setConditionID);
            _setConditionID = "";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (ChapterManager.Instance.CurrentActivity.TargetObject != this.gameObject.name)
            return;

        Touch(other.name, false);
    }

    private void Hover(bool isHover)
    {
        _isHover = isHover;

        UpdateHover();
    }
    
    private void UpdateHover()
    {
        if (_highlightEffect != null)
        {
            if (_isHover)
            {
                _highlightEffect.overlay = 0.5f;
                _highlightEffect.overlayBlending = 0.5f;
            }
            else
            {
                _highlightEffect.overlay = 0f;
            }
        }
    }
}

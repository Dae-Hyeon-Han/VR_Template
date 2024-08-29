using UnityEngine;
using System.Collections;
using Scenario;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; set; }

    public float fadeTime=1f;
    public float UpdateTime_WalkFastMessage = 0.0f;
    public string CurrentLocationID;
    public GameObject Player;
    public Transform LeftHand;
    public Transform RightHand;

    private bool _isYbuttonDown;
    private bool _isBbuttonDown;
    private bool _enableHaptic = true;
    private LeftController _leftController;
    private RightController _rightController;
    private Queue<float> _hapticQueue = new Queue<float>();
    private ContextSettings _currentSettings { get; set; }

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

        if (LeftHand == null)
        {
            Debug.LogError("LeftHand == null");
        }
        else
        {
            _leftController = LeftHand.GetComponent<LeftController>();
        }
       
        if (RightHand == null)
        {
            Debug.LogError("RightHand == null");
        }
        else
        {
            _rightController = RightHand.GetComponent<RightController>();
        }

        StartCoroutine(HapticPlay());

        RegisterWaitConditions();
    }

    private void RegisterWaitConditions()
    {
        foreach (var child in GetWaitConditions())
        {
            ConditionManager.Instance.AddWaitConditions(child);
        }
    }

    private IEnumerator HapticPlay()
    {
        while(gameObject.activeSelf)
        {
            if (_hapticQueue.Count > 0)
            {
                float duration = _hapticQueue.Dequeue();
                _rightController.Controller.SendHapticImpulse(0.5f, duration);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    internal void SendHaptic()
    {
        if(_enableHaptic)
            _hapticQueue.Enqueue(0.1f);
    }

    internal void SetEnableHaptic(bool enable)
    {
        _enableHaptic = enable;
    }

    public void MoveToLocation(string locationID)
    {
        //Debug.Log("로케이션");

        if (string.IsNullOrEmpty(locationID))
            return;

        if (CurrentLocationID != locationID)
        {
            Transform location = LocationManager.Instance.GetLocationTransform(locationID);

            if (location != null && Player != null)
            {
                Player.transform.position = location.position;
                Player.transform.rotation = location.rotation;
            }

            CurrentLocationID = locationID;
        }
    }

    public void SetLeftTool(Activity activity)
    {
        string toolID = activity.LeftHandToolID;
        string boardID = activity.LeftHandBoardID;

        if (string.IsNullOrEmpty(toolID))
            return;

        GameObject selectedHand = LeftHand.Find(toolID)?.gameObject;

        if (selectedHand == null)
        {
            selectedHand = LeftHand.Find("DefaultHand")?.gameObject;
        }

        foreach (Transform child in LeftHand)
        {
            if (selectedHand == child.gameObject)
            {
                child.gameObject.SetActive(true);
                var hand = child.gameObject.GetComponent<IHand>();
                
                if (hand != null)
                    hand.SetActivity(activity);
            }
            else
            {
                child.gameObject.SetActive(false);
            }

        }

        if (boardID.Contains("/"))
        {
            var resource = Resources.Load($"{boardID}");
            if (resource != null)
            {
                if (resource is Sprite sprite)
                {
                    var imageBoard = LeftHand.FindDeepChild("ImageBoard")?.GetComponent<Image>();
                    if (imageBoard != null)
                    {
                        imageBoard.sprite = sprite;
                        boardID = "ImageBoard";
                    }
                    else
                    {
                        Debug.LogError("ImageBoard not fond");
                    }
                }
            }
        }
    }

    public void SetRightTool(string toolID)
    {
        if (string.IsNullOrEmpty(toolID))
            return;

        if (toolID == "DefaultHand")
        {
            _rightController.LineRender.enabled = true;
        }
        else
        {
            _rightController.LineRender.enabled = false;
        }

        foreach (Transform child in RightHand)
        {
            child.gameObject.SetActive(false);

            if (toolID == child.gameObject.name)
            {
                child.gameObject.SetActive(true);
            }
        }   
    }

    public void SetControllerSettings(ContextSettings settings)
    {
        _currentSettings = settings;

        if (settings == null)
            return;

        _leftController.UpdateStatus(!settings.DisableLeftInput, !settings.DisableMove);
        _rightController.UpdateStatus(!settings.DisableRightInput, !settings.DisableTurn, !settings.DisableRay);
    }

    private void Update()
    {

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.B))
        {
            ConditionManager.Instance.SetCondition("BTN_NEXT");
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            ConditionManager.Instance.SetCondition("BTN_PREV");
        }

#endif

        /* 디버깅용
        if (_currentSettings != null)
        {
            if (_leftController.PressY && _leftController.GripDown())
            {
                Debug.Log("********** 테스트1 IF **********");
                if (Debugger.Instance.IsShow)
                {
                    Debugger.Instance.Hide();
                }
                else
                {
                    delayShowDebugger = StartCoroutine(Debugger.Instance.DelayShow());
                }
            }
            else
            {
                if (delayShowDebugger != null)
                {
                    StopCoroutine(delayShowDebugger);
                    delayShowDebugger = null;
                }
            }
        }
        */

        if (true)   //currentSettings != null && currentSettings.EnableButtonB)
        {
            bool Bdown = _rightController.PressB;

            if (_isBbuttonDown != Bdown)
            {
                _isBbuttonDown = Bdown;

                if (_isBbuttonDown)
                {
                    ConditionManager.Instance.SetCondition("BTN_NEXT");
                }
            }
        }

        if (_leftController.LeftAxis() || _leftController.RightAxis() ||
            _leftController.UpAxis() || _leftController.DownAxis())
        {
            UpdateTime_WalkFastMessage += Time.deltaTime;

            if (UpdateTime_WalkFastMessage > 0.2)
            {
                UpdateTime_WalkFastMessage = 0.0f;
            }
        }
    }

    private IEnumerable<IWaitCondition> GetWaitConditions()
    {
        foreach (var component in LeftHand.GetComponentsInChildren<IWaitCondition>(true))
        {
            yield return component;
        }
    }
}

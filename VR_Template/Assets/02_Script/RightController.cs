using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public enum eRightHandType
{
    DefaultHand,
    Grinder,
    UT_Equipment,
    WeldingMachine,
    Spray,
    MT_Equipment,
    MeasuringBar
}

[System.Serializable]
public class RightHands
{
    public eRightHandType Type;
    public GameObject Object;
}

public class RightController : BaseController
{
    [HideInInspector]
    public XRController Controller;
    public List<RightHands> Hands = new List<RightHands>();

    [Header ("Ray")]
    public Gradient ValidColor;
    public Gradient InvalidColor;
    public Transform LineStartPoint;
    public LineRenderer LineRender;

    [SerializeField]
    private DeviceBasedContinuousTurnProvider Turn;
    private bool _enableInput = true;
    private bool _isPressedTrigger = false;
    private bool _isPressingTrigger = false;
    private bool _isUnpressedTrigger = true;
    private bool _isPressedGrip = false;
    private bool _isPressingGrip = false;
    private bool _isUnpressedGrip = true;

    public Collider Collided { get; set; }
    public bool DisableRay => !LineRender.enabled;
    public bool PressB { get; private set; }

    private void Awake()
    {
        if (Controller == null)
            Controller = GetComponent<XRController>();

        if (LineRender == null)
            LineRender = GetComponent<LineRenderer>();

        LineRender.positionCount = 2;
    }

    #region Override Method
    public override Vector3 Position { get { return transform.position; } }
    public override Vector3 Forward { get { return transform.forward; } }
    
    public override bool TriggerPress()
    {
        return _isPressedTrigger;
    }
    
    public override bool TriggerDown()
    {
        return _isPressingTrigger;
    }
    
    public override bool GripPress()
    {
        return _isPressedGrip;
    }
    
    public override bool GripDown()
    {
        return _isPressingGrip;
    }
    #endregion

    public void UpdateStatus(bool enableInput, bool enableTurn, bool enableRay)
    {
        _enableInput = enableInput;

        if (!_enableInput)
        {
            Turn.enabled = false;
            LineRender.enabled = false;
        }
        else
        {
            Turn.enabled = enableTurn;
            LineRender.enabled = enableRay;
        }
    }

    public void UpdateLine(bool hit, float distance)
    {
        LineRender.colorGradient = hit ? ValidColor : InvalidColor;

        Vector3[] positions = new Vector3[2];
        positions[0] = LineStartPoint.position;
        positions[1] = LineStartPoint.position + LineStartPoint.forward * distance;
        LineRender.SetPositions(positions);
    }
    
    public void SetHand(eRightHandType type)
    {
        Hands.ForEach(o => o.Object.SetActive(false));
        RightHands hand = Hands.FirstOrDefault(o => o.Type == type);

        if (hand != null)
            hand.Object.SetActive(true);
        else
            Hands[0].Object.SetActive(true);

        if (type == eRightHandType.DefaultHand)
        {
            LineRender.enabled = true;
        }
        else
        {
            LineRender.enabled = false;
        }
    }
    
    private void Update()
    {
        if (Controller)
        {
            if (_enableInput)
            {
                UpdateTriggerButton();
                UpdateGripButton();
            }

            UpdateBButton();
        }
    }

    private void UpdateBButton()
    {
        Controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isPressedB);
        PressB = isPressedB;
    }

    private void UpdateTriggerButton()
    {
        Controller.inputDevice.TryGetFeatureValue(CommonUsages.triggerButton, out _isPressedTrigger);

        if (_isUnpressedTrigger)
            _isUnpressedTrigger = false;

        if (_isPressedTrigger && !_isPressingTrigger)
        {
            _isPressingTrigger = true;
            _isUnpressedTrigger = false;
        }
        else if (_isPressedTrigger && _isPressingTrigger)
        {
            _isPressingTrigger = false;
            _isUnpressedTrigger = true;
        }
    }

    private void UpdateGripButton()
    {
        Controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out _isPressedGrip);

        if (_isPressedGrip && !_isPressingGrip)
        {
            _isPressingGrip = true;
            _isUnpressedGrip = false;
        }
        else if (!_isPressedGrip && _isPressingGrip)
        {
            _isPressingGrip = false;
            _isUnpressedGrip = true;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        Collided = collider;
    }
}



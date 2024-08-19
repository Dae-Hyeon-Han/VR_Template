using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public enum eLeftHandType
{
    DefaultHand
}

[System.Serializable]
public class LeftHands
{
    public eLeftHandType Type;
    public GameObject Object;
}

public class LeftController : BaseController
{
    public List<LeftHands> Hands = new List<LeftHands>();

    [SerializeField] 
    private DeviceBasedContinuousMoveProvider Move;
    private XRController Controller;
    private bool enableInput = false;
    private bool isPressingX = false;
    private bool isUnpressedX = true;
    private bool isPressingGrip = false;
    private bool isUnpressedGrip = true;
    private Vector2 Axis = Vector3.zero;

    public Collider Collided { get; set; }
    public bool PressY { get; private set; }

    private void Awake()
    {
        if (Controller == null)
            Controller = GetComponent<XRController>();
      
        UpdateStatus(false, false);
    }

    #region Override Method
    public override bool GripDown()
    {
        return isPressingGrip;
    }

    public override bool LeftAxis()
    {
        return Axis.x < 0.0f;
    }
   
    public override bool RightAxis()
    {
        return Axis.x > 0.0f;
    }
    
    public override bool UpAxis()
    {
        return Axis.y > 0.0f;
    }
    
    public override bool DownAxis()
    {
        return Axis.y < 0.0f;
    }
    #endregion

    public void UpdateStatus(bool _enableInput, bool _enableMove)
    {
        enableInput = _enableInput;

        if (!enableInput)
        {
            Move.enabled = false;
        }
        else
        {
            Move.enabled = _enableMove;
        }
    }
   
    public void SetHand(eLeftHandType type)
    {
        Hands.ForEach(o => o.Object.SetActive(false));
        LeftHands hand = Hands.FirstOrDefault(o => o.Type == type);

        if (hand != null)
            hand.Object.SetActive(true);
        else
            Hands[0].Object.SetActive(true);
    }

    private void Update()
    {
        if (Controller)
        {
            if (enableInput)
            {
                UpdateXButton();
                UpdateYButton();
                UpdateGripButton();
            }
        }
    }

    private void UpdateXButton()
    {
        Controller.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool _isPressedX);

        if (_isPressedX && !isPressingX)
        {
            isPressingX = true;
            isUnpressedX = false;
        }
        else if (!_isPressedX && isPressingX)
        {
            isPressingX = false;
            isUnpressedX = true;
        }
    }

    private void UpdateYButton()
    {
        Controller.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool _isPressedY);

        if (_isPressedY && !PressY)
        {
            PressY = true;
        }
        else if (!_isPressedY && PressY)
        {
            PressY = false;
        }
    }

    private void UpdateGripButton()
    {
        Controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool _isPressedGrip);

        if (_isPressedGrip && !isPressingGrip)
        {
            isPressingGrip = true;
            isUnpressedGrip = false;
        }
        else if (!_isPressedGrip && isPressingGrip)
        {
            isPressingGrip = false;
            isUnpressedGrip = true;
        }
    }

    private void OnTriggerEnter(Collider _collider)
    {
        Collided = _collider;
    }
}

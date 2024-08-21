using UnityEngine;
using UnityEngine.XR;
using System.Linq;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public RightController RightController;

    private bool _isPressedTrigger = false;
    private RaycastHit _raycastHit;
    private Interactive _latestIR;

    private void Update()
    {
        bool isRayHit = false;
        RaycastHit[] hits = Physics.RaycastAll(RightController.Position, RightController.Forward);

        if (RightController.DisableRay)
            return;

        foreach (var hit in hits)
        {
            var listener = hit.transform.GetComponent<IRaycastHit>();
            if (listener != null)
            {
                listener.OnRaycastHit(_raycastHit);
            }
        }

        if (Physics.Raycast(RightController.Position, RightController.Forward, out _raycastHit))
        {
            Transform hitTransform = _raycastHit.transform;
            TargetObject targetObject = hitTransform.GetComponent<TargetObject>();
            TargetObject.LastTargetObject = targetObject;
            Interactive interactive = hitTransform.GetComponent<Interactive>();

            if (hitTransform.GetComponent<Button>() != null)
            {
                isRayHit = true;
            }

            if (hitTransform.GetComponent<Toggle>() != null)
            {
                isRayHit = true;
            }

            if (targetObject != null)
            {
                isRayHit = true;
            }

            if (interactive == null)
            {
                if (_latestIR != null)
                {
                    _latestIR.OnExit();
                }
            }

            if (_latestIR != interactive || _latestIR == null)
            {
                _latestIR = interactive;
                if (_latestIR != null)
                {
                    isRayHit = true;

                    FindObjectsOfType<Interactive>().ToList().ForEach(ir => ir.OnExit());
                    _latestIR.OnHover();
                }
            }

            if (RightController.TriggerPress() && !_isPressedTrigger)
            {
                _isPressedTrigger = true;

                TargetObject.LastTargetObject?.Touch("Ray", true);

                if (interactive)
                {
                    interactive.Execute();
                }
            }
            else if (!RightController.TriggerPress() && _isPressedTrigger)
            {
                _isPressedTrigger = false;
            }

        }
        else
        {
            TargetObject.LastTargetObject = null;

            _isPressedTrigger = true;

            if (_latestIR != null)
            {
                _latestIR.OnExit();
                _latestIR = null;
            }
        }

        if (isRayHit)
        {
            RightController.UpdateLine(true, _raycastHit.distance);
        }
        else
        {
            RightController.UpdateLine(false, 5.0f);
        }
    }
}

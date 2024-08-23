using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Test : MonoBehaviour
{
    public GameObject gObj;
    public Vector3 posi;
    public Vector3 rota;
    public Ease ease;
    public float delay;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Move();

        Debug.Log($"{gObj.transform.rotation}");
    }

    private void Move()
    {
        Vector3 vecPos = new Vector3(gObj.transform.position.x + posi.x, 
                                     gObj.transform.position.y + posi.y, 
                                     gObj.transform.position.z + posi.z);
        Vector3 vecRot = new Vector3(rota.x, 
                                     rota.y, 
                                     rota.z);
        gObj.transform.DOMove(vecPos, 0.5f).SetDelay(delay).SetEase(ease);
        gObj.transform.DOLocalRotate(vecRot, 0.5f,RotateMode.FastBeyond360).SetDelay(delay).SetEase(ease);

        //Debug.Log($"old: {gObj.transform.rotation.x}/{gObj.transform.rotation.y}/{gObj.transform.rotation.z}");
        //Debug.Log($"new: {vecRot.x}/{vecRot.y}/{vecRot.z}");
    }
}

using Scenario;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;
    public Dictionary<string, Transform> animDictionary;
    public AnimItem[] items;

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

        animDictionary = new Dictionary<string, Transform>();

        foreach (Transform childTransform in transform)
        {
            animDictionary[childTransform.name] = childTransform;
        }

        //items = animDictionary["U_006"].GetComponents<AnimItem>();

        //Debug.Log($"아이템: {items[0].duration} / {items[1].duration} / {items[2].duration} /");
    }

    public void PlayAnimation(string AnimationID)
    {
        if (animDictionary.ContainsKey(AnimationID))
        {
            // 오브젝트가 여러개의 컴포넌트를 갖고 있을 수 있으므로. 하나의 배열로 저장
            for (int i = 0; i < animDictionary[AnimationID].GetComponents<AnimItem>().Count(); i++)
            {
                items = animDictionary[AnimationID].GetComponents<AnimItem>();
            }

            // 나눈 이유? 클린한 코드를 위해
            for(int i=0; i<items.Count(); i++)
            {
                // 이동하기
                Vector3 vecPos = new Vector3(items[i].obj.transform.position.x + items[i].pos.x,
                                             items[i].obj.transform.position.y + items[i].pos.y,
                                             items[i].obj.transform.position.z + items[i].pos.z);

                items[i].obj.transform.DOMove(vecPos, items[i].duration).
                                       SetDelay(items[i].delayTime).
                                       SetEase(items[i].ease);

                // 회전하기
                Vector3 vecRot = new Vector3(items[i].rot.x, items[i].rot.y, items[i].rot.z);

                //Vector3 vecRot = new Vector3(items[i].obj.transform.rotation.x + items[i].rot.x,
                //                             items[i].obj.transform.rotation.y + items[i].rot.y,
                //                             items[i].obj.transform.rotation.z + items[i].rot.z);

                items[i].obj.transform.DORotate(vecRot, items[i].duration).
                                       SetDelay(items[i].delayTime).
                                       SetEase(items[i].ease);
            }

        }
        else
        {
            Debug.Log($"{AnimationID}라는 애니메이션이 존재하지 않음");
        }
    }
}
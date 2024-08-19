using Scenario;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class AnimationItem
{
    public string ID;
    public GameObject Timeline;
}

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    [Header("Animation Items")]
    public List<AnimationItem> Items;

    private PlayableDirector _currentPlayableDirector = null;
    private AnimationItem _currentAnimationItem = null;

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
    }

    public IEnumerator Play(Activity activity)
    {
        if (_currentPlayableDirector != null)
            Stop();

        if (string.IsNullOrEmpty(activity.AnimationID) == false)
        {
            if (activity.AnimationDelay != 0)
                yield return new WaitForSeconds(activity.AnimationDelay);


            var item = Items.Where(o => o.ID == activity.AnimationID).FirstOrDefault();
            if (item != null)
            {
                _currentAnimationItem = item;

                _currentAnimationItem.Timeline.SetActive(true);

                _currentPlayableDirector = item.Timeline.GetComponent<PlayableDirector>();
                _currentPlayableDirector.played += PlayCurrentPlayableDirector;
                _currentPlayableDirector.stopped += StopCurrentPlayableDirector;
                _currentPlayableDirector.Play();

            }
            else
            {
                Debug.LogError($"Not found AnimationID : {activity.AnimationID}");
            }
        }
        else
        {
            // 디버그로그가 너무 많아서 잠시 주석처리.
            // Debug.Log($"AnimationID is NullOrEmpty");
        }
    }

    private void PlayCurrentPlayableDirector(PlayableDirector playableDirector)
    {
        Debug.Log($"Animation Played");
    }

    private void StopCurrentPlayableDirector(PlayableDirector playableDirector)
    {
        Debug.Log($"Animation Stopted");
        ConditionManager.Instance.SetCondition("ANIMATION_END");
        playableDirector.gameObject.SetActive(false);
    }

    public void Stop()
    {
        if (_currentPlayableDirector != null)
        {
            _currentAnimationItem.Timeline.SetActive(false);

            _currentPlayableDirector.played -= PlayCurrentPlayableDirector;
            _currentPlayableDirector.stopped -= StopCurrentPlayableDirector;

            _currentPlayableDirector.Stop();
            _currentPlayableDirector = null;
        }
    }
}
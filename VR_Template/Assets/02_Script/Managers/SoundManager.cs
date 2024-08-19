using Scenario;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SoundItem
{
    public string ID;
    public AudioClip AudioClip;
}


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("BGM")]
    public AudioSource bgmPlayer = null;
    public float masterVolumeBGM = 1f;

    [Header("Narration")]
    public AudioSource narrationPlayer = null;
    public float masterVolumNarration = 1f;

    public List<SoundItem> SoundItems;

    [Header("OX")]
    public AudioSource CorrectPlayer = null;
    public AudioSource InCorrectPlayer = null;

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

        LoadSounds();
    }

    private void LoadSounds()
    {
        var soundPaths = new List<string>();
 
        soundPaths.Add("Sounds");

        foreach (var path in soundPaths)
        {
            var audioClips = Resources.LoadAll(path, typeof(AudioClip));
            foreach (AudioClip audioClip in audioClips)
            {
                SoundItems.Add(new SoundItem { ID = audioClip.name, AudioClip = audioClip });
            }
        }
    }

    public IEnumerator Play(Activity activity)
    {
        string soundID = activity.SoundID;
        int soundDelay = activity.SoundDelay;

        if (soundDelay != 0)
            yield return new WaitForSeconds(soundDelay);

        if(string.IsNullOrEmpty(soundID))
        {
            soundID = activity.ID.Replace("_", "");
            soundID = soundID + "_TTS";
        }

        var soundItem = SoundItems.Where(o => o.ID == soundID).FirstOrDefault();

        if (soundItem != null)
        {
            narrationPlayer.clip = soundItem.AudioClip;

            if (narrationPlayer.clip != null)
            {
                narrationPlayer.volume = masterVolumNarration;
                narrationPlayer.Play();
                Debug.Log($"나레이션 출력 {soundItem.ID}");

                yield return new WaitUntil(() => !narrationPlayer.isPlaying);

                ConditionManager.Instance.SetCondition("SOUND_END");
            }
            else
            {
                Debug.LogError($"[SoundManager] Sound ID:{soundID} has no sound clip");
            }
        }
        else
        {
            // 디버그로그가 너무 많아서 잠시 주석처리.
            // Debug.Log($"Sound Not Found : {soundID}");
        }
    }

    public void Stop()
    {
        if (narrationPlayer.isPlaying)
        {
            narrationPlayer.Stop();
            Debug.Log($"나레이션 정지");
        }
    }

    public void PlayBGM(string soundID)
    {
        var soundItem = SoundItems.Where(o => o.ID == soundID).FirstOrDefault();

        if (soundItem != null)
        {
            PlayBGM(soundItem.AudioClip);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("clip == null");
            return;
        }

        bgmPlayer.clip = clip;
        bgmPlayer.volume = masterVolumeBGM;
        bgmPlayer.Play();
        bgmPlayer.loop = true;
        Debug.Log("BGM 출력" + bgmPlayer.clip);
    }

    public void StopBGM()
    {
        bgmPlayer.Stop();
        Debug.Log($"BGM 정지");
    }
}
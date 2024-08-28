using DG.Tweening;
using Scenario;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChapterManager : MonoBehaviour
{
    public static ChapterManager Instance;

    public string ScenarioPath;
    // Scenario Document (JSON Format)
    public VRScenario Scenario { get; set; }
    // Activity List from Scenario Document for sequentail access  
    public List<Activity> Activities { get; set; } = new List<Activity>();
    public string CurrentScene { get; set; }
    public Activity CurrentActivity { get; set; }
    public Queue<Chapter> Chapters { get; set; }
    public int SelectedScenario { get; set; }
    public bool IsLoaded { get; set; }

    private string _gotoActivityID;
    private bool _isPlaying = false;
    private List<TargetObject> _targetObjects = new List<TargetObject>();
    private Queue<Activity> _playQueue = new Queue<Activity>();
    // Activity Dictionary from Scenario Document for random access 
    private Dictionary<string, Activity> _activityDictionary = new Dictionary<string, Activity>();

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

    private void Start()
    {
        StartCoroutine(PlayRoutine());

        Load(ScenarioPath);

        DOTween.Init(false, false, LogBehaviour.Default).SetCapacity(100, 20);
    }

    private void Load(string path)
    {
        LoadScenarioDoc(path);
        ScenarioPath = path;
        IsLoaded = true;

        Activity first = Activities.FirstOrDefault();

        if (first != null)
        {
            if (_playQueue.Count == 0)
            {
                EnqueuePlayQueue(first);
            }
            else
            {
                GotoActivity(first.ID);
            }
        }

        //if(InGameDebugger.Instance != null)
        //{
        //    InGameDebugger.Instance.UpdateCombo();
        //}
    }

    private void LoadScenarioDoc(string path)
    {
        Activities.Clear();

        var textData = Resources.Load(path) as TextAsset;
        Scenario = textData.text.ToObject<VRScenario>();

        Activity preActivity = null;

        List<Chapter> removeChapters = new List<Chapter>();
        List<Section> removeSections = new List<Section>();
        List<Activity> removeActivities = new List<Activity>();

        foreach (var part in Scenario.Parts)
        {
            foreach (var chapter in part.Chapters)
            {
                foreach (var section in chapter.Sections)
                {
                    foreach (var activity in section.Activities)
                    {
                        if (string.IsNullOrEmpty(activity?.ID?.Trim()))
                        {
                            removeActivities.Add(activity);
                            continue;
                        }

                        // auto fill
                        if (preActivity != null)
                        {
                            if (string.IsNullOrEmpty(activity.StageID))
                            {
                                activity.StageID = preActivity.StageID;
                            }
                        }

#if !UNITY_EDITOR
                        if(activity.ID == "DEBUG")
                        {
                            removeActivities.Add(activity);
                            continue;
                        }
#endif
                        _activityDictionary[activity.ID] = activity;
                        Activities.Add(activity);

                        if (preActivity != null && string.IsNullOrEmpty(preActivity.NextID))
                        {
                            activity.PrevID = preActivity.ID;
                            preActivity.NextID = activity.ID;
                        }

                        preActivity = activity;
                    }

                    foreach (var activity in removeActivities)
                    {
                        section.Activities.Remove(activity);
                    }


                    if (section.Activities.Count == 0)
                    {
                        removeSections.Add(section);
                    }
                }

                foreach (var section in removeSections)
                {
                    chapter.Sections.Remove(section);
                }
                removeSections.Clear();

                if (chapter.Sections.Count == 0)
                {
                    removeChapters.Add(chapter);
                }
            }

            foreach (var chapter in removeChapters)
            {
                part.Chapters.Remove(chapter);
            }
            removeChapters.Clear();
        }
    }

    private IEnumerator PlayRoutine()
    {
        _isPlaying = true;

        while (_isPlaying)
        {
            if (_playQueue.Count > 0)
            {
                var activity = _playQueue.Peek();
                Debug.Log($"======================================= Run Start - {activity.ID}");
                yield return Run(activity);
                // Debug.Log($"======================================= Run End - {activity.ID}");
                _playQueue.Dequeue();
            }

            yield return null;
        }
    }

    private IEnumerator Run(Activity activity)
    {
        bool enableFadeOutIn;

        //Debug.Log($"{activity.ID}");

        //if (ViveChanger.Instance.isVive)
        //{
        //    enableFadeOutIn = activity.PlayerLocationID != VivePlayerManager.Instance.CurrentLocationID || CurrentActivity?.Chapter != activity?.Chapter;
        //}
        //else
        //{
        //    enableFadeOutIn = activity.PlayerLocationID != PlayerManager.Instance.CurrentLocationID || CurrentActivity?.Chapter != activity?.Chapter;
        //}

        //if(activity.PopupID == "Basic_Event")
        //{
        //    enableFadeOutIn = true;
        //}

        //if (enableFadeOutIn && CurrentActivity != null)
        //{
        //    yield return fvScreenFade.instance.FadeOutSync();
        //}

        CurrentActivity = activity;

        //if (InGameDebugger.Instance != null)
        //{
        //    InGameDebugger.Instance.UpdateSelectCombo();
        //}

        // clear
        ConditionManager.Instance.Clear();
        //MediaPlayer.Instance.Stop();
        SoundManager.Instance.Stop();
        //AnimationManager.Instance.Stop();
        PopupManager.Instance.Hide();
        ActivityManager.Instance?.Select(null);

        DisableTargetObjects();

        // init
        StageManager.Instance.MoveToStage(activity.StageID);
        //PlayerManager.Instance.MoveToLocation(activity.PlayerLocationID);
        #region
        //if(ViveChanger.Instance.isVive)
        //{
        //    VivePlayerManager.Instance.MoveToLocation(activity.PlayerLocationID);
        //    VivePlayerManager.Instance.SetControllerSettings(activity.Settings);
        //    VivePlayerManager.Instance.SetLeftTool(activity);
        //    VivePlayerManager.Instance.SetRightTool(activity.RightHandToolID);
        //}
        //else
        //{
        //    PlayerManager.Instance.MoveToLocation(activity.PlayerLocationID);
        //    PlayerManager.Instance.SetControllerSettings(activity.Settings);
        //    PlayerManager.Instance.SetLeftTool(activity);
        //    PlayerManager.Instance.SetRightTool(activity.RightHandToolID);
        //}
        #endregion

        PlayerManager.Instance.MoveToLocation(activity.PlayerLocationID);
        PlayerManager.Instance.SetControllerSettings(activity.Settings);
        PlayerManager.Instance.SetLeftTool(activity);
        PlayerManager.Instance.SetRightTool(activity.RightHandToolID);
        //AnimationManager.Instance?.PlayAnimation(activity.AnimationID);
        ActivityManager.Instance?.Select(activity.ID);

        EnableTargetObjects(activity.TargetObject);

        SetPresetConditions(activity.PresetConditionIDs);

        //if (activity.Settings != null && activity.Settings.DisableWorkProcessBar)
        //    PopupManager.Instance.Hide("ProgressBar");
        //else
        //    PopupManager.Instance.Show("ProgressBar");

        string nextAcivityID = activity.NextID;

        switch (activity.Type)
        {
            case "VIDEO":
                //MediaPlayer.Instance.Play(activity.Data);
                break;
            default:
                StartCoroutine(PopupManager.Instance.Show(activity));
                StartCoroutine(SoundManager.Instance.Play(activity));
                //StartCoroutine(AnimationManager.Instance.Play(activity));
                break;
        }

        //if (enableFadeOutIn)
        //{
        //    yield return fvScreenFade.instance.FadeInSync();
        //}


        Coroutine durationCorountine = null;

        // Set Current Activity Basic(Default) Condition 
        //     => POPUP_DURATION_END : PopupDuration time in the activity
        //     => BTN_PREV : Previous Activity ID
        //     => BTN_NEXT : Next Activity ID 
        string conditions = $"POPUP_DURATION_END/BTN_PREV={activity.PrevID}/BTN_NEXT={activity.NextID}";

        if (string.IsNullOrEmpty(activity.ConditionIDs) == false)
        {
            conditions = $"{conditions}/{activity.ConditionIDs}";
        }
        else
        {
            if (activity.PopupDuration > 0)
            {
                conditions += $"POPUP_DURATION_END";
            }
            else
            {
                // If Current Activity has no condition, use current acitivity duration time as 3 sec.
                durationCorountine = StartCoroutine(Duration(3));
            }
        }

        var groupCondition = new GroupCondition(conditions);
        //Debug.Log($"Wait For Conditions : {activity.ConditionIDs}");

        bool retryWait;
        do
        {
            retryWait = false;

            yield return ConditionManager.Instance.WaitForConditions(groupCondition);

            if (durationCorountine != null)
                StopCoroutine(durationCorountine);

            if (_gotoActivityID != null)
            {
                nextAcivityID = _gotoActivityID;
                _gotoActivityID = null;
            }
            else
            {
                var (state, conditionID, activityID, message) = groupCondition.GetActiveConditions();

                if (state == true)
                {
                    if (message != null && message.Equals("GOOD", StringComparison.OrdinalIgnoreCase))
                    {
                        if (conditionID != null)
                            ConditionManager.Instance.SetCondition($"{conditionID}_GOOD");

                        yield return PopupManager.Instance.ShowGoodWaitClose();
                    }
                    else if (message != null && message.Equals("WRONG", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return PopupManager.Instance.ShowWrongWaitClose();

                        if (string.IsNullOrEmpty(activityID))
                        {
                            retryWait = true;
                            ConditionManager.Instance.Clear();
                            SetPresetConditions(activity.PresetConditionIDs);
                        }
                    }

                    if (activityID != null)
                        nextAcivityID = activityID;
                }
            }
        }
        while (retryWait);

        if (string.IsNullOrEmpty(nextAcivityID) == false)
        {
            EnqueuePlayQueue(nextAcivityID);
        }
        else
        {
            // todo end, start 처리
            Debug.Log("Application QUIT (+)");
            Stop();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
            Debug.Log("Application QUIT (-)");
            //EnqueuePlayQueue(activity.ID);
        }
    }

    private void SetPresetConditions(string presetConditionIDs)
    {
        if (string.IsNullOrEmpty(presetConditionIDs))
            return;

        //PlayerManager.Instance.SetEnableHaptic(false);

        foreach (var conditionID in presetConditionIDs.Split(','))
        {
            ConditionManager.Instance.SetCondition(conditionID.Trim());
        }

        //PlayerManager.Instance.SetEnableHaptic(true);
    }

    public void EnableTargetObjects(string targetObjects)
    {
        foreach (var targetObject in StageManager.Instance.FindTargetObjects(targetObjects))
        {
            //Debug.Log($"Enable TargetObject : {targetObject.name}");

            StartCoroutine(targetObject.SetEnable(true));
            _targetObjects.Add(targetObject);
        }
    }

    public void DisableTargetObjects()
    {
        foreach (var targetObject in _targetObjects)
        {
            StartCoroutine(targetObject.SetEnable(false));
        }
        _targetObjects.Clear();
    }

    private IEnumerator Duration(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        ConditionManager.Instance.SetCondition("BTN_NEXT");
    }

    private void EnqueuePlayQueue(string activityID)
    {
        if (_activityDictionary.ContainsKey(activityID))
        {
            _playQueue.Enqueue(_activityDictionary[activityID]);
        }
        else
        {
            Debug.LogError($"Not found activityID = {activityID}");
        }
    }

    private void EnqueuePlayQueue(Activity activity)
    {
        _playQueue.Enqueue(activity);
    }

    public void Stop()
    {
        _isPlaying = false;
        //MediaPlayer.Instance.Stop();
        PopupManager.Instance.Hide();
        SoundManager.Instance.Stop();
    }

    public void GotoActivity(string activityID)
    {
        _gotoActivityID = activityID;
        ConditionManager.Instance.SetCondition("BTN_NEXT");
    }
}

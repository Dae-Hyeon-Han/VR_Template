using Scenario;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PopupItem
{
    public string ID;
    public GameObject PopupObject;

    private List<WaitConditionAndActive> _addWaitConditions = new List<WaitConditionAndActive>();

    public void Show(Activity activity)
    {
        if(PopupObject != null)
        {
            PopupObject.SetActive(true);

            var popup = PopupObject.GetComponent<Popup>();
            var components = PopupObject.transform.GetAllComponentsInChildren<WaitConditionAndActive>();

            if (popup != null && activity != null)
            {
                popup.SetText(activity.PopupText1);
                popup.SetText2(activity.PopupText2);
                popup.SetImage(activity.PopupImage);
                popup.SetButtonText(activity.PopupButton1);
                popup.SetButtonText2(activity.PopupButton2);
                popup.SetToolTip(activity.Tooltip);
                popup.UpdateLayout();
            }

            foreach (var child in components)
            {
                _addWaitConditions.Add(child);
                ConditionManager.Instance.AddWaitConditions(child);
            }
        }
    }

    public void Hide()
    {
        foreach (var waitCondition in _addWaitConditions)
        {
            ConditionManager.Instance.RemoveWaitConditions(waitCondition);
        }
        _addWaitConditions.Clear();

        PopupObject?.SetActive(false);
    }

    public void AutoSetButtonAction()
    {
        if (PopupObject != null)
        {
            var buttons = PopupObject.transform.GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                if(button.onClick.GetPersistentEventCount() == 0)
                    button.onClick.AddListener(() => 
                    {
                        ConditionManager.Instance.SetCondition(button.name);
                    });
            }
        }
        else
        {
            Debug.LogError("PopupObject != null");
        }
    }
}


public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; set; }

    public Transform PopupRoot;
    public List<PopupItem> Popups = new List<PopupItem>();
    public List<PopupItem> CurrentPopups { get; set; } = new List<PopupItem>();

    private float _wrongDeactivateTime = 1f;


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

        LoadPopups();
    }

    private void LoadPopups()
    {
        if (PopupRoot != null)
        {
            foreach (Transform child in PopupRoot.transform)
            {
                var popUp = new PopupItem { ID = child.gameObject.name, PopupObject = child.gameObject };
                popUp.AutoSetButtonAction();
                Popups.Add(popUp);
                popUp.Hide();
            }
        }
    }

    public IEnumerator Show(Activity activity)
    {
        if (activity.PopupDelay != 0)
            yield return new WaitForSeconds(activity.PopupDelay);

        if (CurrentPopups.Count != 0)
            Hide();

        if (string.IsNullOrEmpty(activity.PopupID) == false)
        {
            var popupIDs = activity.PopupID.Split(',').Select(o => o.Trim());

            foreach(var popupID in popupIDs)
            {
                var popUp = Popups.Where(o => o.ID.Equals(popupID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if (popUp == null)
                {
                    var prefab = Resources.Load(popupID);
                    if (prefab != null)
                    {
                        popUp = new PopupItem { ID = popupID, PopupObject = Instantiate(prefab, PopupRoot) as GameObject };
                        popUp.AutoSetButtonAction();
                    }
                }

                if (popUp != null)
                {
                    popUp.Show(activity);
                    CurrentPopups.Add(popUp);
                }
                else
                {
                    Debug.LogError($"Popup not found : {popupID}");
                } 
            }
        }
        else
        {
            Debug.Log($"Popup is Empty");
        }

        yield return new WaitForSeconds(activity.PopupDuration);

        // If Popup duration is 0, it will wait finite time.
        // else it use "POPUP_DURATION_END" condition state
        if(activity.PopupDuration > 0)
            ConditionManager.Instance.SetCondition("POPUP_DURATION_END");
    }

    public void Hide()
    {
        foreach(var popup in CurrentPopups)
        {
            popup.Hide();
        }
        CurrentPopups.Clear();
    }

    public PopupItem GetPopupItem(string popupID)
    {
        return Popups.Where(o => o.ID.Equals(popupID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
    }

    public void Show(string popupID)
    {
        PopupItem popup = Popups.Where(o => o.ID.Equals(popupID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

        if (popup == null)
        {
            var prefab = Resources.Load(popupID);
            if (prefab != null)
            {
                popup = new PopupItem { ID = popupID, PopupObject = Instantiate(prefab, PopupRoot) as GameObject };
                popup.AutoSetButtonAction();
            }
        }

        if (popup != null)
        {
            popup.Show(null);
            Debug.Log($"Show popup : {popupID}");
        }
        else
        {
            Debug.LogError($"Popup not found : {popupID}");
        }
    }

    public void Hide(string popupID)
    {
        if(string.IsNullOrEmpty(popupID))
        {
            Debug.LogError($"Hide PopupID == null");
            return;
        }

        PopupItem popup = Popups.Where(o => o.ID.Equals(popupID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

        if (popup != null)
        {
            popup.Hide();
        }
        else
        {
            Debug.LogError($"Popup not found : {popupID}");
        }
    }

    internal void ShowGoodWithButton()
    {
        //PlayerManager.Instance.RightHand.GetComponent<RightController>().SetHand(eRightHandType.DefaultHand);
        Show("Snack_CheckButton");
    }

    internal void HideGood()
    {
        Hide("Snack_CheckButton");
    }

    public void ShowGood()
    {
        Show("Snack_Check");
        //PlayerManager.Instance.SendHaptic();
        Hide("Snack_Check", _wrongDeactivateTime);
    }

    public IEnumerator ShowGoodWaitClose()
    {
        Show("Snack_Check");
        //PlayerManager.Instance.SendHaptic();
        yield return DelayHide("Snack_Check", _wrongDeactivateTime);
    }

    public IEnumerator ShowWrongWaitClose()
    {
        Show("Snack_Error");
        //PlayerManager.Instance.SendHaptic();
        //PlayerManager.Instance.SendHaptic();
        yield return DelayHide("Snack_Error", _wrongDeactivateTime);
    }

    internal void ShowWrong()
    {
        Show("Snack_Error");
        //PlayerManager.Instance.SendHaptic();
        //PlayerManager.Instance.SendHaptic();
        Hide("Snack_Error", _wrongDeactivateTime);
    }

    public void Hide(string popupID, float delaySeconds)
    {
        StartCoroutine(DelayHide(popupID, delaySeconds));
    }

    private IEnumerator DelayHide(string popupID, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        Hide(popupID);
    }

    public GameObject GetPopupByName(string popupID)
    {
        PopupItem tmp = Popups.FirstOrDefault(popup => popup.ID == popupID);

        if(tmp != null)
        {
            return tmp.PopupObject;
        }

        return null;
    }
        
}

using Newtonsoft.Json;
using System;

namespace Scenario
{
    public enum AcitvityType
    {
        Popup,
        ConditionPopup,
    }

    [Serializable]
    public class Activity
    {
        [JsonIgnore]
        public int Index { get; set; }
        public string Part { get; set; }
        public string Chapter { get; set; }
        public string Section { get; set; }
        public string ID { get; set; }
        public string PrevID { get; set; }
        public string NextID { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }   // POPUP

        // Stage
        public string StageID { get; set; }
        public string SettingsEdit { get; set; }  // for edit
        public ContextSettings Settings { get; set; }
        
        // player
        public string PlayerLocationID { get; set; }
        public string LeftHandToolID { get; set; }
        public string LeftHandBoardID { get; set; }
        public string RightHandToolID { get; set; }
        public string TargetObject { get; set; }

        // popup
        public string PopupID { get; set; }
        public int PopupDelay { get; set; }
        public int PopupDuration { get; set; }
        public string PopupText1 { get; set; }
        public string PopupText2 { get; set; }
        public string PopupButton1 { get; set; }
        public string PopupButton2 { get; set; }
        public string PopupImage { get; set; }
        public string Tooltip { get; set; }

        // sound
        public string SoundID { get; set; }
        public int SoundDelay { get; set; }

        // animation
        public string AnimationID { get; set; }
        public int AnimationDelay { get; set; }

        public string Data { get; set; }

        // conditions
        public string PresetConditionIDs { get; set; }
        public string ConditionIDs { get; set; }

    }
}
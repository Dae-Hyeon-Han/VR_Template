namespace Scenario
{
    public class ContextSettings
    {
        #region Controller Settings
        public bool DisableButtonX { get; set; }
        public bool DisableButtonY { get; set; }
        public bool DisableButtonA { get; set; }
        public bool DisableButtonB { get; set; }

        public bool DisableLTrigger { get; set; }
        public bool DisableLGrip { get; set; }
        public bool DisableRTrigger { get; set; }
        public bool DisableRGrip { get; set; }

        public bool DisableMove { get; set; }
        public bool DisableTurn { get; set; }

        public bool DisableRay { get; set; }

        public bool DisableLeftInput { get; set; }
        public bool DisableRightInput { get; set; }
        #endregion

        #region UI Settings
        public bool DisableWorkProcessBar { get; set; }
        #endregion
    }
}

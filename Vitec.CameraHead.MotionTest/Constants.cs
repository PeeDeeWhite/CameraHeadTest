namespace Vitec.CameraHead.MotionTest {

    /// <summary>
    /// Constants used in application
    /// </summary>
    public struct Constants {
        public struct Configuration {
            public const string Targets = nameof(Targets);
            public const string CameraHeads = nameof(CameraHeads);
        }

        public struct ErrorMessages
        {
            public const string ConfigurationSectionNotDefined = "Section {0} not defined - Add User secrets or update appsettings.json";
            public const string TargetNamesRequired = "Target Names are required";
            public const string CameraHeadsRequired = "Camera heads are required";
            public const string CameraHeadTypeRequired = "A camera head type name is required";
            public const string InvalidCameraHeadsType = "Invalid camera head type {0}";
            public const string InvalidViewModelType = "Invalid View model type {0}";
        }
    }
}
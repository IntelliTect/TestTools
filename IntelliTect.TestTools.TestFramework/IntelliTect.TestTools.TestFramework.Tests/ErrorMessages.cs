namespace IntelliTect.TestTools.TestFramework.Tests
{
    public static class ErrorMessages
    {
        public const string ExecuteError = "there must be one and only one execute method";
        public const string MissingInputError = "unable to satisfy test block input";
        public const string MismatchedExecuteOverrideError = "unable to find corresponding execute parameter";
        public const string TooManyExecuteOverridesError = "too many execute overrides were provided";
        public const string AlreadyAddedError = "multiple execute argument overrides of the same type are not allowed";
    }
}

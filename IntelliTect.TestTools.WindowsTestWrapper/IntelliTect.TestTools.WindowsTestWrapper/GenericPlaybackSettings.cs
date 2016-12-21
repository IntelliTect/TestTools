using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Microsoft.VisualStudio.TestTools.UITesting;

namespace IntelliTect.TestTools.WindowsTestWrapper
{
    public static class GenericPlaybackSettings
    {
        /// <summary>
        /// General playback settings for an average application without terribly out of the ordinary quirks.
        /// These can be overriden by simply re-assiging the needed property after referencing this method at test setup/initialization.
        /// Most common override is Playback.PlaybackSettings.Searchtimeout, as it may take the playback engine longer to traverse the control hierarchies of some applications
        /// </summary>
        public static void SetPlaybackSettings()
        {
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.Disabled;
            Playback.PlaybackSettings.MaximumRetryCount = 5;
            Playback.PlaybackSettings.ShouldSearchFailFast = true;
            Playback.PlaybackSettings.DelayBetweenActions = 100;
            Playback.PlaybackSettings.SearchTimeout = 1000;
            Playback.PlaybackSettings.MatchExactHierarchy = false;

            //Tip from Stack Overflow to remove and then re-add error handling to get it to initialize properly
            //Don't know why this works, but it did when Mike Curn checked using VS2010.
            Playback.PlaybackError -= PlaybackErrorHandler;
            Playback.PlaybackError += PlaybackErrorHandler;
        }

        private static void PlaybackErrorHandler(object sender, PlaybackErrorEventArgs e)
        {
            System.Threading.Thread.Sleep(500);
            e.Result = PlaybackErrorOptions.Retry;
        }
    }
}

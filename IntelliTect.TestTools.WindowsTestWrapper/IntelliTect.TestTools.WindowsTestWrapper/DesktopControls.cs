using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WinControls;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntelliTect.TestTools.WindowsTestWrapper
{
    /// <summary>
    /// Finds WinForms or WPF controls on the windows desktop
    /// </summary>
    public abstract partial class DesktopControls
    {
        public static ApplicationUnderTest Aut { get; set; }
        public abstract string AutWindowTitle { get; }
        public abstract string WindowClassName { get; }
        
        /// <summary>
        /// Launch the application
        /// </summary>
        public static void LaunchApplication(string applicationLocation)
        {
            if ( applicationLocation == null )
                throw new ArgumentNullException( nameof( applicationLocation ) );
            Aut = ApplicationUnderTest.Launch(applicationLocation);
            Assert.IsTrue(Aut.Exists, "The application was not launched");
        }

        #region Find specific control by a unique identification

        /// <summary>
        /// Finds the first child of a given parent (window under test if parent left null)
        /// </summary>
        /// <param name="controlType">Control type of UITestControl or one of its derived classes. Declare like: c => new UiTestControl(c)</param>
        /// <param name="parent">The parent control if it is not the main window</param>
        /// <returns>Returns the found control</returns>
        protected T FindControlByChild<T>(Func<UITestControl, T> controlType, UITestControl parent = null) where T : UITestControl
        {
            if (controlType == null)
                throw new ArgumentNullException(nameof(controlType));
            parent = parent ?? FindWinWindowUnderTest();
            T found = controlType(parent);
            found.SearchConfigurations.Add(SearchConfiguration.DisambiguateChild);
            return found;
        }

        /// <summary>
        /// Finds a control given a name and parent (window under test if parent left null)
        /// </summary>
        /// <param name="controlType">Control type of UITestControl or one of its derived classes. Declare like: c => new UiTestControl(c)</param>
        /// <param name="parent">The parent control if it is not the main window</param>
        /// <returns>Returns the found control</returns>
        protected T FindControlByName<T>(string controlName, Func<UITestControl, T> controlType, UITestControl parent = null) where T : UITestControl
        {
            if (controlName == null)
                throw new ArgumentNullException(nameof(controlName));
            if (controlType == null)
                throw new ArgumentNullException(nameof(controlType));
            parent = parent ?? FindWinWindowUnderTest();
            T found = controlType(parent);
            found.SearchProperties.Add(UITestControl.PropertyNames.Name, controlName);
            found.SearchConfigurations.Add(SearchConfiguration.ExpandWhileSearching); //Is it worth it to do this everywhere?
            found.WindowTitles.Add(AutWindowTitle);
            return found;
        }

        /// <summary>
        /// Finds a control given an automationID and parent (window under test if parent left null)
        /// </summary>
        /// <param name="controlType">Control type of UITestControl or one of its derived classes. Declare like: c => new UiTestControl(c)</param>
        /// <param name="parent">The parent control if it is not the main window</param>
        /// <returns>Returns the found control</returns>
        protected T FindWpfControlByAutomationId<T>(string automationId, Func<UITestControl, T> controlType, UITestControl parent = null) where T : UITestControl
        {
            if (automationId == null)
                throw new ArgumentNullException(nameof(automationId));
            if (controlType == null)
                throw new ArgumentNullException(nameof(controlType));
            parent = parent ?? FindWinWindowUnderTest();
            T found = controlType(parent);
            found.SearchProperties.Add(WpfControl.PropertyNames.AutomationId, automationId);    //Should probably null check arguments like automationId
            found.SearchConfigurations.Add(SearchConfiguration.ExpandWhileSearching);
            found.WindowTitles.Add(AutWindowTitle);
            return found;
        }

        /// <summary>
        /// Finds a control given a WinRow value and parent (window under test if parent left null)
        /// </summary>
        /// <param name="controlType">Control type of UITestControl or one of its derived classes. Declare like: c => new UiTestControl(c)</param>
        /// <param name="parent">The parent control if it is not the main window</param>
        /// <returns>Returns the found control</returns>
        protected T FindWinRowByValue<T>(string value, Func<UITestControl, T> controlType, UITestControl parent = null)
                where T : UITestControl
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (controlType == null)
                throw new ArgumentNullException(nameof(controlType));
            parent = parent ?? FindWinWindowUnderTest();
            T found = controlType(parent);
            found.SearchProperties.Add(WinRow.PropertyNames.Value, value, PropertyExpressionOperator.Contains);
            found.SearchConfigurations.Add(SearchConfiguration.AlwaysSearch);
            found.WindowTitles.Add(AutWindowTitle);
            return found;
        }

        /// <summary>
        /// Find a WinForms control by its ControlID
        /// </summary>
        /// <param name="controlType">Control type of UITestControl or one of its derived classes. Declare like: c => new UiTestControl(c)</param>
        /// <param name="parent">The parent control if it is not the main window</param>
        /// <returns>Returns the found control</returns>
        protected T FindWinControlByControlId<T>(string controlId, Func<UITestControl, T> controlType, UITestControl parent = null)
                where T : UITestControl
        {
            if (controlId == null)
                throw new ArgumentNullException(nameof(controlId));
            if (controlType == null)
                throw new ArgumentNullException(nameof(controlType));
            parent = parent ?? FindWinWindowUnderTest();
            T found = controlType(parent);
            found.SearchProperties.Add(WinControl.PropertyNames.ControlId, controlId, PropertyExpressionOperator.Contains);
            found.SearchConfigurations.Add(SearchConfiguration.AlwaysSearch);
            found.WindowTitles.Add(AutWindowTitle);
            return found;
        }

        /// <summary>
        /// Gets an enumerable of all of a specific type of control given a known parent (window under test if null)
        /// </summary>
        /// <param name="controlType">Control type of UITestControl or one of its derived classes. Declare like: c => new UiTestControl(c)</param>
        /// <param name="parent">The parent control if it is not the main window</param>
        /// <returns>Returns the found control</returns>
        protected IEnumerable<T> GetListOfControlsByType<T>(Func<UITestControl, T> controlType, UITestControl parent = null) where T : UITestControl
        {
            if (controlType == null)
                throw new ArgumentNullException(nameof(controlType));
            parent = parent ?? FindWinWindowUnderTest();
            T found = controlType(parent);
            found.SearchConfigurations.Add(SearchConfiguration.DisambiguateChild);
            foreach (T childControl in found.FindMatchingControls())
            {
                yield return childControl;
            }
        }

        /// <summary>
        /// Finds the window under test
        /// Set by inherited class to account for applications that launch unparented windows after initial application launch
        /// Preferred method is to set:
        /// public override string AutWindowTitle => Aut?.Title;
        /// public override string WindowClassName => [classname];
        /// </summary>
        /// <returns>The window under test as defined by the AutWindowTitle property</returns>
        protected WinWindow FindWinWindowUnderTest()
        {
            var foundWindow = new WinWindow();
            foundWindow.SearchProperties.Add(new PropertyExpression(UITestControl.PropertyNames.Name, AutWindowTitle,
                PropertyExpressionOperator.Contains));
            foundWindow.SearchProperties.Add(new PropertyExpression(UITestControl.PropertyNames.ClassName, WindowClassName,
                PropertyExpressionOperator.Contains));
            foundWindow.WindowTitles.Add(AutWindowTitle);
            return foundWindow;
        }
        #endregion
    }
}

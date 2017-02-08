using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;

namespace IntelliTect.TestTools.WindowsTestWrapper
{
    public partial class DesktopControls
    {
        public enum SearchType
        {
            AutomationId,
            ControlName
        }

        /// <summary>
        /// Builds a heirarchy of generic controls by control name.
        /// </summary>
        /// <param name="searchType">Searcy by WPF Automation ID or control name</param>
        /// <param name="baseControl">The first control that needs to be found in the heirarchy</param>
        /// <param name="controlList">Subsequent controls to find in the heirarchy, ending with the specific control that needs to be acted on (leave blank if the control that 
        /// needs to be acted on is high enough in the heirarchy that it can be used as the baseControl argument</param>
        /// <returns>The last control listed if found</returns>
        protected UITestControl BuildControlHierarchy(SearchType searchType, string baseControl, params string[] controlList)
        {
            if (baseControl == null)
                throw new ArgumentNullException(nameof(baseControl));
            UITestControl foundControl;
            switch (searchType)
            {
                case SearchType.AutomationId:
                    foundControl = FindControlImpl((controlName, rootControl) => FindWpfControlByAutomationId(controlName, c => new WpfControl(c), rootControl), baseControl, controlList);
                    break;
                case SearchType.ControlName:
                    foundControl = FindControlImpl((controlName, rootControl) => FindControlByName(controlName, c => new UITestControl(c), rootControl), baseControl, controlList);
                    break;
                default:
                    foundControl = new UITestControl();
                    break;
            }
            return foundControl;
        }

        private UITestControl FindControlImpl(Func<string, UITestControl, UITestControl> findMethod, string baseControl, string[] childControls)
        {
            UITestControl foundControl = findMethod(baseControl, FindWinWindowUnderTest());
            if (childControls?.Any() == true)
            {
                foreach (string control in childControls)
                {
                    foundControl = findMethod(control, foundControl);
                }
            }
            return foundControl;
        }
    }
}
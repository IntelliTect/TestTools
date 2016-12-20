using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;

namespace IntelliTect.TestTools.WindowsTestWrapper
{
    public partial class DesktopControls
    {
        public enum SearchTypes
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
        protected UITestControl BuildControlHeirarchy(SearchTypes searchType, string baseControl, params string[] controlList)
        {
            if (searchType == null)
                throw new ArgumentNullException(nameof(searchType));
            if (baseControl == null)
                throw new ArgumentNullException(nameof(baseControl));

            UITestControl foundControl = new UITestControl();

            if (searchType == SearchTypes.AutomationId)
            {
                foundControl = FindWpfControlByAutomationId(baseControl, c => new WpfControl(c), FindWinWindowUnderTest());

                if ( controlList?.Any() != true )
                {
                    return foundControl;
                }

                if ( controlList.Any() )
                {
                    foreach (string control in controlList)
                    {
                        foundControl = FindWpfControlByAutomationId(control, c => new WpfControl(c), foundControl);
                    }
                }
            }

            if (searchType == SearchTypes.ControlName)
            {
                foundControl = FindControlByName(baseControl, c => new UITestControl(c), FindWinWindowUnderTest());

                if (controlList?.Any() != true)
                {
                    return foundControl;
                }

                if ( controlList.Any() )
                {
                    foreach (string control in controlList)
                    {
                        foundControl = FindControlByName(control, c => new UITestControl(c), foundControl);
                    }
                }
            }

            return foundControl;
        }
    }
}
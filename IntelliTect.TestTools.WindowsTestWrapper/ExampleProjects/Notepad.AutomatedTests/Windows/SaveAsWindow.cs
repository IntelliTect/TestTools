using System.Linq;
using IntelliTect.TestTools.WindowsTestWrapper;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WinControls;

namespace Notepad.AutomatedTests.Windows
{
    public class SaveAsWindow : DesktopControls
    {
        //Name of the window created when saving for the first time or choosing "Save As"
        public override string AutWindowTitle => "Save As";

        //Class of the window created when saving for the first time or choosing "Save As"
        public override string WindowClassName => "#32770";

        public WinControl DetailsPane
        {
            get { return FindControlByName("Details Pane", c => new WinControl(c)); }
        }

        public WinComboBox SaveAsBox
        {
            get { return FindControlByName("File name:", c => new WinComboBox(c), DetailsPane); }
        }

        public UITestControl SaveButton
        {
            get { return BuildControlHierarchy(SearchType.ControlName, "Save"); }
        }

        public UITestControl CancelButton
        {
            get { return BuildControlHierarchy(SearchType.ControlName, "Cancel"); }
        }

        public UITestControl FindSaveLocationToolbar
        {
            get
            {
                return GetListOfControlsByType(c => new WinToolBar(c)).FirstOrDefault(em => em.Name.Contains("Address"));
            }
        }
    }
}
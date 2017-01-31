using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IntelliTect.TestTools.WindowsTestWrapper;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.WpfControls;


namespace SimpleWpfApp.AutomatedUiTests.Windows
{
    public class SimpleWpfAppWindow : DesktopControls
    {
        public override string AutWindowTitle => Aut?.Title;

        //This will vary depending on if the playback engine is using MSAA or UIA.
        //More application support MSAA and so is a safer bet at this time.
        public override string WindowClassName => "HwndWrapper";

        public static void LaunchApplicationUnderTest()
        {
            LaunchApplication( Path.GetFullPath(@"..\..\..\ExampleProjects\SimpleWpfApp\bin\Debug\SimpleWpfApp.exe") );
        }

        public WpfEdit ListEntry
        {
            get { return FindWpfControlByAutomationId( "autoTextInput", c => new WpfEdit( c ) ); }
        }

        public UITestControl SaveButton
        {
            get { return BuildControlHierarchy( SearchType.AutomationId, "autoButtonSave" ); }
        }

        public UITestControl ExpandListItem
        {
            get { return BuildControlHierarchy( SearchType.AutomationId, "autoExpanderList", "HeaderSite" ); }
        }

        private UITestControl ListItems
        {
            get { return BuildControlHierarchy( SearchType.AutomationId, "autoExpanderList", "autoListBoxMyList" ); }
        }

        public IEnumerable<WpfListItem> GetAllListItems
        {
            get { return GetListOfControlsByType( c => new WpfListItem( c ), ListItems ); }
        }

        public bool CheckAllListItems(params string[] expectedItems)
        {
            if ( expectedItems == null )
                throw new ArgumentNullException( nameof( expectedItems ) );

            List<WpfListItem> existingListItems = GetAllListItems?.ToList();

            if ( expectedItems.Length != existingListItems?.Count() )
            {
                throw new Exception("CheckAllListItems parameter count did not match the number of ListItems returned");
            }
            bool allExpectedItemsPresent = true;

            for (int i = 0; i < expectedItems.Length; i++)
            {
                try
                {
                    if (existingListItems[i].GetChildren().FirstOrDefault()?.Name != expectedItems[i])
                    {
                        allExpectedItemsPresent = false;
                        break;
                    }
                    i++;
                }
                catch ( Exception e)
                {
                    Console.Write(e.Message);
                    throw;
                }
            }

            return allExpectedItemsPresent;
        }
    }
}

AspNetCore
==========

Provides test helper classes for instantiating ASP.NET Core classes like RoleManager and UserManager.

Windows UI Test Wrapper
===========

Provides wrappers for calling Microsoft's UiTestControl classes for WPF and WinForm applications in a more concise, reliable manner

Usage
-----
To use, inherit a class from the solution's DesktopControls class and make application-specific calls in the inherited class using generic types:
```
FindWpfControlByAutomationId( "textBoxControl1", c => new WpfEdit( c ) );
```

Inherit a class from BaseTestInherit and set the ApplicationLocation and create a new field for the above inherited class.

Inherit test classes from the BaseTestInherit inherited class, and call methods via the new field.

Example projects at https://github.com/IntelliTect/TestTools
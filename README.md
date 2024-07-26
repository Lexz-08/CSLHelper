# CSLHelper
## Description
Makes creating, destroying, and and printing to a console window easier, with support for virtual terminal sequences and basic text colors.

## How To Use
```csharp
using System.Windows.Forms;
using CSLHelper;

using Icon = System.Drawing.Icon;

namespace MyProgram
{
    public partial class MyForm : Form
    {
        InitializeComponent();

        // the 'IsVirtual' parameter defaults to 'false', so only put 'true' here if you want it enabled
        Console.Inst.CreateConsole("My Console Window", true);
        FormClosing += (_, __) => Console.Inst.DestroyConsole();

        Console.Inst.SetIcon(Icon); // sets the window caption icon and the taskbar icon of the console window
        Console.Inst.SetIconBig(Icon); // sets the taskbar icon of the console window
        Console.Inst.SetIconSmall(Icon); // sets the window caption icon of the console window

        // have following printed text be bright red and bright black (aka gray)
        Console.Inst.SetTextColor(TextColor.BrightRed, BackColor.BrightBlack);
        Console.Inst.WriteLine("Hello World!");
        Console.Inst.WriteLine("Look at my cool text!");

        // if you have 'IsVirtual' set to 'true' when calling 'Console.Inst.CreateConsole',
        // you can use ANSI escape codes to format the text and do other manipulations with the console window
        Console.Inst.Write("\x1b[H\x1b[2J"); // clear the console window

        // the following command is similar to calling:
        // Console.Inst.SetTextColor(TextColor.Red);
        // Console.Inst.Write("This is red text");
        Console.Inst.Write("\x1b[31mThis is red text"); // print red text
    }
}
```

## Download
[CSLHelper.dll](https://github.com/Lexz-08/CSLHelper/releases/latest/download/CSLHelper.dll)

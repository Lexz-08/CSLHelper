using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using Icon = System.Drawing.Icon;

namespace CSLHelper
{
	/// <summary>
	/// 
	/// </summary>
	public class Console
	{
		[DllImport("cslpp.dll", EntryPoint = "CreateConsole", CharSet = CharSet.Unicode)]
		private static extern int fnCreateConsole(string lpConsoleText);

		[DllImport("cslpp.dll", EntryPoint = "DestroyConsole")]
		private static extern int fnDestroyConsole();

		[DllImport("cslpp.dll", EntryPoint = "SetTextAttr")]
		private static extern int fnSetTextAttr(int nTextColor);

		[DllImport("cslpp.dll", EntryPoint = "SetMode")]
		private static extern int fnSetMode(int nConsoleMode);

		[DllImport("cslpp.dll", EntryPoint = "GetMode")]
		private static extern int fnGetMode(out int nConsoleMode);

		[DllImport("cslpp.dll", EntryPoint = "Write", CharSet = CharSet.Ansi)]
		private static extern int fnWrite(string lpOutputText);

		[DllImport("cslpp.dll", EntryPoint = "SetIcon")]
		private static extern int fnSetIcon(IntPtr hIcon);

		[DllImport("cslpp.dll", EntryPoint = "SetIconBig")]
		private static extern int fnSetIconBig(IntPtr hIcon);

		[DllImport("cslpp.dll", EntryPoint = "SetIconSmall")]
		private static extern int fnSetIconSmall(IntPtr hIcon);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool DestroyIcon(IntPtr hIcon);

		[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern IntPtr LoadImageW(IntPtr hIstance, string name, uint type, int cx, int cy, uint fuLoad);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr CreateIconFromResourceEx(
			IntPtr presbits, uint dwResSize, bool fIcon, uint dwVer,
			int cxDesired, int cyDesired, uint uFlags
			);

		private const uint IMAGE_ICON = 1;
		private const uint LR_LOADFROMFILE = 0x10;
		private const uint LR_DEFAULTSIZE = 0x40;
		private const uint VER30 = 0x00030000;

		private TextColor _textColor;
		private BackColor _backColor;

		/// <summary>
		/// The console window associated to the current process.
		/// </summary>
		public static readonly Console Inst = new Console();

		internal Console()
		{
			if (!File.Exists(Environment.CurrentDirectory + "\\cslpp.dll"))
			{
				using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("CSLHelper.cslpp.dll"))
				using (BinaryReader br = new BinaryReader(s))
				using (FileStream fs = new FileStream(Environment.CurrentDirectory + "\\cslpp.dll", FileMode.Create))
				using (BinaryWriter bw = new BinaryWriter(fs))
					bw.Write(br.ReadBytes((int)s.Length));
			}
		}

		/// <summary>
		/// Allocates a new console window and attaches it to the current process.
		/// </summary>
		/// <param name="Title">The title of the console window.</param>
		/// <param name="IsVirtual">Whether the console window should support
		/// <see href="https://learn.microsoft.com/en-us/windows/console/console-virtual-terminal-sequences">virtual terminal sequences</see>.<br/><br/>
		/// 
		/// If you will be using <see cref="SetTextColor(TextColor, BackColor)"/> to set console output text colors, leave this parameter alone.</param>
		/// <exception cref="Win32Exception"></exception>
		public void CreateConsole(string Title, bool IsVirtual = false)
		{
			int win32 = fnCreateConsole(Title);
			if (win32 != 0) throw new Win32Exception(win32);

			if (IsVirtual)
			{
				win32 = fnGetMode(out int mode);
				if (win32 != 0) throw new Win32Exception(win32);

				win32 = fnSetMode(mode | 0x0004);
				if (win32 != 0) throw new Win32Exception(win32);
			}
		}

		/// <summary>
		/// Destroys the console window associated with the current process.
		/// </summary>
		public void DestroyConsole()
		{
			int win32 = fnDestroyConsole();
			if (win32 != 0) throw new Win32Exception(win32);
		}

		/// <summary>
		/// Sets the console output text foreground or background.
		/// </summary>
		/// <param name="TextColor">The foreground color of the console output.</param>
		/// <param name="BackColor">The background color of the console output.</param>
		/// <exception cref="Win32Exception"></exception>
		public void SetTextColor(TextColor TextColor = TextColor.DEF, BackColor BackColor = BackColor.DEF)
		{
			if (TextColor == TextColor.DEF && BackColor == BackColor.DEF)
			{
				int win32 = fnSetTextAttr((int)(_textColor = TextColor.White) | (int)(_backColor = BackColor.Black));
				if (win32 != 0) throw new Win32Exception(win32);
			}
			else if (TextColor == TextColor.DEF && BackColor != BackColor.DEF)
				fnSetTextAttr((int)_textColor | (int)(_backColor = BackColor));
			else if (BackColor == BackColor.DEF && TextColor != TextColor.DEF)
				fnSetTextAttr((int)(_textColor = TextColor) | (int)_backColor);
			else fnSetTextAttr((int)(_textColor = TextColor) | (int)(_backColor = BackColor));
		}

		/// <summary>
		/// Writes the following value to the console output as a string.
		/// </summary>
		/// <param name="Value">The value to be written to the console output.</param>
		/// <exception cref="Win32Exception"></exception>
		public void Write(object Value)
		{
			int win32 = fnWrite(Value.ToString());
			if (win32 != 0) throw new Win32Exception(win32);
		}

		/// <summary>
		/// Writes the following value to the console output as a string with the line terminator following.
		/// </summary>
		/// <param name="Value">The value to be written to the console output.</param>
		/// <exception cref="Win32Exception"></exception>
		public void WriteLine(object Value) => Write(Value + "\n");

		/// <summary>
		/// Sets the window caption icon and taskbar icon of the console window using the given stream.
		/// </summary>
		/// <param name="Stream">The stream containing the buffer of bytes representing the icon to set.</param>
		public void SetIcon(Stream Stream) => fnSetIcon(LoadIconFromStream(Stream));

		/// <summary>
		/// Sets the window caption icon and taskbar icon of the console window using the given buffer.
		/// </summary>
		/// <param name="Buffer">The buffer of bytes representing the icon to set.</param>
		public void SetIcon(byte[] Buffer) => fnSetIcon(LoadIconFromBytes(Buffer));

		/// <summary>
		/// Sets the window caption icon and taskbar icon of the console window using the specified icon file.
		/// </summary>
		/// <param name="FileName">The file containing the icon to set.</param>
		public void SetIcon(string FileName) => fnSetIcon(LoadIconFromFile(FileName));

		/// <summary>
		/// Sets the window caption icon and taskbar icon of the console window using the given icon.
		/// </summary>
		/// <param name="Icon">The icon resource containing the icon to set.</param>
		public void SetIcon(Icon Icon) => SetIcon(Icon.Handle);

		/// <summary>
		/// Sets the window caption icon and taskbar icon of the console window using the given icon handle.
		/// </summary>
		/// <param name="hIcon">The handle to the icon resource containing the icon to set.</param>
		public void SetIcon(IntPtr hIcon) => fnSetIcon(hIcon);

		/// <summary>
		/// Sets the taskbar icon of the console window using the given stream.
		/// </summary>
		/// <param name="Stream">The stream containing the buffer of bytes representing the icon to set.</param>
		public void SetIconBig(Stream Stream) => fnSetIconBig(LoadIconFromStream(Stream));

		/// <summary>
		/// Sets the taskbar icon of the console window using the given buffer.
		/// </summary>
		/// <param name="Buffer">The buffer of bytes representing the icon to set.</param>
		public void SetIconBig(byte[] Buffer) => fnSetIconBig(LoadIconFromBytes(Buffer));

		/// <summary>
		/// Sets the taskbar icon of the console window using the specified icon file.
		/// </summary>
		/// <param name="FileName">The file containing the icon to set.</param>
		public void SetIconBig(string FileName) => fnSetIconBig(LoadIconFromFile(FileName));

		/// <summary>
		/// Sets the taskbar icon of the console window using the given icon.
		/// </summary>
		/// <param name="Icon">The icon resource containing the icon to set.</param>
		public void SetIconBig(Icon Icon) => SetIconBig(Icon.Handle);

		/// <summary>
		/// Sets the taskbar icon of the console window using the given icon handle.
		/// </summary>
		/// <param name="hIcon">The handle to the icon resource containing the icon to set.</param>
		public void SetIconBig(IntPtr hIcon) => fnSetIconBig(hIcon);

		/// <summary>
		/// Sets the window caption icon of the console window using the given stream.
		/// </summary>
		/// <param name="Stream">The stream containing the buffer of bytes representing the icon to set.</param>
		public void SetIconSmall(Stream Stream) => fnSetIconSmall(LoadIconFromStream(Stream));

		/// <summary>
		/// Sets the window caption icon of the console window using the given buffer.
		/// </summary>
		/// <param name="Buffer">The buffer of bytes representing the icon to set.</param>
		public void SetIconSmall(byte[] Buffer) => fnSetIconSmall(LoadIconFromBytes(Buffer));

		/// <summary>
		/// Sets the window caption icon of the console window using the specified icon file.
		/// </summary>
		/// <param name="FileName">The file containing the icon to set.</param>
		public void SetIconSmall(string FileName) => fnSetIconSmall(LoadIconFromFile(FileName));

		/// <summary>
		/// Sets the window caption icon of the console window using the given icon.
		/// </summary>
		/// <param name="Icon">The icon resource containing the icon to set.</param>
		public void SetIconSmall(Icon Icon) => SetIconSmall(Icon.Handle);

		/// <summary>
		/// Sets the window caption icon of the console window using the given icon handle.
		/// </summary>
		/// <param name="hIcon">The handle to the icon resource containing the icon to set.</param>
		public void SetIconSmall(IntPtr hIcon) => fnSetIconSmall(hIcon);

		private IntPtr LoadIconFromStream(Stream Stream)
		{
			using (MemoryStream mem = new MemoryStream())
			{
				Stream.CopyTo(mem);
				return LoadIconFromBytes(mem.ToArray());
			}
		}

		private IntPtr LoadIconFromBytes(byte[] Buffer)
		{
			GCHandle handle = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
			try
			{
				IntPtr hIcon = CreateIconFromResourceEx(handle.AddrOfPinnedObject(), (uint)Buffer.Length, true, VER30, 0, 0, LR_DEFAULTSIZE);
				if (hIcon == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());

				return hIcon;
			}
			finally { handle.Free(); }
		}

		private IntPtr LoadIconFromFile(string FileName)
		{
			IntPtr hIcon = LoadImageW(IntPtr.Zero, FileName, IMAGE_ICON, 0, 0, LR_LOADFROMFILE);
			if (hIcon == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error());

			return hIcon;
		}
	}

	/// <summary>
	/// Represents the foreground color attribute of the console output.
	/// </summary>
	[Flags]
	public enum TextColor
	{
		/// <summary>
		/// Used to represent the default foreground color when calling <see cref="Console.SetTextColor(TextColor, BackColor)"/>.<br/><br/>
		/// 
		/// <i>Do not pass this value, it is just a flag value.</i>
		/// </summary>
		DEF = int.MaxValue,

		// -- Defaults -- //

		/// <summary>
		/// The console output should contain blue in its foreground color.
		/// </summary>
		Blue = 0x0001,

		/// <summary>
		/// The console output should contain green in its foreground color.
		/// </summary>
		Green = 0x0002,

		/// <summary>
		/// The console output should contain red in its foreground color.
		/// </summary>
		Red = 0x0004,


		// -- Extended -- //

		/// <summary>
		/// The console output should have no red, green, or blue in its foreground color.
		/// </summary>
		Black = 0x0000,

		/// <summary>
		/// The console output should contain red, green, and blue in its foreground color.
		/// </summary>
		White = Blue | Green | Red,

		/// <summary>
		/// The console output should contain blue and green in its foreground color.
		/// </summary>
		Cyan = Blue | Green,

		/// <summary>
		/// The console output should contain blue and red in its foreground color.
		/// </summary>
		Purple = Blue | Red,

		/// <summary>
		/// The console output should contain green and red in its foreground color.
		/// </summary>
		Yellow = Green | Red,

		/// <summary>
		/// The console output should contain blue in its foreground color with brightened intensity.
		/// </summary>
		BrightBlue = Blue | 0x0008,

		/// <summary>
		/// The console output should contain blue in its foreground color with brightened intensity.
		/// </summary>
		BrightGreen = Green | 0x0008,

		/// <summary>
		/// The console output should contain blue in its foreground color with brightened intensity.
		/// </summary>
		BrightRed = Red | 0x0008,

		/// <summary>
		/// The console output should contain blue and green in its foreground color with brightened intensity.
		/// </summary>
		BrightCyan = Cyan | 0x0008,

		/// <summary>
		/// The console output should contain blue and red in its foreground color with brightened intensity.
		/// </summary>
		BrightPurple = Purple | 0x0008,

		/// <summary>
		/// The console output should contain green and red in its foreground color with brightened intensity.
		/// </summary>
		BrightYellow = Yellow | 0x0008,

		/// <summary>
		/// The console output should contain red, green and blue in its foreground color with brightened intensity.
		/// </summary>
		BrightWhite = White | 0x0008,

		/// <summary>
		/// The console output should have no red, green, or blue in its foreground color.
		/// </summary>
		BrightBlack = Black | 0x0008
	}

	/// <summary>
	/// Represents the background color attribute of the console output.
	/// </summary>
	[Flags]
	public enum BackColor
	{
		/// <summary>
		/// Used to represent the default background color when calling <see cref="Console.SetTextColor(TextColor, BackColor)"/>.<br/><br/>
		/// 
		/// <i>Do not pass this value, it is just a flag value.</i>
		/// </summary>
		DEF = int.MaxValue,

		// -- Defaults -- //

		/// <summary>
		/// The console output should contain blue in its background color.
		/// </summary>
		Blue = 0x0010,

		/// <summary>
		/// The console output should contain green in its background color.
		/// </summary>
		Green = 0x0020,

		/// <summary>
		/// The console output should contain red in its background color.
		/// </summary>
		Red = 0x0040,

		/// <summary>
		/// The console output background color should be brighter.
		/// </summary>
		Bright = 0x0080,


		// -- Extended -- //

		/// <summary>
		/// The console output should have no red, green, or blue in its background color.
		/// </summary>
		Black = 0x0000,

		/// <summary>
		/// The console output should contain red, green, and blue in its background color.
		/// </summary>
		White = Blue | Green | Red,

		/// <summary>
		/// The console output should contain blue and green in its background color.
		/// </summary>
		Cyan = Blue | Green,

		/// <summary>
		/// The console output should contain blue and red in its background color.
		/// </summary>
		Purple = Blue | Red,

		/// <summary>
		/// The console output should contain green and red in its background color.
		/// </summary>
		Yellow = Green | Red,

		/// <summary>
		/// The console output should contain blue in its background color with brightened intensity.
		/// </summary>
		BrightBlue = Blue | 0x0008,

		/// <summary>
		/// The console output should contain blue in its background color with brightened intensity.
		/// </summary>
		BrightGreen = Green | 0x0008,

		/// <summary>
		/// The console output should contain blue in its background color with brightened intensity.
		/// </summary>
		BrightRed = Red | 0x0008,

		/// <summary>
		/// The console output should contain blue and green in its background color with brightened intensity.
		/// </summary>
		BrightCyan = Cyan | 0x0008,

		/// <summary>
		/// The console output should contain blue and red in its background color with brightened intensity.
		/// </summary>
		BrightPurple = Purple | 0x0008,

		/// <summary>
		/// The console output should contain green and red in its background color with brightened intensity.
		/// </summary>
		BrightYellow = Yellow | 0x0008,

		/// <summary>
		/// The console output should contain red, green and blue in its background color with brightened intensity.
		/// </summary>
		BrightWhite = White | 0x0008,

		/// <summary>
		/// The console output should have no red, green, or blue in its background color.
		/// </summary>
		BrightBlack = Black | 0x0008
	}
}

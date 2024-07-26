using System.Windows.Forms;

using CSLHelper;

namespace DEMO
{
	public partial class Main : Form
	{
		public Main()
		{
			InitializeComponent();

			Console.Inst.CreateConsole("test");
			FormClosing += (_, __) => Console.Inst.DestroyConsole();

			Console.Inst.SetIcon(Icon);

			Console.Inst.SetTextColor(TextColor.Red);
			Console.Inst.WriteLine("(RED)           The quick brown fox jumps over the lazy dog.");
			Console.Inst.SetTextColor(TextColor.Green);
			Console.Inst.WriteLine("(GREEN)         The quick brown fox jumps over the lazy dog.");
			Console.Inst.SetTextColor(TextColor.Blue);
			Console.Inst.WriteLine("(BLUE)          The quick brown fox jumps over the lazy dog.");
			Console.Inst.SetTextColor(TextColor.Yellow);
			Console.Inst.WriteLine("(YELLOW)        The quick brown fox jumps over the lazy dog.");
			Console.Inst.SetTextColor(TextColor.Purple);
			Console.Inst.WriteLine("(PURPLE)        The quick brown fox jumps over the lazy dog.");
			Console.Inst.SetTextColor(TextColor.Cyan);
			Console.Inst.WriteLine("(CYAN)          The quick brown fox jumps over the lazy dog.");
			Console.Inst.SetTextColor(TextColor.White);
			Console.Inst.WriteLine("(WHITE)         The quick brown fox jumps over the lazy dog.");
			Console.Inst.SetTextColor(TextColor.Black, CSLHelper.BackColor.White);
			Console.Inst.WriteLine("(BLACK)         The quick brown fox jumps over the lazy dog.");
			Console.Inst.SetTextColor(BackColor: CSLHelper.BackColor.Black);
			Console.Inst.SetTextColor(TextColor.BrightRed);
			Console.Inst.WriteLine("(BRIGHT RED)    The quick brown fox jumps over the lazy dog.");
			Console.Inst.SetTextColor(TextColor.BrightGreen);
			Console.Inst.WriteLine("(BRIGHT GREEN)  The quick brown fox jumps over the lazy dog.");
			Console.Inst.SetTextColor(TextColor.BrightBlue);
			Console.Inst.WriteLine("(BRIGHT BLUE)   The quick brown fox jumps over the lazy dog.");
			Console.Inst.SetTextColor(TextColor.BrightYellow);
			Console.Inst.WriteLine("(BRIGHT YELLOW) The quick brown fox jumps over the lazy dog.");
			Console.Inst.SetTextColor(TextColor.BrightPurple);
			Console.Inst.WriteLine("(BRIGHT PURPLE) The quick brown fox jumps over the lazy dog.");
			Console.Inst.SetTextColor(TextColor.BrightCyan);
			Console.Inst.WriteLine("(BRIGHT CYAN)   The quick brown fox jumps over the lazy dog.");
			Console.Inst.SetTextColor(TextColor.BrightWhite);
			Console.Inst.WriteLine("(BRIGHT WHITE)  The quick brown fox jumps over the lazy dog.");
			Console.Inst.SetTextColor(TextColor.BrightBlack);
			Console.Inst.WriteLine("(BRIGHT BLACK)  The quick brown fox jumps over the lazy dog.");
		}
	}
}

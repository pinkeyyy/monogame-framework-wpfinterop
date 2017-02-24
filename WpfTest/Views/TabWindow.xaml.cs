using System;

namespace WpfTest.Views
{
	/// <summary>
	/// Interaction logic for TabWindow.xaml
	/// </summary>
	public partial class TabWindow
	{
		public TabWindow()
		{
			InitializeComponent();
		}

		public void Log(string message)
		{
			var now = DateTime.Now.TimeOfDay;
			LogOutput.AppendText($"{now}: {message}{Environment.NewLine}");
			LogOutput.ScrollToEnd();
		}
	}
}

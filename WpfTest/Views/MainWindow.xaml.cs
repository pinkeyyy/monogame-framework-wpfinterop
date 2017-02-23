using System.Windows;

namespace WpfTest.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void OpenNewWindow(object sender, RoutedEventArgs e)
		{
			var w = new SimpleWindow();
			w.Show();
		}

		private void OpenTextInputWindow(object sender, RoutedEventArgs e)
		{
			var w = new TextInputWindow();
			w.Show();
		}

		private void OpenMultipleGameWindow(object sender, RoutedEventArgs e)
		{
			var w = new MultiSceneWindow();
			w.Show();
		}
	}
}
using System.Windows;
using WpfTest.Scenes;

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

		/// <summary>
		/// Opens the window once.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		private static void OpenSingleWindow<T>() where T : Window, new()
		{
			var w = new T();
			w.Show();
		}

		private void OpenNewWindow(object sender, RoutedEventArgs e)
		{
			OpenSingleWindow<SimpleWindow>();
		}

		private void OpenTextInputWindow(object sender, RoutedEventArgs e)
		{
			OpenSingleWindow<TextInputWindow>();
		}

		private void OpenMultipleGameWindow(object sender, RoutedEventArgs e)
		{
			OpenSingleWindow<MultiSceneWindow>();
		}

		private void OpenTabbedGameWindow(object sender, RoutedEventArgs e)
		{
			// manually reset counters so we always have the same id's per tab
			TabScene.Counter = 0;
			OpenSingleWindow<TabWindow>();
		}
	}
}
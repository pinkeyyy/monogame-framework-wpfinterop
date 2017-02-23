using System;
using System.Windows;

namespace WpfTest.Views
{
	/// <summary>
	/// Interaction logic for MultiSceneWindow.xaml
	/// </summary>
	public partial class MultiSceneWindow
	{
		#region Constructors

		public MultiSceneWindow()
		{
			InitializeComponent();
			CinematicExperience.TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);
		}

		#endregion

		#region Methods

		private void Launch_OnClick(object sender, RoutedEventArgs e)
		{
			var window = new SecondWindow();
			window.Show();
		}

		private void Launch_WpfControls(object sender, RoutedEventArgs e)
		{
			var window = new WpfControlWindow();
			window.Closed += (o, args) =>
			{
				window.Dispose();
			};
			window.Show();
		}

		#endregion
	}
}
using System;
using System.Threading;
using System.Windows.Threading;

namespace WpfTest.Views
{
	/// <summary>
	/// Interaction logic for WpfControlWindow.xaml
	/// </summary>
	public partial class WpfControlWindow : IDisposable
	{
		private readonly Timer _timer;

		#region Constructors

		public WpfControlWindow()
		{
			InitializeComponent();

			// manual timer that runs every 50ms to update UI based on game state
			_timer = new Timer(TimerTick, null, 0, 50);
		}

		private void TimerTick(object state)
		{
			TextFromGame.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
			{
				TextFromGame.Text = Game.EnteredMessage;
			}));
		}

		public void Dispose()
		{
			_timer.Dispose();
		}

		#endregion
	}
}
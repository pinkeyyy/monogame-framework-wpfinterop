using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Internals;
using System;
using WpfTest.Components;
using WpfTest.Views;
using Color = Microsoft.Xna.Framework.Color;

namespace WpfTest.Scenes
{
	/// <summary>
	/// Special scene used in the tab window.
	/// Shows the last time Initialize/Dispose was called
	/// </summary>
	public class TabScene : WpfGame
	{
		private int _numberOfInitializeCalls;
		private int _numberOfDisposeCalls;
		private int _numberOfActivateCalls;
		private int _numberOfDeactivateCalls;
		private DateTime _lastActivateCall, _lastDeactivateCall;
		private TextComponent _text;
		internal static int Counter;
		private int _id;
		private bool _lastIsActiveState;

		private TabWindow _tabWindow;

		protected override void Initialize()
		{
			// init is only called once per game
			_numberOfInitializeCalls++;

			new WpfGraphicsDeviceService(this);

			base.Initialize();
			// not really pretty, but gets the job done
			var parent = LogicalTreeHelperEx.FindParent<TabWindow>(this);
			if (parent == null)
			{
				throw new NotSupportedException("This scene only works on TabWindow right now");
			}
			_tabWindow = parent;
			_text = new TextComponent(this, "dummy", new Vector2(0, 0));
			Components.Add(_text);
			_id = ++Counter;
			_tabWindow.Log($"Tabbed game {_id} initialize");
			Activated += OnActivated;
			Deactivated += OnDeactivated;

		}

		private void OnDeactivated(object sender, EventArgs e)
		{
			_tabWindow.Log($"Tabbed game {_id} deactivate");
			_lastDeactivateCall = DateTime.Now;
			_numberOfDeactivateCalls++;
		}

		private void OnActivated(object sender, EventArgs eventArgs)
		{
			_tabWindow.Log($"Tabbed game {_id} activate");
			_lastActivateCall = DateTime.Now;
			_numberOfActivateCalls++;
		}

		protected override void Dispose(bool disposing)
		{
			// Dispose is called once per game (only when the window closes)
			_numberOfDisposeCalls++;

			// dispose auto. clears components but not services
			base.Dispose(disposing);
			_text = null;
			// this service is added by the "new WpfGraphicsDeviceService(this)" call in Initialize
			// stupid behaviour, I know, but it is 1:1 copy of xna/monogame behaviour
			Services.RemoveService(typeof(IGraphicsDeviceService));
			_tabWindow.Log($"Tabbed game {_id} dispose");
		}

		protected override void Update(GameTime gameTime)
		{
			var updatedText = $"Number of initialize calls: {_numberOfInitializeCalls}" + Environment.NewLine +
							  $"Number of dispose calls: {_numberOfDisposeCalls}" + Environment.NewLine +
							  $"Number of activate calls: {_numberOfActivateCalls}" + Environment.NewLine +
							  $"Last activate call at: {_lastActivateCall}" + Environment.NewLine +
							  $"Number of deactivate calls: {_numberOfDeactivateCalls}" + Environment.NewLine +
							  $"Last deactivate call at: {_lastDeactivateCall}" + Environment.NewLine +
							  $"IsActive: {IsActive}";
			if (_lastIsActiveState != IsActive)
			{
				_lastIsActiveState = IsActive;
				_tabWindow.Log($"Tabbed game {_id} change of IsActive to: {IsActive}");
			}
			_text.Text = updatedText;
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);
			base.Draw(gameTime);
		}
	}
}
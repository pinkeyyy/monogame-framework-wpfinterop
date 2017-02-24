using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using System;
using System.Diagnostics;
using WpfTest.Components;

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
		private bool _debugWriteIsActiveStateOnce;

		protected override void Initialize()
		{
			// init is only called once per game
			_numberOfInitializeCalls++;

			new WpfGraphicsDeviceService(this);

			base.Initialize();
			_text = new TextComponent(this, "dummy", new Vector2(0, 0));
			Components.Add(_text);
			_id = ++Counter;
			Debug.WriteLine($"Tabbed game {_id} initialize");
			Activated += OnActivated;
			Deactivated += OnDeactivated;
			_debugWriteIsActiveStateOnce = true;
		}

		private void OnDeactivated(object sender, EventArgs e)
		{
			Debug.WriteLine($"Tabbed game {_id} deactivate");
			_lastDeactivateCall = DateTime.Now;
			_numberOfDeactivateCalls++;
			_debugWriteIsActiveStateOnce = true;
		}

		private void OnActivated(object sender, EventArgs eventArgs)
		{
			Debug.WriteLine($"Tabbed game {_id} activate");
			_lastActivateCall = DateTime.Now;
			_numberOfActivateCalls++;
			_debugWriteIsActiveStateOnce = true;
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
			Debug.WriteLine($"Tabbed game {_id} dispose");
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
			if (_debugWriteIsActiveStateOnce)
			{
				_debugWriteIsActiveStateOnce = false;
				Debug.WriteLine($"Tabbed game {_id} IsActive: {IsActive}");
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
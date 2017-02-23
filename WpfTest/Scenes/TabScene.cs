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
		private int _numberOfConstructorCalls;
		private DateTime _lastInitCall, _lastDisposeCall;
		private TextComponent _text;
		internal static int Counter;
		private int _id;

		public TabScene()
		{
			// constructor is only called once, but humor me
			_numberOfConstructorCalls++;
		}

		protected override void Initialize()
		{
			// initialize is called each time the tab containing this game is loaded
			_numberOfInitializeCalls++;
			_lastInitCall = DateTime.Now;

			new WpfGraphicsDeviceService(this);

			base.Initialize();
			_text = new TextComponent(this, "dummy", new Vector2(0, 0));
			Components.Add(_text);
			_id = ++Counter;
			Debug.WriteLine($"Tabbed game {_id} initialize");
			Activated += OnActivated;
			Deactivated += OnDeactivated;
		}

		private void OnDeactivated(object sender, EventArgs e)
		{
			Debug.WriteLine($"Tabbed game {_id} deactivate");
		}

		private void OnActivated(object sender, EventArgs eventArgs)
		{
			Debug.WriteLine($"Tabbed game {_id} activate");
		}

		protected override void Dispose(bool disposing)
		{
			// Dispose is called each time the tab containing this game is unloaded
			_numberOfDisposeCalls++;
			_lastDisposeCall = DateTime.Now;

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
			var updatedText = $"Number of constructor calls: {_numberOfConstructorCalls}" + Environment.NewLine +
							  $"Number of initialize calls: {_numberOfInitializeCalls}" + Environment.NewLine +
							  $"Last initialize call at: {_lastInitCall}" + Environment.NewLine +
							  $"Number of dispose calls: {_numberOfDisposeCalls}" + Environment.NewLine +
							  $"Last dispose call at: {_lastDisposeCall}" + Environment.NewLine +
							  $"IsActive: {IsActive}";
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
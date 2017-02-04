using System;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;

namespace WpfTest
{
	public class TextInputScene : WpfGame
	{
		private IGraphicsDeviceService _graphicsDeviceManager;
		private WpfKeyboard _keyboard;
		private KeyboardState _keyboardState;
		private KeyboardState _previousKeyboardState;
		private WpfMouse _mouse;
		private MouseState _mouseState;

		/// <summary>
		/// Contains the message entered by the user while this panel had focus.
		/// User can use enter to clear this message.
		/// </summary>
		public string EnteredMessage { get; private set; }

		protected override void Draw(GameTime time)
		{
			GraphicsDevice.Clear(_mouseState.LeftButton == ButtonState.Pressed ? Color.Black : Color.CornflowerBlue);
			
			base.Draw(time);

		}

		protected override void Initialize()
		{
			base.Initialize();
			_graphicsDeviceManager = new WpfGraphicsDeviceService(this);

			_keyboard = new WpfKeyboard(this);
			_mouse = new WpfMouse(this);

			Components.Add(new DrawMeComponent(this));
		}

		protected override void Update(GameTime time)
		{
			_mouseState = _mouse.GetState();
			_keyboardState = _keyboard.GetState();

			if (_keyboardState.IsKeyDown(Keys.Delete))
			{
				// clear message
				EnteredMessage = "";
			}
			else
			{
				var sb = new StringBuilder();
				var currentKeys = _keyboardState.GetPressedKeys();
				var previousKeys = _previousKeyboardState.GetPressedKeys();
				for (int i = (int)Keys.A; i <= (int)Keys.Z; i++)
				{
					if (previousKeys.Contains((Keys) i) && !currentKeys.Contains((Keys) i))
					{
						// key pressed, add key to queue
						sb.Append(((char) i));

					}
				}
				if (previousKeys.Contains(Keys.Space) && !currentKeys.Contains(Keys.Space))
				{
					sb.Append(' ');
				}
				// concat to message
				EnteredMessage += sb.ToString().ToLower();
			}
			_previousKeyboardState = _keyboardState;
			base.Update(time);
		}

	}
}
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
		private MouseState _previousMouseState;
		private SpriteBatch _spriteBatch;
		private SpriteFont _font;

		/// <summary>
		/// Contains the message entered by the user while this panel had focus.
		/// User can use enter to clear this message.
		/// </summary>
		public string EnteredMessage { get; private set; }

		protected override void Draw(GameTime time)
		{
			GraphicsDevice.Clear(_mouseState.LeftButton == ButtonState.Pressed ? Color.Black : Color.CornflowerBlue);

			// since we share the GraphicsDevice with all hosts, we need to save and reset the states
			// this has to be done because spriteBatch internally sets states and doesn't reset themselves, fucking over any 3D rendering (which happens in the DemoScene)

			var blend = GraphicsDevice.BlendState;
			var depth = GraphicsDevice.DepthStencilState;
			var raster = GraphicsDevice.RasterizerState;
			var sampler = GraphicsDevice.SamplerStates[0];

			_spriteBatch.Begin();
			_spriteBatch.DrawString(_font, $"Has focus: {IsFocused}", new Vector2(5, 50), Color.White);
			_spriteBatch.DrawString(_font, $"Focus on: {(FocusOnMouseOver ? "mouse hover" : "first click")}. (Right click in game to toggle this)", new Vector2(5, 50 + 20), Color.White);
			_spriteBatch.DrawString(_font, $"Mouse position: X: {_mouseState.X}, Y: {_mouseState.Y}", new Vector2(5, 50 + 40), Color.White);
			_spriteBatch.End();

			// this base.Draw call will draw "all" components (we only added one)
			// since said component will use a spritebatch to render we need to let it draw before we reset the GraphicsDevice
			// otherwise it will just alter the state again and fuck over all the other hosts
			base.Draw(time);

			GraphicsDevice.BlendState = blend;
			GraphicsDevice.DepthStencilState = depth;
			GraphicsDevice.RasterizerState = raster;
			GraphicsDevice.SamplerStates[0] = sampler;

		}

		protected override void Initialize()
		{
			base.Initialize();
			_graphicsDeviceManager = new WpfGraphicsDeviceService(this);

			_keyboard = new WpfKeyboard(this);
			_mouse = new WpfMouse(this);

			// default font is pre-compiled font for Windows (Arial 12, ? as default char)
			// I get away with this because
			// 1) it's just a demo application
			// 2) it can only run on windows/directX anyway (interop for WPF afterall)
			// 3) This means it doesn't require content compiler to be installed on any machine that runs this demo

			_font = Content.Load<SpriteFont>("DefaultFont");

			_spriteBatch = new SpriteBatch(GraphicsDevice);

			Components.Add(new DrawMeComponent(this, new Point(400, 500)));
		}

		protected override void Update(GameTime time)
		{
			_mouseState = _mouse.GetState();
			_keyboardState = _keyboard.GetState();

			if (_mouseState.RightButton == ButtonState.Pressed && _previousMouseState.RightButton == ButtonState.Released)
			{
				FocusOnMouseOver = !FocusOnMouseOver;
			}
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
					if (previousKeys.Contains((Keys)i) && !currentKeys.Contains((Keys)i))
					{
						// key pressed, add key to queue
						sb.Append(((char)i));

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
			_previousMouseState = _mouseState;
			base.Update(time);
		}

	}
}
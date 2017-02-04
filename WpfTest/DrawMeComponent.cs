using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;

namespace WpfTest
{
	public class DrawMeComponent : WpfDrawableGameComponent
	{
		#region Fields

		private int posX = 300, posY = 100;
		private Texture2D _texture;
		private SpriteBatch _spriteBatch;
		private float _rotation;
		#endregion

		#region Constructors

		public DrawMeComponent(WpfGame game, Point? postion = null) : base(game)
		{
			if (postion.HasValue)
			{
				posX = postion.Value.X;
				posY = postion.Value.Y;
			}
		}

		#endregion

		#region Methods

		protected override void LoadContent()
		{
			_texture = Game.Content.Load<Texture2D>("hello");

			_spriteBatch = new SpriteBatch(GraphicsDevice);
		}

		public override void Update(GameTime gameTime)
		{
			_rotation -= (float)(2f * gameTime.ElapsedGameTime.TotalSeconds);
		}

		public override void Draw(GameTime gameTime)
		{
			_spriteBatch.Begin();
			_spriteBatch.Draw(_texture, new Rectangle(posX, posY, 100, 20), null, Color.White, _rotation, new Vector2(_texture.Width, _texture.Height) / 2f, SpriteEffects.None, 0);
			_spriteBatch.End();
		}

		#endregion
	}
}
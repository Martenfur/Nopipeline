using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Nopipeline.Sample
{
	public class Game1 : Game
	{
		private GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;

		private Texture2D _sprite1;
		private Texture2D _sprite2;
		private Texture2D _sprite3;
		private Texture2D _sprite4;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			IsMouseVisible = true;
		}

		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			_sprite1 = Content.Load<Texture2D>("Graphics/Sprite1");
			_sprite2 = Content.Load<Texture2D>("Graphics/Sprite2");
			_sprite3 = Content.Load<Texture2D>("Graphics/Sprite3");
			_sprite4 = Content.Load<Texture2D>("Graphics/Sprite4");
			Debug.WriteLine(Content.Load<string>("SampleWatch/Watch"));
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			// TODO: Add your update logic here

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// TODO: Add your drawing code here

			_spriteBatch.Begin();
			_spriteBatch.Draw(_sprite1, new Vector2(100, 100), Color.White);
			_spriteBatch.Draw(_sprite2, new Vector2(100 + 32, 100), Color.White);
			_spriteBatch.Draw(_sprite3, new Vector2(100, 100 + 32), Color.White);
			_spriteBatch.Draw(_sprite4, new Vector2(100 + 32, 100 + 32), Color.White);

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}

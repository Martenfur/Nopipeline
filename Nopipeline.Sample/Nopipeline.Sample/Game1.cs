using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Nopipeline.Sample
{
	public class Game1 : Game
	{
		private Color _backgroundColor = new Color(188, 249, 147);

		private GraphicsDeviceManager _graphics;

		public static SpriteBatch SpriteBatch;

		private Lawn _lawn = new Lawn();

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
			SpriteBatch = new SpriteBatch(GraphicsDevice);
			_lawn.Init(Content);
			Debug.WriteLine(Content.Load<string>("SampleWatch/Watch"));
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
			{
				Exit();
			}

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(_backgroundColor);

			SpriteBatch.Begin(SpriteSortMode.FrontToBack);

			_lawn.Draw();

			SpriteBatch.End();

			base.Draw(gameTime);
		}
	}
}

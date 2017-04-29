using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ShapeBlaster
{
	public class GameRoot : Microsoft.Xna.Framework.Game
	{
		// some helpful static properties
		public static GameRoot Instance { get; private set; }
		public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }
		public static Vector2 ScreenSize { get { return new Vector2(Viewport.Width, Viewport.Height); } }
		public static GameTime GameTime { get; private set; }

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		public GameRoot()
		{
			Instance = this;
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			graphics.PreferredBackBufferWidth = 800;
			graphics.PreferredBackBufferHeight = 600;

            graphics.IsFullScreen = true;
		}

		protected override void Initialize()
		{
			base.Initialize();

			EntityManager.Add(PlayerShip.Instance);

			MediaPlayer.IsRepeating = true;
			MediaPlayer.Play(Sound.Music);
		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			Art.Load(Content);
			Sound.Load(Content);
		}

		protected override void Update(GameTime gameTime)
		{
			GameTime = gameTime;
			Input.Update();

			// Allows the game to exit
			if (Input.WasButtonPressed(Buttons.Back) || Input.WasKeyPressed(Keys.Escape))
				this.Exit();

			EntityManager.Update();
			EnemySpawner.Update();
			PlayerStatus.Update();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			// Draw entities. Sort by texture for better batching.
			spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
			EntityManager.Draw(spriteBatch);
			spriteBatch.End();

			// Draw user interface
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

			spriteBatch.DrawString(Art.Font, "Lives: " + PlayerStatus.Lives, new Vector2(5), Color.White);
			DrawRightAlignedString("Score: " + PlayerStatus.Score, 5);
			DrawRightAlignedString("Multiplier: " + PlayerStatus.Multiplier, 35);
			
			// draw the custom mouse cursor
			spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);

			if (PlayerStatus.IsGameOver)
			{
				string text = "Game Over\n" +
					"Your Score: " + PlayerStatus.Score + "\n" +
					"High Score: " + PlayerStatus.HighScore;

				Vector2 textSize = Art.Font.MeasureString(text);
				spriteBatch.DrawString(Art.Font, text, ScreenSize / 2 - textSize / 2, Color.White);
			}

			spriteBatch.End();


			base.Draw(gameTime);
		}

		private void DrawRightAlignedString(string text, float y)
		{
			var textWidth = Art.Font.MeasureString(text).X;
			spriteBatch.DrawString(Art.Font, text, new Vector2(ScreenSize.X - textWidth - 5, y), Color.White);
		}
	}
}

//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

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
using SparTrek;

namespace ShapeBlaster
{
	public class GameRoot : Microsoft.Xna.Framework.Game
	{
		// some helpful static properties
		public static GameRoot Instance { get; private set; }
		public static Viewport Viewport { get { return Instance.GraphicsDevice.Viewport; } }
		public static Vector2 ScreenSize { get { return new Vector2(Viewport.Width, Viewport.Height); } }
		public static GameTime GameTime { get; private set; }

        private Vector2 startButtonPosition;
        private Vector2 exitButtonPosition;
        private Vector2 playerOneButtonPosition;
        private Vector2 playerTwoButtonPosition;
        private Vector2 choosePlayerTextPosition;
        private Vector2 jokeTextPosition;
        private MouseState mouseState;
        private MouseState previousMouseState;
        private Texture2D star;
        private StarField starField; 

        enum GameState { MENU, PLAYING, PLAYERCHOICE, JOKE};

        private GameState gamestate = GameState.MENU;

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
			MediaPlayer.Play(Sound.MenuMusic);

            startButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 50, 200);
            exitButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 50, 250);

            playerOneButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 50, 200);
            playerTwoButtonPosition = new Vector2((GraphicsDevice.Viewport.Width / 2) - 50, 400);

            choosePlayerTextPosition = new Vector2((GraphicsDevice.Viewport.Width / 3), 50);
            jokeTextPosition = new Vector2((GraphicsDevice.Viewport.Width / 7), 50);

            starField = new StarField(
                this.Window.ClientBounds.Width,
                this.Window.ClientBounds.Height,
                300,
                new Vector2(0, 100f),
                star,
                new Rectangle(0, 0, 2, 2)
            );

		}

		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
			Sound.Load(Content);
            Art.Load(Content);
            Art.LoadMenu(Content);
            star = Art.Star;
		}

		protected override void Update(GameTime gameTime)
		{
			GameTime = gameTime;
			Input.Update();

            if (gamestate == GameState.MENU)
            {
                // Allows the game to exit
                if (Input.WasButtonPressed(Buttons.Back) || Input.WasKeyPressed(Keys.Escape))
                {
                    MediaPlayer.Stop();
                    this.Exit();
                }

                //wait for mouseclick
                mouseState = Mouse.GetState();
                if (previousMouseState.LeftButton == ButtonState.Pressed &&
                    mouseState.LeftButton == ButtonState.Released)
                {
                    MouseClicked(mouseState.X, mouseState.Y);
                }
                previousMouseState = mouseState;

            }
            else if (gamestate == GameState.PLAYING)
            {

                // Allows the game to exit
                if (Input.WasButtonPressed(Buttons.Back) || Input.WasKeyPressed(Keys.Escape))
                {
                    MediaPlayer.Stop();
                    this.Exit();
                }
           

                EntityManager.Update();
                EnemySpawner.Update();
                PlayerStatus.Update();
            }
            else if (gamestate == GameState.PLAYERCHOICE)
            {
                // Allows the game to exit
                if (Input.WasButtonPressed(Buttons.Back) || Input.WasKeyPressed(Keys.Escape))
                {
                    gamestate = GameState.MENU;
                }

                //wait for mouseclick
                mouseState = Mouse.GetState();
                if (previousMouseState.LeftButton == ButtonState.Pressed &&
                    mouseState.LeftButton == ButtonState.Released)
                {
                    MouseClicked(mouseState.X, mouseState.Y);
                }
                previousMouseState = mouseState;
            }

            else if (gamestate == GameState.JOKE)
            {
                // Allows the game to exit
                if (Input.WasButtonPressed(Buttons.Back) || Input.WasKeyPressed(Keys.Escape))
                {
                    gamestate = GameState.MENU;
                }

                //wait for mouseclick
                mouseState = Mouse.GetState();
                if (previousMouseState.LeftButton == ButtonState.Pressed &&
                    mouseState.LeftButton == ButtonState.Released)
                {
                    MouseClicked(mouseState.X, mouseState.Y);
                }
                previousMouseState = mouseState;
            }

			base.Update(gameTime);
		}


		protected override void Draw(GameTime gameTime)
		{

            if (gamestate == GameState.PLAYING)
            {
                GraphicsDevice.Clear(Color.Black);

                // Draw entities. Sort by texture for better batching.
                spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive);
                EntityManager.Draw(spriteBatch);
                starField.Draw(spriteBatch); 
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
                starField.Update(gameTime);
                spriteBatch.End();

            }

            else if (gamestate == GameState.MENU)
            {
                GraphicsDevice.Clear(Color.Black);

                // Draw user interface
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
                starField.Draw(spriteBatch); 
                spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
                spriteBatch.Draw(Art.StartButton, startButtonPosition, Color.White);
                spriteBatch.Draw(Art.ExitButton, exitButtonPosition, Color.White);
                spriteBatch.End();
            }

            else if (gamestate == GameState.PLAYERCHOICE)
            {
                GraphicsDevice.Clear(Color.Black);

                // Draw user interface
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
                starField.Draw(spriteBatch); 
                spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
                spriteBatch.Draw(Art.Player, playerOneButtonPosition, Color.White);
                spriteBatch.Draw(Art.Seeker, playerTwoButtonPosition, Color.White);

                string text = "Choose your ship...";

                Vector2 textSize = Art.Font.MeasureString(text);
                spriteBatch.DrawString(Art.Font, text, choosePlayerTextPosition, Color.White);

                spriteBatch.End();
            }

            else if (gamestate == GameState.JOKE)
            {
                GraphicsDevice.Clear(Color.Black);

                // Draw user interface
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
                starField.Draw(spriteBatch); 
                spriteBatch.Draw(Art.Pointer, Input.MousePosition, Color.White);
                spriteBatch.Draw(Art.Player, playerOneButtonPosition, Color.White);
                spriteBatch.Draw(Art.Seeker, playerTwoButtonPosition, Color.White);

                string text = "This program only supports MSU currently! \nGo Green!";

                Vector2 textSize = Art.Font.MeasureString(text);
                spriteBatch.DrawString(Art.Font, text, jokeTextPosition, Color.Green);

                spriteBatch.End();
            }
            starField.Update(gameTime);
			base.Draw(gameTime);
		}

        void MouseClicked(int x, int y)
        {
            //creates a rectangle of 10x10 around the place where the mouse was clicked
            Rectangle mouseClickRect = new Rectangle(x, y, 100, 100);
            if (gamestate == GameState.MENU)
            {
                mouseClickRect = new Rectangle(x, y, 10, 10);

                Rectangle startButtonRect = new Rectangle((int)startButtonPosition.X,
                                           (int)startButtonPosition.Y, 100, 20);
                Rectangle exitButtonRect = new Rectangle((int)exitButtonPosition.X,
                                        (int)exitButtonPosition.Y, 100, 20);

                if (mouseClickRect.Intersects(startButtonRect)) //player clicked start button
                {
                    gamestate = GameState.PLAYERCHOICE;

                }
                else if (mouseClickRect.Intersects(exitButtonRect)) //player clicked exit button
                {
                    MediaPlayer.Stop();
                   this.Exit();
                }
            }

            else if (gamestate == GameState.PLAYERCHOICE || gamestate == GameState.JOKE)
            {
                Rectangle playerOneRect = new Rectangle((int)playerOneButtonPosition.X,
                                           (int)playerOneButtonPosition.Y, 200, 200);
                Rectangle playerTwoRect = new Rectangle((int)playerTwoButtonPosition.X,
                                        (int)playerTwoButtonPosition.Y, 200, 200);

                if (mouseClickRect.Intersects(playerOneRect)) //player clicked start button
                {
                    gamestate = GameState.PLAYING;
                    MediaPlayer.Stop();
                    MediaPlayer.Play(Sound.Music);

                }
                else if (mouseClickRect.Intersects(playerTwoRect)) //player clicked exit button
                {
                    gamestate = GameState.JOKE;
                }
            }
        }

		private void DrawRightAlignedString(string text, float y)
		{
			var textWidth = Art.Font.MeasureString(text).X;
			spriteBatch.DrawString(Art.Font, text, new Vector2(ScreenSize.X - textWidth - 5, y), Color.White);
		}
	}
}

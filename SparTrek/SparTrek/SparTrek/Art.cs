//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace ShapeBlaster
{
	static class Art
	{
		public static Texture2D Player { get; private set; }
        public static Texture2D Star { get; private set; }
		public static Texture2D Seeker { get; private set; }
		public static Texture2D Wanderer { get; private set; }
		public static Texture2D Bullet { get; private set; }
        public static Texture2D Pointer { get; private set; }
        public static Texture2D StartButton { get; private set; }
        public static Texture2D ExitButton { get; private set; }

		public static SpriteFont Font { get; private set; }

		public static void Load(ContentManager content)
		{
			Player = content.Load<Texture2D>("Art/Player");
            Star = content.Load<Texture2D>("Art/STAR");
			Seeker = content.Load<Texture2D>("Art/Seeker");
			Wanderer = content.Load<Texture2D>("Art/Wanderer");
			Bullet = content.Load<Texture2D>("Art/Bullet");
			Pointer = content.Load<Texture2D>("Art/Pointer");

			Font = content.Load<SpriteFont>("Font");
		}

        public static void LoadMenu(ContentManager content)
        {
            Pointer = content.Load<Texture2D>("Art/Pointer");
            StartButton = content.Load<Texture2D>("Art/Start");
            ExitButton = content.Load<Texture2D>("Art/Exit");
        }

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics; 

namespace SparTrek
{

    class Star
    {
        private Texture2D Texture { get; set; }
        public Color TintColor { get; set; }
        public Vector2 Location { get; set; }
        private Vector2 Velocity { get; set; }
        private Rectangle InitialFrame { get; set; }

        public Star(
            Vector2 location,
            Texture2D texture,
            Rectangle initialFrame,
            Vector2 velocity)
        {
            Location = location;
            Texture = texture;
            InitialFrame = initialFrame;
            Velocity = velocity;
        }

        public Rectangle Destination
        {
            get
            {
                return new Rectangle(
                    (int)Location.X, (int)Location.Y,
                    InitialFrame.Width, InitialFrame.Height
                );
            }
        }
        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Location += (Velocity * elapsed);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                Texture, Destination, InitialFrame, TintColor
            );
        } 

    }
}

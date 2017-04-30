//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ShapeBlaster
{
	class PlayerShip : Entity
	{
		private static PlayerShip instance;
		public static PlayerShip Instance 
		{
			get
			{
				if (instance == null)
					instance = new PlayerShip();

				return instance;
			}
		}

		const int cooldownFrames = 6;
		int cooldownRemaining = 0;

		int framesUntilRespawn = 0;
		public bool IsDead { get { return framesUntilRespawn > 0; } }

		static Random rand = new Random();

		private PlayerShip()
		{
			image = Art.Player;
			Position = GameRoot.ScreenSize / 2;
			Radius = 10;
		}

		public override void Update()
		{
			if (IsDead)
			{
				if (--framesUntilRespawn == 0)
				{
					if (PlayerStatus.Lives == 0)
					{
						PlayerStatus.Reset();
						Position = GameRoot.ScreenSize / 2;
					}
				}				
				return;
			}
			
			var aim = Input.GetAimDirection();
			if (aim.LengthSquared() > 0 && cooldownRemaining <= 0)
			{
				cooldownRemaining = cooldownFrames;
				float aimAngle = aim.ToAngle();
				Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);

				float randomSpread = rand.NextFloat(-0.04f, 0.04f) + rand.NextFloat(-0.04f, 0.04f);
				Vector2 vel = MathUtil.FromPolar(aimAngle + randomSpread, 11f);

				Vector2 offset = Vector2.Transform(new Vector2(35, -8), aimQuat);
				EntityManager.Add(new Bullet(Position + offset, vel));

				offset = Vector2.Transform(new Vector2(35, 8), aimQuat);
				EntityManager.Add(new Bullet(Position + offset, vel));

				Sound.Shot.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0);
			}

			if (cooldownRemaining > 0)
				cooldownRemaining--;

			const float speed = 8;
			Velocity = speed * Input.GetMovementDirection();
			Position += Velocity;
			Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);
			
			if (Velocity.LengthSquared() > 0)
				Orientation = Velocity.ToAngle();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			if (!IsDead)
				base.Draw(spriteBatch);
		}

		public void Kill()
		{
			PlayerStatus.RemoveLife();
			framesUntilRespawn = PlayerStatus.IsGameOver ? 300 : 120;
		}
	}
}

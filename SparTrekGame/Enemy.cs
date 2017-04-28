//---------------------------------------------------------------------------------
// Written by Michael Hoffman
// Find the full tutorial at: http://gamedev.tutsplus.com/series/vector-shooter-xna/
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShapeBlaster
{
	class Enemy : Entity
	{
		public static Random rand = new Random();

		private List<IEnumerator<int>> behaviours = new List<IEnumerator<int>>();
		private int timeUntilStart = 60;
		public bool IsActive { get { return timeUntilStart <= 0; } }
		public int PointValue { get; private set; }

		public Enemy(Texture2D image, Vector2 position)
		{
			this.image = image;
			Position = position;
			Radius = image.Width / 2f;
			color = Color.Transparent;
			PointValue = 1;
		}

		public static Enemy CreateSeeker(Vector2 position)
		{
			var enemy = new Enemy(Art.Seeker, position);
			enemy.AddBehaviour(enemy.FollowPlayer());
			enemy.PointValue = 2;

			return enemy;
		}

		public static Enemy CreateWanderer(Vector2 position)
		{
			var enemy = new Enemy(Art.Wanderer, position);
			enemy.AddBehaviour(enemy.MoveRandomly());

			return enemy;
		}

		public override void Update()
		{
			if (timeUntilStart <= 0)
				ApplyBehaviours();
			else
			{
				timeUntilStart--;
				color = Color.White * (1 - timeUntilStart / 60f);
			}

			Position += Velocity;
			Position = Vector2.Clamp(Position, Size / 2, GameRoot.ScreenSize - Size / 2);

			Velocity *= 0.8f;
		}

		private void AddBehaviour(IEnumerable<int> behaviour)
		{
			behaviours.Add(behaviour.GetEnumerator());
		}

		private void ApplyBehaviours()
		{
			for (int i = 0; i < behaviours.Count; i++)
			{
				if (!behaviours[i].MoveNext())
					behaviours.RemoveAt(i--);
			}
		}

		public void HandleCollision(Enemy other)
		{
			var d = Position - other.Position;
			Velocity += 10 * d / (d.LengthSquared() + 1);
		}

		public void WasShot()
		{
			IsExpired = true;
			Sound.Explosion.Play(0.5f, rand.NextFloat(-0.2f, 0.2f), 0);

			PlayerStatus.AddPoints(PointValue);
			PlayerStatus.IncreaseMultiplier();
		}

		#region Behaviours
		IEnumerable<int> FollowPlayer(float acceleration = 1f)
		{
			while (true)
			{
				if (!PlayerShip.Instance.IsDead)
					Velocity += (PlayerShip.Instance.Position - Position).ScaleTo(acceleration);

				if (Velocity != Vector2.Zero)
					Orientation = Velocity.ToAngle();

				yield return 0;
			}
		}

		IEnumerable<int> MoveRandomly()
		{
			float direction = rand.NextFloat(0, MathHelper.TwoPi);

			while (true)
			{
				direction += rand.NextFloat(-0.1f, 0.1f);
				direction = MathHelper.WrapAngle(direction);

				for (int i = 0; i < 6; i++)
				{
					Velocity += MathUtil.FromPolar(direction, 0.4f);
					Orientation -= 0.05f;

					var bounds = GameRoot.Viewport.Bounds;
					bounds.Inflate(-image.Width / 2 - 1, -image.Height / 2 - 1);

					// if the enemy is outside the bounds, make it move away from the edge
					if (!bounds.Contains(Position.ToPoint()))
						direction = (GameRoot.ScreenSize / 2 - Position).ToAngle() + rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);

					yield return 0;
				}
			}
		}
		#endregion
	}
}

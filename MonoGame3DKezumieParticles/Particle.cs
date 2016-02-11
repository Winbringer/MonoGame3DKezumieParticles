using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame3DKezumieParticles
{
    class Particle
    {
        #region Поля
        public int[] index = new int[] { 0,1,2,
        0,2,3};
        float Speed;
        Vector3 Step;
        Vector3 Velocity;
        public Vector3 Position;
        public bool isMoving { get; set; }
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public VertexPositionTexture[] Vertex;
        #endregion

        public Particle(float speed, Vector3 position)
        {
            Vertex = new VertexPositionTexture[4];
            Speed = speed * 1000;
            StartPosition = position;
            EndPosition = position;
        }

        public void Init()
        {
            Position = StartPosition;
            isMoving = true;
            Vertex = new VertexPositionTexture[4];
            Step = new Vector3(
                (EndPosition.X - StartPosition.X) / Speed,
                (EndPosition.Y - StartPosition.Y) / Speed,
            (EndPosition.Z - StartPosition.Z) / Speed
            );

            float X = StartPosition.X;
            float Y = StartPosition.Y;
            float Z = StartPosition.Z;

            Vertex[0] = new VertexPositionTexture(new Vector3(X, Y, Z), new Vector2(0, 0));
            Vertex[1] = new VertexPositionTexture(new Vector3(X, Y, Z), new Vector2(1, 0));
            Vertex[2] = new VertexPositionTexture(new Vector3(X, Y, Z), new Vector2(1, 1));
            Vertex[3] = new VertexPositionTexture(new Vector3(X, Y, Z), new Vector2(0, 1));
        }

        public void Move(GameTime time)
        {
            if (this.Position.Equals(this.EndPosition))
            {
                isMoving = false;
                return;
            }

            double mult = time.ElapsedGameTime.TotalMilliseconds;
            Velocity = new Vector3((EndPosition.X - Position.X) / Speed,
           (EndPosition.Y - Position.Y) / Speed,
           (EndPosition.Z - Position.Z) / Speed);

            Position.X += (float)((Velocity.X * mult));
            Position.Y += (float)((Velocity.Y * mult));
            Position.Z += (float)((Velocity.Z * mult));

            if (Math.Abs((EndPosition.X - Position.X)) < Math.Abs(0.1)) Position.X = EndPosition.X;
            if (Math.Abs((EndPosition.Y - Position.Y)) < Math.Abs(0.1)) Position.Y = EndPosition.Y;
            if (Math.Abs((EndPosition.Z - Position.Z)) < Math.Abs(0.1)) Position.Z = EndPosition.Z;

            float X = Position.X;
            float Y = Position.Y;
            float Z = Position.Z;

            Vertex[0].Position = new Vector3(X, Y, Z);
            Vertex[1].Position = new Vector3(X, Y, Z);
            Vertex[2].Position = new Vector3(X, Y, Z);
            Vertex[3].Position = new Vector3(X, Y, Z);
        }

    }
}

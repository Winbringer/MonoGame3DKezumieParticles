using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoGame3DKezumieParticles
{
    class Particle
    {

        float Speed;
        Vector3 Step;
        Vector3 Velocity;
        public Vector3 Position;
        public bool isMoving { get; set; }
        public Color ColorM;
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public float Size { get; set; }
        public int[] indexes;
        public VertexPositionColor[] Vertex;

        public Particle(float speed, Vector3 position)
        {
            Vertex = new VertexPositionColor[4];
            Speed = speed * 1000;
            StartPosition = position;
            EndPosition = position;
            Size = 1f;
            ColorM = Color.Orange;
            indexes = new int[] {0,1,2,
            0,1,3,
            0,2,3,
            1,2,3
            };

        }

        public void Init()
        {
            Position = StartPosition;
            isMoving = true;
            Vertex = new VertexPositionColor[12];
            Step = new Vector3(
                (EndPosition.X - StartPosition.X) / Speed,
                (EndPosition.Y - StartPosition.Y) / Speed,
            (EndPosition.Z - StartPosition.Z) / Speed
            );

            float X = StartPosition.X;
            float Y = StartPosition.Y;
            float Z = StartPosition.Z;

            Vertex[0] = new VertexPositionColor(new Vector3(X, Y, Z), ColorM);
            Vertex[1] = new VertexPositionColor(new Vector3(X + Size, Y + Size / 6, Z + Size / 6), ColorM);
            Vertex[2] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size, Z + Size / 6), ColorM);
            Vertex[3] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size / 6, Z + Size), ColorM);
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
            if (Math.Abs(EndPosition.X) > Math.Abs(Position.X)) Position.X += (float)(Step.X * mult);
            else
            if (Math.Abs(EndPosition.X) < Math.Abs(Position.X)) Position.X -= (float)(Step.X * mult);

            if (Math.Abs(EndPosition.Y) > Math.Abs(Position.Y)) Position.Y += (float)(Step.Y * mult);
            else
            if (Math.Abs(EndPosition.Y) < Math.Abs(Position.Y)) Position.Y -= (float)(Step.Y * mult);

            if (Math.Abs(EndPosition.Z) > Math.Abs(Position.Z)) Position.Z += (float)(Step.Z * mult);
            else
            if (Math.Abs(EndPosition.Z) < Math.Abs(Position.Z)) Position.Z -= (float)(Step.Z * mult);

            if (Math.Abs((EndPosition.X - Position.X)) < Math.Abs(0.1)) Position.X = EndPosition.X;
            if (Math.Abs((EndPosition.Y - Position.Y)) < Math.Abs(0.1)) Position.Y = EndPosition.Y;
            if (Math.Abs((EndPosition.Z - Position.Z)) < Math.Abs(0.1)) Position.Z = EndPosition.Z;

            float X = Position.X;
            float Y = Position.Y;
            float Z = Position.Z;

            Vertex[0].Position = Position;
            Vertex[1].Position = new Vector3(X + Size, Y + Size / 6, Z + Size / 6);
            Vertex[2].Position = new Vector3(X + Size / 6, Y + Size, Z + Size / 6);
            Vertex[3].Position = new Vector3(X + Size / 6, Y + Size / 6, Z + Size);
        }

    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoGame3DKezumieParticles
{
    class Particle
    {
        Vector3 Velocity;
        public Vector3 Position;
        float deformation;
        float step;
        float Speed;
        Vector3 Step;
        public bool isMoving { get; set; }
        public VertexPositionColor[] vertex;
        public Color Color;
        public Vector3 StartPosition;
        public Vector3 EndPosition;
        public float Size { get; set; }
        int MinIndex = 1;
        public short[] indexes;
        public Particle(float speed, Vector3 position)
        {
            vertex = new VertexPositionColor[12];
            Speed = speed * 1000;
            StartPosition = position;
            EndPosition = position;
            Size = 1f;
            this.Color = Color.Orange;
            indexes = new short[] {0,1,2,
            0,1,3,
            0,2,3,
            1,2,3
            };

        }
        public void Init()
        {
            Position = StartPosition;
            deformation = 1;
            step = 0.01f;
            isMoving = true;
            vertex = new VertexPositionColor[12];
            Step = new Vector3(
                (EndPosition.X - StartPosition.X) / Speed,
                (EndPosition.Y - StartPosition.Y) / Speed,
            (EndPosition.Z - StartPosition.Z) / Speed
            );

            float X = StartPosition.X;
            float Y = StartPosition.Y;
            float Z = StartPosition.Z;

            vertex[0] = new VertexPositionColor(new Vector3(X, Y, Z), Color);
            vertex[1] = new VertexPositionColor(new Vector3(X + Size, Y + Size / 6, Z + Size / 6), Color);
            vertex[2] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size, Z + Size / 6), Color);
            vertex[3] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size / 6, Z + Size), Color);

            //vertex[3] = new VertexPositionColor(new Vector3(X + 0, Y + 0, Z + 0), Color);
            //vertex[4] = new VertexPositionColor(new Vector3(X + Size, Y + Size / 6, Z + Size / 6), Color);
            //vertex[5] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size / 6, Z + Size), Color);

            //vertex[6] = new VertexPositionColor(new Vector3(X + 0, Y + 0, Z + 0), Color);
            //vertex[7] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size / 6, Z + Size), Color);
            //vertex[8] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size, Z + Size / 6), Color);

            //vertex[9] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size / 6, Z + Size), Color);
            //vertex[10] = new VertexPositionColor(new Vector3(X + Size, Y + Size / 6, Z + Size / 6), Color);
            //vertex[11] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size, Z + Size / 6), Color);
        }

        public void Move(GameTime time)
        {
            if (this.Position.Equals(this.EndPosition))
            {
                isMoving = false;
                return;
            }


            Velocity = new Vector3((EndPosition.X - Position.X) / Speed,
           (EndPosition.Y - Position.Y) / Speed,
           (EndPosition.Z - Position.Z) / Speed);
            double mult = time.ElapsedGameTime.TotalMilliseconds;
            Position.X += (float)((Velocity.X * mult)); /* +(Step.X * mult));*/
            Position.Y += (float)((Velocity.Y * mult))*2; /** 3 + (Step.Y * mult));*/
            Position.Z += (float)((Velocity.Z * mult)); /*+ (Step.Z * mult));*/
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

            vertex[0].Position = Position;
            vertex[1].Position = new Vector3(X + Size, Y + Size / 6, Z + Size / 6);
            vertex[2].Position = new Vector3(X + Size / 6, Y + Size, Z + Size / 6);
            vertex[3].Position = new Vector3(X + Size / 6, Y + Size / 6, Z + Size);
            //vertex[4].Position = new Vector3(X + Size, Y + Size / 6, Z + Size / 6);
            //vertex[5].Position = new Vector3(X + Size / 6, Y + Size / 6, Z + Size);

            //vertex[6].Position = new Vector3(X + 0, Y + 0, Z + 0);
            //vertex[7].Position = new Vector3(X + Size / 6, Y + Size / 6, Z + Size);
            //vertex[8].Position = new Vector3(X + Size / 6, Y + Size, Z + Size / 6);

            //vertex[9].Position = new Vector3(X + Size / 6, Y + Size / 6, Z + Size);
            //vertex[10].Position = new Vector3(X + Size, Y + Size / 6, Z + Size / 6);
            //vertex[11].Position = new Vector3(X + Size / 6, Y + Size, Z + Size / 6);
        }

    }
}

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
        Vector3 Position;
        float Speed;
        public bool isMoving { get; set; }
        public VertexPositionColor[] vertex;
        public Color Color { get; set; }
        public Vector3 StartPosition { get; set; }        
        public Vector3 EndPosition { get; set; }
        public float Size { get; set; }

        public Particle(float speed)
        {
            vertex = new VertexPositionColor[12];
            this.Color = Color.Orange;
            Size = 0.02f;
            Speed = speed;
        }
        public void Init()
        {
            isMoving = true;
            Position = StartPosition;
            float X = StartPosition.X;
            float Y = StartPosition.Y;
            float Z = StartPosition.Z;

            vertex[0] = new VertexPositionColor(new Vector3(X, Y, Z), Color);
            vertex[1] = new VertexPositionColor(new Vector3(X + Size, Y + Size / 6, Z + Size / 6), Color);
            vertex[2] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size, Z + Size / 6), Color);

            vertex[3] = new VertexPositionColor(new Vector3(X + 0, Y + 0, Z + 0), Color);
            vertex[4] = new VertexPositionColor(new Vector3(X + Size, Y + Size / 6, Z + Size / 6), Color);
            vertex[5] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size / 6, Z + Size), Color);

            vertex[6] = new VertexPositionColor(new Vector3(X + 0, Y + 0, Z + 0), Color);
            vertex[7] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size / 6, Z + Size), Color);
            vertex[8] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size, Z + Size / 6), Color);

            vertex[9] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size / 6, Z + Size), Color);
            vertex[10] = new VertexPositionColor(new Vector3(X + Size, Y + Size / 6, Z + Size / 6), Color);
            vertex[11] = new VertexPositionColor(new Vector3(X + Size / 6, Y + Size, Z + Size / 6), Color);
        }

        public void Move(GameTime time)
        {
            if (this.Position.Equals(this.EndPosition))
            {
                isMoving= false;
                return;
            }
            Velocity = new Vector3((EndPosition.X -Position.X)/Speed ,
               (EndPosition.Y - Position.Y) / Speed,
               (EndPosition.Z - Position.Z) / Speed );

        }

    }
}

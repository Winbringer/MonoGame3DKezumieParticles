using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading.Tasks;

namespace MonoGame3DKezumieParticles
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        #region Поля
        double time;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BasicEffect effect;
        Particle[] particles;
        VertexPositionColor[] vertex;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;
        Matrix rotashion = Matrix.Identity;
        MouseState mouse;
        MouseState lastMouseState;        
        float cameraDistance;
        SpriteFont font;
        int c;
        double t;
        string s;
        DynamicVertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        private int[] indices;
        #endregion

        public Game1()
        {
            particles = new Particle[1000000];
            Content.RootDirectory = "Content";
            cameraDistance = 200;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 700;
            graphics.PreferredBackBufferWidth = 800;
            Window.Title = "Kezumie";
            IsMouseVisible = true; 
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 200), Vector3.Zero, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                 graphics.PreferredBackBufferWidth /
                (float)graphics.PreferredBackBufferHeight, 1f, 500);
            worldMatrix = Matrix.CreateWorld(new Vector3(0f, 0f, 0f), new Vector3(0, 0, -1), Vector3.Up);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                Random rnd = new Random(i);
                double R = rnd.NextDouble() * 50;
                float sin = (float)(rnd.NextDouble() * 180);
                float cos = (float)(rnd.NextDouble() * 360);
                float x = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Cos(MathHelper.ToRadians(cos)));
                float y = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Sin(MathHelper.ToRadians(cos)));
                float z = (float)(R * Math.Cos(MathHelper.ToRadians(sin)));
                particles[i] = new Particle(15, new Vector3(0f, 0f, 0f))
                {
                    EndPosition = new Vector3(x, y, z),
                    Size = 1f,
                    ColorM = new Color(255, 128, 0, 255)
                };
                particles[i].Init();
            }

            graphics.GraphicsDevice.Flush();
            vertex = new VertexPositionColor[particles.Length * 4];
            vertexBuffer = new DynamicVertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionColor), vertex.Length, BufferUsage.WriteOnly);

            indices = new int[vertex.Length * 3];
            indexBuffer = new IndexBuffer(GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);

            for (int i = 0; i < particles.Length; i++)
            {

                vertex[i * 4] = particles[i].Vertex[0];
                vertex[i * 4 + 1] = particles[i].Vertex[1];
                vertex[i * 4 + 2] = particles[i].Vertex[2];
                vertex[i * 4 + 3] = particles[i].Vertex[3];
                indices[i * 12] = 0 + i * 4;
                indices[i * 12 + 1] = 1 + i * 4;
                indices[i * 12 + 2] = 2 + i * 4;
                indices[i * 12 + 3] = 0 + i * 4;
                indices[i * 12 + 4] = 2 + i * 4;
                indices[i * 12 + 5] = 3 + i * 4;
                indices[i * 12 + 6] = 0 + i * 4;
                indices[i * 12 + 7] = 1 + i * 4;
                indices[i * 12 + 8] = 3 + i * 4;
                indices[i * 12 + 9] = 1 + i * 4;
                indices[i * 12 + 10] = 2 + i * 4;
                indices[i * 12 + 11] = 3 + i * 4;
            }

            indexBuffer.SetData(indices);
            vertexBuffer.SetData(vertex);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);
            effect = new BasicEffect(graphics.GraphicsDevice);
            effect.VertexColorEnabled = true;
            font = Content.Load<SpriteFont>("font");


        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();           
            time += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (time < 32) return;           
                time = 0;
                MoveAsync(particles, gameTime);
                CameraMove();
                for (int i = 0; i < particles.Length; i++)
                {
                    vertex[i * 4] = particles[i].Vertex[0];
                    vertex[i * 4 + 1] = particles[i].Vertex[1];
                    vertex[i * 4 + 2] = particles[i].Vertex[2];
                    vertex[i * 4 + 3] = particles[i].Vertex[3];
                }
            
            base.Update(gameTime);
        }

       

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            FPS(gameTime);
            vertexBuffer.SetData(vertex);
            graphics.GraphicsDevice.Clear(Color.Black);
            graphics.GraphicsDevice.BlendState = BlendState.Additive;
            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            effect.World = worldMatrix;
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;
            graphics.GraphicsDevice.SetVertexBuffer(null);
            graphics.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            graphics.GraphicsDevice.Indices = indexBuffer;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0, 0, vertex.Length,
                    0, vertex.Length / 4
                    );
            }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "For Nami by Victorem" + s, new Vector2(5, 5), Color.Aqua);
            spriteBatch.End();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            effect.World = worldMatrix;
            base.Draw(gameTime);
        }

        private void CameraMove()
        {
            mouse = Mouse.GetState();

            if (Keyboard.GetState().IsKeyDown(Keys.Left)) rotashion *= Matrix.CreateRotationY(-1 * MathHelper.ToRadians(1));
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) rotashion *= Matrix.CreateRotationY(MathHelper.ToRadians(1));
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) rotashion *= Matrix.CreateRotationX(-1 * MathHelper.ToRadians(1));
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) rotashion *= Matrix.CreateRotationX(MathHelper.ToRadians(1));

            if (mouse.ScrollWheelValue < lastMouseState.ScrollWheelValue) cameraDistance -= 10;
            if (mouse.ScrollWheelValue > lastMouseState.ScrollWheelValue) cameraDistance += 10;

            if (Keyboard.GetState().IsKeyDown(Keys.S)) cameraDistance -= 10;
            if (Keyboard.GetState().IsKeyDown(Keys.W)) cameraDistance += 10;
            if (cameraDistance < 1) cameraDistance = 1;
            if (cameraDistance > 500) cameraDistance = 500;

            viewMatrix = rotashion * Matrix.CreateLookAt(new Vector3(0, 0, cameraDistance),
                                              new Vector3(0, 0, 0), Vector3.Up);
            lastMouseState = mouse;
        }

        Task MoveAsync(Particle[] p, GameTime gt)
        {

            return Task.Factory.StartNew(() =>
              {
                  for (int i = 0; i < p.Length; i++)
                  {
                      if (p[i].isMoving) p[i].Move(gt);
                  }

              });
        }

        private void FPS(GameTime gameTime)
        {
            ++c;
            t += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (c > 100)
            {
                s += " " + c + " " + t / 1000 + Environment.NewLine;
                c = 0;
                t = 0;
            }
        }

        //Диапазон координат от -40 до 40
        //float x = (float)(rnd.NextDouble() - rnd.NextDouble()) * 40;
        //float y = (float)(rnd.NextDouble() - rnd.NextDouble()) * 40;
        //float z = (float)(rnd.NextDouble() - rnd.NextDouble()) * 40;
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
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
        Effect effect1;
        Particle[] particles;
        VertexPositionTexture[] vertex;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix rotashion = Matrix.Identity;
        Matrix World = Matrix.Identity;
        Matrix translathion = Matrix.Identity;
        MouseState mouse;
        MouseState lastMouseState;
        KeyboardState l;
        KeyboardState n;
        float cameraDistance;
        SpriteFont font;
        int c;
        double t;
        string s;
        DynamicVertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        private int[] indices;
        Texture2D texture;
        #endregion

        public Game1()
        {

            particles = new Particle[100];
            Content.RootDirectory = "Content";
            cameraDistance = 100;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 700;
            graphics.PreferredBackBufferWidth = 800;
            Window.Title = "Kezumie";
            IsMouseVisible = true;
            //Создаем матрицы вида, проекции и камеры.
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 100), Vector3.Zero, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                 graphics.PreferredBackBufferWidth /
                (float)graphics.PreferredBackBufferHeight, 1f, 2000);
            World = Matrix.CreateWorld(Vector3.Zero, Vector3.Backward, Vector3.Up);


        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            graphics.GraphicsDevice.Flush();
            vertex = new VertexPositionTexture[particles.Length * 4];
            vertexBuffer = new DynamicVertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionTexture), vertex.Length, BufferUsage.WriteOnly);
            indices = new int[particles.Length * 6];
            indexBuffer = new IndexBuffer(graphics.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);

            for (int i = 0; i < particles.Length; i++)
            {
                Random rnd = new Random(i);
                double R = rnd.NextDouble() * 10;
                float sin = (float)(rnd.NextDouble() * 180);
                float cos = (float)(rnd.NextDouble() * 360);
                float x = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Cos(MathHelper.ToRadians(cos)));
                float y = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Sin(MathHelper.ToRadians(cos)));
                float z = (float)(R * Math.Cos(MathHelper.ToRadians(sin)));
                //Создаем частицу с начальными данными
                particles[i] = new Particle(15, new Vector3(0f, 0f, 0f))
                {
                    EndPosition = new Vector3(x, y, z),
                    Size = 10f,
                };
                particles[i].Init();

                vertex[i * 4] = particles[i].Vertex[0];
                vertex[i * 4 + 1] = particles[i].Vertex[1];
                vertex[i * 4 + 2] = particles[i].Vertex[2];
                vertex[i * 4 + 3] = particles[i].Vertex[3];
                indices[i * 6] = 0 + i * 4;
                indices[i * 6 + 1] = 1 + i * 4;
                indices[i * 6 + 2] = 2 + i * 4;
                indices[i * 6 + 3] = 0 + i * 4;
                indices[i * 6 + 4] = 2 + i * 4;
                indices[i * 6 + 5] = 3 + i * 4;
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
            effect1 = Content.Load<Effect>("sd");
            font = Content.Load<SpriteFont>("font");
            using (FileStream fs = new FileStream("Content/smoke5.png", System.IO.FileMode.Open))
                texture = Texture2D.FromStream(graphics.GraphicsDevice, fs);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            texture.Dispose();
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
            //   MoveAsync(particles, gameTime);
            CameraMove();
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i].isMoving)
                {
                    particles[i].Move(gameTime);
                }

                //Vector3 pos = particles[i].Vertex[0].Position;
                //float size = particles[i].Size;
                //Vector3 ofset0 = new Vector3(0, 0, 0);
                //Vector3 ofset = new Vector3(size, 0, 0);
                //Vector3 ofset1 = new Vector3(size, size, 0);
                //Vector3 ofset2 = new Vector3(0, size, 0);
                //particles[i].Vertex[0].Position = pos +
                //(ofset0.X * viewMatrix.Right) + (ofset0.Y * viewMatrix.Up);

                //particles[i].Vertex[1].Position = pos +
                // (ofset.X * viewMatrix.Right) + (ofset.Y * viewMatrix.Up);

                //particles[i].Vertex[2].Position = pos +
                // (ofset1.X * viewMatrix.Right) + (ofset1.Y * viewMatrix.Up);

                //particles[i].Vertex[3].Position = pos +
                //(ofset2.X * viewMatrix.Right) + (ofset2.Y * viewMatrix.Up);



                //                pos - позиция билборда
                //pos_offset - смещение вершины, тоже самое что в примере с шейдером
                //vertex_position = pos + pos_offset.x * right + pos_offset.y * up;
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
            effect1.Parameters["View"].SetValue(viewMatrix);
            effect1.Parameters["Projection"].SetValue(projectionMatrix);
            effect1.Parameters["Texture"].SetValue(texture);
            graphics.GraphicsDevice.BlendState = BlendState.Additive;
            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            graphics.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            graphics.GraphicsDevice.Indices = indexBuffer;

            effect1.CurrentTechnique.Passes[0].Apply();
            graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertex.Length, 0, indices.Length / 3);

            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "For Nami by Victorem" + s, new Vector2(5, 5), Color.Aqua);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        /// <summary>
        /// Метод для передвежения камеры
        /// </summary>
        private void CameraMove()
        {
            mouse = Mouse.GetState();
            //Врашаем камеру вокруг нулевых кордианат
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) rotashion *= Matrix.CreateRotationY(-1 * MathHelper.ToRadians(1));
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) rotashion *= Matrix.CreateRotationY(MathHelper.ToRadians(1));
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) rotashion *= Matrix.CreateRotationX(-1 * MathHelper.ToRadians(1));
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) rotashion *= Matrix.CreateRotationX(MathHelper.ToRadians(1));
            if (Keyboard.GetState().IsKeyDown(Keys.A)) rotashion *= Matrix.CreateRotationZ(-1 * MathHelper.ToRadians(1));
            if (Keyboard.GetState().IsKeyDown(Keys.D)) rotashion *= Matrix.CreateRotationZ(MathHelper.ToRadians(1));
            //Изменяем дистанцию камеры колесиком мыши
            if (mouse.ScrollWheelValue < lastMouseState.ScrollWheelValue) cameraDistance -= 2;
            if (mouse.ScrollWheelValue > lastMouseState.ScrollWheelValue) cameraDistance += 2;
            //Изменяем дистанцию камеры клавиатурой
            if (Keyboard.GetState().IsKeyDown(Keys.S)) cameraDistance -= 1;
            if (Keyboard.GetState().IsKeyDown(Keys.W)) cameraDistance += 1;
            //Проверяем не вышла ли камера за дозволенную дистанцию
            if (cameraDistance < 1) cameraDistance = 1;
            if (cameraDistance > 500) cameraDistance = 500;
            //Установливаем новое значение для камеры
            viewMatrix = rotashion * Matrix.CreateLookAt(new Vector3(0, 0, cameraDistance), Vector3.Zero, Vector3.Up);
            //Сохраняем текушее состояние мыши
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
            if (c == 100)
            {
                s += " " + c + " " + t / 1000 + Environment.NewLine;

            }
        }

        //Диапазон координат от -40 до 40
        //float x = (float)(rnd.NextDouble() - rnd.NextDouble()) * 40;
        //float y = (float)(rnd.NextDouble() - rnd.NextDouble()) * 40;
        //float z = (float)(rnd.NextDouble() - rnd.NextDouble()) * 40;
    }
}

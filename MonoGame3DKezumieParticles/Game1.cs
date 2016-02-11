using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MonoGame3DKezumieParticles
{
    public class Game1 : Game
    {
        #region Поля       
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Effect effect1;
        Particle[] particles;
        VertexPositionTexture[] vertex;
        Matrix projectionMatrix;
        Matrix viewMatrix;      
        SpriteFont font;       
        DynamicVertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        private int[] indices;
        Texture2D texture;
        Vector2 Size;
        Task task = new Task(()=> { });
        #endregion
        /// <summary>
        /// Надо сделать точки -1 -1 и тд чтобы от центра шел ебаный шар
        /// </summary>
        public Game1()
        {
            particles = new Particle[1000000];
            indices = new int[particles.Length * 6];
            vertex = new VertexPositionTexture[particles.Length * 4];
            Size = new Vector2(0.01f, 0.01f);
            cameraDistance = 100;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 700;
            graphics.PreferredBackBufferWidth = 800;
            Content.RootDirectory = "Content";
            Window.Title = "Kezumie";
            IsMouseVisible = true;
            //Создаем матрицы вида, проекции и камеры.
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 100), Vector3.Zero, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                 graphics.PreferredBackBufferWidth /
                (float)graphics.PreferredBackBufferHeight, 1f, 2000);
        }

        protected override void Initialize()
        {
            //Создаем буффер индексов и вершин
            graphics.GraphicsDevice.Flush();
            vertexBuffer = new DynamicVertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionTexture), vertex.Length, BufferUsage.WriteOnly);
            indexBuffer = new IndexBuffer(graphics.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            //Цикл для заполнения данным массива вершин
            for (int i = 0; i < particles.Length; i++)
            {
                Random rnd = new Random(i);
                //Вычисляем позицию частицы в трехмерном пространстве
                double R = rnd.NextDouble() * 20;
                float sin = (float)(rnd.NextDouble() * 180);
                float cos = (float)(rnd.NextDouble() * 360);
                float x = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Cos(MathHelper.ToRadians(cos)));
                float y = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Sin(MathHelper.ToRadians(cos)));
                float z = (float)(R * Math.Cos(MathHelper.ToRadians(sin)));
                //Создаем частицу с начальными данными
                particles[i] = new Particle(2, new Vector3(0f, 0f, 0f)) { EndPosition = new Vector3(x, y, z) };
                particles[i].Init();
                //Переносим данные о точках частицы в массив вершин.
                vertex[i * 4] = particles[i].Vertex[0];
                vertex[i * 4 + 1] = particles[i].Vertex[1];
                vertex[i * 4 + 2] = particles[i].Vertex[2];
                vertex[i * 4 + 3] = particles[i].Vertex[3];
                //Создаем массив индексов для вершин.
                indices[i * 6] = 0 + i * 4;
                indices[i * 6 + 1] = 1 + i * 4;
                indices[i * 6 + 2] = 2 + i * 4;
                indices[i * 6 + 3] = 0 + i * 4;
                indices[i * 6 + 4] = 2 + i * 4;
                indices[i * 6 + 5] = 3 + i * 4;
            }
            //Переносим данные в буффер для видеокарты.
            indexBuffer.SetData(indices);
            vertexBuffer.SetData(vertex);
            //Устанавливаем параметры отображения наших объектов           

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            effect1 = Content.Load<Effect>("sd");
            font = Content.Load<SpriteFont>("font");
            using (FileStream fs = new FileStream("Content/smoke5.png", System.IO.FileMode.Open))
                texture = Texture2D.FromStream(graphics.GraphicsDevice, fs);
            effect1.Parameters["Projection"].SetValue(projectionMatrix);
            effect1.Parameters["Texture"].SetValue(texture);
            effect1.Parameters["Size"].SetValue(Size);
            task.Start();
        }

        protected override void UnloadContent()
        {
            texture.Dispose();
        }
        double time;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            time += gameTime.ElapsedGameTime.TotalMilliseconds;
           if (time < 32) return;
            time = 0;            
            task = MoveAsync(gameTime);
            CameraMove();          
            base.Update(gameTime);
            
        }

        protected override void Draw(GameTime gameTime)
        {
            FPS(gameTime);
            //Ждем завершения асинхронной задачи в которой меняеться позиция вертексов
            task.Wait();
            vertexBuffer.SetData(vertex);
            graphics.GraphicsDevice.Clear(Color.Black);
            effect1.Parameters["View"].SetValue(viewMatrix);
            //В Аддитив режими смешиваються только цвета, прозрачность остаетсья той же
            graphics.GraphicsDevice.BlendState = BlendState.Additive;
            //Сообщаем видеокарте чтобы она не рисовала одно из плоскостей треугольника.
            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            //Линейное сжатие текстуры - она будет сжиматься под соотношение сторо нашего квадрата
            graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            //Устанавливаем чтение глубины, без этого больше 2 объектов один за другим не будет видно (остальных закроют передние) ! Обязательно.
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;           
            //Устанавливаем для видеокарты буффер вершин и индексы для него.          
            graphics.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            graphics.GraphicsDevice.Indices = indexBuffer;
            //Включаем наш шейдер
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
        Matrix rotashion = Matrix.Identity;
        Matrix translathion = Matrix.Identity;
        MouseState mouse;
        MouseState lastMouseState;
        float cameraDistance;
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
        Object o = new Object();
        Task MoveAsync(GameTime gameTime)
        {
            return Task.Factory.StartNew(() =>
              {
                  lock (o)
                  {
                      for (int i = 0; i < particles.Length; i++)
                      {
                          if (particles[i].isMoving)
                          {
                              particles[i].Move(gameTime);
                              vertex[i * 4] = particles[i].Vertex[0];
                              vertex[i * 4 + 1] = particles[i].Vertex[1];
                              vertex[i * 4 + 2] = particles[i].Vertex[2];
                              vertex[i * 4 + 3] = particles[i].Vertex[3];
                          }
                      }
                  }
              });
        }

        int c;
        double t;
        string s;
        private void FPS(GameTime gameTime)
        {
            ++c;
            t += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (c == 1000)
            {
                s += " " + c + " " + t / 1000 + Environment.NewLine;

            }
        }
    }
}

//Диапазон координат от -40 до 40
//float x = (float)(rnd.NextDouble() - rnd.NextDouble()) * 40;
//float y = (float)(rnd.NextDouble() - rnd.NextDouble()) * 40;
//float z = (float)(rnd.NextDouble() - rnd.NextDouble()) * 40;
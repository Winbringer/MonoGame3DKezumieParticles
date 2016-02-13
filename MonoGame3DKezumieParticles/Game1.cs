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
        VertexPositionNormalTexture[] vertex;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        SpriteFont font;
        DynamicVertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        private int[] indices;
        Texture2D texture;
        Vector2 Size;
        bool isMoving;
        #endregion
        //Сделать расчит кривыз Безьера
        public Game1()
        {
            particles = new Particle[1000000];
            Size = new Vector2(1f, 1f);
            cameraDistance = 500;
            indices = new int[particles.Length * 6];
            vertex = new VertexPositionNormalTexture[particles.Length * 4];
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 700;
            graphics.PreferredBackBufferWidth = 800;
            //Убераем ограничения на частоту вызова метода Update и Draw  
            graphics.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            Content.RootDirectory = "Content";
            Window.Title = "Kezumie";
            IsMouseVisible = true;
            isMoving = true;
            //Создаем матрицы вида, проекции и камеры.
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, cameraDistance), Vector3.Zero, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                 graphics.PreferredBackBufferWidth /
                (float)graphics.PreferredBackBufferHeight, 1f, 550);
        }

        #region Инициализация и загрузка начальных данных
        protected override void Initialize()
        {
            //Создаем буффер индексов и вершин
            graphics.GraphicsDevice.Flush();
            vertexBuffer = new DynamicVertexBuffer(graphics.GraphicsDevice, typeof(VertexPositionNormalTexture), vertex.Length, BufferUsage.WriteOnly);
            indexBuffer = new IndexBuffer(graphics.GraphicsDevice, typeof(int), indices.Length, BufferUsage.WriteOnly);
            //Цикл для заполнения данным массива вершин
            for (int i = 0; i < particles.Length; i++)
            {
                Random rnd = new Random(i);
                //Вычисляем позицию частицы в трехмерном пространстве
                double R = rnd.NextDouble() * 100;
                float sin = (float)(rnd.NextDouble() * 180);
                float cos = (float)(rnd.NextDouble() * 360);
                float x = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Cos(MathHelper.ToRadians(cos)));
                float y = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Sin(MathHelper.ToRadians(cos)));
                float z = (float)(R * Math.Cos(MathHelper.ToRadians(sin)));
                //Создаем частицу с начальными данными
                particles[i] = new Particle(3, new Vector3(0, 0, 0)) { EndPosition = new Vector3(x, y, z) };
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
            //Вызываем иниталайз для базового класса и всех компоненетов, если они у нас есть.
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
        }

        protected override void UnloadContent()
        {
            texture.Dispose();
        }
        #endregion

        #region Обновление данных и отображение их на экран
       
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();           
             if(isMoving) isMoving = Move(gameTime);            
            CameraMove(gameTime);
            base.Update(gameTime);
        }
        string s ="";
        double t=0;
       double f=0;
        protected override void Draw(GameTime gameTime)
        {
            ++f;
            t += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (f > 100) { s = "fps: " + Convert.ToInt32((f / t) * 1000); f = 0; t = 0; }
            vertexBuffer.SetData(vertex);
            graphics.GraphicsDevice.Clear(Color.Black);
            effect1.Parameters["View"].SetValue(viewMatrix);
            //В Аддитив режими смешиваються только цвета, прозрачность остаетсья той же
            graphics.GraphicsDevice.BlendState = BlendState.Additive;
            //Сообщаем видеокарте чтобы она не рисовала одну из плоскостей треугольника.
            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            //Линейное сжатие текстуры - она будет сжиматься под соотношение сторо нашего квадрата
            // graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            //Устанавливаем чтение глубины, без этого больше 2 объектов один за другим не будет видно (остальных закроют передние) ! Обязательно.
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            //Устанавливаем для видеокарты буффер вершин и индексы для него.             
            graphics.GraphicsDevice.SetVertexBuffer(vertexBuffer);
            graphics.GraphicsDevice.Indices = indexBuffer;
            //Включаем наш шейдер
            effect1.CurrentTechnique.Passes[0].Apply();
            graphics.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertex.Length, 0, indices.Length / 3);
            DrawString();
            base.Draw(gameTime);
        }
        #endregion

        #region Вспомогательные методы
        private void DrawString()
        {
            //Пишем на экране текст.
            spriteBatch.Begin();
            spriteBatch.DrawString(font,
                "For Nami by Victorem. " + s + Environment.NewLine + "Use arrows on the keyboard and mouse wheel to move the camera",
                new Vector2(5, 5), Color.Red);
            spriteBatch.End();
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        /// <summary>
        /// Метод для передвежения камеры
        /// </summary>
        Matrix rotashion = Matrix.Identity;
        Matrix translathion = Matrix.Identity;
        MouseState mouse;
        MouseState lastMouseState;
        float cameraDistance;
        private void CameraMove(GameTime gameTime)
        {
            //Скорость поворота расчитываемая в зависимости от времени.
            float r =(float)( 0.1 * gameTime.ElapsedGameTime.TotalMilliseconds);
            //Скорость поворота в радианах.
            float pi = MathHelper.ToRadians(r);
            mouse = Mouse.GetState();
            //Врашаем камеру вокруг нулевых кордианат
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) rotashion *= Matrix.CreateRotationY(-1 *pi);
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) rotashion *= Matrix.CreateRotationY(pi);
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) rotashion *= Matrix.CreateRotationX(-1 * pi);
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) rotashion *= Matrix.CreateRotationX(pi);
            if (Keyboard.GetState().IsKeyDown(Keys.A)) rotashion *= Matrix.CreateRotationZ(-1 * pi);
            if (Keyboard.GetState().IsKeyDown(Keys.D)) rotashion *= Matrix.CreateRotationZ(pi);
            //Изменяем дистанцию камеры колесиком мыши
            if (mouse.ScrollWheelValue < lastMouseState.ScrollWheelValue) cameraDistance -= 4*r;
            if (mouse.ScrollWheelValue > lastMouseState.ScrollWheelValue) cameraDistance += 4*r;
            //Изменяем дистанцию камеры клавиатурой
            if (Keyboard.GetState().IsKeyDown(Keys.S)) cameraDistance += r;
            if (Keyboard.GetState().IsKeyDown(Keys.W)) cameraDistance -= r;
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
                  for (int i = 0; i < particles.Length; i++)
                  {
                      lock (o)
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
        bool Move(GameTime gameTime)
        {
            int j=0;
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i].isMoving)
                {
                    if (particles[i].isMoving)
                    {
                        ++j;
                        particles[i].Move(gameTime);
                        vertex[i * 4] = particles[i].Vertex[0];
                        vertex[i * 4 + 1] = particles[i].Vertex[1];
                        vertex[i * 4 + 2] = particles[i].Vertex[2];
                        vertex[i * 4 + 3] = particles[i].Vertex[3];
                    }
                }
            }
            if (j > 50) return true;
            return false;

        }
        #endregion
    }
}

//Диапазон координат от -40 до 40
//float x = (float)(rnd.NextDouble() - rnd.NextDouble()) * 40;
//float y = (float)(rnd.NextDouble() - rnd.NextDouble()) * 40;
//float z = (float)(rnd.NextDouble() - rnd.NextDouble()) * 40;
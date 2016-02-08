using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace MonoGame3DKezumieParticles
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        BasicEffect effect;

        List<Particle> particles;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;
        Matrix rotationMatrix = Matrix.Identity;

        VertexPositionColor[] Vertex;
        float cameraDistance;
        SpriteFont font;

        public Game1()
        {

            Content.RootDirectory = "Content";
            cameraDistance = 50;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = 700;
            graphics.PreferredBackBufferWidth = 800;
            Window.Title = "Kezumie";
            IsMouseVisible = true;
            
            particles = new List<Particle>();

            for (int i = 0; i < 10000; i++)
            {
                particles.Add(new Particle(10, new Vector3(0f, 0f, 0f)));
            }
            Vertex = new VertexPositionColor[particles.Count * 12];
           

            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 200), Vector3.Zero, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                 graphics.PreferredBackBufferWidth /
                (float)graphics.PreferredBackBufferHeight,
                1f, 500);
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
            int i = 0;
            // TODO: Add your initialization logic here
           
            foreach (var item in particles)
            {
                ++i;
                Random rnd = new Random(i);
                //Диапазон координат от -40 до 40
                //float x = (float)(rnd.NextDouble() - rnd.NextDouble()) * 40;
                //float y = (float)(rnd.NextDouble() - rnd.NextDouble()) * 40;
                //float z = (float)(rnd.NextDouble() - rnd.NextDouble()) * 40;
                double R = rnd.NextDouble() * 40; 
                float sin = (float)(rnd.NextDouble() * 180);
                float cos = (float)(rnd.NextDouble() * 360);
                float x = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Cos(MathHelper.ToRadians(cos)));
                float y = (float)(R * Math.Sin(MathHelper.ToRadians(sin)) * Math.Sin(MathHelper.ToRadians(cos)));
                float z = (float)(R * Math.Cos(MathHelper.ToRadians(sin)));

                item.EndPosition = new Vector3(x,y,z);
                item.Size = 1f;
                item.Init();

            }
            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            effect = new BasicEffect(graphics.GraphicsDevice);
           // InitializeEffect();
            font = Content.Load<SpriteFont>("font");
            // TODO: use this.Content to load your game content here
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

            // TODO: Add your update logic here
            int j = 0;
            foreach (var item in particles)
            {
               if(item.isMoving) item.Move(gameTime);
                
                //   item.vertex.CopyTo(Vertex, j);
                //j += 12;            
                
            }
         
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            graphics.GraphicsDevice.Clear(Color.Black);            
            graphics.GraphicsDevice.BlendState = BlendState.Opaque;
            graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphics.GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;       

            // TODO: Add your drawing code here

            effect.VertexColorEnabled = true;
            effect.World = worldMatrix;
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;
            
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                foreach (var item in particles)
                {
                    
                   // graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, item.vertex, 0, 4);
                    graphics.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, item.vertex, 0, item.vertex.Length, item.indexes, 0, 4);
                }
                }
           
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "For Nami by Victorem", new Vector2(5, 5), Color.White);
            spriteBatch.End();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            base.Draw(gameTime);
        }

        /// <summary>
        /// Инициализация базовых эффектов (настроек параметров и способов обработки)
        /// используемых для работы с трехмерной моделью
        /// </summary>
        private void InitializeEffect()
        {
            //Создание объекта для вывода изображений
            effect = new BasicEffect(graphics.GraphicsDevice);
            //Установка матриц
            effect.World = worldMatrix;
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;

            // Цвета различных видов освещения
            effect.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
            effect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);
            effect.SpecularColor = new Vector3(0.25f, 0.25f, 0.25f);
            effect.SpecularPower = 5.0f;
            effect.Alpha = 1.0f;
            //Включим освещение
            effect.LightingEnabled = true;
            if (effect.LightingEnabled)
            {
                effect.DirectionalLight0.Enabled = true; // активируем каждый источник света отдельно
                if (effect.DirectionalLight0.Enabled)
                {
                    // Направление по Х
                    effect.DirectionalLight0.DiffuseColor = new Vector3(1, 0, 0); // диапазон от 0 до 1
                    effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(-1, 0, 0));
                    // Направление от источника света к началу координат сцены
                    effect.DirectionalLight0.SpecularColor = Vector3.One;
                }

                effect.DirectionalLight1.Enabled = true;
                if (effect.DirectionalLight1.Enabled)
                {
                    // Направление по У
                    effect.DirectionalLight1.DiffuseColor = new Vector3(0, 0.75f, 0);
                    effect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(0, -1, 0));
                    effect.DirectionalLight1.SpecularColor = Vector3.One;
                }

                effect.DirectionalLight2.Enabled = true;
                if (effect.DirectionalLight2.Enabled)
                {
                    // Направление по Z
                    effect.DirectionalLight2.DiffuseColor = new Vector3(0, 0, 0.5f);
                    effect.DirectionalLight2.Direction = Vector3.Normalize(new Vector3(0, 0, -1));
                    effect.DirectionalLight2.SpecularColor = Vector3.One;
                }
            }

        }
    }
}

#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

#endregion

namespace Blaxx
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;

        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 1), new Vector3(0, 0, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800 / 480f, 0.1f, 100f);

        public short[] inds; // box inds
        public VertexPositionColor[] tempvert; // box verts

        BasicEffect basicEffect;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Content = new ContentManager(this.Services, "Content");

            tempvert = new VertexPositionColor[8];
            inds = new short[36] { 1, 2, 3, 4, 1, 3, 4, 3, 7, 0, 4, 7, 3, 2, 6, 7, 3, 6, 1, 4, 0, 5, 1, 0, 0, 6, 5, 0, 7, 6, 2, 1, 6, 1, 5, 6 };
            tempvert[0] = new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.5f), Color.Red);
            tempvert[1] = new VertexPositionColor(new Vector3(-0.5f, 0.5f, -0.5f), Color.Red);
            tempvert[2] = new VertexPositionColor(new Vector3(0.5f, 0.5f, -0.5f), Color.Red);
            tempvert[3] = new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.5f), Color.Red);
            tempvert[4] = new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0.5f), Color.Red);
            tempvert[5] = new VertexPositionColor(new Vector3(-0.5f, -0.5f, -0.5f), Color.Red);
            tempvert[6] = new VertexPositionColor(new Vector3(0.5f, -0.5f, -0.5f), Color.Red);
            tempvert[7] = new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.5f), Color.Red);

            basicEffect = new BasicEffect(graphics.GraphicsDevice);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Random rand = new Random(1);
            graphics.GraphicsDevice.Clear(Color.White);
            //DrawBox(new Vector3(0, 0, -100));
            //DrawBox(new Vector3(2, 0, -100));
            //DrawLine(new Vector3(-2, 0, -100), new Vector3(-20, 30, -100));
            for (int i = 0; i < 3000; ++i)
            {
                Vector3 center = new Vector3(0, 0, -150);
                Vector3 coord = GenerateRandomSphereCoordinate(center, 60.0, rand);
                DrawLine(coord, center, 1f);
                DrawBox(coord, 0.5f);
            }
            base.Draw(gameTime);
        }

        public void DrawBox(Vector3 position, float scale)
        {    
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;
            basicEffect.Projection = projection;
            basicEffect.World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            foreach (EffectPass p in basicEffect.CurrentTechnique.Passes)
            {
                p.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, tempvert, 0, 8, inds, 0, 12);
            }
        }

        public void DrawLine(Vector3 pos_one, Vector3 pos_two, float scale)
        {
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;
            basicEffect.Projection = projection;
            Vector3 unit_diff = pos_one - pos_two;
            unit_diff.Normalize();
            basicEffect.World =
                Matrix.CreateScale(new Vector3((pos_one - pos_two).Length() + scale, scale, scale)) *
                Matrix.CreateFromAxisAngle(Vector3.Cross(Vector3.UnitX, unit_diff),
                                           (float)Math.Acos(Vector3.Dot(Vector3.UnitX, unit_diff))) *
                Matrix.CreateTranslation((pos_one + pos_two) / 2.0f); ;

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            foreach (EffectPass p in basicEffect.CurrentTechnique.Passes)
            {
                p.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, tempvert, 0, 8, inds, 0, 12);
            }
        }

        public Vector3 GenerateRandomSphereCoordinate(Vector3 center, double radius, Random rand)
        {
            double U = (float)rand.NextDouble();
            double V = (float)rand.NextDouble();
            double theta = 2 * Math.PI * U;
            double phi = Math.Acos(2 * V - 1);
            double x = radius * Math.Cos(theta) * Math.Sin(phi);
            double y = radius * Math.Sin(theta) * Math.Sin(phi);
            double z = radius * Math.Cos(phi);
            return (new Vector3((float)x, (float)y, (float)z) + center);
        }
    }
}


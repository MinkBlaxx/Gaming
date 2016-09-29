#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

#endregion

namespace Blaxx {
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game {
        GraphicsDeviceManager graphics;

        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Vector3 camera = new Vector3(0, 0, 0);
        private float camera_yaw = 0f;
        private float camera_pitch = 0f;
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, 1), new Vector3(0, 0, 0), Vector3.UnitY);
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800 / 480f, 0.001f, 10000.0f);

        public short[] inds; // box inds
        public VertexPositionColor[] tempvert; // box verts

        BasicEffect basicEffect;

        public Game1() {
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
        protected override void Initialize() {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        protected override void LoadContent() {
            Content = new ContentManager(this.Services, "Content");

            tempvert = new VertexPositionColor[8];
            inds = new short[36] { 1, 2, 3, 4, 1, 3, 4, 3, 7, 0, 4, 7, 3, 2, 6, 7, 3, 6, 1, 4, 0, 5, 1, 0, 0, 6, 5, 0, 7, 6, 2, 1, 6, 1, 5, 6 };
            tempvert[0] = new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.5f), Color.Red);
            tempvert[1] = new VertexPositionColor(new Vector3(-0.5f, 0.5f, -0.5f), Color.Red);
            tempvert[2] = new VertexPositionColor(new Vector3(0.5f, 0.5f, -0.5f), Color.Blue);
            tempvert[3] = new VertexPositionColor(new Vector3(0.5f, 0.5f, 0.5f), Color.Blue);
            tempvert[4] = new VertexPositionColor(new Vector3(-0.5f, 0.5f, 0.5f), Color.Red);
            tempvert[5] = new VertexPositionColor(new Vector3(-0.5f, -0.5f, -0.5f), Color.Red);
            tempvert[6] = new VertexPositionColor(new Vector3(0.5f, -0.5f, -0.5f), Color.Blue);
            tempvert[7] = new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.5f), Color.Blue);

            basicEffect = new BasicEffect(graphics.GraphicsDevice);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Left)) {
                camera_yaw = 
                    (camera_yaw
                     - (float)(Math.PI * 0.03)
                     + (float)(Math.PI * 2.0))
                    % ((float)(Math.PI * 2.0));
            }
            if (state.IsKeyDown(Keys.Right)) {
                camera_yaw =
                    (camera_yaw + (float)(Math.PI * 0.03))
                    % ((float)(Math.PI * 2.0));
            }
            if (state.IsKeyDown(Keys.Up)) {
                camera_pitch =
                    Math.Min(camera_pitch + (float)(Math.PI * 0.03), (float)(Math.PI / 2.0) - 0.0001f);
            }
            if (state.IsKeyDown(Keys.Down)) {
                camera_pitch =
                    Math.Max(camera_pitch - (float)(Math.PI * 0.03), (float)(-Math.PI / 2.0) + 0.0001f);
            }
            double x = Math.Cos(camera_yaw) * Math.Cos(camera_pitch);
            double z = Math.Sin(camera_yaw) * Math.Cos(camera_pitch);
            double y = Math.Sin(camera_pitch);
            Vector3 cam_normal = new Vector3((float)x, (float)y, (float)z);
            float speed = 0.1f;
            if (state.IsKeyDown(Keys.W)) {
                camera += speed * cam_normal;
            }
            if (state.IsKeyDown(Keys.S)) {
                camera -= speed * cam_normal;
            }
            Vector3 left_of_me = Vector3.Cross(cam_normal, Vector3.UnitY);
            left_of_me.Normalize();
            if (state.IsKeyDown(Keys.A)) {
                camera -= speed * left_of_me;
            }
            if (state.IsKeyDown(Keys.D)) {
                camera += speed * left_of_me;
            }
            CalculateCameraView();
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime) {
            Random rand = new Random(1);
            graphics.GraphicsDevice.Clear(Color.White);
            //DrawBox(new Vector3(0, 0, -100));
            //DrawBox(new Vector3(2, 0, -100));
            //DrawLine(new Vector3(-2, 0, -100), new Vector3(-20, 30, -100));
            for (int i = 0; i < 30; ++i)
            {
                Vector3 center = new Vector3(150, 0, 0);
                Vector3 coord = GenerateRandomSphereCoordinate(center, 60.0, rand);
                DrawLine(coord, center, 0.3f);
                DrawBox(coord, 0.5f);
            }
            base.Draw(gameTime);
        }

        public void DrawBox(Vector3 position, float scale) {    
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;
            basicEffect.Projection = projection;
            basicEffect.World = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);
            basicEffect.View = view;

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            foreach (EffectPass p in basicEffect.CurrentTechnique.Passes)
            {
                p.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, tempvert, 0, 8, inds, 0, 12);
            }
        }

        public void DrawLine(Vector3 pos_one, Vector3 pos_two, float scale) {
            basicEffect.VertexColorEnabled = true;
            basicEffect.LightingEnabled = false;
            basicEffect.Projection = projection;
            Vector3 unit_diff = pos_two - pos_one;
            unit_diff.Normalize();
            Vector3 cross = Vector3.Cross(Vector3.UnitX, unit_diff);
            cross.Normalize();
            basicEffect.World =
                Matrix.CreateScale(new Vector3((pos_one - pos_two).Length() + scale, scale, scale))
                * Matrix.CreateFromAxisAngle(
                    cross, (float)Math.Acos(Vector3.Dot(Vector3.UnitX, unit_diff)))
                * Matrix.CreateTranslation((pos_one + pos_two) / 2.0f);
            basicEffect.View = view;

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            foreach (EffectPass p in basicEffect.CurrentTechnique.Passes) {
                p.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, tempvert, 0, 8, inds, 0, 12);
            }
        }

        protected void CalculateCameraView() {
            double x = Math.Cos(camera_yaw) * Math.Cos(camera_pitch);
            double z = Math.Sin(camera_yaw) * Math.Cos(camera_pitch);
            double y = Math.Sin(camera_pitch);
            Vector3 cam_lookat = new Vector3((float)x, (float)y, (float)z) + camera;
            view = Matrix.CreateLookAt(camera, cam_lookat, Vector3.UnitY);
        }

        public Vector3 GenerateRandomSphereCoordinate(Vector3 center, double radius, Random rand) {
            double U = (float)rand.NextDouble();
            double V = (float)rand.NextDouble();
            double theta = 2 * Math.PI * U;
            double phi = Math.Acos(2 * V - 1);
            double x = radius * Math.Cos(theta) * Math.Sin(phi);
            double z = radius * Math.Sin(theta) * Math.Sin(phi);
            double y = radius * Math.Cos(phi);
            return (new Vector3((float)x, (float)y, (float)z) + center);
        }
    }
}


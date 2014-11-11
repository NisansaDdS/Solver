using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Solver
{
    /// <summary>
    /// Chess set graphics from http://www.cgtrader.com/free-3d-models/sport-hobby/board-game/chess-set--3#_=_
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Model chessModel;
        Effect effect;
        Matrix viewMatrix;
        Matrix projectionMatrix;
        GraphicsDevice device;
        Vector3 lightDirection = new Vector3(3, -2, 5);
        Texture2D boardTexture;
        Texture2D wBishopTexture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            device = graphics.GraphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);            
            effect = Content.Load<Effect>("effects");
            boardTexture = Content.Load<Texture2D>("BoardSurface_Color");
            wBishopTexture = Content.Load<Texture2D>("W_BishopSurface_Color");
            //chessModel = LoadModel ("chess");

            chessModel =Content.Load<Model>("chess");
            SetUpCamera();
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            DrawModel1();

            base.Draw(gameTime);
        }


        private void SetUpCamera()
        {
            viewMatrix = Matrix.CreateLookAt(new Vector3(20, 13, -5), new Vector3(8, 0, -7), new Vector3(0, 1, 0));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 0.2f, 500.0f);
        }


        private Model LoadModel(string assetName)
        {

            Model newModel = Content.Load<Model>(assetName); 
            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone(GraphicsDevice);
            return newModel;
        }

        float val = 100000000000000000000000000000000000000f;


        private void DrawModel1()
        {
            val = 0.0005f;//= 10; // 0.0005f;
            Window.Title = " " + val;
            Matrix worldMatrix = Matrix.CreateScale(val, val, val) * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(new Vector3(19, 12, -5));

            Matrix[] chessTransforms = new Matrix[chessModel.Bones.Count];
            chessModel.CopyAbsoluteBoneTransformsTo(chessTransforms);
            // Draw the model. A model can have multiple meshes, so loop.


            foreach (ModelMesh mesh in chessModel.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.TextureEnabled = true;
                    if(mesh.Name.Contains("Board")){
                        effect.Texture = boardTexture;
                    }
                    else if (mesh.Name.Equals("Bishop_1") || mesh.Name.Equals("Bishop_2"))
                    {
                        //Console.WriteLine(mesh.Name);
                        effect.Texture = wBishopTexture;
                    }
                    effect.EnableDefaultLighting();
                    effect.World = chessTransforms[mesh.ParentBone.Index] * worldMatrix;
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }







        private void DrawModel()
        {
            val = 0.0005f ;//= 10; // 0.0005f;
            Window.Title = " " + val;
            Matrix worldMatrix = Matrix.CreateScale(val, val, val) * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(new Vector3(19, 12, -5));

            Matrix[] chessTransforms = new Matrix[chessModel.Bones.Count];
            chessModel.CopyAbsoluteBoneTransformsTo(chessTransforms);
            foreach (ModelMesh mesh in chessModel.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    currentEffect.CurrentTechnique = currentEffect.Techniques["Colored"];
                    //currentEffect.Parameters["xTexture"].SetValue(boardTexture);
                    currentEffect.Parameters["xWorld"].SetValue(chessTransforms[mesh.ParentBone.Index] * worldMatrix);
                    currentEffect.Parameters["xView"].SetValue(viewMatrix);
                    currentEffect.Parameters["xProjection"].SetValue(projectionMatrix);
                    currentEffect.Parameters["xEnableLighting"].SetValue(true);
                    currentEffect.Parameters["xLightDirection"].SetValue(lightDirection);
                    currentEffect.Parameters["xAmbient"].SetValue(0.5f);
                }
                mesh.Draw();
            }
        }
    }
}

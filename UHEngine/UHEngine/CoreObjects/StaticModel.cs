﻿#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UHEngine.CameraManagement;
using UHEngine.ScreenManagement;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using UHEngine.UI;
using BoxCollider;
#endregion

namespace UHEngine.CoreObjects
{
    public class StaticModel
    {
        #region Class Variables
        protected float scale;
        /// <summary>
        /// The Game the object is in
        /// </summary>
        protected Game game;
        /// <summary>
        /// The position of the object in 3D space
        /// </summary>
        public Vector3 Position;
        public bool glow;
        public float startGlowScale;
        public float endGlowScale;
        public float currentGlowScale;
        public int elapsedGlowTime;
        public int maxGlowTime = 40;
        bool glowDecreasing = true;

        CameraManager cameraManager;

        //Model Stuff
        Matrix view;
        public Matrix transforms;
        Matrix[] boneTransforms;
        Matrix rotationMatrixX;
        Matrix rotationMatrixY;
        Matrix rotationMatrixZ;

        public Model model;

        public CollisionMesh CollisionMesh{get; protected set;}

        #endregion

        #region Initialization
        /// <summary>
        /// Default Constructor to setup Model
        /// </summary>
        public StaticModel(Vector3 position)
        {
            model = null;
            game = ScreenManager.Game;
            this.Position = position;
            SetupModel(position);
            SetupCamera();
        }

        /// <summary>
        /// Constructor consisting of a given model
        /// </summary>
        /// <param name="model">Model for use</param>
        public StaticModel(Model newModel, Vector3 position)
        {
            this.model = newModel;
            
            game = ScreenManager.Game;
            SetupModel(position);
            CollisionMesh = new CollisionMesh(model, 4, position);
            SetupCamera();
        }

        /// <summary>
        /// Adds a model to the Static Model and performs setup
        /// </summary>
        /// <param name="model">Model for this instance</param>
        public void SetupModel(Model newModel, Vector3 position)
        {
            this.model = newModel;
            SetupModel(position);
            SetupCamera();
        }

        protected void SetupModel(Vector3 position)
        {
            //set scale
            if(scale == 0)
                scale = 1.0f;

            //save bones
            if (model != null)
            {
                boneTransforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            }

            //setup transforms
            transforms = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);

            //give default rotation
            rotationMatrixX = Matrix.CreateRotationX(0.0f);
            rotationMatrixY = Matrix.CreateRotationY(0.0f);
            rotationMatrixZ = Matrix.CreateRotationZ(0.0f);

            //give default position
            this.Position = position;
        }

        /// <summary>
        /// Sets up default camera information
        /// </summary>
        protected void SetupCamera()
        {
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            view = cameraManager.ViewMatrix;

            if (glow == true)
            {
                this.startGlowScale = scale;
                this.endGlowScale = scale - 0.6f;
                this.currentGlowScale = scale;
            }

        }
        #endregion

        #region Properties
        public string Name { get; set; }
        public Matrix Transforms
        {
            get { return transforms; }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                transforms = Matrix.CreateScale(scale) * Matrix.CreateTranslation(this.Position);
                SetupCamera();
            }
        }
        #endregion

        #region Manipulation
        public void RotateX(float rotation)
        {
            rotationMatrixX = Matrix.CreateRotationX(rotation);
        }

        public void RotateY(float rotation)
        {
            rotationMatrixY = Matrix.CreateRotationY(rotation);
        }

        public void RotateZ(float rotation)
        {
            rotationMatrixZ = Matrix.CreateRotationZ(rotation);
        }
        #endregion

        #region Update and Draw
        public void Update(GameTime gameTime)
        {
            //update view matrix
            //UpdateView();
            UpdateTransforms(gameTime);
        }

        public void UpdateView()
        {
            view = cameraManager.ViewMatrix;
        }

        public void UpdateTransforms(GameTime gameTime)
        {
            if (glow == true)
            {
                elapsedGlowTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedGlowTime >= maxGlowTime)
                {
                    elapsedGlowTime = 0;
                    if (glowDecreasing)
                    {
                        currentGlowScale -= 0.1f;
                    }
                    else
                    {
                        currentGlowScale += 0.1f;
                    }

                    if (currentGlowScale <= endGlowScale || currentGlowScale >= startGlowScale)
                    {
                        glowDecreasing = !glowDecreasing;
                    }
                }

                transforms = Matrix.CreateScale(currentGlowScale) *
                    rotationMatrixY *
                    Matrix.CreateTranslation(Position);
            }
            else
            {
                transforms = Matrix.CreateScale(scale) *
                        rotationMatrixY *
                        Matrix.CreateTranslation(Position);
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (model != null)
            {
                // Draw the model. A model can have multiple meshes, so loop.
                for (int i = 0; i < model.Meshes.Count; i++ )
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in model.Meshes[i].Effects)
                    {
                        //effect.CurrentTechnique.
                        effect.EnableDefaultLighting();
                        effect.World = boneTransforms[model.Meshes[i].ParentBone.Index] * transforms;
                        effect.View = cameraManager.ViewMatrix;
                        effect.Projection = cameraManager.ProjectionMatrix;
                    }
                    // Draw the mesh, using the effects set above.
                    model.Meshes[i].Draw();
                }
            }
        }
        #endregion
    }
}

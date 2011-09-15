using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;
using Microsoft.Xna.Framework;
using UHEngine.ScreenManagement;
using UHEngine.CameraManagement;

namespace UHEngine.CoreObjects
{
    public class AnimatedModel
    {
        #region Fields
        public Model Model { get; set; }
        AnimationPlayer animationPlayer;
        Matrix[] bones;
        CameraManager cameraManager = null;
        SkinningData skinningData = null;
        Vector3 specularVector = new Vector3(0.25f);
        #endregion

        #region Initialization

        public AnimatedModel(Model newModel)
        {
            Model = newModel;
            SetupModel();

            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
        }

        private void SetupModel()
        {
            // Look up our custom skinning information.
            skinningData = Model.Tag as SkinningData;

            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            // Create an animation player, and start decoding an animation clip.
            animationPlayer = new AnimationPlayer(skinningData);

            AnimationClip clip = skinningData.AnimationClips["Take 001"];

            animationPlayer.StartClip(clip);
        }

        #endregion

        #region Clips
        public void PlayClip(string clipName)
        {
            AnimationClip clip = skinningData.AnimationClips[clipName];
            animationPlayer.StartClip(clip);
        }
        #endregion

        #region Update
        public void Update(GameTime gameTime)
        {
            animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            bones = animationPlayer.GetSkinTransforms();
        }
        #endregion

        #region Draw
        public void Draw(GameTime gameTime)
        {

            // Render the skinned mesh.
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);

                    effect.View = cameraManager.ViewMatrix;
                    effect.Projection = cameraManager.ProjectionMatrix;

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = specularVector;
                    effect.SpecularPower = 16;
                }

                mesh.Draw();
            }
        }
        #endregion
    }
}

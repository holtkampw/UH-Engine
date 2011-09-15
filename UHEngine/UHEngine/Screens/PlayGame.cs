using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UHEngine.ScreenManagement;
using UHEngine.UI;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using UHEngine.InputManagement;
using UHEngine.CameraManagement;
using UHEngine.CoreObjects;
using System.IO;
using BoxCollider;

namespace UHEngine.Screens
{
    using Screen = UHEngine.ScreenManagement.Screen;
    class PlayGame : Screen
    {
        #region Fields

        #region UI

        const int NumObjects = 30;
        int FoundObjects = 0;
        const int NumColumns = 3;
        List<UIItem> uiItems = new List<UIItem>();
        Vector2 baseUIIconPosition = new Vector2(54, 10);
        const int SpriteSize = 60;
        Vector2 columnOffset = new Vector2(10 + SpriteSize, 0);
        Vector2 rowOffset = new Vector2(0, 10 + SpriteSize);
        Texture2D uiBackground;
        Color uiBackgroundColor = new Color(255, 255, 255, 200);
        Rectangle uiBackgroundRect = new Rectangle(0, 0, 256, 960);
        Vector2 uiObjectsFoundLocation;
        SpriteFont uiObjectsFoundFont;
        Vector2 uiObjectsFoundFontLocation;

        #endregion UI

        #region Level Model
        StaticModel stage = null;
        #endregion

        #region Collision
        StaticModel collideStage = null;
        CollisionMesh levelCollision = null;
        #endregion

        bool intersected = false;

        List<BoundingSphere> insideBoundingSpheres = new List<BoundingSphere>();
        List<Matrix> modelWorldTransforms = new List<Matrix>();
        List<IActionable> actionableObjects = new List<IActionable>();
        List<GatewayObject> gateways = new List<GatewayObject>();
        List<StaticModel> collisionModels = new List<StaticModel>();

        CameraManager cameraManager = null;

#if WINDOWS_PHONE
        VirtualThumbsticks thumbsticks = null;
#endif
        #endregion

        #region Initialization
        public PlayGame() : base("Play") { }
        #endregion

        #region Load Content
        /// <summary>
        /// Loads All Game Related Content
        /// </summary>
        public override void LoadContent()
        {
            //UI Bar
            //SetupUI();

            cameraManager = new CameraManager(ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport.Width,
                ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport.Height);
            cameraManager.SetPosition(new Vector3(100.0f, 40.0f, 100.0f));
            cameraManager.SetLookAtPoint(new Vector3(0.0f, 0.0f, 0.0f));

            ScreenManager.Game.Services.AddService(typeof(CameraManager), cameraManager);


#if WINDOWS_PHONE
            thumbsticks = new VirtualThumbsticks();
#endif
            SetupLevel();
            SetupObjects();
            SetupGateways();
            SetupDistractors();
            SetupKeyboard();
        }

        /// <summary>
        /// Reset's content if screen still in memory
        /// </summary>
        public override void Reload()
        {

        }

        /// <summary>
        /// Send content to garbage collection when screen removed from memory
        /// </summary>
        public override void UnloadContent()
        {
            ScreenManager.Game.Services.RemoveService(typeof(CameraManager));
        }
        #endregion

        #region Update
        /// <summary>
        /// Update's the player's input
        /// </summary>
        public override void HandleInput()
        {
#if WINDOWS
            ScreenManager.InputManager.Update();

            if (ScreenManager.InputManager.CheckNewReleaseAction(InputAction.ExitGame))
            {
                ScreenManager.RemoveScreen(this);
            }

            if (ScreenManager.InputManager.CheckAction(InputAction.MoveUp))
                cameraManager.StrafeForward();

            if (ScreenManager.InputManager.CheckAction(InputAction.MoveDown))
                cameraManager.StrafeBackward();

            if (ScreenManager.InputManager.CheckAction(InputAction.MoveLeft))
                cameraManager.StrafeLeft();

            if (ScreenManager.InputManager.CheckAction(InputAction.MoveRight))
                cameraManager.StrafeRight();

            if (ScreenManager.InputManager.CheckAction(InputAction.RotateLeft))
                cameraManager.RotateLeft();

            if (ScreenManager.InputManager.CheckAction(InputAction.RotateRight))
                cameraManager.RotateRight();

            if (ScreenManager.InputManager.CheckAction(InputAction.LookUp))
                cameraManager.RotateUp();

            if (ScreenManager.InputManager.CheckAction(InputAction.LookDown))
                cameraManager.RotateDown();


            //Test for Mouse/Menu Interaction
            //UpdatePicking();
            //Point mouse = new Point((int)ScreenManager.Cursor.Position.X,
            //    (int)ScreenManager.Cursor.Position.Y);

            //Rectangle mouseRect = new Rectangle(mouse.X, mouse.Y, 10, 10);
            //ResetButtonStates();
            //if (uiBackgroundRect.Intersects(mouseRect))
            //{
            //    foreach (IActionable actionableObject in actionableObjects)
            //    {
            //        FindableObject findableObject = actionableObject as FindableObject;
            //        if (findableObject == null)
            //            continue;

            //        findableObject.UIItem.Status = UIItemStatus.Inactive;
            //    }
            //    //Check Items
            //    foreach (IActionable actionableObject in actionableObjects)
            //    {
            //        FindableObject findableObject = actionableObject as FindableObject;
            //        if (findableObject == null)
            //            continue;

            //        if (findableObject.UIItem.Bounds.Intersects(mouseRect))
            //        {

            //            //Check Mouse Click To Pick Right Status
            //            if (ScreenManager.Cursor.IsLeftMouseButtonPressed())
            //            {
            //                findableObject.UIItem.Status = UIItemStatus.Click;

            //                if (findableObject.UIItem.OurAction != null)
            //                    findableObject.UIItem.OurAction();
            //            }
            //            else
            //            {
            //                findableObject.UIItem.Status = UIItemStatus.Hover;
            //            }
            //            break;
            //        }
            //    }
            //}
            //else
            //{
            //    foreach (IActionable actionableObject in actionableObjects)
            //    {
            //        FindableObject findableObject = actionableObject as FindableObject;
            //        if (findableObject == null)
            //            continue;

            //        findableObject.UIItem.Status = UIItemStatus.Inactive;
            //    }

            //    if (ScreenManager.Cursor.isLeftMouseButtonJustReleased() && ScreenManager.Cursor.CurrentModel != null)
            //    {
            //        ScreenManager.Cursor.CurrentModel.Action();
            //        // ScreenManager.ShowScreen(new ItemDetailScreen(ScreenManager.Cursor.CurrentModel));
            //    }
            //}
#endif

#if WINDOWS_PHONE
            thumbsticks.Update();

            if (thumbsticks.leftPosition.X > 0)
            {
                cameraManager.StrafeForward();
            }
            else if (thumbsticks.leftPosition.X < 0)
            {
            //    cameraManager.StrafeBackward();
            }

            if (thumbsticks.leftPosition.Y > 0)
            {
              //  cameraManager.RotateLeft();
            }
            else if (thumbsticks.leftPosition.Y < 0)
            {
                //cameraManager.RotateRight();
            }
        
#endif
        }

        /// <summary>
        /// Update's all game logic
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            cameraManager.Update(collisionModels, gameTime);

            foreach (StaticModel model in actionableObjects)
            {
                model.Update(gameTime);
            }

        }
        #endregion

        #region Draw
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            ResetRenderStates();
            DrawPickableObjects(gameTime);
            DrawGatewayObjects(gameTime);
            DrawLevel(gameTime);

#if WINDOWS_PHONE
            thumbsticks.Draw();
#endif


        }
        #endregion

        #region UI Helpers
        private void SetupUI()
        {
            uiBackground = ScreenManager.Game.Content.Load<Texture2D>(@"UI\background");
            uiObjectsFoundLocation = new Vector2(60, 30);
            //uiObjectsFound = ScreenManager.Game.Content.Load<Texture2D>(@"UI\itemsFound");
            uiObjectsFoundFont = ScreenManager.Game.Content.Load<SpriteFont>(@"UI\objectsFoundFont");
            uiObjectsFoundFontLocation = uiObjectsFoundLocation + new Vector2(54, 3);

        }

        private void ResetButtonStates()
        {
            for (int i = 0; i < uiItems.Count; i++)
            {
                uiItems[i].Status = UIItemStatus.Inactive;
                uiItems[i].Found = true; //FIX THIS - Remove before Done! (not all items are always found)
            }
        }

        private void DrawUI(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(uiBackground, Vector2.Zero, uiBackgroundColor);
            //ScreenManager.SpriteBatch.Draw(uiObjectsFound, uiObjectsFoundLocation, Color.White);
            ScreenManager.SpriteBatch.DrawString(uiObjectsFoundFont, FoundObjects.ToString() + "/" + NumObjects.ToString(),
                uiObjectsFoundFontLocation, Color.White);

            ScreenManager.SpriteBatch.DrawString(uiObjectsFoundFont, cameraManager.Position.ToString(), new Vector2(300, 10), Color.White);

            foreach (IActionable actionableObject in actionableObjects)
            {
                FindableObject findableObject = actionableObject as FindableObject;
                if (findableObject == null)
                    continue;


                findableObject.UIItem.Draw(gameTime);
            }

            //for (int i = 0; i < uiItems.Count; i++)
            //{

            //    uiItems[i].Draw(gameTime);
            //}
            ScreenManager.SpriteBatch.End();
        }
        #endregion

        #region Triangle Picking Helpers

        /// <summary>
        /// Runs a per-triangle picking algorithm over all the models in the scene,
        /// storing which triangle is currently under the cursor.
        /// </summary>
        void UpdatePicking()
        {
            // Look up a collision ray based on the current cursor position. See the
            // Picking Sample documentation for a detailed explanation of this.
            Ray cursorRay = ScreenManager.Cursor.CalculateCursorRay(cameraManager.ProjectionMatrix,
                cameraManager.ViewMatrix);

            // Clear the previous picking results.
            insideBoundingSpheres.Clear();

            // Keep track of the closest object we have seen so far, so we can
            // choose the closest one if there are several models under the cursor.
            float closestIntersection = float.MaxValue;

            intersected = false;

            // Loop over all our models.
            for (int i = 0; i < actionableObjects.Count; i++)
            {
                StaticModel staticModel = actionableObjects[i] as StaticModel;
                bool insideBoundingSphere;
                Vector3 vertex1, vertex2, vertex3;

                // Perform the ray to model intersection test.
                float? intersection = RayIntersectsModel(cursorRay, staticModel.model,
                                                         modelWorldTransforms[i],
                                                         out insideBoundingSphere,
                                                         out vertex1, out vertex2,
                                                         out vertex3);

                // If this model passed the initial bounding sphere test, remember
                // that so we can display it at the top of the screen.
                //if (insideBoundingSphere)
                //    insideBoundingSpheres.Add(ModelFilenames[i]);

                // Do we have a per-triangle intersection with this model?
                if (intersection != null)
                {
                    // If so, is it closer than any other model we might have
                    // previously intersected?
                    if (intersection < closestIntersection)
                    {
                        // Store information about this model.
                        closestIntersection = intersection.Value;
                        ScreenManager.Cursor.CurrentModel = (IActionable)staticModel;
                        intersected = true;
                    }
                }
            }

            if (!intersected)
            {
                ScreenManager.Cursor.CurrentModel = null;
            }
        }


        /// <summary>
        /// Checks whether a ray intersects a model. This method needs to access
        /// the model vertex data, so the model must have been built using the
        /// custom TrianglePickingProcessor provided as part of this sample.
        /// Returns the distance along the ray to the point of intersection, or null
        /// if there is no intersection.
        /// </summary>
        static float? RayIntersectsModel(Ray ray, Model model, Matrix modelTransform,
                                         out bool insideBoundingSphere,
                                         out Vector3 vertex1, out Vector3 vertex2,
                                         out Vector3 vertex3)
        {
            vertex1 = vertex2 = vertex3 = Vector3.Zero;

            // The input ray is in world space, but our model data is stored in object
            // space. We would normally have to transform all the model data by the
            // modelTransform matrix, moving it into world space before we test it
            // against the ray. That transform can be slow if there are a lot of
            // triangles in the model, however, so instead we do the opposite.
            // Transforming our ray by the inverse modelTransform moves it into object
            // space, where we can test it directly against our model data. Since there
            // is only one ray but typically many triangles, doing things this way
            // around can be much faster.

            Matrix inverseTransform = Matrix.Invert(modelTransform);

            ray.Position = Vector3.Transform(ray.Position, inverseTransform);
            ray.Direction = Vector3.TransformNormal(ray.Direction, inverseTransform);

            // Look up our custom collision data from the Tag property of the model.
            Dictionary<string, object> tagData = (Dictionary<string, object>)model.Tag;

            if (tagData == null)
            {
                throw new InvalidOperationException(
                    "Model.Tag is not set correctly. Make sure your model " +
                    "was built using the custom TrianglePickingProcessor.");
            }

            // Start off with a fast bounding sphere test.
            BoundingSphere boundingSphere = (BoundingSphere)tagData["BoundingSphere"];

            if (boundingSphere.Intersects(ray) == null)
            {
                // If the ray does not intersect the bounding sphere, we cannot
                // possibly have picked this model, so there is no need to even
                // bother looking at the individual triangle data.
                insideBoundingSphere = false;

                return null;
            }
            else
            {
                // The bounding sphere test passed, so we need to do a full
                // triangle picking test.
                insideBoundingSphere = true;

                // Keep track of the closest triangle we found so far,
                // so we can always return the closest one.
                float? closestIntersection = null;

                // Loop over the vertex data, 3 at a time (3 vertices = 1 triangle).
                Vector3[] vertices = (Vector3[])tagData["Vertices"];

                for (int i = 0; i < vertices.Length; i += 3)
                {
                    // Perform a ray to triangle intersection test.
                    float? intersection;

                    RayIntersectsTriangle(ref ray,
                                          ref vertices[i],
                                          ref vertices[i + 1],
                                          ref vertices[i + 2],
                                          out intersection);

                    // Does the ray intersect this triangle?
                    if (intersection != null)
                    {
                        // If so, is it closer than any other previous triangle?
                        if ((closestIntersection == null) ||
                            (intersection < closestIntersection))
                        {
                            // Store the distance to this triangle.
                            closestIntersection = intersection;

                            // Transform the three vertex positions into world space,
                            // and store them into the output vertex parameters.
                            Vector3.Transform(ref vertices[i],
                                              ref modelTransform, out vertex1);

                            Vector3.Transform(ref vertices[i + 1],
                                              ref modelTransform, out vertex2);

                            Vector3.Transform(ref vertices[i + 2],
                                              ref modelTransform, out vertex3);
                        }
                    }
                }

                return closestIntersection;
            }
        }


        /// <summary>
        /// Checks whether a ray intersects a triangle. This uses the algorithm
        /// developed by Tomas Moller and Ben Trumbore, which was published in the
        /// Journal of Graphics Tools, volume 2, "Fast, Minimum Storage Ray-Triangle
        /// Intersection".
        /// 
        /// This method is implemented using the pass-by-reference versions of the
        /// XNA math functions. Using these overloads is generally not recommended,
        /// because they make the code less readable than the normal pass-by-value
        /// versions. This method can be called very frequently in a tight inner loop,
        /// however, so in this particular case the performance benefits from passing
        /// everything by reference outweigh the loss of readability.
        /// </summary>
        static void RayIntersectsTriangle(ref Ray ray,
                                          ref Vector3 vertex1,
                                          ref Vector3 vertex2,
                                          ref Vector3 vertex3, out float? result)
        {
            // Compute vectors along two edges of the triangle.
            Vector3 edge1, edge2;

            Vector3.Subtract(ref vertex2, ref vertex1, out edge1);
            Vector3.Subtract(ref vertex3, ref vertex1, out edge2);

            // Compute the determinant.
            Vector3 directionCrossEdge2;
            Vector3.Cross(ref ray.Direction, ref edge2, out directionCrossEdge2);

            float determinant;
            Vector3.Dot(ref edge1, ref directionCrossEdge2, out determinant);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -float.Epsilon && determinant < float.Epsilon)
            {
                result = null;
                return;
            }

            float inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            Vector3 distanceVector;
            Vector3.Subtract(ref ray.Position, ref vertex1, out distanceVector);

            float triangleU;
            Vector3.Dot(ref distanceVector, ref directionCrossEdge2, out triangleU);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleU < 0 || triangleU > 1)
            {
                result = null;
                return;
            }

            // Calculate the V parameter of the intersection point.
            Vector3 distanceCrossEdge1;
            Vector3.Cross(ref distanceVector, ref edge1, out distanceCrossEdge1);

            float triangleV;
            Vector3.Dot(ref ray.Direction, ref distanceCrossEdge1, out triangleV);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleV < 0 || triangleU + triangleV > 1)
            {
                result = null;
                return;
            }

            // Compute the distance along the ray to the triangle.
            float rayDistance;
            Vector3.Dot(ref edge2, ref distanceCrossEdge1, out rayDistance);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
            {
                result = null;
                return;
            }

            result = rayDistance;
        }

        #endregion Triangle Picking Helpers

        #region Setup
        private void SetupLevel()
        {
            stage = new StaticModel(ScreenManager.Game.Content.Load<Model>(@"Levels\uhEnginePlayground"), Vector3.Zero);
            stage.Scale = 100;

            collideStage = new StaticModel(ScreenManager.Game.Content.Load<Model>(@"Levels\uhEnginePlayground"), Vector3.Zero);

            levelCollision = new CollisionMesh(stage.model, 1);
            collisionModels.Add(collideStage);
        }

        private void SetupObjects()
        {
            List<string> tempItemList = new List<string>();
            List<Vector3> tempPosition = new List<Vector3>();
            tempItemList.Add(@"PickableObjects\barrel");
            tempPosition.Add(new Vector3(100, 0, 100));
            tempItemList.Add(@"PickableObjects\bike");
            tempPosition.Add(new Vector3(300, 0, 100));
            tempItemList.Add(@"PickableObjects\chair");
            tempPosition.Add(new Vector3(10, 0, 100));
            tempItemList.Add(@"PickableObjects\drum");
            tempPosition.Add(new Vector3(400, 0, 100));
            tempItemList.Add(@"PickableObjects\glasses");
            tempPosition.Add(new Vector3(100, 0, 500));
            tempItemList.Add(@"PickableObjects\horse");
            tempPosition.Add(new Vector3(100, 0, 200));
            tempItemList.Add(@"PickableObjects\key");
            tempPosition.Add(new Vector3(100, 0, 1000));
            tempItemList.Add(@"PickableObjects\leaf");
            tempPosition.Add(new Vector3(10, 0, 10));
            tempItemList.Add(@"PickableObjects\suitcase");
            tempPosition.Add(new Vector3(160, 0, 160));
            tempItemList.Add(@"PickableObjects\airplane");
            tempPosition.Add(new Vector3(190, 0, 100));
            tempItemList.Add(@"PickableObjects\table");
            tempPosition.Add(new Vector3(150, 0, 100));
            tempItemList.Add(@"PickableObjects\tank");
            tempPosition.Add(new Vector3(100, 0, 250));
            tempItemList.Add(@"PickableObjects\wheel");
            tempPosition.Add(new Vector3(200, 0, 400));

            Vector2 currentPosition = baseUIIconPosition;

            int counter = 1;
            for(int i =0; i< tempItemList.Count; i++)
            {
                string item = tempItemList[i];
                FindableObject model = new FindableObject(ScreenManager.Game.Content.Load<Model>(item), tempPosition[i]);
                model.UIItem = new UIItem(model.Image, currentPosition);
                model.Action = () => ScreenManager.ShowScreen(new ItemDetailScreen(model));
                collisionModels.Add(model);


                actionableObjects.Add(model);
                modelWorldTransforms.Add(model.transforms);
                insideBoundingSpheres.Add(model.model.Meshes[model.model.Root.Index].BoundingSphere);

                if (counter % 3 == 0)
                {
                    counter = 1;
                    currentPosition.X = baseUIIconPosition.X;
                    currentPosition.Y += rowOffset.Y;
                }
                else
                {
                    counter++;
                    currentPosition += columnOffset;
                }
            }
        }

        private void SetupDistractors()
        {
            DistractorObject distractorObject = new DistractorObject(ScreenManager.Game.Content.Load<Model>(@"PickableObjects\wheel"), new Vector3(200, 30, 500));
            distractorObject.Action = () => distractorObject.Position = new Vector3(distractorObject.Position.X, 30, distractorObject.Position.Z);

            actionableObjects.Add(distractorObject);
            collisionModels.Add(distractorObject);
            modelWorldTransforms.Add(distractorObject.transforms);
            insideBoundingSpheres.Add(distractorObject.model.Meshes[distractorObject.model.Root.Index].BoundingSphere);
        }

        private void SetupGateways()
        {
            GatewayObject gatewayObject = new GatewayObject(
                ScreenManager.Game.Content.Load<Model>(@"PickableObjects\barrel"),
                new Vector3(200, 0, 100));

            gatewayObject.Keys.Add((FindableObject)actionableObjects.Where(o =>
            {
                FindableObject fo = o as FindableObject;
                if (fo == null)
                    return false;

                return fo.Name == "table";
            }).First());

            gateways.Add(gatewayObject);
            collisionModels.Add(gatewayObject);
        }

        private void SetupKeyboard()
        {
            ScreenManager.InputManager.AddInput(InputAction.MoveLeft, Microsoft.Xna.Framework.Input.Keys.A);
            ScreenManager.InputManager.AddInput(InputAction.MoveRight, Microsoft.Xna.Framework.Input.Keys.D);
            ScreenManager.InputManager.AddInput(InputAction.MoveUp, Microsoft.Xna.Framework.Input.Keys.W);
            ScreenManager.InputManager.AddInput(InputAction.MoveDown, Microsoft.Xna.Framework.Input.Keys.S);
            ScreenManager.InputManager.AddInput(InputAction.RotateLeft, Microsoft.Xna.Framework.Input.Keys.Left);
            ScreenManager.InputManager.AddInput(InputAction.RotateRight, Microsoft.Xna.Framework.Input.Keys.Right);
            ScreenManager.InputManager.AddInput(InputAction.LookUp, Microsoft.Xna.Framework.Input.Keys.Up);
            ScreenManager.InputManager.AddInput(InputAction.LookDown, Microsoft.Xna.Framework.Input.Keys.Down);
        }
        #endregion

        #region DrawHelpers
        private void DrawPickableObjects(GameTime gameTime)
        {
            foreach (StaticModel model in actionableObjects)
            {
                model.Draw(gameTime);
            }
        }

        private void DrawGatewayObjects(GameTime gameTime)
        {
            foreach (GatewayObject gateway in gateways)
            {
                if (gateway.IsActive)
                    gateway.Draw(gameTime);
            }
        }

        private void DrawLevel(GameTime gameTime)
        {
            stage.Draw(gameTime);
        }
        #endregion

    }
}

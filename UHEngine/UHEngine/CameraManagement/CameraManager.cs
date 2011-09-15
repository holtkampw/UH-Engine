#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BoxCollider;
using UHEngine.CoreObjects;

#endregion

namespace UHEngine.CameraManagement
{
    public class CameraManager : CollisionTreeElemDynamic
    {
        #region Class Variables
        float aspectRatio;
        public Matrix ViewMatrix;
        public Matrix ProjectionMatrix;
        Matrix rotationMatrix;
        private Vector3 position;
        Vector3 LookAtPoint;
        float rotationLeftRight;
        float rotationUpDown;
        Vector3 cameraReference = new Vector3(0, 0, 10);
        Vector3 newPosition;
        public BoundingSphere Sphere;
        Matrix forwardMovement;
        int ForwardMovementAmount = -1;
        int BackwardMovementAmount = 1;
        int LeftMovementAmount = 1;
        int RightMovementAmount = -1;
        float RotationAmount = 0.04f;
        Vector3 tempV;
        bool cameraMoved = true;

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                newPosition = value;
                cameraMoved = true;
            }
        }
        #endregion

        #region Initialization
        public CameraManager(int width, int height)
        {
            //tmp position
            Position = Vector3.Zero;
            newPosition = Vector3.Zero;

            //tmp lookAtPoint
            LookAtPoint = Vector3.Zero;

            //set aspect ratio
            aspectRatio = (float)width / (float)height;

            //setup projection
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                            MathHelper.ToRadians(40.0f),//40
                            this.aspectRatio,
                            1.0f,
                            10000.0f);

            SetViewMatrix();
            rotationLeftRight = 0.0f;
            rotationUpDown = 0.0f;

            box = new CollisionBox(new Vector3(-15, -100, -15), new Vector3(15, 30, 15));
        }
        #endregion

        public void SetViewMatrix()
        {
            ViewMatrix = Matrix.CreateLookAt(Position, LookAtPoint, Vector3.Up);
        }

        #region Manipulation
        public void StrafeLeft()
        {
            forwardMovement = Matrix.CreateRotationY(rotationLeftRight);
            tempV = new Vector3(LeftMovementAmount, 0, 0);
            tempV = Vector3.Transform(tempV, forwardMovement);
            newPosition = Position;
            newPosition.Z += tempV.Z;
            newPosition.X += tempV.X;
            Sphere.Center = newPosition;
            cameraMoved = true;
        }

        public void StrafeRight()
        {
            forwardMovement = Matrix.CreateRotationY(rotationLeftRight);
            tempV = new Vector3(RightMovementAmount, 0, 0);
            tempV = Vector3.Transform(tempV, forwardMovement);
            newPosition = Position;
            newPosition.Z += tempV.Z;
            newPosition.X += tempV.X;
            Sphere.Center = newPosition;
            cameraMoved = true;
        }

        public void StrafeForward()
        {
            forwardMovement = Matrix.CreateRotationY(rotationLeftRight);
            tempV = new Vector3(0, 0, ForwardMovementAmount);
            tempV = Vector3.Transform(tempV, forwardMovement);
            newPosition = Position;
            newPosition.Z += tempV.Z;
            newPosition.X += tempV.X;
            Sphere.Center = newPosition;
            cameraMoved = true;
        }

        public void StrafeBackward()
        {
            forwardMovement = Matrix.CreateRotationY(rotationLeftRight);
            tempV = new Vector3(0, 0, BackwardMovementAmount);
            tempV = Vector3.Transform(tempV, forwardMovement);
            newPosition = Position;
            newPosition.Z += tempV.Z;
            newPosition.X += tempV.X;
            Sphere.Center = newPosition;
            cameraMoved = true;
        }

        public void RotateLeft()
        {
            rotationLeftRight += RotationAmount;
            cameraMoved = true;
        }

        public void RotateRight()
        {
            rotationLeftRight -= RotationAmount;
            cameraMoved = true;
        }

        public void RotateUp()
        {
            rotationUpDown += RotationAmount;
            rotationUpDown = MathHelper.Clamp(rotationUpDown, -0.5f, 0.5f);
            cameraMoved = true;
        }

        public void RotateDown()
        {
            rotationUpDown += RotationAmount;
            rotationUpDown = MathHelper.Clamp(rotationUpDown, -0.5f, 0.5f);
            cameraMoved = true;
        }

        public void SetPosition(Vector3 position)
        {
            this.Position = position;
            cameraMoved = true;
        }

        public void SetLookAtPoint(Vector3 lookAtPoint)
        {
            this.LookAtPoint = lookAtPoint;
            cameraMoved = true;
        }
        #endregion

        #region Update
        public void Update(List<StaticModel> collisionModels, GameTime gameTime)
        {
            Vector3 collideNewPos;

            if (cameraMoved)
            {
                cameraMoved = false;
                for (int i = 0; i < collisionModels.Count; i++)
                {
                    GatewayObject gateway = collisionModels[i] as GatewayObject;

                    if (gateway != null && !gateway.IsActive)
                        continue;

                    if (collisionModels[i].CollisionMesh.BoxMove(box, Position,
                            newPosition,
                            1.0f, 0.0f, 3, out collideNewPos))
                    {
                        newPosition = collideNewPos;
                        // i = 0;
                    }
                }

                Position = newPosition;
                rotationMatrix = Matrix.CreateRotationX(rotationUpDown) * Matrix.CreateRotationY(rotationLeftRight);

                //For rotating camera position around look at point
                Vector3 cameraRotatedPosition = Vector3.Transform(cameraReference, rotationMatrix);
                //Vector3 cameraRotatedUpVector = Vector3.Transform(Vector3.Up, rotationMatrix);
                LookAtPoint = Position + cameraRotatedPosition;
                ViewMatrix = Matrix.CreateLookAt(Position, LookAtPoint, Vector3.Up);
            }

        }
        #endregion
    }
}

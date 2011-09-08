#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BoxCollider;
using GermanGame.CoreObjects;

#endregion

namespace GermanGame.CameraManagement
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

        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                newPosition = value;
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
            int amount = 1;
            //Position = new Vector3(Position.X + amount, Position.Y, Position.Z);
            //LookAtPoint = new Vector3(LookAtPoint.X + amount, LookAtPoint.Y, LookAtPoint.Z);

            Matrix forwardMovement = Matrix.CreateRotationY(rotationLeftRight);
            Vector3 v = new Vector3(amount, 0, 0);
            v = Vector3.Transform(v, forwardMovement);
            newPosition = Position;
            newPosition.Z += v.Z;
            newPosition.X += v.X;
            Sphere.Center = newPosition;
        }

        public void StrafeRight()
        {
            int amount = -1;

            Matrix forwardMovement = Matrix.CreateRotationY(rotationLeftRight);
            Vector3 v = new Vector3(amount, 0, 0);
            v = Vector3.Transform(v, forwardMovement);
            newPosition = Position;
            newPosition.Z += v.Z;
            newPosition.X += v.X;
            Sphere.Center = newPosition;

            //Position = new Vector3(Position.X + amount, Position.Y, Position.Z);
            //LookAtPoint = new Vector3(LookAtPoint.X + amount, LookAtPoint.Y, LookAtPoint.Z);
        }

        public void StrafeY(float amount)
        {
            //Position = new Vector3(Position.X, Position.Y + amount, Position.Z);
            //LookAtPoint = new Vector3(LookAtPoint.X, LookAtPoint.Y + amount, LookAtPoint.Z);
        }

        public void StrafeForward()
        {
            int amount = 1;

            Matrix forwardMovement = Matrix.CreateRotationY(rotationLeftRight);
            Vector3 v = new Vector3(0, 0, amount);
            v = Vector3.Transform(v, forwardMovement);
            newPosition = Position;
            newPosition.Z += v.Z;
            newPosition.X += v.X;
            Sphere.Center = newPosition;
            //Position = new Vector3(Position.X, Position.Y, Position.Z + amount);
            //LookAtPoint = new Vector3(LookAtPoint.X, LookAtPoint.Y, LookAtPoint.Z + amount);
        }

        public void StrafeBackward()
        {
            int amount = -1;

            Matrix forwardMovement = Matrix.CreateRotationY(rotationLeftRight);
            Vector3 v = new Vector3(0, 0, amount);
            v = Vector3.Transform(v, forwardMovement);
            newPosition = Position;
            newPosition.Z += v.Z;
            newPosition.X += v.X;
            Sphere.Center = newPosition;

            //Position = new Vector3(Position.X, Position.Y, Position.Z + amount);
            //LookAtPoint = new Vector3(LookAtPoint.X, LookAtPoint.Y, LookAtPoint.Z + amount);
        }

        public void RotateLeft()
        {
            float amount = 0.04f;
            rotationLeftRight += amount;
            //position = new Vector3(position.X + amount, position.Y, position.Z);
        }

        public void RotateRight()
        {
            float amount = -0.04f;
            rotationLeftRight += amount;
            //position = new Vector3(position.X + amount, position.Y, position.Z);
        }

        public void RotateY(float amount)
        {
            rotationUpDown -= amount;
            //position = new Vector3(position.X, position.Y + amount, position.Z);
        }

        public void RotateUp()
        {
            float amount = -0.04f;
            rotationUpDown += amount;
            rotationUpDown = MathHelper.Clamp(rotationUpDown, -0.5f, 0.5f);
        }

        public void RotateDown()
        {
            float amount = 0.04f;
            rotationUpDown += amount;
            rotationUpDown = MathHelper.Clamp(rotationUpDown, -0.5f, 0.5f);
        }

        public void SetPosition(Vector3 position)
        {
            this.Position = position;
        }

        public void SetLookAtPoint(Vector3 lookAtPoint)
        {
            this.LookAtPoint = lookAtPoint;
        }
        #endregion

        #region Update
        public void Update(List<StaticModel> collisionModels, GameTime gameTime)
        {
            Vector3 collideNewPos;

            for (int i = 0; i < collisionModels.Count; i++)
            {
                GatewayObject gateway = collisionModels[i] as GatewayObject;

                if (gateway != null && !gateway.IsActive)
                    continue;

                if ( collisionModels[i].CollisionMesh.BoxMove(box, Position,//new Vector3(Position.X, 0, Position.Z),
                    //new Vector3(newPosition.X, 0, newPosition.Z),
                        newPosition,
                        1.0f, 0.0f, 3, out collideNewPos))
                {
                    newPosition = collideNewPos;
                   // i = 0;
                }
            }

            //box.BoxIntersect(
            Position = newPosition;
            rotationMatrix = Matrix.CreateRotationX(rotationUpDown) * Matrix.CreateRotationY(rotationLeftRight);

            //For rotating look at point around camera position
            //Vector3 cameraRotatedTarget = Vector3.Transform(new Vector3(0,0,-1), rotationMatrix);
            //Vector3 cameraFinalTarget = position + cameraRotatedTarget;
            //Vector3 cameraRotatedUpVector = Vector3.Transform(Vector3.Up, rotationMatrix);
            //viewMatrix = Matrix.CreateLookAt(position, cameraFinalTarget, cameraRotatedUpVector);

            //For rotating camera position around look at point
            Vector3 cameraRotatedPosition = Vector3.Transform(cameraReference, rotationMatrix);
            //Vector3 cameraRotatedUpVector = Vector3.Transform(Vector3.Up, rotationMatrix);
            LookAtPoint = Position + cameraRotatedPosition;
            ViewMatrix = Matrix.CreateLookAt(Position, LookAtPoint, Vector3.Up);

            //standard without rotation
            //viewMatrix = Matrix.CreateLookAt(position, lookAtPoint, Vector3.Up);
        }
        #endregion
    }
}

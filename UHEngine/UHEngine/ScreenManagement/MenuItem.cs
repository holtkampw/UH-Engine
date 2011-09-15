using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UHEngine.ScreenManagement
{
    class MenuItem
    {
        public string Name { get; set; }
        public Action Action { get; set; }
        public bool Selected { get; set; }
        public Rectangle Area { get; set; }
        public Vector2 Position { get; set; }

        public MenuItem(string name, Action action, Vector2 position)
        {
            this.Name = name;
            this.Action += action;
            this.Selected = false;
            this.Position = position;
            this.Area = new Rectangle((int)position.X, (int)position.Y, 200, 200);
        }
    }
}

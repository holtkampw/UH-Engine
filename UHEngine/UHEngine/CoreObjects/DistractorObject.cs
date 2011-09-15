using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UHEngine.CoreObjects
{
    class DistractorObject : StaticModel, IActionable
    {
        public Action Action { get; set; }

        public DistractorObject(Model model, Vector3 position)
            : base(model, position) { }
    }
}

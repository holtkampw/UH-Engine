using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UHEngine.CoreObjects;

namespace UHEngine.CoreObjects
{
    class GatewayObject : StaticModel
    {
        public List<FindableObject> Keys { get; set; }

        public bool IsActive
        {
            get
            {
                return Keys.Count > Keys.Count(fo => fo.IsFound);
            }
        }

        public GatewayObject(Model model, Vector3 position)
            :base(model, position)
        {
            Keys = new List<FindableObject>();
        }
    }
}

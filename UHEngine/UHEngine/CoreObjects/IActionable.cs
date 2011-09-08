using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GermanGame.CoreObjects
{
    public interface IActionable
    {
        Action Action { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UHEngine.CoreObjects
{
    public interface IActionable
    {
        Action Action { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace EnvironmentalApp.Data
{
    class PinCreation
    {
     public Pin CreatePin(MapClickedEventArgs e, String name, string quality)
        { 
            var p = new Pin()
            {
                Location = e.Location,
                Label = name,
                Address = "Air Quality: High, Water Quality: Normal",
                Type = PinType.Place
            };
            return p;
        }
    }
}

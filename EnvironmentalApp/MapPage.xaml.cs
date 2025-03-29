namespace EnvironmentalApp;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Collections.ObjectModel;
using System;

public partial class MapPage : ContentPage
{
    void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        System.Diagnostics.Debug.WriteLine($"MapClick: {e.Location.Latitude}, {e.Location.Longitude}");

        var p = new Pin()
        {
            Location = e.Location,
            Label = "A pin",
            Address = "A place"
        };
        myMap.Pins.Add(p);
    }

    public MapPage()
    {
        InitializeComponent();

        var p = new Pin()
        {
            Location = new Location(55.9533, -3.1883), // Edinburgh, Scotland
            Label = "Sensor 123",
            Address = "Air Quality: High, Water Quality: Normal",
            Type = PinType.Place
        };

        myMap.Pins.Add(p);
    }
    private void Button_Clicked(object sender, EventArgs e)
    {
        var p = new Pin()
        {
            Location = new Location(40, 2),
            Label = "Test",
            Address = "Test2"
        };

        myMap.Pins.Add(p);

        myMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(40, 2), Distance.FromKilometers(10)));
    }
}
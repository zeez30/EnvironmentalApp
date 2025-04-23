namespace EnvironmentalApp;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Collections.ObjectModel;
using System;
using EnvironmentalApp.Data;

public partial class MapPage : ContentPage
{
    public Microsoft.Maui.Controls.Maps.Map MyMap => myMap;
    /// <summary>
    /// Create a Pin Upon Map left Click
    /// </summary>
    /// <param name="sender">The sender object</param>
    /// <param name="e">Map click event args</param>
    public void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        //System.Diagnostics.Debug.WriteLine($"MapClick: {e.Location.Latitude}, {e.Location.Longitude}");
        var pinCreation = new PinCreation();
        var p = pinCreation.CreatePin(e, "Sensor 123", "Air Quality: High, Water Quality: Normal");
        myMap.Pins.Add(p);
    }

    /// <summary>
    /// Constructor for MapPage testing
    /// </summary>
    /// <param name="mockMap"></param>
    public MapPage(Microsoft.Maui.Controls.Maps.Map mockMap)
    {
        InitializeComponent();
        myMap = mockMap;
    }

    /// <summary>
    /// Constructor for default MapPage, creates a inital pin for testing purposes.
    /// </summary>
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
    /// <summary>
    /// Button click event to add a pin to the map at a predefined location.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">Map click event args.</param>
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
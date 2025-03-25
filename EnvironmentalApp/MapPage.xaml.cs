namespace EnvironmentalApp;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

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
    }


    private void Button_Clicked(object sender, EventArgs e)
    {
        var p = new Pin()
        {
            Location = new Location(40, 2),
            Label = "Test",
            Address = "Test2"
        };

        p.MarkerClicked += P_MarkerClicked;

        myMap.Pins.Add(p);

        myMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(40, 2), Distance.FromKilometers(10)));
    }

    private void P_MarkerClicked(object sender, PinClickedEventArgs e)
    {
        DisplayAlert("Test1", "Test2", "Test3");
    }
}
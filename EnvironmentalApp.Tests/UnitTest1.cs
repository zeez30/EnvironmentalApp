using Xunit;
using Moq;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using EnvironmentalApp;

namespace EnvironmentalApp.Tests
{
    public class MapPageTests
    {
        [Fact]
        public void OnMapClicked_ShouldAddPinToMap()
        {
            // Arrange
            var mockMap = new Mock<Microsoft.Maui.Controls.Maps.Map>();  // Mock the actual Map
            var mapPage = new MapPage(mockMap.Object);
            // Inject the mocked map into the MapPage

            var e = new MapClickedEventArgs(new Location(55.9533, -3.1883)); // Simulate a map click

            // Act
            mapPage.OnMapClicked(null, e);  // Call the public method

            // Assert
            mockMap.Verify(m => m.Pins.Add(It.Is<Pin>(p => p.Label == "A pin" && p.Address == "A place")), Times.Once);
        }
    }
}

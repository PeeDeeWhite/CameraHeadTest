namespace Vitec.CameraHead.UnitTests {
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Vitec.CameraHead.Models;

    [TestClass]
    public class FHR35Tests {

        [TestMethod]
        public void WhenPositionSet_ShouldRaiseEvents() {
            var fhr35 = new FHR35("Test");

            using (var monitoredSubject = fhr35.Monitor()) {
                fhr35.SetPosition(new Position(10.0, 10.0));
                monitoredSubject.Should().Raise(nameof(FHR35.OnStatusChanged));
                monitoredSubject.Should().Raise(nameof(FHR35.OnPositionChanged));
            }
        }
    }
}
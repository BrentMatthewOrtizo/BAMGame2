using Game399.Shared.Logic;
using Xunit;

namespace Game399.Tests
{
    public class FarmLogicTests
    {
        [Fact]
        public void Can_Plant_When_Area_Is_Empty()
        {
            var farm = new FarmLogic(minPlantDistance: 0.5f);

            bool planted = farm.TryPlant(0f, 0f);

            Assert.True(planted);
            Assert.Single(farm.PlantedPositions);
        }

        [Fact]
        public void Cannot_Plant_Too_Close_To_Existing_Crop()
        {
            var farm = new FarmLogic(0.5f);
            farm.TryPlant(0f, 0f);

            bool result = farm.TryPlant(0.2f, 0.1f);

            Assert.False(result);
            Assert.Single(farm.PlantedPositions);
        }

        [Fact]
        public void Can_Plant_When_Distance_Is_Sufficient()
        {
            var farm = new FarmLogic(0.5f);
            farm.TryPlant(0f, 0f);

            bool result = farm.TryPlant(1f, 0f);

            Assert.True(result);
            Assert.Equal(2, farm.PlantedPositions.Count);
        }

        [Fact]
        public void Removing_Crop_Frees_Up_Spot()
        {
            var farm = new FarmLogic(0.5f);
            farm.TryPlant(0f, 0f);

            bool removed = farm.Remove(0f, 0f);
            bool canReplant = farm.TryPlant(0f, 0f);

            Assert.True(removed);
            Assert.True(canReplant);
            Assert.Single(farm.PlantedPositions);
        }

        [Fact]
        public void Throws_When_MinDistance_Is_Invalid()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new FarmLogic(0f));
        }
    }
}
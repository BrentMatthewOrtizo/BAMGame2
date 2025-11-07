using System;
using System.Collections.Generic;

namespace Game399.Shared.Logic
{
    /// <summary>
    /// Represents logic for determining valid crop placements
    /// and maintaining a list of planted crop positions.
    /// </summary>
    public class FarmLogic
    {
        public readonly List<(float x, float y)> PlantedPositions = new();
        public float MinPlantDistance { get; }

        public FarmLogic(float minPlantDistance)
        {
            if (minPlantDistance <= 0)
                throw new ArgumentOutOfRangeException(nameof(minPlantDistance), "Minimum distance must be positive.");

            MinPlantDistance = minPlantDistance;
        }

        /// <summary>
        /// Attempts to plant a crop at the given coordinates.
        /// Returns true if the planting is valid, false otherwise.
        /// </summary>
        public bool TryPlant(float x, float y)
        {
            var newPos = (x, y);

            // Check overlap with existing crops
            foreach (var existing in PlantedPositions)
            {
                if (Distance(existing, newPos) < MinPlantDistance)
                    return false;
            }

            PlantedPositions.Add(newPos);
            return true;
        }

        /// <summary>
        /// Removes a crop from the given position (within small tolerance).
        /// </summary>
        public bool Remove(float x, float y)
        {
            int removed = PlantedPositions.RemoveAll(p => Distance(p, (x, y)) < 0.01f);
            return removed > 0;
        }

        private static float Distance((float x, float y) a, (float x, float y) b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
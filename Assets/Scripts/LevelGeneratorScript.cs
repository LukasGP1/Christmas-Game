using UnityEngine;

public class LevelGeneratorScript : MonoBehaviour
{
    [SerializeField] public GameObject floor;
    [SerializeField] public GameObject fartLoadingZone;
    [SerializeField] public GameObject levelEnd;
    [SerializeField] public GameObject deathPlane;
    [SerializeField] public GameObject fire;
    [SerializeField] public GameObject water;

    public bool RandomChance(System.Random random, float chance)
    {
        // Clamp the chance between 0 and 100
        chance = Mathf.Clamp(chance, 0f, 100f);

        // Generate random a float between 0 and 100
        float randomValue = (float)random.NextDouble() * 100f;

        // Return whether the generated value is greater than the chance
        return randomValue < chance;
    }

    public void GenerateLevel(int levelIndex, int seed, GameObject superParent)
    {
        // Make gameobject to contain all objects of the level
        Transform parent = new GameObject("Level" + levelIndex).transform;
        parent.SetParent(superParent.transform);

        // Create System.Random Object fo RNG
        System.Random random = new System.Random(seed + levelIndex);

        // Generate the number of level tiles randomly
        int tileCount = random.Next(3 * levelIndex + 1, 5 * levelIndex + 3);

        // Get the rect of the start tile and copy it to an extra variable
        Rect rect = new Rect(new Vector2(0, -1000 * levelIndex - 0.5f), 3, 20);
        Rect startRect = new Rect(rect.GetCenter(), rect.GetDimensions());
        
        // Get rect of the fart loading zone by copying the floor rect, shifting it up and make it less wide
        Rect fartRect = new Rect(rect.GetCenter(), rect.GetDimensions());
        fartRect.ShiftY(0.1f);
        fartRect.SetDimensionX(fartRect.GetDimensions().x - 0.1f);

        // Loop to place all tiles (start and end tile are not included in tile count)
        for (int i = 0; i < tileCount + 2; i++)
        {
            // Instantiate the floor
            Transform instantiatedFloor = Instantiate(floor, rect.GetCenter(), new Quaternion(), parent).transform;
            instantiatedFloor.localScale = rect.GetDimensions();

            // Instantiate Fart loading zone
            Transform instantiatedFartLoadingZone = Instantiate(fartLoadingZone, fartRect.GetCenter(), new Quaternion(), parent).transform;
            instantiatedFartLoadingZone.localScale = fartRect.GetDimensions();

            // If it's not the starting tile, there is a 20% chance to place a fire on the tile
            if (RandomChance(random, 20) && i != 0 && i != tileCount + 1) Instantiate(fire, new Vector3(rect.GetCenter().x, rect.GetTopY(), 0), new Quaternion(), parent);

            // Instantiate the last tile (you need to adjust the height because of its different shape)
            if (i == tileCount + 1) Instantiate(levelEnd, new Vector3(rect.GetCenter().x, rect.GetTopY() + 2.5f, 0), new Quaternion(), parent);

            if (i != tileCount + 1)
            {
                // Shift the Rect to the right so its center is directly next to the previous tile
                rect.ShiftX(rect.GetDimensions().x / 2);

                // Randomize the width
                rect.SetDimensionX(random.Next(2, 20));

                // Shift the Rect to the right so its left edge is directly next to the previous tile
                rect.ShiftX(rect.GetDimensions().x / 2);

                // Shift the Rect a random amount to the right
                rect.ShiftX(random.Next(5, 10));

                // Shift the Rect a random amount up or down (clamping is already in the function in the Rect class)
                rect.ShiftTopEdgeY(random.Next(-5, 5));

                // Get rect of the fart loading zone by copying the floor rect, shifting it up and make it less wide
                fartRect = new Rect(rect.GetCenter(), rect.GetDimensions());
                fartRect.ShiftY(0.1f);
                fartRect.SetDimensionX(fartRect.GetDimensions().x - 0.1f);
            }
        }

        // Create the rect used for the water and the bottom death plane, which is 5 wider than the whole level
        Rect bottomRect = new Rect(startRect.GetLeftX() - 5, rect.GetRightX() + 5, startRect.GetTopY() - 3, 6);

        // Instantiate the water
        Transform instantiatedWater = Instantiate(water, bottomRect.GetCenter(), new Quaternion(), parent).transform;
        instantiatedWater.localScale = bottomRect.GetDimensions();

        // Shift bottomRect down and adjust its height to make the bottom death plane
        bottomRect.ShiftY(-1);
        bottomRect.ShiftTopEdgeY(-5);

        // Instantiate the bottom death plane
        Transform instantiatedDeathPlane = Instantiate(deathPlane, bottomRect.GetCenter(), new Quaternion(), parent).transform;
        instantiatedDeathPlane.localScale = bottomRect.GetDimensions();
    }
}
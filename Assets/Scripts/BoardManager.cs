using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Range
    {
        public int min;
        public int max;

        public Range(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public int InclusiveRandom()
        {
            return Random.Range(min, max + 1);
        }
    }

    public int rows = 8;
    public int columns = 8;
    public Range wallRange = new Range(5, 9);
    public Range foodRange = new Range(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;
    private readonly List<Vector3> gridPositions = new List<Vector3>();

    void InitializeList()
    {
        gridPositions.Clear();
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate;
                if (x == -1 || y == -1 || x == columns || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                else
                {
                    toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                }
                GameObject instance =
                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, Range range)
    {
        int count = range.InclusiveRandom();
        for (int i = 0; i < count; i++)
        {
            GameObject tile = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tile, RandomPosition(), Quaternion.identity);
//            tile.transform.SetParent(boardHolder);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallRange);
        LayoutObjectAtRandom(foodTiles, foodRange);
        int enemyCount = (int) Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, new Range(enemyCount, enemyCount));
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
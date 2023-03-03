using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameSRC : MonoBehaviour
{
    // Start is called before the first frame update
    public Texture2D[] backgroundImages;

    public string imageDirectoryPath;
    public TextAsset mapJson;
    public GameObject terrainPrefab;
    public GameObject spacePrefab;
    public GameObject mobPrefab;

    private Map map;
    void Start()
    {
        string jsonString = mapJson.text;

        // Deserialize the JSON data into a Map object
        map = JsonUtility.FromJson<Map>(jsonString);

        // Instantiate the terrain and space prefabs to create the map
        for (int x = 0; x < map.column; x++)
        {
            for (int y = 0; y < map.row; y++)
            {
                int tileIndex = y * map.column + x;
                bool isCollision = (map.data[tileIndex] == '1');

                Vector3 position = new Vector3(x, y, 0);
                GameObject tile = Instantiate(isCollision ? terrainPrefab : spacePrefab, position, Quaternion.identity);

                // Set the color and texture of the tile based on its background index
                int imageIndex = map.imgBgrs[tileIndex % map.imgBgrs.Length];
                // Color color = ParseColor(map.colorBgrs[imageIndex]);
                // tile.GetComponent<SpriteRenderer>().color = color;
                string imagePath = Path.Combine(imageDirectoryPath, map.id + ".png");
                if (File.Exists(imagePath))
                {
                    byte[] imageData = File.ReadAllBytes(imagePath);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageData);
                    tile.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                }
                else
                {
                    Debug.LogError("Image file not found: " + imagePath);
                }

            }
        }

        // Instantiate the mobs at their respective positions
        foreach (Mob mob in map.mobs)
        {
            Vector3 position = new Vector3(mob.x, mob.y, 0);
            Instantiate(mobPrefab, position, Quaternion.identity);
        }
    }

    private Color ParseColor(string colorString)
    {
        string[] rgb = colorString.Split(',');
        int r = int.Parse(rgb[0]);
        int g = int.Parse(rgb[1]);
        int b = int.Parse(rgb[2]);
        return new Color(r / 255f, g / 255f, b / 255f);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
[System.Serializable]
public class Map
{
    public int id;
    public string name;
    public int type;
    public int planetId;
    public int row;
    public int column;
    public int[] imgBgrs;
    public string[] colorBgrs;
    public string data;
    public Mob[] mobs;
}

[System.Serializable]
public class Mob
{
    public int id;
    public int x;
    public int y;
}

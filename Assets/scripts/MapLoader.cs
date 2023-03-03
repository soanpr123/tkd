using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Cinemachine;
public class MapLoader : MonoBehaviour
{
    public string mapUrl;
    public string imageDirectoryPath;

    public GameObject terrainPrefab;
    public GameObject spacePrefab;
    public GameObject mobPrefab;
    public GameObject map;
    private GameObject obj;
    private Dictionary<int, Texture2D> textures = new Dictionary<int, Texture2D>();
    private Dictionary<int, GameObject> mobs = new Dictionary<int, GameObject>();
    private Texture2D texture;
    private BoxCollider2D barrierCollider;
    public GameObject playerPrefab;
    public GameObject barrierObj;
    public GameObject Player;
    private CinemachineVirtualCamera virtualCamera;
    // private Vector2 tileSize;
    // Start is called before the first frame update
    void Start()
    {
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        StartCoroutine(LoadMap());
    }

    private IEnumerator LoadMap()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(mapUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load map: {request.error}");
                yield break;
            }

            string json = request.downloadHandler.text;
            MapData mapData = JsonUtility.FromJson<MapData>(json);

            Vector2 tileSize = terrainPrefab.GetComponent<BoxCollider2D>().size;
            float mapWidth = mapData.column * tileSize.x;
            float mapHeight = mapData.row * tileSize.y;
            Vector3 mapCenter = new Vector3(mapWidth / 2, -mapHeight / 2, 0);
            List<Vector2> points = new List<Vector2>();
            points.Add(new Vector2(0, 0));
            points.Add(new Vector2(0, -mapHeight));
            points.Add(new Vector2(mapWidth, -mapHeight));
            points.Add(new Vector2(mapWidth, 0));
            barrierObj.GetComponent<PolygonCollider2D>().points = points.ToArray();
            barrierObj.GetComponent<PolygonCollider2D>().transform.position = mapCenter;

            string imagePath = Path.Combine(imageDirectoryPath, mapData.id + ".png");

            for (int row = 0; row < mapData.row; row++)
            {
                for (int col = 0; col < mapData.column; col++)
                {
                    int idx = row * mapData.column + col;
                    int bgrId = mapData.data[idx] - '0';

                    GameObject prefab = bgrId == 1 ? terrainPrefab : spacePrefab;
                    Vector2 pos = new Vector2(col * tileSize.x + 0.35f, -row * tileSize.y - 0.4f);
                    obj = Instantiate(prefab, pos, Quaternion.identity);
                    obj.transform.SetParent(transform);

                }
            }

            if (File.Exists(imagePath))
            {
                byte[] imageData = File.ReadAllBytes(imagePath);
                texture = new Texture2D(2, 2);
                texture.LoadImage(imageData);
                map.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                obj = Instantiate(map, mapCenter, Quaternion.identity);

                obj = Instantiate(barrierObj, new Vector2(0, 0), Quaternion.identity);
                GameObject playerObj = Instantiate(Player, new Vector2(10, 0), Quaternion.identity);

                map.transform.position = mapCenter;
                obj.transform.SetParent(transform);
                virtualCamera.Follow = playerObj.transform;


                // map.transform.position = new Vector3(mapData.column / 2.9f, -mapData.row / 3.3f, 0);
            }
            else
            {
                Debug.LogError("Image file not found: " + imagePath);
            }
        }
    }
    void Update()
    {

    }

    [System.Serializable]
    public class MapData
    {
        public int id;
        public string name;
        public int type;
        public int planetId;
        public int row;
        public int column;
        public List<int> imgBgrs;
        public List<string> colorBgrs;
        public string data;
        public List<MobData> mobs;
    }
    [System.Serializable]
    public class MobData
    {
        public int id;
        public int x;
        public int y;
    }
    // Update is called once per frame

}

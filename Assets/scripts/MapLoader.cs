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
    public string imageBgDirectory;

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
    private GameObject bg1;
    private GameObject bgi;
    public GameObject barrierObj;
    public GameObject Player;
    private GameObject playerObj;
    private CinemachineVirtualCamera virtualCamera;
    private GameObject camm;
    float mapWidth ;
    float mapHeight;
    Vector3 mapCenter;
    // private Vector2 tileSize;
    // Start is called b    efore the first frame update
    private CinemachineConfiner cinemachineConfiner;
    void Start()
    {
        camm = GameObject.Find("playerCam");
        virtualCamera = camm.GetComponent<CinemachineVirtualCamera>();
        cinemachineConfiner = camm.GetComponent<CinemachineConfiner>();
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
             mapWidth = mapData.column * tileSize.x;
             mapHeight = mapData.row * tileSize.y;
             mapCenter = new Vector3(mapWidth / 2, -mapHeight / 2, 0);
            Vector3 mapTopRight = new Vector3(mapWidth, 0, 0) + mapCenter;
            float maxMapHeight = mapTopRight.y;
            List<Vector2> points = new List<Vector2>();
            points.Add(new Vector2(0, 0));
            points.Add(new Vector2(0, -mapHeight));
            points.Add(new Vector2(mapWidth, -mapHeight));
            points.Add(new Vector2(mapWidth, 0));
           
 


            // Thêm confiner vào virtual camera

            string imagePath = Path.Combine(imageDirectoryPath, mapData.id + ".png");
            string imagebg = Path.Combine(imageBgDirectory, mapData.imgBgrs[0] + ".png");

            for (int row = 0; row < mapData.row; row++)
            {
                for (int col = 0; col < mapData.column; col++)
                {
                    int idx = row * mapData.column + col;
                    int bgrId = mapData.data[idx] - '0';

                    GameObject prefab = bgrId == 1 ? terrainPrefab : spacePrefab;
                    Vector2 pos = new Vector2(col * tileSize.x + 0.35f, -row * tileSize.y - 0.4f);
                    obj = Instantiate(prefab, pos, Quaternion.identity);


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

                playerObj = Instantiate(Player, new Vector2(10, 0), Quaternion.identity);

                map.transform.position = mapCenter;
                barrierObj.GetComponent<PolygonCollider2D>().points = points.ToArray();
                barrierObj.GetComponent<PolygonCollider2D>().transform.position = mapCenter;
                Player.GetComponent<PlayerMove>().maxJum = barrierObj.GetComponent<PolygonCollider2D>().bounds.size.y / 2;
                virtualCamera.Follow = playerObj.transform;
                cinemachineConfiner.m_BoundingShape2D =  obj.GetComponent<Collider2D>();
                

            }
            else
            {
                Debug.LogError("Image file not found: " + imagePath);
            }
            bg1 = new GameObject("bg0");

            bg1.AddComponent<SpriteRenderer>();
           
            if (File.Exists(imagebg))
            {
               

                GameObject bgmap = new GameObject("mapBgr");

                byte[] imageData1 = File.ReadAllBytes(imagebg);
                texture = new Texture2D(2, 2);
                texture.LoadImage(imageData1);
                bg1.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                bg1.AddComponent<BgEfff>().speed =bg1.GetComponent<SpriteRenderer>().size.y/2.5f;
                //bg1.AddComponent<BgEfff>().maxMove = barrierObj.GetComponent<PolygonCollider2D>().bounds.size.y / 2;
                bg1.transform.SetParent(bgmap.transform);
                bg1.transform.position = mapCenter;
                bg1.GetComponent<SpriteRenderer>().sprite.texture.wrapMode = TextureWrapMode.Repeat;
                bg1.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Tiled;
                bg1.GetComponent<SpriteRenderer>().tileMode = SpriteTileMode.Continuous;
                bg1.GetComponent<SpriteRenderer>().size = new Vector2(mapWidth, bg1.GetComponent<SpriteRenderer>().size.y);
                // for (int i = 1; i < mapData.imgBgrs.Count; i++)
                // {   
                bgi = new GameObject("bg" + 1);

                bgi.AddComponent<SpriteRenderer>();
                string bgs = Path.Combine(imageBgDirectory, mapData.imgBgrs[1] + ".png");
                byte[] imageBgs = File.ReadAllBytes(bgs);
                texture = new Texture2D(2, 2);
                texture.LoadImage(imageBgs);
                bgi.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                bgi.transform.SetParent(bgmap.transform);
                Vector2 poss = new Vector2(mapCenter.x, mapCenter.y - 1.08f);
                bgi.AddComponent<backGroundController>().backGroundH = mapHeight / 5;
                bgi.transform.position = poss;
                bgi.GetComponent<SpriteRenderer>().sprite.texture.wrapMode = TextureWrapMode.Repeat;
                bgi.GetComponent<SpriteRenderer>().drawMode = SpriteDrawMode.Tiled;
                bgi.GetComponent<SpriteRenderer>().tileMode = SpriteTileMode.Continuous;
                bgi.GetComponent<SpriteRenderer>().size = new Vector2(mapWidth, bgi.GetComponent<SpriteRenderer>().size.y);
                // };

                // // Thiết lập kích thước của sprite renderer để hiển thị đầy đủ bản đồ
                // bg1.transform.localScale = new Vector3(mapWidth, mapHeight, 1f);
                // bg1.GetComponent<SpriteRenderer>().bounds.size=Vector3(mapWidth,mapHeight,-10);
            }
            else
            {
                Debug.LogError("Image file not found: " + imagePath);
            }
            obj.transform.SetParent(transform);
        }
    }
    void Update()
    {
        if (playerObj != null)
        {
            Debug.Log(playerObj.transform.position);
            if (playerObj.transform.position.x < 0 || playerObj.transform.position.x > mapWidth)
            {
                // Nếu nhân vật đang nằm ngoài map thì đặt lại vị trí của nó ở biên giới hạn của map
                playerObj.transform.position = new Vector3(Mathf.Clamp(playerObj.transform.position.x+(Player.GetComponent<SpriteRenderer>().bounds.size.x / 2), 0, mapWidth), playerObj.transform.position.y, playerObj.transform.position.z);
            }

            // Kiểm tra vị trí y của nhân vật có nằm trong giới hạn của map không
            if (playerObj.transform.position.y > 0 || playerObj.transform.position.y < -mapHeight)
            {
                // Nếu nhân vật đang nằm ngoài map thì đặt lại vị trí của nó ở biên giới hạn của map
                playerObj.transform.position = new Vector3(playerObj.transform.position.x, Mathf.Clamp(playerObj.transform.position.y+((Player.GetComponent<SpriteRenderer>().bounds.size.y / 2)), -mapHeight, 0), playerObj.transform.position.z);
            }
        }
    }


    void createGameObject()
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

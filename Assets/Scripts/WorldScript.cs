using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class WorldScript : MonoBehaviour
{
    //Text Boxes
    public Text HealthText;
    public Text TimerText;

    //Player
    public GameObject Player;
    public GameObject PlayerCharacter;
    PlayerScript player_script;
    public float _playerSpeed = 15.0f;

    //Enemy
    public GameObject Enemy;
    public List<GameObject> Enemies;
    
    //Camera
    public Camera Cam;
    private Vector3 CameraTargetLocation;
    private float CamMoveSpeed = 10;
    private float step;
    
    //Pause game
    public bool gameRunning = true;
    public Canvas pauseMenu;
    public Canvas gameOver;

    //Allow other scripts to access this script
    public static WorldScript Instance;

    //Loot Chest Sprites
    public List<Sprite> ChestOpen;
    public List<Sprite> ChestLoot;
    
    //Room Generation
    public GameObject SpawnRoom;
    public List<GameObject> Rooms;
    public GameObject RoomParts;
    public Grid TileMapGrid;
    GameObject RoomPartsFloor; 
    GameObject RoomPartsDoors;
    GameObject RoomPartsWalls; 
    public List<Vector2Int> FloorLayout;
    public Vector2Int CurrentRoom = Vector2Int.zero;
    GameObject EmptyGameObj;
    public List<Vector2> Locations;

    //A* pathing
    //public AIPath AStarPathing;
    //public GameObject AStarPathing;
    public GridGraph AStarGrid;
    public AstarData data;
    public TextAsset PathingData;

    //Timer variables
    float timer = 0.0f;
    float timerMins;
    float timerSecs;
    float timerMili;


    // Start is called before the first frame update
    void Start()
    {
        //AIPathObj = Instantiate(AIPathing, Vector3.zero, Quaternion.identity);
        //AStarGrid = AStarPathing.GetComponent<GridGraph>();
        pauseMenu.gameObject.SetActive(false);
        gameOver.gameObject.SetActive(false);
        RoomPartsFloor = RoomParts.transform.GetChild(0).gameObject;
        RoomPartsDoors = RoomParts.transform.GetChild(1).gameObject;
        RoomPartsWalls = RoomParts.transform.GetChild(2).gameObject;
        Instantiate(SpawnRoom, new Vector3(0.0f,0.0f,-1.0f), Quaternion.identity);
        createMap();
        spawnRooms();
        spawnPlayer();
        //createPathing();
        Instance = this;
        Enemies = new List<GameObject>();
        step = CamMoveSpeed * Time.fixedDeltaTime;
        Cam.transform.position = new Vector3(0, 0, -10);
        Cam.transform.position = new Vector3(0, 0, -10);
        CameraTargetLocation = Cam.transform.position;
    }


    // Update is called once per frame
    void Update()
    {
        HealthText.text = "Health: " + PlayerCharacter.GetComponent<PlayerScript>().health.ToString();
        if (gameRunning)
        {
            timer += Time.deltaTime;
            timerMins = Mathf.Floor(timer / 60);
            timerSecs = Mathf.Floor(timer % 60);
            timerMili = Mathf.Floor(timer * 1000) % 1000;

            if (timerMins < 10)
            {
                TimerText.text = "0" + timerMins.ToString() + ":";
            }
            else
            {
                TimerText.text = timerMins.ToString() + ":";
            }
            if (timerSecs < 10)
            {
                TimerText.text += "0" + timerSecs.ToString() + ".";
            }
            else
            {
                TimerText.text += timerSecs.ToString() + ".";
            }
            if (timerMili < 10)
            {
                TimerText.text += "00" + timerMili.ToString();
            }
            else if (timerMili < 100)
            {
                TimerText.text += "0" + timerMili.ToString();
            }
            else
            {
                TimerText.text += timerMili.ToString();
            }
        }


        if (gameRunning == false)
        {
            player_script.speed = 0.0f;
        }
        else if (gameRunning == true)
        {
            player_script.speed = _playerSpeed;
        }
        
        if (Input.anyKey)
        {
            switch (Input.inputString.ToString())
            {
                case ("i"):
                    Debug.Log("I Pressed");
                    break;
                case ("k"):
                    Debug.Log("K Pressed");
                    break;
                case ("j"):
                    Debug.Log("J Pressed");
                    break;
                case ("l"):
                    break;
                case ("g"):
                    break;
                default:
                    break;
            }

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseGame();
        }
        for (int x = 0; x < Enemies.Count; x++) {
            if (Enemies[x].GetComponent<EnemyScript>().health <= 0)
            {
                Debug.Log(Enemies);
                Destroy(Enemies[x]);
                Enemies.RemoveAt(x);
            }
        }

        if (Cam.transform.position != CameraTargetLocation)
        {
            Cam.transform.position = Vector3.MoveTowards(Cam.transform.position, CameraTargetLocation, step);
        }
        else
        {
            if (pauseMenu.gameObject.activeInHierarchy || gameOver.gameObject.activeInHierarchy)
            {
                gameRunning = false;

            }
            else
            {
                gameRunning = true;
            }
        }
    }

    public void createMap()
    {
        FloorLayout.Add(new Vector2Int( 0,  0));
        FloorLayout.Add(new Vector2Int( 0,  1));
        FloorLayout.Add(new Vector2Int( 1,  0));
        FloorLayout.Add(new Vector2Int( 0, -1));
        FloorLayout.Add(new Vector2Int(-1,  0));
        Vector2Int newRoom;

        for(int i = 0; i < 8; i++)
        {
            //Choose a random room in the list
            newRoom = FloorLayout[Random.Range(0, FloorLayout.Count)];
            while (FloorLayout.Contains(newRoom)) {
                //Add new item adjacent to room in list
                switch (Random.Range(0, 5))
                {
                    case 0:         // Add Room Top
                        newRoom.y++;
                        break;
                    case 1:         // Add Room Right
                        newRoom.x++;
                        break;
                    case 2:         // Add Room Bottom
                        newRoom.y--;
                        break;
                    case 3:         // Add Room Left
                        newRoom.x--;
                        break;
                }
            }

            FloorLayout.Add(newRoom);
        }

    }

    public void spawnRooms()
    {
        for (int i = 1; i < FloorLayout.Count; i++)
        {
            EmptyGameObj = new GameObject("Room_" + i);
            
            EmptyGameObj.transform.position = new Vector3(FloorLayout[i].x * 36, FloorLayout[i].y * 20, -1);
            EmptyGameObj.transform.parent = TileMapGrid.transform;
            Instantiate(RoomPartsFloor.transform.GetChild(Random.Range(0, RoomPartsFloor.transform.childCount)), EmptyGameObj.transform);

            for (int x = 0; x < 4; x++){
                Vector2Int temp = FloorLayout[i];
                switch (x)
                {
                    case 0:
                        temp.y++;
                        break;
                    case 1:
                        temp.x++;
                        break;
                    case 2:
                        temp.y--;
                        break;   
                    case 3:
                        temp.x--;
                        break;
                    default:
                        break;
                }
                if (FloorLayout.Contains(temp))
                {
                    Instantiate(RoomPartsDoors.transform.GetChild(x), EmptyGameObj.transform);
                }
                else
                {
                    Instantiate(RoomPartsWalls.transform.GetChild(x), EmptyGameObj.transform);
                }
            }
        }
    }

    public void spawnEnemies()
    {
        if (CurrentRoom != Vector2Int.zero)
        {
            for (int i = 0; i < TileMapGrid.transform.GetChild(FloorLayout.IndexOf(CurrentRoom) - 1).GetChild(0).GetChild(1).childCount; i++)
            {
                Vector3 temp = TileMapGrid.transform.GetChild(FloorLayout.IndexOf(CurrentRoom) - 1).GetChild(0).GetChild(1).GetChild(i).position;
                GameObject EnemySpawn = Instantiate(Enemy, temp, Quaternion.identity);
                EnemyScript _enemyScript = EnemySpawn.GetComponent<EnemyScript>();
                _enemyScript.Player = PlayerCharacter;
                _enemyScript.health = 15;
                _enemyScript.damage = 3;
                Enemies.Add(EnemySpawn);

                Destroy(TileMapGrid.transform.GetChild(FloorLayout.IndexOf(CurrentRoom) - 1).GetChild(0).GetChild(1).GetChild(i).gameObject);
            }
            
        }
       
    }

    public void spawnPlayer()
    {
        PlayerCharacter = Instantiate(Player, new Vector3(0, 0, -1), Quaternion.identity);
        player_script = PlayerCharacter.GetComponent<PlayerScript>();
        player_script.health = 15;
        player_script.shotDamage = 5;
        player_script.speed = _playerSpeed;

    }

    public void roomTransition(string direction)
    {
        gameRunning = false;
        switch (direction.ToLower())
        {
            case ("up"):
                CameraTargetLocation.y += 20;
                CurrentRoom.y++;
                break;
            case ("down"):
                CameraTargetLocation.y -= 20;
                CurrentRoom.y--;
                break;
            case ("left"):
                CameraTargetLocation.x -= 36;
                CurrentRoom.x--;
                break;
            case ("right"):
                CameraTargetLocation.x += 36;
                CurrentRoom.x++;
                break;
            default:
                break;
        }
        spawnEnemies();
        AstarPath.active.data.gridGraph.center = new Vector3(CurrentRoom.x * 36, CurrentRoom.y * 20, 0);
        AstarPath.active.Scan();

    }

    public void pauseGame()
    {
        if (gameRunning)
        {
            gameRunning = false;
            pauseMenu.gameObject.SetActive(true);
        }
        else
        {
            gameRunning = true;
            pauseMenu.gameObject.SetActive(false);
        }
    }

    public void showGameOver()
    {
        gameRunning = false;
        gameOver.gameObject.SetActive(true);

    }

}


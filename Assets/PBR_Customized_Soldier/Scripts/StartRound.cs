using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StartRound : MonoBehaviour
{
    public Camera DeadCam;
    [System.Serializable]
    public class Pool
    {
        public string Tag;
        public GameObject Prefab;
        public int Size;
    }
    public List<Pool> pools;

    public GameObject PlayerPrefab;
    public GameObject TBotPrefab;
    public GameObject CTBotPrefab;
    public const int TeamSize = 5;

    //public Transform[] TSpawnPoints;
    private string PlayerIdentity;
    //public Transform[] CTSpawnPoints;

    [System.Serializable]
    public class MyClass
    {
        public Transform transform;
        public bool IsUsed;
    }
    public List<MyClass> TSpawnPoints = new List<MyClass>();
    public List<MyClass> CTSpawnPoints = new List<MyClass>();

    private GameObject player;
    private PlayerHealth playerHealth;
    private PlayerGunShoot playerGunShoot;

    private GameObject[] TBots = new GameObject[TeamSize];
    private GameObject[] CTBots = new GameObject[TeamSize];
    [SerializeField]private Image[] TImages = new Image[TeamSize];
    [SerializeField]private Image[] CTImages = new Image[TeamSize];

    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public Transform[] Waypoints;

    public PlayerInGameInputs PlayerInputRef;
    //public bool Patrolling;

    private float TimeToCheck;
    private float TimeElapsed;
    public Canvas HUD;

   // public Image WinPanel;
    public TextMeshProUGUI WinnerTeam;

    [System.Serializable]
    public class HealthHUD
    {
        public Slider HealthSlider;
        public TextMeshProUGUI HealthAmount;

        public void SetHealth(float health)
        {
            HealthSlider.value = health;
            HealthAmount.text = health.ToString().Replace(".00","");
        }
    }
    public HealthHUD healthHUD;

    [System.Serializable]
    public class BulletCount
    {
        public TextMeshProUGUI BulletInCurrentMag;
        public TextMeshProUGUI MagInGun;

        public void SetAmmoAndMag(int ammo,int magingun)
        {
            BulletInCurrentMag.text = ammo.ToString();
            MagInGun.text = "/ "+magingun.ToString();
        }
    }
    public BulletCount bulletCount;

    public TextMeshProUGUI TimeRemaining;
    private int timeRemaining;

    public TextMeshProUGUI ctRoundWins;
    private int CTRoundWins;
    private int TotalRounds;
    public TextMeshProUGUI tRoundWins;
    private int TRoundWins;
    public int Maxounds;

    //Start is called before the first frame update
    void Start()
    {
        Resources.UnloadUnusedAssets();
        PlayerPrefab.GetComponent<Name>().name = PlayerPrefs.GetString("WhoAmI","CT");
        SetRoundWins();
        Time.timeScale = 1;
        //WinnerTeam = WinPanel.GetComponentInChildren<TextMeshProUGUI>();

        timeRemaining = 60;
        StartCoroutine(TimerStart());
        TimeToCheck = 2f;
        TimeElapsed = 0f;
        bool flag = true;
        PlayerIdentity = PlayerPrefab.GetComponent<Name>().name;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < pool.Size; i++)
            {
                GameObject obj = Instantiate(pool.Prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.Tag, objectPool);
        }

        for (int i = 0; i < TeamSize; i++)
        {
            //int t = Random.Range(0, TSpawnPoints.Count);
            //while (TSpawnPoints[t].IsUsed)
            //{
            //    t = Random.Range(0, TSpawnPoints.Count);
            //    if (!TSpawnPoints[t].IsUsed)
            //    {
            //        TSpawnPoints[t].IsUsed = true;
            //        break;
            //    }
            //}
            int t;
            do
            {
                t = Random.Range(0, TSpawnPoints.Count);
            } while (TSpawnPoints[t].IsUsed);

            //int ct = Random.Range(0, CTSpawnPoints.Count);
            //while (CTSpawnPoints[t].IsUsed)
            //{
            //    t = Random.Range(0, CTSpawnPoints.Count);
            //    if (!CTSpawnPoints[t].IsUsed)
            //    {
            //        CTSpawnPoints[t].IsUsed = true;
            //        break;
            //    }
            //}

            int ct;
            do
            {
                ct = Random.Range(0, CTSpawnPoints.Count);
            } while (CTSpawnPoints[ct].IsUsed);

            CTBots[i] = Instantiate(CTBotPrefab, CTSpawnPoints[ct].transform.position, Quaternion.identity);
            TBots[i] = Instantiate(TBotPrefab, TSpawnPoints[t].transform.position, Quaternion.identity);
            CTBots[i].GetComponentInChildren<Camera>().enabled = false;
            TBots[i].GetComponentInChildren<Camera>().enabled = false;

            if (PlayerIdentity == "CT")
            {
                if (flag)
                {
                    player = Instantiate(PlayerPrefab, CTSpawnPoints[ct].transform.position, Quaternion.identity);
                    Destroy(CTBots[i]);
                    CTBots[i] = player;
                    PlayerInputRef.PlayerReference = player;
                    CTBots[i].GetComponentInChildren<Camera>().enabled = true;
                    flag = false;
                }
                //TBots[i].GetComponent<EnemyAI>().target = player.GetComponent<Transform>();
            }

            if (PlayerIdentity == "T")
            {
                if (flag)
                {
                    player = Instantiate(PlayerPrefab, TSpawnPoints[ct].transform.position, Quaternion.identity);
                    Destroy(TBots[i]);
                    TBots[i] = player;
                    PlayerInputRef.PlayerReference = player;
                    TBots[i].GetComponentInChildren<Camera>().enabled = true;
                    flag = false;
                }
                //CTBots[i].GetComponent<EnemyAI>().target = player.GetComponent<Transform>();
            }
        }

        playerHealth = player.GetComponent<PlayerHealth>();
        playerGunShoot = player.GetComponent<PlayerGunShoot>();
        StartCoroutine(CheckTeamsAlive());
        StartCoroutine(UpdateTime());
        StartCoroutine(CheckPlayer());
    }


    public void DieTrigger(string whoami, Transform postion)
    {
        SpawnPlayerFromPool(whoami, postion);
    }

    private void SpawnPlayerFromPool(string key, Transform Location)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogError("Object With Tag " + key + " Does Not Exist");
            return;
        }
        //Debug.Log(key);
        GameObject obj = poolDictionary[key].Dequeue();
        obj.transform.position = Location.position;
        obj.transform.rotation = Location.rotation;
        obj.SetActive(true);
        // poolDictionary[key].Enqueue(obj);
        
    }
    void Update()
    {

        //TimeElapsed += Time.deltaTime;
       // float temp = TimeElapsed;
        
        foreach (GameObject TBot in TBots)
        {
           // TimeElapsed = temp;
            if (TBot == null)
            {
                continue;
            }

            EnemyAI TBotFollow = TBot.GetComponent<EnemyAI>();
            if (TBotFollow == null)
                continue;

            float DistanceToTargetHuman = 99999999f;            //Distance To The Target If It Is Bot Or Player
            int IndexOfBot=99;
            for (int i = 0; i < TeamSize; i++)
            {
                if (CTBots[i] == null)
                    continue;
                float Distance = Vector3.Distance(TBot.transform.position, CTBots[i].transform.position);
                //   Debug.Log(Distance);
                if (TBotFollow.FollowDistance > Distance)
                {
                    //TBotFollow.TargetIdentity = "human";
                    if (Distance < DistanceToTargetHuman)
                    {
                        IndexOfBot = i;
                        DistanceToTargetHuman = Distance;
                        //TBotFollow.target = CTBots[i].transform;
                    }
                }
                else
                {
                    if (TimeElapsed > TimeToCheck)
                    {
                       // TimeElapsed = 0f;
                        TBotFollow.TargetIdentity = "waypoint";
                         //Debug.Log("Waypoint");
                        TBotFollow.target = Waypoints[Random.Range(0, Waypoints.Length)];
                    }
                }
            }
            //Set Transform Of Target
            if (IndexOfBot!=99)
            {
                TBotFollow.TargetIdentity = "human";
                TBotFollow.target = CTBots[IndexOfBot].transform;
            }
        }
       // TimeElapsed = temp;
        foreach (GameObject CTBot in CTBots)
        {
           // TimeElapsed = temp;
            if (CTBot == null)
            {
                continue;
            }

            EnemyAI CTBotFollow = CTBot.GetComponent<EnemyAI>();
            if (CTBotFollow == null)
                continue;

            float DistanceToTargetHuman = 9999999f;         //Distance To The Target If It Is Bot Or Player
            int IndexOfBot = 99;
            for (int i = 0; i < TeamSize; i++)
            {
                if (TBots[i] == null)
                {
                    continue;
                }
                    
                float Distance = Vector3.Distance(CTBot.transform.position, TBots[i].transform.position);
                if (CTBotFollow.FollowDistance > Distance)
                {
                    //Debug.Log(TimeElapsed + "time" + "in human");
                    //CTBotFollow.TargetIdentity = "human";
                    if (Distance < DistanceToTargetHuman)
                    {
                        IndexOfBot = i;
                        DistanceToTargetHuman = Distance;
                        //CTBotFollow.target = CTBots[i].transform;
                    }
                }
                else
                {
                    if (TimeElapsed > TimeToCheck)
                    {
                       // TimeElapsed = 0f;
                        //Debug.Log(TimeElapsed + "time" +"in waypoint");
                        CTBotFollow.TargetIdentity = "waypoint";
                       // Debug.Log(temp+"in waypoint");
                        CTBotFollow.target = Waypoints[Random.Range(0, Waypoints.Length)];
                    }
                }
               
                
}
            Debug.Log(TimeElapsed);
            if (IndexOfBot != 99)
            {
                CTBotFollow.TargetIdentity = "human";
                CTBotFollow.target = TBots[IndexOfBot].transform;
            }
        }


        for (int i = 0; i < TeamSize; i++)
        {
            if (CTBots[i]==null)
            {
                CTImages[i].CrossFadeAlpha(0, 0.1f, true);
            }
            if (TBots[i]==null)
            {
                TImages[i].CrossFadeAlpha(0, 0.1f, true);
            }
        }
        healthHUD.SetHealth(playerHealth.health);
        bulletCount.SetAmmoAndMag(playerGunShoot.BulletInMag, playerGunShoot.BulletRemaining);
     //   Debug.Log(TimeElapsed);
    }
    
    IEnumerator UpdateTime()
    {
        while(true)
        {
           
                yield return new WaitForSeconds(1f);
            TimeElapsed++;
            if (TimeElapsed > TimeToCheck)
            {
                yield return new WaitForSeconds(Time.deltaTime);
                TimeElapsed = 0f;
            }
        }
    }
    IEnumerator TimerStart()
    {
        while (timeRemaining>0)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining--;
            UpdateCounter();
           // Debug.Log(timeRemaining); 
        }
        if (timeRemaining<=0)
        {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(3f);
            WinnerTeam.text = "Round " + TotalRounds + " Over";
            Time.timeScale = 1;
            RestartRound();
        }
    }

    private void UpdateCounter()
    {
        TimeRemaining.text = timeRemaining.ToString();
    }

    private void RestartRound()
    {
        Debug.Log("In Restart Round");
        bool TWin = false;
        bool CTWin = false;
        foreach (GameObject Tbot in TBots)
        {
            if (Tbot != null)
            {
                TWin = true;
                break;
            }
            else
            {
                TWin = false;
            }
        }
        foreach (GameObject CTbot in  CTBots)
        {
            if(CTbot != null)
            {
                CTWin = true;
                break;
            }
            else
            {
                CTWin = false;
            }
            
        }
        if(TWin != CTWin)
        {
            if (TWin)
            {
                // int lastWin = PlayerPrefs.GetInt("TRoundWins", 0);
                Debug.Log("Terrorist Wins");
                PlayerPrefs.SetInt("TRoundWins", ++CTRoundWins);
                WinnerTeam.text = ("Round " + TotalRounds + " Over Counter-Terrorist Win");

            }
            if (CTWin)
            {
                // int lastWin = PlayerPrefs.GetInt("CTRoundWins", 0);
                Debug.Log("CounterTerrorist Wins");
                PlayerPrefs.SetInt("CTRoundWins", ++TRoundWins);
                WinnerTeam.text = ("Round " + TotalRounds + " Over Terrorist Win");
            }
        }

        TotalRounds++;
        
        if (TotalRounds <= Maxounds)
        {
            Debug.LogError("Game Ended");
            PlayerPrefs.SetInt("TotalRounds", TotalRounds);
            StopCoroutine(TimerStart());
            StartCoroutine(ReloadLevel());
        }
        else
        {
            CTRoundWins = PlayerPrefs.GetInt("CTRoundWins",0);
            TRoundWins = PlayerPrefs.GetInt("TRoundWins",0);
            PlayerPrefs.SetInt("TotalRounds", TotalRounds);

            if (CTRoundWins > TRoundWins)
            {
                WinnerTeam.text = "GameOver:Counter-Terroist Win....";
                Debug.Log(CTRoundWins + "Counter Wins");
            }
            else if (TRoundWins > CTRoundWins)
            {
                WinnerTeam.text = "GameOver:Terroist Win....";
                Debug.Log(CTRoundWins);
            }
            else
            {
                WinnerTeam.text = "GameOver:You Tied....";
            }
            WinnerTeam.enabled = true;
            StartCoroutine(LoadMainMenu());
        }
    }

    IEnumerator ReloadLevel()
    {
        yield return null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    IEnumerator CheckTeamsAlive()
    {
        while (true)
        {
            bool reload = false;

            foreach (GameObject item in CTBots)
            {
                if (item != null)
                {
                    reload = false;
                    break;
                }
                reload = true;
            }
            if (reload)
            {
                yield return new WaitForSeconds(3f);
                RestartRound();
            }

            foreach (GameObject item in TBots)
            {
                if (item != null)
                {
                    reload = false;
                    break;
                }
                reload = true;
            }
            if (reload)
            {
                yield return new WaitForSeconds(3f);
                RestartRound();
            }
            yield return new WaitForSeconds(4f);
        }
       
    }

    public void SetRoundWins()
    {
        //if(TotalRounds==1)
        //{
        //    Debug.LogError("The Game Rounds Resets");
        //    PlayerPrefs.SetInt("CTRoundWins", 0);
        //    PlayerPrefs.SetInt("TRoundWins", 0);
        //}
        TotalRounds = PlayerPrefs.GetInt("TotalRounds", 1);
        
        CTRoundWins = PlayerPrefs.GetInt("CTRoundWins",0);
        ctRoundWins.text = CTRoundWins.ToString();

        TRoundWins = PlayerPrefs.GetInt("TRoundWins", 0);
        tRoundWins.text = TRoundWins.ToString();
    }

    IEnumerator CheckPlayer()
    {
        while (true)
        {
            if (player == null)
            {
                Debug.LogError("Player Died");
                DeadCam.enabled = true;
                DeadCam = Camera.main;
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}

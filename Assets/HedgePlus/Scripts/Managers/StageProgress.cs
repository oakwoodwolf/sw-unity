using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class StageProgress : MonoBehaviour
{
    public static StageProgress instance;
    PlayerController PlayerObject;
    PlayerCamera camera;
    Vector3 InitialPosition;
    Quaternion InitialRotation;
    public Vector3 SpawnPosition { get; set; }
    public Quaternion SpawnRotation { get; set; }

    public Animator fadeAnim;
    public float RespawnDelay = 2f;
    public string GameOverScene;
    Spawner[] ObjectSpawners;
    CheckpointActor[] Checkpoints;
    ItemCapsuleActor[] Capsules;

    //Public objects
    public AudioSource MusicSource;
    public AudioClip StageMusic;
    public AudioClip StageClear;
    public AudioClip InvincibleTheme;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        //Get the PlayerObject and its initial position and rotation
        PlayerObject = FindObjectOfType<PlayerController>();
        camera = FindObjectOfType<PlayerCamera>();
        InitialPosition = PlayerObject.transform.position;
        InitialRotation = PlayerObject.transform.rotation;
        //Set starting spawn position/rotation
        SpawnPosition = InitialPosition;
        SpawnRotation = InitialRotation;
        //Get every Object Spawner and Checkpoint
        ObjectSpawners = FindObjectsOfType<Spawner>();
        Checkpoints = FindObjectsOfType<CheckpointActor>();
        Capsules = FindObjectsOfType<ItemCapsuleActor>();

        ChangeMusic("stage music", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Respawn()
    {
        //Reset every available Object Spawner
        foreach (Spawner s in ObjectSpawners)
        {
            if (!s.gameObject.activeSelf)
                s.gameObject.SetActive(true);
            if (s.DidSpawn && !s.spawnedObject.activeSelf)
                s.DidSpawn = false;
        }
        foreach (ItemCapsuleActor c in Capsules)
        {
            if (c.CanRespawn && !c.IsActive)
                c.IsActive = true;
        }
        //Move Sonic to the starting position and rotation
        PlayerObject.transform.position = SpawnPosition;
        PlayerObject.transform.rotation = SpawnRotation;
        //Reset velocity
        PlayerObject.rigidBody.velocity = Vector3.zero;
        //Reset rings
        PlayerHealth.RingCount = 0;
        PlayerHealth.IsDead = false;
        PlayerObject.InputLocked = false;
        PlayerObject.RotationLocked = false;
        PlayerHealth _health = PlayerObject.GetComponent<PlayerHealth>();
        _health.LifeCount--;
        _health.RingsToLife = _health.RingsPerOneUp;
        ScoreController._score.Score = 0;
        //Reset Camera
        camera.RespawnCamera(SpawnPosition, PlayerObject.transform.forward);
    }

    public void Restart()
    {
        //Reset every available checkpoint
        foreach (CheckpointActor c in Checkpoints)
        {
            if (c.IsActive)
                c.IsActive = false;
        }
        //Reset the spawn position/rotation, respawn
        SpawnPosition = InitialPosition;
        SpawnRotation = InitialRotation;
        Respawn();
    }

    public void ChangeMusic (string name, bool loop)
    {
        MusicSource.Stop();
        string _caseName = name.ToLower();
        switch (_caseName)
        {
            default:
                MusicSource.clip = StageMusic;
                Debug.LogError("The given music name does not exist.");
                break;
            case "stage music":
                MusicSource.clip = StageMusic;
                break;
            case "invincibility":
                MusicSource.clip = InvincibleTheme;
                break;
            case "stage clear":
                MusicSource.clip = StageClear;
                break;
        }
        MusicSource.loop = loop;
        MusicSource.Play();
    }
    public void BeginRespawn()
    {
        StartCoroutine("StartRespawn");
    }
    public void BeginGameOver()
    {
        StartCoroutine("StartGameOver");
    }
    IEnumerator StartRespawn()
    {
        fadeAnim.SetBool("Fade", true);
        yield return new WaitForSeconds(RespawnDelay);
        Respawn();
        fadeAnim.SetBool("Fade", false);
    }
    IEnumerator StartGameOver()
    {
        fadeAnim.SetBool("Fade", true);
        yield return new WaitForSeconds(RespawnDelay);
        SceneManager.LoadSceneAsync(GameOverScene);
    }
}

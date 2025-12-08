using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Scene Config")]
    public static GameManager _Instance;
    public static int mainScene;

    [Header("CheckPoint")]
    [SerializeField] private GateBehavior lastGate;
    private List<string> closedMazes = new List<string>();

    [Header("Game State")]
    public int mazesDone = 0;

    [Header("Components")]
    [SerializeField] private TransitionManager transitionManager;
    public Transform playerRef;

    [Header("Music")]
    [SerializeField] private Transform musicHolder;
    private List<AudioSource> musicSrc = new List<AudioSource>();
    public int musinx;

    private void Awake()
    {
        // Set up Singleton
        if (null == _Instance && _Instance != this)
        {
            Debug.Log("ASSIGNING GAMEMANAGER: " + gameObject);
            _Instance = this;
            DontDestroyOnLoad(gameObject);
            mainScene = SceneManager.GetActiveScene().buildIndex;
        }
        else Destroy(gameObject);
    }

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void Start()
    {
        // TransitionManager to play scene swap animations
        //tm = GetComponentInChildren<TransitionManager>();

        closedMazes.Clear();
        mazesDone = closedMazes.Count;

        foreach (AudioSource src in musicHolder.GetComponents<AudioSource>())
        {
            musicSrc.Add(src);
        }
        StartMusic();
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        // The Main Scene is the forest level
        // We need to the gates to remember their completion status
        // This behavior only happens for the forest, not when we enter a maze level

        if (scene.buildIndex == mainScene)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            RestoreGateState(scene);
            StartMusic();
        }

        // Loading a maze level where we need a cursor
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    private void StartMusic()
    {
        // Plays music based on levels completed

        if (0 >= musicSrc.Count) return;
        musinx = mazesDone;
        if (musicSrc.Count <= musinx) musinx = musicSrc.Count - 1;

        AudioSource bgm = musicSrc[musinx];
        if (bgm != null && !bgm.isPlaying) 
        {
            bgm.Play();
        }
    }

    private void RestoreGateState(Scene scene)
    {
        // We need to place the player in the correct world position
        // We also need to play the scene transtion animation
        // If we don't have a reference to a last gate then we're at the start
        if (!lastGate) return;

        StartCoroutine(UpdatePlayer());
        if (transitionManager != null) StartCoroutine(UpdateTM());
    }

    private IEnumerator UpdatePlayer()
    {
        // Ensure we have reference to the player in scene

        yield return new WaitWhile(()=> null == playerRef);
        PlayerExitGate(playerRef, lastGate.exit);
    }

    private IEnumerator UpdateTM()
    {
        // Hacky fix for resetting transition canvas 
        // When the Forest scene is loaded again the 
        // canvas appears full black over the scene
        // in editor this behavior can be remedied 
        // by disabling and enabling the canvas
        transitionManager.gameObject.SetActive(false);
        yield return new WaitUntil( ()=> transitionManager.gameObject.activeSelf == false);

        transitionManager.gameObject.SetActive(true);
        yield return new WaitUntil( ()=> transitionManager.gameObject.activeSelf == true);

        transitionManager.PlayTransition(false, CompleteMaze);
    }

    private void PlayerExitGate(Transform p, Transform t)
    {
        // Move player to the exit position of a gate
        p.GetComponent<Rigidbody>().position = t.position;
    }

    public void SetGate(GateBehavior gba, Transform player = null)
    {
        // If a player walks through a gate they should either: 
        // 1. Start maze (maze incomplete)
        // 2. Traverse through (complete)

        if (!closedMazes.Contains(gba.mazelink))
        {
            lastGate = gba;
            EnterMaze(gba.mazelink);
        }
        else if (player != null) PlayerExitGate(player, gba.exit);
    }

    public void EnterMaze(string lvl = "Maze_1")
    {
        // Transition Manager plays animation and call LoadMaze
        transitionManager.PlayTransition(true, LoadMaze, lvl);
    }

    private void LoadMaze(string lvl)
    {
        // We moved the Manager into the scene on load
        // move it bck to don't destroy when we leave

        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadMazeAsync(lvl));
    }

    private IEnumerator LoadMazeAsync(string lvl)
    {
        // We need to stop the audio source as 
        // it is carried by this manager
        // The TransitionManager also has DOTweens
        // to this gameobject and must be released

        AsyncOperation a = SceneManager.LoadSceneAsync(lvl, LoadSceneMode.Single);
        musicSrc[musinx].Stop();

        while (!a.isDone) yield return null;
        tm.Free();
    }

    public void ExitMaze(string sceneName = null)
    {
        StartCoroutine(ExitMazeAsync(sceneName));
    }

    private IEnumerator ExitMazeAsync(string sceneName)
    {
        // Handle End Scene BuildIndex

        int nextScene = SceneManager.GetSceneByName(sceneName).buildIndex;
        nextScene = (-1 == nextScene) ? 1 : nextScene;
        AsyncOperation a = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);

        while (!a.isDone) yield return null;
        tm.gameObject.SetActive(true);
    }

    private void CompleteMaze(string s = null)
    {
        // Move maze to closed list

        closedMazes.Add(lastGate.mazelink);
        mazesDone++;
    }

    public void Kill()
    {
        // Clear singleton and unload level

        Time.timeScale = 0;
        _Instance = null;

        SceneManager.LoadSceneAsync("Ending", LoadSceneMode.Single);
        Destroy(gameObject);
    }
}

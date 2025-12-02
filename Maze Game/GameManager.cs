using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private TransitionManager tm;
    public Transform playerRef;

    [Header("Music")]
    [SerializeField] private Transform musicHolder;
    private List<AudioSource> musiq = new List<AudioSource>();
    public int musinx;

    private void Awake()
    {
        //Debug.Log(mazesDone + " Game Manager awake: " + gameObject);
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
        //tm = GetComponentInChildren<TransitionManager>();
        Debug.Log(tm?.gameObject.name);

        closedMazes.Clear();
        mazesDone = closedMazes.Count;

        foreach (AudioSource src in musicHolder.GetComponents<AudioSource>())
        {
            musiq.Add(src);
        }
        StartMusic();
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == mainScene)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            RestoreGateState(scene);
            StartMusic();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    private void StartMusic()
    {
        if (0 >= musiq.Count) return;
        musinx = mazesDone;
        if (musiq.Count <= musinx) musinx = musiq.Count - 1;

        AudioSource bgm = musiq[musinx];
        if (bgm != null && !bgm.isPlaying) 
        {
            bgm.Play();
        }
    }

    private void RestoreGateState(Scene scene)
    {
        if (!lastGate) return;

        StartCoroutine(UpdatePlayer());
        if (tm != null) StartCoroutine(UpdateTM());
    }

    private IEnumerator UpdatePlayer()
    {
        yield return new WaitWhile(()=> null == playerRef);
        PlayerExitGate(playerRef, lastGate.exit);
    }

    private IEnumerator UpdateTM()
    {
        tm.gameObject.SetActive(false);
        yield return new WaitUntil( ()=> tm.gameObject.activeSelf == false);

        tm.gameObject.SetActive(true);
        yield return new WaitUntil( ()=> tm.gameObject.activeSelf == true);

        tm.PlayTransition(false, CompleteMaze);
    }

    private void PlayerExitGate(Transform p, Transform t)
    {
        p.GetComponent<Rigidbody>().position = t.position;
    }

    public void SetGate(GateBehavior gba, Transform player = null)
    {
        if (!closedMazes.Contains(gba.mazelink))
        {
            lastGate = gba;
            EnterMaze(gba.mazelink);
        }
        else if (player != null) PlayerExitGate(player, gba.exit);
    }

    public void EnterMaze(string lvl = "Maze_1")
    {
        //LoadMaze(lvl);
        tm.PlayTransition(true, LoadMaze, lvl);
    }

    private void LoadMaze(string lvl)
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(LoadMazeAsync(lvl));
    }

    private IEnumerator LoadMazeAsync(string lvl)
    {
        AsyncOperation a = SceneManager.LoadSceneAsync(lvl, LoadSceneMode.Single);
        musiq[musinx].Stop();

        while (!a.isDone) yield return null;
        //tm.Free();
    }

    public void ExitMaze(string sceneName = null)
    {
        StartCoroutine(ExitMazeAsync(sceneName));
    }

    private IEnumerator ExitMazeAsync(string sceneName)
    {
        int nextScene = SceneManager.GetSceneByName(sceneName).buildIndex;
        nextScene = (-1 == nextScene) ? 1 : nextScene;
        AsyncOperation a = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Single);

        while (!a.isDone) yield return null;
        //tm.gameObject.SetActive(true);
    }

    private void CompleteMaze(string s = null)
    {
        Debug.Log(s + " Completed Maze: " + lastGate);

        closedMazes.Add(lastGate.mazelink);
        mazesDone++;

        Debug.Log(mazesDone);
    }

    public void Kill()
    {
        Debug.Log("GameOver");
        Time.timeScale = 0;
        _Instance = null;

        SceneManager.LoadSceneAsync("Ending", LoadSceneMode.Single);
        Destroy(gameObject);
    }
}

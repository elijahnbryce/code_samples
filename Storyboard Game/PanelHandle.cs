using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DialogueSequence;
using static Panel;

public class PanelHandle : MonoBehaviour
{
    public static PanelHandle _Instance;

    [Header("Panels")]
    public Image back;
    public Image fore;
    [SerializeField] private List<Panel> panels = new List<Panel>();
    private int inx = 0;
    [SerializeField] private Transform puzzleParent; 

    [Header("UI")]
    [SerializeField] private GameObject theButton;
    [SerializeField] private TextMeshProUGUI puzzlePrompt;
    private static SusHandler susH;

    [Header("Dialogue")]
    [SerializeField] private DialogueSequence sequence;
    public List<PuzzlingConversation> dialogueSequences;
    public int sequenceInx = 0;
    public bool wordPuzzle = false;
    public string currentMsg;


    private void Awake()
    {
        if (_Instance == null) 
            _Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        wordPuzzle = false;
        puzzleParent.GetComponent<FormSentence>().Switch(false);


        inx = 0;
        ShowPanel(panels[inx]);
        susH = SusHandler._Instance;
        susH.Pause();
        //theButton.SetActive(false);
    }

    private void Update()
    {
        // Debugging
        if (Input.GetKeyDown(KeyCode.D))
        {
            ChangePage();
        }
    }

    public void ChangePage(int change = 1)
    {
        if (!DialogueSystem._Instance.QueueEmpty())
            return;

        inx = Mathf.Min(inx + change, panels.Count - 1);

        ShowPanel(panels[inx]);
        if (inx == panels.Count - 1)
        {
            SceneManager.LoadScene(2);
            //quitButton.SetActive(true);
        }
    }

    public void ShowPanel(Panel p)
    {
        back.sprite = p.bg;

        dialogueSequences = p.convo;
        StartCoroutine(SpeakWhenSpokenTo());
    }

    public IEnumerator SpeakWhenSpokenTo()
    {
        Debug.Log(panels[inx].name + " " + dialogueSequences.Count);
        foreach (var seq in dialogueSequences)
        {
            wordPuzzle = seq.spawnsPuzzle;
            sequence.msgs = seq.dialogue;
            sequence.SendSequence();

            yield return new WaitUntil(() => DialogueSystem._Instance.QueueEmpty());
            yield return StartCoroutine(GoPuzzle());
        }
        ChangePage();
        yield break;
    }

    private IEnumerator GoPuzzle()
    {
        // Start Sentence Scrabble MiniGame
        puzzleParent.gameObject.SetActive(wordPuzzle);
        puzzleParent.GetComponent<FormSentence>().Switch(wordPuzzle);
        puzzlePrompt.text = currentMsg;
        susH.Resume();

        // Wait for terminate
        yield return new WaitWhile(() => wordPuzzle);
        
        susH.Pause();
        susH.IncrSpd();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}

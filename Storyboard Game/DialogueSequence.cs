using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSequence : MonoBehaviour
{
    private static DialogueSystem dm;

    public enum Speaker
    {
        Poe = 0,
        Officer1 = 1,
        Officer2 = 2
    }

    [System.Serializable] 
    public struct Dialogue
    {
        public Speaker name; public string description;
    }

    [Header("Conversation")]
    public List<Dialogue> msgs = new List<Dialogue>();

    private void Start()
    {
        dm = DialogueSystem._Instance;
    }
    public void SendSequence()
    {
        foreach (Dialogue msg in msgs)
        {
            ParseAndSend(msg);
        }
    }

    private void ParseAndSend(Dialogue msg)
    {
        switch (msg.name)
        {
            case Speaker.Poe:
                dm.EnqueDiag(dm.PoeSay(msg.description));
                break;
            case Speaker.Officer1:
                dm.EnqueDiag(dm.PawSay(msg.description)); 
                break;
            case Speaker.Officer2:
                dm.EnqueDiag(dm.BunSay(msg.description));
                break;
            default:
                Debug.Log("Can't determine sender: " + msg.name + ": " + msg.description);
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DialogueSequence;

[CreateAssetMenu]
public class Panel : ScriptableObject
{
    [System.Serializable]
    public struct PuzzlingConversation
    {
        // break sequences of conversations that have minigame afterwards
        public List<Dialogue> dialogue;
        public bool spawnsPuzzle;
    }

    public Sprite bg, fg;
    public List<PuzzlingConversation> convo; 
}

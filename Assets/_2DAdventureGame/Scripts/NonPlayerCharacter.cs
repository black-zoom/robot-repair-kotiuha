using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    public GameObject dialogueBubble;
    public string dialogueText;

    void Start()
    {
        dialogueBubble.SetActive(false);
    }
}

using UnityEngine;

public class NPCsDialogueSystem : MonoBehaviour
{
    [SerializeField] private GameObject DialogueMark;

    void Start()
    {
        DialogueMark.SetActive(true);
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueMark.SetActive(false);
            Debug.Log("Hi, this is me.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DialogueMark.SetActive(true);
        }
    }
}

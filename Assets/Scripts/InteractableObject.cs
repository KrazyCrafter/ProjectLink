using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string PlayerNeeded;
    public GameObject CreatedObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreatedObject.SetActive(false);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == PlayerNeeded)
        {
            CreatedObject.SetActive(true);
        }
    }
}

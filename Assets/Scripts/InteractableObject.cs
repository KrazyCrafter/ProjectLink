using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string PlayerNeeded;
    public GameObject CreatedObject;
    public GameObject CivilianIcon;

    //Temporary for concept testing
    public int CivilianCountNeeded;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(PlayerHealth.CiviliansSaved < CivilianCountNeeded)
        {
            CreatedObject.SetActive(false);
            CivilianIcon.SetActive(false);
        }
        else
        {
            CreatedObject.SetActive(true);
            CivilianIcon.SetActive(true);
        }
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == PlayerNeeded)
        {
            CreatedObject.SetActive(true);
            CivilianIcon.SetActive(true);
        }
    }
}

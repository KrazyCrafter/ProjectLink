using UnityEngine;

public class PrisonCell : MonoBehaviour
{
    [SerializeField] private AudioSource FoundSound;
    private bool BeenFound;
    [SerializeField] private int CellNumber;
    [SerializeField] private GameObject Released;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BeenFound = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TryOpen()
    {
        if(PlayerHealth.KeysFound < CellNumber)
        {
            if(!BeenFound)
            {
                FoundSound.Play();
                BeenFound = true;
            }
        }
        else
        {
            Instantiate(Released, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}

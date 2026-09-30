using UnityEngine;

public class Key : MonoBehaviour
{
    public int KeyID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(PlayerHealth.KeysFound >= KeyID)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

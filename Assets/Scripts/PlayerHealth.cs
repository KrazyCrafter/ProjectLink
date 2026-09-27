using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }
    [SerializeField] private int Health;
    [SerializeField] private int Level;
    public static int TimesDied;
    public bool isDead;

    void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Health = 3;
    }
    private void Respawn()
    {
        isDead = false;
        SceneManager.LoadScene(Level);
    }
    IEnumerator Death()
    {
        Debug.Log("Died");
        isDead = true;
        GetComponent<Grappler>().RemoveWeb();
        TimesDied++;
        yield return new WaitForSeconds(2);
        Respawn();
    }
    public void TakeDamage(int damage)
    {
        Health -= damage;
        if(Health <= 0)
        {
            StartCoroutine(Death());
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Threat")
        {
            TakeDamage(3);
        }
        else if (collision.gameObject.tag == "Altar")
        {
            if(Level < 6)
            {
                SceneManager.LoadScene(Level + 1);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}

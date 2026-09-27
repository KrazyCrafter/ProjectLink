using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int Health;
    [SerializeField] private int Level;
    public static int TimesDied;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Health = 3;
    }
    private void Respawn()
    {
        SceneManager.LoadScene(Level);
    }
    IEnumerator Death()
    {
        Debug.Log("Died");
        GetComponent<Grappler>().RemoveWeb();
        GetComponent<Grappler>().enabled = false;
        GetComponent<Movement>().enabled = false;
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

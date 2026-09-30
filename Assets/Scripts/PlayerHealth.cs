using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }
    [SerializeField] private int Health;
    [SerializeField] private int Level;

    public static int KeysFound = 0;
    public static int CiviliansSaved = 0;

    public static int TimesDied = 0;
    public bool isDead;
    public Image[] Hearts;

    public TextMeshProUGUI DeathCounter;

    void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Health = 3;
        if (TimesDied > 0)
        {
            DeathCounter.text = "Deaths: " + TimesDied;
        }
        else
        {
            DeathCounter.gameObject.SetActive(false);
        }
    }
    public void Respawn()
    {
        isDead = false;
        SceneManager.LoadScene(Level);
    }
    IEnumerator Death()
    {
        Debug.Log("Died");
        Movement.Instance.PlayAnimator();
        GetComponent<Grappler>().RemoveWeb();
        GetComponent<Grappler>().enabled = false;
        GetComponent<Movement>().enabled = false;
        TimesDied++;
        yield return new WaitForSeconds(2);
        Respawn();
    }
    public void TakeDamage(int damage)
    {
        if (!isDead)
        {
            Health -= damage;
            for (int i = 0; i < Hearts.Length; i++)
            {
                if (i < Health)
                {
                    Hearts[i].color = new Color(1, 1, 1, 1);
                }
                else
                {
                    Hearts[i].color = new Color(1, 1, 1, 0);
                }
            }
            if (Health <= 0)
            {
                isDead = true;
                StartCoroutine(Death());
            }
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
            MainMenuScript.LevelsBeaten = Mathf.Max(Level, MainMenuScript.LevelsBeaten);
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
    public void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.tag == "Key")
        {
            KeysFound = Mathf.Max(KeysFound, collider.gameObject.GetComponent<Key>().KeyID);
            Destroy(collider.gameObject);
            SceneManager.LoadScene(Level - 1);
        }
    }
}

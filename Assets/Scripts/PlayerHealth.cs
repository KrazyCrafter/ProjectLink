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

    public static int KeysFound;
    public static int CiviliansSaved;

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
        isDead = true;
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
        Health -= damage;
        for(int i = 0; i < Hearts.Length; i++)
        {
            if(i < Health)
            {
                Hearts[i].color = new Color(1, 1, 1, 1);
            }
            else
            {
                Hearts[i].color = new Color(1, 1, 1, 0);
            }
        }
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
}

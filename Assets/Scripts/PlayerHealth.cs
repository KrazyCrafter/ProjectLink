using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int Health;
    [SerializeField] private Vector2 SpawnPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnPoint = transform.position;
        Respawn();
    }
    private void Respawn()
    {
        transform.position = SpawnPoint;
        Health = 3;
    }
    IEnumerator Death()
    {
        Debug.Log("Died");
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
}

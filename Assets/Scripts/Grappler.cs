using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grappler : MonoBehaviour
{
    [SerializeField] private Transform grappleSpawn;
    [SerializeField] private GameObject webObject;
    private SpriteRenderer webRenderer;
    private Vector2 mousePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        webRenderer = webObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MousePosition(InputAction.CallbackContext context)
    {
        if(!context.Equals(null))
            mousePos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    public void Grapple(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            SpawnWeb();
        }
        else if(context.canceled)
        {
            RemoveWeb();
        }
    }

    private void SpawnWeb()
    {
        webObject.transform.position = grappleSpawn.position;
        webRenderer.enabled = true;
        StartCoroutine(StretchWeb());
    }

    private void RemoveWeb()
    {
        webRenderer.enabled = false;
        webObject.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
    }

    private IEnumerator StretchWeb()
    {
        while (webObject.transform.localScale.x < 10f)
        {
            Vector2 rotation = mousePos - new Vector2(webObject.transform.position.x, webObject.transform.position.y);
            webObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg);
            webObject.transform.localScale = webObject.transform.localScale
                                                 + new Vector3(0.25f, 0.0f, 0.0f);

            yield return null;
        }

        yield return true;
    }
}

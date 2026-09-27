using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Grappler : MonoBehaviour
{
    // [SerializeField] private Transform grappleSpawn;
    // [SerializeField] private GameObject webObject;
    // private SpriteRenderer webRenderer;

    private Rigidbody2D rb;
    private LineRenderer lr;
    private DistanceJoint2D dj;
    public bool isGrappling;
    [SerializeField] private LayerMask grappleLayer;
    private Vector2 mousePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // webRenderer = webObject.GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        dj = GetComponent<DistanceJoint2D>();
        lr.enabled = false;
        dj.enabled = false;
        isGrappling = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isGrappling)
        {
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, dj.connectedAnchor);
        }
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
            Debug.Log("Web");
            SpawnWeb();
        }
        else if(context.canceled)
        {
            Debug.Log("Cancel");
            RemoveWeb();
        }
    }

    private void SpawnWeb()
    {
        if(Physics2D.OverlapCircle(mousePos, 0.1f, grappleLayer))
        {
            isGrappling = true;
            lr.enabled = true;
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, mousePos);

            dj.enabled = true;
            dj.connectedAnchor = mousePos;
        }
        // webObject.transform.position = grappleSpawn.position;
        // webRenderer.enabled = true;
        // StartCoroutine(StretchWeb());
    }

    private void RemoveWeb()
    {
        isGrappling = false;
        lr.enabled = false;
        dj.enabled = false;
        // webRenderer.enabled = false;
        // webObject.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
    }

    private IEnumerator StretchWeb()
    {
        // while (webObject.transform.localScale.x < 10f)
        // {
        //     Vector2 rotation = mousePos - new Vector2(webObject.transform.position.x, webObject.transform.position.y);
        //     webObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg);
        //     webObject.transform.localScale = webObject.transform.localScale
        //                                          + new Vector3(0.25f, 0.0f, 0.0f);
        //     webObject.transform.position = grappleSpawn.position + new Vector3(rotation.x, rotation.y, 0.0f) * 0.5f;
        //     yield return null;
        // }

        yield return true;
    }
}

using UnityEngine;
using UnityEngine.InputSystem;

public class Grappler : MonoBehaviour
{
    private Vector2 mousePos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(mousePos);
    }

    public void MousePosition(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector2>();
    }

    public void Grapple(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            // Debug.Log("Grapple!");
        }
    }
}

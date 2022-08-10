using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float cameraSpeed = 100f;
    CharacterController controller;
    public GameObject camera;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        camera.transform.Rotate(new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * cameraSpeed);
        //clamp camera movement
        float z = camera.transform.eulerAngles.z;
        float x = camera.transform.eulerAngles.x;
        x = (x > 90 || x < -90) ? 0 : (Mathf.Clamp(x, -90, 90) - x);
        camera.transform.Rotate(x, 0, -z);

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            move += camera.transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            move += -camera.transform.forward;
        }
        

        controller.Move(move * Time.deltaTime * speed);
    }
}

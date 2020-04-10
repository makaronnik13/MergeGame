using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : Singleton<CameraController>
{

    public Camera UiCamera;
    private float needDepth = 8;

    private float needSize = 50;

	// Update is called once per frame
	void Update ()
    {
        //depth
        needSize -= Input.mouseScrollDelta.y*50f;
        needSize = Mathf.Clamp(needSize, 30f, 75f);
        needDepth -= Input.mouseScrollDelta.y * Time.deltaTime*5;
        needDepth = Mathf.Clamp(needDepth, 3,10);
        Vector3 cameraLocalPos = Camera.main.transform.localPosition;
        cameraLocalPos = new Vector3(cameraLocalPos.x, cameraLocalPos.y, -needDepth);
        Camera.main.transform.localPosition = Vector3.Lerp(Camera.main.transform.localPosition, cameraLocalPos, Time.deltaTime*3);

        //rotation
        if (Input.GetKey(KeyCode.Q))
        {
            transform.RotateAroundLocal(Vector3.up, Time.deltaTime*2);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.RotateAroundLocal(Vector3.up, -Time.deltaTime*2);
        }

        //movement
        transform.Translate(new Vector3(Input.GetAxis("Horizontal") * Time.deltaTime*5, 0, Input.GetAxis("Vertical")*5 * Time.deltaTime));
        // GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().m_Lens.FieldOfView = Mathf.Lerp(GetComponentInChildren<Cinemachine.CinemachineVirtualCamera>().m_Lens.FieldOfView, needSize, Time.deltaTime*3f);
        // GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.FieldOfView -= Input.mouseScrollDelta.y;
        // UiCamera.fieldOfView = GetComponent<Cinemachine.CinemachineVirtualCamera>().m_Lens.FieldOfView;

    }

	public void AimedBlockChanged (Block aim)
	{
       // GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = aim.transform;
        //GetComponent<Cinemachine.CinemachineVirtualCamera>().LookAt = aim.transform;
    }
}

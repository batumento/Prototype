using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    [SerializeField] GameObject cameraPoint;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void LateUpdate()
    {
        transform.position = new Vector3(cameraPoint.transform.position.x, cameraPoint.transform.position.y, cameraPoint.transform.position.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, cameraPoint.transform.rotation, 0.15f);//Kmaeranýn dönüþüne yumuþaklýk katýyorum
    }
}

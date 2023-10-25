using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class InteractSystem : MonoBehaviour
{
    [Header("Field of View")]
    public float viewRadius = 2.5f; // รัศมีของ Field of View
    [Range(0, 360)]
    public float viewAngle = 50f; // มุมของ Field of View
    public Transform eye; // Transform ที่แทนตาของ Player
    public GameObject Player;
    public GameObject CameraInHideObject1;
    public GameObject CameraInHideObject2;
    public ExitHide ExitFromHide;
    public bool Exit;
    public LayerMask visionObstructingLayer; // ใส่ Layer ของ Enemy , Layer ของสิ่งกีดขวาง และ Layer ของที่สามารถ Interact ได้

    // Start is called before the first frame update
    void Start()
    {
       
        Player.SetActive(true);
        CameraInHideObject1.SetActive(false);
        CameraInHideObject2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        DrawFieldOfView();
    }

    void DrawFieldOfView()
    {
        Vector3 origin = eye.position; // ตำแหน่งเริ่มต้นของ Field of View ที่ใช้ Transform ของตา

        for (float angle = -viewAngle / 2; angle < viewAngle / 2; angle += 1)
        {
            // สร้าง Ray สำหรับการตรวจสอบ Field of View จากตำแหน่งตาไปยังทิศทางที่กำหนดโดยมุม angle
            Vector3 direction = Quaternion.Euler(0, angle, 0) * eye.forward;

            // กำหนดจุดสิ้นสุดของ Ray ที่อยู่บนระหว่างตำแหน่งตาและ Field of View
            Vector3 end = origin + direction * viewRadius;

            RaycastHit ObstructingHit;

            // ตรวจสอบการชนของ Ray กับ Layer ของ Obstructing
            if (Physics.Linecast(origin, end, out ObstructingHit, visionObstructingLayer))
            {
                // ถ้าตรวจพบ Obstructing
                Debug.DrawLine(origin, ObstructingHit.point, Color.red);
                // ตรวจสอบการตรวจพบ Tag "Player" ในการชน
                if (ObstructingHit.collider.CompareTag("CanHide1") && Input.GetKeyDown(KeyCode.E))
                {

                    Player.SetActive(false);
                    CameraInHideObject1.SetActive(true);
                    Exit = ExitFromHide.CanExit = true;
                }
                if (ObstructingHit.collider.CompareTag("CanHide2") && Input.GetKeyDown(KeyCode.E))
                {

                    Player.SetActive(false);
                    CameraInHideObject2.SetActive(true);
                    Exit = ExitFromHide.CanExit = true;
                }

            }

            // ตรวจสอบการชนของ Ray กับ Layer ของ Player
            else
            {
                // ถ้าไม่มีการตรวจพบ Player, วาดเส้น Ray สีเขียว
                Debug.DrawRay(origin, direction * viewRadius, Color.green);
            }
        }

    }


    void OnDrawGizmos()
    {
        DrawFieldOfView(); // เรียกฟังก์ชัน DrawFieldOfView() เพื่อสร้างเส้น Field of View ในการดูแบบ Gizmos ในแอดเดร์
    }

}

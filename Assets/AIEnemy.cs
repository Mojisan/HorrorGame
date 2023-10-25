using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIEnemy : MonoBehaviour
{

    [Header("Field of View")]
    public float viewRadius = 10f; // รัศมีของ Field of View
    [Range(0, 360)]
    public float viewAngle = 90f; // มุมของ Field of View
    public bool playerDetected = false; // สถานะการตรวจพบ Player
    public Transform eye; // Transform ที่แทนตาของ Enemy
    public LayerMask visionLayer; // ใส่ Layer ของ Player และ Layer ของสิ่งกีดขวาง

    [Header("Patrol")]
    public NavMeshAgent navAgent; // คอมโพเนนต์สำหรับการเดินของ Enemy
    public List<Transform> PatrolPoints; // จุด Patrol ที่ Enemy จะเดินไป
    public Transform currentPatrol; // จุด Patrol ปัจจุบัน
    public float waitTime = 2.0f; // ระยะเวลาที่ต้องหยุดเดิน

    public float chaseSpeed = 5f; // ความเร็วในโหมดการไล่ล่า
    public float patrolSpeed = 3f; // ความเร็วในโหมดการไล่ล่า
    public Transform playerTransform; // ตัวแปรที่จะเก็บ Transform ของ Player


    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        // เริ่มต้นสุ่มจุด Patrol ที่จะไป
        PickRandomPatrol();
        
    }
    void Update()
    {
        if (playerDetected)
        {
            if (playerTransform != null) // ตรวจสอบว่า playerTransform ไม่เป็น null
            {
                navAgent.speed = chaseSpeed;
                navAgent.SetDestination(playerTransform.position); // ตั้งเป้าหมายเพื่อไล่ล่า Player
                Debug.Log("Chase");
            }
        }
        else if (!navAgent.pathPending && navAgent.remainingDistance < 0.1f)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                PickRandomPatrol();
            }
        }
        else if (!playerDetected)
        {
            // ถ้าไม่ตรวจพบ Player
            navAgent.speed = patrolSpeed; // เริ่มการเดินใหม่โดยตั้งความเร็วเป็นค่าปกติ
            navAgent.isStopped = false; // เริ่มการเดินหากถูกหยุด
        }

        DrawFieldOfView(); // เรียกฟังก์ชัน DrawFieldOfView() เพื่อสร้าง Field of View ในแต่ละเฟรม
    }


    void DrawFieldOfView()
    {
        Vector3 origin = eye.position; // ตำแหน่งเริ่มต้นของ Field of View ที่ใช้ Transform ของตา
        playerDetected = false; // เริ่มต้นตั้งค่า playerDetected เป็น false

        for (float angle = -viewAngle / 2; angle < viewAngle / 2; angle += 1)
        {
            // สร้าง Ray สำหรับการตรวจสอบ Field of View จากตำแหน่งตาไปยังทิศทางที่กำหนดโดยมุม angle
            Vector3 direction = Quaternion.Euler(0, angle, 0) * eye.forward;

            // กำหนดจุดสิ้นสุดของ Ray ที่อยู่บนระหว่างตำแหน่งตาและ Field of View
            Vector3 end = origin + direction * viewRadius;

            RaycastHit ObstructingHit;

            // ตรวจสอบการชนของ Ray กับ Layer ของ Obstructing
            if (Physics.Linecast(origin, end, out ObstructingHit, visionLayer))
            {
                // ถ้าตรวจพบ Obstructing
                Debug.DrawLine(origin, ObstructingHit.point, Color.red);
                // ตรวจสอบการตรวจพบ Tag "Player" ในการชน
                if (ObstructingHit.collider.CompareTag("Player"))
                {
                    Debug.Log("Hello"); // แสดงข้อความ "Hello" ในคอนโซล
                    playerDetected = true; // ตั้งค่าสถานะ playerDetected เป็น true
                    playerTransform = ObstructingHit.collider.transform; // เก็บ Transform ของ Player

                    // หมุนตาม Player
                    RotateTowardsPlayer(playerTransform);
                }

            }

            // ตรวจสอบการชนของ Ray กับ Layer ของ Player
            else
            {
                // ถ้าไม่มีการตรวจพบ Player, วาดเส้น Ray สีเขียว
                Debug.DrawRay(origin, direction * viewRadius, Color.green);
            }
        }

        // ตรวจสอบค่า playerDetected หลังจากที่ลูปเสร็จสิ้น
        if (!playerDetected)
        {
            // กระทำที่ต้องการเมื่อไม่ตรวจพบ Player
            // สามารถใส่โค้ดที่คุณต้องการให้มันทำที่นี่
        }
    }


    void OnDrawGizmos()
    {
        DrawFieldOfView(); // เรียกฟังก์ชัน DrawFieldOfView() เพื่อสร้าง Field of View ในการดูแบบ Gizmos ในแอดเดร์
    }

    void PickRandomPatrol()
    {
        if (PatrolPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, PatrolPoints.Count);
            currentPatrol = PatrolPoints[randomIndex];
            navAgent.SetDestination(currentPatrol.position);
            waitTime = 2.0f; // ระยะเวลาที่ต้องหยุดเดิน
        }
    }

    void RotateTowardsPlayer(Transform target)
    {
        // คำนวณการหมุนของ Enemy เพื่อมอง Player
        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(targetPosition);
    }
}

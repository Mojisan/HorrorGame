using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIEnemy : MonoBehaviour
{

    [Header("Field of View")]
    public float viewRadius = 10f; // ����բͧ Field of View
    [Range(0, 360)]
    public float viewAngle = 90f; // ����ͧ Field of View
    public bool playerDetected = false; // ʶҹС�õ�Ǩ�� Player
    public Transform eye; // Transform ���᷹�Ңͧ Enemy
    public LayerMask visionObstructingLayer; // ��� Layer �ͧ Player ��� Layer �ͧ��觡մ��ҧ

    [Header("Movement")]
    public NavMeshAgent navAgent; // �ҧ�Թ�ͧ Enemy
    public List<Transform> PatrolPoints; // �ش Patrol ��� Enemy ���Թ�
    public Transform currentPatrol; // �ش Patrol �Ѩ�غѹ
    public float waitTime = 2.0f; // �������ҷ���ͧ��ش�Թ

    public float chaseSpeed = 5f; // ����������������������
    public float patrolSpeed = 3f; // ��������������Ҵ���ǹ
    public Transform playerTransform; // ����÷����� Transform �ͧ Player


    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        // ������������ش Patrol �����
        PickRandomPatrol();
        
    }
    void Update()
    {
        if (playerDetected)
        {
            if (playerTransform != null) // ��Ǩ�ͺ��� playerTransform ����� null
            {
                navAgent.speed = chaseSpeed;
                navAgent.SetDestination(playerTransform.position); // �������������������� Player
                Debug.Log("Chase");
            }
        }
        else if (!navAgent.pathPending && navAgent.remainingDistance < 0.1f)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0) //��Ҷ֧���ش Patrol���������2�����ǡ������ش Patrol����
            {
                PickRandomPatrol();
            }
        }
        else if (!playerDetected)
        {
            // �������Ǩ�� Player
            navAgent.speed = patrolSpeed; // ���������Թ�����µ�駤��������繤�һ���
            navAgent.isStopped = false; // ���������Թ�ҡ�١��ش
        }

        DrawFieldOfView(); 
    }


    void DrawFieldOfView()
    {
        Vector3 origin = eye.position; // ���˹�������鹢ͧ Field of View ����� Transform �ͧ��

        for (float angle = -viewAngle / 2; angle < viewAngle / 2; angle += 1)
        {
            // ���ҧ Ray ����Ѻ��õ�Ǩ�ͺ Field of View �ҡ���˹觵���ѧ��ȷҧ����˹������ angle
            Vector3 direction = Quaternion.Euler(0, angle, 0) * eye.forward;

            // ��˹��ش����ش�ͧ Ray ������躹�����ҧ���˹觵���� Field of View
            Vector3 end = origin + direction * viewRadius;

            RaycastHit ObstructingHit;

            // ��Ǩ�ͺ��ê��ͧ Ray �Ѻ Layer �ͧ Obstructing
            if (Physics.Linecast(origin, end, out ObstructingHit, visionObstructingLayer))
            {
                // ��ҵ�Ǩ�� Obstructing
                Debug.DrawLine(origin, ObstructingHit.point, Color.red);
                // ��Ǩ�ͺ��õ�Ǩ�� Tag "Player" 㹡�ê�
                if (ObstructingHit.collider.CompareTag("Player"))
                {
                    Debug.Log("Hello"); // �ʴ���ͤ��� "Hello" 㹤͹��
                    playerDetected = true; // ��駤��ʶҹ� playerDetected �� true
                    playerTransform = ObstructingHit.collider.transform; // �� Transform �ͧ Player

                    // ��ع��� Player
                    RotateTowardsPlayer(playerTransform);
                }

            }

            // ��Ǩ�ͺ��ê��ͧ Ray �Ѻ Layer �ͧ Player
            else
            {
                // �������ա�õ�Ǩ�� Player, �Ҵ��� Ray ������
                Debug.DrawRay(origin, direction * viewRadius, Color.green);
            }
        }

    }


    void OnDrawGizmos()
    {
        DrawFieldOfView(); // ���¡�ѧ��ѹ DrawFieldOfView() �������ҧ��� Field of View 㹡�ô�Ẻ Gizmos ��ʹ���
    }

    void PickRandomPatrol()
    {
        if (PatrolPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, PatrolPoints.Count);
            currentPatrol = PatrolPoints[randomIndex];
            navAgent.SetDestination(currentPatrol.position);
            waitTime = 2.0f; // �������ҷ���ͧ��ش�Թ
        }
    }

    void RotateTowardsPlayer(Transform target)
    {
        // �ӹǳ�����ع�ͧ Enemy �����ͧ Player
        Vector3 targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(targetPosition);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class InteractSystem : MonoBehaviour
{
    [Header("Field of View")]
    public float viewRadius = 2.5f; // ����բͧ Field of View
    [Range(0, 360)]
    public float viewAngle = 50f; // ����ͧ Field of View
    public Transform eye; // Transform ���᷹�Ңͧ Player
    public GameObject Player;
    public GameObject CameraInHideObject1;
    public GameObject CameraInHideObject2;
    public ExitHide ExitFromHide;
    public bool Exit;
    public LayerMask visionObstructingLayer; // ��� Layer �ͧ Enemy , Layer �ͧ��觡մ��ҧ ��� Layer �ͧ�������ö Interact ��

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

}

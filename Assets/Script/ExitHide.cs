using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ExitHide : MonoBehaviour
{
    public List<GameObject> objectsToHide;
    public GameObject Player;
    public bool CanExit = false;

    void Update()
    {
        if (CanExit == true &&Input.GetKeyDown(KeyCode.E))
        {
            // ǹ�ٻ��ҹ��¡����лԴ����ͧ�������Ѻ�ء GameObject ���¡��
            foreach (GameObject obj in objectsToHide)
            {
                obj.SetActive(false);
            }
            Player.SetActive(true);
            CanExit = false;
        }
    }
}

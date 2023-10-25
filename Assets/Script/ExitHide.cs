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
            // วนลูปผ่านรายการและปิดการมองเห็นสำหรับทุก GameObject ในรายการ
            foreach (GameObject obj in objectsToHide)
            {
                obj.SetActive(false);
            }
            Player.SetActive(true);
            CanExit = false;
        }
    }
}

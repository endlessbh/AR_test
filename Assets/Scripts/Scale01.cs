using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//放大缩小功能
public class Scale01 : MonoBehaviour
{

    Vector2 oldPos1;
    Vector2 oldPos2;

    //private Vector3 initialPosition;

    // Use this for initialization
    void Start()
    {
        //initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 2)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                Vector2 temPos1 = Input.GetTouch(0).position;
                Vector2 temPos2 = Input.GetTouch(1).position;

                if (isEnLarge(oldPos1, oldPos2, temPos1, temPos2))
                {
                    float oldScale = transform.localScale.x;
                    //放大的倍数 如果要修改 列如:oldScale * 1.025f 修改成 oldScale * 1.25f 根据自己需求修改
                    float newScale = oldScale * 1.025f;

                    transform.localScale = new Vector3(newScale, newScale, newScale);
                }
                else
                {
                    float oldScale = transform.localScale.x;
                    //缩小的倍数 如果要修改 列如:oldScale / 1.025f 修改成 oldScale / 1.25f 根据自己需求修改
                    float newScale = oldScale / 1.025f;

                    transform.localScale = new Vector3(newScale, newScale, newScale);
                }

                oldPos1 = temPos1;
                oldPos2 = temPos2;

            }
        }
    }

    bool isEnLarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
    {

        float length1 = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
        float length2 = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));

        if (length1 < length2)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
}
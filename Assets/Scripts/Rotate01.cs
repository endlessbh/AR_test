using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate01 : MonoBehaviour
{

    //上一个手指的位置
    Vector3 PrevlousPosition;
    //手指的偏移值
    Vector3 Offset;

    //速度
    float xSpeed = 150f;
    float ySpeed = 150f;

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;
    }

    public void ResetPosition()
    {
        transform.position = initialPosition;
    }

    // Update is called once per frame
    void Update()
    {

        //如果触摸了屏幕
        if (Input.GetMouseButton(0))
        {
            //判断是几个手指触摸
            if (Input.touchCount == 1)
            {
                //第一个触摸到手指头 phase状态 Moved滑动
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    //根据你旋转的 模型物体 是要围绕哪一个轴旋转 Vector3.up是围绕Y旋转
                    transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * -xSpeed * Time.deltaTime);
                    //transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime);

                }
            }
        }
    }
}
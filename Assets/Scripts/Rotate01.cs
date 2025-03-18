using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate01 : MonoBehaviour
{

    //��һ����ָ��λ��
    Vector3 PrevlousPosition;
    //��ָ��ƫ��ֵ
    Vector3 Offset;

    //�ٶ�
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

        //�����������Ļ
        if (Input.GetMouseButton(0))
        {
            //�ж��Ǽ�����ָ����
            if (Input.touchCount == 1)
            {
                //��һ����������ָͷ phase״̬ Moved����
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    //��������ת�� ģ������ ��ҪΧ����һ������ת Vector3.up��Χ��Y��ת
                    transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * -xSpeed * Time.deltaTime);
                    //transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime);

                }
            }
        }
    }
}
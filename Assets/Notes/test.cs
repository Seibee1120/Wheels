using CircularScrollView;
using MGame.General;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public GameObject scroll;
     UINormalLoopScrollView uin;
    void Start()
    {
        uin = scroll.GetComponent<UINormalLoopScrollView>();
        // �󶨻ص��¼� ��ť�¼�
        uin.Init(callBack, CellClickCallBackFunc, "Button");
        // ������Ҫˢ�µ�����
        uin.ShowList(500);
    }
    void callBack(GameObject go, int id)
    {
        // ����ˢ��ui��ʾ
        Text text = go.transform.Find("Text").gameObject.GetComponent<Text>();

    }
    void CellClickCallBackFunc(GameObject go, int Index)
    {
        // ��������ť�¼�
        Debug.Log("-----�����-------------"+ Index);
    }
}

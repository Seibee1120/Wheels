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
        // 绑定回调事件 按钮事件
        uin.Init(callBack, CellClickCallBackFunc, "Button");
        // 设置需要刷新的数量
        uin.ShowList(500);
    }
    void callBack(GameObject go, int id)
    {
        // 这里刷新ui显示
        Text text = go.transform.Find("Text").gameObject.GetComponent<Text>();

    }
    void CellClickCallBackFunc(GameObject go, int Index)
    {
        // 这里点击按钮事件
        Debug.Log("-----点击了-------------"+ Index);
    }
}

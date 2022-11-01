using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;

public class GetSpriteByPrefab : MonoBehaviour
{
    
    [MenuItem("Tools/GetPrefabChildImageAtlas")]
    public static void GetAllImage()
    {
        GameObject go = Selection.activeGameObject;
        Image[] allImage = go.GetComponentsInChildren<Image>(true);
        List<string> nameList = new List<string>();
        if (allImage.Length > 0)
        {
            for (int i = 0; i < allImage.Length; i++)
            {
                if (allImage[i].sprite)
                {
                    //Debug.Log("========获取精灵图片的名字========="+ allImage[i].sprite.name);
                    Sprite spr =  allImage[i].sprite;
                    // var path = Directory.GetFiles(Application.dataPath, );
                    string path = AssetDatabase.GetAssetPath(spr);
                    if (!string.IsNullOrEmpty(path))
                    {
                        string[] AllparentName = path.Split('/');
                    //Debug.Log("===========获取精灵图片的路径===1================" + path);
                    //Debug.Log("===========获取精灵图片的路径==================="+ AllparentName.Length);
                        if (AllparentName.Length > 2)
                        {
                            string parName = AllparentName[AllparentName.Length - 1];
                            if (!nameList.Exists(e => e.EndsWith(parName)))
                            {
                                 nameList.Add(parName);                           
                                 Debug.Log("======关联图集名称======" + parName);
                            }
                        }

                    }

                }
            }
        }
    }
    [MenuItem("Tools/GetPrefabChildSprite")]
    public static void GetAllImageSprite()
    {
        GameObject go = Selection.activeGameObject;
        Image[] allImage = go.GetComponentsInChildren<Image>(true);
        List<string> nameList = new List<string>();
        if (allImage.Length > 0)
        {
            for (int i = 0; i < allImage.Length; i++)
            {
                Sprite spr = allImage[i].sprite;
                string path = AssetDatabase.GetAssetPath(spr);
                if (!string.IsNullOrEmpty(path))
                {
                    string[] AllparentName = path.Split('/');
                    if (AllparentName.Length > 2)
                    {
                        string parName = AllparentName[AllparentName.Length - 1];
                        Debug.Log("======关联图集名称======" + parName+"=====当前图的名字======"+ allImage[i].sprite.name+"========当前图片在预制体中的位置========"+ allImage[i].name);
                    }
                }
             }
        }
    }
}

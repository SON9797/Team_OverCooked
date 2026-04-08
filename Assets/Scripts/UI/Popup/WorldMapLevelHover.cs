using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class ImageList
{
    public string key;
    public Sprite image;
}
public class WorldMapLevelHover : MonoBehaviour
{
    [SerializeField] int starcount;
    [SerializeField] List<ImageList> imageList;

    Dictionary<string, Sprite> imageDict;
    void Start()
    {
        for(int i = 0; i < imageList.Count; i++)
        {
            imageDict[imageList[i].key]=imageList[i].image;
        }
    }

    void Update()
    {
        
    }
}

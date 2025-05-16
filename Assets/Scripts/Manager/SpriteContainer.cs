using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapSprites
{
    public Sprite background;
    public Sprite[] tiles, decos, walls, filterwalls, objs,objbases, noneobjs,filters, stamps;
}

public class SpriteContainer : MonoBehaviour
{
    private SpriteContainer(){}
    private static SpriteContainer instance;
    public static SpriteContainer Instance => instance;
    public MapSprites[] mapSpritesArray;
    
    void Awake()
    {
        if(instance==null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

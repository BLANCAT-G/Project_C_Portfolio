using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixed_BlackHole : IObject
{
    public Sprite blackSprite;
    public override void Awake()
    {
        moveLog = new Stack<ObjData>();
        spriter = GetComponent<SpriteRenderer>();
    }

    public override void Interaction()
    {
        List<GameObject> coll = MapManager.Instance.gameGrid[objPos.x, objPos.y];
        foreach (GameObject c in coll)
        {
            if (!c.activeSelf) continue;
            if (c.gameObject == this.gameObject)
                continue;
            IObject io = c.gameObject.GetComponent<IObject>();
            if (io.Type != ObjType.BlackHole && io.Type != ObjType.Fixed_BlackHole && io.isBlack != isBlack) continue;
            ObjType objType = io.Type;
            ColorType objColor = io.colorType;
            switch (objType)
            {
                case ObjType.BlackHole:
                    CompleteInteract(io);
                    gameObject.SetActive(false);
                    break;
                default:
                    if (io.is3D)
                    {
                        if(isBlack) io.MoveToColorWorld();
                        else io.MoveToBlackWorld();
                        io.UpdateColor();
                    }
                    
                    
                    break;
            }
        }
    }
    
    public override void ToBlackColor()
    {
        spriter.sprite = blackSprite;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roller : IObject
{
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
            if (c.gameObject == this.gameObject) continue;
            
            IObject io = c.gameObject.GetComponent<IObject>();
            if (!GameManager.Instance.isPlayerBlack && (io.isBlack || isBlack)) continue;
            ObjType objType = c.gameObject.GetComponent<IObject>().Type;
            
            switch (objType)
            {
                case ObjType.BlackHole: case ObjType.Fixed_BlackHole:
                    break;
                default:
                    EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType); SoundBox.instance.PlaySFX("Interact");
                    c.GetComponent<IObject>().OnAlpha();
                    break;
            }
            
            
        }
        UpdateColor();
    }
}

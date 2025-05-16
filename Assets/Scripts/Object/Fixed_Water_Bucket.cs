using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixed_Water_Bucket : IObject
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
            if (c.gameObject == gameObject)
                continue;
            IObject io = c.gameObject.GetComponent<IObject>();
            if (io.isAlpha != this.isAlpha) continue;
            ObjType objType = c.gameObject.GetComponent<IObject>().Type;
            ColorType objColor = c.gameObject.GetComponent<IObject>().colorType;
            switch (objType)
            {
                case ObjType.Paint: case ObjType.Water_Bucket: case ObjType.SandColor: case ObjType.TrailBrush: case ObjType.Mop:
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform);
                    CompleteInteract(io);
                    break;
                case ObjType.Acryl:
                    if (objColor != ColorType.None)
                    {
                        io.ColorChange(ColorType.None);
                        EffectManager.Instance.ExecuteEffect(EffectType.Pond, transform, colorType); SoundBox.instance.PlaySFX("ColorRelease");
                    }
                    break;
                default:
                    break;
            }
            
        }
    }
}

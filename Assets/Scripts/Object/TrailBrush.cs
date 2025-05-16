using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailBrush : IObject
{
    public override void Awake()
    {
        moveLog = new Stack<ObjData>();
        spriter=GetComponent<SpriteRenderer>();
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
            if (io.isAlpha != this.isAlpha) continue;
            ObjType objType = io.Type;
            ColorType objColor = io.colorType;
            switch (objType)
            {
                case ObjType.Player:
                    break;
                case ObjType.Fixed_Paint:
                    break;
                case ObjType.BlackSmoke:
                    break;
                case ObjType.Eraser:
                    break;
                case ObjType.Tile:
                    break;
                case ObjType.Easel:
                    break;
                case ObjType.Fixed_Water_Bucket:
                    break;
                //물감처리 case color//sand
                case ObjType.TrailBrush:
                    if (this.GetInstanceID() < io.gameObject.GetInstanceID())
                        break;
                    gameObject.SetActive(false);
                    EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                    break;
                case ObjType.Acryl:
                    isAcryl = true;
                    if (objColor != ColorType.None)
                    {
                        isSuperAcryl = true; 
                        acrylColor = objColor;
                        AcrylEff.gameObject.GetComponent<SpriteRenderer>().color = acrylColor.ToColor();
                    }
                    AcrylEff.SetActive(true);
                    EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType); SoundBox.instance.PlaySFX("Interact");
                    CompleteInteract(io);
                    break;
                default:
                    if (isAcryl && objType == ObjType.WoodHammer)
                        break;
                    gameObject.SetActive(false);
                    EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                    CompleteInteract(io);
                    break;
            }
        }
        StateUpdate();
    }
}

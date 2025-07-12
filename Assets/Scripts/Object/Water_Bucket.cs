using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water_Bucket : IObject
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
            if (c.gameObject == this.gameObject)
                continue;
            IObject io = c.gameObject.GetComponent<IObject>();
            if (io.isAlpha != this.isAlpha) continue;
            if (!GameManager.Instance.isPlayerBlack && (io.isBlack || isBlack)) continue;
            ObjType objType = c.gameObject.GetComponent<IObject>().Type;
            ColorType objColor = c.gameObject.GetComponent<IObject>().colorType;
            switch (objType)
            {
                case ObjType.Player:
                case ObjType.BlackSmoke:
                case ObjType.Eraser:
                case ObjType.Brush:
                case ObjType.Easel:
                case ObjType.InkStone: case ObjType.Roller:
                case ObjType.BlackHole: case ObjType.Fixed_BlackHole:
                    break;
                case ObjType.Acryl:
                    if (objColor == ColorType.None)
                    {
                        isAcryl = true;
                        AcrylEff.SetActive(true);
                        EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType); SoundBox.instance.PlaySFX("Interact");
                        CompleteInteract(io);
                    }                   
                    
                    break;
                case ObjType.Tile:
                    CompleteInteract(io);
                    gameObject.SetActive(false);
                    EffectManager.Instance.ExecuteEffect(EffectType.Pond,transform);
                    break;
                //case ObjType.Sponge:
                //    CompleteInteract(io);
                //    EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                //    gameObject.SetActive(false);
                //    break;
                case ObjType.Water_Bucket:
                    if (this.GetInstanceID() < io.gameObject.GetInstanceID())
                        break;
                    gameObject.SetActive(false);
                    EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform);
                    break;
                case ObjType.Fixed_Paint:
                    break;
                default:
                    if (isAcryl && objType == ObjType.WoodHammer)
                        break;
                    EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                    CompleteInteract(io);
                    gameObject.SetActive(false);
                    break;
            }
            
        }
        UpdateColor();
        StateUpdate();
    }
}

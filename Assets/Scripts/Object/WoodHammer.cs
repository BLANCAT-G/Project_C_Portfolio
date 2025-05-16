using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class WoodHammer : IObject
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
            ObjType objType = io.Type;
            ColorType objColor = io.colorType;
            switch (objType)
            {
                case ObjType.Tile:
                    break;
                case ObjType.BlackSmoke:
                    break;
                case ObjType.Eraser:
                    break;
                case ObjType.Player:
                    break;
                case ObjType.WoodHammer:
                    if (this.GetInstanceID() < io.gameObject.GetInstanceID())
                        break;
                    EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform);
                    gameObject.SetActive(false);
                    break;
                case ObjType.Fixed_Paint:
                    break;
                default:
                    if(io.isAcryl)
                    {
                        io.isAcryl = false;
                        io.AcrylEff.SetActive(false);
                        io.AcrylEff.gameObject.GetComponent<SpriteRenderer>().color = new Color(221f / 255f, 221f / 255f, 221f / 255f);
                        EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType); SoundBox.instance.PlaySFX("Interact");
                        SoundBox.instance.PlaySFX("InterRelease");
                        gameObject.SetActive(false);
                        RefreshInteractObject();
                        io.RefreshInteractObject();
                        break;
                    }
                    gameObject.SetActive(false);
                    EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform);
                    CompleteInteract(io);
                    break;
            }
        }
    }
    
}

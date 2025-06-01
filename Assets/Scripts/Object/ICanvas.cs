using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ICanvas : IObject
{
    [SerializeField]
    private InGameController inGameController;
    private void Start()
    {
        inGameController = FindObjectOfType<InGameController>();
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
            ObjType objType = c.gameObject.GetComponent<IObject>().Type;
            ColorType objColor = c.gameObject.GetComponent<IObject>().colorType;
            switch (objType)
            {
                case ObjType.Tile:
                    break;
                case ObjType.Easel:
                    break;
                case ObjType.Paint:
                    if (isBrush)
                    {
                        colorType = PCHManager.MixColor(colorType, objColor);
                        isBrush = false;
                    }
                    else
                    {
                        colorType = objColor;
                    }
                    ColorChange(colorType);
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    CompleteInteract(io);
                    break;
                case ObjType.Fixed_Paint:
                    if (isBrush)
                    {
                        colorType = PCHManager.MixColor(colorType, objColor);
                        isBrush = false;
                    }
                    else
                    {
                        colorType = objColor;
                    }
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    RefreshInteractObject();
                    ColorChange(colorType);
                    break;
                case ObjType.Brush:
                    if (objColor != ColorType.None)
                    {
                        colorType = PCHManager.MixColor(colorType, objColor);
                        ColorChange(colorType);
                        EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                        CompleteInteract(io);
                    }
                    else
                    {
                        CompleteInteract(io);
                        EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                    }
                    break;
                case ObjType.Water_Bucket:
                    colorType = ColorType.White;
                    ColorChange(colorType);
                    colorType = ColorType.None;
                    EffectManager.Instance.ExecuteEffect(EffectType.Pond, transform);  SoundBox.instance.PlaySFX("ColorRelease");
                    CompleteInteract(io);
                    break;
                case ObjType.Fixed_Water_Bucket:
                    colorType = ColorType.White;
                    ColorChange(colorType);
                    colorType = ColorType.None;
                    RefreshInteractObject();
                    EffectManager.Instance.ExecuteEffect(EffectType.Pond, transform);  SoundBox.instance.PlaySFX("ColorRelease");
                    break;
                case ObjType.Player:
                    if(colorType==objColor)
                    {
                        EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType); SoundBox.instance.PlaySFX("Interact");
                        inGameController.Win(colorType);
                        Destroy(GameManager.Instance.gameObject);
                        Destroy(MapManager.Instance.gameObject);
                    }
                    break;
                case ObjType.Canvas:
                    colorType = PCHManager.MixColor(colorType, objColor);
                    ColorChange(colorType);
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    CompleteInteract(io);
                    break;
                case ObjType.Acryl:
                    isAcryl = true;
                    if (objColor != ColorType.None)
                    {
                        isSuperAcryl = true;
                        colorType = objColor;
                        acrylColor = objColor;
                        ColorChange(colorType);
                        AcrylEff.gameObject.GetComponent<SpriteRenderer>().color = acrylColor.ToColor();
                    }
                    AcrylEff.SetActive(true);
                    EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType); SoundBox.instance.PlaySFX("Interact");
                    CompleteInteract(io);
                    break;
                case ObjType.SandColor:
                    if (isBrush)
                    {
                        colorType = PCHManager.MixColor(colorType, objColor);
                        isBrush = false;
                    }
                    else
                    {
                        colorType = objColor;
                    }
                    prevColor = colorType;
                    ColorChange(colorType);
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    SandColor sc = io as SandColor;
                    sandCount = sc.count;
                    sandCountText.gameObject.SetActive(true);
                    CompleteInteract(io);
                    break;
                case ObjType.Sponge:
                    colorType = PCHManager.SubstractColor(colorType, objColor);
                    ColorChange(colorType);
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    CompleteInteract(io);
                    break;
                default:
                    if (isAcryl && objType == ObjType.WoodHammer)
                        break;
                    CompleteInteract(io);
                    EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform);
                    gameObject.SetActive(false);
                    break;
            }
            
        }
        StateUpdate();
    }
    
    public override bool MoveCheck(Vector3 dir)
    {
        if (isMoving) return false;
        int dx = (int)dir.x, dy = (int)dir.y;
        if (!CheckInGrid(new GridPos(objPos.x + dx * 2, objPos.y + dy * 2))) return false;
        if (MapManager.Instance.wallGrid[objPos.x + dx * 2, objPos.y + dy * 2]) return false;
        SetHitObjects(dir);
        if (hitFwall) return false;
        if (hitPalette && hitPalette.activeSelf) return false;
        else if (hitFilter)
        {
            Filter filter = hitFilter.GetComponent<Filter>();
            if (filter && ((filter.isOneTime && filter.used) || (filter.filterType==FilterType.None && (filter.colorType != colorType || filter.isAlpha!=isAlpha))))
            {
                return false;
            }
            
        }
        else if (hitOppositeFilter)
        {
            return false;
        }

        foreach (GameObject h in hit)
        {
            if (!h.activeSelf) continue;
            if (h.CompareTag("Wall"))
                return false;
            IObject obj = h.gameObject.GetComponent<IObject>();
            if (obj.isAlpha==isAlpha && obj.Type == ObjType.Tile && obj.colorType != colorType)
                return false;
            if (obj.is3D && (obj.Type!=ObjType.WoodHammer) && (isAcryl || obj.isAcryl) && obj.isAlpha==isAlpha)
            {
                if (obj.MoveCheck(dir) == false)
                    return false;
            }
        }

        return true;
    }
}

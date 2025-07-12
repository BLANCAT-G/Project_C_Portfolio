using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Sponge : IObject
{
    private Color baseColor=new Color(239f / 255f, 210f / 255f, 147f / 255f); 
   
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
            ObjType objType = io.Type;
            ColorType objColor = io.colorType;
            switch (objType)
            {
                case ObjType.Player:
                case ObjType.BlackSmoke:
                case ObjType.SandColor:
                case ObjType.Tile:
                case ObjType.Easel:
                case ObjType.InkStone: case ObjType.Roller:
                case ObjType.BlackHole: case ObjType.Fixed_BlackHole:
                    break;
                case ObjType.Sponge:
                    if (colorType != ColorType.None && objColor == ColorType.None)
                    {
                        CompleteInteract(io);
                        EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                    }
                    else if (colorType == ColorType.None && objColor != ColorType.None)
                    {
                        break;
                    }
                    else if (colorType != objColor && (colorType != ColorType.None && objColor != ColorType.None))
                    {
                        if (this.GetInstanceID() < io.gameObject.GetInstanceID())
                            break;
                        colorType = PCHManager.MixColor(colorType, objColor);
                        ColorChange(colorType);
                        EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                        CompleteInteract(io);
                    }
                    else
                    {
                        if (this.GetInstanceID() < io.gameObject.GetInstanceID())
                            break;
                        EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                        gameObject.SetActive(false);
                    }
                    break;
                case ObjType.Eraser:
                    break;
                case ObjType.Paint:
                    if(colorType == ColorType.None)
                    {
                        colorType = objColor;
                        ColorChange(colorType);
                        EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                        CompleteInteract(io);
                    }
                    else if (colorType == objColor)
                    {
                        CompleteInteract(io);
                        gameObject.SetActive(false);
                        EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                    }
                    else
                    {
                        io.colorType = PCHManager.SubstractColor(objColor, colorType);
                        io.ColorChange(io.colorType);
                        gameObject.SetActive(false);
                        EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    }
                    break;
                case ObjType.Fixed_Paint:
                    if (colorType != ColorType.None)
                    {
                        colorType = PCHManager.MixColor(colorType, objColor);
                    }
                    else
                        colorType = objColor;
                    ColorChange(colorType);
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    RefreshInteractObject();
                    break;
                case ObjType.Acryl:
                    if (io.isSuperAcryl)
                        break;
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
                case ObjType.Water_Bucket:
                    colorType = ColorType.None;
                    spriter.color = baseColor;
                    EffectManager.Instance.ExecuteEffect(EffectType.Pond, transform);  SoundBox.instance.PlaySFX("ColorRelease");
                    break;
                case ObjType.Fixed_Water_Bucket:
                    colorType = ColorType.None;
                    spriter.color = baseColor;
                    EffectManager.Instance.ExecuteEffect(EffectType.Pond, transform);  SoundBox.instance.PlaySFX("ColorRelease");
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
        UpdateColor();
        StateUpdate();
    }

    public override void ColorChange(ColorType cT)
    {
        colorType=cT;
        if (isSuperAcryl)
        {
            if (cT == ColorType.None)
            {
                isSuperAcryl = false;
                AcrylEff.gameObject.GetComponent<SpriteRenderer>().color = new Color(221f/255f, 221f / 255f, 221f / 255f, 1);
            }
            else
            {
                acrylColor = cT;
                AcrylEff.gameObject.GetComponent<SpriteRenderer>().color = acrylColor.ToColor();
            }
        }
        if (cT == ColorType.None) spriter.color = baseColor;
        else spriter.color = cT.ToColor();
        
        if(isAlpha) OnAlpha();
        else OffAlpha();
    }

    public override void ToBlackColor()
    {
        if (isSuperAcryl)
        {
            AcrylEff.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
        }
        if (colorType== ColorType.None) spriter.color = baseColor;
        else spriter.color = Color.black;
        
        if(isAlpha) OnAlpha();
        else OffAlpha();
    }
}

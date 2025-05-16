using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Easel : IObject
{
    public bool colorSaved = false;
    public Sprite[] easelSprites;
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
            ObjType objType = c.gameObject.GetComponent<IObject>().Type;
            ColorType objColor = c.gameObject.GetComponent<IObject>().colorType;
            switch (objType)
            {
                case ObjType.TrailBrush: case ObjType.Mop:
                    break;
                case ObjType.Acryl:
                    if (colorSaved)
                    {
                        io.ColorChange(colorType);
                        gameObject.SetActive(false);
                        EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                    }
                    break;
                case ObjType.Canvas:
                    if (colorSaved)
                    {
                        io.ColorChange(colorType);
                        gameObject.SetActive(false);
                        EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                    }
                    break;
                case ObjType.SandColor: case ObjType.Paint:
                    if (colorSaved)
                    {
                        io.ColorChange(colorType);
                        gameObject.SetActive(false);
                        EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                    }
                    else
                    {
                        ColorChange(objColor);
                        colorSaved = true;
                        EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform,colorType);
                        CompleteInteract(io);
                    }
                    break;
                case ObjType.Brush:
                    if (colorSaved)
                    {
                        if (objColor == ColorType.None) io.ColorChange(colorType);
                        else io.ColorChange(PCHManager.MixColor(objColor, colorType)); 
                        gameObject.SetActive(false);
                        EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                    }
                    else
                    {
                        if (objColor != ColorType.None)
                        {
                            ColorChange(objColor);
                            colorSaved = true;
                        }
                        EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform,colorType);
                        CompleteInteract(io);
                    }
                    break;
                case ObjType.Sponge:
                    if (colorSaved)
                    {
                        if (objColor == ColorType.None)
                        {
                            io.ColorChange(colorType);
                            gameObject.SetActive(false);
                            EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                        }
                        else
                        {
                            ColorChange(PCHManager.SubstractColor(colorType,objColor));
                            EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform,colorType);
                            CompleteInteract(io);
                        }
                    }
                    else
                    {
                        EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                        CompleteInteract(io);
                    }
                    break;
                case ObjType.Water_Bucket:
                    if (colorSaved)
                    {
                        ColorChange(ColorType.None);
                        colorSaved = false;
                    }
                    EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                    CompleteInteract(io);
                    break;
                default:
                    break;
            }
            
        }
    }

    public override void ColorChange(ColorType cT)
    {
        colorType = cT;
        if (cT == ColorType.None)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = easelSprites[0];
            transform.Find("Base").gameObject.SetActive(false);
            colorSaved = false;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = easelSprites[1];
            transform.Find("Base").gameObject.SetActive(true);
            spriter.color = cT.ToColor();
            colorSaved = true;
        }
        
        if(isAlpha) OnAlpha();
        else OffAlpha();
    }
}

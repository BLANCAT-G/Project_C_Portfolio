using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandColor : IObject
{
    public uint count;
    
    public override void Awake()
    {
        moveLog = new Stack<ObjData>();
        spriter = gameObject.transform.Find("Base").GetComponent<SpriteRenderer>();
        ColorChange(colorType);
    }

    public void Start()
    {
        sandCountText.gameObject.SetActive(true);
        sandCountText.text = count.ToString();
    }

    public override void Interaction()
    {
        Collider2D[] coll=Physics2D.OverlapBoxAll(transform.position, Vector2.zero, 0); 
        foreach (Collider2D c in coll)
        {
            if (c.gameObject == this.gameObject)
                continue;
            IObject io = c.gameObject.GetComponent<IObject>();
            ObjType objType = c.gameObject.GetComponent<IObject>().Type;
            ColorType objColor = c.gameObject.GetComponent<IObject>().colorType;
            switch (objType)
            {
                case ObjType.Player:
                case ObjType.BlackSmoke:
                case ObjType.Eraser:
                case ObjType.Tile:
                    break;
                case ObjType.Sponge:
                    if (objColor == ColorType.None)
                    {
                        EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType);
                        CompleteInteract(io);
                        break;
                    }
                    colorType = PCHManager.SubstractColor(colorType, objColor);
                    ColorChange(colorType);
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    CompleteInteract(io);
                    break;
                case ObjType.Paint:
                    colorType = PCHManager.MixColor(colorType, objColor);
                    ColorChange(colorType);
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    CompleteInteract(io);
                    break;
                case ObjType.SandColor:
                    if (this.GetInstanceID() < io.gameObject.GetInstanceID())
                    {
                        break;
                    }
                    SandColor sc = io as SandColor;
                    colorType = PCHManager.MixColor(colorType, objColor);
                    ColorChange(colorType);
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    sandCount = (uint)Mathf.Max((int)sandCount, (int)sc.sandCount);
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
                    ColorChange(colorType);
                    break;
                case ObjType.Brush:
                    if(objColor!=ColorType.None)
                    {
                        colorType = PCHManager.MixColor(colorType, objColor);
                        ColorChange(colorType);
                        EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                        CompleteInteract(io);
                    }
                    break;
                default:
                    if (isAcryl && objType == ObjType.WoodHammer)
                        break;
                    gameObject.SetActive(false);
                    CompleteInteract(io);
                    EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType);
                    break;
            }
            
        }
        StateUpdate();
    }
}

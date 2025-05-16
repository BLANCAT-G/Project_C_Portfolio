using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brush : IObject
{
    public Sprite[] brushSprites;
    private Animator animator;
    public override void Awake()
    {
        base.Awake();

        if (colorType == ColorType.None)
            spriter.gameObject.SetActive(false);
    }
    public override void Interaction()
    {
        List<GameObject> coll = MapManager.Instance.gameGrid[objPos.x, objPos.y];
        foreach (GameObject c in coll)
        {
            if (!c.activeSelf) continue;
            if (c.gameObject == this.gameObject) continue;
            IObject io = c.gameObject.GetComponent<IObject>();
            if (io.isAlpha != this.isAlpha) continue;
            ObjType objType = c.gameObject.GetComponent<IObject>().Type;
            ColorType objColor = c.gameObject.GetComponent<IObject>().colorType;
            switch (objType)
            {
                case ObjType.Player:
                case ObjType.Canvas:
                case ObjType.BlackSmoke:
                    break;
                case ObjType.Eraser:
                    break;
                case ObjType.Easel:
                    break;
                case ObjType.Brush:
                    if(colorType!=ColorType.None&& objColor== ColorType.None)
                    {
                        EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                        CompleteInteract(io);
                    }
                    else if (colorType == ColorType.None && objColor != ColorType.None)
                    {
                        break;
                    }
                    else if(colorType != objColor&&(colorType != ColorType.None && objColor != ColorType.None))
                    {
                        if (this.GetInstanceID() < io.gameObject.GetInstanceID())
                            break;
                        colorType = PCHManager.MixColor(colorType, objColor);
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
                case ObjType.Paint:
                    if (colorType != ColorType.None)
                        break;                   
                    colorType = objColor;
                    spriter.gameObject.SetActive(true);
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    CompleteInteract(io);
                    break;
                case ObjType.Fixed_Paint:
                    if (colorType != ColorType.None)
                    {
                        colorType = PCHManager.MixColor(colorType, objColor);
                    }
                    else
                        colorType = objColor;
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    spriter.gameObject.SetActive(true);
                    break;
                case ObjType.Acryl:
                    if (objColor != ColorType.None)
                        break;
                    isAcryl = true;
                    AcrylEff.SetActive(true);
                    //if (objColor != ColorType.None)
                    //{
                    //    isSuperAcryl = true;
                    //    colorType = objColor;
                    //    acrylColor = objColor;
                    //    AcrylEff.gameObject.GetComponent<SpriteRenderer>().color = acrylColor.ToColor();
                    //}
                    EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType); SoundBox.instance.PlaySFX("Interact");
                    CompleteInteract(io);
                    break;
                case ObjType.Water_Bucket:
                    colorType = ColorType.None;
                    spriter.gameObject.SetActive(false);
                    EffectManager.Instance.ExecuteEffect(EffectType.Pond, transform);  SoundBox.instance.PlaySFX("ColorRelease");
                    CompleteInteract(io);
                    break;
                case ObjType.Fixed_Water_Bucket:
                    colorType = ColorType.None;
                    spriter.gameObject.SetActive(false);
                    EffectManager.Instance.ExecuteEffect(EffectType.Pond, transform);  SoundBox.instance.PlaySFX("ColorRelease");
                    break;
                case ObjType.SandColor:
                    if(colorType==ColorType.None)
                    {
                        prevColor = colorType;
                        colorType = objColor;
                        EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                        SandColor sc = io as SandColor;
                        sandCount = sc.count;
                        sandCountText.gameObject.SetActive(true);
                        sandCountText.text = sandCount.ToString();
                        CompleteInteract(io);
                        break;
                    }
                    break;
                case ObjType.Tile:
                    break;
                default:
                    if (isAcryl && objType == ObjType.WoodHammer)
                        break;
                    gameObject.SetActive(false);
                    EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform);
                    CompleteInteract(io);
                    break;

            }

        }
        ColorChange(colorType);
        StateUpdate();
    }
    public override void ColorChange(ColorType cT)
    {
        colorType = cT;
        animator = GetComponent<Animator>();
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
        if (cT == ColorType.None)
        {
            animator.SetBool("isNone",true);
            transform.Find("Base").gameObject.SetActive(false);
        }
        else
        {
            animator.SetBool("isNone",false);
            transform.Find("Base").gameObject.SetActive(true);
            spriter.color = cT.ToColor();
        }
    }
}

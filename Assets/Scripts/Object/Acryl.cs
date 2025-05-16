using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Acryl : IObject
{
    private Color baseColor;
    public override void Awake()
    {
        base.Awake();
        baseColor= new Color(75f / 255f, 75f / 255f, 75f / 255f);
        spriter.color = baseColor;
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
            ObjType objType = c.gameObject.GetComponent<IObject>().Type;
            ColorType objColor = c.gameObject.GetComponent<IObject>().colorType;
            switch (objType)
            {
                case ObjType.Tile:
                    break;
                case ObjType.Easel:
                    break;
                case ObjType.Fixed_Water_Bucket:
                    break;
                case ObjType.Paint:
                    colorType = objColor;
                    ColorChange(colorType);
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    c.gameObject.SetActive(false);
                    break;
                case ObjType.Fixed_Paint:
                    colorType = objColor;
                    ColorChange(colorType);
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    break;
                case ObjType.Water_Bucket:
                    if (colorType != ColorType.None)
                    {
                        colorType= ColorType.None;
                        spriter.color = baseColor;
                        EffectManager.Instance.ExecuteEffect(EffectType.Pond, transform);  SoundBox.instance.PlaySFX("ColorRelease");
                        CompleteInteract(io);
                    }
                    break;
                case ObjType.Acryl:
                    if(isSuperAcryl&&!io.isSuperAcryl)
                    {
                        EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform); SoundBox.instance.PlaySFX("InterVanish");
                        CompleteInteract(io);
                        break;
                    }
                    else if(!isSuperAcryl&&io.isSuperAcryl)
                    {
                        break;
                    }

                    if (this.GetInstanceID() < io.gameObject.GetInstanceID())
                        break;
                    if(isSuperAcryl&&io.isSuperAcryl)
                    {
                        colorType = PCHManager.MixColor(colorType, objColor);
                        ColorChange(colorType);
                        EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                        CompleteInteract(io);
                        break;
                    }
                    EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                    gameObject.SetActive(false);
                    break;
                case ObjType.SandColor:
                    prevColor = colorType;
                    //if (colorType == ColorType.None) 
                    colorType = objColor;
                    //else 
                    //    colorType = PCHManager.MixColor(colorType, objColor);              
                    ColorChange(colorType);
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    SandColor sc = io as SandColor;
                    sandCount = sc.count;
                    sandCountText.gameObject.SetActive(true);
                    sandCountText.text = sandCount.ToString();
                    CompleteInteract(io);
                    break;
                case ObjType.Sponge:
                    if(colorType!=ColorType.None&&isSuperAcryl)
                    {
                        colorType = PCHManager.SubstractColor(colorType, objColor);
                        ColorChange(colorType);
                        EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                        CompleteInteract(io);
                    }
                    break;
                case ObjType.Brush:
                    if(colorType!=ColorType.None&&objColor!= ColorType.None)
                    {
                        colorType = PCHManager.MixColor(colorType, objColor);
                        ColorChange(colorType);
                        EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                        CompleteInteract(io);
                        break;
                    }
                    break;
                default:
                    break;
            }
            
        }
        StateUpdate();
    }
    public override void Move(Vector3 dir)
    {
        
        if (hitFilter)
        {
            Filter filter = hitFilter.GetComponent<Filter>();
            if (filter.isOneTime)
            {
                filter.used = true;
            }
    
            if (filter.filterType == FilterType.Mix && filter.isAlpha==isAlpha)
            {
                colorType = PCHManager.MixColor(colorType, filter.colorType);
                ColorChange(colorType);
            }
            else if (filter.filterType == FilterType.Substract && filter.isAlpha==isAlpha)
            {
                colorType = PCHManager.SubstractColor(colorType, filter.colorType);
                ColorChange(colorType);
            }
        }
        if (sandCount > 0)
        {
            sandCount--;
            sandCountText.text = sandCount.ToString();
        }
        if (sandCountText.gameObject.activeSelf && sandCount == 0)
        {
            sandCountText.gameObject.SetActive(false);
            colorType = ColorType.None;
            spriter.color = baseColor;
        }
        
        foreach (GameObject h in hit)
        {
            if (!h.activeSelf) continue;
            IObject obj = h.gameObject.GetComponent<IObject>();
            if (h.CompareTag("Wall")) continue;
            if ( obj.is3D && (obj.Type!=ObjType.WoodHammer) && (isAcryl || obj.isAcryl) && obj.isAlpha==isAlpha)
            {
                obj.Move(dir);
            }
        }
        
        
        MapManager.Instance.moveObjList.Add(this.gameObject);
        
        isMoving = true;
        StartCoroutine(MoveCoroutine(new GridPos(objPos.x + (int)dir.x*2, objPos.y + (int)dir.y*2)));
    }
}

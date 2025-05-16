using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Tile : IObject
{
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
                case ObjType.Paint:
                case ObjType.SandColor:
                    colorType = objColor;
                    ColorChange(colorType);
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    CompleteInteract(io);
                    break;
                //case ObjType.SandColor:
                //    prevColor = colorType;
                //    if (isBrush)
                //    {
                //        colorType = PCHManager.MixColor(colorType, objColor);
                //        isBrush = false;
                //    }
                //    else
                //    {
                //        colorType = objColor;
                //    }
                //    ColorChange(colorType);
                //    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                //    SandColor sc = io as SandColor;
                //    sandCount = sc.count;
                //    sandCountText.gameObject.SetActive(true);
                //    CompleteInteract(io);
                //    break;
                case ObjType.Brush:
                    if (objColor != ColorType.None)
                    {
                        colorType = PCHManager.MixColor(colorType, objColor);
                        ColorChange(colorType);
                        EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                        CompleteInteract(io);
                        break;
                    }
                    break;
                case ObjType.Sponge:
                    if (objColor != ColorType.None)
                    {
                        colorType = PCHManager.SubstractColor(colorType, objColor);
                        ColorChange(colorType);
                        EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                        CompleteInteract(io);
                    }
                    break;
                default:
                    break;

            }
        }
    }
    
    public override void Undo()
    {
        if (moveLog.Count == 0)
        {
            MapManager.Instance.DeleteColorTile(gameObject.GetComponent<IObject>());
            MapManager.Instance.gameGrid[objPos.x, objPos.y].Remove(gameObject);
            Destroy(gameObject);
            return;
        }
        ObjData lastData = moveLog.Pop();
        gameObject.SetActive(lastData.isActive);
        isAcryl = lastData.isAcryl;
        isAlpha = lastData.isAlpha;
        isBrush = lastData.isBrush;
        isSuperAcryl = lastData.isSuperAcryl;
        isTrail = lastData.isTrail;
        colorType = lastData.colorType;
        acrylColor = lastData.acrylColor;
        sandCount = lastData.sandCount;
        ColorChange(colorType);
        StateUpdate();
        if (!objPos.Compare(lastData.objPos))
        {
            MapManager.Instance.gameGrid[objPos.x, objPos.y].Remove(this.gameObject);
            objPos = new GridPos(lastData.objPos);
            MapManager.Instance.gameGrid[objPos.x,objPos.y].Add(this.gameObject);
            StartCoroutine(MoveCoroutine(objPos));
        }
    }
}

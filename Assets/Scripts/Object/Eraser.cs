using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eraser : IObject
{
    public override void Awake()
    {
        moveLog = new Stack<ObjData>();
        spriter = GetComponent<SpriteRenderer>();
    }
    public override void Interaction()
    {
        List<GameObject> coll = MapManager.Instance.gameGrid[objPos.x, objPos.y];
        bool interacted = false;
        foreach (GameObject c in coll)
        {
            if (!c.activeSelf) continue;
            if (c.gameObject == this.gameObject)
                continue;
            IObject io = c.gameObject.GetComponent<IObject>();
            ObjType objType = io.Type;
            ColorType objColor = io.colorType;
            switch (objType)
            {
                case ObjType.Eraser:
                    if (this.GetInstanceID() < io.gameObject.GetInstanceID())
                        break;
                    gameObject.SetActive(false);
                    EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                    break;
                case ObjType.BlackSmoke:
                    CompleteInteract(io);
                    gameObject.SetActive(false);
                    EffectManager.Instance.ExecuteEffect(EffectType.Vanish, transform, colorType); SoundBox.instance.PlaySFX("InterVanish");
                    break;
                default:
                    interacted = true;
                    io.OnAlpha();
                    EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform);
                    break;
            }
        }

        if (interacted) gameObject.SetActive(false);
        
    }
}

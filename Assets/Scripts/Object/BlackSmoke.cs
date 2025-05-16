using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackSmoke : IObject
{
    public override void Awake()
    {
        moveLog = new Stack<ObjData>();
        spriter = GetComponent<SpriteRenderer>();
    }
    public override void Interaction()
    {
        bool interacted = false;
        List<GameObject> coll = MapManager.Instance.gameGrid[objPos.x, objPos.y];
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
                case ObjType.BlackSmoke:
                    if (this.GetInstanceID() < io.gameObject.GetInstanceID())
                        break;
                    gameObject.SetActive(false);
                    break;
                case ObjType.Eraser:
                    CompleteInteract(io);
                    gameObject.SetActive(false);
                    break;
                default:
                    io.OffAlpha();
                    interacted = true;
                    EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform);
                    break;
            }
        }
        if(interacted) gameObject.SetActive(false);
    }
}

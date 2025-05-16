using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stamp : IObject
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
            ObjType objType = c.gameObject.GetComponent<IObject>().Type;
            ColorType objColor = c.gameObject.GetComponent<IObject>().colorType;
            switch (objType)
            {
                case ObjType.Player:
                    List<Palette> palettes = MapManager.Instance.GetPalettes();
                    foreach (Palette p in palettes)
                    {
                        if(!p.gameObject.activeSelf) continue;
                        if(objColor==colorType && p.CheckStamp(colorType))
                        {
                            EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType); SoundBox.instance.PlaySFX("Interact");
                            SoundBox.instance.PlaySFX("StampOn");
                        }
                        p.CheckOpen();
                    }
                    //gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
            
        }
    }
}

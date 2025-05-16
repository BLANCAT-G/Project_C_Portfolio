using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EffectType
{
    Pond,
    Vanish,
    ColorInteract,
    Interact,
}
public class EffectManager : MonoBehaviour
{
    private EffectManager() { }

    private static EffectManager instance;

    public static EffectManager Instance => instance;
    [SerializeField]
    private GameObject[] effectPrefab;

    [SerializeField]
    private ColorType effColorType;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    public void ExecuteEffect(EffectType type,Transform transform,ColorType colorType=ColorType.White)
    {
        effColorType=colorType;
        GameObject go=Instantiate(effectPrefab[(int)type], transform.position,Quaternion.identity);
        if(type==EffectType.ColorInteract)
        {
            go.GetComponent<SpriteRenderer>().color = effColorType.ToColor();
        }
    }
    public ColorType GetEffectColor()
    {
        return effColorType;
    }
}

using UnityEngine;



public class Player : IObject
{
    private ColorType baseColor = ColorType.None;
    public Animator animator;
    public Animator animatorBase;
    public GameObject layoutGroup;
    
    // [SerializeField]
    // private GameObject TrailEff;
    // [SerializeField]
    // private SpriteRenderer trailsr;

    

    
    public override void Interaction()
    {
        Collider2D[] coll=Physics2D.OverlapBoxAll(transform.position, Vector2.zero, 0);   
        foreach (Collider2D c in coll)
        {
            if (c.gameObject == this.gameObject)
                continue;
            IObject io= c.gameObject.GetComponent<IObject>();
            ObjType objType = io.Type;
            ColorType objColor = io.colorType;
            switch (objType)
            {
                case ObjType.Paint:
                    if (isBrush)
                    {
                        colorType = PCHManager.MixColor(colorType, objColor);
                        isBrush = false;
                    }else
                    {
                        colorType = objColor;
                    }
                    if(sandCountText.gameObject.activeSelf)
                    {
                        sandCountText.gameObject.SetActive(false);
                        sandCount = 0;
                    }
                    ColorChange(colorType);
                    //trailsr.color = spriter.color;
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    CompleteInteract(io);
                    break;           
                case ObjType.Acryl:
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
                    EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType);
                    ColorChange(colorType);
                    CompleteInteract(io);
                    break;
                case ObjType.WoodHammer:
                    isAcryl = false;
                    isSuperAcryl = false;
                    AcrylEff.SetActive(false);
                    AcrylEff.gameObject.GetComponent<SpriteRenderer>().color = new Color(221f/255f, 221f / 255f, 221f / 255f, 1);
                    EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType);
                    CompleteInteract(io);
                    break;
                case ObjType.Fixed_Paint:
                    if (isBrush)
                    {
                        colorType = PCHManager.MixColor(colorType, objColor);
                        isBrush = false;
                    }else
                    {
                        colorType = objColor;
                    }
                    //trailsr.color = spriter.color;
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    ColorChange(colorType);
                    break;
                case ObjType.Water_Bucket:
                    isBrush = false;
                    colorType = baseColor;
                    ColorChange(colorType);
                    c.gameObject.SetActive(false);
                    EffectManager.Instance.ExecuteEffect(EffectType.Pond,transform);
                    SoundBox.instance.PlaySFX("ColorRelease");
                    CompleteInteract(io);
                    break;
                case ObjType.Fixed_Water_Bucket:
                    isBrush = false;
                    colorType = baseColor;
                    ColorChange(colorType);
                    EffectManager.Instance.ExecuteEffect(EffectType.Pond,transform);
                    SoundBox.instance.PlaySFX("ColorRelease");
                    RefreshInteractObject();
                    io.RefreshInteractObject();
                    break;
                case ObjType.Brush:
                    if(objColor!=ColorType.None)
                    {
                        colorType = PCHManager.MixColor(colorType, objColor);
                        ColorChange(colorType);
                        //trailsr.color = spriter.color;
                        EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                        CompleteInteract(io);
                        break;
                    }
                    isBrush = true;
                    EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType);
                    CompleteInteract(io);
                    break;
                case ObjType.Sponge: 
                    if(objColor==ColorType.None)
                    {
                        EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType);
                        CompleteInteract(io);
                        break;
                    }
                    colorType = PCHManager.SubstractColor(colorType, objColor);
                    ColorChange(colorType);
                    //trailsr.color = spriter.color;
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    CompleteInteract(io);
                    break;
                case ObjType.SandColor:
                    prevColor = colorType;
                    if (isBrush)
                    {
                        colorType = PCHManager.MixColor(colorType, objColor);
                        isBrush = false;
                    }
                    else
                    {
                        colorType = objColor;
                    }
                    
                    ColorChange(colorType);
                    //trailsr.color = spriter.color;
                    EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                    SandColor sc = io as SandColor;
                    sandCount = sc.count;
                    sandCountText.gameObject.SetActive(true);
                    sandCountText.text = sandCount.ToString();
                    CompleteInteract(io);
                    break;
                case ObjType.Easel:
                    Easel easel = c.gameObject.GetComponent<Easel>();
                    if (easel.colorSaved)
                    {
                        if (isBrush)
                        {
                            colorType = PCHManager.MixColor(colorType, objColor);
                            isBrush = false;
                        }
                        else
                            colorType = objColor;
                        ColorChange(colorType);
                        EffectManager.Instance.ExecuteEffect(EffectType.ColorInteract, transform, colorType); SoundBox.instance.PlaySFX("ColorChange");
                        c.gameObject.SetActive(false);
                    }
                    else
                    {
                        easel.colorSaved = true;
                        easel.colorType = colorType;
                        easel.ColorChange(colorType);
                        EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType);
                    }
                    break;
                case ObjType.TrailBrush:
                    isTrail = true;
                    TrailEff.SetActive(true);
                    //trailsr.color = spriter.color;
                    EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType);
                    CompleteInteract(io);
                    break;
                case ObjType.Mop:
                    isTrail = false;
                    TrailEff.SetActive(false);
                    EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType);
                    CompleteInteract(io);
                    break;
                default:
                    break;
            }
        }
        StateUpdate();
    }

    public override bool MoveCheck(Vector3 dir)
    {
        if (isMoving) return false;
        Flip(dir);

        hit = Physics2D.OverlapBoxAll(transform.position + dir, Vector2.zero, 0);
        filterObj = Physics2D.OverlapBox(transform.position + dir * 0.51f, Vector2.zero, 0);
        oppositeFilterObj = Physics2D.OverlapBox(transform.position + dir * 0.49f, Vector2.zero, 0);
        if (filterObj)
        {
            if (filterObj.CompareTag("Wall")) return false;
            Filter filter = filterObj.GetComponent<Filter>();
            if (filter && ((filter.isOneTime && filter.used) || (filter.filterType==FilterType.None && filter.colorType != colorType)))
            {
                return false;
            }

            Palette palette = filterObj.GetComponent<Palette>();
            if (palette) return false;
        }
        else if (oppositeFilterObj)
        {
            return false;
        }

        foreach (Collider2D h in hit) // 분리한 이유: 타일 위에 오브젝트가 올라가있을 수 있기 때문에
        {
            if (h.CompareTag("Wall"))
                return false;
            IObject obj = h.gameObject.GetComponent<IObject>();
            if (obj.Type == ObjType.Tile && obj.colorType != colorType)
                return false;
        }
        
        foreach (Collider2D h in hit)
        {
            IObject obj = h.gameObject.GetComponent<IObject>();
            if (obj.is3D && (obj.Type!=ObjType.WoodHammer) && (isAcryl || obj.isAcryl))
            {
                if (obj.MoveCheck(dir) == false)
                    return false;
            }
        }

        return true;
    }

    public override void Move(Vector3 dir)
    {
        
        if (filterObj)
        {
            Filter filter = filterObj.GetComponent<Filter>();
            if (filter.isOneTime)
            {
                filter.Use();
            }

            if (filter.filterType == FilterType.Mix )
            {
                colorType = PCHManager.MixColor(colorType, filter.colorType);
                ColorChange(colorType);
            }
            else if (filter.filterType == FilterType.Substract)
            {
                colorType = PCHManager.SubstractColor(colorType, filter.colorType);
                ColorChange(colorType);
            }
        }
        
        if(sandCount>0)
        {
            sandCount--;
            sandCountText.text = sandCount.ToString();
        }    
        if(sandCountText.gameObject.activeSelf&&sandCount==0)
        {
            sandCountText.gameObject.SetActive(false);
            colorType = prevColor;
            ColorChange(colorType);
        }
        if (isSuperAcryl && colorType!=ColorType.None)
        {
            foreach (Collider2D h in hit)
            {
                IObject obj = h.gameObject.GetComponent<IObject>();
                if (obj.is3D)
                {
                    if (obj.colorType != ColorType.None)
                    {
                        obj.colorType = this.acrylColor;
                        obj.ColorChange(this.colorType);
                    }
                    obj.acrylColor = this.acrylColor;
                    obj.StateUpdate();
                }
            }
        }
        if (isTrail && colorType!=ColorType.None)
        {
            bool k = true;
            Collider2D[] coll=Physics2D.OverlapBoxAll(transform.position, Vector2.zero, 0);
            foreach (Collider2D c in coll)
            {
                if (c.gameObject.GetComponent<IObject>().is3D==false)
                {
                    k = false;
                    break;
                }
            }

            if (k)
            {
                GameObject colorTile=Instantiate(MapManager.Instance.defaultColorTile);
                colorTile.transform.position = transform.position;
                colorTile.GetComponent<IObject>().colorType = this.colorType;
                colorTile.GetComponent<IObject>().ColorChange(this.colorType);
                MapManager.Instance.AddColorTile(colorTile.GetComponent<IObject>());
            } 
        }
        
        foreach (Collider2D h in hit)
        {
            IObject obj = h.gameObject.GetComponent<IObject>();
            if (h.CompareTag("Wall")) continue;
            if ( obj.is3D && (obj.Type!=ObjType.WoodHammer) && (isAcryl || obj.isAcryl))
            {
                obj.Move(dir);
            }
        }
        
        isMoving = true;
        StartCoroutine(MoveCoroutine(dir));
    }
    
    public override void SaveData()
    {
        ObjData newData = new ObjData(transform.position, GameManager.Instance.faceDir,gameObject.activeSelf, isAcryl, isAlpha, isBrush, isSuperAcryl,isTrail,sandCount, colorType,acrylColor);
        moveLog.Push(newData);
    }
    public void Flip(Vector3 dir)
    {
        animator.ResetTrigger("Side");
        animator.ResetTrigger("Up");
        animator.ResetTrigger("Down");
        animatorBase.ResetTrigger("Side");
        animatorBase.ResetTrigger("Up");
        animatorBase.ResetTrigger("Down");
        if (dir == Vector3.left)
        {
            animator.SetTrigger("Side");
            animatorBase.SetTrigger("Side");
            Vector3 m_Scale = transform.localScale;
            m_Scale.x = Mathf.Abs(m_Scale.x);
            transform.localScale = m_Scale;

            layoutGroup.gameObject.transform.localScale = m_Scale;
            AcrylEff.gameObject.transform.localScale = m_Scale;
        }
        else if (dir == Vector3.right)
        {
            animator.SetTrigger("Side");
            animatorBase.SetTrigger("Side");
            Vector3 m_Scale = transform.localScale;
            m_Scale.x = -Mathf.Abs(m_Scale.x);
            transform.localScale = m_Scale;
            layoutGroup.gameObject.transform.localScale = m_Scale;
            AcrylEff.gameObject.transform.localScale = m_Scale;

        }
        else if (dir == Vector3.up)
        {
            animator.SetTrigger("Up");
            animatorBase.SetTrigger("Up");
        }
        else if (dir == Vector3.down)
        {
            animator.SetTrigger("Down");
            animatorBase.SetTrigger("Down");
        }

    }

    public override void Undo()
    {
        if (moveLog.Count == 0)
        {
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
        StateUpdate();
        ColorChange(colorType);
        Flip(lastData.faceDir);
        if (transform.position != lastData.pos)
            StartCoroutine(MoveCoroutine((lastData.pos - transform.position).normalized));
    }

    public override void ColorChange(ColorType cT)
    {
        colorType = cT;
        if (cT == ColorType.None)
        {
            spriter.color =  ColorType.PlayerNone.ToColor();
            if (isAcryl)
            {
                isSuperAcryl = false;
                AcrylEff.gameObject.GetComponent<SpriteRenderer>().color = new Color(221f/255f, 221f / 255f, 221f / 255f, 1);
            }
        }
        else
        {
            spriter.color = cT.ToColor();
            if (isSuperAcryl)
            {
                acrylColor = colorType;
                AcrylEff.gameObject.GetComponent<SpriteRenderer>().color = acrylColor.ToColor();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            EffectManager.Instance.ExecuteEffect(EffectType.Vanish,transform);
    }
    public void SetSandCount(uint i)
    {
        sandCount = i;
        sandCountText.gameObject.SetActive(true);
    }

   
}

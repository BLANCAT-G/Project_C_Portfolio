using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
public class Test_Paint : IObject
{
    public SpriteRenderer sr;
    public override void Interaction()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        sr= GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [ButtonMethod]
    void ChangeColor()
    {
        sr.color= colorType.ToColor();
    }
}

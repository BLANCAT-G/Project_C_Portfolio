using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectNode : MonoBehaviour
{
    public SpriteRenderer sr;
    [SerializeField]
    protected int myID;
    public Sprite[] sprites;
    public SelectNode prevNode;
    public SelectNode nextNode;

    public bool isLock;
    // Start is called before the first frame update
    protected void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    
    public int GetID()
    {
        return (int)myID;
    }
}

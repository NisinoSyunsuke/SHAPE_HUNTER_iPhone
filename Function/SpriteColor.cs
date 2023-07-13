using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteColor : MonoBehaviour
{
    enum Type { spriteRenderer, Image } [SerializeField] Type type;
    [ColorUsage(true, true), SerializeField] Color color;
    SpriteRenderer spriteRenderer;
    Image image;
    [SerializeField] bool realTime;
    void Start()
    {
        switch (type)
        {
            case Type.spriteRenderer:
                spriteRenderer = GetComponent<SpriteRenderer>();
                spriteRenderer.material.SetColor("_Color", color);
                break;
            case Type.Image:
                image = GetComponent<Image>();
                image.material.SetColor("_Color", color);
                break;
        }
        
    }
    private void FixedUpdate()
    {
        if (realTime) { 
            switch (type) {
                case Type.spriteRenderer: spriteRenderer.material.SetColor("_Color", color); break;
            } 
        }
    }
}

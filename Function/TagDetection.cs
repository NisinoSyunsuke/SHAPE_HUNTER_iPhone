using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagDetection : MonoBehaviour
{
    enum Type { trigger, collision }
    [SerializeField] Type type;
    [SerializeField] string tagName;
    private bool isGround = false;
    private bool isGroundEnter, isGroundStay;
    bool isCollisonEnter, isCollisonStay;
    //bool isGroundExit; //ç°ÇÕóléqå©Ç≈égÇ¡ÇƒÇ¢Ç»Ç¢
    public bool Detection()
    {
        switch (type)
        {
            case Type.trigger:
                isGround = false;
                if (isGroundEnter || isGroundStay)
                {
                    isGround = true;
                }
                //else if (isGroundExit) { isGround = false; }
                isGroundEnter = false;
                isGroundStay = false;
                //isGroundExit = false;
                return isGround;
            case Type.collision:
                isGround = false;
                if (isCollisonEnter || isCollisonStay)
                {
                    isGround = true;
                }
                //else if (isGroundExit) { isGround = false; }
                isCollisonEnter = false;
                isCollisonStay = false;
                return isGround;
            default: return false;
        }
    
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == tagName)
        {
            isGroundEnter = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == tagName)
        {
            isGroundStay = true;
        }
    }
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.tag == tag_name)
    //    { isGroundExit = true; } }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == tagName)
        {
            isCollisonEnter = true;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == tagName)
        {
            isCollisonStay = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : Collidable
{

    //Logic
    protected bool collected;

    protected override void OnCollide(Collider2D coll)
    {
        if(coll.name == "Player")
        {
            OnCollide();
        }

    }
    protected virtual void OnCollide()
    {
        collected = true;
    }

}

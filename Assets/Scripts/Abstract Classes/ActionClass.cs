using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionClass : SelectClass
{
    //The following are 'properties' in C# that make quick getters and setters for private fields. ac.Target for access
    public EntityClass Target { get; set; }
    public EntityClass Origin { get; set; }

    public int Damage { get; protected set; }
    public int Block { get; protected set; }
    public int Speed { get; protected set; }

    public CardType CardType { get; protected set; }

    protected Vector3 OriginalPosition;

    protected bool EnqueueMoveDown = false;

    public abstract void ExecuteActionEffect();

    public override void OnMouseDown()
    {
        HighlightManager.OnActionClicked(this);
    }
    public virtual void OnHit()
    {
        this.Target.TakeDamage(Damage);
    }

    public override void OnMouseEnter()
    {
        if (!isOutlined)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f, 1);
            transform.position += new Vector3((float)0.04, (float)0.4, 0);
        }
        else
        {
            EnqueueMoveDown = false;
        }
    }

    public override void OnMouseExit()
    {
        if (!isOutlined)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);
            transform.position -= new Vector3((float)0.04, (float)0.4, 0);
        }
        else
        {
            EnqueueMoveDown = true;
        }
    }

    public override void Highlight()
    {
        isOutlined = true;
        GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f, 1);
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public override void DeHighlight()
    {
        isOutlined = false;
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);
        transform.rotation = Quaternion.Euler(0, 0, -5);
        if (EnqueueMoveDown)
        {
            transform.position -= new Vector3((float)0.04, (float)0.4, 0);
            EnqueueMoveDown = false;
        }
    }
}

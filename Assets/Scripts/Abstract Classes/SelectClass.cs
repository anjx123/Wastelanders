using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectClass : MonoBehaviour
{
    protected string myName;
    protected string myDescription;
    protected Material outliner;
    protected Material ogMaterial;
    public Transform myTransform;
    protected bool isOutlined = false; // deals with enemy selection

    private bool grewLarger; //Checks if entity was highlighted first to ensure proper dehighlighting. 

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetName() {
        return myName;
    }

    public virtual void OnMouseEnter()
    {
        if (CombatManager.Instance.CanHighlight() && !grewLarger)
        {
            myTransform.localScale += new Vector3((float)0.05, (float)0.05, 0);
            grewLarger = true;
        }
    }

    public virtual void OnMouseExit()
    {
        if (grewLarger)
        {
            myTransform.localScale -= new Vector3((float)0.05, (float)0.05, 0);
            grewLarger = false;
        }
    }

    public string GetDescription()
    {
        return myDescription;
    }


    public abstract void OnMouseDown();

    public virtual bool Toggle()
    {
        isOutlined = !isOutlined;

        if (isOutlined) {
            Highlight();
        } else {
            DeHighlight();
        }

        return isOutlined;
    }

    public virtual void Highlight()
    {
        //Renderer renderer = GetComponent<Renderer>();
        isOutlined = true;
        //renderer.material = outliner;
        GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1);
    }

    public virtual void DeHighlight() 
    {
        //Renderer renderer = GetComponent<Renderer>();
        isOutlined = false;
        //renderer.material = ogMaterial;
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);
    }

}

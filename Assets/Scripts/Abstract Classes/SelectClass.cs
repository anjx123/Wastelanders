using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectClass : MonoBehaviour
{

    public string myName;
    protected Material outliner;
    protected Material ogMaterial;
    protected bool isOutlined = false; // deals with enemy selection

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

    // abstract as actions and entities have different ways to display selection
    public abstract void OnMouseEnter();
    public abstract void OnMouseExit();

    public abstract void OnMouseDown();

    public bool Toggle()
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

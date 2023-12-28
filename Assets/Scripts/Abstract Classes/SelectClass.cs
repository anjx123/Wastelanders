using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectClass : MonoBehaviour
{

    protected string myName;
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

    public string getName() {
        return myName;
    }

    public void OnMouseEnter() 
    {
        if (!isOutlined) {
            //Renderer renderer = GetComponent<Renderer>();
            //renderer.material = outliner;
            GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1);
        }
    }

    public void OnMouseExit()
    {
        if (!isOutlined) {
            //Renderer renderer = GetComponent<Renderer>();
            //renderer.material = ogMaterial;
            GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);
        }
    }

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

    public void Highlight()
    {
        //Renderer renderer = GetComponent<Renderer>();
        isOutlined = true;
        //renderer.material = outliner;
        GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1);
    }

    public void DeHighlight() 
    {
        //Renderer renderer = GetComponent<Renderer>();
        isOutlined = false;
        //renderer.material = ogMaterial;
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] protected GameObject checkmark;

    public virtual void OnMouseDown()
    {
        Debug.Log(gameObject.name);
    }

    public void SetSelected(bool isSelected)
    {
        checkmark.SetActive(isSelected);
    }
}

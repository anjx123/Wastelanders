// SpriteFadeHandler.cs
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFadeHandler : FadeHandlerBase
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    protected override float CurrentAlpha => spriteRenderer.color.a;
    public string FadeSortingLayer => spriteRenderer.sortingLayerName;
    public int FadeSortingLayerId => spriteRenderer.sortingLayerID;
    public int FadeSortingOrder => spriteRenderer.sortingOrder;
    public float FadeScreenZValue => spriteRenderer.gameObject.transform.position.z;

    protected override void SetAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color currentColor = spriteRenderer.color;
            currentColor.a = Mathf.Clamp01(alpha);
            spriteRenderer.color = currentColor;
        }
    }
}
using UnityEngine;

public static class UIUtility
{
    public static bool IsOverlappingWith(this RectTransform a, RectTransform b)
    {
        Vector3[] aCorners = new Vector3[4];
        Vector3[] bCorners = new Vector3[4];
            
        a.GetWorldCorners(aCorners);
        b.GetWorldCorners(bCorners);
        Rect aRect = new Rect(aCorners[0].x, aCorners[0].y, aCorners[2].x - aCorners[0].x, aCorners[2].y - aCorners[0].y);
        Rect bRect = new Rect(bCorners[0].x, bCorners[0].y, bCorners[2].x - bCorners[0].x, bCorners[2].y - bCorners[0].y);

        return aRect.Overlaps(bRect);
    }
}
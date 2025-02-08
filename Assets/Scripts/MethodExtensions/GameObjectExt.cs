using UnityEngine;

public static class GameObjectExt
{
    public static RectTransform RectTransform(this GameObject go)
    {
        return (RectTransform)(go.transform);
    }
}
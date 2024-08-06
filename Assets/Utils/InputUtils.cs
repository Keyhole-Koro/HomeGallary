using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public static class InputUtils
{
    public static bool OnLeftMouseButtonDown()
    {
        return Input.GetMouseButton(0);
    }

    public static RaycastResult GetFrontMostResult(List<RaycastResult> results)
    {
        if (results.Count == 0)
        {
            return default;
        }

        return results[0];
    }
}

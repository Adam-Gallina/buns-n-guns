using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyDebug : MonoBehaviour
{
    #region Debugging
    public static void DrawSquare(Vector2 minPoint, Vector2 maxPoint, Color col, float duration)
    {
        Debug.DrawLine(minPoint,                            new Vector2(minPoint.x, maxPoint.y), col, duration);
        Debug.DrawLine(new Vector2(minPoint.x, maxPoint.y), maxPoint,                            col, duration);
        Debug.DrawLine(maxPoint,                            new Vector2(maxPoint.x, minPoint.y), col, duration);
        Debug.DrawLine(new Vector2(maxPoint.x, minPoint.y), minPoint,                            col, duration);

        Debug.DrawLine(minPoint, maxPoint, col, duration);
    }
    #endregion

    public static Vector2 Rotate(Vector2 targetVector, float degrees)
    {
        float angle = Mathf.Deg2Rad * degrees;
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        return new Vector2(cos * targetVector.x - sin * targetVector.y,
                           sin * targetVector.x + cos * targetVector.y);
    }

    public static void SetFancyText(Transform textParent, string text)
    {
        foreach (Text t in textParent.GetComponentsInChildren<Text>())
            t.text = text;
    }

    public static void SetFancyTextOpacity(Transform textParent, float opacity)
    {
        foreach (Text t in textParent.GetComponentsInChildren<Text>())
        {
            Color c = t.color;
            c.a = opacity;
            t.color = c;
        }
    }

    public static EffectBase SpawnEffect(GameObject effectPrefab, Vector3 targetPos)
    {
        GameObject effect = Instantiate(effectPrefab);
        effect.transform.position = targetPos;
        return effect.GetComponent<EffectBase>();
    }
}

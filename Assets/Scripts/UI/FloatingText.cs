using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public static FloatingText instance;

    [SerializeField] private GameObject textPrefab;
    [SerializeField] private float textSpeed;
    [SerializeField] private float defaultTextLifetime = 0.75f;

    private void Awake()
    {
        instance = this;
    }

    public void CreateText(Vector2 targetPos, string text, float lifetime=0)
    {
        if (lifetime == 0)
            lifetime = defaultTextLifetime;

        StartCoroutine(FloatText(targetPos, text, lifetime));
    }

    private IEnumerator FloatText(Vector2 targetPos, string text, float lifetime)
    {
        Transform floatingText = Instantiate(textPrefab, transform).transform;
        floatingText.GetComponentInChildren<Rigidbody2D>().velocity = new Vector2(0, textSpeed);
        MyDebug.SetFancyText(floatingText, text);

        float endTime = Time.time + lifetime;
        while (Time.time < endTime)
        {
            floatingText.position = Camera.main.WorldToScreenPoint(targetPos);
            MyDebug.SetFancyTextOpacity(floatingText, (endTime - Time.time) / lifetime);
            yield return new WaitForEndOfFrame();
        }

        Destroy(floatingText.gameObject);
    }
}

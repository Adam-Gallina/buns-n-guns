using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heartbar : Healthbar
{
    [Header("Image")]
    public GameObject UIheartPrefab;
    public int healthPerHeart = 1;
    public int heartPadding = 50;
    private List<Image> hearts = new List<Image>();

    public override void AssignEntity(HealthBase entity, string entityName)
    {
        base.AssignEntity(entity, entityName);

        float maxHealthInHearts = (float)entity.maxHealth / healthPerHeart;
        CreateHearts((int)(maxHealthInHearts + 0.5f));
        DisplayHealth(entity.maxHealth);
    }

    public void CreateHearts(int heartCount)
    {
        RectTransform heartTransform = UIheartPrefab.GetComponent<RectTransform>();
        float distBetweenHearts = heartTransform.sizeDelta.x * heartTransform.localScale.x + heartPadding;

        for (int i = 0; i < heartCount; i++)
        {
            RectTransform newHeart = Instantiate(UIheartPrefab, transform).GetComponent<RectTransform>();
            newHeart.localPosition = new Vector3(distBetweenHearts * i, 0, 0);

            hearts.Add(newHeart.GetChild(1).GetComponent<Image>());
        }
    }

    protected override void DisplayHealth(float health)
    {
        float remainingHealth = health / healthPerHeart;
        for (int i = 0; i < hearts.Count; i++)
        {
            if (remainingHealth < 1)
            {
                hearts[i].fillAmount = remainingHealth;
                remainingHealth = 0;
            }
            else
            {
                hearts[i].fillAmount = 1;
                remainingHealth--;
            }
        }

        currDisplayedHealth = health;
    }
}

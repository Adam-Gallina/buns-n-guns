using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    protected HealthBase targetEntity;
    protected float maxHealth;
    protected float currDisplayedHealth;
    [SerializeField] protected float healthChangeTime = 0.25f;
    protected float currHealthSpeed;

    [SerializeField] protected Image healthbarImage;
    [SerializeField] protected Transform healthbarText;

    public virtual void AssignEntity(HealthBase entity, string entityName)
    {
        targetEntity = entity;
        maxHealth = targetEntity.maxHealth;

        DisplayHealth(maxHealth);
        if (healthbarText)
            MyDebug.SetFancyText(healthbarText, entityName);
    }

    protected virtual void Update()
    {
        if (targetEntity)
        {
            float targetHealth = targetEntity.GetCurrHealth();
            if (targetHealth != currDisplayedHealth)
            {
                UpdateDisplayedHealth(targetHealth);
            }
            else
            {
                currHealthSpeed = 0;
            }
        }
    }

    protected virtual void UpdateDisplayedHealth(float targetHealth)
    {
        float newSpeed = Mathf.Abs(targetHealth - currDisplayedHealth) / healthChangeTime;
        if (newSpeed > currHealthSpeed)
            currHealthSpeed = newSpeed;
        int dir = targetHealth > currDisplayedHealth ? 1 : -1;


        if (Mathf.Abs(targetHealth - currDisplayedHealth) < currHealthSpeed * Time.deltaTime)
        {
            DisplayHealth(targetHealth);
        }
        else
        {
            float newHealth = currDisplayedHealth + (dir * currHealthSpeed * Time.deltaTime);
            DisplayHealth(newHealth);
        }
    }

    protected virtual void DisplayHealth(float health)
    {
        float fillPercent = health / maxHealth;
        healthbarImage.fillAmount = fillPercent;
        currDisplayedHealth = health;
    }

    public void ShowHealthbar()
    {
        gameObject.SetActive(true);
    }

    public void HideHealthbar()
    {
        gameObject.SetActive(false);
    }
}

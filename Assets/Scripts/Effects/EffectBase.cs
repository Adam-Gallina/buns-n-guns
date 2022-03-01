using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBase : MonoBehaviour
{
    protected ParticleSystem system;

    [Tooltip("True - callback when emission stops\nFalse - callback when system stops")]
    [SerializeField] public bool callbackOnEmissionEnd = false;
    [Tooltip("True - destroy when emission stops\nFalse - destroy when system stops")]
    [SerializeField] public bool destroyOnEmissionEnd = false;

    protected System.Action callback;
    protected bool sentCallback = false;

    protected virtual void Awake()
    {
        system = GetComponent<ParticleSystem>();
    }

    public void SetCallback(System.Action callback)
    {
        this.callback = callback;
    }

    protected void Update()
    {
        if (callbackOnEmissionEnd ? !system.isEmitting : system.isStopped)
        {
            if (!sentCallback)
            {
                callback?.Invoke();
                sentCallback = true;
            }

            if ((destroyOnEmissionEnd && !system.isEmitting) || system.isStopped)
                Destroy(gameObject);
        }
        else if (LevelController.instance.paused)
            system.Pause();
        else if (!system.isPlaying)
            system.Play();
    }

    public void SetEffectColor(Color newColor)
    {
        ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient(newColor);
        ParticleSystem.MainModule main = system.main;
        main.startColor = gradient;
    }
}

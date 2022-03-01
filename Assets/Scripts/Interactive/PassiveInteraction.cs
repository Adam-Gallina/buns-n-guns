using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveInteraction : Interactive
{
    [SerializeField] public ActiveRequirement[] required;

    protected override void Update()
    {
        base.Update();

        CheckRequirements();
    }

    protected virtual void CheckRequirements()
    {
        bool newActivationState = true;

        foreach (ActiveRequirement req in required)
        {
            if (!req.IsActive())
            {
                newActivationState = false;
                break;
            }
        }

        UpdateActivation(newActivationState);
    }

    protected virtual void UpdateActivation(bool newActivationState)
    {
        if (newActivationState != activated)
        {
            activated = newActivationState;

            if (activated)
                Activate();
            else
                Deactivate();
        }
    }

    protected abstract void Activate();
    protected abstract void Deactivate();

}

[System.Serializable]
public class ActiveRequirement
{
    public Interactive target;
    public bool activeState = true;

    public bool IsActive()
    {
        return target.GetStatus() == activeState;
    }
}

using UnityEditor.Animations;
using UnityEngine;
using System.Linq;
using System.Collections;

public interface IPlayableEnemyCard
{
#nullable enable
    // "Macro" that helps you automate adding animations that belong to foreign cards. 
    //This method will alter AnimationController ASSET directly, creating the correct animation transitions, that persist even after the runtime environment. :fearful: 
    protected static void ApplyForeignAttackAnimation(
        EntityClass entityClass,
        AnimationClip? animationClip,
        string triggerName)
    {
        AnimatorController? animatorController = entityClass.AnimatorController;

        if (animatorController == null || animationClip == null)
        {
            Debug.LogWarning("AnimatorController or AnimationClip is null. AnimatorController must not be null to support foreign animations.");
            return;
        }

        if (!ParameterExists(animatorController, triggerName))
        {
            AnimatorControllerParameter parameter = new AnimatorControllerParameter
            {
                name = triggerName,
                type = AnimatorControllerParameterType.Trigger
            };
            animatorController.AddParameter(parameter);
        }

        // Ensure the target clip doesn't already exist in the controller
        var stateMachine = animatorController.layers[0].stateMachine;
        var existingState = stateMachine.states.FirstOrDefault(s => s.state.name == animationClip.name);

        if (existingState.state != null)
        {
            Debug.LogWarning($"State {animationClip.name} already exists. Skipping addition.");
            return;
        }

        // Add new state for the spawning clip
        AnimatorState attackAnimationState = stateMachine.AddState(animationClip.name);
        attackAnimationState.motion = animationClip;

        AnimatorState defaultState = stateMachine.defaultState;
        if (defaultState == null)
        {
            Debug.LogError("Default state not found in the animator controller.");
            return;
        }

        // Create a transition from the default state to the spawning state
        AnimatorStateTransition toAttackTransition = defaultState.AddTransition(attackAnimationState);
        toAttackTransition.hasExitTime = false;
        toAttackTransition.hasFixedDuration = false;
        toAttackTransition.duration = 0f;
        toAttackTransition.AddCondition(AnimatorConditionMode.If, 0, triggerName);

        // Create a transition from the spawning state back to the default state
        AnimatorStateTransition toDefaultTransition = attackAnimationState.AddTransition(defaultState);
        toDefaultTransition.hasExitTime = true;
        toDefaultTransition.exitTime = 1f;
        toDefaultTransition.duration = 0f;
        Debug.Log("Attack Animation state and transitions added successfully and called.");
    }
    private static bool ParameterExists(AnimatorController controller, string paramName)
    {
        return controller.parameters.Any(p => p.name == paramName);
    }
}
public interface IPlayableBeetleCard : IPlayableEnemyCard { }
public interface IPlayableFrogCard : IPlayableEnemyCard { }
public interface IPlayableSlimeCard : IPlayableEnemyCard { }
public interface IPlayableQueenCard : IPlayableEnemyCard { }
public interface IPlayablePrincessFrogCard : IPlayableEnemyCard { }

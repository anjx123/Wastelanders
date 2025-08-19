using LevelSelectInformation;
using System;
using System.Collections;
using System.Collections.Generic;
using Systems.Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static BattleIntroEnum;

public class PreBounty_1 : MonoBehaviour
{
    [SerializeField] private GameObject jackie;
    [SerializeField] private GameObject ives;

    [SerializeField] private Transform ivesTarget;
    
    [SerializeField] private FadeScreenHandler fadeScreenHandler;
    
    [SerializeField] private List<DialogueText> bountyDiscussionDialogue;

    [SerializeField] private float ivesMoveSpeed = 2f;
    
    public void Start()
    {
        StartCoroutine(StartScene());
    }

    public IEnumerator StartScene()
    {
        fadeScreenHandler.SetDarkScreen();
        yield return new WaitForSeconds(1f);
        
        yield return fadeScreenHandler.FadeInLightScreen(1f);

        yield return MoveCharacterToTarget(ives, ivesTarget, ivesMoveSpeed);
    }

    private IEnumerator MoveCharacterToTarget(GameObject character, Transform target, float speed)
    {
        Animator animator = character.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetBool("IsMoving", true);
        }

        Vector3 originalScale = character.transform.localScale;

        while (Vector3.Distance(character.transform.position, target.position) > 0.05f)
        {
            // Determine direction
            float direction = target.position.x - character.transform.position.x;

            // Flip character by adjusting scale
            if (direction != 0)
            {
                character.transform.localScale = new Vector3(
                    Mathf.Sign(direction) * Mathf.Abs(originalScale.x), 
                    originalScale.y, 
                    originalScale.z
                );
            }

            // Move character
            character.transform.position = Vector3.MoveTowards(
                character.transform.position,
                target.position,
                speed * Time.deltaTime
            );

            yield return null;
        }

        // Snap to exact position
        character.transform.position = target.position;

        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }
    }

}

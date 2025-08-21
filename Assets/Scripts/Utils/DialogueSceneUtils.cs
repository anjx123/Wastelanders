using System.Collections;
using UnityEngine;

namespace Utils
{
    public class DialogueSceneUtils
    {
        public static IEnumerator MoveCharacterToTarget(GameObject character, Transform target, float speed)
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
}
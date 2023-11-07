using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonPlayableCharacter : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] List<Vector3> movementPattern;
    int currentPattern = 0;
    float speed = 3f;
    float timer = 0f;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer < movementPattern[currentPattern].z) 
        {
            transform.position = new Vector3(
                transform.position.x + movementPattern[currentPattern].x * Time.deltaTime * speed,
                transform.position.y + movementPattern[currentPattern].y * Time.deltaTime * speed,
                transform.position.z
            );
        }
        else
        {
            timer = 0f;
            currentPattern++;
            if (currentPattern >= movementPattern.Count)
            {
                currentPattern = 0;
            }
        }
        bool moving = movementPattern[currentPattern].x != 0 || movementPattern[currentPattern].y != 0;
        if (moving)
        {
            animator.SetFloat("moveX", movementPattern[currentPattern].x);
            animator.SetFloat("moveY", movementPattern[currentPattern].y);
        }
        animator.SetBool("moving", moving);
    }
}
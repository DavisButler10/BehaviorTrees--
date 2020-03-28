using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrive : SteeringBehavior
{
    public Kinematic character;
    public GameObject target;

    float maxAcceleration = 0.3f;
    float maxSpeed = 0.5f;

    float targetRadius = 1f;

    float slowRadius = 2f;

    float timeToTarget = 0.5f;
    protected virtual Vector3 getTargetPosition()
    {
        return target.transform.position;
    }

    public override SteeringOutput getSteering()
    {
        SteeringOutput result = new SteeringOutput();

        Vector3 direction = getTargetPosition() - character.transform.position;
        float distance = direction.magnitude;

        float targetSpeed = 0f;
        if (distance > slowRadius)
        {
            targetSpeed = maxSpeed;
        }
        else 
        {
            targetSpeed = maxSpeed * (distance - targetRadius) / targetRadius;
        }

        Vector3 targetVelocity = direction;
        targetVelocity.Normalize();
        targetVelocity *= targetSpeed;

        result.linear = targetVelocity - character.linear;
        result.linear /= timeToTarget;

        if (result.linear.magnitude > maxAcceleration)
        {
            result.linear.Normalize();
            result.linear *= maxAcceleration;
        }

        return result;
    }
}
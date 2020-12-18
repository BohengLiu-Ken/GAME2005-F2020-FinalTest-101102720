using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBody : MonoBehaviour
{
    // Variables
    public float mass;
    public float friction;
    public Vector3 velocity;
    public Vector3 acceleration;
    public float bounce;
    public bool floorHit;

    public float speed;
    public float gravity;
    public Vector3 forward;

    public int objectType;

    void OnEnable()
    {
        if (gameObject.GetComponent<SphereProperties>() != null)
        {
            velocity = forward * speed;
            acceleration = new Vector3(0.0f, gravity, 0.0f);
        }
        else if (gameObject.GetComponent<CubeBehaviour>() != null)
        {
            acceleration = new Vector3(0.0f, gravity, 0.0f);
            floorHit = false;
        }
    }

    // Update is called once per frame

    void Update()
    {
        velocity += (acceleration * Time.deltaTime);
        transform.position += velocity * Time.deltaTime;

        if (gameObject.GetComponent<CubeBehaviour>() != null && floorHit)
        {
            velocity.y = 0;
            acceleration.y = 0;
        }
    }

    public void CollisionResponseCubeCube(CubeBehaviour cube)
    {
        if (cube.tag == "Floor")
        {
            velocity.y *= 0.0f;
            acceleration.y *= 0.0f;
            floorHit = true;
        }
        else if (cube.tag == "Box")
        {
            velocity.y *= 0.0f;
            acceleration.y *= 0.0f;
        }
        else if (cube.tag == "WallZ")
        {
            velocity.z *= -1 * bounce;
            Debug.Log("In Debug Log");
        }
        else if (cube.tag == "WallX")
        {
            velocity.x *= -1 * bounce;
        }
    }

    public void CollisionResponseCube(CubeBehaviour cube)
    {
        PhysicsBody cubePB = cube.GetComponent<PhysicsBody>();

        Vector3 finalVelocity;
        transform.position -= velocity * Time.deltaTime;
        if (cube.tag == "Floor")
        {
            velocity.y *= -1f * bounce;

            if (velocity.x <= 0.15)
            {
                velocity.x = 0.0f;
                acceleration.y = 0.0f;
            }
        }
        else if (cube.tag == "WallZ")
        {

            velocity.z *= -1 * bounce;

        }
        else if (cube.tag == "WallX")
        {
            velocity.x *= -1 * bounce;
        }
        else
        {
            // Debug.Break();
            finalVelocity =
                ((mass - cubePB.mass) / (mass + cubePB.mass)) * velocity
                + ((2 * cubePB.mass) / (mass + cubePB.mass)) * cubePB.velocity;
            velocity = finalVelocity * bounce;
        }
    }

    public void CollisionResolveSphereCube(CubeBehaviour cube, Vector3 normal)
    {
        PhysicsBody cubePB = cube.GetComponent<PhysicsBody>();

        transform.position -= velocity * Time.deltaTime;

        if (cube.tag == "Floor")
        {
            velocity.y *= -1f * bounce;
            velocity.x *= bounce;
            velocity.z *= bounce;

            if (velocity.y <= 0.15)
            {

                velocity.y = 0.0f;

                acceleration.y = 0.0f;
            }

            if (Vector3.Magnitude(velocity) < 0.61f)
            {
                this.GetComponent<SphereProperties>().Despawn();
            }
        }
        else if (cube.tag == "WallZ")
        {
            velocity.z *= -1 * bounce;
            Debug.Log("In Debug Log");

        }
        else if (cube.tag == "WallX")
        {
            velocity.x *= -1 * bounce;
        }
        else if (cube.tag == "Box")
        {
            Debug.Log("Normal " + normal);
            Vector3 relativeVelocity = cubePB.velocity - velocity;

            float velAlongNormal = Vector3.Dot(relativeVelocity, normal);
            Debug.Log("Velocity along Normal: " + velAlongNormal);

            Vector3 tangentVectorForFriction = relativeVelocity - velAlongNormal * normal;

            float e = Mathf.Min(bounce, cubePB.bounce);
            float minFriction = Mathf.Min((float)friction, (float)cubePB.friction);

            Debug.Log("MinFriction: " + minFriction);


            // Directly
            float j = -(1 - e) * velAlongNormal;

            // With Friction
            float jt = -(1 - e) * Vector3.Dot(relativeVelocity, tangentVectorForFriction);
            Debug.Log("Dot Product: " + Vector3.Dot(relativeVelocity, tangentVectorForFriction));

            friction = Mathf.Sqrt(friction * cubePB.friction);

            // Debug.Log("j with bounce change " + j);
            float inverseMassSphere = 1 / mass;
            float inverseMassCube = 1 / cubePB.mass;
            Debug.Log("Inverse of Sphere " + inverseMassSphere);
            Debug.Log("Inverse of Cube " + inverseMassCube);

            // Normal
            j /= (inverseMassSphere + inverseMassCube);

            // With Friction
            jt /= (inverseMassCube + inverseMassSphere);
            //Debug.Log("j value " + j);

            jt = Mathf.Min(jt, j * friction);

            Vector3 impulse = j * normal * 3;
            // Debug.Log("Impulse: " + impulse);

            velocity -= inverseMassSphere * impulse;
            velocity *= bounce;
            cubePB.velocity.x += inverseMassCube * impulse.x;
            cubePB.velocity.z += inverseMassCube * impulse.z;
        }
    }

    public void CollisionResponseSphere(SphereProperties sphere)
    {
        PhysicsBody spheresPB = sphere.GetComponent<PhysicsBody>();

        Vector3 finalVelocity;
        finalVelocity =
              ((mass - spheresPB.mass) / (mass + spheresPB.mass)) * velocity
              + ((2 * spheresPB.mass) / (mass + spheresPB.mass)) * spheresPB.velocity;
        velocity = finalVelocity * bounce;
    }
}

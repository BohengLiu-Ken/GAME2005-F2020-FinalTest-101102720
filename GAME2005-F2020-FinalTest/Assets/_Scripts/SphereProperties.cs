using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SphereProperties : MonoBehaviour
{
    public float speed = 5;
    float lifeStart;
    public bool isColliding;
    static int staticNumber;
    int bulletNumber;
    private PhysicsBody pb;
    private MeshFilter meshFilter;
    private float m_radius;
    private Bounds m_bounds;
    Vector3 m_scale;
    Vector3 min, max;

    // public PhysicsBody
    void Awake()
    {
        lifeStart = 0;
        bulletNumber = staticNumber++;

        // Calculate Radius
        meshFilter = GetComponent<MeshFilter>();
        m_bounds = meshFilter.mesh.bounds;

        m_radius = m_bounds.extents.x;
        m_radius = Math.Max(m_bounds.extents.x, Math.Max(m_bounds.extents.y, m_bounds.extents.z));
        max = Vector3.Scale(m_bounds.max, transform.localScale) + transform.position;
        min = Vector3.Scale(m_bounds.min, transform.localScale) + transform.position;
        pb = GetComponent<PhysicsBody>();
    }

    // Update is called once per frame
    void Update()
    {
        lifeStart++;
    }

    public void Despawn()
    {
        gameObject.SetActive(false);
        lifeStart = 0;
    }


    // Got Skeletal Mesh Right, now use it for physics
    void OnDrawGizmos()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, m_radius);
    }

    public float getRadius()
    {
        return m_radius;
    }

    public void reverseY(CubeBehaviour cube)
    {

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

[RequireComponent(typeof(ObiSolver))]
public class CustomCollisionEventHandler : MonoBehaviour
{
    ObiSolver solver;

    public static ObiSolver.ObiCollisionEventArgs frame;

    void Awake()
    {
        solver = GetComponent<ObiSolver>();
    }

    void OnEnable()
    {
        solver.OnCollision += Solver_OnCollision;
    }

    void OnDisable()
    {
        solver.OnCollision -= Solver_OnCollision;
    }

    void Solver_OnCollision(object sender, ObiSolver.ObiCollisionEventArgs e)
    {
        frame = e;
    }

    void OnDrawGizmos()
    {
        if (solver == null || frame == null || frame.contacts == null) return;

        Gizmos.matrix = solver.transform.localToWorldMatrix;

        for (int i = 0; i < frame.contacts.Count; ++i)
        {
            Gizmos.color = (frame.contacts.Data[i].distance < 0) ? Color.red : Color.green;

            Vector3 point = frame.contacts.Data[i].point;
            Vector3 normal = frame.contacts.Data[i].normal;

            Gizmos.DrawSphere(point, 0.025f);

            Gizmos.DrawRay(point, normal.normalized * frame.contacts[i].distance);
        }
    }

}

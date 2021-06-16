using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using FateGames;

public class Cable : MonoBehaviour
{

    public Transform Start, End;
    private ObiRope rope;
    private ObiRopeBlueprint blueprint;
    private ObiRopeExtrudedRenderer ropeRenderer;
    private static CableSystem cableSystem;
    ObiPath path;
    private void Awake()
    {
        if (!cableSystem)
            cableSystem = CableSystem.Instance;
        rope = gameObject.AddComponent<ObiRope>();
        rope.selfCollisions = false;
        ropeRenderer = gameObject.AddComponent<ObiRopeExtrudedRenderer>();
        ropeRenderer.section = CableSystem.Instance.Section;
        ropeRenderer.uvScale = new Vector2(1, 5);
        ropeRenderer.normalizeV = false;
        ropeRenderer.uvAnchor = 1;
        rope.GetComponent<MeshRenderer>().material = CableSystem.Instance.Material;
        rope.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        // Setup a blueprint for the rope:
        blueprint = ScriptableObject.CreateInstance<ObiRopeBlueprint>();
        blueprint.resolution = 0.5f;

        // Tweak rope parameters:
        rope.maxBending = 0.02f;
        rope.ropeBlueprint = blueprint;
        path = blueprint.path;
        blueprint.thickness = 0.05f;
        rope.stretchingScale = 0.01f;
    }

    public IEnumerator GenerateCable(Transform start, Transform end)
    {
        Start = start;
        End = end;
        path.Clear();
        path.AddControlPoint(start.position, -transform.right * 0.1f, transform.right * 0.1f, Vector3.up, 0.1f, 0.1f, 0.8f, 1, Color.white, "start");
        path.AddControlPoint(end.position, -transform.right * 0.1f, transform.right * 0.1f, Vector3.up, 0.1f, 0.1f, 0.8f, 1, Color.white, "end");
        Attach(start, blueprint.groups[0]);
        Attach(end, blueprint.groups[1]);
        path.FlushEvents();
        yield return blueprint.Generate();
    }

    private void Attach(Transform target, ObiParticleGroup particleGroup)
    {
        ObiParticleAttachment attachment = gameObject.AddComponent<ObiParticleAttachment>();
        attachment.target = target;
        attachment.particleGroup = particleGroup;
    }

    public void SetColor(Color color)
    {
        rope.GetComponent<MeshRenderer>().material.color = color;
    }

    public bool IntersectsWith(Cable cable)
    {
        bool result = false;
        if (Start == cable.Start || Start == cable.End || End == cable.Start || End == cable.End)
            return result;
        Vector3 pointA = Start.position;
        Vector3 pointB = cable.Start.position;
        pointB.y = pointA.y;
        Vector3 dirA = End.position - Start.position;
        dirA.y = 0;
        Vector3 dirB = cable.End.position - cable.Start.position;
        dirB.y = 0;
        /*Debug.DrawRay(pointA, dirA, Color.blue);
        Debug.DrawRay(pointB, dirB, Color.cyan);*/
        if (Math3D.LineLineIntersection(out Vector3 intersectionPoint, pointA, dirA, pointB, dirB))
        {
            if (Math3D.IsCBetweenAB(Start.position, End.position, intersectionPoint) && Math3D.IsCBetweenAB(cable.Start.position, cable.End.position, intersectionPoint))
                //((PolePlacementLevel)LevelManager.Instance).debugSpheres[0].transform.position = intersectionPoint;
                result = true;
        }



        return result;
    }

    public bool IsAvaliable()
    {
        bool result = true;
        for (int i = 0; i < cableSystem.Connections.Count; i++)
        {
            Cable otherCable = cableSystem.Connections[i];
            if (IntersectsWith(otherCable))
            {
                result = false;
                break;
            }
        }
        return result;
    }

    private void OnDestroy()
    {
        CableSystem.Instance.Connections.Remove(this);
    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using FateGames;

public class CableSystem : MonoBehaviour
{
    public static CableSystem Instance;
    public Material Material;
    public ObiRopeSection Section;
    [SerializeField] Socket[] starts, ends;
    [SerializeField] ObiSolver solver;
    [SerializeField] GameObject cablePrefab;
    public Color UnplaceableColor, PlaceableColor;
    private PolePlacementLevel levelManager;
    public List<Cable> Connections = new List<Cable>();
    public Socket previewSocket;
    private Cable[] previewCables = new Cable[2];

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (starts.Length == ends.Length)
        {
            for (int i = 0; i < starts.Length; i++)
            {
                CreateCable(starts[i].Pole.Head, ends[i].Pole.Head, true);
                starts[i].Occupied = true;
                ends[i].Occupied = true;
            }
        }
        levelManager = (PolePlacementLevel)LevelManager.Instance;
    }

    public Cable FindCable(PolePlacementLevel.Connection connection)
    {
        for (int i = 0; i < Connections.Count; i++)
        {
            if (connection.Check(Connections[i]))
                return Connections[i];
        }
        return null;
    }

    private void Update()
    {
        if (!levelManager.Player.Locked && !levelManager.Player.Pole.Attached && CustomCollisionEventHandler.frame.contacts.Count > 0)
        {
            Oni.Contact contact = CustomCollisionEventHandler.frame.contacts.Data[0];
            ObiSolver.ParticleInActor pa = solver.particleToActor[contact.particle];
            ObiColliderBase collider = ObiColliderWorld.GetInstance().colliderHandles[contact.other].owner;
            Pole pole = collider.GetComponent<Pole>();
            Cable cable = pa.actor.GetComponent<Cable>();
            pole.Attach(cable);
        }
    }

    public Cable CreateCable(Transform start, Transform end, bool isConnection)
    {
        Cable cable = Instantiate(cablePrefab, solver.transform).GetComponent<Cable>();
        StartCoroutine(cable.GenerateCable(start, end));
        if (isConnection)
            Connections.Add(cable);
        return cable;
    }

    public void Preview()
    {

    }

}

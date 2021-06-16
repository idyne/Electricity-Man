using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Player : MonoBehaviour
{
    private Swerve swerve = null;
    private Animator anim = null;
    private bool isHolding = true;
    private PolePlacementLevel levelManager;
    [SerializeField]
    private bool locked = false;

    public Pole Pole;

    [SerializeField] private float speed = 3;
    private void Awake()
    {
        swerve = InputManager.Instance.Swerve;
        swerve.OnRelease = Release;
        anim = GetComponent<Animator>();
        levelManager = (PolePlacementLevel)LevelManager.Instance;
    }
    private void Update()
    {
        if (isHolding)
            Move();
    }

    private void Move()
    {
        if (Pole.Attached)
        {
            Socket nearestSocket = FindNearestSocket();
            if (Vector3.Distance(transform.position, nearestSocket.transform.position) < 1)
            {
                levelManager.SocketCursor.transform.position = nearestSocket.transform.position;
                levelManager.SocketCursor.SetActive(true);
                if (!nearestSocket.Occupied)
                {
                    Vector3 pos = nearestSocket.transform.position;
                    pos.y = levelManager.previewPole.transform.position.y;
                    levelManager.previewPole.transform.position = pos;
                    levelManager.previewPole.SetActive(true);
                }
                else
                {
                    levelManager.previewPole.SetActive(false);
                }
            }
            else
            {
                levelManager.previewPole.SetActive(false);
                levelManager.SocketCursor.SetActive(false);
            }
        }
        else
        {
            levelManager.previewPole.SetActive(false);
            levelManager.SocketCursor.SetActive(false);
        }
        if (swerve.Active && swerve.Difference.magnitude >= 0.1f)
        {
            locked = false;
            anim.SetBool("Moving", true);
            anim.SetBool("Holding", isHolding);
            Vector3 direction = new Vector3(swerve.Difference.x, 0, swerve.Difference.y);
            direction.Normalize();
            transform.LookAt(transform.position + direction);
            transform.rotation *= Quaternion.Euler(Vector3.up * 180);
            transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward * speed, Time.deltaTime * speed);
        }
        else
        {
            anim.SetBool("Moving", false);
            anim.SetBool("Holding", isHolding);
        }

    }

    private void Release()
    {
        print("Release");
        if (isHolding && Pole.Attached && Pole.Detachable)
        {
            Socket nearestSocket = FindNearestSocket();
            if (Vector3.Distance(transform.position, nearestSocket.transform.position) < 1)
            {
                locked = true;
                bool occupied = nearestSocket.Occupied;
                Pole.GetEnds(out Transform start, out Transform end);
                bool success = nearestSocket.Occupy(start, end);
                if (success)
                {
                    Pole.Detach();
                    if (!levelManager.CheckPattern())
                        TeleportToStartPoint();
                    else
                    {
                        locked = true;
                        GetComponent<Rigidbody>().isKinematic = true;
                        Pole.gameObject.SetActive(false);
                    }
                    CreateNewRecord(start, nearestSocket.Pole.Head, end, nearestSocket, !occupied);
                }

            }
        }
    }

    private void CreateNewRecord(Transform start, Transform middle, Transform end, Socket socket, bool occupied)
    {
        Transform[] oldEnds = new Transform[2];
        oldEnds[0] = start;
        oldEnds[1] = end;
        PolePlacementLevel.Connection oldConnection = new PolePlacementLevel.Connection(oldEnds);
        PolePlacementLevel.Connection[] newConnections = new PolePlacementLevel.Connection[2];
        Transform[] newEnds_1 = new Transform[2];
        newEnds_1[0] = start;
        newEnds_1[1] = middle;
        newConnections[0] = new PolePlacementLevel.Connection(newEnds_1);
        Transform[] newEnds_2 = new Transform[2];
        newEnds_2[0] = middle;
        newEnds_2[1] = end;
        newConnections[1] = new PolePlacementLevel.Connection(newEnds_2);
        levelManager.Records.Push(new PolePlacementLevel.Record(oldConnection, newConnections, socket, occupied));
        levelManager.recordCount++;
    }

    private Socket FindNearestSocket()
    {
        float minDistance = int.MaxValue;
        Socket nearestSocket = null;
        for (int i = 0; i < levelManager.Sockets.Count; ++i)
        {
            Socket socket = levelManager.Sockets[i];
            float distance = Vector3.Distance(transform.position, socket.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestSocket = socket;
            }
        }
        return nearestSocket;
    }

    public void TeleportToStartPoint()
    {
        Destroy(Instantiate(levelManager.poofEffectPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity), 3);
        transform.position = levelManager.StartPoint.position;
        transform.localRotation = Quaternion.identity;
        Destroy(Instantiate(levelManager.poofEffectMutePrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity), 3);
    }

    public bool Locked
    {
        get
        {
            return locked;
        }
    }
}

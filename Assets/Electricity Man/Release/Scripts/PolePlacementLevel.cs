using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class PolePlacementLevel : LevelManager
{
    private Player player;
    private CableSystem cableSystem;
    public List<Socket> Sockets;
    public Transform StartPoint;
    public GameObject previewPole;
    public Transform[] debugSpheres;
    public GameObject SocketCursor;
    private Engineer[] engineers;
    public bool Done = false;
    [SerializeField] private Connection[] pattern;
    public Stack<Record> Records = new Stack<Record>();
    public int recordCount = 0;
    public GameObject poofEffectPrefab, poofEffectMutePrefab;
    public Renderer[] ledLights;
    public int[] ledLigthMaterialIndexes;
    public GameObject patternGameObject;
    public GameObject undoButton;

    [System.Serializable]
    public class Connection
    {
        [SerializeField]
        public List<Transform> ends;

        public Connection() { }
        public Connection(Transform[] ends)
        {
            this.ends = new List<Transform>(ends);
        }

        public bool Check(Cable cable)
        {
            if (ends.Contains(cable.Start) && ends.Contains(cable.End))
                return true;
            return false;
        }
    }

    public class Record
    {
        public Connection oldConnection;
        public List<Connection> newConnections;
        public Socket newSocket;
        public bool occupied = false;

        public Record(Connection oldConnection, Connection[] newConnections, Socket newSocket, bool occupied)
        {
            this.oldConnection = oldConnection;
            this.newConnections = new List<Connection>(newConnections);
            this.newSocket = newSocket;
            this.occupied = occupied;
        }
    }

    public void Undo()
    {
        if (Records.Count > 0 && !Done)
        {
            Record record = Records.Pop();
            recordCount = Records.Count;
            cableSystem.CreateCable(record.oldConnection.ends[0], record.oldConnection.ends[1], true);
            if (record.occupied)
            {
                record.newSocket.Pole.gameObject.SetActive(false);
                record.newSocket.Occupied = false;
            }

            for (int i = 0; i < record.newConnections.Count; i++)
            {
                Cable cable = cableSystem.FindCable(record.newConnections[i]);
                cableSystem.Connections.Remove(cable);
                Destroy(cable.gameObject);
            }
            player.Pole.Detach();
            player.TeleportToStartPoint();
        }
    }
    private new void Awake()
    {
        base.Awake();
        player = FindObjectOfType<Player>();
        cableSystem = FindObjectOfType<CableSystem>();
        Sockets = new List<Socket>(FindObjectsOfType<Socket>());
        engineers = FindObjectsOfType<Engineer>();
    }
    public override void FinishLevel(bool success)
    {
        patternGameObject.transform.LeanScale(Vector3.zero, 0.5f).setEaseInQuad();
        undoButton.transform.LeanScale(Vector3.zero, 0.5f).setEaseInQuad();
        Done = true;
        LeanTween.delayedCall(1, () =>
        {
            FindObjectOfType<DynamicCamera>().MoveToNextPosition(out float t);
            LeanTween.delayedCall(t, () =>
            {
                AnimateLeds();
                LeanTween.delayedCall(0.7f, () =>
                {
                    foreach (Engineer engineer in engineers)
                        engineer.Cheer();
                });
            });
            LeanTween.delayedCall(t + 1, () => { GameManager.Instance.FinishLevel(true); });
        });
    }

    private void AnimateLeds()
    {
        for (int i = 0; i < ledLights.Length; i++)
        {
            Material material = ledLights[i].materials[ledLigthMaterialIndexes[i]];
            LeanTween.delayedCall(0.1f, () => { material.EnableEmission(); });
            LeanTween.delayedCall(0.15f, () => { material.DisableEmission(); });
            LeanTween.delayedCall(0.35f, () => { material.EnableEmission(); });
            LeanTween.delayedCall(0.45f, () => { material.DisableEmission(); });
            LeanTween.delayedCall(0.7f, () => { material.EnableEmission(); });
            //material.SetVector("_EmissionColor", Color.blue * 10f);
        }
    }

    public override void StartLevel()
    {
        print("Pole placement level");

    }
    public Player Player
    {
        get
        {
            return player;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            FinishLevel(true);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Undo();
        }
    }

    public bool CheckPattern()
    {
        bool result = false;
        if (pattern.Length == cableSystem.Connections.Count)
        {
            int count = 0;
            for (int i = 0; i < pattern.Length; i++)
            {
                for (int j = 0; j < cableSystem.Connections.Count; j++)
                {
                    if (pattern[i].Check(cableSystem.Connections[j]))
                    {
                        count++;
                        break;
                    }
                }
            }
            if (count == pattern.Length)
            {
                result = true;
                FinishLevel(true);
            }

        }
        return result;
    }

}

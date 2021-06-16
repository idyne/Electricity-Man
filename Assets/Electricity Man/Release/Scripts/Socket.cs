using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class Socket : MonoBehaviour
{
    public bool Occupied = false;
    public StaticPole Pole;
    private static CableSystem cableSystem;
    private static PolePlacementLevel levelManager;
    private void Awake()
    {
        if (!cableSystem)
            cableSystem = CableSystem.Instance;
        if (!levelManager)
            levelManager = (PolePlacementLevel)LevelManager.Instance;
    }

    public bool Occupy(Transform start, Transform end)
    {

        bool success = IsAvaliable(start, Pole.Head) && IsAvaliable(Pole.Head, end);
        if (success)
        {
            Pole.gameObject.SetActive(true);
            Occupied = true;
            cableSystem.CreateCable(start, Pole.Head, true);
            cableSystem.CreateCable(Pole.Head, end, true);
        }
        return success;
    }

    private bool IsAvaliable(Transform start, Transform end)
    {
        bool avaliable = true;
        List<Socket> socketsBetween = new List<Socket>();
        for (int i = 0; i < levelManager.Sockets.Count; i++)
        {
            Socket socket = levelManager.Sockets[i];
            if (socket.Occupied && Math3D.IsCBetweenAB(start.position, end.position, socket.Pole.Head.position))
            {
                print(start.position);
                print(end.position);
                print(socket.Pole.Head.position);
                socketsBetween.Add(socket);
            }
        }
        /*for (int i = 0; i < socketsBetween.Count; i++)
        {
            for (int j = 0; j < socketsBetween.Count; j++)
            {
                for (int k = 0; k < cableSystem.Connections.Count; k++)
                {
                    Cable connection = cableSystem.Connections[i];
                    print(socketsBetween[i].Pole.Head == connection.Start);
                    print(socketsBetween[j].Pole.Head == connection.End);
                    print(socketsBetween[i].name + " " + (socketsBetween[i].Pole.Head == connection.End));
                    print(socketsBetween[j].name + " " + (socketsBetween[j].Pole.Head == connection.Start));
                    if (socketsBetween[i].Pole.Head == connection.Start && socketsBetween[j].Pole.Head == connection.End
                        || socketsBetween[i].Pole.Head == connection.End && socketsBetween[j].Pole.Head == connection.Start)
                    {
                        avaliable = false;
                        break;
                    }
                }
                if (!avaliable)
                    break;
            }
            if (!avaliable)
                break;
        }*/
        if (avaliable)
        {
            socketsBetween.Sort(delegate (Socket a, Socket b)
            {
                return Vector3.Distance(start.position, a.Pole.Head.position)
                .CompareTo(
                  Vector3.Distance(start.position, b.Pole.Head.position));
            });
            for (int i = 0; i < cableSystem.Connections.Count; i++)
            {
                Cable otherCable = cableSystem.Connections[i];
                if (otherCable.Start == start && otherCable.End == end || otherCable.Start == end && otherCable.End == start)
                {
                    avaliable = false;
                    break;
                }
            }
        }

        return avaliable;
    }
}

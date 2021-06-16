using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

public class Pole : MonoBehaviour
{
    public Transform Head = null;
    public bool Attached = false;
    private CableSystem cableSystem;
    private ObiCollider obiCollider;
    private Cable[] cables = new Cable[2];
    public bool Detachable
    {
        get
        {
            return cables[0].IsAvaliable() && cables[1].IsAvaliable();
        }
    }


    private void Awake()
    {
        cableSystem = CableSystem.Instance;
        obiCollider = GetComponent<ObiCollider>();
    }

    private void Update()
    {
        if (Attached)
        {
            cables[0].SetColor(cables[0].IsAvaliable() ? cableSystem.PlaceableColor : cableSystem.UnplaceableColor);
            cables[1].SetColor(cables[1].IsAvaliable() ? cableSystem.PlaceableColor : cableSystem.UnplaceableColor);
        }

    }

    public void Attach(Cable cable)
    {
        obiCollider.enabled = false;
        cables[0] = cableSystem.CreateCable(cable.Start, Head, false);
        cables[1] = cableSystem.CreateCable(Head, cable.End, false);
        Destroy(cable.gameObject);
        Attached = true;
    }

    public void Detach()
    {
        if (Attached)
        {
            for (int i = 0; i < cables.Length; i++)
            {
                Cable cable = cables[i];
                cables[i] = null;
                Destroy(cable.gameObject);
            }
            Attached = false;
            obiCollider.enabled = true;
        }
    }

    public void GetEnds(out Transform start, out Transform end)
    {
        start = null;
        end = null;
        if (Attached)
        {
            start = cables[0].Start;
            end = cables[1].End;
        }
    }
}

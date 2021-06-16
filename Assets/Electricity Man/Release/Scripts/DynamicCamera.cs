using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;

public class DynamicCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 1;
    [SerializeField] private Vector3 nextPosition = new Vector3(7.61f, 26.05f, 9.59f);
    [SerializeField] private float nextAngle = 45;
    private PolePlacementLevel levelManager;

    private void Awake()
    {
        levelManager = (PolePlacementLevel)LevelManager.Instance;
    }

    private void Update()
    {
        if (!levelManager.Done)
            Follow();
    }

    private void Follow()
    {
        Vector3 pos = transform.position;
        float x = target.position.x;
        pos.x = (9.05f * x + 0.625f) / 9;
        transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);
    }
    public void MoveToNextPosition(out float t)
    {
        t = 2;
        transform.LeanMove(nextPosition, t);
        transform.LeanRotateX(nextAngle, t);

    }
}

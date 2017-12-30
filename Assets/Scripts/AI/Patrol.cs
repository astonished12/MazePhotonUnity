using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Patrol : MonoBehaviour
{

    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private GameObject worldGen;

    void Start()
    {
        worldGen = GameObject.Find("WorldGen").gameObject;
        agent = GetComponent<NavMeshAgent>();
        points = new Transform[4];

        CreateReferencePoitns();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;
        GotoNextPoint();
    }

    void CreateReferencePoitns()
    {
        int width = worldGen.GetComponent<MazeGenerator>().Width;
        int height = worldGen.GetComponent<MazeGenerator>().Height;
        GameObject leftUpperCorner = new GameObject();
        leftUpperCorner.name = "leftUpperCorner";
        leftUpperCorner.transform.position = worldGen.GetComponent<MazeGenerator>().cellsGroundPositionSpawn[0];
        GameObject rightUpperCorner = new GameObject();
        rightUpperCorner.name = "rightUpperCorner";
        rightUpperCorner.transform.position = worldGen.GetComponent<MazeGenerator>().cellsGroundPositionSpawn[width-1];
        GameObject leftDownCorner = new GameObject();
        leftDownCorner.name = "leftDownCorner";
        leftDownCorner.transform.position = worldGen.GetComponent<MazeGenerator>().cellsGroundPositionSpawn[width*height-width];
        GameObject rightDownCorner = new GameObject();
        rightDownCorner.name = "rightDownCorner";
        rightDownCorner.transform.position = worldGen.GetComponent<MazeGenerator>().cellsGroundPositionSpawn[width*height-1];

        int k = 0;
        points[k++] = leftUpperCorner.transform;
        points[k++] = rightUpperCorner.transform;
        points[k++] = leftDownCorner.transform;
        points[k++] = rightDownCorner.transform;

    }

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.SetDestination(points[destPoint].position);

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }


    void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (agent.remainingDistance < 0.5f)
            GotoNextPoint();
    }
}


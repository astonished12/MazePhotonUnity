using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class Patrol : Photon.MonoBehaviour
{

    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private GameObject worldGen;
    private Animator anim;

    void Awake()
    {
        worldGen = GameObject.Find("WorldGen").gameObject;
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        points = new Transform[4];
        CreateReferencePoitns();
    }
    void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {
            GetComponent<PhotonView>().RPC("GoToNextPoint", PhotonTargets.All,0);
            GetComponent<PhotonView>().RPC("ChangeAnimToPatrol", PhotonTargets.All);
        }
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

  
    void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (agent.remainingDistance < 0.5f && PhotonNetwork.isMasterClient)
        {
            destPoint = (destPoint + 1) % points.Length;
            GetComponent<PhotonView>().RPC("GoToNextPoint", PhotonTargets.All, destPoint);
        }
    }

    [PunRPC]
    public void GoToNextPoint(int destPoint)
    {
        agent.SetDestination(points[destPoint].position);
    }

    [PunRPC]
    public void ChangeAnimToPatrol()
    {
        anim.SetBool("Patrol",true);
    }
    

   

}



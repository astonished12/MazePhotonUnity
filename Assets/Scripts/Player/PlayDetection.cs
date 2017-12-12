using UnityEngine;

public class PlayDetection : MonoBehaviour
{

    private float trackingRange = 1.5f;

    void Start()
    {
       // Random.InitState(MazeGenerator.realSize);

    }
    // Update is called once per frame
    void Update()
    {
        TargetEnemy();

    }

    void TargetEnemy()
    {
        float hirange = float.MaxValue;
        Collider[] cols = Physics.OverlapSphere(transform.position, trackingRange);
        foreach (Collider col in cols)
        {
            GameObject target = col.gameObject;
            if (target != gameObject && target.CompareTag("Exit"))
            {
                int randomPositionInMatrix = UnityEngine.Random.Range(0, MazeGenerator.realSize * MazeGenerator.realSize);
                GameObject.Find("Network").GetComponent<PhotonView>().RPC("ChangePosition", PhotonTargets.All, randomPositionInMatrix);
                var h = GetComponent<Health>();
                h.GetComponent<PhotonView>().RPC("GiveHealth", PhotonTargets.All, 100);
                var ps = GetComponent<PlayerShoting>();
                ps.GetComponent<PhotonView>().RPC("GiveBullets",PhotonTargets.All,20);

            }
        }

    }
}

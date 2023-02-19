using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MoleManager : MonoBehaviourPunCallbacks
{
    bool m_IsPlayerInSight;

    public bool[] isUpArray = new bool[9];
    public int i_tmp;

    public void IsRun()
    {
        m_IsPlayerInSight = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_IsPlayerInSight)
        {
            if (transform.position.y > 0.0f)
                transform.position -= new Vector3(0, 1.0f, 0) * Time.deltaTime;
            else
            {
                photonView.RPC("DestroyRPC", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    void DestroyRPC()
    {
        isUpArray[i_tmp] = false;
        Destroy(gameObject);
    }
}

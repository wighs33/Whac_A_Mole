using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class AttackObserver : MonoBehaviourPunCallbacks
{
    public PlayerManager m_playerManager;
    GameObject mole_get;

    void OnTriggerEnter(Collider other)
    {
        foreach (var mole in GameObject.FindGameObjectsWithTag("Mole"))
            if (other.transform == mole.transform)
            {
                mole_get = mole;
                return;
            }
    }

    void OnTriggerExit(Collider other)
    {
        foreach (var mole in GameObject.FindGameObjectsWithTag("Mole"))
            if (other.transform == mole.transform)
            {
                mole_get = null;
                return;
            }
    }

    void Update()
    {
        if (mole_get && transform.GetComponentInParent<Animator>().GetBool("Attack"))
        {
            mole_get.GetComponent<PhotonView>().RPC("DestroyRPC", RpcTarget.All);
            m_playerManager.RaiseScore();
        }
    }
}

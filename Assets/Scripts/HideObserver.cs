using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class HideObserver : MonoBehaviourPunCallbacks
{
    public MoleManager m_moleManager;

    public Transform playerTrans1;
    public Transform playerTrans2;

    bool m_IsPlayerInRange1;
    bool m_IsPlayerInRange2;

    void OnTriggerEnter(Collider other)
    {
        if (other.transform == playerTrans1)
        {
            m_IsPlayerInRange1 = true;
            return;
        }
        if (PhotonNetwork.PlayerList.Length != 1 && other.transform == playerTrans2)
        {
            m_IsPlayerInRange2 = true;
        }
    }

    void Update()
    {
        if (m_IsPlayerInRange1 || m_IsPlayerInRange2)
        {
            if (PhotonNetwork.PlayerList.Length == 1) playerTrans2 = playerTrans1;

            Vector3 direction = m_IsPlayerInRange1
            ? (playerTrans1.transform.position - transform.position + Vector3.up)
            : (playerTrans2.transform.position - transform.position + Vector3.up);

            Ray ray = new Ray(transform.position, direction);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit))
            {
                if (raycastHit.collider.transform
                    == playerTrans1 ? playerTrans1.transform : playerTrans2.transform)
                {
                    m_moleManager.IsRun();
                }
            }
        }
    }
}

// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerAnimatorManager.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in PUN Basics Tutorial to deal with the networked player Animator Component controls.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public TextMesh nickname;

    public Animator m_Animator;

    public float turnSpeed = 20f;

    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    int score;

    public void RaiseScore()
    {
        score += 1;
    }

    public int GetScore()
    {
        return score;
    }

    #region MonoBehaviour CallBacks

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();

        score = 0;
    }

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity on every frame.
    /// </summary>
    void Update()
    {

        // Prevent control is connected to Photon and represent the localPlayer
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        // failSafe is missing Animator component on GameObject
        if (!m_Animator)
        {
            return;
        }

        AnimatorStateInfo stateInfo = m_Animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName("Base Layer.Attack1"))
        {
            if (Input.GetButtonDown("Attack"))
            {
                m_Animator.SetBool("Attack", true);
            }
        }
        else
        {
            m_Animator.SetBool("Attack", false);
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            m_Movement.Set(horizontal, 0f, vertical);
            m_Movement.Normalize();

            bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
            bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
            bool isWalking = hasHorizontalInput || hasVerticalInput;
            m_Animator.SetBool("IsWalking", isWalking);

            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);
            m_Rotation = Quaternion.LookRotation(desiredForward);
        }
    }

    void OnAnimatorMove()
    {
        if (photonView.IsMine)
        {
            //transform.position += m_Movement * Time.deltaTime;
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * Time.deltaTime * 2.0f);
            m_Rigidbody.MoveRotation(m_Rotation);
            nickname.transform.rotation = Quaternion.identity;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(score);
            stream.SendNext(nickname.text);
        }
        else
        {
            score = (int)stream.ReceiveNext();
            nickname.text = (string)stream.ReceiveNext();
        }
    }
    #endregion

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private GameObject m_playerPrefab;

    [SerializeField]
    private GameObject molePrefab;
    
    float m_timer;
    int TimeCheck = 0;

    GameObject newMole = null;
    GameObject player1 = null;
    GameObject player2 = null;
    Transform player2_transform = null;
    PlayerManager player2_pm;

    bool[] isUpArray = new bool[9];

    public Text countText;
    public Text p1Text;
    public Text p2Text;

    public Text score1Text;
    public Text score2Text;

    public Image gameEnd;
    public Text winnerNameEnd;
    public Text loserNameEnd;
    public Text winnerScoreEnd;
    public Text loserScoreEnd;
    public Text winner;

    PlayerManager playerManager;

    public DB_Manager db_m;
    bool isGameEnd;

    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;

        score1Text.text = "0";
        if (PhotonNetwork.PlayerList.Length != 1)
            score2Text.text = "0";

        winnerScoreEnd.text = "0";
        if (PhotonNetwork.PlayerList.Length != 1)
            loserScoreEnd.text = "0";
    }

    // Start is called before the first frame update
    void Start()
    {
        SetNickname();
        SpawnPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameEnd) return;

        Countdown_AndIf0GameEnd();

        if (photonView.IsMine)
        {
            if (PhotonNetwork.PlayerList.Length != 1)
                StartCoroutine(GetPlayer2Info());

            score1Text.text = playerManager.GetScore().ToString();

            SpawnAMole_Every5Seconds();
        }
        else
        {
            score2Text.text = playerManager.GetScore().ToString();
        }
    }

    void SetNickname()
    {
        if (photonView.IsMine)
        {
            p1Text.text = PhotonNetwork.NickName;
            if (PhotonNetwork.PlayerList.Length != 1)
                p2Text.text = PhotonNetwork.PlayerList[1].NickName;
        }
        else
            p2Text.text = PhotonNetwork.NickName;
    }

    void SpawnPlayers()
    {
        if (photonView.IsMine)
        {
            player1 = PhotonNetwork.Instantiate(this.m_playerPrefab.name, new Vector3(1.0f, 0.5f, 0.0f), Quaternion.identity, 0);
            playerManager = player1.GetComponent<PlayerManager>();
            playerManager.nickname.text = p1Text.text;
        }
        else
        {
            player2 = PhotonNetwork.Instantiate(this.m_playerPrefab.name, new Vector3(-1.0f, 0.5f, 0.0f), Quaternion.identity, 0);
            playerManager = player2.GetComponent<PlayerManager>();
            playerManager.nickname.text = p2Text.text;
        }
    }

    IEnumerator GetPlayer2Info()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player1.transform.position != player.transform.position)
            {
                player2_transform = player.transform;
                score2Text.text = player.GetComponent<PlayerManager>().GetScore().ToString();
                yield return new WaitForEndOfFrame();
            }
        }
    }

    void Settlement()
    {
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            winner.gameObject.SetActive(false);
            winnerNameEnd.text = p1Text.text + " : ";
            winnerScoreEnd.text = score1Text.text;
            return;
        }

        if (int.Parse(score1Text.text) == int.Parse(score2Text.text))
        {
            winnerNameEnd.text = p1Text.text + " : ";
            loserNameEnd.text = p2Text.text + " : ";
            winner.text = "Draw";
            return;
        }

        winnerNameEnd.text = int.Parse(score1Text.text) > int.Parse(score2Text.text) ? p1Text.text + " : " : p2Text.text + " : ";
        loserNameEnd.text = int.Parse(score1Text.text) < int.Parse(score2Text.text) ? p1Text.text + " : " : p2Text.text + " : ";
        winnerScoreEnd.text = int.Parse(score1Text.text) > int.Parse(score2Text.text) ? score1Text.text : score2Text.text;
        loserScoreEnd.text = int.Parse(score1Text.text) < int.Parse(score2Text.text) ? score1Text.text : score2Text.text;
    }

    void Countdown_AndIf0GameEnd()
    {
        m_timer += Time.deltaTime;

        if (countText.text == "0")
        {
            Settlement();

            gameEnd.gameObject.SetActive(true);

            db_m.WriteDB(p1Text.text, int.Parse(winnerScoreEnd.text));
            if (PhotonNetwork.PlayerList.Length != 1)
                db_m.WriteDB(p2Text.text, int.Parse(loserScoreEnd.text));

            isGameEnd = true;
        }
        else
        {
            if (photonView.IsMine)
                countText.text = (120 - (int)m_timer).ToString();
        }
    }

    void SpawnAMole(int i_tmp, int mole_x_normal, int mole_z_normal, int mole_angle)
    {
        newMole = PhotonNetwork.Instantiate(this.molePrefab.name, new Vector3(mole_x_normal * 6.0f, 1.0f, mole_z_normal * 6.0f), Quaternion.AngleAxis(mole_angle * 90.0f, Vector3.up), 0);
        MoleManager moleManager = newMole.GetComponent<MoleManager>();
        moleManager.isUpArray = isUpArray;
        moleManager.i_tmp = i_tmp;

        GameObject view = newMole.transform.Find("PointOfView").gameObject;
        HideObserver hideObserver = view.GetComponent<HideObserver>();

        hideObserver.playerTrans1 = player1.transform;
        if (PhotonNetwork.PlayerList.Length != 1)
            hideObserver.playerTrans2 = player2_transform;
    }

    void SpawnAMole_PositionCheck()
    {
        int mole_x_normal = UnityEngine.Random.Range(-1, 2);
        int mole_z_normal = UnityEngine.Random.Range(-1, 2);
        int mole_angle = UnityEngine.Random.Range(0, 4);

        for (int i_mole_x_noraml = -1; i_mole_x_noraml < 2; ++i_mole_x_noraml)
            if (mole_x_normal == i_mole_x_noraml)
            {
                for (int i_mole_z_normal = -1; i_mole_z_normal < 2; ++i_mole_z_normal)
                    if (mole_z_normal == i_mole_z_normal)
                    {
                        int i_tmp = (mole_x_normal + 1) * 3 + mole_z_normal + 1;
                        //x,z방향 둘다 0이면 인덱스는 4
                        if (i_tmp != 4 && !isUpArray[i_tmp])
                        {
                            isUpArray[i_tmp] = true;
                            SpawnAMole(i_tmp, mole_x_normal, mole_z_normal, mole_angle);
                            return;
                        }
                    }
            }
    }

    void SpawnAMole_Every5Seconds()
    {
        //5초마다
        if ((int)m_timer % 5 == 0)
        {
            //하나만
            if (TimeCheck != (int)m_timer)
            {
                newMole = null;
                TimeCheck = (int)m_timer;
                SpawnAMole_PositionCheck();
            }
            if (newMole)
                newMole.transform.position += new Vector3(0, 0.5f, 0) * Time.deltaTime;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(countText.text);
            stream.SendNext(score1Text.text);
            stream.SendNext(p1Text.text);
        }
        else
        {
            countText.text = (string)stream.ReceiveNext();
            score1Text.text = (string)stream.ReceiveNext();
            p1Text.text = (string)stream.ReceiveNext();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LobbyScene");
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        countText.text = "0";
    }

    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
}

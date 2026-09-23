using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class NetworkManager : MonoBehaviourPunCallbacks
{
    public int maxPlayers = 10;
    public static NetworkManager instance;
    
    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }
    
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

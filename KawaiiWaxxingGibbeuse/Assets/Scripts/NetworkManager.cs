using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class NetworkManager : MonoBehaviour
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

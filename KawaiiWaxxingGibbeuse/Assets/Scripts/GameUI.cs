using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUI : MonoBehaviour
{
    public Slider healthBar;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI playerInfoText;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI winText;
    public Image winBackground;

    private PlayerController player;

    public static GameUI instance;
    

    void Awake()
    {
        instance = this;
    }

    public void Initialize (PlayerController localPlayer)
    {
        player = localPlayer;
        healthBar.maxValue = player.maxHP;
        healthBar.value = player.curHP;
        healthText.text = player.maxHP.ToString();

        UpdatePlayerInfoText();
        UpdateAmmoText(player.activeGun);
    }

    public void UpdateHealthBar()
    {
        healthBar.value = player.curHP;
        healthText.text = player.curHP.ToString();
    }

    public void UpdateAmmoText(string activeGun)
    {
        if(activeGun == "Pistol")
        ammoText.text = player.weapon.ps.mag + " / " + player.weapon.ps.curAmmo;

        if(activeGun == "Shotgun")
        ammoText.text = player.weapon.sg.mag + " / " + player.weapon.sg.curAmmo;

        if(activeGun == "Rifle")
        ammoText.text = player.weapon.rf.mag + " / " + player.weapon.rf.curAmmo;
    }

    public void SetWinText(string winnerName)
    {
        winBackground.gameObject.SetActive(true);
        winText.text = winnerName + " wins";
    }

    public void UpdatePlayerInfoText()
    {
        playerInfoText.text = "<b>Alive:</b> " + GameManager.instance.alivePlayers + "\n<b> Kills:</b> " + player.kills;
    }

}

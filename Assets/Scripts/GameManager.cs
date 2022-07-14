using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public ParticleSystem BurstParticle;
    public Transform DestructablesParent;

    public GameObject MobileUI;

    public bool isMobile = false;

    public override void Awake()
    {
        base.Awake();
        if (SystemInfo.deviceType == DeviceType.Handheld)
            isMobile = true;
        else
            isMobile = false;
    }

    private void Start()
    {
        if (isMobile)
            MobileUI.SetActive(true);
        else
            MobileUI.SetActive(false);

    }

    public void ResetDestructable()
    {
        foreach (Transform i in DestructablesParent)
            i.gameObject.SetActive(true);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

}

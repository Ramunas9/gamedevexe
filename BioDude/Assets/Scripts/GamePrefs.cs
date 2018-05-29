﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GamePrefs {
    static GameObject[] weaponArray;

    public static void DeletePlayerProgress()
    {
        PlayerPrefs.DeleteKey("LastLevelCheckpoint");
        PlayerPrefs.DeleteKey("PlayerHP");
        PlayerPrefs.DeleteKey("pistolAmmo");
        PlayerPrefs.DeleteKey("shotgunAmmo");
        PlayerPrefs.DeleteKey("assaultRifleAmmo");
        PlayerPrefs.DeleteKey("rocketAmmo");
        PlayerPrefs.DeleteKey("fragGrenadeAmmo");
        PlayerPrefs.DeleteKey("gravnadeAmmo");
        PlayerPrefs.DeleteKey("PistolDiscovered");
        PlayerPrefs.DeleteKey("RocketLauncherDiscovered");
        PlayerPrefs.DeleteKey("AssaultRifleDiscovered");
        PlayerPrefs.DeleteKey("ShotgunDiscovered");
        PlayerPrefs.DeleteKey("DualPistolDiscovered");
    }


}
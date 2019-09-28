using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalStates {
    // some global states stored here
    public static bool isShift = false;
    // If the grid is set to be visible
    public static bool isGridVisible = true;
    // If the shadow is visible
    public static bool isShadowVisible = true;

    public static bool isIndicatorEnabled = true;

    public static Portalble.PortalbleConfig globalConfigFile = new Portalble.PortalbleConfig();
}

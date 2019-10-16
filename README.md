# Portalble
Welcome to Portal-ble! This system allows you to directly manipulate virtual objects in a smartphone augmented reality (AR)
environment with your bare hands. Current master branch is for Android only. Please see iOS branch for updates and schedules.

**Important: This branch is Android only, your android version should be at least >= Ver.7.0 (Nougat) with latest ARCore installed. iOS version will be ready soon**

Website: https://portalble.cs.brown.edu

# Environment Requirements:

Android SDK 7.0 or greater, you can find Android SDK by installing [Android Studio](https://developer.android.com/studio)

[Leap Motion Orion 4.0](https://developer.leapmotion.com/releases/leap-motion-orion-400) (**note, Leap Moion Orion 3.2, Orion Beta, or other ealier Leap Motion drivers WILL NOT WORK with our system. Please upgrade to Leap Motion Orion 4.0**)

[Unity Hub + Unity 2019.1.14f1](https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup.exe) or Later

Use Unity Package Manager to install the following:

Unity ARFoundation 2.1.1

ARCore XR Plugin 2.1.1

ARKit XR Plugin 2.1.1

# Example File:
Examples/Grab/BasicGrab.unity

# Building for Android:
[Pre-flight] - Check you have installed the [Android Studio](https://developer.android.com/studio) and [Leap Motion Orion 4.0](https://developer.leapmotion.com/releases/leap-motion-orion-400) and Using a **64-bit windows computer**

[Step 0] - Make sure your **smartphone** and **windows pc** are under the **same** network

[Step 1 -  Unity Configuration](https://youtu.be/JmuZOQ3fii4 "Step 1 -  Unity Configuration")

Step 2- plugin Leap Motion device to a Windows 64-bit PC (or Intel computer stick below), 
Run this file:  **.../Portalble/PortalbleServer/x64/Rlease/PortalbleService.exe****
You should see message "HMD is all set" at the end of the log.

Step 3- Run the android instance on your phone, if it succeeds, you will see a message logs on the **PortalbleService.exe** window, if **PortalbleService is not refreshing, click on that window and press anykey to refresh**

# Hardware configuration
Leap Motion
[Intel Compute Stick](https://www.intel.com/content/www/us/en/products/boards-kits/compute-stick/stk2m3w64cc.html)
[20 W battery power bank (5V, 4Ah) or equivalent](https://www.amazon.com/gp/product/B01LRQDAEI/ref=ppx_yo_dt_b_search_asin_title?ie=UTF8&psc=1)
[Lasercut case and 3D print files]


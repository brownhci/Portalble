 
# Portalble
Welcome to Portal-ble! This system allows you to directly manipulate virtual objects in a smartphone augmented reality (AR)
environment with your bare hands. Current master branch is for Android only. Please see iOS branch for updates and schedules.

**Important: your android version should be at least >= Ver.7.0 (Nougat) with latest ARCore installed. 
iOS version >= Ver.13

Website: https://portalble.cs.brown.edu

# Environment Requirements:

- [Leap Motion Hand Tracker](https://www.leapmotion.com/where-to-buy/global/)

- [Leap Motion Orion 4.0](https://developer.leapmotion.com/releases/leap-motion-orion-400) (**note, Leap Moion Orion 3.2, Orion Beta, or other ealier Leap Motion drivers WILL NOT WORK with our system. Please upgrade to Leap Motion Orion 4.0**)

- [Unity Hub + Unity 2019.1.14f1](https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup.exe) or Later

- If somehow the Unity XR / AR dependancies are missing, download the following using **Unity Package Manager**

    Unity ARFoundation 2.1.1

    ARCore XR Plugin 2.1.1

    ARKit XR Plugin 2.1.1
    
## Making it "Portable"
You do not need the comptue stick if you only want to test the hand tracking in stationary manner. Any 64-bit windows computer can be used as a server. 

[Intel Compute Stick](https://www.intel.com/content/www/us/en/products/boards-kits/compute-stick/stk2m3w64cc.html)

[20 W battery power bank (5V, 4Ah) or equivalent](https://www.amazon.com/gp/product/B01LRQDAEI/ref=ppx_yo_dt_b_search_asin_title?ie=UTF8&psc=1)

# For Android
## Building:
[Pre-flight] - Check you have installed the [Android Studio](https://developer.android.com/studio) with Android SDK > 7.0; [Leap Motion Orion 4.0](https://developer.leapmotion.com/releases/leap-motion-orion-400) and Using a **64-bit windows computer**

[Step 1] - Make sure your **smartphone** and **windows pc** are in the **same** network

[Step 2 -  Unity Configuration](https://youtu.be/JmuZOQ3fii4 "Step 1 -  Unity Configuration")

[Step 3- plugin Leap Motion device to a Windows 64-bit PC (or Intel computer stick below)] 
Run this file:  **.../Portalble/PortalbleServer/x64/Rlease/PortalbleService.exe****
You should see message "HMD is all set" at the end of the log.

[Step 4]- Run the android instance on your phone, if it succeeds, you will see a message logs on the **PortalbleService.exe** window indicating a connection is open. If not check for 1) the ip in Unity scene is the ip of the computer (or compute Unit) with Leap Motion; 2) if **PortalbleService is refreshing, if not, click on that window to set focus and press anykey to refresh**. You should see your keystroke appears which indicates the window is still alive.

# For iOS (Ver. > 13.0)
## 1. Clone repository
1.1 Copy PortalbleServer to a windows machine / computation unit that  has Leap Motion connected.<br>
1.2 Install **Leap Motion Orion 4.0** on that windows machine.

## 2. Open example "ExampleGrab.Unity"
2.1 Change the ip address on WebsocketManager (change it to the ip of the **windows machine that connects to Leap Motion**<br>
2.2 Switch build mode to iOS, click **player settings** button to change your package name and bundleid.
2.2 Run PortalbleServer/x64/Release/PortalbleService.exe<br>
2.3 You should see headset found or new device ready. If you see error message, check:
- you have the right orion version installed, must be >4.0
- your PortalbleServer is on a 64-bit computer not 32-bit
- you have installed the Leap Motion Driver and can turn on Leap Motion Visualizer to confirm.

## 4. Go to the build folder
4.1 if you have not installed Pod, do `sudo gem install cocoapods`<br>
- On newer versions of MacOS, you might experience issues with setting `$PATH`. If so, use `sudo gem install -n /usr/local/bin cocoapods`

4.2 Go to the build folder, and run: `pod init`<br>
4.3 Open Podfile (using any text editor)<br>
4.4 Find the line "# Pods for Unity-iPhone", below it insert a **new line** `pod 'jetfire', '~> 0.1.5'`<br>
- For more information about using Cocoapods, check out [this tutorial](https://guides.cocoapods.org/using/using-cocoapods).


4.5 Save and close the file, and in that directory, run `pod install` to install the depencies and create a `.xcworksapce` file<br>
4.6 Open `Unity-iPhone.xcworkspace`<br>
4.7 Select the Unity-iPhone project, link your provisions, and build!

## After you have built for first time...
use unity command + b to automatically update the XCode files, no need to do step 4 again if you use command + b

# Example File:
Examples/Grab/BasicGrab.unity

# Calibrate:
[Add Calibration Unity Scene to your project](https://youtu.be/XQJYxD5Hnbc?t=125)



**Lasercut case and 3D print files are in the folder "3D print and lasercut file". Please also purchase thses [suction caps](https://www.amazon.com/gp/product/B0018N26LY/ref=ppx_yo_dt_b_asin_title_o01_s00?ie=UTF8&psc=1) for mounting the leapmotion to your smartphone

# Hacks
How do i edit the Portalble-Unity window menu?
**change the file in Portalble/Editor/PortalbleWindowManager.cs**

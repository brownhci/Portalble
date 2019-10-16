# Portalble
Welcome to Portal-ble! This system allows you to directly manipulate virtual objects in a smartphone augmented reality (AR)
environment with your bare hands. Current master branch is for Android only. Please see iOS branch for updates and schedules.

Website: https://portalble.cs.brown.edu

#Developing Environment Requirements:

Unity 2019.1.14f1 or Later

Unity ARFoundation 2.1.1

ARCore XR Plugin 2.1.1

ARKit XR Plugin 2.1.1

# Example File:
Examples/Grab/BasicGrab.unity

# Building for Android:
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


# Immersal Demo App Q1/2021
This repository contains a reference demo app using the Immersal SDK and Back4App.com realtime-database backend for a simple multi-user AR experience. The app features a quick "automatic mapping" mode that captures 40 images of the surroundings and constructs a map, that is available for use almost instantaneously in the "content placement" mode. When multiple devices have been localized against the same map, the same content can be seen on all of them; adding new content, removing content, moving content (by dragging) etc. will be updated automatically to all clients.

This sample code can be used as a basis for a more complicated AR project using the Immersal SDK.

# Compatibility (tested)

- Unity 2019.4 LTS
- AR Foundation 4
- Immersal SDK v1.11.x

# Installation steps

1. Clone these repositories

```
git clone https://github.com/immersal/demo-app-2021.git
```

```
git clone https://github.com/immersal/arcloud-sdk-samples.git
```
Copy the Assets/ImmersalSDK folder from arcloud-sdk-samples under demo-app-2021/Assets/.
2. Download our Unity Plugin (`ImmersalSDKvX_X_X.unitypackage`) from [here](https://developers.immersal.com/)
3. Launch Unity, click on **Open Project**, navigate to the `demo-app-2021` folder on your computer and press Apply/OK.
4. Click on **Assets -> Import Package -> Custom Package** and load the `ImmersalSDKvX_X_X.unitypackage`.

Please visit our [Developer Documentation](https://developers.immersal.com/docs/ "SDK Documentation") for more detailed instructions as to how to use the Immersal SDK.

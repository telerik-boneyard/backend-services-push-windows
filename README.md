# Basic Push Notifications Sample App for Windows Store

<a href="https://github.com/telerik/backend-services-push-windows" target="_blank"><img style="padding-left:20px" src="http://docs.telerik.com/platform/samples/images/get-github.png" alt="Get from GitHub" title="Get from GitHub"></a>

* [Overview](#overview)
* [Requirements](#requirements)
* [Configuration](#configuration)
* [Running the Sample](#running-the-sample)

## Overview

This repository contains a basic sample app that can receive push notifications sent from its Telerik Platform backend. It is a native Windows Store app built using .NET and Visual Studio.

The sample app utilizes the following Telerik products and SDKs:

- [Telerik Backend Services](http://docs.telerik.com/platform/backend-services/)&mdash;this is the backend of Telerik Platform where you can store data, files, and user accounts as well as set up and send push notifications
- [Telerik Backend Services .NET SDK](http://docs.telerik.com/platform/backend-services/dotnet/getting-started-dotnet-sdk)&mdash;to connect the app to Telerik Backend Services

### Features

The app's upper segment is used to send a toast push notification containing a sample text:

- Click Register to automatically register the device with Telerik Backend Services.
- Fill in the Toast Message field and click Send Push Notification to send a toast notification based on the "ToastText01" template and "Toast Message" as text. For a complete set of toast templates, refer to [Windows Dev Center](http://msdn.microsoft.com/en-us/library/windows/apps/hh761494.aspx).

The app's lower segment shows how to pin, register, and send a push notification to a secondary tile:
- Pin a secondary tile by pressing the Pin Second Tile button and approve the action.
- Register the device with Telerik Backend Services, fill all required fields and click Send Push Second Tile. This sends a tile notification based on the "TileSquarePeekImageAndText01" template which requires an image (.png 150 x 150px) and 4 text fields. For a complete set of tile templates, refer to [Windows Dev Center](http://msdn.microsoft.com/en-us/library/windows/apps/hh761491.aspx).

## Requirements

Before you begin, you need to ensure that you have the following:

- **An active [Telerik Platform](https://platform.telerik.com) account**
Ensure that you can log in to a Telerik Platform account. This can be a free trial account.
- **A Telerik Backend Services project** You can use an existing project or create a new one. 
- **Microsoft Visual Studio** You need it to load the Visual Studio project file.

## Configuration

The sample app comes fully functional, but to see it in action you must link it to your own Telerik Platform account.

What you need to set:

### API Key for Telerik Backend Services

This is a unique string that links the sample mobile app to a project in Telerik Backend Services where all the data is read from/saved.

1. Open your Telerik Backend Services project and go to **Overview > API Keys**.
2. Take note of your API Key.
3. Open the `MainPage.xaml.cs` file in Visual Studio.
4. Find the `BackendServicesApiKey` literal and replace its value with the actual Backend Services API Key that you acquired earlier.

### Push Notifications on the Backend

To enable Telerik Backend Services to send push notifications, you need to configure it, entering a Package SID and a Client Secret that you need to acquire from Microsoft.

To help you run the sample faster, we've pre-initialized it to use Package SID and Client Secret owned by Telerik. These are the steps that you need to complete on your own:

> Be aware that these values must be used only for testing purposes. Telerik could disable this Windows Store application at any time and without notice.

1. Log in to Telerik Platform.
2. Go to your application and then enter the Backend Services project that you want to use for this sample app.
3. Ensure that the Push Notifications service is enabled.
4. Navigate to **Push Notifications > Settings**.
5. Click the Windows check box an enter these values:

Setting|Value
---|---
Package Security Identifier (Package SID)|ms-app://s-1-15-2-3157833167-2319104520-3008724778-1960889256-2485977247-996848071-3039990472
Client Secret|fIpx0SSm9Dbrnk4gmOkdjRCAGdrXiapc

If you intend to use your own values, keep in mind that the Package SID must be equal to your app's Package info which is set to Telerik's app by default. For a Windows Store app this information is stored within `Package.appxmanifest`. The easiest way to connect your Windows Store Dashboard App to your desktop app is through Visual Studio. Go to **Project > Store > Associate App With the Store...**, log in with your Windows Dev Center account, and finally select your application.

## Running the Sample

Once the app is configured, you can run it on a real device from within Visual Studio.

> Push notifications are not supported when running the app on device simulators.

> Ensure that the device that you are using has Internet connectivity when running the sample.



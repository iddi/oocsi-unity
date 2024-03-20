Preparation to use this example:

Unity package preparation:
0. Create a new Unity project
1. Replace the whole Asset folder in your Unity project folder
2. Open "Package Manager" (in [Window] tab)
3. Cick the "+" on the top-left corner, choose "Add package from Github URL"
4. Paste "https://github.com/endel/NativeWebSocket.git#upm", [Enter] or click [Add] on the right side
5. Packages preparation done

In Unity Scene:
1. Find Canvas object from the component list on the left side (usually), and click it.
2. Open "Inspector" window if it's not open: [Window] tab -> [General] -> Inspector
3. Click "Add Component" at the bottom of the Inspector window, choose "Scripts" -> "Send OOCSI Msg With J Object"
4. In the "Send OOCSI Msg With J Object" block of Inspector window, click the small circles on the right side of 
   components start with "Button ..." and choose the only component on the list
5. Enter the channels for sending and receiving messages from OOCSI
6. Scene preparation done

In Data Foundry:
1. An active project 
2. An active IoT dataset, which is set to listen to the "Channel_to_send" of OOCSI channel you have in Unity.
3. A device in the same project, the ID ("device_id" in script) is required for Data Foundry to store data from OOCSI

Execute: 
Click the play icon in the center of up side

Sending --
1. Just "Click me!!"
2. Check the incoming data of your IoT dataset on Data Foundry

Receiving --
1. Open the URL with browser: https://oocsi.id.tue.nl/test/visual
2. Enter the "Channel_to_receive" of OOCSI channel you have in Unity to the "Channel Name" on the right side. 
3. Click "Send"
4. Check your Unity, the text will be replaced by the incoming message
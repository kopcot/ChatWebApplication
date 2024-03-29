Simple example of chatting app using SignalR with Blazor and Razor pages 
It is completly written in the C#, without JS-calls (they are called internally by Blazor)

This application is running on the docker at QNAP TS-233 with linux/arm64/v8 platform

# Usage
Try to open client and server in the different windows.
Client cannot write any text without changing username first and he is not able to change name after it.
This is not applied for admins.

Client is not able to see additional messages (like changing name information or connecting another user), while admin will be able to see it.

Closing page mean lost messages from current page.
On the other active pages messages will stay.

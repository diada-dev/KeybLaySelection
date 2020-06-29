C# project for Microsoft Visual Studio 2016 and later.

With this you can easily switch between keyboard layouts using external buttons.

At startup, you must select a listening COM port and set keyboard layouts.

When a command arrives from the COM port, the corresponding keyboard layout will be installed.

The control device must send 0xAA (170) and then the number of the called keyboard layout. The device can be assembled based on Arduino UNO. An example sketch is located in the Arduino_key folder.
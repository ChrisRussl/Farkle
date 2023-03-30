
Example: WCF EventWaitHandle, INFO-5060 W2023

The purpose of this example is to demonstrate the following features in
the TextClient project:

1.  The use of an object of type System.Threading.EventWaitHandle to temporarily
    pause each instance of the client until it receives a signal that it's turn 
    has come to play the game.

2.  The use of the SetConsoleCtrlHandler() function to register an event handler 
    that will trigger if the user tries to close the console window prematurely 
    (before the game is over) by clicking the close button at the top-right corner. 
    In this example the handler function is used to allow the client to "unregister" 
    from the callbacks gracefully. Note that the code in the "unmanaged" region at 
    the bottom must be included if you wish to use this technique.
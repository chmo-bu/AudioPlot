// This work is licensed under the Creative Commons Attribution-ShareAlike 4.0 International License. 
// To view a copy of this license, visit http://creativecommons.org/licenses/by-sa/4.0/ 
// or send a letter to Creative Commons, PO Box 1866, Mountain View, CA 94042, USA.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace TCPSocket {
    public class TCPClient {  	
        #region private members 	
        private TcpClient socketConnection; 	
        private Thread clientReceiveThread;
        private readonly object mu = new object();
        private bool received = true;
        #endregion  	

        // constructor
        public TCPClient() {
            ConnectToTcpServer();     
        }  	
        
        /// <summary> 	
        /// Setup socket connection. 	
        /// </summary> 	
        private void ConnectToTcpServer () { 		
            try {  			
                clientReceiveThread = new Thread (new ThreadStart(ListenForData)); 			
                clientReceiveThread.IsBackground = true; 			
                clientReceiveThread.Start();  		
            } 		
            catch (Exception e) { 			
                Debug.Log("On client connect exception " + e); 		
            } 	
        }  	
        /// <summary> 	
        /// Runs in background clientReceiveThread; Listens for incomming data. 	
        /// </summary>     
        public void ListenForData() { 		
            try { 			
                socketConnection = new TcpClient("localhost", 8052);  			
                Byte[] bytes = new Byte[1024];             
                while (true) { 				
                    // Get a stream object for reading 				
                    using (NetworkStream stream = socketConnection.GetStream()) { 					
                        int length; 					
                        // Read incomming stream into byte arrary. 					
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 						
                            var incommingData = new byte[length]; 						
                            Array.Copy(bytes, 0, incommingData, 0, length); 						
                            // Convert byte array to string message. 						
                            string serverMessage = Encoding.ASCII.GetString(incommingData);
                            // if (serverMessage.Equals("received")) {
                            //     // lock(mu) {
                            //         received = true;
                            //     // }
                            //     Debug.Log("got message!");
                            // }					
                            // Debug.Log("server message received as: " + serverMessage); 					
                        } 				
                    } 			
                }         
            }         
            catch (SocketException socketException) {             
                Debug.Log("Socket exception: " + socketException);         
            }     
        }  	
        /// <summary> 	
        /// Send message to server using socket connection. 	
        /// </summary> 	
        public void SendMessage(byte[] data) {
            if (socketConnection == null) {             
                return;         
            }
            try { 			
                // Get a stream object for writing. 			
                NetworkStream stream = socketConnection.GetStream(); 			
                if (stream.CanWrite) {                  				
                    // Write byte array to socketConnection stream.                 
                    stream.Write(data, 0, data.Length);                 
                    Debug.Log("Client sent his message - should be received by server");
                    // received = false;         
                }         
            } 		
            catch (SocketException socketException) {             
                Debug.Log("Socket exception: " + socketException);         
            }     
        } 
    }
}
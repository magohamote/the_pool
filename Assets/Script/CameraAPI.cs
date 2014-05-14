
using UnityEngine;

using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class CameraAPI : MonoBehaviour
{

	// The port number to listent to for faceAPI
	public int port_faceAPI = 29129;

	// udpclient object for faceAPI
	private UdpClient client_faceAPI;
	
	// position of the head
	public float xPos;
	public float yPos;
	public float zPos;
	
	// receiving Thread
	private Thread receiveThread;
	
	// start from shell
	private static void Main() {
		CameraAPI receiveObj = new CameraAPI();
		receiveObj.init();
		string text = "";
		do {
			text = Console.ReadLine();
		} while (!text.Equals("exit"));
	}

	public void Start()
	{
		init ();
	}

	public void FixedUpdate()
	{
		transform.localPosition = new Vector3(xPos*10, (yPos*10)+2, (-zPos*10));
	}

	// Called when application quits
	public void OnApplicationQuit() {
		print("Application is quitting");
		try {
			client_faceAPI.Close();
		} catch (SocketException se) {
			print("Error while tjrying to close socket");
			print(se.ToString());
		}
	}

	/**
	 * Create and starts the UdpClient thread that will receive data input to be used
	 * for the movement of the object.
	 */
	private void init() {
		print("UDPSend.init()");
		
		receiveThread = new Thread(new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();
		
		print("Udp thread started on port_faceAPI " + port_faceAPI);
	}

	/**
	 * Thread body that will listen to the given port_android for positionning data.
	 */
	private void ReceiveData() {
		client_faceAPI = new UdpClient (port_faceAPI);
		
		while (true) {
			try {
				IPEndPoint anyIP_faceAPI = new IPEndPoint(IPAddress.Any, 0);
				byte[] data_faceAPI = client_faceAPI.Receive(ref anyIP_faceAPI);
				string positionData = Encoding.UTF8.GetString(data_faceAPI);
				print(">> " + positionData);
				
				parsePositionData(positionData);
				
			} catch (Exception e) {
				print("Error while trying to read data from socket");
				print(e.ToString());
				return;
			}
		}
	}
	
	private void parsePositionData(string positionData) {
		String[] str = positionData.Split(' ');
		int index = 0;
		xPos = float.Parse(str[index++]);
		yPos = float.Parse(str[index++]);
		zPos = float.Parse(str[index++]);
	}

}

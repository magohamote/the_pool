/*
 * Monitor UPD data
 */

using UnityEngine;

using System.Collections;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


public class UDPReceiver: MonoBehaviour
{
	//Forward/backward speed
	public float speed = 1000f;
	
	//Rotation speed
	public float rotationSpeed = 100f;
	
	// The port number to listen to for android
	public int port_android = 15935;

	// Points GUI
	public GUIText pointsText;
	
	// Timer GUI
	public GUIText timerText;

	// Points counter
	private int points;

	// Timer value
	private float timeLeft;

	// receiving Thread
	private Thread receiveThread;

	// udpclient object for android
	private UdpClient client_android;

	// Used to identify
	private long lastTimestamp = 0;
	
	// Used to identify
	private float[] lastValues = null;

	// Constant defining the maximal time of a game
	private const float MAX_TIME = 60f;
	
	// start from shell
	private static void Main() {
		UDPReceiver receiveObj = new UDPReceiver();
		receiveObj.init();
		string text = "";
		do {
			text = Console.ReadLine();
		} while (!text.Equals("exit"));
	}
	
	// Start called at the beginning of the simulation.
	public void Start() {
		// Simualtion constants
		
		points = 0;
		UpdateScoreGUI();
		
		UpdateTimer();

		init();
	}
	
	// Update is called once per frame
	public void Update () {
		UpdateTimer();
		if (timeLeft <= 0f) {
			//Stop game
		}
	}
	
	// FixedUpdate is called before physics calculations
	public void FixedUpdate () {
		// Keyboard controller
		float horizontal = Input.GetAxis("Horizontal");
		transform.Rotate(0, horizontal * 90 * Time.deltaTime, 0);

		float vertical = Input.GetAxis ("Vertical");
		Vector3 moveDirection = transform.forward * vertical * -1 * speed;
		if (Input.GetKey ("space")) {
			moveDirection.y = 1000;
		}
		rigidbody.AddForce(moveDirection * Time.deltaTime);

		// Reset values if keyboard detected
		if (horizontal != 0 || vertical != 0) {
			lastValues = null;
		}

		// App controller
		if (lastValues != null && lastValues.Length == 3)
		{
			float rotationScale = (lastValues[1]/10.0f) * rotationSpeed * Time.deltaTime;
			transform.Rotate(Vector3.up * rotationScale);
			
			//Vector3 moveDirection = transform.forward * Input.GetAxis("Vertical") * -1 * speed;
			//rigidbody.AddForce(moveDirection * Time.deltaTime);
		
			
			float scaledSpeed = -(lastValues[2]/10.0f) * speed * Time.deltaTime;
			rigidbody.AddForce(transform.forward * scaledSpeed);
		}
	}

	// Triggered when the object collides with an object.
	public void OnTriggerEnter (Collider other) {
		if (other.gameObject.tag == "PickUp") {
			Destroy(other.gameObject);
			//audio.Play();
			points = points + 1;
			UpdateScoreGUI();
		}
	}

	// Called when application quits
	public void OnApplicationQuit() {
		print("Application is quitting");
		try {
			client_android.Close ();
		} catch (SocketException se) {
			print("Error while trying to close socket");
			print(se.ToString());
		}
	}
	
	/**
	 * Updates the score counter.
	 */
	private void UpdateScoreGUI () {
	//	pointsText.text = "Points: " + points.ToString ();
	}

	/**
	 * Updates the timer.
	 */
	private void UpdateTimer () {
	//	timeLeft = MAX_TIME - Time.realtimeSinceStartup;
	//	string timeStr = timeLeft.ToString("n1");
		//timerText.text = "Time: " + timeStr;
	}
	
	/**
	 * Create and starts the UdpClient thread that will receive data input to be used
	 * for the movement of the object.
	 */
	private void init() {
		print("UDPSend.init()");
		
		lastValues = new float[3];
		receiveThread = new Thread(new ThreadStart(ReceiveData));
		receiveThread.IsBackground = true;
		receiveThread.Start();

		print("Udp thread started on port_android " + port_android);
	}
	
	/**
	 * Thread body that will listen to the given port_android for positionning data.
	 */
	private void ReceiveData() {
		client_android = new UdpClient(port_android);

		while (true) {
			try {
				IPEndPoint anyIP_android = new IPEndPoint(IPAddress.Any, 0);
				byte[] data_android = client_android.Receive(ref anyIP_android);
				string sensorData = Encoding.UTF8.GetString(data_android);
				print(">> " + sensorData);
				
				parseSensorData(sensorData);

			} catch (Exception e) {
				print("Error while trying to read data from socket");
				print(e.ToString());
				return;
			}
		}
	}
	
	/**
	 * Parses the received packets and sets the values to be used for
	 * the next movement update if the timestamp is greater.
	 * @param timestampedPacket The packet received by the client.
	 */
	private void parseSensorData(string timestampedPacket) {
		string[] timestampValues = timestampedPacket.Split(':');
		long currentTimestamp = Convert.ToInt64(timestampValues[0]);
		if (currentTimestamp > lastTimestamp && timestampValues != null && timestampValues.Length == 4)
		{
			float[] values = new float[3];
			values[0] = Convert.ToSingle(timestampValues[1]);
			values[1] = Convert.ToSingle(timestampValues[2]);
			values[2] = Convert.ToSingle(timestampValues[3]);
			lastTimestamp = currentTimestamp;
			lastValues = values;
			
			print(">> Vals: x=" + lastValues[0] + ", y=" + lastValues[1] + ", z=" + lastValues[2]);
		}
	}
}

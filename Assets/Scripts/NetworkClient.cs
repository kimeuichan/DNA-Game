using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Text;

public class NetworkClient : MonoBehaviour{

	//server ip, port
	string serverIp = "35.163.251.160";
	int serverPort = 31486;
	IPEndPoint iep;

	//소켓으로 받을 데이터 선언
	private byte[] data = new byte[1024];
	private int size = 1024;

	private Boolean isConnected;
	Socket m_client;

	void Start(){
		StartClient ();
	}

	public void StartClient(){
		// Create EndPoint
		iep = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
		// Create a TCP/IP socket.
		Socket newsocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		// Conncet to the remote endpoint
		try{
			newsocket.BeginConnect(iep, new AsyncCallback(Connected), newsocket);
			isConnected = true;

			Application.LoadLevel("Login");
			Debug.Log("Coneected");
		}

		catch(Exception e){
			isConnected = false;
			Debug.Log("UnConeected");
			Debug.Log (e.ToString());
		}
	}

	public void StopClient(){
		m_client.Close ();
		isConnected = false;
	}

	public Boolean IsConnected {
		get {
			return isConnected;
		}
	}

	private void Connected(IAsyncResult iar){
		m_client = (Socket)iar.AsyncState;
		// Connect Sucess
		try{
			m_client.EndConnect(iar);
			m_client.BeginReceive(data, 0,size, SocketFlags.None, new AsyncCallback(RecevieData), m_client);
		}
		// Connect Fail
		catch{
			
		}
	}

	private void RecevieData(IAsyncResult iar){
		Debug.Log ("Recevie");
		Socket remote = (Socket)iar.AsyncState;
		int recv = remote.EndReceive (iar);
		string stringData = Encoding.UTF8.GetString (data, 0, recv);
		Debug.Log (stringData);
	}

	public void Send(byte[] data, int size, DnaInfo.packet_type protocolType){
		//make header
		PB_header header = new PB_header();
		header.type = protocolType;
		header.size = data.Length;

		//header -> byte array
		int headerSize = Marshal.SizeOf(header);
		byte[] headerArray = new byte[size];
		IntPtr ptr = Marshal.AllocHGlobal(size);
		Marshal.StructureToPtr(header, ptr, true);
		Marshal.Copy(ptr, headerArray, 0, headerSize);
		Marshal.FreeHGlobal(ptr);

		//byte array merge
		byte[] send_data = new byte[data.Length+headerArray.Length];
		System.Buffer.BlockCopy(headerArray, 0, send_data, 0, headerArray.Length);
		System.Buffer.BlockCopy(data, 0, send_data, headerArray.Length, data.Length);

		//data send
		m_client.BeginSend(send_data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendData), m_client);
	}

	//send callback
	private void SendData(IAsyncResult iar){
		Debug.Log ("Send Callback");
		Socket remote = (Socket)iar.AsyncState;
		int sent = remote.EndSend (iar);
		remote.BeginReceive (data, 0, size, SocketFlags.None, new AsyncCallback (RecevieData), remote);
	}

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
}

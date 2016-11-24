using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Text;

public class NetworkClient : MonoBehaviour{
	public class AsyncObject {
		public Byte[] Buffer;
		public Socket WorkingSocket;
		public AsyncObject(Int32 bufferSize) {
			this.Buffer = new Byte[bufferSize];
		}
	}
	//server ip, port
	string serverIp = "35.163.251.160";
	int serverPort = 31486;

	private AsyncCallback m_fnReceiveHandler;
	private AsyncCallback m_fnSendHandler;

	//소켓으로 받을 데이터 선언

	private Boolean isConnected;
	Socket m_client = null;

	void Start(){
		m_fnReceiveHandler = handleDataRecive;
		m_fnSendHandler = handleDataSend;
		StartClient ();
	}

	public void StartClient(){
		// Create a TCP/IP socket.
		Socket newsocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
		isConnected = false;
		// Conncet to the remote endpoint
		try{
			m_client.Connect(serverIp, serverPort);
			isConnected = true;

			Application.LoadLevel("Login");
			Debug.Log("Coneected");
		}

		catch(Exception e){
			isConnected = false;
			Debug.Log("UnConeected");
			Debug.Log (e.ToString());
		}

		if (isConnected) {
			AsyncObject ao = new AsyncObject (4096);
			ao.WorkingSocket = m_client;
			m_client.BeginReceive (ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnReceiveHandler, ao);
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

	private void handleDataRecive(IAsyncResult ar){
		Debug.Log ("Recevie");
		AsyncObject ao = (AsyncObject)ar.AsyncState;

		Int32 recvBytes;
		try{
			recvBytes = ao.WorkingSocket.EndReceive(ar);
		}catch{
			return;
		}

		if (recvBytes > 0) {
			Byte[] msgByte = new Byte[recvBytes];
			Array.Copy (ao.Buffer, msgByte, recvBytes);
		}

		try{
			ao.WorkingSocket.BeginReceive(ao.Buffer, 0, ao.Buffer.Length, SocketFlags.None, m_fnReceiveHandler, ao);
		}catch{
			return;
		}
	}

	public void Send(byte[] data, int size, DnaInfo.packet_type protocolType){

		AsyncObject ao = new AsyncObject (1);

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
		m_client.BeginSend(send_data, 0, send_data.Length, SocketFlags.None, m_fnSendHandler, ao);
	}

	//send callback
	private void handleDataSend(IAsyncResult ar){
		AsyncObject ao = (AsyncObject)ar.AsyncState;

		Int32 sentBytes;
		try{
			sentBytes = ao.WorkingSocket.EndSend(ar);
		}catch{
			return;
		}

		if(sentBytes > 0){
			Byte[] msgByte = new Byte[sentBytes];
			Array.Copy (ao.Buffer, msgByte, sentBytes);
		}
	}

	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
}

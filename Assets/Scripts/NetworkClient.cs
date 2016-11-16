using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class NetworkClient {

	public string serverIp;
	public int serverPort = 31486;
	IPEndPoint iep = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
	//소켓으로 받을 데이터 선언
	private byte[] data = new byte[1024];

	private Boolean isConnected;

	Socket m_client;

	void StartClient(){
		// Create a TCP/IP socket.
		Socket newsocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		// Conncet to the remote endpoint
		newsocket.BeginConnect(iep, new AsyncCallback(Connected), newsocket);
		try{
			m_client.Connect(serverIp, serverPort);
			isConnected = true;
		}
		catch{
			isConnected = false;
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
			m_client.BeginReceive(data, 0, new AsyncCallback(RecevieData), m_client);
		}
		// Connect Fail
		catch{
			
		}
	}

	private void RecevieData(IAsyncResult iar){
		Socket remote = (Socket)iar.AsyncState;
		int recv = remote.EndReceive (iar);
		string stringData
	}

	public void Send(byte[] data, int size){
		m_client.BeginSend(message, 0, message.Length, SocketFlags.None, new AsyncCallback(SendData), client);
	}

}

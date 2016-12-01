using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityEngine;

public class PB_handler	: MonoBehaviour{
	private DnaInfo.LoginRequest loginReq = new DnaInfo.LoginRequest();
	private NetworkClient client;


	public void Start(){
		client = GameObject.Find ("Network Client").GetComponent<NetworkClient> ();
	}

	public void handleDataRecive(Byte[] recive_data){
		
	}

	public void Send(byte[] data, int size, DnaInfo.packet_type protocolType){
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

		client.Send (send_data);
	}

	public void LoginReq(string userName){
		byte[] send_data = new byte[1024];
		loginReq.Id = userName;
		Google.Protobuf.CodedOutputStream reqStream = new Google.Protobuf.CodedOutputStream (send_data);
		// Make MemoryStream
		loginReq.WriteTo (reqStream);
		Send (send_data, send_data.Length, DnaInfo.packet_type.LoginReq);
	}
}

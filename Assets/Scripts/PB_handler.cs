using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

public class PB_handler	{
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

		NetworkClient.Send (send_data);
	}
}

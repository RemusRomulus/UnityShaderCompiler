using System;
using System.Net;
using System.Threading;
using System.Text;
using System.IO;
using System.Net.Sockets;

namespace OfflineShaderCompiler {
public class SocketPipe {
	public Socket listener;
	Socket handler;

	public NetworkStream Stream;
	public BinaryReader Reader;
	
	static bool IsSocketConnected(Socket s)
	{
		return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
	}

	public SocketPipe(IPEndPoint endpoint) {
		listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		listener.Bind(endpoint);
		listener.Listen(10);
	}

	public void WaitForConnection() {
		handler = listener.Accept();
		Stream = new NetworkStream(handler);
		Reader = new BinaryReader(Stream);
	}

	byte[] toWrite;

	public void Write(byte[] buffer, int offset, int count) {
		if (toWrite == null || toWrite.Length < count)
			toWrite = new byte[count];
		
		Array.Copy(buffer, offset, toWrite, 0, count);

		int bytesSent = handler.Send(toWrite, 0, count, SocketFlags.None);
	}

	public void Dispose() { }
	
	public bool IsConnected { get { return handler != null && IsSocketConnected(handler); } }

	public int ReadByte() {
		byte[] buf = new byte[1];

		int numBytes = handler.Receive(buf);

		if (numBytes < 1)
			return -1;
		else if (numBytes == 1)
			return (int)buf[0];
		else
			throw new Exception("unexpected number of bytes read " + numBytes);
	}
}
}

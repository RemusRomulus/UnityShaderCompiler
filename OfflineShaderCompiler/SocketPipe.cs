using System;
using System.Net;
using System.Threading;
using System.Text;
using System.Net.Sockets;

namespace OfflineShaderCompiler {
public class SocketPipe : IDisposable {
	public Socket listener;
	Socket handler;
	
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
		handler.ReceiveTimeout = 3000;
		Console.WriteLine("accepted socket " + handler);
	}

	byte[] toWrite;

	public void Write(byte[] buffer, int offset, int count) {
		if (toWrite == null)
			toWrite = new byte[count];
		else if (toWrite.Length < count)
			Array.Resize(ref toWrite, count);

		Array.Copy(buffer, offset, toWrite, 0, count);
		int bytesSent = handler.Send(toWrite, 0, count, SocketFlags.None);
		Console.WriteLine("WRITE: " + Encoding.UTF8.GetString(toWrite));
		if (bytesSent != count)
			throw new Exception("wrote " + bytesSent + " bytes but tried to write " + count);
	}

	public void Dispose() { }
	
	public bool IsConnected { get { return handler != null && IsSocketConnected(handler); } }

	byte[] readBuffer = new byte[1];

	public int ReadByte() {
		int numBytes = handler.Receive(readBuffer);
		if (numBytes == 1)
			return (int)readBuffer[0];
		else
			return -1;
	}
}
}

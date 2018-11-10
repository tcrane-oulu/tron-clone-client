using System.Net.Sockets;
using System;
using System.IO;
using UnityEngine;

public class NetworkClient : IDisposable
{

    public delegate void ConnectedHandler(bool connected);
    public event ConnectedHandler Connected;
    public delegate void PacketHandler(IServerPacket packet);
    public event PacketHandler OnPacket;

    public bool Closed
    {
        get { return closed; }
    }
    private TcpClient socket;
    private NetworkStream stream;
    private byte[] inBuffer;
    private bool closed = false;

    public NetworkClient()
    {
        socket = new TcpClient();
    }

    ~NetworkClient()
    {
        Dispose();
    }

    // Attempts to connect to the given host and port.
    public void Connect(string host, int port)
    {
        if (!socket.Connected)
        {
            StartConnection(host, port);
        }
        else
        {
            Debug.Log("Socket already connected.");
        }
    }

    // Sends a client packet to the server.
    public void Send(IClientPacket packet)
    {
        if (!socket.Connected)
        {
            return;
        }
        MemoryStream ms = new MemoryStream();
        using (PacketWriter writer = new PacketWriter(ms))
        {
            // Write the packet ID.
            writer.Write((byte)packet.Id);

            // Write a placeholder int32 for the length - we will overwrite this later.
            writer.Write(0);

            // Write the actual packet data
            packet.Write(writer);
            var data = ms.ToArray();

            // Overwrite the length placeholder with the actual length
            writer.WriteLength(data);

            // Actually write the data to the socket.
            stream.BeginWrite(data, 0, data.Length, OnWrite, null);
        }
    }

    // Starts the connection process.
    private void StartConnection(string host, int port)
    {
        Debug.Log("Starting connection");
        socket.BeginConnect(host, port, OnConnect, null);
        inBuffer = new byte[5];
    }

    // Called when the connection process has completed.
    private void OnConnect(IAsyncResult result)
    {
        if (socket.Connected)
        {
            Debug.Log("Connected.");
            closed = false;
            stream = socket.GetStream();

            // Try to read 5 bytes of data (the packet header).
            stream.BeginRead(inBuffer, 0, 5, OnRead, null);
        }
        else
        {
            Debug.Log("Couldn't connect.");
            Dispose();
        }
        if (Connected != null)
        {
            Connected.Invoke(socket.Connected);
        }
        socket.EndConnect(result);
    }

    private void OnWrite(IAsyncResult result)
    {
        stream.EndWrite(result);
    }

    // Called when data has been read from the stream.
    private void OnRead(IAsyncResult result)
    {
        stream.EndRead(result);

        // If the length of the buffer is 5, we should process the packet header.
        if (inBuffer.Length == 5)
        {
            using (PacketReader reader = new PacketReader(new MemoryStream(inBuffer)))
            {
                // Skip the packet ID since we don't need it now.
                reader.ReadByte();

                // Read the length from the packet header.
                int newLength = reader.ReadInt32();

                // Resize the packet buffer to the new length.
                var tmp = new byte[newLength];
                if (newLength < 5)
                {
                    Dispose();
                    return;
                }
                inBuffer.CopyTo(tmp, 0);
                inBuffer = tmp;
            }

            // Try to read the enough bytes to fill the packet buffer.
            // We already have 5 bytes in the buffer, so we read inBuffer.Length - 5.
            stream.BeginRead(inBuffer, 5, inBuffer.Length - 5, OnRead, null);
        }
        // If the length is not 5, we have a whole packet that we should process.
        else
        {
            using (PacketReader reader = new PacketReader(new MemoryStream(inBuffer)))
            {
                // Read the packet header.
                byte id = reader.ReadByte();
                int length = reader.ReadInt32();

                // Create a new instance of the packet we received.
                IServerPacket packet = PacketResolver.Create((PacketType)id);
                if (packet != null)
                {
                    // Read the packet and print its info.
                    packet.Read(reader);
                    // Debug.Log(string.Format("{0}, {1}", id, length));
                    // Debug.Log(packet.ToString());
                    if (OnPacket != null)
                    {
                        OnPacket.Invoke(packet);
                    }
                }
                else
                {
                    // We don't have a definition for the packet type we
                    // received, so we cannot instantiate an instance of it.
                    Debug.Log(string.Format("Unable to create packet {0}", id));
                }
            }

            // Reset the buffer so we are ready to receive the next packet header.
            inBuffer = new byte[5];
            stream.BeginRead(inBuffer, 0, 5, OnRead, null);
        }
    }

    // Called to release the resources held by this instance.
    // Note that this resource doesn't actually hold any unmanaged
    // resources, so we simply close the stream and the socket.
    public void Dispose()
    {
        if (!closed)
        {
            socket.Close();
            stream.Close();
            inBuffer = new byte[5];
            closed = true;
            Debug.Log("Disposed");
        }
    }
}
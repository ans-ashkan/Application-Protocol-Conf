using System;
using System.Net.Sockets;

namespace ChatServer
{
    public class SocketContainer
    {
        public Action<byte[]> MessageArrived { get; set; }
        public Socket Socket { get; set; }
        public byte[] LengthBuffer { get; set; }
        public byte[] DataBuffer { get; set; }
        public int BytesReceived { get; set; }
        public byte[] RawBuffer { get; set; }

        public const int RawBufferSize = 1024;
        public static int MaxMessageSize = (int)Math.Pow(1024, 6);

        public SocketContainer()
        {
            LengthBuffer = new byte[sizeof(int)];
        }

        public byte[] WrapMessage(byte[] message)
        {
            // Get the length prefix for the message
            byte[] lengthPrefix = BitConverter.GetBytes(message.Length);

            // Concatenate the length prefix and the message
            byte[] ret = new byte[lengthPrefix.Length + message.Length];
            lengthPrefix.CopyTo(ret, 0);
            message.CopyTo(ret, lengthPrefix.Length);

            return ret;
        }

        public void ResetBuffer()
        {
            if (RawBuffer == null)
            {
                RawBuffer = new byte[RawBufferSize];
            }
            for (int i = 0; i < RawBuffer.Length; i++)
            {
                RawBuffer[i] = 0;
            }
        }

        public void DataReceived(byte[] data)
        {
            // Process the incoming data in chunks, as the ReadCompleted requests it

            // Logically, we are satisfying read requests with the received data, instead of processing the
            //  incoming buffer looking for messages.

            int i = 0;
            while (i != data.Length)
            {
                // Determine how many bytes we want to transfer to the buffer and transfer them
                int bytesAvailable = data.Length - i;
                if (DataBuffer != null)
                {
                    // We're reading into the data buffer
                    int bytesRequested = DataBuffer.Length - BytesReceived;

                    // Copy the incoming bytes into the buffer
                    int bytesTransferred = Math.Min(bytesRequested, bytesAvailable);
                    Array.Copy(data, i, DataBuffer, BytesReceived, bytesTransferred);
                    i += bytesTransferred;

                    // Notify "read completion"
                    this.ReadCompleted(bytesTransferred);
                }
                else
                {
                    // We're reading into the length prefix buffer
                    int bytesRequested = LengthBuffer.Length - BytesReceived;

                    // Copy the incoming bytes into the buffer
                    int bytesTransferred = Math.Min(bytesRequested, bytesAvailable);
                    Array.Copy(data, i, LengthBuffer, BytesReceived, bytesTransferred);
                    i += bytesTransferred;

                    // Notify "read completion"
                    this.ReadCompleted(bytesTransferred);
                }
            }
        }

        public void ReadCompleted(int count)
        {
            // Get the number of bytes read into the buffer
            BytesReceived += count;

            if (DataBuffer == null)
            {
                // We're currently receiving the length buffer

                if (BytesReceived != sizeof(int))
                {
                    // We haven't gotten all the length buffer yet: just wait for more data to arrive
                }
                else
                {
                    // We've gotten the length buffer
                    int length = BitConverter.ToInt32(LengthBuffer, 0);

                    // Sanity check for length < 0
                    if (length < 0)
                        throw new System.Net.ProtocolViolationException("Message length is less than zero");

                    // Another sanity check is needed here for very large packets, to prevent denial-of-service attacks
                    if (MaxMessageSize > 0 && length > MaxMessageSize)
                        throw new System.Net.ProtocolViolationException("Message length " + length.ToString(System.Globalization.CultureInfo.InvariantCulture) + " is larger than maximum message size " + MaxMessageSize.ToString(System.Globalization.CultureInfo.InvariantCulture));

                    // Zero-length packets are allowed as keepalives
                    if (length == 0)
                    {
                        BytesReceived = 0;
                        if (this.MessageArrived != null)
                            this.MessageArrived(new byte[0]);
                    }
                    else
                    {
                        // Create the data buffer and start reading into it
                        DataBuffer = new byte[length];
                        BytesReceived = 0;
                    }
                }
            }
            else
            {
                if (BytesReceived != DataBuffer.Length)
                {
                    // We haven't gotten all the data buffer yet: just wait for more data to arrive
                }
                else
                {
                    // We've gotten an entire packet
                    if (this.MessageArrived != null)
                        this.MessageArrived(DataBuffer);

                    // Start reading the length buffer again
                    DataBuffer = null;
                    BytesReceived = 0;
                }
            }
        }
    }
}
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

public class TwitchIRC : MonoBehaviour {
    // This information is necessary
    private string channel_name = string.Empty;
    private string nickname = string.Empty;
    private string o_auth_token = string.Empty;
    private string server = "irc.twitch.tv";
    private int port = 6667;

    private string buffer = string.Empty;
    private bool threads_halt = false;

    private Queue<string> commands = new Queue<string>();
    private List<string> recieved_messages = new List<string>();
    private class MessageEvent : UnityEvent<string> {}
    private MessageEvent message_received_event = new MessageEvent();

    private Thread incoming_thread;
    private Thread outgoing_thread;

    private void
    OnDestroy() {
        threads_halt = true;
    }

    private void
    OnDisable() {
        threads_halt = true;
    }

    private void
    OnEnable() {
        threads_halt = false;
        StartIRC();
    }

    private void
    ProcessInput(TextReader input, NetworkStream network_stream) {
        while (!threads_halt) {
            if (!network_stream.DataAvailable) continue;
            buffer = input.ReadLine();

            // 001 command is received after successful connection
            if (buffer.Split(' ')[1] == "001") {
                commands.Enqueue("JOIN #" + channel_name);
            }

            if (buffer.Contains("PRIVMSG #")) {
                lock (recieved_messages) {
                    recieved_messages.Add(buffer);
                }
            }
        }
    }

    private void
    ProcessOutput(TextWriter output) {
        System.Diagnostics.Stopwatch clock = new System.Diagnostics.Stopwatch();
        clock.Start();

        while (!threads_halt) {
            lock (commands) {
                // Delay to avoid unnecessary actions every update
                if (commands.Count > 0 && clock.ElapsedMilliseconds > 2000) {
                        output.WriteLine(commands.Dequeue());
                        output.Flush();
                        clock.Reset();
                        clock.Start();
                }
            }
        }
    }

    private void
    SendMsg(string msg) {
        lock (commands) {
            commands.Enqueue("PRIVMSG #" + channel_name + " :" + msg);
        }
    }

    private void
    StartIRC() {
        TcpClient socket = new TcpClient();
        socket.Connect(server, port);

        if (!socket.Connected)
            return;

        // Stream for all traffic
        NetworkStream network_stream = socket.GetStream();
        StreamReader input = new StreamReader(network_stream);
        StreamWriter output = new StreamWriter(network_stream);

        // Send our credentials to the server
        output.WriteLine("PASS " + o_auth_token);
        output.WriteLine("NICK " + nickname.ToLower());
        output.Flush();

        // Lambda functions
        incoming_thread = new Thread(() => ProcessInput(input, network_stream));
        incoming_thread.Start();

        outgoing_thread = new Thread(() => ProcessOutput(output));
        outgoing_thread.Start();
    }

    private void
    Update() {
        lock (recieved_messages) {
            if (recieved_messages.Count > 0) {
                for (int i = 0; i < recieved_messages.Count; ++i) {
                    // Will call specified function listening
                    message_received_event.Invoke(recieved_messages[i]);
                }

                recieved_messages.Clear();
            }
        }
    }
}

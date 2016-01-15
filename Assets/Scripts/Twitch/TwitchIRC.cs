/*
 * Useful IRC commands
 * .clear
 * .slow <seconds>, .slowoff
 * .subscribers, .subscribersoff
 * PRIVMSG #channel : <message>
 *
 * Useful group chat commands
 * .w username <message>
 */

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

public class TwitchIRC : MonoBehaviour {
    // This information is necessary
    private string _channel_name = string.Empty;
    private string _nickname = string.Empty;
    private string _o_auth_token = string.Empty;

    private string buffer = string.Empty;
    private bool threads_halt = false;

    private int _irc_port = 6667;
    private string _irc_server = "irc.twitch.tv";
    private Queue<string> irc_commands = new Queue<string>();
    private List<string> irc_received_messages = new List<string>();
    public class MessageEvent : UnityEvent<string> {}
    private MessageEvent _irc_message_received_event = new MessageEvent();
    private Thread irc_incoming_thread;
    private Thread irc_outgoing_thread;
    private float irc_output_timer = 0.0f;

    private int _whisper_port = 443;
    private string _whisper_server = "199.9.253.59";
    private Queue<string> whisper_commands = new Queue<string>();
    private Thread whisper_outgoing_thread;

    public string channel_name {
        get {return this._channel_name;}
        set {this._channel_name = value;}
    }

    public MessageEvent irc_message_received_event {
        get {return this._irc_message_received_event;}
        set {this._irc_message_received_event = value;}
    }

    public int irc_port {
        get {return this._irc_port;}
        set {this._irc_port = value;}
    }

    public string irc_server {
        get {return this._irc_server;}
        set {this._irc_server = value;}
    }

    public string nickname {
        get {return this._nickname;}
        set {this._nickname = value;}
    }

    public string o_auth_token {
        get {return this._o_auth_token;}
        set {this.o_auth_token = value;}
    }

    public int whisper_port {
        get {return this._whisper_port;}
        set {this._whisper_port = value;}
    }

    public string whisper_server {
        get {return this._whisper_server;}
        set {this._whisper_server = value;}
    }

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
        StartWhisper();
    }

    private void
    IRCProcessInput(TextReader input, NetworkStream network_stream) {
        while (!threads_halt) {
            if (!network_stream.DataAvailable) continue;
            buffer = input.ReadLine();

            // 001 command is received after successful connection
            if (buffer.Split(' ')[1] == "001")
                IRCPutCommand("JOIN #" + _channel_name);

            if (buffer.Contains("PRIVMSG #")) {
                lock (irc_received_messages) {
                    irc_received_messages.Add(buffer);
                }
            }
        }
    }

    private void
    IRCProcessOutput(TextWriter output) {
        irc_output_timer += Time.deltaTime;

        while (!threads_halt) {
            lock (irc_commands) {
                // Delay to avoid unnecessary actions every update
                if (irc_commands.Count > 0 && irc_output_timer > 2000.0f) {
                    output.WriteLine(irc_commands.Dequeue());
                    output.Flush();
                    irc_output_timer = 0.0f;
                }
            }
        }
    }

    public void
    IRCPutCommand(string command) {
        lock (irc_commands) {
            irc_commands.Enqueue(command);
        }
    }

    public void
    IRCPutMessage(string message) {
        lock (irc_commands) {
            irc_commands.Enqueue("PRIVMSG #" + _channel_name + " :" + message);
        }
    }

    private void
    StartIRC() {
        TcpClient socket = new TcpClient();
        socket.Connect(_irc_server, _irc_port);

        if (!socket.Connected)
            return;

        // Stream for all traffic
        NetworkStream network_stream = socket.GetStream();
        StreamReader input = new StreamReader(network_stream);
        StreamWriter output = new StreamWriter(network_stream);

        // Send our credentials to the server
        output.WriteLine("PASS " + _o_auth_token);
        output.WriteLine("NICK " + _nickname.ToLower());
        output.Flush();

        // Lambda functions
        irc_outgoing_thread = new Thread(() => IRCProcessOutput(output));
        irc_outgoing_thread.Start();

        irc_incoming_thread = new Thread(() => IRCProcessInput(input, network_stream));
        irc_incoming_thread.Start();
    }

    private void
    StartWhisper() {
        TcpClient socket = new TcpClient();
        socket.Connect(_whisper_server, _whisper_port);

        if (!socket.Connected)
            return;

        // Stream for all traffic
        NetworkStream network_stream = socket.GetStream();
        StreamWriter output = new StreamWriter(network_stream);

        // Send our credentials to the server
        output.WriteLine("PASS " + _o_auth_token);
        output.WriteLine("NICK " + _nickname.ToLower());
        output.Flush();

        // Lambda functions
        whisper_outgoing_thread = new Thread(() => WhisperProcessOutput(output));
        whisper_outgoing_thread.Start();
    }

    private void
    Update() {
        lock (irc_received_messages) {
            if (irc_received_messages.Count > 0) {
                for (int i = 0; i < irc_received_messages.Count; ++i) {
                    // Will call specified function listening
                    _irc_message_received_event.Invoke(irc_received_messages[i]);
                }

                irc_received_messages.Clear();
            }
        }
    }

    private void
    WhisperProcessOutput(TextWriter output) {
        System.Diagnostics.Stopwatch clock = new System.Diagnostics.Stopwatch();
        clock.Start();

        while (!threads_halt) {
            lock (whisper_commands) {
                // Delay to avoid unnecessary actions every update
                if (whisper_commands.Count > 0 && clock.ElapsedMilliseconds > 2000) {
                    output.WriteLine(whisper_commands.Dequeue());
                    output.Flush();
                    clock.Reset();
                    clock.Start();
                }
            }
        }
    }

    public void
    WhisperPutMessage(string user, string message) {
        lock (irc_commands) {
            whisper_commands.Enqueue("PRIVMSG #" + _channel_name + " :.w " + user + " " + message);
        }
    }
}

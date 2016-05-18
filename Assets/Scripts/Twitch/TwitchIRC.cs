/*
 * NOTE
 * /command works native to Twitch
 * .command works with Better Twitch Tv
 *
 * Additional Information
 * CAP REQ :twitch.tv/commands to request enabled raw commands
 * CAP REQ :twitch.tv/membership to request IRC v3 capability registration
 * CAP REQ :twitch.tv/tags to request IRC v3 message tags
 *
 * Useful IRC commands
 * /clear
 * /slow <seconds>, /slowoff
 * /subscribers, /subscribersoff
 * PRIVMSG #channel : <message>
 *
 * Useful group chat commands
 * /w username <message>
 */

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;

public class TwitchIRC : MonoBehaviour {
    // This information is necessary
    public static string _channel_name = "featherweighttv";
    public static string _nickname = "featherweighttv";
    public static string _o_auth_token = "oauth:lyin207fadyrq39m89ukcqa07ic3c8";

    private static string buffer = string.Empty;
    private static bool threads_halt = false;

    public static int _irc_port = 6667;
    public static string _irc_server = "irc.twitch.tv";
    private static Queue<string> irc_commands = new Queue<string>();
    private static List<string> irc_received_messages = new List<string>();
    public class MessageEvent : UnityEvent<string> {}
    private static MessageEvent _irc_message_received_event = new MessageEvent();
    private static Thread irc_incoming_thread;
    private static Thread irc_outgoing_thread;

    public static int _whisper_port = 443;
    public static string _whisper_server = "irc.chat.twitch.tv";
    private static Queue<string> whisper_commands = new Queue<string>();
    private static Thread whisper_outgoing_thread;

    private static bool _valid_login = false;

    public static string channel_name {
        get {return _channel_name;}
        set {_channel_name = value;}
    }

    public static MessageEvent irc_message_received_event {
        get {return _irc_message_received_event;}
        set {_irc_message_received_event = value;}
    }

    public static int irc_port {
        get {return _irc_port;}
        set {_irc_port = value;}
    }

    public static string irc_server {
        get {return _irc_server;}
        set {_irc_server = value;}
    }

    public static string nickname {
        get {return _nickname;}
        set {_nickname = value;}
    }

    public static string o_auth_token {
        get {return _o_auth_token;}
        set {_o_auth_token = value;}
    }

    public static bool valid_login {
        get {return _valid_login;}
        set {_valid_login = value;}
    }

    public static int whisper_port {
        get {return _whisper_port;}
        set {_whisper_port = value;}
    }

    public static string whisper_server {
        get {return _whisper_server;}
        set {_whisper_server = value;}
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

    private static void
    IRCProcessInput(TextReader input, NetworkStream network_stream) {
        while (!threads_halt) {
            if (!network_stream.DataAvailable)
                continue;

            buffer = input.ReadLine();
            buffer = buffer.ToLower();

            lock (irc_received_messages) {
                irc_received_messages.Add(buffer);
            }

            Thread.Sleep(10);
        }
    }

    private static void
    IRCProcessOutput(TextWriter output) {
        Stopwatch clock = new Stopwatch();
        clock.Start();

        while (!threads_halt) {
            lock (irc_commands) {
                // Delay to avoid unnecessary actions every update
                // Only 20 commands can be sent in a span of 30 seconds
                if (irc_commands.Count > 0 && clock.ElapsedMilliseconds > 2000.0f) {
                    output.WriteLine(irc_commands.Dequeue());
                    output.Flush();
                    clock.Reset();
                    clock.Start();
                }
            }

            Thread.Sleep(10);
        }
    }

    public static void
    IRCPutCommand(string command) {
        lock (irc_commands) {
            irc_commands.Enqueue(command);
        }
    }

    public static void
    IRCPutMessage(string message) {
        lock (irc_commands) {
            irc_commands.Enqueue("PRIVMSG #" + _channel_name + " :" + message);
        }
    }

    private static void
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

    private static void
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

    private static void
    WhisperProcessOutput(TextWriter output) {
        Stopwatch clock = new Stopwatch();
        clock.Start();

        while (!threads_halt) {
            lock (whisper_commands) {
                // Delay to avoid unnecessary actions every update
                // Only 20 commands can be sent in a span of 30 seconds
                if (whisper_commands.Count > 0 && clock.ElapsedMilliseconds > 2000) {
                    output.WriteLine(whisper_commands.Dequeue());
                    output.Flush();
                    clock.Reset();
                    clock.Start();
                }
            }

            Thread.Sleep(10);
        }
    }

    public static void
    WhisperPutMessage(string user, string message) {
        lock (whisper_commands) {
            whisper_commands.Enqueue("PRIVMSG #" + _channel_name + " :/w " + user + " " + message);
        }
    }
}

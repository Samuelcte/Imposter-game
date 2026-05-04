using System;
using System.Net.Sockets;
using System.Text;

const string defaultHost = "192.168.178.53";
const int defaultPort = 7777;

var host = args.Length > 0 ? args[0] : defaultHost;
var port = args.Length > 1 && int.TryParse(args[1], out var p) ? p : defaultPort;

using var client = new TcpClient();
try
{
	await client.ConnectAsync(host, port);

	await using var stream = client.GetStream();
	using var reader = new StreamReader(stream, Encoding.UTF8);
	using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

	Console.Write("Nachricht: ");
	var message = Console.ReadLine();
	if (string.IsNullOrWhiteSpace(message))
	{
		message = "Hallo Server";
	}

	await writer.WriteLineAsync(message);
	var response = await reader.ReadLineAsync();
	Console.WriteLine(response);
}
catch (Exception ex)
{
	Console.WriteLine($"Verbindung fehlgeschlagen: {ex.Message}");
}
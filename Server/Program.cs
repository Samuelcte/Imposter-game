using System.Net;
using System.Net.Sockets;
using System.Text;

const int defaultPort = 7777;
var port = args.Length > 0 && int.TryParse(args[0], out var parsedPort) ? parsedPort : defaultPort;

var listener = new TcpListener(IPAddress.Any, port);

listener.Start();
Console.WriteLine($"Server laeuft auf Port {port}.");

while (true)
{
	var tcpClient = await listener.AcceptTcpClientAsync();
	_ = Task.Run(() => HandleClientAsync(tcpClient));
}

static async Task HandleClientAsync(TcpClient tcpClient)
{
	Console.WriteLine($"Client verbunden: {tcpClient.Client.RemoteEndPoint}");

	await using var stream = tcpClient.GetStream();
	using var reader = new StreamReader(stream, Encoding.UTF8);
	using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

	try
	{
		while (true)
		{
			var message = await reader.ReadLineAsync();
			if (message is null)
			{
				break;
			}

			Console.WriteLine($"Vom Client: {message}");
			await writer.WriteLineAsync($"Server hat empfangen: {message}");
		}
	}
	finally
	{
		tcpClient.Close();
		Console.WriteLine("Client getrennt.");
	}
}

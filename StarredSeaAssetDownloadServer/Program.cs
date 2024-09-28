using System.Net;
using System.Text;

class HttpServer
{
    public static HttpListener listener;
    public static string webRootFolder = "./web";
    public static string[] urls = { "http://+:9878/" };
    public static int pageViews = 0;
    public static int requestCount = 0;
    public static string pageData =
        "<!DOCTYPE>" +
        "<html>" +
        "  <head>" +
        "    <title>Starred Sea Asset Server</title>" +
        "  </head>" +
        "  <body>" +
        "    <p>Assets Served this Session: {0}</p>" +
        "  </body>" +
        "</html>";
    public static string NotFoundData =
        "<!DOCTYPE>" +
        "<html>" +
        "  <head>" +
        "    <title>Starred Sea Asset Server</title>" +
        "  </head>" +
        "  <body>" +
        "    <p>Asset not found.</p>" +
        "  </body>" +
        "</html>";


    public static async Task HandleIncomingConnections()
    {
        bool runServer = true;

        // While a user hasn't visited the `shutdown` url, keep on handling requests
        while (runServer)
        {
            try {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                requestCount++;


                // Write the response info
                byte[] data = new byte[] { };

                if (req.Url.AbsolutePath != "/favicon.ico")
                {
                    pageViews += 1;
                    // Print out some info about the request
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Requested: " + req.Url.AbsolutePath.ToString());
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(" - To: " + req.UserHostName);
                }
                else
                {
                    string filePath = webRootFolder + "favicon.ico";
                    if (File.Exists(filePath))
                    {
                        data = File.ReadAllBytes(filePath);
                        resp.ContentType = "application/octet-stream";
                        resp.ContentLength64 = data.LongLength;
                    }
                }
                if (req.Url.AbsolutePath == "/index.html" || req.Url.AbsolutePath == "/")
                {
                    string disableSubmit = !runServer ? "disabled" : "";
                    data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews));
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                }
                else if (req.Url.AbsolutePath.StartsWith("/assets/"))
                {
                    string filePath = webRootFolder + "assets/" + (req.Url.AbsolutePath.Replace("/assets/", "").Replace("..", ""));
                    if (File.Exists(filePath))
                    {
                        data = File.ReadAllBytes(filePath);
                        resp.ContentType = "application/octet-stream";
                        resp.ContentLength64 = data.LongLength;
                    }
                    else
                    {
                        data = Encoding.UTF8.GetBytes(NotFoundData);
                        resp.ContentType = "text/html";
                        resp.ContentEncoding = Encoding.UTF8;
                        resp.ContentLength64 = data.LongLength;
                    }
                }
                else
                {
                    data = Encoding.UTF8.GetBytes(NotFoundData);
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.ContentLength64 = data.LongLength;
                }

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("EXCEPTION SERVING FILE!");
                Console.WriteLine(e.Message);
            }
        }
    }


    public static void Main(string[] args)
    {
        //Change colors to differentiate from server at a glance
        Console.BackgroundColor = ConsoleColor.DarkGray;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.SetWindowSize(80, 4);
        Console.Title = "StarredSeaMUON Asset Server";
        Console.Clear();
        //find web root
        if(File.Exists("webroot.txt"))
        {
            string[] fl = File.ReadAllLines("webroot.txt");
            if(fl.Length >= 1)
            {
                string d = fl[0].Replace("\\", "/");
                if (Directory.Exists(d))
                {
                    webRootFolder = d;
                    if(!webRootFolder.EndsWith("/"))
                    {
                        webRootFolder += "/";
                    }
                }
                else
                {
                    Console.WriteLine("Web root not found! check webroot.txt");
                }
            }
            else
            {
                Console.WriteLine("webroot.txt malformed!");
            }
        }


        // Create a Http server and start listening for incoming connections
        listener = new HttpListener();
        foreach (string url in urls)
        {
            listener.Prefixes.Add(url);
        }
        listener.Start();
        Console.WriteLine("Listening for connections.");

        // Handle requests
        Task listenTask = HandleIncomingConnections();
        listenTask.GetAwaiter().GetResult();

        // Close the listener
        listener.Close();
    }
}
using DuckRace.Scripts.DataModels;
using Godot;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DuckRace.Scripts.Global
{
    public partial class GlobalDataManager : Node
    {
        // A concurrent Bag is practically just a List i can remove entrys from without it causing problems with threadsafety,
        // shouldn't actually be a problem but i like that i can read an remove in one go
        public ConcurrentBag<DuckRaceInfo> RaceParticipants = new ConcurrentBag<DuckRaceInfo>();

        public static GlobalDataManager Instance;
        // The FPS that the app wants to run at when not in focus and no race is running, so it takes less resources
        public int DestinationFPS = 1;

        private const string DuckPipeName = "Duck_Race_Pipe";
        private bool _isListening = false;
        private readonly List<Color> _allNamedColors = new List<Color>();



        public override void _Ready()
        {
            GD.Print("Manager");
            Instance = this;
            // Always limit the frames, for this part we only need 1 fps, 0 would be uncapped
            Engine.MaxFps = 1;
            // Set Named Colors for random Color Assignment, this is not really necessary
            SetAllNamedColors();


            // Reading the command line arguments
            var arguments = OS.GetCmdlineArgs();
            var indexOfData = Array.FindIndex(arguments, line => line == "--data");

            //In thios case we just use them as a flag
            var startMinimized = Array.FindIndex(arguments, line => line == "--startMinimized") > -1;
            var overrideExitButton = Array.FindIndex(arguments, line => line == "--overrideExitButton") > -1;
            string duckData = string.Empty;

            SetupStatusIndicator(overrideExitButton);

            if (startMinimized)
            {
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.NoFocus, true);
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Minimized);
            }
            else
            {
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.NoFocus, false);
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DestinationFPS = 60;
                Engine.MaxFps = 60;
            }


            if (indexOfData > -1 && indexOfData + 1 < arguments.Length)
            {
                // Finding the --data and cleaning up the JSON for parsing
                duckData = arguments[indexOfData + 1].Replace("'", "\"");
            }
            else if (OS.HasFeature("editor"))
            {
                // When we are debugging use an example array
                duckData = "[ { \"Id\": \"AAgs5H0txAV59us-H_xaysFw\", \"ColorName\": \"random\", \"HasPartyHat\": false, \"Name\": \"Rudi\" }, { \"Id\": \"BBgs5H0txAV59us-H_xaysFw\", \"ColorName\": \"name\", \"HasPartyHat\": false, \"Name\": \"Seki\" }, { \"Id\": \"DDgs5H0txAV59us-H_xaysFw\", \"ColorName\": \"REBECCAPURPLE\", \"HasPartyHat\": false, \"Name\": \"Doggy\" }, { \"Id\": \"EEgs5H0txAV59us-H_xaysFw\", \"ColorName\": \"#03f221\", \"HasPartyHat\": false, \"Name\": \"Mira the Lobter\" }, { \"Id\": \"FFgs5H0txAV59us-H_xaysFw\", \"ColorName\": \"Red\", \"HasPartyHat\": true, \"Name\": \"Piwo\" }, { \"Id\": \"CCgs5H0txAV59us-H_xaysFw\", \"ColorName\": \"id\", \"HasPartyHat\": false, \"Name\": \"Edit\" } ]";
            }

            // We use the .Net mutex for inter-process synchronization, Godot has one too but that one seems to be mainly for its own threads and not system wide
            // I am using this mutex to determine if the software is already running, in which case i will just pipe over the start parameters
            // Oh and this "out var mutexWasCreated" is a C#-ism basically we declare a bool and mutate it in the method, indicating success
            System.Threading.Mutex m = new System.Threading.Mutex(true, "MyMutex", out var mutexWasCreated);

            if (!mutexWasCreated)
            {

                SendByPipeAndQuit(duckData);
            }
            else
            {
                SetRaceParticipants(duckData);
            }
        }


        /// <summary>
        /// Using reflection to get all named Colors. I usually google how to do this, it is a more "obscure" language feature, and some purists get all angry if you use it.
        /// Not that important to understand. I just took this because generating random colors that don't look ugly is a bit hard.
        /// </summary>
        private void SetAllNamedColors()
        {
            var allColors = typeof(Colors)
                .GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Where(x => x.PropertyType == typeof(Color))
                .OrderBy(x => x.Name)
                .Select(x => (Color)x.GetValue(null))
                .ToList();

            _allNamedColors.AddRange(allColors);
        }

        public Color GetRandomColor()
        {
            return _allNamedColors[Random.Shared.Next(0, _allNamedColors.Count)];
        }

        /// <summary>
        /// Get a named color based on whatever string is provided
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
        public Color GetRandomColor(string seed)
        {
            // Humanities worst way to generate a seed
            int seedValue = 0;
            for (int i = 0; i < seed.Length; i++)
            {
                seedValue += i*seed[i];
            }

            var rng = new Random(seedValue);
            GD.Print(seedValue);
            return _allNamedColors[rng.Next(0, _allNamedColors.Count)];
        }

        private void SetRaceParticipants(string duckData)
        {
            List<DuckRaceInfo> parsedDucks = null;
            if (!string.IsNullOrEmpty(duckData))
            {
                // If there is a format error in our JSON object this might fail so i am catching it
                try
                {
                    //lazy hack for json convert so i don't have to write a custom converter
                    duckData = duckData.Replace("\"true\"", "true").Replace("\"false\"", "false");


                    parsedDucks = JsonSerializer.Deserialize<List<DuckRaceInfo>>(duckData,
                        new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                }
                catch
                {
                    // To prevent game from crashing if the Duck Data is malformed
                    GD.Print("Ducks Could not be parsed");
                }
            }

            if (parsedDucks is not null)
            {
                // Adding each Duck to a Thread safe Bag, just in case because i use Threads and i don't want the program to crash for no reason if a collection is suddenly modified
                // Probably unnecessary
                parsedDucks.ForEach(x => RaceParticipants.Add(x));
            }
            
        }

        public override void _Process(double delta)
        {
            // The Reason i do the Thread creation like this is because i do not want to risk .Net creating a thread from a Thread from a Thread until exhaution
            // I did not test if that would actually happen but i just figured i add a precaution
            if (!_isListening)
            {
                ListenForPipes();
            }
        }


        /// <summary>
        /// We use a Windows Datapipe to throw the data to another already listening process
        /// </summary>
        /// <param name="duckData"></param>
        private void SendByPipeAndQuit(string duckData)
        {
            try
            {
                var client = new NamedPipeClientStream(DuckPipeName);
                StreamWriter writer = new StreamWriter(client);
                client.Connect(300);
                writer.Write(duckData);
                writer.Flush(); 
                Thread.Sleep(500);
                client.Dispose();

            }
            finally
            {
                // We pause all game loops so nothing weird happens before quitting
                GetTree().Paused = true;
                // We always want to close the app when we get here
                GetTree().Quit();

            }

        }

        // Launching a Thread to listen for a pipe to provide duck race info
        // If we get info we kill the Thread and start a new one in _Process
        public void ListenForPipes()
        {
            _isListening = true;

            Task.Factory.StartNew(() =>
            {
                using (var server = new NamedPipeServerStream(DuckPipeName))
                {
                    StreamReader reader = new StreamReader(server);
                    server.WaitForConnection();
                    var line = reader.ReadToEnd();
                    server.Disconnect();
                    SetRaceParticipants(line);
                }
                _isListening = false;
            }, TaskCreationOptions.LongRunning);

        }


        // This is all System Icon stuff

        private void SetupStatusIndicator(bool overrideExitButton)
        {
            var statusIndicator = new StatusIndicator();
            statusIndicator.Icon = ResourceLoader.Load<Texture2D>("res://Icon.png");
            statusIndicator.Tooltip = "Duck Race";
            statusIndicator.Pressed += StatusIndicatorOnPressed;
            AddChild(statusIndicator);

            var menu = new PopupMenu();
            AddChild(menu);
            menu.AddItem("Show Window", 1);
            menu.AddSeparator();
            menu.AddItem("Quit", 2);
            statusIndicator.Menu = menu.GetPath();
            menu.IdPressed += MenuOnIdPressed;

            if (overrideExitButton)
            {
                GetTree().SetAutoAcceptQuit(false);
                GetWindow().CloseRequested += OnCloseRequested;
            }

        }

        private void OnCloseRequested()
        {
            if (Input.IsKeyPressed(Key.Shift))
            {
                GetTree().Quit();
            }

            DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.NoFocus, true);
            DisplayServer.WindowSetMode(DisplayServer.WindowMode.Minimized);
            DestinationFPS = 1;
        }

        private void MenuOnIdPressed(long id)
        {
            switch (id)
            {
                case 1:
                    DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.NoFocus, false);
                    DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                    DestinationFPS = 60;
                    Engine.MaxFps = 60;
                    break;
                case 2:
                    GetTree().Quit();
                    break;
            }
        }

        private void StatusIndicatorOnPressed(long mousebutton, Vector2I mouseposition)
        {
            if (mousebutton == (long)MouseButton.Left)
            {
                DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.NoFocus, false);
                DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
                DestinationFPS = 60;
                Engine.MaxFps = 60;
            }
        }


    }
}

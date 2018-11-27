using LedItOut;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LedItOut
{
    public partial class Program
    {
        static byte[] sendbuffer;
        static UdpClient u;
        static BitArray b;

        private static DesktopDuplicatorReader _desktopDuplicatorReader;
        private static CancellationTokenSource _cancellationTokenSource;


        static void Main(string[] args)
        {
            InitDefault();
            _desktopDuplicatorReader.FrameReady += _desktopDuplicatorReader_FrameReady;
            Start();

            while (true)
            {
                var line = Console.ReadLine();
                if (line == "exit" || line == "q")
                    return;

                byte i_line;
                if (Byte.TryParse(line, out i_line))
                {
                    u.Send(new byte[] { 0x01, i_line }, 2);
                }
            }

        }
        private static void _desktopDuplicatorReader_FrameReady()
        {
            _desktopDuplicatorReader_FrameReadyAsync(); // DoAsync

        }
        private static async void _desktopDuplicatorReader_FrameReadyAsync()
        {
            byte this_r = 0;
            byte this_g = 0;
            byte this_b = 0;
            int bufferPos = 20;
            int count = 0;

            lock (SpotSet.Lock)
            {

                for (int i = 0; i < SpotSet.Spots.Length; i++)
                {
                    var spot = SpotSet.Spots[i];

                    //if (i == 0)
                    //{
                    //    spot.Changed = false;
                    //    b.Set(i, true);
                    //    sendbuffer[bufferPos++] = Convert.ToByte(255);
                    //    sendbuffer[bufferPos++] = Convert.ToByte(0);
                    //    sendbuffer[bufferPos++] = Convert.ToByte(0);
                    //}
                    //else 
                    //{
                    //    spot.Changed = false;
                    //    b.Set(i, true);
                    //    sendbuffer[bufferPos++] = Convert.ToByte(0);
                    //    sendbuffer[bufferPos++] = Convert.ToByte(255);
                    //    sendbuffer[bufferPos++] = Convert.ToByte(0);
                    //}
                    if (true | spot.Changed)
                    {
                        this_r = Convert.ToByte(spot.Red);
                        this_g = Convert.ToByte(spot.Green);
                        this_b = Convert.ToByte(spot.Blue);
                        spot.Changed = false;
                        b.Set(i, true);
                        sendbuffer[bufferPos++] = Convert.ToByte(this_r);
                        sendbuffer[bufferPos++] = Convert.ToByte(this_g);
                        sendbuffer[bufferPos++] = Convert.ToByte(this_b);
                    }
                    else
                    {
                        b.Set(i, false);
                        count++;
                    }
                }
            }

            if (count != SpotSet.Spots.Length)
            {
                b.CopyTo(sendbuffer, 0);
                await u.SendAsync(sendbuffer, bufferPos);
            }
            
        }


        public static void InitDefault()
        {
            UserSettings userSettings = UserSettings.Instance.LoadFromProperty();
            var expectedScreenBound = Screen.PrimaryScreen.Bounds;

            userSettings.UDPPort = 2390;
            userSettings.SpotsY = 78;
            userSettings.SpotsX = 2;
            userSettings.LedsPerSpot = 1;
            userSettings.BorderDistanceX = 0;
            userSettings.BorderDistanceY = 0;
            userSettings.OffsetX = 0;
            userSettings.OffsetY = 0;
            userSettings.OffsetLed = -1;

            if (userSettings.SpotsY > 2)
            {
                //var oneSideY = (userSettings.SpotsY / userSettings.SpotsX);
                userSettings.SpotHeight = (expectedScreenBound.Height - userSettings.SpotsY) / userSettings.SpotsY;
            }
            else
                userSettings.SpotHeight = expectedScreenBound.Height / 10;

            if (userSettings.SpotsX > 2)
            {
                //var oneSideX = (userSettings.SpotsX / userSettings.SpotsY);
                userSettings.SpotWidth = (expectedScreenBound.Height - userSettings.SpotsX) / userSettings.SpotsX;
            }
            else
                userSettings.SpotWidth = expectedScreenBound.Width / 10;

            u = new UdpClient("192.168.0.200", 2390); //192.168.42.110
           
            sendbuffer = new byte[1024];

            
            SpotSet.Refresh();
            b = new BitArray(SpotSet.Spots.Length);
            _cancellationTokenSource = new CancellationTokenSource();
            _desktopDuplicatorReader = new DesktopDuplicatorReader();
        }

        public static void Start()
        { 

            var thread = new Thread(() => _desktopDuplicatorReader.Run(_cancellationTokenSource.Token))
            {
                IsBackground = true,
                Priority = ThreadPriority.BelowNormal,
                Name = "DesktopDuplicatorReader"
            };
            thread.Start();
        }

    }
}

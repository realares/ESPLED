using NLog;
using Ra.LedItOut.Properties;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LedItOut
{

    public sealed class UserSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ILogger _log = LogManager.GetCurrentClassLogger();

        private int _spotsX;
        private int _spotsY;
        private int _ledsPerSpot;
        private int _offsetX;
        private int _offsetY;
        private bool _transferActive;
        private bool _overlayActive;
        private int _udpPort;
        private byte _saturationTreshold;
        private int _spotWidth;
        private int _spotHeight;
        private bool _mirrorX;
        private bool _mirrorY;
        private int _offsetLed;
        private int _borderDistanceX;
        private int _borderDistanceY;
        private bool _autostart;
        private bool _startMinimized;
        private DateTime? _lastUpdateCheck;

        private static Lazy<UserSettings> _usersetting = new Lazy<UserSettings>();
        public static UserSettings Instance = _usersetting.Value;

        public UserSettings()
        {

        }
        
        public UserSettings LoadFromProperty()
        {
            var settings = Settings.Default;

            _spotsX = settings.SPOTS_X;
            _spotsY = settings.SPOTS_Y;
            _ledsPerSpot = settings.LEDS_PER_SPOT;
            _offsetX = settings.OFFSET_X;
            _offsetY = settings.OFFSET_Y;
            _transferActive = settings.TRANSFER_ACTIVE;
            _overlayActive = settings.OVERLAY_ACTIVE;
            _udpPort = settings.UDP_PORT;
            _saturationTreshold = settings.SATURATION_TRESHOLD;
            _spotWidth = settings.SPOT_WIDTH;
            _spotHeight = settings.SPOT_HEIGHT;
            _mirrorX = settings.MIRROR_X;
            _mirrorY = settings.MIRROR_Y;
            _offsetLed = settings.OFFSET_LED;
            _borderDistanceX = settings.BORDER_DISTANCE_X;
            _borderDistanceY = settings.BORDER_DISTANCE_Y;
            _autostart = settings.AUTOSTART;
            _startMinimized = settings.START_MINIMIZED;
            _lastUpdateCheck = settings.LAST_UPDATE_CHECKDATE_UTC;

            _log.Info($"UserSettings created.");
            return this;
        }

        public bool UseLinearLighting
        {
            get => Settings.Default.USE_LINEAR_LIGHTING;
            set
            {
                Settings.Default.USE_LINEAR_LIGHTING = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }
        public int SpotsX
        {
            get { return _spotsX; }
            set
            {
                _spotsX = value;
                Settings.Default.SPOTS_X = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public int SpotsY
        {
            get { return _spotsY; }
            set
            {
                _spotsY = value;
                Settings.Default.SPOTS_Y = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public int LedsPerSpot
        {
            get { return _ledsPerSpot; }
            set
            {
                _ledsPerSpot = value;
                Settings.Default.LEDS_PER_SPOT = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public int OffsetX
        {
            get { return _offsetX; }
            set
            {
                _offsetX = value;
                Settings.Default.OFFSET_X = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public int OffsetY
        {
            get { return _offsetY; }
            set
            {
                _offsetY = value;
                Settings.Default.OFFSET_Y = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public bool TransferActive
        {
            get { return _transferActive; }
            set
            {
                _transferActive = value;
                Settings.Default.TRANSFER_ACTIVE = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public bool OverlayActive
        {
            get { return _overlayActive; }
            set
            {
                _overlayActive = value;
                Settings.Default.OVERLAY_ACTIVE = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public int UDPPort
        {
            get { return _udpPort; }
            set
            {
                _udpPort = value;
                Settings.Default.UDP_PORT = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public byte SaturationTreshold
        {
            get { return _saturationTreshold; }
            set
            {
                _saturationTreshold = value;
                Settings.Default.SATURATION_TRESHOLD = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public int SpotWidth
        {
            get { return _spotWidth; }
            set
            {
                _spotWidth = value;
                Settings.Default.SPOT_WIDTH = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public int SpotHeight
        {
            get { return _spotHeight; }
            set
            {
                _spotHeight = value;
                Settings.Default.SPOT_HEIGHT = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public bool MirrorX
        {
            get { return _mirrorX; }
            set
            {
                _mirrorX = value;
                Settings.Default.MIRROR_X = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public bool MirrorY
        {
            get { return _mirrorY; }
            set
            {
                _mirrorY = value;
                Settings.Default.MIRROR_Y = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public int OffsetLed
        {
            get { return _offsetLed; }
            set
            {
                _offsetLed = value;
                Settings.Default.OFFSET_LED = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public int BorderDistanceX
        {
            get { return _borderDistanceX; }
            set
            {
                _borderDistanceX = value;
                Settings.Default.BORDER_DISTANCE_X = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public int BorderDistanceY
        {
            get { return _borderDistanceY; }
            set
            {
                _borderDistanceY = value;
                Settings.Default.BORDER_DISTANCE_Y = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public bool Autostart
        {
            get { return _autostart; }
            set
            {
                _autostart = value;
                Settings.Default.AUTOSTART = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public bool StartMinimized
        {
            get { return _startMinimized; }
            set
            {
                _startMinimized = value;
                Settings.Default.START_MINIMIZED = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public DateTime? LastUpdateCheck
        {
            get { return _lastUpdateCheck; }
            set
            {
                _lastUpdateCheck = value;
                Settings.Default.LAST_UPDATE_CHECKDATE_UTC = value;
                Settings.Default.Save();
                OnPropertyChanged();
            }
        }
    }
}

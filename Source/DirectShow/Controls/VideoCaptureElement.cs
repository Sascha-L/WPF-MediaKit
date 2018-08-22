using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using WPFMediaKit.DirectShow.MediaPlayers;
using DirectShowLib;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;

namespace WPFMediaKit.DirectShow.Controls
{
    /// <summary>
    /// The VideoCaptureElement is a WPF control that
    /// displays video from a capture device, such as 
    /// a web cam.
    /// </summary>
    public class VideoCaptureElement : MediaElementBase
    {
        public VideoCaptureElement()
        {
            CommandBindings.Add(new CommandBinding(ShowPropertyPagesCommand, OnShowPropertyPagesCommand));
        }

        #region DesiredPixelWidth

        public static readonly DependencyProperty DesiredPixelWidthProperty =
            DependencyProperty.Register("DesiredPixelWidth", typeof(int), typeof(VideoCaptureElement),
                new FrameworkPropertyMetadata(0));

        public int DesiredPixelWidth
        {
            get { return (int)GetValue(DesiredPixelWidthProperty); }
            set { SetValue(DesiredPixelWidthProperty, value); }
        }

        #endregion

        #region MaxResolution

        public static readonly DependencyProperty MaxResolutionProperty =
            DependencyProperty.Register("MaxResolution", typeof(bool), typeof(VideoCaptureElement));

        public bool MaxResolution
        {
            get { return (bool)GetValue(MaxResolutionProperty); }
            set { SetValue(MaxResolutionProperty, value); }
        }

        #endregion

        #region DesiredPixelHeight

        public static readonly DependencyProperty DesiredPixelHeightProperty =
            DependencyProperty.Register("DesiredPixelHeight", typeof(int), typeof(VideoCaptureElement),
                new FrameworkPropertyMetadata(0));

        public int DesiredPixelHeight
        {
            get { return (int)GetValue(DesiredPixelHeightProperty); }
            set { SetValue(DesiredPixelHeightProperty, value); }
        }

        #endregion

        #region FPS

        public static readonly DependencyProperty FPSProperty =
            DependencyProperty.Register("FPS", typeof(int), typeof(VideoCaptureElement),
                new FrameworkPropertyMetadata(30));

        public int FPS
        {
            get { return (int)GetValue(FPSProperty); }
            set { SetValue(FPSProperty, value); }
        }

        #endregion

        #region Commands
        public static readonly RoutedCommand ShowPropertyPagesCommand = new RoutedCommand();

        private void OnShowPropertyPagesCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ShowPropertyPage();
        }
        #endregion

        #region VideoCaptureSource

        public static readonly DependencyProperty VideoCaptureSourceProperty =
            DependencyProperty.Register("VideoCaptureSource", typeof(string), typeof(VideoCaptureElement),
                new FrameworkPropertyMetadata("",
                    new PropertyChangedCallback(OnVideoCaptureSourceChanged)));

        private bool m_sourceChanged;

        public string VideoCaptureSource
        {
            get { return (string)GetValue(VideoCaptureSourceProperty); }
            set { SetValue(VideoCaptureSourceProperty, value); }
        }


        public static readonly DependencyProperty VideoCaptureDeviceProperty =
            DependencyProperty.Register("VideoCaptureDevice", typeof(DsDevice), typeof(VideoCaptureElement),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnVideoCaptureSourceChanged)));

        private bool m_DeviceChanged;

        public DsDevice VideoCaptureDevice
        {
            get { return (DsDevice)GetValue(VideoCaptureDeviceProperty); }
            set { SetValue(VideoCaptureDeviceProperty, value); }
        }

        private static void OnVideoCaptureSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VideoCaptureElement)d).OnVideoCaptureSourceChanged(e);
        }

        protected virtual void OnVideoCaptureSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == VideoCaptureSourceProperty)
                m_sourceChanged = true;
            else if (e.Property == VideoCaptureDeviceProperty)
                m_DeviceChanged = true;

            if (HasInitialized)
                PlayerVideoCaptureSource();
        }

        private void PlayerVideoCaptureSource()
        {
            if (m_sourceChanged)
            {
                string videoSource = VideoCaptureSource;
                VideoCapturePlayer.Dispatcher.BeginInvoke((Action)delegate
                {
                    VideoCapturePlayer.VideoCaptureSource = videoSource;
                });
                m_sourceChanged = false;
            }
            else if (m_DeviceChanged)
            {
                DsDevice device = VideoCaptureDevice;
                VideoCapturePlayer.Dispatcher.BeginInvoke((Action)delegate
                {
                    VideoCapturePlayer.VideoCaptureDevice = device;
                });
                m_DeviceChanged = false;
            }

            //Dispatcher.BeginInvoke((Action)delegate
            //{
            if (IsLoaded)
                ExecuteMediaState(LoadedBehavior);
            //else
            //    ExecuteMediaState(UnloadedBehavior);
            //});
        }

        #endregion

        #region EnableSampleGrabbing

        public static readonly DependencyProperty EnableSampleGrabbingProperty =
            DependencyProperty.Register("EnableSampleGrabbing", typeof(bool), typeof(VideoCaptureElement),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnEnableSampleGrabbingChanged)));

        public bool EnableSampleGrabbing
        {
            get { return (bool)GetValue(EnableSampleGrabbingProperty); }
            set { SetValue(EnableSampleGrabbingProperty, value); }
        }

        private static void OnEnableSampleGrabbingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VideoCaptureElement)d).OnEnableSampleGrabbingChanged(e);
        }

        protected virtual void OnEnableSampleGrabbingChanged(DependencyPropertyChangedEventArgs e)
        {
            VideoCapturePlayer.EnableSampleGrabbing = (bool)e.NewValue;
        }

        #endregion

        #region UseYuv

        public static readonly DependencyProperty UseYuvProperty =
            DependencyProperty.Register("UseYuv", typeof(bool), typeof(VideoCaptureElement),
                new FrameworkPropertyMetadata((bool)false,
                    new PropertyChangedCallback(OnUseYuvChanged)));

        public bool UseYuv
        {
            get { return (bool)GetValue(UseYuvProperty); }
            set { SetValue(UseYuvProperty, value); }
        }

        /// <summary>
        /// Handles changes to the UseYuv property.
        /// </summary>
        private static void OnUseYuvChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VideoCaptureElement)d).OnUseYuvChanged(e);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the UseYuv property.
        /// </summary>
        protected virtual void OnUseYuvChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        public static readonly DependencyProperty OutputFileNameProperty = DependencyProperty.Register("OutputFileName", typeof(string), typeof(VideoCaptureElement), new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnOutputFileNameChanged)));

        protected static void OnOutputFileNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((VideoCaptureElement)d).OnOutputFileNameChanged(e);
        }

        protected virtual void OnOutputFileNameChanged(DependencyPropertyChangedEventArgs e)
        {
            this.VideoCapturePlayer.FileName = (string)e.NewValue;
        }

        public string OutputFileName
        {
            get { return (string)GetValue(OutputFileNameProperty); }
            set { SetValue(OutputFileNameProperty, value); }
        }

        #region FileName



        #endregion

        public override void EndInit()
        {
            SetParameters();
            PlayerVideoCaptureSource();
            base.EndInit();
        }

        public override void Play()
        {
            SetParameters();
            base.Play();
        }

        public event EventHandler<VideoSampleArgs> NewVideoSample;

        private void InvokeNewVideoSample(VideoSampleArgs e)
        {
            EventHandler<VideoSampleArgs> sample = NewVideoSample;
            if (sample != null) sample(this, e);
        }

        private void PlayerNewVideoSample(object sender, VideoSampleArgs e)
        {
            InvokeNewVideoSample(e);
        }

        protected override void InitializeMediaPlayer()
        {
            base.InitializeMediaPlayer();

            VideoCapturePlayer.NewVideoSample += PlayerNewVideoSample;
        }

        protected VideoCapturePlayer VideoCapturePlayer
        {
            get
            {
                return MediaPlayerBase as VideoCapturePlayer;
            }
        }

        /// <summary>
        /// Sets the parameters to the video capture player
        /// </summary>
        private void SetParameters()
        {
            int height = 0;
            int width = 0;
            if (this.MaxResolution && this.VideoCaptureDevice != null)
            {
                var reses = GetAllAvailableResolution(this.VideoCaptureDevice);
                var maxRes = reses.FirstOrDefault(p => p.Width == reses.Max(r => r.Width));

                width = maxRes.Width;
                height = maxRes.Height;
            }
            else
            {
                height = DesiredPixelHeight;
                width = DesiredPixelWidth;
            }

            int fps = FPS;
            bool useYuv = UseYuv;
            string filename = OutputFileName;

            VideoCapturePlayer.Dispatcher.BeginInvoke((Action)delegate
            {
                VideoCapturePlayer.UseYuv = useYuv;
                VideoCapturePlayer.FPS = fps;
                VideoCapturePlayer.DesiredWidth = width;
                VideoCapturePlayer.DesiredHeight = height;
                VideoCapturePlayer.FileName = filename;
            });
        }

        public void ShowPropertyPage()
        {
            var window = Window.GetWindow(this);
            var hwnd = IntPtr.Zero;

            if (window != null)
            {
                hwnd = new WindowInteropHelper(window).Handle;
            }

            MediaPlayerBase.Dispatcher.BeginInvoke((Action)(() => VideoCapturePlayer.ShowCapturePropertyPages(hwnd)));
        }

        protected override MediaPlayerBase OnRequestMediaPlayer()
        {
            return new VideoCapturePlayer();
        }

        public List<Resolution> GetAllAvailableResolution(DsDevice vidDev)
        {
            try
            {
                int hr;
                int max = 0;
                int bitCount = 0;

                IBaseFilter sourceFilter = null;

                var m_FilterGraph2 = new FilterGraph() as IFilterGraph2;

                hr = m_FilterGraph2.AddSourceFilterForMoniker(vidDev.Mon, null, vidDev.Name, out sourceFilter);

                var pRaw2 = DsFindPin.ByCategory(sourceFilter, PinCategory.Capture, 0);

                var AvailableResolutions = new List<Resolution>();

                VideoInfoHeader v = new VideoInfoHeader();
                IEnumMediaTypes mediaTypeEnum;
                hr = pRaw2.EnumMediaTypes(out mediaTypeEnum);

                AMMediaType[] mediaTypes = new AMMediaType[1];
                IntPtr fetched = IntPtr.Zero;
                hr = mediaTypeEnum.Next(1, mediaTypes, fetched);

                while (fetched != null && mediaTypes[0] != null)
                {
                    Marshal.PtrToStructure(mediaTypes[0].formatPtr, v);
                    if (v.BmiHeader.Size != 0 && v.BmiHeader.BitCount != 0)
                    {
                        if (v.BmiHeader.BitCount > bitCount)
                        {
                            AvailableResolutions.Clear();
                            max = 0;
                            bitCount = v.BmiHeader.BitCount;


                        }
                        AvailableResolutions.Add(new Resolution(v.BmiHeader.Width, v.BmiHeader.Height));
                        if (v.BmiHeader.Width > max || v.BmiHeader.Height > max)
                            max = (Math.Max(v.BmiHeader.Width, v.BmiHeader.Height));
                    }
                    hr = mediaTypeEnum.Next(1, mediaTypes, fetched);
                }
                return AvailableResolutions;
            }

            catch (Exception ex)
            {
                //Log(ex);
                return new List<Resolution>();
            }
        }
    }

    public class Resolution
    {
        public Resolution(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public int Width
        {
            get; set;
        }

        public int Height
        {
            get; set;
        }
    }
}

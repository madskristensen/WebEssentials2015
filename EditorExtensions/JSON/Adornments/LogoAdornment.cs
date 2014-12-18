using MadsKristensen.EditorExtensions.Settings;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MadsKristensen.EditorExtensions.JSON
{
    class LogoAdornment
    {
        private IAdornmentLayer _adornmentLayer;
        private Image _adornment;
        private readonly double _initOpacity;
        private double _currentOpacity;

        public LogoAdornment(IWpfTextView view, string imageName, bool isVisible, double initOpacity)
        {
            _adornmentLayer = view.GetAdornmentLayer(LogoLayer.LayerName);
            _currentOpacity = isVisible ? initOpacity : 0;
            _initOpacity = initOpacity;

            CreateImage(imageName);

            view.ViewportHeightChanged += SetAdornmentLocation;
            view.ViewportWidthChanged += SetAdornmentLocation;
            VisibilityChanged += ToggleVisibility;

            if (_adornmentLayer.IsEmpty)
                _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _adornment, null);
        }

        private void ToggleVisibility(object sender, bool isVisible)
        {
            _adornment.Opacity = isVisible ? _initOpacity : 0;
            _currentOpacity = _adornment.Opacity;
        }

        private void CreateImage(string imageName)
        {
            _adornment = new Image();
            _adornment.Source = BitmapFrame.Create(new Uri("pack://application:,,,/WebEssentials2015;component/JSON/Resources/" + imageName, UriKind.RelativeOrAbsolute));
            _adornment.ToolTip = "Click to toggle visibility";
            _adornment.Opacity = _currentOpacity;

            _adornment.MouseEnter += (s, e) => { _adornment.Opacity = 1D; };
            _adornment.MouseLeave += (s, e) => { _adornment.Opacity = _currentOpacity; };
            _adornment.MouseLeftButtonUp += (s, e) => { OnVisibilityChanged(_currentOpacity == 0); };
        }

        private void SetAdornmentLocation(object sender, EventArgs e)
        {
            IWpfTextView view = (IWpfTextView)sender;
            Canvas.SetLeft(_adornment, view.ViewportRight - _adornment.Source.Width - 20);
            Canvas.SetTop(_adornment, view.ViewportBottom - _adornment.Source.Height - 20);
        }

        public static event EventHandler<bool> VisibilityChanged;

        private static void OnVisibilityChanged(bool isVisible)
        {
            if (VisibilityChanged != null)
            {
                WESettings.Instance.General.ShowLogoWatermark = isVisible;
                SettingsStore.Save();

                VisibilityChanged(null, isVisible);
            }
        }
    }
}

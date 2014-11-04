using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MadsKristensen.EditorExtensions.JSON
{
    class LogoAdornment
    {
        private IAdornmentLayer _adornmentLayer;
        private Image _adornment;

        public LogoAdornment(IWpfTextView view, string imageName)
        {
            _adornmentLayer = view.GetAdornmentLayer(LogoProvider.LayerName);
            CreateImage(imageName);

            view.ViewportHeightChanged += SetAdornmentLocation;
            view.ViewportWidthChanged += SetAdornmentLocation;

            Update();
        }

        private void CreateImage(string imageName)
        {
            _adornment = new Image();
            _adornment.Source = BitmapFrame.Create(new Uri("pack://application:,,,/WebEssentials2015;component/JSON/Resources/" + imageName, UriKind.RelativeOrAbsolute));
            _adornment.ToolTip = "Click to hide";
            _adornment.Opacity = 0.5D;

            _adornment.MouseEnter += (s, e) => { _adornment.Opacity = 1D; };
            _adornment.MouseLeave += (s, e) => { _adornment.Opacity = 0.5D; };
            _adornment.MouseLeftButtonUp += (s, e) => { _adornment.Visibility = Visibility.Hidden; };
        }

        private void SetAdornmentLocation(object sender, EventArgs e)
        {
            IWpfTextView view = (IWpfTextView)sender;
            Canvas.SetLeft(_adornment, view.ViewportRight - 100);
            Canvas.SetTop(_adornment, view.ViewportTop + 20);
        }

        private void Update()
        {
            if (_adornmentLayer.IsEmpty)
                _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _adornment, null);
        }
    }
}

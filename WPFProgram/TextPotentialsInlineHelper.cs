using EndoAshu.StarSavior.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace StarSaviorAssistant
{
    public static class TextPotentialsInlineHelper
    {
        public class LinkClickedEventArgs : EventArgs
        {
            public string PotentialName { get; }
            public string Text { get; }
            public LinkClickedEventArgs(string potentialName, string text)
            {
                PotentialName = potentialName;
                Text = text;
            }
        }

        private class PopupData
        {
            public TextBlock TargetTextBlock { get; set; }
            public string Content { get; set; }
        }

        public static readonly DependencyProperty BoundTextProperty =
        DependencyProperty.RegisterAttached("BoundText", typeof(string), typeof(TextPotentialsInlineHelper),
            new PropertyMetadata(null, OnBoundTextChanged));

        public static string GetBoundText(DependencyObject obj) => (string)obj.GetValue(BoundTextProperty);
        public static void SetBoundText(DependencyObject obj, string value) => obj.SetValue(BoundTextProperty, value);

        private static Popup? _currentPopup = null;

        private static void OnBoundTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock textBlock)
            {
                string fullText = e.NewValue as string;
                textBlock.Inlines.Clear();

                if (string.IsNullOrEmpty(fullText)) return;
                
                var linkDictionary = EventLoader.Potentials;

                ComposeInlines(textBlock, fullText, linkDictionary);
            }
        }

        private static void ComposeInlines(TextBlock textBlock, string fullText, IDictionary<string, string> linkDictionary)
        {
            int currentPosition = 0;

            foreach (var item in linkDictionary)
            {
                string linkKey = item.Key;

                int index = fullText.IndexOf(linkKey, currentPosition);

                if (index >= currentPosition)
                {
                    string prefix = fullText.Substring(currentPosition, index - currentPosition);
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        textBlock.Inlines.Add(new Run(prefix));
                    }

                    Hyperlink link = new Hyperlink();
                    link.Inlines.Add(new Run(linkKey));
                    link.Tag = new PopupData
                    {
                        TargetTextBlock = textBlock,
                        Content = $"{item.Key} : {item.Value}"
                    };
                    link.MouseEnter += Link_MouseEnter;
                    link.MouseLeave += Link_MouseLeave;

                    textBlock.Inlines.Add(link);

                    currentPosition = index + linkKey.Length;
                }
            }

            if (currentPosition < fullText.Length)
            {
                string suffix = fullText.Substring(currentPosition);
                textBlock.Inlines.Add(new Run(suffix));
            }
        }

        private static void Link_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Hyperlink clickedLink && clickedLink.Tag is string toolTipContent)
            {
                ToolTip toolTip = new ToolTip
                {
                    Content = toolTipContent,
                    Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint,
                    IsOpen = true
                };

                clickedLink.ToolTip = toolTip;

                DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(5) };
                timer.Tick += (s, ev) =>
                {
                    toolTip.IsOpen = false;
                    ((DispatcherTimer)s).Stop();
                };
                timer.Start();
            }
        }

        private static void Link_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_currentPopup != null && _currentPopup.IsOpen)
            {
                _currentPopup.IsOpen = false;
                _currentPopup = null;
            }

            if (sender is Hyperlink link && link.Tag is PopupData data)
            {
                Border popupContent = new Border
                {
                    Background = Application.Current.Resources[ThemeManager.MAIN_BG_KEY] as SolidColorBrush,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(3),
                    Padding = new Thickness(8),
                    Child = new TextBlock
                    {
                        Text = data.Content,
                        Foreground = Application.Current.Resources["SubText"] as SolidColorBrush,
                        FontWeight = FontWeights.SemiBold
                    }
                };

                Popup popup = new Popup
                {
                    PlacementTarget = data.TargetTextBlock,
                    Placement = PlacementMode.Mouse,
                    AllowsTransparency = true,
                    StaysOpen = true,
                    Child = popupContent
                };

                popup.IsOpen = true;
                _currentPopup = popup;
            }
        }


        private static void Link_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_currentPopup != null)
            {
                _currentPopup.IsOpen = false;
                _currentPopup = null;
            }
        }
    }
}

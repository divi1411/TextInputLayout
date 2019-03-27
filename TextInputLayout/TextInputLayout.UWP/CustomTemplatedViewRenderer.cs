using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextInputLayout;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.UWP;
using Forms = Xamarin.Forms;

[assembly: ExportRenderer(typeof(CustomTemplatedView), typeof(TextInputLayout.UWP.CustomTemplatedViewRenderer))]
namespace TextInputLayout.UWP
{
    public class CustomTemplatedViewRenderer : ViewRenderer<CustomTemplatedView, FrameworkElement>
    {
        CustomTemplatedView customTemplatedView;

        TextBox nativeTextBox;

        private string textBoxText = string.Empty;

        private bool isInternalChange;
        protected override void OnElementChanged(ElementChangedEventArgs<CustomTemplatedView> e)
        {
            base.OnElementChanged(e);

            var element = e.NewElement;
            if (element != null && element is CustomTemplatedView)
            {
                customTemplatedView = element as CustomTemplatedView;
                UpdateNativeView(customTemplatedView.InputView);
                if (nativeTextBox != null)
                {
                    nativeTextBox.Padding = new Thickness(0, 24, 0, 10);
                }
            }
        }

        private void UpdateNativeView(Forms.View view)
        {
            if (nativeTextBox != null)
            {
                nativeTextBox.TextChanged -= NativeTextBox_TextChanged;
                nativeTextBox.Loaded -= NativeTextBox_Loaded;
            }

            if (view == null)
            {
                return;
            }

            if (Platform.GetRenderer(view) == null)
            {
                Platform.SetRenderer(view, Platform.CreateRenderer(view));
            }

            var renderer = Platform.GetRenderer(view);
            if (renderer is Panel)
            {
                var panel = renderer as Panel;
                foreach (var child in panel.Children)
                {
                    if (child is FormsTextBox)
                    {
                        (child as FormsTextBox).BackgroundFocusBrush = new SolidColorBrush(Colors.Transparent);
                    }

                    if (child is TextBox)
                    {
                        nativeTextBox = child as TextBox;
                        if (nativeTextBox != null)
                        {
                            nativeTextBox.IsTabStop = customTemplatedView.IsEnabled;
                            textBoxText = nativeTextBox.Text;
                            isInternalChange = !string.IsNullOrEmpty(textBoxText);
                            if (isInternalChange)
                            {
                                nativeTextBox.Text = string.Empty;
                            }

                            if (nativeTextBox != null)
                            {
                                nativeTextBox.TextChanged += NativeTextBox_TextChanged;
                                nativeTextBox.Loaded += NativeTextBox_Loaded;
                            }
                        }

                        break;
                    }
                }
            }
        }

        private void NativeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isInternalChange && nativeTextBox.Text == textBoxText)
            {
                isInternalChange = false;
                return;
            }
        }

        private void NativeTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(nativeTextBox);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(nativeTextBox, i);
                if (child is Grid)
                {
                    var parent = child as Grid;
                    if (parent == null)
                    {
                        continue;
                    }

                    var clearButton = parent.FindName("DeleteButton") as Button;
                    if (clearButton != null)
                    {
                        clearButton.Visibility = Visibility.Collapsed;
                        clearButton.Style = null;
                        (clearButton.Parent as Panel)?.Children.Remove(clearButton);
                    }

                    var borderElement = parent.FindName("BorderElement") as Border;
                    if (borderElement != null)
                    {
                        borderElement.Visibility = Visibility.Collapsed;
                    }

                    var backgroundElement = parent.FindName("BackgroundElement") as Border;
                    if (backgroundElement != null)
                    {
                        backgroundElement.Visibility = Visibility.Collapsed;
                    }

                    UpdateText();
                    //Forms.Device.BeginInvokeOnMainThread(() => UpdateText());

                    break;
                }
            }
         
        }

        void UpdateText()
        {
            if (this.isInternalChange)
            {
                var currentText = nativeTextBox.Text;
                textBoxText = string.IsNullOrEmpty(currentText) ? textBoxText : currentText;
                nativeTextBox.Text = textBoxText;
            }
        }
    }
}

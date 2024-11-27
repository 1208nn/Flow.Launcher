﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Flow.Launcher.Infrastructure;
using Flow.Launcher.Infrastructure.Logger;

namespace Flow.Launcher.Core
{
    public partial class MessageBoxEx : Window
    {
        private static MessageBoxEx msgBox;
        private static MessageBoxResult _result = MessageBoxResult.None;

        private readonly MessageBoxButton _button;

        private MessageBoxEx(MessageBoxButton button)
        {
            _button = button;
            InitializeComponent();
        }

        /// 1 parameter
        public static MessageBoxResult Show(string messageBoxText)
        {
            return Show(messageBoxText, string.Empty, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
        }

        // 2 parameter
        public static MessageBoxResult Show(string messageBoxText, string caption)
        {
            return Show(messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK);
        }

        /// 3 parameter
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button) 
        {
            return Show(messageBoxText, caption, button, MessageBoxImage.None, MessageBoxResult.OK);
        }

        // 4 parameter
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            return Show(messageBoxText, caption, button, icon, MessageBoxResult.OK);
        }

        // 5 parameter, Final Display Message. 
        public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                return Application.Current.Dispatcher.Invoke(() => Show(messageBoxText, caption, button, icon, defaultResult));
            }

            try
            {
                msgBox = new MessageBoxEx(button);
                if (caption == string.Empty && button == MessageBoxButton.OK && icon == MessageBoxImage.None)
                {
                    msgBox.Title = messageBoxText;
                    msgBox.DescOnlyTextBlock.Visibility = Visibility.Visible;
                    msgBox.DescOnlyTextBlock.Text = messageBoxText;
                }
                else
                {
                    msgBox.Title = caption;
                    msgBox.TitleTextBlock.Text = caption;
                    msgBox.DescTextBlock.Text = messageBoxText;
                    SetImageOfMessageBox(icon);
                }
                SetButtonVisibilityFocusAndResult(button, defaultResult);
                msgBox.ShowDialog();
                return _result;
            }
            catch (Exception e)
            {
                Log.Error($"|MessageBoxEx.Show|An error occurred: {e.Message}");
                msgBox = null;
                return MessageBoxResult.None;
            }
        }

        private static void SetButtonVisibilityFocusAndResult(MessageBoxButton button, MessageBoxResult defaultResult)
        {
            switch (button)
            {
                case MessageBoxButton.OK:
                    msgBox.btnCancel.Visibility = Visibility.Collapsed;
                    msgBox.btnNo.Visibility = Visibility.Collapsed;
                    msgBox.btnYes.Visibility = Visibility.Collapsed;
                    msgBox.btnOk.Focus();
                    _result = MessageBoxResult.OK;
                    break;
                case MessageBoxButton.OKCancel:
                    msgBox.btnNo.Visibility = Visibility.Collapsed;
                    msgBox.btnYes.Visibility = Visibility.Collapsed;
                    if (defaultResult == MessageBoxResult.Cancel)
                    {
                        msgBox.btnCancel.Focus();
                        _result = MessageBoxResult.Cancel;
                    }
                    else
                    {
                        msgBox.btnOk.Focus();
                        _result = MessageBoxResult.OK;
                    }
                    break;
                case MessageBoxButton.YesNo:
                    msgBox.btnOk.Visibility = Visibility.Collapsed;
                    msgBox.btnCancel.Visibility = Visibility.Collapsed;
                    if (defaultResult == MessageBoxResult.No)
                    {
                        msgBox.btnNo.Focus();
                        _result = MessageBoxResult.No;
                    }
                    else
                    {
                        msgBox.btnYes.Focus();
                        _result = MessageBoxResult.Yes;
                    }
                    break;
                case MessageBoxButton.YesNoCancel:
                    msgBox.btnOk.Visibility = Visibility.Collapsed;
                    if (defaultResult == MessageBoxResult.No)
                    {
                        msgBox.btnNo.Focus();
                        _result = MessageBoxResult.No;
                    }
                    else if (defaultResult == MessageBoxResult.Cancel)
                    {
                        msgBox.btnCancel.Focus();
                        _result = MessageBoxResult.Cancel;
                    }
                    else
                    {
                        msgBox.btnYes.Focus();
                        _result = MessageBoxResult.Yes;
                    }
                    break;
                default:
                    break;
            }
        }

        private static void SetImageOfMessageBox(MessageBoxImage icon)
        {
            switch (icon)
            {
                case MessageBoxImage.Exclamation:
                    msgBox.SetImage("Exclamation.png");
                    msgBox.Img.Visibility = Visibility.Visible;
                    break;
                case MessageBoxImage.Question:
                    msgBox.SetImage("Question.png");
                    msgBox.Img.Visibility = Visibility.Visible;
                    break;
                case MessageBoxImage.Information:
                    msgBox.SetImage("Information.png");
                    msgBox.Img.Visibility = Visibility.Visible;
                    break;
                case MessageBoxImage.Error:
                    msgBox.SetImage("Error.png");
                    msgBox.Img.Visibility = Visibility.Visible;
                    break;
                default:
                    msgBox.Img.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void SetImage(string imageName)
        {
            string uri = Constant.ProgramDirectory + "/Images/" + imageName;
            var uriSource = new Uri(uri, UriKind.RelativeOrAbsolute);
            Img.Source = new BitmapImage(uriSource);
        }

        private void KeyEsc_OnPress(object sender, ExecutedRoutedEventArgs e)
        {
            if (_button == MessageBoxButton.YesNo)
                return;
            else if (_button == MessageBoxButton.OK)
                _result = MessageBoxResult.OK;
            else
                _result = MessageBoxResult.Cancel;
            DialogResult = false;
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender == btnOk)
                _result = MessageBoxResult.OK;
            else if (sender == btnYes)
                _result = MessageBoxResult.Yes;
            else if (sender == btnNo)
                _result = MessageBoxResult.No;
            else if (sender == btnCancel)
                _result = MessageBoxResult.Cancel;
            else
                _result = MessageBoxResult.None;
            msgBox.Close();
            msgBox = null;
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            if (_button == MessageBoxButton.YesNo)
                return;
            else if (_button == MessageBoxButton.OK)
                _result = MessageBoxResult.OK;
            else
                _result = MessageBoxResult.Cancel;
            msgBox.Close();
            msgBox = null;
        }
    }
}

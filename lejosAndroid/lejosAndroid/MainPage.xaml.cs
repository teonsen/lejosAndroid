﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Essentials;
using lejos_client;

namespace lejosAndroid
{
    public partial class MainPage : ContentPage
    {
        //EV3オブジェクト
        EV3 _ev3;
        bool _OrientationMode = false;

        public MainPage()
        {
            InitializeComponent();
            // アプリ開始時にEV3がWiFiで取得したアドレスに接続
            _ev3 = new EV3("192.168.1.108", 6789);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            OrientationSensor.ReadingChanged += OrientationSensor_ReadingChanged;
            OrientationSensor.Start(SensorSpeed.UI);
        }

        void OrientationSensor_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            var data = e.Reading;
            // 調整するパラメータ
            float paramX = 3;
            float paramY = 5.5F;
            int paramLR = 140; // 直角に傾けるくらいで左右モードにする

            // Xに係数をかけて左右のSpeedとする
            int ox = (int)(data.Orientation.X * 100 * paramX);
            // Yに係数をかけて前後のSpeedとする
            int oy = (int)(data.Orientation.Y * 100 * paramY);
            int ow = (int)(data.Orientation.W * 100);
            int oz = (int)(data.Orientation.Z * 100);
            Device.BeginInvokeOnMainThread(() =>
            {
                LableOrientation.Text = $"Orientation X={ox}, Y={oy}, Z={oz}, W={ow}";
            });
            if (_OrientationMode)
            {
                if (Math.Abs(ox) > paramLR)
                {
                    if (ox < 0)
                    {
                        _ev3.Wheels.TurnLeft(ox, 360);
                    }
                    else
                    {
                        _ev3.Wheels.TurnRight(ox, 360);
                    }
                }
                else
                {
                    if (oy >= 0)
                    {
                        _ev3.Wheels.GoForward(oy, 720);
                    }
                    else
                    {
                        _ev3.Wheels.GoBackward(oy, 720);
                    }
                }
            }
        }

        private void OnForwardButton_Clicked(object sender, EventArgs e)
        {
            _ev3.Wheels.GoForward(150, 720);
        }

        private void OnLeftButton_Clicked(object sender, EventArgs e)
        {
            _ev3.Wheels.TurnLeft(100, 100);
        }

        private void OnRightButton_Clicked(object sender, EventArgs e)
        {
            _ev3.Wheels.TurnRight(100, 100);
        }

        private void OnBackButton_Clicked(object sender, EventArgs e)
        {
            _ev3.Wheels.GoBackward(150, 720);
        }

        private void OnStopButton_Clicked(object sender, EventArgs e)
        {
            _ev3.Wheels.Stop();
        }

        private void OnToggled(object sender, ToggledEventArgs e)
        {
            _OrientationMode = this.@switch.IsToggled;
            if (!_OrientationMode)
            {
                _ev3.Wheels.Stop();
            }
        }
    }
}

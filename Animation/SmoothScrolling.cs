using Avalonia.Input;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using Avalonia.Controls;
using Microsoft.CodeAnalysis.CSharp.Syntax;





class SmoothScrolling
{



    public Action<double> Trigger { get; set; }



    //Handles SmoothScrolling
    public double ScrollingImpulseSpeed = 1000; //this will be timesd by avarage scroll impulse which is 50 => 50*20 = 1000 pixles/s 
    public double ScrollingConstantDeacceleration = 2000; //2000 pixles/s
    public double ScrollingMaxVelocity = 1000;
    public double ScrollingTrusholdVelocity = 1000;
    public double ScrollingDamping = 3; // damping will only be applied after the speed goes behind the trushold velocity
    public int Tick = 8; //in ms (7ms is about 125fps)
    public int TimeStamp = 0; //in ms

    public bool StopCurrentAnimation = false;




    private bool FunctionRunning = false;
    private double CurrentVelocity = 0;
    private double CurrentDirection = 0; // 1 is going down and -1 is going up
    private Stopwatch stopWatch = new Stopwatch();




    //Over ride the event handler with this function
    public async void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        e.Handled = true; //OverRiding the scrolling function
        if (-(e.Delta.Y / Math.Abs(e.Delta.Y)) != CurrentDirection)
        {
            CurrentVelocity = 0;
        }


        CurrentDirection = -(e.Delta.Y / Math.Abs(e.Delta.Y));
        CurrentVelocity += Math.Abs(ScrollingImpulseSpeed * e.Delta.Y);

        if (!FunctionRunning)
        {
            await ApplySmoothScrolling();
        }
    }



    private async Task ApplySmoothScrolling()
    {
        FunctionRunning = true;
        stopWatch.Start();
        stopWatch.Restart();


        while (CurrentVelocity > 0)
        {
            if (stopWatch.ElapsedMilliseconds - TimeStamp < Tick)
            {
                await Task.Delay(Tick / 2);
                continue;
            }
            if (StopCurrentAnimation) {
                StopCurrentAnimation = false;
                break;
            }

            TimeStamp += Tick;




            double _progress = CurrentVelocity * Tick / 1000;


            if (Trigger != null) Trigger(_progress * CurrentDirection);
            


            if (ScrollingMaxVelocity < CurrentVelocity) {
                CurrentVelocity -= (ScrollingConstantDeacceleration * 2 *  CurrentVelocity / ScrollingTrusholdVelocity) * Tick / 1000;
                continue;
            }

            if (CurrentVelocity < ScrollingTrusholdVelocity)
            {
                CurrentVelocity -= (ScrollingConstantDeacceleration * ScrollingDamping) * Tick / 1000;
                continue;
            }

            CurrentVelocity -= ScrollingConstantDeacceleration * Tick / 1000; // apply deacceleration
        }

        CurrentVelocity = 0;
        FunctionRunning = false;
        stopWatch.Stop();
        TimeStamp = 0;

    }

}


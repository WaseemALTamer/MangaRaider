using Avalonia.Input;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using Avalonia.Controls;





class SmoothScrolling
{

    ScrollViewer Window { get; set; }

    public SmoothScrolling(ScrollViewer Viwer)
    {

        Window = Viwer;

    }



    //Handles SmoothScrolling
    public double ScrollingImpulseSpeed = 1000; //500 pixles/s
    public double ScrollingConstantDeacceleration = 2000; //2000 pixles/s
    public double ScrollingMaxVelocity = 1000;
    public double ScrollingTrusholdVelocity = 1000;
    public double ScrollingDamping = 3; // damping will only be applied after the speed goes behind the trushold velocity
    public int Tick = 8; //in ms (7ms is about 125fps)
    public int TimeStamp = 0; //in ms





    private bool SS_FunctionRunning = false;
    private double SS_CurrentVelocity = 0;
    private double CurrentDirection = 0; // 1 is going down and -1 is going up
    private Stopwatch SS_StopWatch = new Stopwatch();




    //Over ride the event handler with this function
    public async void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        e.Handled = true; //OverRiding the scrolling function
        if (-(e.Delta.Y / Math.Abs(e.Delta.Y)) != CurrentDirection)
        {
            SS_CurrentVelocity = 0;
        }


        CurrentDirection = -(e.Delta.Y / Math.Abs(e.Delta.Y));

        SS_CurrentVelocity += Math.Abs(ScrollingImpulseSpeed);





        if (!SS_FunctionRunning)
        {
            await ApplySmoothScrolling();
        }
    }



    private async Task ApplySmoothScrolling()
    {
        SS_FunctionRunning = true;
        SS_StopWatch.Start();
        SS_StopWatch.Restart();


        while (SS_CurrentVelocity > 0)
        {
            if (SS_StopWatch.ElapsedMilliseconds - TimeStamp < Tick)
            {
                await Task.Delay(Tick / 2);
                continue;
            }
            TimeStamp += Tick;




            double _progress = SS_CurrentVelocity * Tick / 1000;

            //if (ScrollingMaxVelocity < SS_CurrentVelocity)
            //{
            //    SS_CurrentVelocity = ScrollingMaxVelocity;
            //}




            Window.Offset = new Avalonia.Vector(Window.Offset.X, Window.Offset.Y + (_progress * CurrentDirection));

            if (ScrollingMaxVelocity < SS_CurrentVelocity) {
                SS_CurrentVelocity -= (ScrollingConstantDeacceleration * 2 *  SS_CurrentVelocity / ScrollingTrusholdVelocity) * Tick / 1000;
                continue;
            }

            if (SS_CurrentVelocity < ScrollingTrusholdVelocity)
            {
                SS_CurrentVelocity -= (ScrollingConstantDeacceleration * ScrollingDamping) * Tick / 1000;
                continue;
            }

            SS_CurrentVelocity -= ScrollingConstantDeacceleration * Tick / 1000; // apply deacceleration


        }

        SS_CurrentVelocity = 0;
        SS_FunctionRunning = false;
        SS_StopWatch.Stop();
        TimeStamp = 0;

    }

}


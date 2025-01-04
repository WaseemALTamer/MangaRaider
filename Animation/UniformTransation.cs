using System.Threading.Tasks;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Input;
using System;

class UniformTransation
{
    public double Duration; //in ms
    public double StartingValue;
    public double EndingValue;
    public Action<double> Trigger;
    public double CurrentValue;
    public int Tick = 8; //in ms (this is 125fps


    private bool FunctionRunning = false;


    public async void TranslateForward(object? sender, object e)
    {

        if (!FunctionRunning)
        {
            _startingValue = StartingValue;
            _endingValue = EndingValue;
            _duration = Duration;
            _timeStamp = 0;
            _stopwatch.Reset();
            _stopwatch.Start();
            await Transation();
        }
        else {
            _stopwatch.Stop();
            _startingValue = CurrentValue;
            _endingValue = EndingValue;
            _duration = Duration * ((_endingValue - CurrentValue) / (_endingValue - _startingValue));
            _timeStamp = 0;
            _stopwatch.Reset();
            _stopwatch.Start();
        }
    }

    public async void TranslateBackward(object? sender, object e)
    {
        if (!FunctionRunning){
            _startingValue = EndingValue;
            _endingValue = StartingValue;
            _duration = Duration;
            _timeStamp = 0;
            _stopwatch.Reset();
            _stopwatch.Start();
            await Transation();
        }
        else
        {
            _stopwatch.Stop();
            _startingValue = CurrentValue;
            _endingValue = StartingValue;
            _duration = Duration * ((_endingValue - CurrentValue) / (_endingValue - _startingValue));
            _timeStamp = 0;
            _stopwatch.Reset();
            _stopwatch.Start();
        }
    }




    private Stopwatch _stopwatch = new Stopwatch();
    private double _startingValue;
    private double _endingValue;
    private double _duration;
    private double _timeStamp = 0;

    async private Task Transation() {
        FunctionRunning = true;


        while (_stopwatch.ElapsedMilliseconds < Duration) {
            if (_stopwatch.ElapsedMilliseconds - _timeStamp < Tick){
                await Task.Delay(Tick / 2);
                continue;
            }
            _timeStamp += Tick;
            double _delta = (_timeStamp / _duration) * (_endingValue - _startingValue);
            CurrentValue = _delta + _startingValue;


            if (Trigger != null) Trigger(CurrentValue);
            
        }
        if (Trigger != null) Trigger(_endingValue);

        FunctionRunning = false;
    }
}
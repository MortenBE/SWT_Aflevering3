using System;
using System.Threading;
using MicrowaveOvenClasses.Boundary;
using NUnit.Framework;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework.Internal.Execution;

namespace Microwave.Test.Unit
{
    
    [TestFixture]
    public class IntegrationTest1
    {
            
        private ITimer _timer;
        private IDisplay _display;
        private IUserInterface _ui;
        private IOutput _output;
        private PowerTube _powerTube;
        private CookController _sut;
        private double _percentage;

        [SetUp]
        public void SetUp()
        {
            _output = Substitute.For<IOutput>();
            _timer = Substitute.For<ITimer>();
            _display = Substitute.For<IDisplay>();
            _ui = Substitute.For<IUserInterface>();
            _powerTube = new PowerTube(_output);
            _sut = new CookController(_timer, _display, _powerTube);

        }

        [TestCase(1500, 15)]
        [TestCase(123, 15)]
        [TestCase(-10, 50)]
        public void StartCooking_PowerOutOfRange(int power, int time)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _sut.StartCooking(power, time));
        }

        [Test]
        public void StartCooking_PowerIsOn()
        {
            _sut.StartCooking(50,10);
            Assert.Throws<ApplicationException>(() => _sut.StartCooking(50,10));
        }
        
        [Test]
        public void StopCooking_OutputCorrect()
        {
            _sut.StartCooking(20, 2);
            _sut.Stop();
            _output.Received().OutputLine(Arg.Is<string>(a => a.Equals($"PowerTube turned off")));
        }

        [Test]
        public void TestCookControllerTimerExpire()
        {
            _sut.StartCooking(50, 50);
            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);
            _output.Received().OutputLine(Arg.Is<string>(a => a.Contains($"turned off"))); 
        }
        
        [TestCase(50, 1)]
        public void TestCookControllerToDisplay(int power, int time)
        {
            _sut.StartCooking(power, time);
            _display.Received().ShowPower(power);
            _output.Received().OutputLine(Arg.Is<string>( str => str.Equals($"PowerTube works with {power}")));
            _output.ClearReceivedCalls();
            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);
            _output.Received().OutputLine(Arg.Is<string>(str => 
                str.Equals($"PowerTube turned off")));
        }
        
        [Test]
        public void TestCookControllerStopToOutput()
        {
            _sut.StartCooking(80, 10);
            _sut.Stop();
            _output.Received().OutputLine(Arg.Is<string>(str => 
                str.Equals($"PowerTube turned off")));
        }
        
        [TestCase(50, 60)]
        public void TestCookControllerAndDisplay(int power, int time)
        {
            _sut.StartCooking(power, time);
            //StartCooking

            _output.Received().OutputLine(Arg.Is<string>( str => 
                str.Equals($"PowerTube works with {power}")));

            //_output.ClearReceivedCalls();
            ////OnTimerTick
            //_timer.TimerTick += Raise.Event();
            //Thread.Sleep(1000);
            //_output.Received(1).OutputLine("Display shows: 00:59");
           // _output.Received().OutputLine(Arg.Is<string>(str => str.Equals($"Display shows: 00:00")));
        }
    }
}
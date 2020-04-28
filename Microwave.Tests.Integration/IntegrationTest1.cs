using System;
using System.Threading;
using MicrowaveOvenClasses.Boundary;
using NUnit.Framework;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using Timer = MicrowaveOvenClasses.Boundary.Timer;
using NUnit.Framework.Internal.Execution;

namespace Microwave.Tests.Integration
{
    
    [TestFixture]
    public class IntegrationTest1
    {
            
        private ITimer _timer;
        private IDisplay _display;
        //private IUserInterface _ui;
        private IOutput _output;

        private PowerTube _powerTube;
        private CookController _sut;
        private double _percentage;

        [SetUp]
        public void SetUp()
        {
            _output = Substitute.For<IOutput>();

            _timer = new Timer();
            _display = new Display(_output);
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
        
        [TestCase(50, 60)]
        public void TestCookControllerAndDisplay(int power, int time)
        {
            //Act 
            _sut.StartCooking(power, time);

            //Assert for PowerTube
            _output.Received().OutputLine(Arg.Is<string>(str => str.Equals($"PowerTube works with {power}")));

            //Clear call
            _output.ClearReceivedCalls();

            //Act for Display
            Thread.Sleep(1000);

            //Assert for Display
            _output.Received().OutputLine(Arg.Is<string>(str => str.Equals($"Display shows: 00:59")));
        }

        [Test]
        public void StartCoockingAndWait1Second_DisplayHeyShowCorrectTime()
        {
            _sut.StartCooking(100, 1);
            Thread.Sleep(1000);

            _output.Received().OutputLine($"Display shows: 00:00");

            _output.ClearReceivedCalls();
        }
        [Test]
        public void TestCookControllerTimerExpire()
        {
            //Act 
            _sut.StartCooking(50, 1);
            Thread.Sleep(2000);


            _output.Received().OutputLine(Arg.Is<string>(a => a.Contains($"turned off")));
        }

        [TestCase(50, 1)]
        public void TestCookControllerToDisplay(int power, int time)
        {
            _sut.StartCooking(power, time);
            
            _output.Received().OutputLine(Arg.Is<string>(str => str.Equals($"PowerTube works with {power}")));
            _output.ClearReceivedCalls();

            Thread.Sleep(2000);

            _output.Received().OutputLine(Arg.Is<string>(str =>
                str.Equals($"PowerTube turned off")));
        }

        [TestCase(100,50)]
        public void StartCookingTwice(int power, int time)
        {
            _sut.StartCooking(power,time);
            Assert.Throws<ApplicationException>(() => _sut.StartCooking(power, time));
        }
    }
}
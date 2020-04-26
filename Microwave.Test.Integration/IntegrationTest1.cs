using System;
using MicrowaveOvenClasses.Boundary;
using NUnit.Framework;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;

namespace Microwave.Test.Integration
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

        [SetUp]
        public void SetUp()
        {
            _output = Substitute.For<IOutput>();
            _timer = Substitute.For<ITimer>();
            _display = Substitute.For<IDisplay>();
            _ui = Substitute.For<IUserInterface>();
            _powerTube = new PowerTube(_output);
            _sut = new CookController(_timer, _display, _powerTube, _ui);
        }

        [TestCase(1500, 15)]
        [TestCase(123, -10)]
        [TestCase(-10, 50)]
        public void StartCooking_PowerOutOfRange(int power, int time)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _sut.StartCooking(power, time));
        }

        [TestCase(50, 10)]
        [TestCase(2, 75)]
        [TestCase(15, 222)]
        public void StartCooking_PowerIsOn(int power, int time)
        {
            _sut.StartCooking(power, time);
            Assert.Throws<ApplicationException>(() => _sut.StartCooking(power, time));
        }
        
        [TestCase(10, 10)]
        [TestCase(20, 3)]
        [TestCase(45, 22)]
        public void StopCooking_OutputCorrect(int power, int time)
        {
            _sut.StartCooking(power, time);
            _sut.Stop();
            _output.Received().OutputLine(Arg.Is<string>(a => a.Equals($"PowerTube turned off")));
        }

        [Test]
        public void OnTimerExpired_Output()
        {
            _sut.StartCooking(50, 50);
            _timer.Expired += Raise.EventWith(this, EventArgs.Empty);
            _output.Received().OutputLine(Arg.Is<string>(a => a.Contains($"turned off"))); 
        } 
    }
}
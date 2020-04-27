using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Tests.Integration
{
    [TestFixture]
    class IT2_UI_Buttons
    {
        //Modules to test:
        private IButton _timeButton;
        private IButton _powerButton;
        private IButton _startCancelButton;
        private IUserInterface _userInterface;

        //Modules for stubs:
        private ILight _light;
        private IDoor _door;
        private IDisplay _display;
        private ITimer _timer;
        private ICookController _cookController;
        
        private IOutput _output;

        [SetUp]
        public void SetUp()
        {
            //Modules for stubs:
            _light = Substitute.For<ILight>();
            _display = Substitute.For<IDisplay>();
            _door = Substitute.For<IDoor>();
            _cookController = Substitute.For<ICookController>();

            //Modules to test:
            _timeButton = new Button();
            _powerButton = new Button();
            _startCancelButton = new Button();

            _userInterface = new UserInterface(
                _powerButton, _timeButton, _startCancelButton,
                _door, _display, _light, _cookController);
        }

        [Test]
        public void PressPowerButton_PressCancelbutton_DisplayClear()
        {
            //Act
            _powerButton.Press();
            _startCancelButton.Press();

            //Assert
            _display.Received().Clear();
        }

        //Each press increases the selected power level with 50 W, until 700, where it will return to 50 W on the next press.
        [TestCase(1, 50)]
        [TestCase(2, 100)]
        [TestCase(10, 500)]
        [TestCase(13, 650)]
        [TestCase(14, 700)] //Max power
        [TestCase(15, 50)]  //Reset to 50W
        [TestCase(16, 100)]
        public void PressPowerButton_ShowPowerCorrectPowerOnDisplay(int pressPowerButton, int expectedResult)
        {
            //Act
            for (int i = 0; i < pressPowerButton; i++)
            {
                _powerButton.Press();
            }
            //Assert
            _display.Received().ShowPower(expectedResult);
        }

        [TestCase(1,1)]
        [TestCase(2, 2)]
        [TestCase(59, 59)]
        [TestCase(60, 60)]  //Max time
        [TestCase(61, 1)]   //Reset       
        public void PressPowerButton_PressTimeButton_ShowPowerCorrectTimeOnDisplay(int pressTimeButton, int expectedMinutes)
        {
            //Act
            _powerButton.Press();
            for (int i = 0; i < pressTimeButton; i++)
            {
                _timeButton.Press();
            }
            //Assert
            _display.Received().ShowTime(expectedMinutes, 0);
        }

        [Test]
        public void PressPowerButton_PressTimeButton_PressStartButton_TurnOnLight_AssertClearScreen()
        {
            //Act
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            //Assert
            _light.Received().TurnOn();
        }

        [Test]
        public void PressPowerButton_PressTimeButton_PressStartButtonAndCancel_TurnOffLight()
        {
            //Act
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            //Assert
            _light.Received().TurnOff();
        }

        [TestCase(1,1)]
        [TestCase(3, 1)]
        [TestCase(1, 2)]
        [TestCase(5, 5)]
        public void StartCoocking_CheckCorrectTimeAndPower(int powerPressed, int timePressed)
        {
            //Act
            for (int i = 0; i < powerPressed; i++)
            {
                _powerButton.Press();
            }
            for (int i = 0; i < timePressed; i++)
            {
                _timeButton.Press();
            }

            _startCancelButton.Press();

            //Assert
            _cookController.Received().StartCooking(powerPressed*50, timePressed*60);
        }

        [Test]
        public void CancelCoocking()
        {
            //Act
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            _startCancelButton.Press();

            _cookController.Received().Stop();
        }
    }
}

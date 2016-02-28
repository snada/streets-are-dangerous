using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Devices.Sensors;


namespace StreetsAreDangerous
{
    public class InputManager
    {
        #region Dichiarazione Eventi
        /// <summary>
        /// Evento lanciato al verificarsi di un Tap.
        /// </summary>
        public event Action<Vector2> onTap;
        /// <summary>
        /// Evento lanciato al verificarsi di un DoubleTap.
        /// </summary>
        public event Action<Vector2> onDoubleTap;
        /// <summary>
        /// Evento lanciato al verificarsi di un Hold.
        /// </summary>
        public event Action<Vector2> onHold;
        /// <summary>
        /// Evento lanciato al verificarsi di un Flick.
        /// </summary>
        public event Action<Vector2> onFlick;
        /// <summary>
        /// Evento lanciato dopo un cambiamento dei dati dell'accelerometro.
        /// </summary>
        public event Action<Vector3> onAccelerometerChanged;
        #endregion

        //Indica se c'è bisogno della lettura dei dati dell'accelerometro
        private bool enableAccelerometer;
        //Dati per gestione accelerometro
        private Accelerometer accelerometer;
        public Vector3 accelerometerReading;

        /// <summary>
        /// Ritorna se l'inputmanager corrente analizza lo stato dell'accelerometro.
        /// </summary>
        public bool IsAccelerometerEnabled
        {
            get
            {
                return enableAccelerometer;
            }
        }

        /// <summary>
        /// Genera un nuovo InputManager.
        /// </summary>
        /// <param name="enableAccelerometer">Indica se attivare la lettura dei dati dell'accelerometro.</param>
        public InputManager(bool enableAccelerometer)
        {
            TouchPanel.EnabledGestures = GestureType.Tap | GestureType.Hold | GestureType.DoubleTap | GestureType.Flick;

            this.enableAccelerometer = enableAccelerometer;

            if (enableAccelerometer)
            {
                accelerometer = new Accelerometer();
                accelerometerReading = new Vector3();
                accelerometer.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(accelerometer_CurrentValueChanged);
                accelerometerReading = new Vector3();
                accelerometer.Start();
            }
        }

        public void Update()
        {
            GestureSample a = new GestureSample();
            while (TouchPanel.IsGestureAvailable)
            {
                a = TouchPanel.ReadGesture();
                switch (a.GestureType)
                {
                    case GestureType.Tap:
                        if (onTap != null)
                            onTap(a.Position);
                        break;

                    case GestureType.DoubleTap:
                        if (onDoubleTap != null)
                            onDoubleTap(a.Position);
                        break;

                    case GestureType.Hold:
                        if (onHold != null)
                            onHold(a.Position);
                        break;

                    case GestureType.Flick:
                        if (onFlick != null)
                            onFlick(a.Delta);
                        break;
                }
            }
        }

        private void accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            accelerometerReading.X = e.SensorReading.Acceleration.X;
            accelerometerReading.Y = e.SensorReading.Acceleration.Y;
            accelerometerReading.Z = e.SensorReading.Acceleration.Z;
            if (onAccelerometerChanged != null)
                onAccelerometerChanged(accelerometerReading);
        }
    }
}

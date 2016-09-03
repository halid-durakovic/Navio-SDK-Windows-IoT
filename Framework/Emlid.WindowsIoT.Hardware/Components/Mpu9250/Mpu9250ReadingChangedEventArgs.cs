using System;

namespace Emlid.WindowsIot.Hardware.Components.Mpu9250
{
    /// <summary>
    /// <see cref="Mpu9250Device.ReadingChanged" />.
    /// </summary>
    public class Mpu9250ReadingChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The timestamp of the current sensor reading in Utc format.
        /// </summary>>
        public DateTime Timestamp { get; protected set; }

        /// <summary>
        /// Gets the most recent accelerometer reading.
        /// </summary>>
        public Mpu9250SensorReading Reading { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mpu9250ReadingChangedEventArgs"/> class.
        /// </summary>
        /// <param name="reading">The sensor readings.</param>
        public Mpu9250ReadingChangedEventArgs(Mpu9250SensorReading reading)
        {
            Timestamp = DateTime.UtcNow;
            Reading = reading;
        }
    }
}

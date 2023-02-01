using Bruderer.Core.Domain.Models.ValueObjects;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Constants
{
    public static class UnitConstants
    {
        #region fields

        private static Dictionary<UnitsEnumeration, UnitValue> _UnitDictionary = new Dictionary<UnitsEnumeration, UnitValue>();

        #endregion
        #region ctor

        static UnitConstants()
        {
            _UnitDictionary.Add(UnitsEnumeration.Percent, Percent);

            _UnitDictionary.Add(UnitsEnumeration.AngleDegree, AngleDegree);
            _UnitDictionary.Add(UnitsEnumeration.AngleMinute, AngleMinute);
            _UnitDictionary.Add(UnitsEnumeration.AngleSecond, AngleSecond);

            _UnitDictionary.Add(UnitsEnumeration.Meter, Meter);
            _UnitDictionary.Add(UnitsEnumeration.Micrometer, Micrometer);
            _UnitDictionary.Add(UnitsEnumeration.Millimeter, Millimeter);
            _UnitDictionary.Add(UnitsEnumeration.Centimeter, Centimeter);

            _UnitDictionary.Add(UnitsEnumeration.TimeMicroSecond, TimeMicroSecond);
            _UnitDictionary.Add(UnitsEnumeration.TimeMilliSecond, TimeMilliSecond);
            _UnitDictionary.Add(UnitsEnumeration.TimeSecond, TimeSecond);
            _UnitDictionary.Add(UnitsEnumeration.TimeMinute, TimeMinute);
            _UnitDictionary.Add(UnitsEnumeration.TimeHour, TimeHour);
            _UnitDictionary.Add(UnitsEnumeration.TimeDay, TimeDay);
            _UnitDictionary.Add(UnitsEnumeration.LightYear, LightYear);

            _UnitDictionary.Add(UnitsEnumeration.DegreePerSecond, DegreePerSecond);
            _UnitDictionary.Add(UnitsEnumeration.Revolution, Revolution);
            _UnitDictionary.Add(UnitsEnumeration.RevolutionPerMinute, RevolutionPerMinute);
            _UnitDictionary.Add(UnitsEnumeration.RevolutionPerSecond, RevolutionPerSecond);
            _UnitDictionary.Add(UnitsEnumeration.DegreePerSecondSquared, DegreePerSecondSquared);
            _UnitDictionary.Add(UnitsEnumeration.MeterPerMinute, MeterPerMinute);
            _UnitDictionary.Add(UnitsEnumeration.MillimetrePerSecond, MillimetrePerSecond);
            _UnitDictionary.Add(UnitsEnumeration.MillimetrePerMinute, MillimetrePerMinute);
            _UnitDictionary.Add(UnitsEnumeration.MillimetrePerKiloNewton, MillimetrePerKiloNewton);
            _UnitDictionary.Add(UnitsEnumeration.MetrePerSecond, MetrePerSecond);
            _UnitDictionary.Add(UnitsEnumeration.Hertz, Hertz);
            _UnitDictionary.Add(UnitsEnumeration.Acceleration, Acceleration);
            _UnitDictionary.Add(UnitsEnumeration.Jerk, Jerk);
            _UnitDictionary.Add(UnitsEnumeration.Inertia, Inertia);
            _UnitDictionary.Add(UnitsEnumeration.Torque, Torque);

            _UnitDictionary.Add(UnitsEnumeration.Bar, Bar);
            _UnitDictionary.Add(UnitsEnumeration.Psi, Psi);

            _UnitDictionary.Add(UnitsEnumeration.DegreeCelsius, DegreeCelsius);
            _UnitDictionary.Add(UnitsEnumeration.DegreeFahrenheit, DegreeFahrenheit);

            _UnitDictionary.Add(UnitsEnumeration.PowerFactor, PowerFactor);
            _UnitDictionary.Add(UnitsEnumeration.Ampere, Ampere);
            _UnitDictionary.Add(UnitsEnumeration.Voltage, Voltage);
            _UnitDictionary.Add(UnitsEnumeration.Watthour, Watthour);
            _UnitDictionary.Add(UnitsEnumeration.KiloWatthour, KiloWatthour);
            _UnitDictionary.Add(UnitsEnumeration.Watt, Watt);
            _UnitDictionary.Add(UnitsEnumeration.KiloWatt, KiloWatt);
            _UnitDictionary.Add(UnitsEnumeration.KiloVoltAmpere, KiloVoltAmpere);

            _UnitDictionary.Add(UnitsEnumeration.Kilogram, Kilogram);
            _UnitDictionary.Add(UnitsEnumeration.Gram, Gram);

            _UnitDictionary.Add(UnitsEnumeration.Newton, Newton);
            _UnitDictionary.Add(UnitsEnumeration.NewtonPerSquareMillimetre, NewtonPerSquareMillimetre);
            _UnitDictionary.Add(UnitsEnumeration.KiloNewton, KiloNewton);

            _UnitDictionary.Add(UnitsEnumeration.KilogramPerCubicMeter, KilogramPerCubicMeter);
            _UnitDictionary.Add(UnitsEnumeration.KilogramPerCubicDeciMeter, KilogramPerCubicDeciMeter);
            _UnitDictionary.Add(UnitsEnumeration.GramPerCubicCentiMeter, GramPerCubicCentiMeter);
        }

        #endregion
        #region methods

        public static UnitValue GetUnitValue(UnitsEnumeration unit)
        {
            return _UnitDictionary[unit];
        }

        #endregion

        public static UnitValue Percent = new UnitValue("P1", 1301, "%", "percent");

        #region Angle

        public static UnitValue AngleDegree = new UnitValue("DD", 17476, "°", "degree [unit of angle]");

        public static UnitValue AngleMinute = new UnitValue("D61", 4470321, "'", "minute [unit of angle]");

        public static UnitValue AngleSecond = new UnitValue("D62", 4470322, "\"", "second [unit of angle]");

        #endregion

        #region Length

        public static UnitValue Meter = new UnitValue("MTR", 5067858, "m", "metre");

        public static UnitValue Micrometer = new UnitValue("4H", 13384, "µm", "micrometre (micron)");

        public static UnitValue Millimeter = new UnitValue("MMT", 5066068, "mm", "millimetre");

        public static UnitValue Centimeter = new UnitValue("CMT", 4410708, "cm", "centimetre");

        #endregion

        #region Time

        public static UnitValue TimeMicroSecond = new UnitValue("B98", 4340024, "µs", "microsecond");

        public static UnitValue TimeMilliSecond = new UnitValue("C26", 4403766, "ms", "millisecond");

        public static UnitValue TimeSecond = new UnitValue("SEC", 5457219, "s", "second [unit of time]");

        public static UnitValue TimeMinute = new UnitValue("MIN", 5065038, "min", "minute [unit of time]");

        public static UnitValue TimeHour = new UnitValue("HUR", 4740434, "h", "hour");

        public static UnitValue TimeDay = new UnitValue("DAY", 4473177, "d", "day");

        public static UnitValue LightYear = new UnitValue("B57", 4338999, "ly", "light year");

        #endregion

        #region Drives

        public static UnitValue DegreePerSecond = new UnitValue("E96", 4536630, "°/s", "degree per second");

        public static UnitValue Revolution = new UnitValue("M44", 5059636, "rev", "revolution");

        public static UnitValue RevolutionPerMinute = new UnitValue("RPM", 5394509, "r/min", "revolutions per minute");

        public static UnitValue RevolutionPerSecond = new UnitValue("RPS", 5394515, "r/s", "revolutions per second");

        public static UnitValue DegreePerSecondSquared = new UnitValue("M45", 5059637, "°/s²", "degree [unit of angle] per second squared");

        public static UnitValue MeterPerMinute = new UnitValue("2X", 5060144, "m/min", "meter per minute");

        public static UnitValue MillimetrePerSecond = new UnitValue("C16", 4403510, "mm/s", "millimetre per second");

        public static UnitValue MillimetrePerMinute = new UnitValue("H81", 4732977, "mm/min", "millimetre per minute");

        public static UnitValue MillimetrePerKiloNewton = new UnitValue("MKN", 0000000, "mm/kN", "millimetre per kilonewton");

        public static UnitValue MetrePerSecond = new UnitValue("MTS", 5067859, "m/s", "metre per second");

        public static UnitValue Hertz = new UnitValue("HTZ", 4740186, "Hz", "hertz");

        public static UnitValue Acceleration = new UnitValue("ACC", 0000000, "m/s2", "acceleration");

        public static UnitValue Jerk = new UnitValue("JRK", 0000000, "m/s3", "jerk");

        public static UnitValue Inertia = new UnitValue("IRA", 0000000, "kg m²", "inertia");

        public static UnitValue Torque = new UnitValue("NU", 20053, "Nm", "newton metre");

        #endregion

        #region Pressure

        public static UnitValue Bar = new UnitValue("BAR", 4342098, "bar", "bar [unit of pressure]");

        public static UnitValue Psi = new UnitValue("PS", 20563, "lbf/in²", "pound-force per square inch");

        #endregion

        #region Temperature

        public static UnitValue DegreeCelsius = new UnitValue("CEL", 4408652, "°C", "degree Celsius");

        public static UnitValue DegreeFahrenheit = new UnitValue("FAH", 4604232, "°F", "degree Fahrenheit");

        #endregion

        #region Eletrical

        public static UnitValue PowerFactor = new UnitValue("", 0, "cos(phi)", "Power Factor Phi");

        public static UnitValue Ampere = new UnitValue("AMP", 4279632, "A", "ampere");

        public static UnitValue Voltage = new UnitValue("VLT", 5655636, "V", "volt");

        public static UnitValue Watthour = new UnitValue("WHR", 5720146, "W·h", "watt hour");

        public static UnitValue KiloWatthour = new UnitValue("KWH", 4937544, "kW·h", "kilowatt hour");

        public static UnitValue Watt = new UnitValue("WTT", 5723220, "W", "watt");

        public static UnitValue KiloWatt = new UnitValue("KWT", 4937556, "kW", "kilowatt");

        public static UnitValue KiloVoltAmpere = new UnitValue("KVA", 4937281, "kV·A", "kilovolt - ampere");

        #endregion

        #region Weight

        public static UnitValue Kilogram = new UnitValue("KGM", 4933453, "kg", "kilogram");

        public static UnitValue Gram = new UnitValue("GRM", 4674125, "g", "gram");

        #endregion

        #region Force

        public static UnitValue Newton = new UnitValue("NEW", 5129559, "N", "newton");

        public static UnitValue KiloNewton = new UnitValue("B47", 4338743, "kN", "kilonewton");

        #endregion

        #region Density

        public static UnitValue KilogramPerCubicMeter = new UnitValue("KMQ", 4934993, "kg/m³", "kilogram per cubic metre");

        public static UnitValue KilogramPerCubicDeciMeter = new UnitValue("B34", 4338484, "kg/dm³", "kilogram per cubic decimetre");

        public static UnitValue GramPerCubicCentiMeter = new UnitValue("23", 12851, "g/cm³", "kilogram per cubic centimetre");

        #endregion

        #region Material

        public static UnitValue NewtonPerSquareMillimetre = new UnitValue("C56", 4404534, "N/mm²", "newton per square millimetre");

        #endregion
    }
}


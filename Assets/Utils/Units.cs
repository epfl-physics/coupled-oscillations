// Class for astronomical units
public static class Units
{
    // Constants
    public static float newtonG_SI = 0.000000000066743f; // m^3 / kg / s^2
    public static float au_SI = 149597870700f;           // m
    public static float earth_to_moon_SI = 384399000f;   // m
    public static float r_sun_SI = 696340000f;           // m
    public static double m_sun_SI = 1.98847e30;          // kg
    public static float r_earth_SI = 6371000f;           // m
    public static double m_earth_SI = 5.9722e24;         // kg
    public static float r_moon_SI = 1737400f;            // m
    public static double m_moon_SI = 7.342e22;           // kg
    public static float year_SI = 31556952f;             // s
    public static float month_SI = year_SI / 12f;        // s
    public static float day_SI = 86400f;                 // s

    // Unit options
    public enum UnitTime { Year, Month, Day }
    public enum UnitLength { AU, SolarRadius, EarthRadius }
    public enum UnitMass { SolarMass, EarthMass }

    public static float NewtonG(UnitTime unitTime, UnitLength unitLength, UnitMass unitMass)
    {
        // Time
        float t = year_SI;
        if (unitTime == UnitTime.Month)
        {
            t = month_SI;
        }
        else if (unitTime == UnitTime.Day)
        {
            t = day_SI;
        }

        // Length
        float l = au_SI;
        if (unitLength == UnitLength.SolarRadius)
        {
            l = r_sun_SI;
        }
        else if (unitLength == UnitLength.EarthRadius)
        {
            l = r_earth_SI;
        }

        // Mass
        double m = m_sun_SI;
        if (unitMass == UnitMass.EarthMass)
        {
            m = m_earth_SI;
        }

        return (float)(newtonG_SI * m * t * t / l / l / l);
    }

    public static float EarthRotationPeriod(UnitTime unitTime)
    {
        float result = 0;
        switch (unitTime)
        {
            case UnitTime.Day:
                result = 1;
                break;
            case UnitTime.Month:
                result = day_SI / month_SI;
                break;
            case UnitTime.Year:
                result = day_SI / year_SI;
                break;
            default:
                break;
        }
        return result;
    }

    public static float EarthRadius(UnitLength unitLength)
    {
        float result = 0;
        switch (unitLength)
        {
            case UnitLength.AU:
                result = r_earth_SI / au_SI;
                break;
            case UnitLength.EarthRadius:
                result = 1;
                break;
            case UnitLength.SolarRadius:
                result = r_earth_SI / r_sun_SI;
                break;
            default:
                break;
        }
        return result;
    }

    public static float EarthMass(UnitMass unitMass)
    {
        float result = 1;
        if (unitMass == UnitMass.SolarMass)
        {
            result = (float)(m_earth_SI / m_sun_SI);
        }
        return result;
    }

    public static float LunarRadius(UnitLength unitLength)
    {
        float result = 0;
        switch (unitLength)
        {
            case UnitLength.AU:
                result = r_moon_SI / au_SI;
                break;
            case UnitLength.EarthRadius:
                result = r_moon_SI / r_earth_SI;
                break;
            case UnitLength.SolarRadius:
                result = r_moon_SI / r_sun_SI;
                break;
            default:
                break;
        }
        return result;
    }

    public static float LunarMass(UnitMass unitMass)
    {
        float result = 0;
        switch (unitMass)
        {
            case UnitMass.EarthMass:
                result = (float)(m_moon_SI / m_earth_SI);
                break;
            case UnitMass.SolarMass:
                result = (float)(m_moon_SI / m_sun_SI);
                break;
            default:
                break;
        }
        return result;
    }

    public static float LunarDistance(UnitLength unitLength)
    {
        float result = 0;
        switch (unitLength)
        {
            case UnitLength.AU:
                result = earth_to_moon_SI / au_SI;
                break;
            case UnitLength.EarthRadius:
                result = earth_to_moon_SI / r_earth_SI;
                break;
            case UnitLength.SolarRadius:
                result = earth_to_moon_SI / r_sun_SI;
                break;
            default:
                break;
        }
        return result;
    }
}

# Almanac Part 2: Digging into the rabit hole

## How to compute Julian day (JD)?
Use this formula:

$$
JD = 367 * Y - ⌊(7 * (Y + ⌊(M + 9) / 12⌋)) / 4⌋ + ⌊(275 * M) / 9⌋ + D + 1721013.5 + (UT / 24)
$$
Where:
- Y = year (e.g. 2025)
- M = month (e.g. 4)
- D = day (e.g. 10)
- UT = time in hours (e.g. 18.5 for 6:30 PM UTC)

## How to compute Julian centuries?
The epoch J2000.0 (January 1, 2000, 12:00 UTC) is the modern standard astronomical reference time for celestial coordinates and time calculations.
Astronomical formulas for precession, nutation, sidereal time, and other time-dependent phenomena are often expressed as polynomials in Julian centuries since J2000.0.
Using Julian centuries as the time variable simplifies calculations and standardizes measurements across different epochs.
$$
T = (JD - 2451545.0) / 36525
$$
where 2451545.0 is the Julian Date of J2000.0

## How to compute Greenwich sidereal time?
To calculate the Greenwich Mean Sidereal Time (GMST) for a given date and Universal Time (UT), you can use the following widely accepted formula based on the number of days since the J2000.0 epoch (January 1, 2000, 12:00 UT):
1. Calculate the Julian Date (JD) for the given UT date and time
2. Compute the number of days since J2000.0
3. Calculate GST in degrees
4. Convert GMT from degrees to hours

$Formula^1$ to calculate GST in degrees is -
$$
GST = 280.46061837 + 360.98564736629 * (JD - 2451545.0) 
      + 0.000387933 * T^2 - T^3 / 38710000
$$

## How to compute local sidereal time?
There are two types:
- Greenwich Sidereal Time (GST) → sidereal time at 0° longitude
- Local Sidereal Time (LST) → sidereal time at your longitude

$$
LST = GST + longitude / 15.0
$$
Because 360° = 24 hours. You divide by 15 when converting longitude from degrees to hours because 15 degrees of Earth's rotation correspond exactly to 1 hour of sidereal time. This is due to the fact that Earth completes a full 360° rotation in about 24 hours.
Thus, to convert an angle in degrees to the equivalent time in hours, you divide by 15. This is why in the local sidereal time formula, longitude in degrees is divided by 15 to express it in hours before adding it to Greenwich Sidereal Time (GST).

Also,
- 🌍 East longitudes → positive
- 🌍 West longitudes → negative

## How to calculate θ (theta)? (sidereal rotation angle)
To compute θ (sidereal rotation angle):
1. Convert your date/time to Julian Day
2. Calculate Julian centuries since J2000.0
3. Use astronomical formula to get Greenwich Sidereal Time
4. Adjust for your longitude
5. Convert result from seconds to radians

## Example - compute local sidereal time for Bangalore
Here are the steps again, which we will go through in detail -
1. Compute Julian day (JD)
2. Compute Julian Centuries since J2000.0
3. Compute Greenwich Sidereal Time (GST) in degrees
4. Convert to Local Sidereal Time (LST)

Let's break the individual steps and go through each one in detail -
Co-ordinates of Bangalore are (12.971599, 77.594566)
1. Date & time: 2025-04-10 18:00 UTC
2. Julian Day from the formula mentioned above:
$$
JD = 2460465.25
$$
3. Julian centuries from the formula mentioned above:
$$
T = (2460465.25 - 2451545.0) / 36525 = ~0.2445
$$
4. Greenwich sidereal time in degrees from the formula mentioned above:
$$
GST ≈ 280.46061837 + 360.98564736629 * (JD - 2451545.0)
    ≈ 280.46061837 + 360.98564736629 * 8920.25
    ≈ 322.176° (after modulus 360)
$$
5. Local sidereal time from the formula mentioned above:
$$
LST = GST + 77.5946 = 322.176 + 77.5946 = 399.77
$$
399.77 → wrap to 39.77°
6. LST in hours -
39.77° / 15 = 2.65 sidereal hours
So the Local Sidereal Time in Bangalore at 6 PM UTC on April 10, 2025 is about 2:39 AM sidereal time.

## Refernces -
1. IAU model 1982 to calculate GST
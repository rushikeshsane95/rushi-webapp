# Almanac Part 1: Hows? Whys? and Whats?

Panchang is a traditional Hindu calendar that provides information about the lunar phases, auspicious timings, and various festivals. 
It is widely used in India for determining the best times for rituals, ceremonies, and other important events. But how many of us really understand the intricacies of Panchang?
It's not just about dates and times; it's a blend of astronomy, mathematics, and planetary significance. In fact, it is a scientific approach to timekeeping that has been passed down through generations.
In this blog post, I will delve into the fascinating world of Panchang, exploring its components, significance, and how it can enhance our understanding of time and its impact on our lives.
I will try to break down this complex subject into simple terms, making it accessible to everyone. Whether you're a seasoned practitioner or a curious beginner, 
this post will provide valuable insights into the world of Panchang and its relevance in our daily lives.

## Few concepts good to know

### Tithi
Tithi is a lunar day in the Hindu calendar, which is based on the phases of the moon.
It is the time taken for the moon to move approximately 12 degrees away from the sun.


### Nakshatra
Nakshatra is a lunar mansion or constellation in the Hindu calendar.
There are 27 Nakshatras, each representing a specific star or group of stars.
Each Nakshatra is divided into 4 quarters (padas), and each quarter is associated with a specific planet.
The Nakshatras are used to determine the auspiciousness of a particular time for rituals and ceremonies.

### Yoga
Yoga is a combination of the positions of the sun and moon.
It is calculated based on the angular distance between the sun and moon.
There are 27 Yogas, and each Yoga is associated with a specific planetary influence.
The Yogas are used to determine the auspiciousness of a particular time for rituals and ceremonies.

### Karana
Karana is half of a Tithi.
There are 11 Karanas in total, and each Karana is associated with a specific planetary influence.
The Karanas are used to determine the auspiciousness of a particular time for rituals and ceremonies.

### Vaar
Vaar is the day of the week in the Hindu calendar.
There are 7 Vaaras, each associated with a specific planet.
The Vaaras are used to determine the auspiciousness of a particular time for rituals and ceremonies.

### Paksha
Paksha is the lunar fortnight in the Hindu calendar. 
There are 2 Pakshas in a lunar month:
1. Shukla Paksha (waxing phase) - from the new moon to the full moon
2. Krishna Paksha (waning phase) - from the full moon to the new moon

## Some more advanced concepts

### Julian day
The Julian Day is a continuous count of days since noon UTC on January 1, 4713 BCE (that’s more than 6,000 years ago!). It's used by astronomers and scientists to:  
- Avoid dealing with calendar systems (like leap years, months, etc.)
- Easily calculate time intervals (just subtract JD numbers)

Let’s say you're comparing:
March 1, 2025 and April 10, 2025
In calendar terms, you’d have to handle:
- Months with different lengths
- Leap years
- Time zones

With Julian Day, both dates are just big floating-point numbers. Example:
- March 1, 2025, 12:00 UTC → JD 2460425.0
- April 10, 2025, 12:00 UTC → JD 2460465.0

Julian Days include decimals:
- JD 2460465.0 → means noon UTC
- JD 2460465.25 → 6 PM UTC
- JD 2460465.5 → midnight next day UTC

### Julian century
Julian centuries are units of time used in astronomy to measure intervals relative to a standard epoch, specifically the epoch J2000.0, which corresponds to January 1, 2000, at 12:00 Terrestrial Time (TT). One Julian century is defined as exactly 36,525 days (100 Julian years of 365.25 days each).

What is a Julian century?
It is a time interval of 100 Julian years.
A Julian year is exactly 365.25 days of 86,400 seconds each, a constant unit of duration used in astronomy to avoid variations inherent in tropical or calendar years. Therefore, one Julian century = 100 × 365.25 days = 36,525 days.

### Sidereal time
Sidereal Time is a way to keep time based on the rotation of the Earth relative to the stars — not the Sun.
A sidereal day is the time it takes Earth to rotate once relative to the distant stars — about 23 hours, 56 minutes, and 4 seconds.
In contrast, a solar day (what your clock shows) is about 24 hours, because the Earth has moved a bit in its orbit around the Sun each day.

Because the stars stay fixed in the sky over millions of years (relatively speaking), sidereal time helps predict where a star (or the Sun, or Moon) will be in the sky at a given location and time.

It's like this:
- "If a star is directly overhead at 9:00 PM sidereal time today, it will be overhead at 9:00 PM sidereal time tomorrow too."
- But clock time won't show that, because Earth has to spin a little more each day to "catch up" with the Sun.

### Sidereal rotation angle θ (theta)
Even though sidereal time is star-based, it helps track all celestial objects, including the Sun.
Sidereal time is used to compute θ (theta), which is the angle between your location and the celestial object (like the Sun) on the celestial sphere.
Knowing theta and your latitude, we calculate the altitude (height above the horizon) of the Sun. From that, it can tell:
- When the Sun rises (altitude goes from negative to positive)
- When the Sun sets (altitude goes from positive to negative)

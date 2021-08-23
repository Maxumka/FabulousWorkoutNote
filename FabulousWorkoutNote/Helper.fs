namespace FabulousWorkoutNote

open System

module Helper = 
    let setTime (date : DateTime) = 
        DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0)



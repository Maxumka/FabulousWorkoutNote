namespace FabulousWorkoutNote

open System

type Set = 
    {Id : int
     Weight : double 
     Reps : int
     ExerciseId : int}

type Exercise = 
    {Id : int
     Name : string
     Sets : Set list
     Workout : Workout}

and Workout = 
    {Id : int
     Date : DateTime; 
     Exercises : Exercise list}

type Result<'a> = 
    | Ok of 'a
    | Fail of string

type ExtMsg = 
    | NoOp 
    | NavExercisePage of Exercise
    | NavWorkoutPageAfterDelete of int
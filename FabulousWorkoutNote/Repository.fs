namespace FabulousWorkoutNote

open Model
open Entity
open Context 
open System
open System.Linq
open Microsoft.EntityFrameworkCore

module Repository = 
    let private initContext dbPath = 
        let context = new AppDbContext(dbPath)
        SQLitePCL.Batteries_V2.Init()
        context.Database.EnsureCreated() |> ignore
        context

    let private getSetsByExerciseId (ctx : AppDbContext) exerciseId = 
        ctx.Sets.ToList()
                .Where(fun s -> s.ExerciseId = exerciseId)
                .ToList()

    let private getExerciseByWorkoutId (ctx : AppDbContext) workoutId = 
        ctx.Exercises.ToList()
                     .Where(fun e -> e.WorkoutId = workoutId)
                     .Select(fun e -> e.Sets <- getSetsByExerciseId ctx e.Id; e)
                     .ToList()

    let getWorkoutByDate dbPath (date : DateTime) = 
        let ctx = initContext dbPath
        let workoutEntity = ctx.Workouts
                               .ToList()
                               .FirstOrDefault(fun e -> e.Date.Date = date.Date)  
        match workoutEntity with 
        | null -> 
            {id = 0; date = date; exercises = []}
        | _ -> 
            workoutEntity.Exercises 
                <- getExerciseByWorkoutId ctx workoutEntity.Id
            Workout.FromEntity workoutEntity

    let addOrUpdate dbPath (workout : Workout) = 
        let ctx = initContext dbPath
        let workoutEntity = 
            ctx.Workouts.ToList().FirstOrDefault(fun w -> w.Date.Date = workout.date.Date)
        match workoutEntity with
        | null -> 
            ctx.Add(workout.ToEntity()) |> ignore          
        | _ -> 
            ctx.Remove(workoutEntity) |> ignore
            ctx.SaveChanges() |> ignore
            ctx.Add(workout.ToEntity()) |> ignore
        ctx.SaveChanges() |> ignore
namespace FabulousWorkoutNote

open System
open Entity

module Model = 
    type Set = 
        { id : int;
          weight : int; 
          reps : int }
          member this.ToEnity() = 
            let entity = SetEntity()
            entity.Id <- this.id
            entity.Weight <- this.weight
            entity.Reps <- this.reps
            entity
          static member FromEntity(entity : SetEntity) = 
            {id = entity.Id; weight = entity.Weight; reps = entity.Reps}

    type Exercise = 
        { id : int;
          name : string;
          sets : Set list }
          member this.ToEntity() = 
            let entity = ExerciseEntity()
            let sets = ResizeArray(this.sets |> List.map (fun s -> s.ToEnity()))
            entity.Id <- this.id
            entity.Name <- this.name
            entity.Sets <- sets
            entity
          static member FromEntity(entity : ExerciseEntity) = 
            let sets = entity.Sets |> Seq.toList |> List.map Set.FromEntity
            {id = entity.Id;name = entity.Name; sets = sets }

    type Workout = 
        { id : int;
          date : DateTime; 
          exercises : Exercise list }
          member this.ToEntity() = 
            let entity = WorkoutEntity()
            let exercises = ResizeArray(this.exercises |> List.map(fun e -> e.ToEntity()))
            entity.Id <- this.id
            entity.Date <- this.date
            entity.Exercises <- exercises
            entity
          static member FromEntity(entity : WorkoutEntity) = 
            let exercises = entity.Exercises |> Seq.toList |> List.map Exercise.FromEntity
            {id = entity.Id;date = entity.Date; exercises = exercises}


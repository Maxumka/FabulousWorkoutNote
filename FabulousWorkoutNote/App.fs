// Copyright Fabulous contributors. See LICENSE.md for license.
namespace FabulousWorkoutNote

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open System

module App = 
    type Model = 
        {WorkoutModel : WorkoutPage.Model
         ExerciseModel : ExercisePage.Model option}

    let init date () = 
        {WorkoutModel = WorkoutPage.init date
         ExerciseModel = None}, Cmd.none

    type Msg = 
        | WorkoutMsg of WorkoutPage.Msg
        | ExerciseMsg of ExercisePage.Msg

    let updateExtMsg extMsg (model : Model) = 
        match extMsg with 
        | NavExercisePage e -> 
            let ep = ExercisePage.init e
            {model with ExerciseModel = Some ep}
        | NavWorkoutPageAfterDelete id -> 
            let es = model.WorkoutModel.Workout.Exercises |> List.filter (fun e -> e.Id <> id)
            let w = {model.WorkoutModel.Workout with Exercises = es}
            let wm = {model.WorkoutModel with Workout = w}
            {model with WorkoutModel = wm; ExerciseModel = None}
        | _ -> model

    let update msg (model : Model) = 
        match msg with 
        | WorkoutMsg msg -> 
            let (workoutModel, cmd, extMsg) = WorkoutPage.update msg model.WorkoutModel
            let model = {model with WorkoutModel = workoutModel; ExerciseModel = None}
            updateExtMsg extMsg model, Cmd.map WorkoutMsg cmd
        | ExerciseMsg msg -> 
            match model.ExerciseModel with 
            | Some m -> 
                let (exerciseModel, cmd, extMsg) = ExercisePage.update msg m
                let exercises = 
                    model.WorkoutModel.Workout.Exercises
                    |> List.map (fun e -> if e.Id = exerciseModel.exercise.Id then exerciseModel.exercise else e)
                let workoutModel = 
                    {model.WorkoutModel with Workout = {model.WorkoutModel.Workout with Exercises = exercises}}
                let model = {model with ExerciseModel = Some exerciseModel; WorkoutModel = workoutModel}
                updateExtMsg extMsg model, Cmd.map ExerciseMsg cmd
            | None -> 
                model, Cmd.none

    let view (model : Model) dispatch =  
        let exercisePage = 
            model.ExerciseModel |> Option.map (fun e -> ExercisePage.view e (ExerciseMsg >> dispatch))
    
        let workoutPage = 
            WorkoutPage.view model.WorkoutModel (WorkoutMsg >> dispatch)

        View.NavigationPage(pages = [
            yield workoutPage
            match exercisePage with 
            | Some e -> yield e 
            | None -> ()
        ])

type App () as app = 
    inherit Application ()

    let runner = 
        Program.mkProgram (App.init DateTime.Now) App.update App.view
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app
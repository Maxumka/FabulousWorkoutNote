// Copyright Fabulous contributors. See LICENSE.md for license.
namespace FabulousWorkoutNote

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open System
open System.Collections.Generic
open Model 
open CustomControl
open Fabulous.XamarinForms.ViewExtensions

module App = 
    type Model = 
        { workoutId : int;
          selectedDate : DateTime;
          exercisePages : List<ExercisePage.Model>;
          selectedIndexExercise : int option }

    let init dbPath () = 
        let workout = Repository.getWorkoutByDate dbPath DateTime.Now
        let exercisePages = workout.exercises |> List.map ExercisePage.init |> ResizeArray
        { workoutId = workout.id;
          selectedDate = DateTime.Now;
          exercisePages = exercisePages;
          selectedIndexExercise = None }, Cmd.none

    type Msg = 
        | SelectDate of DateTime 
        | SelectExercise of int 
        | AddExercise 
        | UpdateExercise of ExercisePage.Msg
        | Save

    let updateSelectDate dbPath date (model : Model) = 
        let workout = Repository.getWorkoutByDate dbPath date
        let exercisePages = workout.exercises |> List.map ExercisePage.init |> ResizeArray
        {model with workoutId = workout.id;
                    selectedDate = date; 
                    exercisePages = exercisePages}

    let updateAddExercise (model : Model) = 
        let exercise = {id = 0; name = "exercise"; sets = []}
        let exercisePage = ExercisePage.init exercise
        model.exercisePages.Add(exercisePage)
        model

    let updateExercise msg (model : Model) = 
        let updateExercise index = 
            match msg with 
            | ExercisePage.DeleteExercise -> 
                model.exercisePages.RemoveAt(index)
                {model with selectedIndexExercise = None}
            | _ -> 
                model.exercisePages.[index] 
                    <- ExercisePage.update msg model.exercisePages.[index]
                model
        match model.selectedIndexExercise with 
        | Some i -> updateExercise i 
        | None -> model

    let updateSave dbPath (model : Model) = 
        let exercises = model.exercisePages 
                        |> Seq.map (fun e -> e.exercise) 
                        |> Seq.toList
        let workout = {id = model.workoutId; date = model.selectedDate; exercises = exercises}
        Repository.addOrUpdate dbPath workout 

    let updateRemoveExercise (model : Model) = 
        match model.selectedIndexExercise with 
        | None -> model 
        | Some index -> 
            model.exercisePages.RemoveAt(index)
            {model with selectedIndexExercise = None}

    let update dbPath msg model =
        match msg with 
        | SelectDate date -> 
            let newModel = updateSelectDate dbPath date model
            {newModel with selectedIndexExercise = None}, Cmd.none
        | SelectExercise index -> 
            {model with selectedIndexExercise = Some index}, Cmd.none
        | AddExercise ->
            let newModel = updateAddExercise model
            {newModel with selectedIndexExercise = None}, Cmd.none
        | UpdateExercise msg -> 
            updateExercise msg model, Cmd.none
        | Save -> 
            updateSave dbPath model
            {model with selectedIndexExercise = None}, Cmd.none

    let viewWorkout (model : Model) dispatch = 
        let day = model.selectedDate.Day
        let month = model.selectedDate.ToString("MMMM")
        let dayOfWeek = model.selectedDate.DayOfWeek

        // Actions
        let selectDate = fun (d : DateChangedEventArgs) -> d.NewDate |> SelectDate |> dispatch
        let addExercise = fun () -> dispatch AddExercise
        let selectExercise = fun (i : int) -> i |> SelectExercise |> dispatch
        let saveWorkout = fun () -> dispatch Save

        View.ContentPage(
            backgroundColor = Color.FromHex("#ececec"),
            title = $"{dayOfWeek}, {month} {day}",
            toolbarItems = [
                View.ToolbarItem(
                    text = "Save", 
                    command = saveWorkout
                )
            ],
            content = View.Grid(
                rowdefs = [Dimension.Stars 0.6; Dimension.Stars 2.],
                children = [
                    Component.frameSelectWorkoutDate [
                        View.Label(
                            margin = Thickness(10., 0., 10., 0.), 
                            text = "Select workout date: ", 
                            fontSize = FontSize.Size 22.
                        )
                        View.DatePicker(
                            margin = Thickness(10., 0., 10., 0.), 
                            fontSize = FontSize.Size 20., 
                            format = "dd-MM-yyyy",
                            dateSelected = selectDate
                        )
                    ]
                    (Component.gridExercises [
                        View.ListView(separatorVisibility = SeparatorVisibility.None,                                        
                                      rowHeight = 120, 
                                      selectionMode = ListViewSelectionMode.None,
                                      itemTapped = selectExercise,
                                      items = [
                                        for exercise in model.exercisePages do 
                                            Component.viewCellExercise exercise.exercise
                                      ]).RowSpan(2).ColumnSpan(2)
                        Component.buttonAddExercise addExercise
                    ]).Row(1)
                ]
            )
        )

    let view (model : Model) dispatch =        
        let exercisePage = 
            model.selectedIndexExercise
            |> Option.map(fun i -> ExercisePage.view model.exercisePages.[i] (UpdateExercise >> dispatch))

        View.NavigationPage(pages = [
            yield viewWorkout model dispatch
            match exercisePage with 
            | Some e -> yield e 
            | None -> ()
        ])

type App (dbPath) as app = 
    inherit Application ()

    let init = App.init dbPath
    let update = App.update dbPath 

    let runner = 
        Program.mkProgram init update App.view
        |> Program.withConsoleTrace
        |> XamarinFormsProgram.run app
namespace FabulousWorkoutNote

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open System

module WorkoutPage = 
    type Model = 
        { Workout : Workout
          selectedIndexExercise : int option }

    let init date = 
        let workout = HttpService.getWorkoutByDate date
        let exercises = workout.Exercises |> List.sortBy (fun e -> e.Id)
        let workout' = {workout with Exercises = exercises}
        { Workout = workout'
          selectedIndexExercise = None }

    type Msg = 
        | SelectDate of DateTime 
        | SelectExercise of int 
        | AddExercise 
        | OkAddExercise of Exercise

    let addExerciseAsync (model : Model) = 
        async {
            let exercise = {Id = 0; Name = "new exercise"; Sets = []; Workout = model.Workout}
            let! exercise' = HttpService.addExercise exercise
            return exercise' |> OkAddExercise } |> Cmd.ofAsyncMsg

    let update msg model =
        match msg with 
        | AddExercise -> 
            model, addExerciseAsync model, ExtMsg.NoOp
        | OkAddExercise exercise -> 
            let es = model.Workout.Exercises @ [exercise]
            let w = {model.Workout with Exercises = es}
            {model with Workout = w}, Cmd.none, ExtMsg.NoOp
        | SelectExercise index -> 
            model, Cmd.none, ExtMsg.NavExercisePage model.Workout.Exercises.[index]
        | SelectDate date -> 
            init date, Cmd.none, ExtMsg.NoOp
            

    let viewWorkout (model : Model) dispatch = 
        let day = model.Workout.Date.Day
        let month = model.Workout.Date.ToString("MMMM")
        let dayOfWeek = model.Workout.Date.DayOfWeek

        // Actions
        let selectDate = fun (d : DateChangedEventArgs) -> d.NewDate |> SelectDate |> dispatch
        let addExercise = fun () -> dispatch AddExercise
        let selectExercise = fun (i : int) -> i |> SelectExercise |> dispatch

        View.ContentPage(
            backgroundColor = Color.FromHex("#ececec"),
            title = $"{dayOfWeek}, {month} {day}",
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
                                        for exercise in model.Workout.Exercises do 
                                            Component.viewCellExercise exercise
                                      ]).RowSpan(2).ColumnSpan(2)
                        Component.buttonAddExercise addExercise
                    ]).Row(1)
                ]
            )
        )

    let view (model : Model) dispatch =  
        viewWorkout model dispatch


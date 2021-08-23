namespace FabulousWorkoutNote

open Model 
open CustomControl
open Xamarin.Forms
open Fabulous.XamarinForms

module ExercisePage = 
    type Model = 
        { exercise : Exercise;
          newSet : Set;
          selectedIndexSet : int option}

    let init exercise =
        { exercise = exercise;
          newSet = {id = 0; weight = 0; reps = 0};
          selectedIndexSet = None}

    type Msg = 
        | UpdateName of string
        | UpdateWeight of string
        | UpdateReps of string
        | AddNewSet 
        | ClearNewSet
        | DeleteExercise
        | SelectSet of int option
        | UpdateSet
        | DeleteSet

    let updateAddSet (sets : Set list) (set : Set) = 
        match set.reps with 
        | 0 -> sets 
        | _ -> sets @ [set]

    let updateSet (model : Model) = 
        match model.selectedIndexSet with 
        | None -> 
            model
        | Some index -> 
            let sets =
                model.exercise.sets 
                |> List.mapi(fun i v -> if index = i then model.newSet else v)
            {model with exercise = {model.exercise with sets = sets}}

    let deleteSet (model : Model) = 
        match model.selectedIndexSet with 
        | None -> model 
        | Some index -> 
            let sets = model.exercise.sets |> ResizeArray
            sets.RemoveAt(index) 
            {model with exercise = {model.exercise with sets = sets |> Seq.toList}}

    let updateSelectSet (model : Model) = 
        match model.selectedIndexSet with 
        | Some index -> 
            {model with newSet = model.exercise.sets.[index]}
        | None -> model

    let update msg model =
        match msg with
        | UpdateName name -> 
            {model with exercise = {model.exercise with name = name}}
        | UpdateWeight weight -> 
            {model with newSet = {model.newSet with weight = weight |> int}}
        | UpdateReps reps -> 
            {model with newSet = {model.newSet with reps = reps |> int}}
        | AddNewSet -> 
            {model with exercise = {model.exercise with sets = updateAddSet model.exercise.sets model.newSet}}
        | ClearNewSet -> {model with newSet = {id = 0; weight = 0; reps = 0}}
        | SelectSet index -> 
            let newModel = {model with selectedIndexSet = index}
            updateSelectSet newModel
        | UpdateSet -> 
            let newModel = updateSet model 
            {newModel with selectedIndexSet = None; 
                           newSet = {id = 0; weight = 0; reps = 0}}
        | DeleteSet -> 
            let newModel = deleteSet model 
            {newModel with selectedIndexSet = None
                           newSet = {id = 0; weight = 0; reps = 0}}
        | _ -> model

    let view (model : Model) dispatch = 
        // Actions
        let updateName = (fun (e: TextChangedEventArgs) -> e.NewTextValue |> UpdateName |> dispatch)
        let updateWeight = (fun (e: TextChangedEventArgs) -> e.NewTextValue |> UpdateWeight |> dispatch)
        let updateReps = (fun (e: TextChangedEventArgs) -> e.NewTextValue |> UpdateReps |> dispatch)
        let addNewSet = fun () -> dispatch AddNewSet
        let deleteExercise = fun () -> dispatch DeleteExercise
        let selectSet = (fun (i : int option) -> i |> SelectSet |> dispatch)
        let updateSet = fun () -> dispatch UpdateSet
        let deleteSet = fun () -> dispatch DeleteSet

        View.ContentPage(
            backgroundColor = Color.FromHex("#ececec"),
            toolbarItems = [
                View.ToolbarItem(
                    text = "Delete",
                    command = deleteExercise
                )
            ],
            content = View.Grid(
                rowdefs = [Dimension.Stars 0.5; Dimension.Stars 1.; Dimension.Stars 2.],
                children = [
                    View.Frame(
                        margin = Thickness(10., 10., 10., 0.),
                        backgroundColor = Color.FromHex("#e1f5fe"),
                        hasShadow = true,
                        cornerRadius = 10.,
                        content = View.StackLayout(
                            children = [
                                View.Entry(
                                    text = model.exercise.name,
                                    textChanged = updateName
                                )
                            ]
                        )
                    ).Row(0)
                    View.Frame(
                        margin = Thickness(10., 10., 10., 0.),
                        backgroundColor = Color.FromHex("#e1f5fe"),
                        hasShadow = true,
                        cornerRadius = 10.,
                        content = View.Grid(
                            rowdefs = [Dimension.Stars 0.4; Dimension.Stars 1.; Dimension.Stars 1.],
                            coldefs = [Dimension.Star; Dimension.Star],
                            children = [
                                View.Label(
                                    text = "WEIGHT (kgs)",
                                    fontAttributes = FontAttributes.Bold
                                ).Row(0).Column(0)
                                View.Label(
                                    text = "REPS",
                                    fontAttributes = FontAttributes.Bold
                                ).Row(0).Column(1)
                                match model.selectedIndexSet with 
                                | Some index ->
                                    View.Entry(
                                        text = (model.exercise.sets.[index].weight |> string),
                                        textChanged = updateWeight
                                    ).Row(1).Column(0)
                                    View.Entry(
                                        text = (model.exercise.sets.[index].reps |> string),
                                        textChanged = updateReps
                                    ).Row(1).Column(1)
                                    View.Button(
                                        text = "update",
                                        cornerRadius = 10,
                                        backgroundColor = Color.FromHex("#0069C0"),
                                        command = updateSet
                                    ).Row(2).Column(0).WhiteText()
                                    View.Button(
                                        text = "delete",
                                        cornerRadius = 10,
                                        backgroundColor = Color.FromHex("#0069C0"),
                                        command = deleteSet
                                    ).Row(2).Column(1).WhiteText()
                                | None -> 
                                    View.Entry(
                                        text = (model.newSet.weight |> string),
                                        textChanged = updateWeight
                                    ).Row(1).Column(0)
                                    View.Entry(
                                        text = (model.newSet.reps |> string),
                                        textChanged = updateReps
                                    ).Row(1).Column(1)
                                    View.Button(
                                        text = "add",
                                        cornerRadius = 10,
                                        backgroundColor = Color.FromHex("#0069C0"),
                                        command = addNewSet
                                    ).Row(2).Column(0).WhiteText()
                                    View.Button(
                                        text = "clear",
                                        cornerRadius = 10,
                                        backgroundColor = Color.FromHex("#0069C0")
                                    ).Row(2).Column(1).WhiteText()
                            ]
                        )
                    ).Row(1)
                    View.ListView(
                        separatorVisibility = SeparatorVisibility.None,
                        rowHeight = 90,
                        selectedItem = model.selectedIndexSet,
                        itemSelected = selectSet,
                        items = [
                            for set in model.exercise.sets do 
                                Component.viewCellSet set
                        ]
                    ).Row(2)
                ]
            )
        )
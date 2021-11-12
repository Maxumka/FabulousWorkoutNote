namespace FabulousWorkoutNote

open CustomControl
open Xamarin.Forms
open Fabulous.XamarinForms
open Fabulous

module ExercisePage = 
    type Model = 
        {exercise : Exercise
         newSet : Set
         selectedIndexSet : int option}

    let init exercise =
        {exercise = exercise;
         newSet = {Id = 0; Weight = 0; Reps = 0; ExerciseId = exercise.Id};
         selectedIndexSet = None}

    type Msg = 
        // http service msg
        | UpdateName of string
        | DoneUpdateName of unit

        | AddNewSet 
        | DoneAddSet of Set

        | DeleteExercise
        | DoneDeleteExercise of unit

        | UpdateSet
        | DoneUpdateSet of unit

        | DeleteSet
        | DoneDeleteSet of unit

        // other msg
        | UpdateWeight of string
        | UpdateReps of string
        | SelectSet of int option
        | ClearNewSet

    let updateSet (model : Model) = 
        match model.selectedIndexSet with 
        | None -> 
            model
        | Some index -> 
            let sets =
                model.exercise.Sets |> ListH.updateAt index model.newSet
            {model with exercise = {model.exercise with Sets = sets}}

    let deleteSet (model : Model) = 
        match model.selectedIndexSet with 
        | None -> model 
        | Some index -> 
            let sets = model.exercise.Sets |> ListH.removeAt index
            {model with exercise = {model.exercise with Sets = sets}}

    let updateSelectSet (model : Model) = 
        match model.selectedIndexSet with 
        | Some index -> 
            {model with newSet = model.exercise.Sets.[index]}
        | None -> model

    let update msg model =
        let updateName name =
            async {
                let exercise = {model.exercise with Name = name}
                do! HttpService.udpateExercise exercise
                return () |> DoneUpdateName } |> Cmd.ofAsyncMsg
         
        let addSet (set : Set) = 
            async {
                let! set = HttpService.addSet set
                return set |> DoneAddSet } |> Cmd.ofAsyncMsg

        let updateSet' (set : Set) = 
            async {
                do! HttpService.updateSet set 
                return () |> DoneUpdateSet } |> Cmd.ofAsyncMsg

        let deleteSet' id = 
            async {
                do! HttpService.deleteSet id 
                return () |> DoneDeleteSet } |> Cmd.ofAsyncMsg

        let deleteExercise' id = 
            async {
                do! HttpService.deleteExercise id
                return () |> DoneDeleteExercise } |> Cmd.ofAsyncMsg

        match msg with
        | UpdateName name -> 
            let model = {model with exercise = {model.exercise with Name = name}}
            model, updateName name, ExtMsg.NoOp
        | DoneUpdateName _ -> 
            model, Cmd.none, ExtMsg.NoOp
        
        | UpdateWeight weight -> 
            {model with newSet = {model.newSet with Weight = weight |> double}}, Cmd.none, ExtMsg.NoOp
        | UpdateReps reps -> 
            {model with newSet = {model.newSet with Reps = reps |> int}}, Cmd.none, ExtMsg.NoOp
        
        | AddNewSet -> 
            model, addSet model.newSet, ExtMsg.NoOp
        | DoneAddSet set -> 
            let sets = model.exercise.Sets @ [set]
            let exercise = {model.exercise with Sets = sets}
            {model with exercise = exercise}, Cmd.none, ExtMsg.NoOp

        | ClearNewSet -> 
            let newSet = {model.newSet with Weight = 0; Reps = 0}
            {model with newSet = newSet}, Cmd.none, ExtMsg.NoOp
        | SelectSet index -> 
            let newModel = {model with selectedIndexSet = index}
            updateSelectSet newModel, Cmd.none, ExtMsg.NoOp

        | UpdateSet -> 
            let newModel = updateSet model 
            match newModel.selectedIndexSet with 
            | Some i -> 
                let set = newModel.exercise.Sets.[i]
                {newModel with selectedIndexSet = None}, updateSet' set, ExtMsg.NoOp
            | None -> 
                {newModel with selectedIndexSet = None}, Cmd.none, ExtMsg.NoOp

        | DoneUpdateSet _ -> 
            model, Cmd.none, ExtMsg.NoOp

        | DeleteSet -> 
            match model.selectedIndexSet with 
            | Some i -> 
                let id = model.exercise.Sets.[i].Id
                let newModel = deleteSet model 
                {newModel with selectedIndexSet = None}, deleteSet' id, ExtMsg.NoOp
            | None -> 
                {model with selectedIndexSet = None}, Cmd.none, ExtMsg.NoOp

        | DeleteExercise -> 
            model, deleteExercise' model.exercise.Id, ExtMsg.NoOp
        | DoneDeleteExercise _ -> 
            model, Cmd.none, ExtMsg.NavWorkoutPageAfterDelete model.exercise.Id

        | _ -> model, Cmd.none, ExtMsg.NoOp

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
                                    text = model.exercise.Name,
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
                                        text = (model.exercise.Sets.[index].Weight |> string),
                                        textChanged = updateWeight
                                    ).Row(1).Column(0)
                                    View.Entry(
                                        text = (model.exercise.Sets.[index].Reps |> string),
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
                                        text = (model.newSet.Weight |> string),
                                        textChanged = updateWeight
                                    ).Row(1).Column(0)
                                    View.Entry(
                                        text = (model.newSet.Reps |> string),
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
                            for set in model.exercise.Sets do 
                                Component.viewCellSet set
                        ]
                    ).Row(2)
                ]
            )
        )
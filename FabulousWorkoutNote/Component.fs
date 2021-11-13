namespace FabulousWorkoutNote

open System
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open CustomControl

module Component = 
    let lineBlue = 
        View.BoxView(
            backgroundColor = Color.FromHex("#2195f2"), 
            horizontalOptions = LayoutOptions.FillAndExpand
         ).HeightRequest(0.5)

    let buttonAddExercise command =
        View.Button(cornerRadius = 30, text = "+",
                    verticalOptions = LayoutOptions.Center,
                    horizontalOptions = LayoutOptions.Center,
                    backgroundColor = Color.FromHex("#0069C0"),
                    command = command,
                    fontSize = FontSize.Size 28.).WidthRequest(60.).HeightRequest(60.).Row(1).Column(1).WhiteText()

    let frameSelectWorkoutDate children = 
        View.Frame(margin = Thickness(10., 10., 10., 0.), 
                   hasShadow = true, 
                   cornerRadius = 10., 
                   backgroundColor = Color.FromHex("#e1f5fe"), 
                   content = View.StackLayout(children = children)).Row(0)

    let gridExercises children = 
        View.Grid(rowdefs = [Dimension.Stars 1.; Dimension.Stars 0.3], 
                  coldefs = [Dimension.Stars 1.; Dimension.Stars 0.3],
                  backgroundColor = Color.FromHex("#ececec"), 
                  children = children)

    let viewCellExercise (exercise : Exercise) = 
        View.ViewCell(view = View.StackLayout(children = [
            View.Frame(cornerRadius = 10., 
                       margin = Thickness(30., 10., 30., 10.), 
                       hasShadow = true,
                       content = View.StackLayout(children = [
                View.Label(text = exercise.Name, 
                           margin = Thickness(5., 0., 0., 0.), 
                           fontSize = FontSize.Size 18.)
                View.BoxView(backgroundColor = Color.FromHex("#2195f2"), 
                             horizontalOptions = LayoutOptions.FillAndExpand).HeightRequest(0.5)
            ]))
        ]))

    let viewCellSet (set : Set) = 
        View.ViewCell(
            view = View.Frame(
                cornerRadius = 10.,
                margin = Thickness(10., 10., 10., 10.),
                hasShadow = true,
                content = View.Grid(
                    coldefs = [Dimension.Star; Dimension.Star],
                    children = [
                        View.Label(
                            text = $"{set.Weight} kgs",
                            fontSize = FontSize.Size 20.0,
                            horizontalOptions = LayoutOptions.Center,
                            verticalOptions = LayoutOptions.Center
                        ).Column(0)
                        View.Label(
                            text = $"{set.Reps} reps",
                            fontSize = FontSize.Size 20.0,
                            horizontalOptions = LayoutOptions.Center,
                            verticalOptions = LayoutOptions.Center
                        ).Column(1)
                    ]
                )
            )
        )


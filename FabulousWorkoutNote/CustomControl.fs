namespace FabulousWorkoutNote

open Xamarin.Forms
open Fabulous.XamarinForms
open Fabulous

module CustomControl = 
    type ViewElement with
        member this.WhiteText() = this.TextColor(Color.White)
        member this.ColorText(hex : string) = this.TextColor(Color.FromHex(hex))

    type EntryBehavior() =
        inherit Xamarin.Forms.Behavior<Entry>()
        let onTextChanged(sender: obj, args: TextChangedEventArgs) =
            let numbers = ['0'; '1'; '2'; '3'; '4'; '5'; '6'; '7'; '8'; '9']
            let compare c = numbers |> List.contains c 
            let isValid = args.NewTextValue |> String.forall compare
            match isValid with
            | true -> (sender :?> Entry).Text <- args.NewTextValue
            | false -> (sender :?> Entry).Text <- args.OldTextValue

        override this.OnAttachedTo(entry: Entry) =
            entry.TextChanged.AddHandler(fun x y -> onTextChanged(x,y))
            base.OnAttachedTo(entry)

        override this.OnDetachingFrom(entry: Entry) =
            entry.TextChanged.RemoveHandler(fun x y -> onTextChanged(x,y))
            base.OnDetachingFrom(entry)

    type Fabulous.XamarinForms.View with 
        static member inline EntryBehavior() = 
            let attribCount = 0
            let attribs = ViewBuilders.BuildView(attribCount) 

            let update (prevOpt: ViewElement voption) (source: ViewElement) (target: Xamarin.Forms.Behavior) = 
                ViewBuilders.UpdateBindableObject(prevOpt, source, target)
                
            let updateAttachedProperties propertyKey prevOpt source targetChild =
                ViewBuilders.UpdateBindableObjectAttachedProperties(propertyKey, prevOpt, source, targetChild)

            ViewElement.Create<EntryBehavior>(EntryBehavior, update, updateAttachedProperties, attribs)
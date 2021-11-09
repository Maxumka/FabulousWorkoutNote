namespace FabulousWorkoutNote

module ListH = 
    let updateAt index value source = 
        source |> List.mapi (fun i v -> if i = index then value else v)

    let rec removeAt index source = 
        match index, source with 
        | 0, _::xs -> xs
        | i, x::xs -> x::removeAt (i - 1) xs
        | i, [] -> failwith "index out of range in list"
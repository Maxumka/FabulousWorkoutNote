namespace FabulousWorkoutNote

open System
open FsHttp
open FsHttp.DslCE
open FSharp.Json

module HttpService = 

    let [<Literal>] private url = "http://10.0.2.2:5000"

    let private addWorkout (workout : Workout) = 
        http {
            POST $"{url}/addWorkout"
            body 
            json (workout |> Json.serialize)
        }
        |> Response.toText
        |> Json.deserialize<Result<int>>
        |> function
            | Ok id -> {workout with Id = id}
            | Fail msg -> workout

    let getWorkoutByDate (date : DateTime) = 
        let date' = date.ToString("f")
        let a = http {
            GET $"{url}/getWorkoutByDate/{date'}" 
        }
        let s = a |> Response.toText
        s
        |> Json.deserialize<Result<Workout>>
        |> function 
            | Ok workout -> workout
            | Fail _ -> addWorkout {Id = 0; Date = date; Exercises = []}

    let getSetsByExerciseId (id : int) = 
        async {
            let! response = 
                httpAsync {
                    GET $"{url}/getSetsByExerciseId/{id}"
                }
            return 
                response 
                |> Response.toText
                |> Json.deserialize<Result<Set list>>
                |> function 
                    | Ok sets -> sets 
                    | Fail _ -> []
        }

    let addExercise (exercise : Exercise) = 
        async {
            let! response = 
                httpAsync {
                    POST $"{url}/addExercise"
                    body 
                    json (exercise |> Json.serialize)
                }
            let result = 
                response |> Response.toText |> Json.deserialize<Result<int>>
                |> function 
                    | Ok id -> {exercise with Id = id}
                    | Fail msg -> {exercise with Name = msg}
            return result
        }

    let udpateExercise (exercise : Exercise) = 
        async {
            let! response = 
                httpAsync {
                    PUT $"{url}/updateExercise"
                    body 
                    json (exercise |> Json.serialize)
                } 
            response 
            |> Response.toText 
            |> ignore
        }    

    let deleteExercise (id : int) = 
        async {
            let! response = 
                httpAsync {
                    DELETE $"{url}/deleteExercise/{id}"
                }
            response 
            |> Response.toText
            |> ignore
        }

    let addSet (set : Set) = 
        async {
            let! response = 
                httpAsync {
                    POST $"{url}/addSet"
                    body
                    json (set |> Json.serialize)
                }
            return 
                response 
                |> Response.toText
                |> Json.deserialize<Result<int>>
                |> function
                    | Ok id -> {set with Id = id}
                    | Fail _ -> set
        }

    let updateSet (set : Set) = 
        async {
            let! response = 
                httpAsync {
                    PUT $"{url}/updateSet"
                    body
                    json (set |> Json.serialize)
                }
            response 
            |> Response.toText 
            |> ignore
        }

    let deleteSet (id : int) = 
        async {
            let! response = 
                httpAsync {
                    DELETE $"{url}/deleteSet/{id}"
                }
            response 
            |> Response.toText
            |> ignore
        }


